SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddFlight]
      @FlightNo varchar(50),
      @Origin Char(20),
      @Destination Char(20),
      @NoOfLegs int,
      @Distance float,
	  @LastUpdateDate datetime,
	  @ActiveInd bit,
      @flightId int output
AS
BEGIN
      SET NOCOUNT ON;
      INSERT INTO  T_Flight(FlightNo, Origin, Destination, NoOfLegs, Distance, LastUpdateDate, ActiveInd)
      VALUES (@FlightNo, @Origin, @Destination, @NoOfLegs, @Distance, @LastUpdateDate, @ActiveInd)
      SET @flightId=SCOPE_IDENTITY()
      RETURN  @flightId
END