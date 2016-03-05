CREATE PROCEDURE [dbo].[AddUser]
      @UserName varchar(50),
      @Password varchar(50),
      @PersonID int,
      @RegisterDateTime DateTime,
	  @lastUpdatedDateTime DateTime,
	  @activeInd bit,
	  @securityQues varchar(200),
	  @securityAns varchar(200),
	  @UserID int output
AS
BEGIN
       SET NOCOUNT ON;
      INSERT INTO  T_User(UserName, Password, PersonID, RegisterDateTime, lastUpdatedDateTime, activeInd, securityQues, securityAns)
      VALUES (@UserName, @Password, @PersonID, @RegisterDateTime, @lastUpdatedDateTime, @activeInd, @securityQues,@securityAns)
      SET @UserID=SCOPE_IDENTITY()
      RETURN  @UserID
END