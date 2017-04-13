using PoleStar.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Shapes;

namespace PoleStar.Utils
{
    public class HeatMap
    {
        public static Color GetColorByDensity(float value, float maxVal)
        {
            if (maxVal == 0) maxVal = 1;
            float val = value / maxVal; // Assuming that range starts from 0
            Color color = new Color();

            color.A = 255;

            if (val > 1)
                val = 1;
            if (val > 0.5f)
            {
                val = (val - 0.5f) * 2;
                color.R = Convert.ToByte(255 * val);
                color.G = Convert.ToByte(255 * (1 - val));
                color.B = 0;
            }
            else
            {
                val = val * 2;
                color.R = 0;
                color.G = Convert.ToByte(255 * val);
                color.B = Convert.ToByte(255 * (1 - val));
            }

            return color;
        }

        public static List<Sample> ConvexHull(List<Sample> sampleGroup)
        {
            sampleGroup.Sort(delegate (Sample a, Sample b)
            {
                int xdiff = a.Latitude.CompareTo(b.Latitude);
                if (xdiff != 0) return xdiff;
                else return a.Longitude.CompareTo(b.Longitude);
            });

            List<Sample> lower = new List<Sample>();
            for(int i = 0; i < sampleGroup.Count; i++)
            {
                while (lower.Count >= 2 && CHCross(lower[lower.Count - 2], lower[lower.Count - 1], sampleGroup[i]) <= 0)
                    lower.RemoveAt(lower.Count - 1);

                lower.Add(sampleGroup[i]);
            }

            List<Sample> upper = new List<Sample>();
            for (int i = sampleGroup.Count - 1; i >= 0; i--)
            {
                while (upper.Count >= 2 && CHCross(upper[upper.Count - 2], upper[upper.Count - 1], sampleGroup[i]) <= 0)
                    upper.RemoveAt(upper.Count - 1);

                upper.Add(sampleGroup[i]);
            }

            upper.RemoveAt(upper.Count - 1);
            lower.RemoveAt(lower.Count - 1);

            return lower.Concat(upper).ToList();
        }

        private static float CHCross(Sample a, Sample b, Sample o)
        {
            return (a.Latitude - o.Latitude) * (b.Longitude - o.Longitude) - (a.Longitude - o.Longitude) * (b.Latitude - o.Latitude);
        }
    }
}
