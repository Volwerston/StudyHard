IF COL_LENGTH('dbo.User', 'PictureUrl') IS NULL
BEGIN
	ALTER TABLE [dbo].[User] ADD PictureUrl NVARCHAR(MAX) NULL
	ALTER TABLE [dbo].[User] ADD CONSTRAINT df_User_PictureUrl
		DEFAULT 'https://lh3.googleusercontent.com/-XdUIqdMkCWA/AAAAAAAAAAI/AAAAAAAAAAA/4252rscbv5M/photo.jpg'
		FOR PictureUrl
END

GO 

IF COL_LENGTH('dbo.User', 'PictureUrl') IS NOT NULL
BEGIN
	UPDATE [dbo].[User] SET PictureUrl = 'https://lh3.googleusercontent.com/-XdUIqdMkCWA/AAAAAAAAAAI/AAAAAAAAAAA/4252rscbv5M/photo.jpg'
	WHERE PictureUrl IS NULL
END
GO
CREATE OR ALTER PROCEDURE dbo.GetOrAddUser
@email nvarchar(256),
@name nvarchar(256),
@pictureUrl nvarchar(max)
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
			INSERT INTO [dbo].[User] (Email, Name, Gender, BirthDate, PictureUrl) VALUES(@email, @name, null, null, @pictureUrl)
			SELECT @userId=IDENT_CURRENT('dbo.User')
		END
		ELSE
		BEGIN
			UPDATE [dbo].[User] SET PictureUrl = @pictureUrl
				WHERE Email = @email
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