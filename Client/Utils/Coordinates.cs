using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace PoleStar.Utils
{
    public static class Coordinates
    {
        public static double DistanceTo(this Sample baseCoordinates, Sample targetCoordinates)
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

        public static async Task<MapAddress> CoordsToAddress(double lat, double lon)
        {
            // The location to reverse geocode.
            BasicGeoposition location = new BasicGeoposition();
            location.Latitude = lat;
            location.Longitude = lon;
            Geopoint pointToReverseGeocode = new Geopoint(location);

            // Reverse geocode the specified geographic location.
            MapLocationFinderResult result =
                  await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            // If the query returns results, display the name of the town
            // contained in the address of the first result.
            if (result.Status == MapLocationFinderStatus.Success)
            {
                return result.Locations[0].Address;
            }

            return null;
        }
    }
}
