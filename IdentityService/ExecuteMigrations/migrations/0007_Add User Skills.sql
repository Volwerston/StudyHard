IF OBJECT_ID('dbo.UserSkills', 'U') IS NULL 
BEGIN 
	CREATE TABLE [UserSkills](
		UserId INT REFERENCES [dbo].[User](Id),
		SkillId INT REFERENCES [dbo].[CourseType](Id),
		CONSTRAINT PK_UserId_SkillId PRIMARY KEY(UserId, SkillId))
END
GO