CREATE OR ALTER PROCEDURE dbo.UpdateUser
@UserId INT,
@Name NVARCHAR(MAX),
@Gender INT,
@BirthDate DATETIME
AS
BEGIN
UPDATE [User] SET 
	Name = @Name, Gender = @Gender, BirthDate = @BirthDate
	WHERE Id = @UserId
END