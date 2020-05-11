IF OBJECT_ID('dbo.CourseType', 'U') IS NULL 
BEGIN 
	CREATE TABLE [CourseType](
		Id INT IDENTITY(1,1) NOT NULL,
		Type NVARCHAR(400) NOT NULL UNIQUE,
        CONSTRAINT PK_Course_Type_Id PRIMARY KEY(Id)
	)
END
GO

IF OBJECT_ID('dbo.Course', 'U') IS NOT NULL
    BEGIN
        ALTER TABLE [Course] ADD CourseTypeId INT REFERENCES [CourseType] (Id)
    END
GO

INSERT INTO [CourseType] (Type) VALUES ('Math');
INSERT INTO [CourseType] (Type) VALUES ('Physics');
INSERT INTO [CourseType] (Type) VALUES ('Biology');
INSERT INTO [CourseType] (Type) VALUES ('Chemistry');
INSERT INTO [CourseType] (Type) VALUES ('Literature');
INSERT INTO [CourseType] (Type) VALUES ('Programming');