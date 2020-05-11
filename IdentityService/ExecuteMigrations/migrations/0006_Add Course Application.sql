IF OBJECT_ID('dbo.CourseApplication', 'U') IS NULL 
BEGIN 
	CREATE TABLE [CourseApplication](
		Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(400) NOT NULL,
        ShortDescription NVARCHAR(4000) NOT NULL,
		ApplicantId INT REFERENCES [User] (Id),
		CourseTypeId INT REFERENCES [CourseType] (Id),
        CreatedDate DATETIME NOT NULL,
        Active BIT NOT NULL DEFAULT 1,
        CONSTRAINT PK_Course_Application_Id PRIMARY KEY(Id)
	)
END
GO