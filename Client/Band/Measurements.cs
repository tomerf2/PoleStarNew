using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Microsoft.Band;
using Microsoft.Band.Sensors;

namespace PoleStar.Band
{
    class Measurements
    {
        //boolean fields that indicate what values are stored
        public bool has_loc = false;
        public bool has_heart = false;
        public bool has_distance = false;
        public bool has_worn = false;
        public bool has_steps = false;
        public bool has_pace = false;

        //data fields
        private DateTime timestamp;
        private Geoposition location;
        private BandContactState worn; //not worn=0, worn=1

        private long distance; //Distance traveled in cm since the Band was last factory-reset 
        private double pace; //Current pace of the Band in ms/m

        private int heartrate;
        private HeartRateQuality quality;

        private long steps; //Total number of steps taken since the Band was last factory-reset


        public async Task GetAllMeasurements(BandManager bandInstance)
        {
            try
            {
                Location = await LocationManager.GetPosition(); //location
                Timestamp = DateTime.Now; //timestamp
                await bandInstance.GetSensorReadings(this, 60, true, true, false, false);


                int test = 0;
            }
            catch (LocationPermissionException e1)
            {
                //TODO
            }
            catch (BandException e2)
            {
                //TODO
            }
        }


        public Geoposition Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
                has_loc = true;
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }

            set
            {
                timestamp = value;
            }
        }

        public int Heartrate
        {
            get
            {
                return heartrate;
            }

            set
            {
                heartrate = value;
                has_heart = true;
            }
        }

        public HeartRateQuality Quality
        {
            get
            {
                return quality;
            }

            set
            {
                quality = value;
            }
        }

        public BandContactState Worn
        {
            get
            {
                return worn;
            }

            set
            {
                worn = value;
                has_worn = true;
            }
        }

        public long Distance
        {
            get
            {
                return distance;
            }

            set
            {
                distance = value;
                has_distance = true;
            }
        }


        public long Steps
        {
            get
            {
                return steps;
            }

            set
            {
                steps = value;
                has_steps = true;
            }
        }

        public double Pace
        {
            get
            {
                return pace;
            }

            set
            {
                pace = value;
                has_pace = true;
            }
        }
    }
}
