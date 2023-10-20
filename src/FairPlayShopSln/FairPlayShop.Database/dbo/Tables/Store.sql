CREATE TABLE [dbo].[Store]
(
	[StoreId] BIGINT NOT NULL CONSTRAINT PK_Store PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [OwnerId] NVARCHAR(450) NOT NULL, 
    CONSTRAINT [FK_Store_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id])
)

GO

CREATE UNIQUE INDEX [UI_Store_Name] ON [dbo].[Store] ([Name])
