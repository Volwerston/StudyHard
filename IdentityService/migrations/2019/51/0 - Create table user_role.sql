CREATE TABLE [dbo].[user_role]
(
	Email nvarchar(256) not null,
	Role nvarchar(64) not null,
	Constraint PK_Email_Role PRIMARY KEY(Email, Role)
)