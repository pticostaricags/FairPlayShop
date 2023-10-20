CREATE TABLE [dbo].[Country]
(
	[CountryId] INT NOT NULL CONSTRAINT PK_Country PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL
)

GO

CREATE UNIQUE INDEX [uI_Country_Name] ON [dbo].[Country] ([Name])
