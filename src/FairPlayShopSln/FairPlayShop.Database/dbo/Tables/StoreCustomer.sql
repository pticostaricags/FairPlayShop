CREATE TABLE [dbo].[StoreCustomer]
(
	[StoreCustomerId] BIGINT NOT NULL CONSTRAINT PK_StoreCustomer PRIMARY KEY IDENTITY, 
    [StoreId] BIGINT NOT NULL, 
    [Firstname] NVARCHAR(50) NOT NULL, 
    [Lastname] NVARCHAR(50) NOT NULL, 
    [Surname] NVARCHAR(50) NOT NULL, 
    [EmailAddress] NVARCHAR(50) NOT NULL, 
    [PhoneNumber] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_StoreCustomer_Store] FOREIGN KEY ([StoreId]) REFERENCES [Store]([StoreId])
)
