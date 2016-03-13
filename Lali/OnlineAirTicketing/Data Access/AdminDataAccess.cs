using Models;
using Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    public class AdminDataAccess
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminDataAccess"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdminDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Validates the admin cred.
        /// </summary>
        /// <param name="usrname">The usrname.</param>
        /// <param name="passwrd">The password.</param>
        /// <returns><c>true</c> if user is valid, <c>false</c> otherwise.</returns>
        public bool validateAdminCred(string usrname, string passwrd)
        {
            bool returnVal = false;
            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT username, password from dbo.T_Admin "
                    + "WHERE password = @passwrd and username = @usrname ";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@passwrd", RSAAlgo.DecryptText(passwrd));
                command.Parameters.AddWithValue("@usrname", usrname);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        returnVal = true;
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return returnVal;
        }

        /// <summary>
        /// Adds the flight.
        /// </summary>
        /// <param name="flightDetail">The flight detail.</param>
        /// <returns>System.Int32.</returns>
        public int AddFlight(FlightDetail flightDetail)
        {
            int flightId = 0;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            
            try
            {
                //call the AddFight stored procedure and pass details of flight as parameters.
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddFlight";
                cmd.Parameters.Add("@FlightNo", SqlDbType.VarChar).Value = flightDetail.flightNumber;
                cmd.Parameters.Add("@Origin", SqlDbType.Char).Value = flightDetail.origin;
                cmd.Parameters.Add("@Destination", SqlDbType.Char).Value = flightDetail.destination;
                cmd.Parameters.Add("@NoOfLegs", SqlDbType.Int).Value = flightDetail.noOfLegs;
                cmd.Parameters.Add("@Distance", SqlDbType.Float).Value = flightDetail.distance;
                cmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@ActiveInd", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@flightId", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Connection = con;

                //open the connection
                con.Open();
                //execute the query
                cmd.ExecuteNonQuery();
                //retrieve the flight ID that is just saved in the database
                flightId = Convert.ToInt32(cmd.Parameters["@flightId"].Value);
                foreach (FlightLegDetail flightLeg in flightDetail.flightLegs)
                {
                    DateTime duration = DateTime.ParseExact(flightLeg.duration, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                    DateTime arrivalTime = DateTime.ParseExact(flightLeg.arrivalTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                    DateTime departTime = DateTime.ParseExact(flightLeg.departTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                    cmd = new SqlCommand();

                    //call AddFlightLeg stored procedure to add flight leg
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "AddFlightLeg";
                    cmd.Parameters.Add("@FlightLegNo", SqlDbType.Int).Value = flightLeg.flightlegNo;
                    cmd.Parameters.Add("@FlightId", SqlDbType.Int).Value = flightId;
                    cmd.Parameters.Add("@Duration", SqlDbType.Time).Value = duration.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@ArrivalTime", SqlDbType.Time).Value = arrivalTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@DepartTime", SqlDbType.Time).Value = departTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@DepartAirport", SqlDbType.Char).Value = flightLeg.departingAirport;
                    cmd.Parameters.Add("@ArrivalAirport", SqlDbType.Char).Value = flightLeg.arrivalAirport;
                    cmd.Parameters.Add("@BaseFare", SqlDbType.Float).Value = flightLeg.baseFare;
                    cmd.Parameters.Add("@Origin", SqlDbType.Char).Value = flightLeg.legOrigin;
                    cmd.Parameters.Add("@Destination", SqlDbType.Char).Value = flightLeg.legDestination;
                    cmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@ActiveInd", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@flightLegId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Connection = con;
                    //execute the query
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return flightId;
        }

        /// <summary>
        /// Gets the flights.
        /// </summary>
        /// <returns>List of FlightDetail</returns>
        public List<FlightDetail> GetFlights()
        {
            List<FlightDetail> flightList = new List<FlightDetail>();
            // Provide the query string to get flight details that are active
            string queryString =
                "SELECT FlightID, FlightNo, Origin, Destination, NoOfLegs, Distance"
                + " from dbo.T_Flight"
                + " where ActiveInd = 'true'";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //retrieve all the flight details
                        FlightDetail flight = new FlightDetail();
                        flight.flightID = Convert.ToString(reader[0]);
                        flight.flightNumber = Convert.ToString(reader[1]);
                        flight.origin = Convert.ToString(reader[2]);
                        flight.destination = Convert.ToString(reader[3]);
                        flight.distance = (float)Convert.ToDouble(reader[5]);
                        flight.noOfLegs = Convert.ToInt32(reader[4]);
                        flightList.Add(flight);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return flightList;
        }


        /// <summary>
        /// Gets the flight legs.
        /// </summary>
        /// <param name="flightID">The flight identifier.</param>
        /// <returns>List of FlightLegDetail.</returns>
        public List<FlightLegDetail> GetFlightLegs(int flightID)
        {
            List<FlightLegDetail> flightLegList = new List<FlightLegDetail>();
            // Provide the query string to get flight leg details with flightId as place holder
            string queryString =
                "SELECT FlightLegID, FlightLegNo, Duration, ArrivalTime, DepartTime, DepartAirport,"
                + "ArrivalAirport, BaseFare, Origin, Destination from dbo.T_FlightLeg"
                + " where ActiveInd = 'true' and FlightID = @flightID";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@flightID", flightID);
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //retrieve flight leg details
                        FlightLegDetail flightLeg = new FlightLegDetail();
                        flightLeg.flightLegID = Convert.ToString(reader[0]);
                        flightLeg.flightlegNo = Convert.ToString(reader[1]);
                        flightLeg.duration = Convert.ToString(reader[2]);
                        flightLeg.arrivalTime = Convert.ToString(reader[3]);
                        flightLeg.departTime = Convert.ToString(reader[4]);
                        flightLeg.departingAirport = Convert.ToString(reader[5]);
                        flightLeg.arrivalAirport = Convert.ToString(reader[6]);
                        flightLeg.baseFare = (float)Convert.ToDouble(reader[7]);
                        flightLeg.legOrigin = Convert.ToString(reader[8]);
                        flightLeg.legDestination = Convert.ToString(reader[9]);
                        flightLegList.Add(flightLeg);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return flightLegList;
        }

        /// <summary>
        /// Deletes the flight.
        /// </summary>
        /// <param name="flightID">The flight identifier.</param>
        /// <returns><c>true</c> if flight deleted successfully, <c>false</c> otherwise.</returns>
        public bool DeleteFlight(int flightID)
        {
            bool result = false;
            // Provide the query string with a parameter placeholder for flight ID to be deleted.
            string queryString =
                "update T_Flight set ActiveInd = 'false' where FlightID = @flightID";


            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@flightID", flightID);
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    queryString = "update T_FlightLeg set ActiveInd = 'false' where FlightID = @flightID";

                    command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@flightID", flightID);
                    //execute the  query
                    command.ExecuteNonQuery();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return result;
        }

        /// <summary>
        /// Modifies the flight.
        /// </summary>
        /// <param name="flightID">The flight identifier.</param>
        /// <param name="departedFlightLegID">The departed flight leg identifier.</param>
        /// <param name="departTime">The depart time.</param>
        /// <param name="arrivalFlightLegID">The arrival flight leg identifier.</param>
        /// <param name="arrivalTime">The arrival time.</param>
        /// <returns><c>true</c> if flight successfully modified, <c>false</c> otherwise.</returns>
        public bool ModifyFlight(string flightID, string departedFlightLegID, string departTime, string arrivalFlightLegID, string arrivalTime)
        {
            bool result = false;
            

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {

                
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    DateTime tempArrivalTime = DateTime.ParseExact(arrivalTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
                    DateTime tempDepartTime = DateTime.ParseExact(departTime, "HH:mm:ss",
                                                CultureInfo.InvariantCulture);
                    // Provide the query string with a parameter placeholder for the flight and flight Leg ID to be updated.
                    string queryString =
                        "update T_FlightLeg set DepartTime = @departTime where FlightID = @flightID and FlightLegID = @flightLegId";

                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@departTime", tempDepartTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture));
                    command.Parameters.AddWithValue("@flightID", flightID);
                    command.Parameters.AddWithValue("@flightLegId", departedFlightLegID);
                    //open the connection and execute the query
                    connection.Open();
                    command.ExecuteNonQuery();
                    queryString = "update T_FlightLeg set ArrivalTime = @arrivalTime where FlightID = @flightID and FlightLegID = @flightLegId";

                    command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@arrivalTime", tempArrivalTime.ToString("HH:mm:ss", CultureInfo.CurrentCulture));
                    command.Parameters.AddWithValue("@flightID", flightID);
                    command.Parameters.AddWithValue("@flightLegId", arrivalFlightLegID);
                    command.ExecuteNonQuery();
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return result;
            }
        }
    }
}
