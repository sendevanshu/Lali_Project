using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UserDataAccess
    {
        private string connectionString;
        public UserDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public bool validateUserInfo(string username, int userID)
        {
            bool returnVal = false;
            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT username, userId from dbo.T_User "
                    + "WHERE userId = @usrId and username = @usrname ";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@usrId", userID);
                command.Parameters.AddWithValue("@usrname", username);

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

    }
}

