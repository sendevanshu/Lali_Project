using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AddFlight";
            cmd.Parameters.Add("@FlightNo", SqlDbType.VarChar).Value = txtFirstName.Text.Trim();
            cmd.Parameters.Add("@Origin", SqlDbType.VarChar).Value = txtLastName.Text.Trim();
            cmd.Parameters.Add("@Destination", SqlDbType.DateTime).Value = txtBirthDate.Text.Trim();
            cmd.Parameters.Add("@NoOfLegs", SqlDbType.VarChar).Value = txtCity.Text.Trim();
            cmd.Parameters.Add("@Distance", SqlDbType.VarChar).Value = txtCountry.Text.Trim();
            cmd.Parameters.Add("@LastUpdateDate", SqlDbType.VarChar).Value = txtCountry.Text.Trim();
            cmd.Parameters.Add("@ActiveInd", SqlDbType.VarChar).Value = txtCountry.Text.Trim();
            cmd.Parameters.Add("@flightId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                string id = cmd.Parameters["@flightId"].Value.ToString();
                lblMessage.Text = "Record inserted successfully. ID = " + id;
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
            throw new NotImplementedException();
        }
    }
}
