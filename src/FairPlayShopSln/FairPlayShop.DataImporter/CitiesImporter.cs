using CsvHelper;
using CsvHelper.Configuration;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace FairPlayShop.DataImporter
{
    public class CitiesImporter(ILogger<CitiesImporter> _logger, FairPlayShopDatabaseContext fairPlayShopDatabaseContext) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    if (!await fairPlayShopDatabaseContext!.City.AnyAsync(cancellationToken: stoppingToken))
                    {
                        using HttpClient httpClient = new();
                        const string link = "https://simplemaps.com/static/data/world-cities/basic/simplemaps_worldcities_basicv1.76.zip";
                        var zipFileData = await httpClient.GetStreamAsync(link, cancellationToken: stoppingToken);
                        using ZipArchive zipArchive = new ZipArchive(zipFileData);
                        var csvFileEntry = zipArchive.GetEntry("worldcities.csv");
                        using (Stream stream = csvFileEntry!.Open())
                        {
                            using StreamReader streamReader = new(stream);
                            using CsvParser csvParser = new(streamReader, configuration:
                                new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                                {
                                    PrepareHeaderForMatch = args => args.Header.ToLower()
                                });
                            using CsvReader csvReader = new(csvParser);
                            await ImportDataAsync(_logger, csvReader);
                        }
                        _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
                    }
                    else
                    {
                        _logger.LogInformation("Data is already in the database. No action taken");
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ImportDataAsync(ILogger<CitiesImporter> _logger, CsvReader csvReader)
        {
            var records = csvReader.GetRecords<Models.WorldCityModel>().ToArray();
            if (records!.Length > 0)
            {
                var countries = records
                    .Select(p => p.Country?.Titleize())
                    .Distinct()
                    .OrderBy(p => p);
                foreach (var singleCountry in countries)
                {
                    _logger.LogInformation("Processing country {singleCountry}", singleCountry);
                    var stateOrProvinces = records.Where(p => p.Country == singleCountry)
                        .Select(p => p.Admin_name)
                        .Distinct();
                    Country countryEntity = new()
                    {
                        Name = singleCountry
                    };
                    foreach (var singlestateOrProvnce in stateOrProvinces)
                    {
                        _logger.LogInformation("Processing State/Province {singlestateOrProvnce}", singlestateOrProvnce);
                        var cities = records
                            .Where(p => p.Country == singleCountry && p.Admin_name == singlestateOrProvnce)
                            .Select(p => p.City)
                            .Distinct();
                        StateOrProvince stateOrProvinceEntity = new()
                        {
                            Name = singlestateOrProvnce
                        };
                        foreach (var singlecity in cities)
                        {
                            _logger.LogInformation("Processing City {singlecity}", singlecity);
                            City cityEntity = new()
                            {
                                Name = singlecity
                            };
                            stateOrProvinceEntity.City.Add(cityEntity);
                        }
                        countryEntity.StateOrProvince.Add(stateOrProvinceEntity);
                    }
                    await fairPlayShopDatabaseContext.AddAsync(countryEntity);
                }
                await fairPlayShopDatabaseContext.SaveChangesAsync();
            }
        }
    }
}
