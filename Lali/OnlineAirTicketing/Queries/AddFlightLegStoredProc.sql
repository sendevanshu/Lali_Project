SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddFlightLeg]
      @FlightLegNo int,
	  @FlightId int,
      @Duration time(7),
	  @ArrivalTime time(7),
	  @DepartTime time(7),
	  @DepartAirport Char(50),
	  @ArrivalAirport Char(50),
	  @BaseFare float,
	  @Origin Char(20),
      @Destination Char(20),
	  @LastUpdateDate datetime,
	  @ActiveInd bit,
      @flightLegId int output
AS
BEGIN
      SET NOCOUNT ON;
      INSERT INTO  T_FlightLeg(FlightLegNo, FlightID, Duration, ArrivalTime, DepartTime, DepartAirport, ArrivalAirport, BaseFare, Origin, Destination, LastUpdatedDate, ActiveInd)
      VALUES (@FlightLegNo, @FlightId, @Duration, @ArrivalTime, @DepartTime, @DepartAirport, @ArrivalAirport, @BaseFare, @Origin, @Destination, @LastUpdateDate, @ActiveInd)
      SET @flightLegId=SCOPE_IDENTITY()
      RETURN  @flightLegId
END


