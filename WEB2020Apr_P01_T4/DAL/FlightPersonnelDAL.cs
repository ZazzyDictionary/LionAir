﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using WEB2020Apr_P01_T4.Models;

namespace WEB2020Apr_P01_T4.DAL
{
    public class FlightPersonnelDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor     
        public FlightPersonnelDAL()
        {
            //Read ConnectionString from appsettings.json file       
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionAirConnectionString");

            //Instantiate a SqlConnection object with the     
            //Connection String read.     
            conn = new SqlConnection(strConn);
        }

        

        public List<FlightPersonnel> GetAllStaff()
        {
            //Create a SqlCommand object from connection object      
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement          
            cmd.CommandText = @"SELECT * FROM Staff";
            //Open a database connection         
            conn.Open();
            //Execute the SELECT SQL through a DataReader       
            SqlDataReader reader = cmd.ExecuteReader();

            //Read all records until the end, save data into a staff list     
            List<FlightPersonnel> staffList = new List<FlightPersonnel>();
            while (reader.Read())
            {
                staffList.Add(
                    new FlightPersonnel
                    {
                        StaffID = reader.GetInt32(0),
                        StaffName = reader.GetString(1),
                        Gender = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                        DateEmployed = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null,
                        Vocation = !reader.IsDBNull(4) ? reader.GetString(4) : (string)null,
                        EmailAddr = reader.GetString(5),
                        Status = reader.GetString(7),
                    }
                    );
            }
            //Close DataReader      
            reader.Close();
            //Close the database connection      
            conn.Close();

            return staffList;
        }

