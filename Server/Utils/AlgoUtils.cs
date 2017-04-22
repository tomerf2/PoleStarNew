using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Device.Location;
using Microsoft.Owin.Security;
using Server.DataObjects;
using Server.Models;
using Server.Hubs;
using System.Collections;
using System.Diagnostics;
using Server.Controllers;

namespace Server.Utils
{
    public class AlgoUtils
    {
        public static double calcDist(GeoCoordinate currentLoc, GeoCoordinate otherLoc)
        {
            double result = (currentLoc.GetDistanceTo(otherLoc)) / 1000; //return distance in KM
            return result; 
        }

        public static Location closestKnownLocation(GeoCoordinate currentLoc, Location[] knownLocations)
        {
            GeoCoordinate otherLoc = new GeoCoordinate(knownLocations[0].Latitude, knownLocations[0].Longitude);
            double minDistToKnownLocation = AlgoUtils.calcDist(currentLoc, otherLoc);
            Location closestKnownLocation = knownLocations[0];
            double currentDist;

            for (int i = 1; i < knownLocations.Length; i++)
            {
                otherLoc.Latitude = knownLocations[i].Latitude;
                otherLoc.Longitude = knownLocations[i].Longitude;
                currentDist = AlgoUtils.calcDist(currentLoc, otherLoc);
                if (currentDist < minDistToKnownLocation)
                {
                    minDistToKnownLocation = currentDist;
                    closestKnownLocation = knownLocations[i];
                }
            }

            return closestKnownLocation;
        }

        public enum Status
        {
            Safety,
            Wandering,
            Distress,
            Risk,
            ConnectionLost,
            NeedsAssistance, //help button pressed
            Learning //not enough samples collected
        }

        public enum HeatMapDensity
        {
            High,
            Medium,
            Low,
            Zero
        }

        public static AlgoUtils.HeatMapDensity heatMapAreaDensityLevel(GeoCoordinate currentLoc)
        {
            MobileServiceContext db = new MobileServiceContext();

            //take samples from closeDistance (parameter)
            List<Sample> closeDistanceSamples = getGeoNearSamples(WanderingAlgo.latestSample);
            //take last 300 samples of current patient regardless of location
            Sample[] samplesArr = db.Samples.Where(p => p.PatientID == WanderingAlgo.latestSample.PatientID).OrderByDescending(p => p.CreatedAt).Take(300).ToArray();

            double densityRatio = (closeDistanceSamples.Count / samplesArr.Length);

            if (densityRatio < 0.05)
            {
                return AlgoUtils.HeatMapDensity.Zero;
            }
            else if (densityRatio < 0.1)
            {
                return AlgoUtils.HeatMapDensity.Low;
            }
            else if (densityRatio < 0.3)
            {
                return AlgoUtils.HeatMapDensity.Medium;
            }
            else //densityRatio is over 35%, very high!
            {
                return AlgoUtils.HeatMapDensity.High;
            }
        }

        public static double avgHeartRate(string currentPatientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            bool enoughNearSamples = false;
            double totalSum = 0;
            int count = 0;

            List<Sample> closeDistanceSamples = getGeoNearSamples(WanderingAlgo.latestSample);
            if (closeDistanceSamples.Count > 10)
            {
                enoughNearSamples = true;
            }

            if (enoughNearSamples)
            {
                totalSum = closeDistanceSamples.Sum(p => p.HeartRate);
                count = closeDistanceSamples.Count;
            }
            else //use last 300 samples, regardless their GeoLocation
            {
                //take last 300 samples of current patient
                Sample[] samplesArr = db.Samples.Where(p => p.PatientID == currentPatientID).OrderByDescending(p => p.CreatedAt).Take(300).ToArray();
                totalSum = samplesArr.Sum(p => p.HeartRate);
                count = samplesArr.Length;
            }


            return (totalSum / count); //avgHR of patient
        }

        public static bool isHeartRateInSafeRange(double currentHR)
        {
            if (currentHR >= WanderingAlgo.HEART_RATE_BOTTOM_LIMIT && currentHR <= WanderingAlgo.HEART_RATE_TOP_LIMIT)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isTimeInSafeRange(int currentHour)
        {
            if (currentHour < WanderingAlgo.emergencyTimeRangeSTART
                || currentHour > WanderingAlgo.emergencyTimeRangeEND)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int[] getSafeTimeRangeForLocation(Sample latestSample, string currentPatientID)
        {
            //calcs the safe time range for current sample,
            //according to past samples from WanderingAlgo.closeDistance (parameter) away!!//
            List<Sample> relevantSamples = getGeoNearSamples(latestSample);

            int bottomLimit = relevantSamples.Min(p => p.CreatedAt.Value.Hour);
            int topLimit = relevantSamples.Max(p => p.CreatedAt.Value.Hour);
            int averageHour = (int)relevantSamples.Average(p => p.CreatedAt.Value.Hour);
            int[] result = { bottomLimit, topLimit, averageHour };
            return (result);
        }

        public static bool isTimeInNormalRange(int currentHour)
        {
            if (currentHour > WanderingAlgo.bottomNormalTimeRange
                && currentHour < WanderingAlgo.topNormalTimeRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Boolean learningStage(string patientID)
        {
            int numOfSamples = SampleController.GetSampleCountforPatientrID(patientID);
            if (numOfSamples < 200)
            {
                return true; //in learning stage
            }
            return false;//not in learning stage
        }


        public static void sendSMS(IEnumerable<Caregiver> caregiversArr, Status status)
        {
            foreach (var caregiver in caregiversArr)
            {
                try
                {
                    NotificationHub.static_sendSMS(caregiver.Id, caregiver.Phone, status);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }

        }


        public static List<Sample> getGeoNearSamples(Sample latestSample)
        {
            MobileServiceContext db = new MobileServiceContext();
            Sample[] samplesArr = db.Samples.Where(p => p.PatientID == latestSample.PatientID).ToArray();
            List<Sample> relevantSamples = new List<Sample>();
            GeoCoordinate latestSampleLoc = new GeoCoordinate(latestSample.Latitude, latestSample.Longitude);
            GeoCoordinate someSample = new GeoCoordinate();

            foreach (var sample in samplesArr)
            {
                someSample.Latitude = sample.Latitude;
                someSample.Longitude = sample.Longitude;
                if (calcDist(latestSampleLoc, someSample) < WanderingAlgo.closeDistance)
                {
                    relevantSamples.Add(sample);
                }
            }

            return relevantSamples;
        }

    }
    
}