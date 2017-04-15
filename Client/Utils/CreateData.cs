using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoleStar.Utils
{
    class CreateData
    {
        public static async void createGroup(string groupID, string groupName, string password)
            {
                Group newGroup = new Group() { Id = groupID, Name = groupName, Code = password };
                IMobileServiceTable<Group> groupTable = App.MobileService.GetTable<Group>();
                await groupTable.InsertAsync(newGroup);
            }

        public static async void createPatient(string patientID, string patientName, string email, string password, string groupID)
        {
            Patient newPatient = new Patient() { Id = patientID, GroupID = groupID, Name = patientName, Email = email, Password = Crypto.CreateMD5Hash(password) };
            IMobileServiceTable<Patient> patientTable = App.MobileService.GetTable<Patient>();
            await patientTable.InsertAsync(newPatient);
        }

        public static async void createCaregiver(string caregiverID, string email, string password, string groupID)
        {
            IMobileServiceTable<Caregiver> caregiverTable = App.MobileService.GetTable<Caregiver>();
            string phoneNum = GenerateNumber();
            Caregiver newCaregiver = new Caregiver() { Id = caregiverID, GroupID = groupID, Phone = phoneNum, Email = email, IsApproved = false };
            await caregiverTable.InsertAsync(newCaregiver);
        }

        public static async void createLocation(string locationID, string patientID, float latit, float longt, string desc, int heartrate)
        {
            IMobileServiceTable<Location> locationTable = App.MobileService.GetTable<Location>();
            Location newLocation = new Location();
            newLocation.Id = locationID;
            newLocation.PatientID = patientID;
            newLocation.HeartRate = heartrate;
            newLocation.Longitude = longt;
            newLocation.Latitude = latit;
            newLocation.Description = desc;
            await locationTable.InsertAsync(newLocation);
        }



        public static string GenerateNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;
        }

        public static void createAndInsertData()
        {
            //createGroup("group1", "the tigers", "123456789");
            //createPatient("patient1", "John Smith", "oldjohn@gmail.com", "123456789", "group1");

            //createCaregiver("caregiver1", "tomerfried@gmail.com", "123456789", "group1");
            //createCaregiver("caregiver2", "guygrin@gmail.com", "123456789", "group1");
            //createCaregiver("caregiver3", "benliv@gmail.com", "123456789", "group1");
            //createCaregiver("caregiver4", "yehudaaf@gmail.com", "123456789", "group1");

            //createLocation("location1", "46ab7b52-f38d-47c6-9c2c-696d590d3da1", (float)32.087675, (float)34.782371, "home, ibn gabirol 130, TA", 75);
            //createLocation("location2", "46ab7b52-f38d-47c6-9c2c-696d590d3da1", (float)32.089471, (float)34.782750, "doctor, ibn gabirol 150, TA", 89);
            //createLocation("location3", "patient1", (float)32.078939, (float)34.773873, "gym, dizengoff 93, TA", 122);
            //createLocation("location4", "patient1", (float)32.083164, (float)34.814402, "grandson's house, bialik 37, RG", 84);
            //createLocation("location5", "patient1", (float)32.089368, (float)34.790280, "barber", 90);
        }
    }
}
