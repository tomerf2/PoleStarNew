using Server.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Server.Controllers;
using Server.Utils;
using System.Device.Location;

namespace Server.Utils
{
    class WanderingAlgo
    {
        /// 0.1 load all data on patient
        /// 0.2 load caregivers
        Caregiver[] caregiversArr;
        Location[] knownLocations;
        Location closestKnownLocation;
        GeoCoordinate currentLoc;
        Sample latestSample;
        DateTimeOffset sampleTime;
        public static readonly DateTimeOffset emergencyTimeRangeSTART = new DateTimeOffset(new DateTime(0, 0, 0, 1, 0, 0));
        public static readonly DateTimeOffset emergencyTimeRangeEND = new DateTimeOffset(new DateTime(0, 0, 0, 6, 0, 0));
        double AVG_PATIENT_HR;
        public static double HEART_RATE_BOTTOM_LIMIT;
        public static double HEART_RATE_TOP_LIMIT;
        public static int bottomNormalTimeRange;
        public static int topNormalTimeRange;
        public static double avgNormalTimeRange;



        /// Step A - PREPROCESS
        public void preprocessAlgoData(string currentPatientID)
        {
            /// 1. Get latest sample
            SampleController sampleController = new SampleController();
            latestSample = sampleController.GetLatestSampleForPatient(currentPatientID);

            //insert caregivers to caregiversArr
            PatientController patientController = new PatientController();
            caregiversArr = patientController.GetCaregiversforPatientID(currentPatientID);

            //extract patient's known locations
            LocationController locationController = new LocationController();
            knownLocations = locationController.GetKnownLocationsforPatientID(currentPatientID);

            //set currentLoc and closestKnowLocation
            currentLoc = new GeoCoordinate(latestSample.Latitude, latestSample.Longitude);
            closestKnownLocation = AlgoUtils.closestKnownLocation(currentLoc, knownLocations);

            //get latest sample time
            sampleTime = latestSample.CreatedAt.Value;

            //get avg patient's HR, and set our limits
            AVG_PATIENT_HR = AlgoUtils.avgHeartRate(currentPatientID);
            HEART_RATE_BOTTOM_LIMIT = 1.7 * AVG_PATIENT_HR;
            HEART_RATE_TOP_LIMIT = 0.5 * AVG_PATIENT_HR;

        }


        /// Step B - MONITORING
        public AlgoUtils.Status monitorAndAlert(string currentPatientID)
        {
            //***************** SETUP *****************//
            bool lessThanHalf = false;
            bool lessThan2 = false;
            bool lessThan5 = false;
            bool lessThan10 = false;

            double minDistToKnownLocation = AlgoUtils.calcDist(currentLoc, new GeoCoordinate(closestKnownLocation.Latitude, closestKnownLocation.Longitude));

            //get normal time range parameters for closest known location
            int[] timeRangeParameters = AlgoUtils.getSafeTimeRangeForLocation(closestKnownLocation, currentPatientID);
            bottomNormalTimeRange = timeRangeParameters[0];
            topNormalTimeRange = timeRangeParameters[1];
            avgNormalTimeRange = timeRangeParameters[2];

            if (minDistToKnownLocation <= 10) lessThan10 = true;
            if (minDistToKnownLocation <= 5) lessThan5 = true;
            if (minDistToKnownLocation <= 2) lessThan2 = true;
            if (minDistToKnownLocation <= 0.5) lessThanHalf = true;

            //*****************************************//


            if (lessThanHalf) //patient is very close to a known location
            {
                //time doesn't matter, just make sure heartRate is okay
                if (AlgoUtils.isHeartRateInSafeRange(latestSample.HeartRate))
                {
                    return AlgoUtils.Status.Safety;
                }
                else
                {
                    return AlgoUtils.Status.Distress; //potential distress or heart issues
                }
            }
            //check if it's an emergency time period
            else if (AlgoUtils.isTimeInSafeRange(sampleTime.Hour) == false)
            {
                //patient is not close enough to a known location + it's currently the EMERGENCY TIME PERIOD
                return AlgoUtils.Status.Risk;
            }

            else if (lessThan2 || AlgoUtils.heatMapAreaDensityLevel(currentLoc) == AlgoUtils.HeatMapDensity.High)
            {
                //patient is somewhat close to a known location, not in a dangerous time
                if (AlgoUtils.isHeartRateInSafeRange(latestSample.HeartRate))
                {
                    if (AlgoUtils.isTimeInNormalRange(sampleTime.Hour) == false)
                    {
                        return AlgoUtils.Status.Safety;
                    }
                    else
                    {
                        return AlgoUtils.Status.Wandering;
                    }
                }
                else
                {
                    return AlgoUtils.Status.Distress; //potential distress or heart issues
                }


            }
            else if (lessThan5 || AlgoUtils.heatMapAreaDensityLevel(currentLoc) == AlgoUtils.HeatMapDensity.Medium)
            {
                //patient is not too far from a known location
                if (AlgoUtils.isHeartRateInSafeRange(latestSample.HeartRate))
                {
                    if (AlgoUtils.isTimeInNormalRange(sampleTime.Hour) == false
                        || (sampleTime.Hour < avgNormalTimeRange*0.6 || (sampleTime.Hour > avgNormalTimeRange * 1.4)))
                    {
                        return AlgoUtils.Status.Wandering;
                    }
                }
                else
                {
                    return AlgoUtils.Status.Distress; //potential distress or heart issues
                }

            }
            else if (lessThan10 || AlgoUtils.heatMapAreaDensityLevel(currentLoc) == AlgoUtils.HeatMapDensity.Low)
            {
                //patient is somewhat far from a known location
                if (AlgoUtils.isHeartRateInSafeRange(latestSample.HeartRate))
                {
                    return AlgoUtils.Status.Wandering;
                }
                else
                {
                    return AlgoUtils.Status.Distress; //potential distress or heart issues
                }

            }
            
            //patient is very far from a known location, and in probably ZERO-Density area in heat map
            //patient is possibly lost!
            return AlgoUtils.Status.Risk;


            /// 2.4 Default: sample.location>10km from known location     OR in never visited before poligon on heatmap
            ///     2.4.1 send PUSH to caregiver - "Your beloved John is at (sample.location) area - do you know it?"
            ///         2.4.1.1 if caregiver knows it then let him add location as known location (with desc. etc.)
            ///         2.4.1.2 else if caregiver marks patient as wandering then enter POSSIBLE_RISK_MODE

        }
        /// **notice: T1>T2>T3**


        


