using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.DataObjects;

namespace Server.Utils
{
    public class DetectionAlgo
    {
        /// Pseudo Code
        /// Step A - PREPROCESS
        /// 0.1 load all data on patient - static vars
        /// 0.2 load caregivers (?) - static vars
        ///
        /// 
        /// Step B - MONITORING_MODE
        /// 1. Get latest sample
        /// 2. Switch (sample.location)
        /// 2.1 Case <in radius<2km from previously visited location>:  OR in RED poligon on heatmap
        ///     2.1.1 if (sample.time) is standard (in range T1 of previous visits) then
        ///         2.1.1.1 if (sample.heartrate) is standard (range of X s.d from avg. heartrate) then OKAY
        ///         2.1.1.2 else POSSIBLE_DISTRESS_MODE
        ///     2.1.2 else enter POSSIBLE_WANDERING_MODE
        /// 2.2 Case <in radius<5km from previously visited location>:  OR in YELLOW poligon on heatmap
        ///     2.2.1 if (sample.time) is standard (in range T2 of previous visits) then
        ///         2.2.1.1 if (sample.heartrate) is standard (range of X s.d from avg. heartrate) then OKAY
        ///         2.2.1.2 else POSSIBLE_DISTRESS_MODE
        ///     2.2.2 else enter POSSIBLE_WANDERING_MODE
        /// 2.3 Case <in radius<10km from previously visited location>: OR in GREEN poligon on heatmap
        ///     2.3.1 if (sample.time) is standard (in range T3 of previous visits) then
        ///         2.3.1.1 if (sample.heartrate) is standard (range of X s.d from avg. heartrate) then OKAY
        ///         2.3.1.2 else POSSIBLE_DISTRESS_MODE
        ///     2.3.2 else enter POSSIBLE_WANDERING_MODE
        /// 2.4 Default: sample.location>10km from known location     OR in never visited before poligon on heatmap
        ///     2.4.1 send PUSH to caregiver - "Your beloved John is at (sample.location) area - do you know it?"
        ///         2.4.1.1 if caregiver knows it then let him add location as known location (with desc. etc.)
        ///         2.4.1.2 else if caregiver marks patient as wandering then enter POSSIBLE_RISK_MODE
        ///
        /// 
        /// **notice: T1>T2>T3**
        /// Step C - POSSIBLE_WANDERING_MODE
        /// 1. send PUSH to caregiver - "Do you know your beloved John is currently at (sample.location)?
        ///                              It's a strange time for him to be there"
        ///     1.1 if caregiver knows then
        ///         1.1.1 add sample.time to sample.location (if it's not happening automatically)
        ///         1.1.2 enter MONITORING_MODE
        ///     1.2 else if caregiver marks patient as wandering then enter POSSIBLE_RISK_MODE 
        ///
        ///
        /// Step D - POSSIBLE_DISTRESS_MODE
        /// 1. send PUSH to caregiver - "Your beloved John is currently at (sample.location)?
        ///                              His heartrate is a bit abnormal - please contact him"
        /// 2. after 2 minutes send PUSH to caregiver - "Is your beloved John okay?" 
        ///     1.1 if caregiver says he's okay, enter MONITORING_MODE
        ///     1.2 else enter POSSIBLE_RISK_MODE 
        ///
        /// 
        /// Step E - POSSIBLE_RISK_MODE
        /// 1. Speed up sample rate (?) - every 1-3 minutes
        /// 2. Notify all patient's caregivers
        /// 3. Speak to patient (?)
        /// 4. Open patient's microphone and transmit to caregivers
        /// 5. Make alarm sounds - "I need help" - so passbyers can quickly assist (?)
        /// 6. When caregiver hits "I'm safe" button - enter MONITORING_MODE (can also mark I'm safe remotely?)




        public static double DistanceTo(Sample baseCoordinates, Sample targetCoordinates)
        {
            var baseRad = Math.PI * baseCoordinates.Latitude / 180;
            var targetRad = Math.PI * targetCoordinates.Latitude / 180;
            var theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            var thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.34;  //Convert miles to meters
        }

    
}
}