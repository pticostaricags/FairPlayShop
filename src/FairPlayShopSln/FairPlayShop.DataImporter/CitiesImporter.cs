using CsvHelper;
using CsvHelper.Configuration;
using FairPlayShop.DataAccess.Data;
using FairPlayShop.DataAccess.Models;
using Humanizer;

namespace FairPlayShop.DataImporter
{
    public class CitiesImporter(ILogger<CitiesImporter> _logger, IConfiguration configuration, FairPlayShopDatabaseContext fairPlayShopDatabaseContext) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var importFileFullPath = configuration["ImportFileFullPath"];
                    using (Stream stream = File.Open(importFileFullPath!, mode: FileMode.Open))
                    {
                        using (StreamReader streamReader = new(stream))
                        {
                            using CsvParser csvParser = new(streamReader, configuration:
                                new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture));
                            using CsvReader csvReader = new(csvParser);
                            {
                                var records = csvReader.GetRecords<Models.WorldCityModel>().ToArray();
                                if (records != null)
                                {
                                    var countries = records
                                        .Select(p => p.country?.Titleize())
                                        .ToArray()
                                        .Distinct()
                                        .OrderBy(p => p);
                                    foreach (var singleCountry in countries)
                                    {
                                        _logger.LogInformation("Processing country {singleCountry}", singleCountry);
                                        var stateOrProvinces = records.Where(p => p.country == singleCountry)
                                            .Select(p => p.admin_name)
                                            .ToArray()
                                            .Distinct();
                                        Country countryEntity = new Country()
                                        {
                                            Name = singleCountry
                                        };
                                        foreach (var singlestateOrProvnce in stateOrProvinces)
                                        {
                                            _logger.LogInformation("Processing State/Province {singlestateOrProvnce}", singlestateOrProvnce);
                                            var cities = records
                                                .Where(p => p.country == singleCountry && p.admin_name == singlestateOrProvnce)
                                                .Select(p => p.city)
                                                .ToArray()
                                                .Distinct();
                                            StateOrProvince stateOrProvinceEntity = new StateOrProvince()
                                            {
                                                Name = singlestateOrProvnce
                                            };
                                            foreach (var singlecity in cities)
                                            {
                                                _logger.LogInformation("Processing City {singlecity}", singlecity);
                                                City cityEntity = new City()
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
                    _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
