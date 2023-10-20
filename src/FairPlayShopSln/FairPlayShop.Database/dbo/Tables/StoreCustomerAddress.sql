CREATE TABLE [dbo].[StoreCustomerAddress]
(
	[StoreCustomerAddressId] BIGINT NOT NULL CONSTRAINT PK_StoreCustomerAddress PRIMARY KEY IDENTITY, 
    [CountryId] INT NOT NULL, 
    [Firstname] NVARCHAR(50) NOT NULL, 
    [Lastname] NVARCHAR(50) NOT NULL, 
    [Company] NVARCHAR(50) NULL, 
    [AddressLine1] NVARCHAR(50) NOT NULL, 
    [AddressLine2] NVARCHAR(50) NOT NULL, 
    [CityId] INT NOT NULL, 
    [PostalCode] NVARCHAR(10) NOT NULL, 
    [PhoneNumber] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_StoreCustomerAddress_City] FOREIGN KEY ([CityId]) REFERENCES [City]([CityId]) 
)
