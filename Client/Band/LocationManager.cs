using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace PoleStar.Band
{
    class LocationManager
    {
        public async static Task<Geoposition> GetPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync(); //request access
            if (accessStatus != GeolocationAccessStatus.Allowed)
            {
                throw new LocationPermissionException("Access to Location Services Required");
            }

            var geolocator = new Geolocator { DesiredAccuracyInMeters = 0 }; //0 means give what ever you got
            var position = await geolocator.GetGeopositionAsync();
            return position;
        }
    }
}

public class LocationPermissionException : Exception
{
    public LocationPermissionException(string message) : base(message)
    {
    }
}