        public int Add(FlightPersonnel flightPersonnel)
        {    
            //Create a SqlCommand object from connection object    
            SqlCommand cmd = conn.CreateCommand(); 

            //Specify an INSERT SQL statement which will    
            //return the auto-generated StaffID after insertion   
            cmd.CommandText = @"INSERT INTO Staff (StaffName, Gender, DateEmployed, Vocation, EmailAddr, Status) 
                                OUTPUT INSERTED.StaffID VALUES(@StaffName, @Gender, @DateEmployed, @Vocation, @EmailAddr, @Status)";  
            
            //Define the parameters used in SQL statement, value for each parameter  
            //is retrieved from respective class's property.    
            cmd.Parameters.AddWithValue("@StaffName", flightPersonnel.StaffName);  

            if (flightPersonnel.Gender == null)
            {
                cmd.Parameters.AddWithValue("@Gender", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Gender", flightPersonnel.Gender);
            }
            
            if (flightPersonnel.DateEmployed == null || flightPersonnel.DateEmployed.ToString() == "")
            {
                cmd.Parameters.AddWithValue("@DateEmployed", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DateEmployed", flightPersonnel.DateEmployed);
            }
            
            if (flightPersonnel.Vocation == null || flightPersonnel.Vocation.ToString() == "")
            {
                cmd.Parameters.AddWithValue("@Vocation", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Vocation", flightPersonnel.Vocation);
            }

            cmd.Parameters.AddWithValue("@EmailAddr", flightPersonnel.EmailAddr);

            cmd.Parameters.AddWithValue("@Status", flightPersonnel.Status);
            
            //A connection to database must be opened before any operations made. 
            conn.Open();

            //ExecuteScalar is used to retrieve the auto-generated 
            ////StaffID after executing the INSERT SQL statement   

            //flightPersonnel.StaffID = (int)cmd.ExecuteScalar();  
            flightPersonnel.StaffID = (int)cmd.ExecuteNonQuery();

            //A connection should be closed after operations. 
            conn.Close();

            //Return id when no error occurs.
            return flightPersonnel.StaffID;
        }

        public bool IsEmailExist(string Email, int StaffID)
        {
            bool EmailFound = false;

            //Create a SqlCommand object and specify the SQL statement 
            //to get a staff record with the email address to be validated
            SqlCommand cmd = conn.CreateCommand(); 
            cmd.CommandText = @"SELECT StaffID from Staff WHERE EmailAddr = @selectedEmail;";   
            cmd.Parameters.AddWithValue("@selectedEmail", Email); 

            //Open a database connection and excute the SQL statement  
            conn.Open();   
            SqlDataReader reader = cmd.ExecuteReader(); 

            if (reader.HasRows)
            { 
                //Records found  
                while (reader.Read())
                {      
                    if (reader.GetInt32(0) != StaffID)       
                        //The email address is used by another staff   
                        EmailFound = true;      
                }  
            }   
            else 
            { 
                //No record     
                EmailFound = false;
                // The email address given does not exist 
            }   

            reader.Close();  
            conn.Close();  
            return EmailFound;
        }

        public FlightPersonnel GetDetails(int staffId)
        {
            FlightPersonnel flightPersonnel = new FlightPersonnel();

            //Create a SqlCommand object from connection object   
            SqlCommand cmd = conn.CreateCommand();

            //Specify the SELECT SQL statement that   
            //retrieves all attributes of a staff record.  
            cmd.CommandText = @"SELECT * FROM Staff WHERE StaffID = @selectedStaffID";

            //Define the parameter used in SQL statement, value for the 
            //parameter is retrieved from the method parameter “staffId”.  
            cmd.Parameters.AddWithValue("@selectedStaffID", staffId);

            //Open a database connection    
            conn.Open();

            //Execute SELCT SQL through a DataReader  
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database     
                while (reader.Read())
                {
                    flightPersonnel.StaffID = reader.GetInt32(0); 
                    flightPersonnel.StaffName = reader.GetString(1);
                    //flightPersonnel.Gender = reader.GetString(2)[0];
                    //flightPersonnel.DateEmployed = reader.GetDateTime(3);
                    flightPersonnel.Gender = !reader.IsDBNull(2) ? reader.GetString(2) : null;
                    flightPersonnel.DateEmployed = !reader.IsDBNull(3) ? reader.GetDateTime(3) : (DateTime?)null;
                    //flightPersonnel.Vocation = reader.GetString(4);
                    flightPersonnel.Vocation = !reader.IsDBNull(4) ? reader.GetString(4) : (string)null;                  
                    flightPersonnel.EmailAddr = reader.GetString(5);
                    flightPersonnel.Status = reader.GetString(7);
                }
            }
            //Close data reader  
            reader.Close();

            //Close database connection  
            conn.Close();

            return flightPersonnel;
        }

        public bool VaildStaff(String email, String password, out int staffID)
        {

            staffID = 0;
            try
            {

                // writing sql query  
                SqlCommand cm = new SqlCommand(String.Format("SELECT StaffID FROM Staff WHERE UPPER(EmailAddr) = UPPER('{0}') AND Password = '{1}'",
                    email.ToUpper(),
                    password
                    ), conn);


                // Opening Connection  
                conn.Open();
                // Executing the SQL query  
                SqlDataReader sqlDataReader = cm.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    staffID = sqlDataReader.GetInt32(0);
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception e)
            {
                return false;
            }
            // Closing the connection  
            finally
            {
                conn.Close();
            }
        }

        public bool VaildAdmin(String email, String password, out int staffID)
        {

            staffID = 0;
            try
            {

                // writing sql query  
                SqlCommand cm = new SqlCommand(String.Format("SELECT StaffID FROM Staff WHERE UPPER(EmailAddr) = UPPER('{0}') AND Password = '{1}' AND Vocation = 'Administrator' ",
                    email.ToUpper(),
                    password
                    ), conn);


                // Opening Connection  
                conn.Open();
                // Executing the SQL query  
                SqlDataReader sqlDataReader = cm.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    staffID = sqlDataReader.GetInt32(0);
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception e)
            {
                return false;
            }
            // Closing the connection  
            finally
            {
                conn.Close();
            }
        }

        public int Update(FlightPersonnel flightPersonnel)
        {
            //Create a SqlCommand object from connection object  
            SqlCommand cmd = conn.CreateCommand();

            //Specify an UPDATE SQL statement  
            cmd.CommandText = @"UPDATE Staff SET Status=@status WHERE StaffID = @selectedStaffID";

            //Define the parameters used in SQL statement, value for each parameter   
            //is retrieved from respective class's property.  
            cmd.Parameters.AddWithValue("@status", flightPersonnel.Status);

            cmd.Parameters.AddWithValue("@selectedStaffID", flightPersonnel.StaffID);
            //Open a database connection  
            conn.Open();

            //ExecuteNonQuery is used for UPDATE and DELETE   
            int count = (int)(cmd.ExecuteNonQuery());

            //Close the database connection  
            conn.Close();

            return count;

        }

        public List<FlightPersonnel> GetAllFlightPersonal(int id)
        {
            List<FlightPersonnel> flightPersonal = new List<FlightPersonnel>();
            try
            {

                // writing sql query  
                SqlCommand cm = new SqlCommand("SELECT s.StaffName, s.Vocation, c.ScheduleID, c.Role FROM Staff s INNER JOIN FlightCrew c ON s.StaffID = c.StaffID WHERE s.StaffID = " + id, conn);

                //Open the connection
                conn.Open();

                //Excuting the query
                SqlDataReader sqlDataReader = cm.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    flightPersonal.Add(new FlightPersonnel
                    {
                        StaffName = sqlDataReader.GetString(0),
                        Vocation = sqlDataReader.GetString(1),
                        ScheduleID = sqlDataReader.GetInt32(2),
                        Role = sqlDataReader.GetString(3),


                    });
                }


                return flightPersonal;


            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

    }
    
}