        ///////////////TO BE CALLED FROM SERVER//////////////
        public void wanderingDetectionAlgo(string currentPatientID)
        {
            preprocessAlgoData(currentPatientID); //update latest sample for our patient & his caregiversArr
            AlgoUtils.Status patientStatus = monitorAndAlert(currentPatientID); //main business logic

            //Call2Action
            switch (patientStatus)
            {
                case AlgoUtils.Status.Safety:
                    {
                        //TODO:SAFETY stuff
                        break;
                    }

                case AlgoUtils.Status.Wandering:
                    {
                        //TODO:WANDERING stuff
                        /// Step C - POSSIBLE_WANDERING_MODE
                        /// 1. send PUSH to caregiver - "Do you know your beloved John is currently at (sample.location)?
                        ///                              It's a strange time for him to be there"
                        ///     1.1 if caregiver knows then
                        ///         1.1.1 add sample.time to sample.location (if it's not happening automatically)
                        ///         1.1.2 enter MONITORING_MODE
                        ///     1.2 else if caregiver marks patient as wandering then enter POSSIBLE_RISK_MODE 
                        break;
                    }

                case AlgoUtils.Status.Distress:
                    {
                        //TODO:DISTRESS stuff
                        /// Step D - POSSIBLE_DISTRESS_MODE
                        /// 1. send PUSH to caregiver - "Your beloved John is currently at (sample.location)?
                        ///                              His heartrate is a bit abnormal - please contact him"
                        /// 2. after 2 minutes send PUSH to caregiver - "Is your beloved John okay?" 
                        ///     1.1 if caregiver says he's okay, enter MONITORING_MODE
                        ///     1.2 else enter POSSIBLE_RISK_MODE 
                        break;
                    }

                case AlgoUtils.Status.Risk:
                    {
                        //TODO:RISK stuff
                        /// Step E - POSSIBLE_RISK_MODE
                        /// 1. Speed up sample rate (?) - every 1-3 minutes
                        /// 2. Notify all patient's caregivers
                        /// 3. Speak to patient (?)
                        /// 4. Open patient's microphone and transmit to caregivers
                        /// 5. Make alarm sounds - "I need help" - so passbyers can quickly assist (?)
                        /// 6. When caregiver hits "I'm safe" button - enter MONITORING_MODE (can also mark I'm safe remotely?)
                        break;
                    }
            }
        }
    }
}