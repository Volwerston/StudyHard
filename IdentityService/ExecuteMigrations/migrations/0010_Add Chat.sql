IF OBJECT_ID('dbo.Chat', 'U') IS NULL 
BEGIN 
	CREATE TABLE [Chat](
		Id INT IDENTITY(1,1) NOT NULL,
		UserId1 INT REFERENCES [User] (Id),
		UserId2 INT REFERENCES [User] (Id),
        CONSTRAINT PK_Chat_Id PRIMARY KEY(Id),
        CONSTRAINT UK_Chat_Users_Id UNIQUE (UserId1, UserId2)
	)
END
GO

IF OBJECT_ID('dbo.Message', 'U') IS NULL 
BEGIN 
	CREATE TABLE [Message](
		Id INT IDENTITY(1,1) NOT NULL,
        Content NVARCHAR(4000) NOT NULL,
        SentDateTime DATETIME NOT NULL,
		ChatId INT REFERENCES [Chat] (Id),
		SentBy INT REFERENCES [User] (Id),
        CONSTRAINT PK_Message_Id PRIMARY KEY(Id)
	)
END
GO