﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Data;


namespace Taxi_Management_System
{
    public static class DriverGlobals
    {
        public static Queue<Driver> FreeDrivers = new Queue<Driver>();
        public static Dictionary<string, Driver> DriverMap = new Dictionary<string, Driver>();
    }

    public class Driver
    {
        public string DriverUsername;
        public string DriverPassword;
        public string DriverName;
        public string DriverID;
        public string CarID;
        public string Salary;
        public string Status;
        public List<Trip> DriverTrip_ = new List<Trip>();

        //Functions
        //Filling the map of drivers
        public void FillDriverMap()
        {
            //Mohie
            StreamReader Stream = new StreamReader(@"C:\Users\Menna\Source\Repos\TaxiSystem\Taxi Management System\Text Files\Drivers.txt");

            //Eman
            //StreamReader Stream = new StreamReader(@"C:\Users\Eman\Source\Repos\TaxiSystem\Taxi Management System\Text Files\Drivers.txt");

            //StreamReader Stream = new StreamReader("Drivers.txt");

            char[] Delimeters = {' ', ',', '.', ':', '\t'};
            string Line = Stream.ReadLine();
            
            while (!string.IsNullOrWhiteSpace(Line))
            {
                Driver tempDriver = new Driver();
                string[] Words = Line.Split(Delimeters);

                tempDriver.DriverUsername = Words[0];
                tempDriver.DriverPassword = Words[1];
                tempDriver.DriverName = Words[2] + ' ' + Words[3];
                tempDriver.DriverID = Words[4];
                tempDriver.Salary = Words[5];
                tempDriver.Status = Words[6];

                DriverGlobals.DriverMap.Add(tempDriver.DriverUsername, tempDriver);

                if (tempDriver.Status == "1")
                    DriverGlobals.FreeDrivers.Enqueue(DriverGlobals.DriverMap[tempDriver.DriverUsername]);

                Line = Stream.ReadLine();
            }

            Stream.Close();
        }

        //Function to change the Driver Status
        public void ChangeStatus(string name)
        {
            if (DriverGlobals.DriverMap[name].Status == "0")
            {
                DriverGlobals.DriverMap[name].Status = "1";
                DriverGlobals.FreeDrivers.Enqueue(DriverGlobals.DriverMap[name]);
            }
            else
            {
                DriverGlobals.DriverMap[name].Status = "0";
            }
        }
        
        //Recieving a request
        public void RecieveRequest(Driver FreeDriver_, Client Customer)
        {
            Trip TempTrip = new Trip();
            FreeDriver_.ChangeStatus(FreeDriver_.DriverUsername);
            TempTrip.ClientName = Customer.ClientName;
            TempTrip.From = "";
            TempTrip.To = "";
            DriverTrip_.Add(TempTrip);

            //////////////////////////Send trip data to the admin List
            Trip TempTripp = new Trip();
            TempTripp.DriverName = FreeDriver_.DriverName;
            TempTripp.ClientName = TempTrip.ClientName;
            TempTripp.From = TempTrip.From;
            TempTripp.To = TempTrip.To;
            TempTripp.Date = TempTrip.Date;

            Admin TempAdmin = new Admin();
            TempAdmin.SendTripData(TempTripp);
        }

        public void ViewdriverHistory(string uname,DataGridView D)
        {
            int num_trips;
            num_trips = DriverGlobals.DriverMap[uname].DriverTrip_.Count;
            DataTable DriverTrips = new DataTable();
            DriverTrips.Columns.Add("Client Name");
            DriverTrips.Columns.Add("From");
            DriverTrips.Columns.Add("To");
            DriverTrips.Columns.Add("Date");

            DataRow NewTrip;
            for (int i = 0; i < num_trips; i++)
            {
                NewTrip = DriverTrips.NewRow();
                NewTrip["Client Name"] = DriverGlobals.DriverMap[uname].DriverTrip_[i].ClientName;
                NewTrip["From"] = DriverGlobals.DriverMap[uname].DriverTrip_[i].From;
                NewTrip["To"] = DriverGlobals.DriverMap[uname].DriverTrip_[i].To;
                NewTrip["Date"] = DriverGlobals.DriverMap[uname].DriverTrip_[i].Date;
                DriverTrips.Rows.Add(NewTrip);
            }
            D.DataSource = DriverTrips;
    }
}
}
