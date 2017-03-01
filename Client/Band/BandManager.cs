using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using PoleStar.Utils;

namespace PoleStar.Band
{
    class BandManager
    {
        IBandInfo[] pairedBands;
        public IBandClient bandClient;

        public async Task BandInit()
        {
            try
            {
                pairedBands = await BandClientManager.Instance.GetBandsAsync();
                bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]); //connect

                // Check if the current user has given consent to the collection of heart rate sensor data.
                if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() !=
                UserConsent.Granted)
                {
                    // We don't have user consent, so let's request it.
                    await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                }
            }
            catch (BandException e1)
            {
                DialogBox.ShowOk("Error", "Not connected to Microsoft Band. Please verfiy connection and restart app.");
            }
            catch (IndexOutOfRangeException e1)
            {
                //TODO - No Band
            }
            catch (Exception e3)
            {
                DialogBox.ShowOk("Error", "Not connected to Microsoft Band. Please verfiy connection and restart app.");
            }

        }

        public async Task GetSensorReadings(Measurements measurements, int measuretime, bool worn, bool heart, bool steps, bool distance)
        {
            try
            {

                if (worn)
                {
                    bandClient.SensorManager.Contact.ReadingChanged += (s, args) =>
                    {
                        measurements.Worn = args.SensorReading.State;
                    };
                    await bandClient.SensorManager.Contact.StartReadingsAsync();
                }

                if (heart)
                {
                    // check current user heart rate consent
                    if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() !=
                    UserConsent.Granted)
                    {
                        // user hasn’t consented, request consent
                        await
                        bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
                    }
                    bandClient.SensorManager.HeartRate.ReadingChanged += (s, args) =>
                    {
                        measurements.Heartrate = args.SensorReading.HeartRate;
                        measurements.Quality = args.SensorReading.Quality;
                    };
                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                }

                if (steps)
                {
                    bandClient.SensorManager.Pedometer.ReadingChanged += (s, args) =>
                    {
                        measurements.Steps = args.SensorReading.TotalSteps;
                    };
                    await bandClient.SensorManager.Pedometer.StartReadingsAsync();
                }

                if (distance)
                {
                    bandClient.SensorManager.Distance.ReadingChanged += (s, args) =>
                    {
                        measurements.Pace = args.SensorReading.Pace;
                        measurements.Distance = args.SensorReading.TotalDistance;
                    };
                    await bandClient.SensorManager.Distance.StartReadingsAsync();
                }

                //Wait for readings
                await Task.Delay(TimeSpan.FromSeconds(measuretime));

                //Stop all readings
                await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                await bandClient.SensorManager.Contact.StopReadingsAsync();
                //await bandClient.SensorManager.Pedometer.StopReadingsAsync();
                //await bandClient.SensorManager.Distance.StopReadingsAsync();
            }

            catch (BandException e1)
            {
                //TODO
            }
        }
    }
}
