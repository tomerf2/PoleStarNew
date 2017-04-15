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
using System.Diagnostics;
using Server.Hubs;
using System.Collections;

namespace Server.Utils
{
    class WanderingAlgo
    {
        /// 0.1 load all data on patient
        /// 0.2 load caregivers
        //public static Caregiver[] caregiversArr;
        public static IEnumerable<Caregiver> caregiversArr;
        public static Location[] knownLocations;
        public static Location closestKnownLocation;
        public static GeoCoordinate currentLoc;
        public static Sample latestSample;
        public static DateTimeOffset sampleTime;
        public static readonly int emergencyTimeRangeSTART = 1;
        public static readonly int emergencyTimeRangeEND = 6;
        public readonly int CONNECTION_LOST_TIME_DIF = 2;
        double AVG_PATIENT_HR;
        public static double HEART_RATE_BOTTOM_LIMIT;
        public static double HEART_RATE_TOP_LIMIT;
        public static int bottomNormalTimeRange;
        public static int topNormalTimeRange;
        public static double avgNormalTimeRange;
        public static string patientName;
        public static string patientID;
        public static NotificationHub notificationHub = new NotificationHub();



        /// Step A - PREPROCESS
        public void preprocessAlgoData(string currentPatientID)
        {
            Trace.TraceInformation(String.Format("Current PatientID is {0}", currentPatientID));

            /// 1. Get latest sample
            SampleController sampleController = new SampleController();
            latestSample = sampleController.GetLatestSampleForPatient(currentPatientID);
            Trace.TraceInformation(String.Format("Latest sample timestamp is {0}", latestSample.CreatedAt));

            //insert caregivers to caregiversArr
            PatientController patientController = new PatientController();
            caregiversArr = patientController.GetCaregiversforPatientID(currentPatientID);
            patientName = patientController.GetPatientName(currentPatientID);
            Trace.TraceInformation(String.Format("Patient name is {0}", patientName));
            Trace.TraceInformation(String.Format("Caregivers array first email is {0}", (caregiversArr.First()).Email));


            //extract patient's known locations
            LocationController locationController = new LocationController();
            knownLocations = locationController.GetKnownLocationsforPatientID(currentPatientID);
            Trace.TraceInformation(String.Format("First known location is {0}", knownLocations[0].Description));

            //set currentLoc and closestKnowLocation
            currentLoc = new GeoCoordinate(latestSample.Latitude, latestSample.Longitude);
            closestKnownLocation = AlgoUtils.closestKnownLocation(currentLoc, knownLocations);
            Trace.TraceInformation(String.Format("Current location is {0}", currentLoc.ToString()));
            Trace.TraceInformation(String.Format("Closest known location is {0}", closestKnownLocation.Description));

            //get latest sample time
            sampleTime = latestSample.CreatedAt.Value;
            Trace.TraceInformation(String.Format("Latest sample time is {0}", sampleTime));

            //get avg patient's HR, and set our limits
            AVG_PATIENT_HR = AlgoUtils.avgHeartRate(currentPatientID);
            HEART_RATE_BOTTOM_LIMIT = 1.7 * AVG_PATIENT_HR;
            HEART_RATE_TOP_LIMIT = 0.5 * AVG_PATIENT_HR;
            Trace.TraceInformation(String.Format("Avg patient heartrate is {0}", AVG_PATIENT_HR));
            Trace.TraceInformation(String.Format("BottomLimit patient heartrate is {0}", HEART_RATE_BOTTOM_LIMIT));
            Trace.TraceInformation(String.Format("TopLimit patient heartrate is {0}", HEART_RATE_TOP_LIMIT));



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
            Trace.TraceInformation(String.Format("Dist to closest known location is {0}", minDistToKnownLocation));

            //get normal time range parameters for closest known location
            int[] timeRangeParameters = AlgoUtils.getSafeTimeRangeForLocation(closestKnownLocation, currentPatientID);
            bottomNormalTimeRange = timeRangeParameters[0];
            topNormalTimeRange = timeRangeParameters[1];
            avgNormalTimeRange = timeRangeParameters[2];
            Trace.TraceInformation(String.Format("bottom normal time range is {0}", bottomNormalTimeRange));
            Trace.TraceInformation(String.Format("top normal time range is {0}", topNormalTimeRange));
            Trace.TraceInformation(String.Format("avg normal time range is {0}", avgNormalTimeRange));

            if (minDistToKnownLocation <= 10) lessThan10 = true;
            if (minDistToKnownLocation <= 5) lessThan5 = true;
            if (minDistToKnownLocation <= 2) lessThan2 = true;
            if (minDistToKnownLocation <= 0.5) lessThanHalf = true;

            //*****************************************//

            if ((DateTime.Now.Hour - sampleTime.Hour) > CONNECTION_LOST_TIME_DIF)
            {
                return AlgoUtils.Status.ConnectionLost;
            }


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
            Trace.AutoFlush = true;
            patientID = currentPatientID;
            preprocessAlgoData(currentPatientID); //update latest sample for our patient & his caregiversArr
            Trace.TraceInformation(String.Format("Preprocess stage is finished"));

            AlgoUtils.Status patientStatus = monitorAndAlert(currentPatientID); //main business logic
            Trace.TraceInformation(String.Format("Monitor and alert stage is done"));
            Trace.TraceInformation(String.Format("Patient status is {0}", patientStatus));


            //Call2Action
            switch (patientStatus)
            {
                case AlgoUtils.Status.Safety:
                    {
                        Trace.TraceInformation(String.Format("Patient is Safe, doing safe stuff"));
                        //TODO:SAFETY stuff
                        break;
                    }

                case AlgoUtils.Status.Wandering:
                    {
                        Trace.TraceInformation(String.Format("Patient is Wandering, doing wandering stuff"));
                        
                        AlgoUtils.sendWanderingNotification();
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
                        AlgoUtils.sendDistressNotification();
                        Trace.TraceInformation(String.Format("Patient is in Distress, doing distress stuff"));
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
                        AlgoUtils.sendRiskNotification();
                        Trace.TraceInformation(String.Format("Patient is at Risk, doing risk stuff"));
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
                case AlgoUtils.Status.ConnectionLost:
                    {
                        Trace.TraceInformation(String.Format("Patient has lost connection, doing connection lost stuff"));
                        //TODO: connectionLost stuff
                        break;
                    }
            }
        }
    }
}