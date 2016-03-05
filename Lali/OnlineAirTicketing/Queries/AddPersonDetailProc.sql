CREATE PROCEDURE [dbo].[AddPerson]
      @Name varchar(100),
      @ContactNo varchar(50),
      @Address varchar(200),
      @Gender Char(10),
	  @PersonId int output
AS
BEGIN
      SET NOCOUNT ON;
      INSERT INTO  T_Person(Name, ContactNo, Address, Gender)
      VALUES (@Name, @ContactNo, @Address, @Gender)
      SET @PersonId=SCOPE_IDENTITY()
      RETURN  @PersonId
END