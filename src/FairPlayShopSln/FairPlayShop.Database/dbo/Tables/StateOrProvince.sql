CREATE TABLE [dbo].[StateOrProvince]
(
	[StateOrProvinceId] INT NOT NULL CONSTRAINT PK_StateOrProvince PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [CountryId] INT NOT NULL, 
    CONSTRAINT [FK_StateOrProvince_Country] FOREIGN KEY ([CountryId]) REFERENCES [Country]([CountryId])
)
