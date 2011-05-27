using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Fem.MGRTE._2D.DataStructures;

namespace Vts.Fem.MGRTE._2D
{
    public static class MathFunctions
    {

        public static double Area(double x1, double y1, double x2, double y2, double x3, double y3)
        // Purpose: Calculate the area of a triangle
        {
            return 0.5 * Math.Abs((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1));
        }

        public static int FindMin(int n, double[] d)
        // Purpose: this function is to find the minimum from the vector d with the size n.
        {
            int i;
            double dmin;
            int nmin;
            nmin = 0; dmin = d[nmin];
            for (i = 1; i < n; i++)
            {
                if (d[i] < dmin)
                { nmin = i; dmin = d[i]; }
            }
            return nmin;
        }


        public static void Intepolation_a(double theta_m, double dtheta, int ns, int[] b, double[] b2, double constant)
        // Purpose: this function is to find two linearly intepolated angles "b" and weights "b2" for the angle "theta_m"
        {
            int theta1, theta2;
            double w1, w2;

            theta1 = (int)Math.Floor(theta_m / dtheta) + 1;
            w2 = (theta_m - (theta1 - 1) * dtheta) / dtheta;
            w1 = 1.0 - w2;
            if (theta1 == ns)
            { theta2 = 1; }
            else
            { theta2 = theta1 + 1; }
            b[0] = theta1 - 1; b[1] = theta2 - 1;
            b2[0] = w1 * constant; b2[1] = w2 * constant;

        }

        public static double Length(double x1, double y1, double x2, double y2)
        //Purpose: this function calculate the distance between two points
        {
            return Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
        }


    }
}
