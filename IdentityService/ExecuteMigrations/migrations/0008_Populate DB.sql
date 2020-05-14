﻿-- Roles

INSERT INTO [dbo].[Role] VALUES('Tutor')

--User

INSERT INTO [dbo].[User] VALUES('name1@gmail.com', 'John Doe', 1, '1970-06-12')
INSERT INTO [dbo].[User] VALUES('name2@gmail.com', 'Marie Stewart', 0, '1983-03-21')
INSERT INTO [dbo].[User] VALUES('name3@gmail.com', 'William Kurt', 1, '1983-01-09')
INSERT INTO [dbo].[User] VALUES('name4@gmail.com', 'Sannie Thorsen', 0, '1972-05-15')
INSERT INTO [dbo].[User] VALUES('name5@gmail.com', 'Mattew Ritz', 1, '1959-06-03')
INSERT INTO [dbo].[User] VALUES('name6@gmail.com', 'Nancy Johnson', 0, '1963-04-09')
INSERT INTO [dbo].[User] VALUES('name7@gmail.com', 'Ole Monred', 1, '1981-07-27')
INSERT INTO [dbo].[User] VALUES('name8@gmail.com', 'Ann Baylish', 0, '1972-02-24')
INSERT INTO [dbo].[User] VALUES('name9@gmail.com', 'Jeremy Bennings', 1, '1967-08-08')
INSERT INTO [dbo].[User] VALUES('name10@gmail.com', 'Kate Blagden', 0, '1989-10-13')

-- UserRole

INSERT INTO [dbo].[UserRoles] VALUES(1, 1)
INSERT INTO [dbo].[UserRoles] VALUES(2, 1)
INSERT INTO [dbo].[UserRoles] VALUES(3, 1)
INSERT INTO [dbo].[UserRoles] VALUES(4, 1)
INSERT INTO [dbo].[UserRoles] VALUES(5, 1)
INSERT INTO [dbo].[UserRoles] VALUES(6, 1)
INSERT INTO [dbo].[UserRoles] VALUES(7, 1)
INSERT INTO [dbo].[UserRoles] VALUES(8, 1)
INSERT INTO [dbo].[UserRoles] VALUES(9, 1)
INSERT INTO [dbo].[UserRoles] VALUES(10, 1)
INSERT INTO [dbo].[UserRoles] VALUES(1, 2)
INSERT INTO [dbo].[UserRoles] VALUES(2, 2)
INSERT INTO [dbo].[UserRoles] VALUES(3, 2)
INSERT INTO [dbo].[UserRoles] VALUES(6, 2)
INSERT INTO [dbo].[UserRoles] VALUES(7, 2)
INSERT INTO [dbo].[UserRoles] VALUES(9, 2)
INSERT INTO [dbo].[UserRoles] VALUES(10, 2)

-- UserSkills

INSERT INTO [dbo].[UserSkills] VALUES(1, 1)
INSERT INTO [dbo].[UserSkills] VALUES(1, 2)
INSERT INTO [dbo].[UserSkills] VALUES(1, 3)
INSERT INTO [dbo].[UserSkills] VALUES(1, 4)
INSERT INTO [dbo].[UserSkills] VALUES(1, 5)
INSERT INTO [dbo].[UserSkills] VALUES(2, 1)
INSERT INTO [dbo].[UserSkills] VALUES(2, 2)
INSERT INTO [dbo].[UserSkills] VALUES(2, 6)
INSERT INTO [dbo].[UserSkills] VALUES(3, 3)
INSERT INTO [dbo].[UserSkills] VALUES(3, 4)
INSERT INTO [dbo].[UserSkills] VALUES(3, 5)
INSERT INTO [dbo].[UserSkills] VALUES(6, 5)
INSERT INTO [dbo].[UserSkills] VALUES(6, 6)
INSERT INTO [dbo].[UserSkills] VALUES(7, 5)
INSERT INTO [dbo].[UserSkills] VALUES(7, 1)
INSERT INTO [dbo].[UserSkills] VALUES(7, 2)
INSERT INTO [dbo].[UserSkills] VALUES(10, 1)
INSERT INTO [dbo].[UserSkills] VALUES(10, 4)
INSERT INTO [dbo].[UserSkills] VALUES(10, 5)
INSERT INTO [dbo].[UserSkills] VALUES(10, 6)
INSERT INTO [dbo].[UserSkills] VALUES(2, 3)
INSERT INTO [dbo].[UserSkills] VALUES(2, 5)
INSERT INTO [dbo].[UserSkills] VALUES(6, 1)
INSERT INTO [dbo].[UserSkills] VALUES(6, 2)
INSERT INTO [dbo].[UserSkills] VALUES(7, 3)
INSERT INTO [dbo].[UserSkills] VALUES(7, 6)
INSERT INTO [dbo].[UserSkills] VALUES(10, 3)
INSERT INTO [dbo].[UserSkills] VALUES(9, 1)
INSERT INTO [dbo].[UserSkills] VALUES(9, 4)
INSERT INTO [dbo].[UserSkills] VALUES(9, 5)
INSERT INTO [dbo].[UserSkills] VALUES(9, 6)
INSERT INTO [dbo].[UserSkills] VALUES(9, 3)