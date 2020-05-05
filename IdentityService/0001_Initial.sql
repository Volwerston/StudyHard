--SETTING ESSENTIAL TABLES
IF OBJECT_ID('dbo.User', 'U') IS NULL 
BEGIN 
	CREATE TABLE [User](
		Id INT IDENTITY(1,1) NOT NULL,
		Email NVARCHAR(100) UNIQUE NOT NULL,
		Name NVARCHAR(200) NOT NULL,
		Gender INT NULL,
		BirthDate DATETIME,
		CONSTRAINT PK_User_Id PRIMARY KEY(Id)
	)
END
IF OBJECT_ID('dbo.Role', 'U') IS NULL 
BEGIN 
	CREATE TABLE [Role](
		Id INT IDENTITY(1,1) NOT NULL,
		Name NVARCHAR(100) NOT NULL UNIQUE,
		CONSTRAINT PK_Role_Id PRIMARY KEY(Id)
	)
END
IF OBJECT_ID('dbo.Course', 'U') IS NULL 
BEGIN 
	CREATE TABLE [Course](
		Id INT IDENTITY(1,1) NOT NULL,
		Name NVARCHAR(400) NOT NULL UNIQUE,
		Description NVARCHAR(MAX) NULL,
		CreatedDate DATETIME NOT NULL,
		Active BIT NOT NULL DEFAULT 1,
		CONSTRAINT PK_Course_Id PRIMARY KEY(Id)
	)
END
GO

