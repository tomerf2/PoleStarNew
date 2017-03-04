using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace PoleStar.Utils
{
    public class HeatMap
    {
        //private static Dictionary<double, Color> mItems = new Dictionary<double, Color>();

        public HeatMap()
        {
            //mItems.Add(4.22, Colors.LightBlue);
            //mItems.Add(10.94, Colors.Blue);
        }

        /*public static Color GetColorByDistance(double dist)
        {
            double bottomBound = 0.0;

            foreach(var item in mItems)
            {
                if (dist > bottomBound && dist < item.Key)
                    return item.Value;

                bottomBound = item.Key;
            }

            return Colors.Transparent;
        }*/

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
    }
}
