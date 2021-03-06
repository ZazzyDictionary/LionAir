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
    public class FlightCrewDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        //Constructor     
        public FlightCrewDAL()
        {
            //Read ConnectionString from appsettings.json file       
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("LionAirConnectionString");

            //Instantiate a SqlConnection object with the     
            //Connection String read.     
            conn = new SqlConnection(strConn);
        }

        public List<FlightCrew> GetAllCrew()
        {
            //Create a SqlCommand object from connection object      
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement          
            cmd.CommandText = @"SELECT * FROM FlightCrew";
            //Open a database connection         
            conn.Open();
            //Execute the SELECT SQL through a DataReader       
            SqlDataReader reader = cmd.ExecuteReader();

            //Read all records until the end, save data into a staff list     
            List<FlightCrew> crewList = new List<FlightCrew>();
            while (reader.Read())
            {
                crewList.Add(
                    new FlightCrew
                    {
                        ScheduleID = reader.GetInt32(0),
                        StaffID = reader.GetInt32(1),
                        Role = reader.GetString(2),
                    }
                    );
            }
            //Close DataReader      
            reader.Close();
            //Close the database connection      
            conn.Close();

            return crewList;
        }

        public List<FlightCrew> GetFlightCrew(int staffid)
        {  
            //Create a SqlCommand object from connection object  
            SqlCommand cmd = conn.CreateCommand(); 

            //Specify the SQL statement that select all branches 
            cmd.CommandText = @"SELECT * FROM FlightCrew WHERE StaffID = @selectedID"; 

            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “branchNo”.  
            cmd.Parameters.AddWithValue("@selectedID", staffid);      
            //Open a database connection    
            conn.Open(); 

            //Execute SELCT SQL through a DataReader  
            SqlDataReader reader = cmd.ExecuteReader(); 

            List<FlightCrew> crewList = new List<FlightCrew>(); 
            while (reader.Read())
            {
                crewList.Add(new FlightCrew
                {
                    ScheduleID = reader.GetInt32(0),
                    StaffID = reader.GetInt32(1),
                    Role = reader.GetString(2),  
                }       
                );   
            } 
            
            //Close DataReader  
            reader.Close(); 
            
            //Close database connection  
            conn.Close();    

            return crewList;
        }

        public bool Checkcrew (List<int> idList)
        {
            if (idList.Distinct().Count() == idList.Count())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Assign(List<FlightCrew> flightcrew, List<int> staffID)
        {
            int rowaffected = 0;
            if (Checkcrew(staffID))
            {
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"INSERT INTO FlightCrew (ScheduleID, StaffID, Role) VALUES (@ScheduleID, @StaffID, @Role)";

                for (int i = 0; i< flightcrew.Count(); i++)
                {
                    cmd.Parameters.Clear();
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ScheduleID", flightcrew[i].ScheduleID);
                    cmd.Parameters.AddWithValue("@StaffID", flightcrew[i].StaffID);
                    cmd.Parameters.AddWithValue("@Role", flightcrew[i].Role);
                    rowaffected += cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            else
            {
                rowaffected = -1;
            }
            return rowaffected;
            
        }

        public List<FlightCrew> GetPilotID()
        {
            //Create a SqlCommand object from connection object      
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement          
            cmd.CommandText = @"SELECT * FROM Staff WHERE Vocation = 'Pilot' AND Status = 'Active'";
            //Open a database connection         
            conn.Open();
            //Execute the SELECT SQL through a DataReader       
            SqlDataReader reader = cmd.ExecuteReader();

            //Read all records until the end, save data into a staff list     
            List<FlightCrew> staffList = new List<FlightCrew>();
            while (reader.Read())
            {
                staffList.Add(
                    new FlightCrew
                    {
                        StaffID = reader.GetInt32(0),
                    }
                    );
            }
            //Close DataReader      
            reader.Close();
            //Close the database connection      
            conn.Close();

            return staffList;
        }

        public List<FlightCrew> GetFAID()
        {
            //Create a SqlCommand object from connection object      
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement          
            cmd.CommandText = @"SELECT * FROM Staff WHERE Vocation = 'Flight Attendant' AND Status = 'Active'";
            //Open a database connection         
            conn.Open();
            //Execute the SELECT SQL through a DataReader       
            SqlDataReader reader = cmd.ExecuteReader();

            //Read all records until the end, save data into a staff list     
            List<FlightCrew> staffList = new List<FlightCrew>();
            while (reader.Read())
            {
                staffList.Add(
                    new FlightCrew
                    {
                        StaffID = reader.GetInt32(0),
                    }
                    );
            }
            //Close DataReader      
            reader.Close();
            //Close the database connection      
            conn.Close();

            return staffList;
        }

    }
     
}
