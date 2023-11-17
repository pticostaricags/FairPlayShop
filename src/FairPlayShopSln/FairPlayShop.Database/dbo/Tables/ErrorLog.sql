CREATE TABLE [dbo].[ErrorLog]
(
	[ErrorLogId] BIGINT NOT NULL CONSTRAINT PK_ErrorLog PRIMARY KEY IDENTITY, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [StackTrace] NVARCHAR(MAX) NOT NULL, 
    [FullException] NVARCHAR(MAX) NOT NULL,
    [RowCreationDateTime] DATETIMEOFFSET NOT NULL, 
    [SourceApplication] NVARCHAR(250) NOT NULL 
)