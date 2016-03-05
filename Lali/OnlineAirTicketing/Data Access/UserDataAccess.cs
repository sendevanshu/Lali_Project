using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
        public int validateUserInfo(string username, string password)
        {
            int userID = 0;
            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT userID from dbo.T_User "
                    + "WHERE password = @password and username = @usrname ";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@password", password);
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
                        userID = Convert.ToInt32(reader[0]); 
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
            return userID;
        }

        public int addUser(UserDetail userDetail)
        {
            int userID = 0;
            int personID = 0;
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddPerson";
                cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = userDetail.personName;
                cmd.Parameters.Add("@ContactNo", SqlDbType.VarChar).Value = userDetail.contactNumber;
                cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = userDetail.address;
                cmd.Parameters.Add("@Gender", SqlDbType.Char).Value = userDetail.gender;
                cmd.Parameters.Add("@PersonId", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Connection = con;

                con.Open();
                cmd.ExecuteNonQuery();
                personID = Convert.ToInt32(cmd.Parameters["@PersonId"].Value);
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddUser";
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userDetail.username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = userDetail.password;
                cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = personID;
                cmd.Parameters.Add("@RegisterDateTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@lastUpdatedDateTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@activeInd", SqlDbType.Bit).Value = true;
                cmd.Parameters.Add("@securityQues", SqlDbType.VarChar).Value = userDetail.securityQues;
                cmd.Parameters.Add("@securityAns", SqlDbType.VarChar).Value = userDetail.securityAns;
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
                userID = Convert.ToInt32(cmd.Parameters["@UserID"].Value);
                
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
            return userID;
        }

        public UserDetail getUserInfo(int userID)
        {
            UserDetail userDetail = null;
            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT username, password, personID from dbo.T_User "
                    + "WHERE userID = @userID and activeInd = 'true' ";

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@userID", userID);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userDetail = new UserDetail();
                        userDetail.userID = userID;
                        userDetail.username = Convert.ToString(reader[0]);
                        userDetail.password = Convert.ToString(reader[1]);
                        userDetail.personID = Convert.ToInt32(reader[2]);
                    }
                    reader.Close();
                    queryString = "SELECT name, contactNo, address, gender from dbo.T_Person "
                    + "WHERE personID = @personID";
                    // Create the Command and Parameter objects.
                    command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@personID", userDetail.personID);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userDetail.personName = Convert.ToString(reader[0]);
                        userDetail.address = Convert.ToString(reader[2]);
                        userDetail.contactNumber = Convert.ToString(reader[1]);
                        userDetail.gender = Convert.ToString(reader[3]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return userDetail;
        }
    }
}

