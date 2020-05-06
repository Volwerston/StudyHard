DROP TABLE IF EXISTS [dbo].[user_role]
GO

IF OBJECT_ID('dbo.UserRoles', 'U') IS NULL 
BEGIN 
CREATE TABLE [dbo].[UserRoles]
(
	UserId int not null FOREIGN KEY REFERENCES [User](Id),
	RoleId int not null FOREIGN KEY REFERENCES Role(Id),
	Constraint PK_UserId_RoleId PRIMARY KEY(UserId, RoleId)
)
END
GO

INSERT INTO Role (Name) VALUES ('Regular')
GO

CREATE OR ALTER PROCEDURE dbo.GetOrAddUser
@email nvarchar(256),
@name nvarchar(256)
AS
BEGIN
	DECLARE @exists NUMERIC
	DECLARE @userId NUMERIC = -1
	DECLARE @regularRoleId NVARCHAR(100)

	BEGIN TRAN;

	BEGIN TRY
		SELECT @exists=COUNT(1) 
		FROM [dbo].[User]
		WHERE Email = @email

		SELECT @regularRoleId=MIN(Id) FROM [dbo].[Role] WHERE Name='Regular'

		IF(@exists = 0) 
		BEGIN 
			INSERT INTO [dbo].[User] VALUES(@email, @name, null, null)
			SELECT @userId=IDENT_CURRENT('dbo.User')
		END

		IF(@userId <> -1)
		BEGIN
			INSERT INTO [dbo].[UserRoles] VALUES(@userId, @regularRoleId)
		END

		COMMIT TRAN;
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
	END CATCH
END