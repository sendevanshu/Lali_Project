using Models;
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
        private string connectionString;
        public AdminDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

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
                command.Parameters.AddWithValue("@passwrd", passwrd);
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

        public int AddFlight(FlightDetail flightDetail)
        {
            int flightId = 0;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
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
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                flightId = Convert.ToInt32(cmd.Parameters["@flightId"].Value);
                foreach (FlightLegDetail flightLeg in flightDetail.flightLegs)
                {
                    DateTime duration = DateTime.ParseExact(flightLeg.duration, "HH:mm",
                                        CultureInfo.InvariantCulture);
                    DateTime arrivalTime = DateTime.ParseExact(flightLeg.arrivalTime, "HH:mm",
                                        CultureInfo.InvariantCulture);
                    DateTime departTime = DateTime.ParseExact(flightLeg.departTime, "HH:mm",
                                        CultureInfo.InvariantCulture);
                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "AddFlightLeg";
                    cmd.Parameters.Add("@FlightLegNo", SqlDbType.Int).Value = flightLeg.flightlegNo;
                    cmd.Parameters.Add("@FlightId", SqlDbType.Int).Value = flightId;
                    cmd.Parameters.Add("@Duration", SqlDbType.Time).Value = duration.ToString("hh:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@ArrivalTime", SqlDbType.Time).Value = arrivalTime.ToString("hh:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@DepartTime", SqlDbType.Time).Value = departTime.ToString("hh:mm:ss", CultureInfo.CurrentCulture);
                    cmd.Parameters.Add("@DepartAirport", SqlDbType.Char).Value = flightLeg.departingAirport;
                    cmd.Parameters.Add("@ArrivalAirport", SqlDbType.Char).Value = flightLeg.arrivalAirport;
                    cmd.Parameters.Add("@BaseFare", SqlDbType.Float).Value = flightLeg.baseFare;
                    cmd.Parameters.Add("@Origin", SqlDbType.Char).Value = flightLeg.legOrigin;
                    cmd.Parameters.Add("@Destination", SqlDbType.Char).Value = flightLeg.legDestination;
                    cmd.Parameters.Add("@LastUpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@ActiveInd", SqlDbType.Bit).Value = true;
                    cmd.Parameters.Add("@flightLegId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Connection = con;
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
    }
}
