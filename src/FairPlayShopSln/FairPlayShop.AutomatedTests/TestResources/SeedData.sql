﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS
(
   SELECT * FROM ProductStatus WHERE ProductStatusId = 1
)
BEGIN
    SET IDENTITY_INSERT [ProductStatus] ON
    INSERT INTO ProductStatus(ProductStatusId, [Name]) VALUES(1,'Draft')
    SET IDENTITY_INSERT [ProductStatus] OFF
END
IF NOT EXISTS
(
   SELECT * FROM ProductStatus WHERE ProductStatusId = 2
)
BEGIN
    SET IDENTITY_INSERT [ProductStatus] ON
    INSERT INTO ProductStatus(ProductStatusId, [Name]) VALUES(2,'Active')
    SET IDENTITY_INSERT [ProductStatus] OFF
END