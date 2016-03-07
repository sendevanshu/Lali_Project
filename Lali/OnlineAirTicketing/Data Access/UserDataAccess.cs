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
                        userDetail.confirmPassword = userDetail.password;
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

        public bool updateUserDetail(UserDetail userDetail)
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

                    // Provide the query string with a parameter placeholder.
                    string queryString =
                        "update T_User set username = @username, password= @password where userID = @userID";

                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@username", userDetail.username);
                    command.Parameters.AddWithValue("@password", userDetail.password);
                    command.Parameters.AddWithValue("@userID", userDetail.userID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    queryString = "update T_Person set Name = @name, ContactNo = @contactno, address= @address From T_Person as P, T_User as U  where p.PersonID = u.PersonID and u.userID = @userID";

                    command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@name", userDetail.personName);
                    command.Parameters.AddWithValue("@contactno", userDetail.contactNumber);
                    command.Parameters.AddWithValue("@address", userDetail.address);
                    command.Parameters.AddWithValue("@userID", userDetail.userID);
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

        public bool makeBooking(BookingData departureFlight, BookingData returnFlight, int userID, out string pnrno)
        {
            bool result = false;
            //generate booking pnr
            pnrno = RandomString(10);
            //enter data for passenger
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                try
                {

                    // Provide the query string with a parameter placeholder.
                    string queryString =
                        "insert into T_Person (Name, ContactNo) OUTPUT INSERTED.PersonID values (@name, @contactno)";

                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@name", departureFlight.passengerName);
                    command.Parameters.AddWithValue("@contactno", departureFlight.contactnumber);

                    connection.Open();
                    int personID = (Int32) command.ExecuteScalar();
                    queryString =
                        "insert into T_Passenger (PersonID, Age, lastUpdatedDate, activeInd) OUTPUT INSERTED.PassengerID values (@personID, @age, @lastUpdatedDate, @activeInd)";

                    command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@personID", personID);
                    command.Parameters.AddWithValue("@age", departureFlight.age);
                    command.Parameters.AddWithValue("@lastUpdatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@activeInd", true);
                    int passengerID = (Int32)command.ExecuteScalar();

                    foreach (int flightlegID in departureFlight.flightLegID)
                    {
                        Random rnd = new Random();
                        int seatno = rnd.Next(1, 6);
                        queryString = "insert into T_LegInstance (FlightID, FlightLegID," +
                            " SeatID, TravelDate, LastUpdatedDate, ActiveInd)"+
                            " OUTPUT INSERTED.LegInstanceID values (@flightID, @flightlegID, @seatID, @traveldate, @lastupdateddate, @activeInd)";

                        command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@flightID", departureFlight.flightID);
                        command.Parameters.AddWithValue("@flightlegID", flightlegID);
                        command.Parameters.AddWithValue("@seatID", seatno);
                        command.Parameters.AddWithValue("@traveldate", departureFlight.travelDate);
                        command.Parameters.AddWithValue("@lastupdateddate", DateTime.Now);
                        command.Parameters.AddWithValue("@activeInd", true);
                        int legInstanceID = (Int32)command.ExecuteScalar();

                        queryString = "insert into T_BookingDetail (LegInstanceID, TicketPNR," +
                            " BookingTime, lastUpdatedDate, actInd, UserID, passengerID, amount)" +
                            "  values (@legInstanceID, @ticketPNR, @bookingTime, @lastUpdatedDate, @actInd, @UserID, @passengerID, @amount)";

                        command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@legInstanceID", legInstanceID);
                        command.Parameters.AddWithValue("@ticketPNR", pnrno);
                        command.Parameters.AddWithValue("@bookingTime", DateTime.Now);
                        command.Parameters.AddWithValue("@lastUpdatedDate", DateTime.Now);
                        command.Parameters.AddWithValue("@actInd", true);
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@passengerID", passengerID);
                        command.Parameters.AddWithValue("@amount", (float)Convert.ToDouble(departureFlight.cost));
                        command.ExecuteNonQuery();

                    }

                    foreach (int flightlegID in returnFlight.flightLegID)
                    {
                        Random rnd = new Random();
                        int seatno = rnd.Next(1, 6);
                        queryString = "insert into T_LegInstance (FlightID, FlightLegID," +
                            " SeatID, TravelDate, LastUpdatedDate, ActiveInd)" +
                            " OUTPUT INSERTED.LegInstanceID values (@flightID, @flightlegID, @seatID, @traveldate, @lastupdateddate, @activeInd)";

                        command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@flightID", returnFlight.flightID);
                        command.Parameters.AddWithValue("@flightlegID", flightlegID);
                        command.Parameters.AddWithValue("@seatID", seatno);
                        command.Parameters.AddWithValue("@traveldate", returnFlight.travelDate);
                        command.Parameters.AddWithValue("@lastupdateddate", DateTime.Now);
                        command.Parameters.AddWithValue("@activeInd", true);
                        int legInstanceID = (Int32)command.ExecuteScalar();

                        queryString = "insert into T_BookingDetail (LegInstanceID, TicketPNR," +
                            " BookingTime, lastUpdatedDate, actInd, UserID, passengerID, amount)" +
                            "  values (@legInstanceID, @ticketPNR, @bookingTime, @lastUpdatedDate, @actInd, @UserID, @passengerID, @amount)";

                        command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@legInstanceID", legInstanceID);
                        command.Parameters.AddWithValue("@ticketPNR", pnrno);
                        command.Parameters.AddWithValue("@bookingTime", DateTime.Now);
                        command.Parameters.AddWithValue("@lastUpdatedDate", DateTime.Now);
                        command.Parameters.AddWithValue("@actInd", true);
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@passengerID", passengerID);
                        command.Parameters.AddWithValue("@amount", (float)Convert.ToDouble(returnFlight.cost));
                        command.ExecuteNonQuery();

                    }

                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return result;
            }
            //for each leg enter data in leginstance

            //for each leg instance id insert data in booking id
           
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<BookingData> GetBookingDetailsForUser(int userID)
        {
            List<BookingData> bookingDetails = new List<BookingData>();
            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT lg.flightID, lg.flightLegID, lg.seatID, lg.traveldate, bok.ticketpnr,"+
                " bok.amount, pas.age, per.name, per.contactno from dbo.T_User as usr, dbo.T_Person as per, dbo.T_Passenger as pas, dbo.T_LegInstance as lg, T_BookingDetail as bok "
                    + "WHERE lg.activeInd = 'true' and bok.legInstanceID = lg.legInstanceID and bok.UserID = @userID and bok.passengerID = pas.passengerID and pas.personID = per.personID ";

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
                    
                    while (reader.Read())
                    {
                        
                        BookingData bookingData = new BookingData();
                        bookingData.flightID = Convert.ToInt32(reader[0]);
                        int prevFlightID = bookingData.flightID;
                        bookingData.travelDate = Convert.ToString(reader[3]);
                        bookingData.pnrno = Convert.ToString(reader[4]);
                        bookingData.amount = Convert.ToDouble(reader[5]);
                        bookingData.age = Convert.ToString(reader[6]);
                        bookingData.passengerName = Convert.ToString(reader[7]);
                        bookingData.contactnumber = Convert.ToString(reader[8]);
                        bookingData.flightLegID = new List<int>();
                        bookingData.seatno = new List<int>();
                        bookingData.flightLegID.Add(Convert.ToInt32(reader[1]));
                        bookingData.seatno.Add(Convert.ToInt32(reader[2]));
                        while (reader.Read())
                        {
                            if (prevFlightID != Convert.ToInt32(reader[0]))
                                break;
                            bookingData.flightLegID.Add(Convert.ToInt32(reader[1]));
                            bookingData.seatno.Add(Convert.ToInt32(reader[2]));
                        }
                        bookingDetails.Add(bookingData);

                    }
                    reader.Close();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return bookingDetails;
        }

        public bool cancelTicket(string pnrno)
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

                    // Provide the query string with a parameter placeholder.
                    string queryString =
                        "update T_LegInstance set activeInd = 'false' From T_LegInstance as L, T_BookingDetail as B  where  B.legInstanceID = L.legInstanceID and  B.TicketPNR = @pnrno";

                    // Create the Command and Parameter objects.
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@pnrno", pnrno);
                    
                    connection.Open();
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

