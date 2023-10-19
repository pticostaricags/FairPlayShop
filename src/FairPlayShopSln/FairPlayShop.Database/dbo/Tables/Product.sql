CREATE TABLE [dbo].[Product]
(
	[ProductId] BIGINT NOT NULL CONSTRAINT PK_Product PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(500) NOT NULL,
    [QuantityInStock] INT NOT NULL, 
    [ProductStatusId] TINYINT NOT NULL, 
    [OwnerId] NVARCHAR(450) NOT NULL, 
    CONSTRAINT [FK_Product_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_Product_ProductStatus] FOREIGN KEY ([ProductStatusId]) REFERENCES [ProductStatus]([ProductStatusId])
)

GO

CREATE UNIQUE INDEX [UI_Product_Name] ON [dbo].[Product] ([Name],[OwnerId])
