﻿IF OBJECT_ID('dbo.Blog', 'U') IS NULL 
BEGIN 
	CREATE TABLE [dbo].[Blog](
		Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        Title NVARCHAR(256) NOT NULL,
		Text NVARCHAR(4000) NOT NULL,
		CreationDateTimeUtc DATETIME2 NOT NULL,
		UserId INT NOT NULL FOREIGN KEY REFERENCES [dbo].[User](Id)
	)
END
GO