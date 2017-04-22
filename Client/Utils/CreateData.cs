using Microsoft.WindowsAzure.MobileServices;
using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;


namespace PoleStar.Utils
{
    class CreateData
    {
        private static Random random = new Random();
        private static string patientID = StoredData.getPatientID();
        private static IMobileServiceTable<Sample> sampleTable = App.MobileService.GetTable<Sample>();

        //DemoDay Info:
        /*
         * Group:
         * name: DemoGroup
         * password: 123
         * 
         * Patient:
         * name: DemoPatient
         * email: Demopatient@gmail.com
         * password: 123
         * 
         * Caregiver1:
         * email: c1@gmail.com
         * phone: 1111111111
         * 
         * Caregiver1:
         * email: c2@gmail.com
         * phone: 2222222222
         */

        //Sample Locations
        //House - Shoftim 24 (over 100 samples)
        public static double HouseLat = 32.076227;
        public static double HouseLong = 34.780776;

        //University (80 samples)
        public static double UniLat = 32.112783;
        public static double UniLong = 34.806206;

        //Parents - Netanya (40+ samples)
        public static double ParentsLat = 32.302502;
        public static double ParentsLong = 34.876437;

        //Store - London ministore (20+ samples)
        public static double StoreLat = 32.074852;
        public static double StoreLong = 34.781883;

        //Grandparents - Haifa (20+ samples)
        public static double GrandParLat = 32.799565;
        public static double GrandParLong = 35.000884;

        //Work (70+samples)
        public static double WorkLat = 32.101180;
        public static double WorkLong = 34.850500;

        public static async Task insertSamples(int cnt, double latitude, double longitude)
        {
            for (int i = 0; i < cnt; i++)
            {
                double lat = latitude + (random.Next(0, 2) * 2 - 1)*GetRandomNumber(0.000000001, 0.00012);
                double lon = longitude + (random.Next(0, 2) * 2 - 1) * GetRandomNumber(0.000000001, 0.00012);
                int heartrate = 75 + (random.Next(0, 2) * 2 - 1) * random.Next(0, 15);
                Sample toInsert = new Sample()
                {
                    Id = Guid.NewGuid().ToString(),
                    PatientID = patientID,
                    HeartRate = heartrate,
                    Latitude = (float)lat,
                    Longitude = (float)lon
                };

                await sampleTable.InsertAsync(toInsert);
            }
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        //For Use
        //await CreateData.insertSamples(30, CreateData.HouseLat, CreateData.HouseLong);
        //await CreateData.insertSamples(22, CreateData.GrandParLat, CreateData.GrandParLong);
        //await CreateData.insertSamples(46, CreateData.ParentsLat, CreateData.ParentsLong);
        //await CreateData.insertSamples(25, CreateData.StoreLat, CreateData.StoreLong);
        //await CreateData.insertSamples(80, CreateData.UniLat, CreateData.UniLong);
        //await CreateData.insertSamples(77, CreateData.WorkLat, CreateData.WorkLong);
    }
}
