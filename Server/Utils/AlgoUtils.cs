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
            ConnectionLost
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
            //TODO: check if currentLoc is in a desned area on heat map or not
            return AlgoUtils.HeatMapDensity.High;
        }

        public static double avgHeartRate(string currentPatientID)
        {
            MobileServiceContext db = new MobileServiceContext();
            
            //TODO: take first 1000
            Sample[] samplesArr = db.Samples.Where(p => p.PatientID == currentPatientID).ToArray();
            double totalSum = 0;
            totalSum = samplesArr.Sum(p => p.HeartRate);

            return (totalSum / samplesArr.Length); //avgHR of patient
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
            //calcs the safe time range for current sample, according to past samples from 0.3 KM away!!//
            MobileServiceContext db = new MobileServiceContext();
            Sample[] samplesArr = db.Samples.Where(p => p.PatientID == currentPatientID).ToArray();
            List<Sample> relevantSamples = new List<Sample>();
            GeoCoordinate latestSampleLoc = new GeoCoordinate(latestSample.Latitude, latestSample.Longitude);
            GeoCoordinate someSample = new GeoCoordinate();

            foreach (var sample in samplesArr)
            {
                someSample.Latitude = sample.Latitude;
                someSample.Longitude = sample.Longitude;
                if (calcDist(latestSampleLoc,someSample) < 0.3)
                {
                    relevantSamples.Add(sample);
                }
            }

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

        public static void sendWanderingNotification()
        {
            string patientName = WanderingAlgo.patientName;
            foreach (var caregiver in WanderingAlgo.caregiversArr)
            {
                WanderingAlgo.notificationHub.sendNotificationToCareGivers(((Caregiver)caregiver).Id, patientName, Status.Wandering);
            }
            WanderingAlgo.notificationHub.sendSMSToCareGivers(WanderingAlgo.patientID, patientName, Status.Wandering);
        }

        public static void sendRiskNotification()
        {
            string patientName = WanderingAlgo.patientName;
            foreach (var caregiver in WanderingAlgo.caregiversArr)
            {
                WanderingAlgo.notificationHub.sendNotificationToCareGivers(((Caregiver)caregiver).Id, patientName, Status.Risk);
            }
            WanderingAlgo.notificationHub.sendSMSToCareGivers(WanderingAlgo.patientID, patientName, Status.Risk);
        }
        public static void sendDistressNotification()
        {
            string patientName = WanderingAlgo.patientName;
            foreach (var caregiver in WanderingAlgo.caregiversArr)
            {
                WanderingAlgo.notificationHub.sendNotificationToCareGivers(((Caregiver)caregiver).Id, patientName, Status.Distress);
            }
            WanderingAlgo.notificationHub.sendSMSToCareGivers(WanderingAlgo.patientID, patientName, Status.Distress);
        }

        public static void sendLostConnNotification()
        {
            string patientName = WanderingAlgo.patientName;
            foreach (var caregiver in WanderingAlgo.caregiversArr)
            {
                WanderingAlgo.notificationHub.sendLostConnNotificationToCareGivers(((Caregiver)caregiver).Id, patientName);
            }
        }
    }
}