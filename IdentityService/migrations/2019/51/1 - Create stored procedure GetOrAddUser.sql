CREATE PROCEDURE dbo.GetOrAddUser
@email nvarchar(256),
@role nvarchar(64)
AS
BEGIN
	DECLARE @exists NUMERIC

	SELECT @exists=COUNT(1) 
	FROM [dbo].[user_role]
	WHERE Email = @email AND Role=@role

	IF(@exists = 0) 
	BEGIN 
		INSERT INTO [dbo].[user_role] VALUES(@email, @role)
	END
END