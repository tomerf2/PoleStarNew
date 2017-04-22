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


        ////University (80 samples)
        //public static double UniLat = 32.112783;
        //public static double UniLong = 34.806206;

        ////Parents - Netanya (40+ samples)
        //public static double ParentsLat = 32.302502;
        //public static double ParentsLong = 34.876437;


        ////Grandparents - Haifa (20+ samples)
        //public static double GrandParLat = 32.799565;
        //public static double GrandParLong = 35.000884;

        ////Work (70+samples)
        //public static double WorkLat = 32.101180;
        //public static double WorkLong = 34.850500;

        //Sample Locations
        //KNOWN - House - Shoftim 24 (130+ samples) > 30% of all samples <=> high density
        public static double HouseLat = 32.076227;
        public static double HouseLong = 34.780776;

        //KNOWN - Seniors Club - Givataiym (30+ samples)
        public static double ClubLat = 32.069109;
        public static double ClubLong = 34.806445;

        //Daugther - Herzeliya (115+ samples) > 30% of all samples <=> high density
        public static double DaugtherLat = 32.155735;
        public static double DaugtherLong = 34.844036;

        //Gariatric Doctor - Center Tel-Aviv (10+ samples)
        public static double DocLat = 32.075952;
        public static double DocLong = 34.774814;

        //Grandson - Ramat Hasharon (5+ samples)
        public static double GrandsonLat = 32.140193;
        public static double GrandsonLong = 34.842493;

        //Store - London ministore (10+ samples)
        public static double StoreLat = 32.074852;
        public static double StoreLong = 34.781883;

        //Feldenkrais Gym - North Tel-Aviv (40+ samples, avg. hearrate 103)
        public static double FeldenLat = 32.107991;
        public static double FeldenLong = 34.793992;

        //Sample locations to trigger wanderingAlgo - during DEMO (?)//
        //Drove to Hashoftim 24 Holon by mistake
        //Location - Over 10 KM (9-10 KM from Home, over 11 from Seniors Club) LowDensed area (1 sample)
        //Time - normal time (not emergency period)
        //HR - normal
        //Result - RISK (for good reason)
        public static double sample1Lat = 32.016822;
        public static double sample1Long = 34.798152;

        //Sample locations to trigger wanderingAlgo - during DEMO (?)//
        //Yafo - Jerusalem Blvd. - went to buy his favorite sourcroute
        //Location - 5-10 KM from closest known (Home ?) LowDensed area (1 sample)
        //Time - normal time (not emergency period)
        //HR - normal
        //Result - Wandering - need to contact the patient, and perhaps add to known location
        public static double sample2Lat = 32.042995;
        public static double sample2Long = 34.757491;

        //Sample locations to trigger wanderingAlgo - during DEMO (?)//
        //right after feldenkrais training - Green Ramat Aviv (Reading 23, next to Reading 35 where the gym is at)
        //Location - 2-5 KM from and MedDensed area (very close to Gym) (1 sample) - caregiver will add to knownlocations
        //Time - normal time (not emergency period)
        //HR - high (around 100)! but is in normal average for location - need to mention that
        //Result - Wandering (due to distance from known - buy let caregiver add the location to known) - vision is to add it automatically
        public static double sample3Lat = 32.106431;
        public static double sample3Long = 34.794054;

        //Sample locations to trigger wanderingAlgo - during DEMO (?)//
        //Ibn Gabirol 110 - normal location for our patient.. BUT hearrate is super abnormal - alert!!
        //Location - 0.5-2 KM from known location (Home), but heart cond. might be bad (1 sample)
        //Time - normal time (not emergency period)
        //HR - high!! say about 140 (where avg. there would be around 75+-)
        public static double sample4Lat = 32.084601;
        public static double sample4Long = 34.781739;


        //Sample locations to trigger wanderingAlgo - during DEMO (?)//
        //at daughter's house - Pines 10/8 Herzeliya - which wasn't added to the known location (by accident, but our algo was smart enough to know it's still safe)
        //Location - Over 10KM from knownlocation.. BUT - in high density place (daughter's house)
        //Time - normal time (not emergency period)
        //HR - normal
        //Result - Safety! + better add to known locations
        public static double sample5Lat = 32.155877;
        public static double sample5Long = 34.843798;



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
