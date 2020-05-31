ALTER TABLE [dbo].[Course]
ADD 
CustomerId INT FOREIGN KEY REFERENCES [dbo].[User](Id),
TutorId INT FOREIGN KEY REFERENCES [dbo].[User](Id),
CourseApplicationId INT FOREIGN KEY REFERENCES [dbo].[CourseApplication](Id)