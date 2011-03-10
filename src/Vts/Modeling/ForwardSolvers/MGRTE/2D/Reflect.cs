using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Modeling.ForwardSolvers.MGRTE._2D.DataStructures;

namespace Vts.Modeling.ForwardSolvers.MGRTE._2D
{
    class Reflect
    {
        public double Pi = GlobalConstants.Pi;

        public void BoundReflection(int ns,double[][] theta,SpatialMesh smesh,double index_i,double index_o, BoundaryCoupling b)
        {

            // Purpose: this fucntion is to find the coupling relation between directions on the boundary
            //          due to reflection and refraction in the presence of refraction index mismatch at the boundary.
            //          For the data structure of "b", see "struct boundarycoupling" in "solver".

            int i,j;
            int [] sign = new int [2] {-1,1};
            double dx,dy,sn,dtheta=2*Pi/ns, ratio_reflection,ratio_refraction;
            double[] temp = new double[2];
            double theta0,theta_i,theta_m,theta_m2;            

            for(i=0;i<smesh.ne;i++)
            {
                dx = smesh.p[smesh.e[i][1]][0] - smesh.p[smesh.e[i][2]][0];
                dy = smesh.p[smesh.e[i][1]][1] - smesh.p[smesh.e[i][2]][1];
                if (smesh.ori[i] == 0)
                { dx = -dx; dy = -dy; }// to make sure that (dx,dy) goes clockwisely.
                for(j=0;j<ns;j++)
                {
                    sn = theta[j][0] * smesh.n[i][0] + theta[j][1] * smesh.n[i][1];// "s" dot "n" for the angle "s"
                    if (sn<0)
                    {
                        theta0=Pi-Math.Acos(sn);
                        ratio_reflection = Reflection(theta0,index_i,index_o);
                        Refraction(temp,theta0,index_o,index_i);
                        ratio_refraction=temp[0];theta_i=temp[1];

                        if (theta[j][0] * dx + theta[j][1] * dy > 0)// the ONLY place for clockwise (dx,dy)
                        {
                            theta_m  = Mod2pi(theta[j][2] + (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] + (theta_i - theta0));
                        }
                        else
                        {
                            theta_m  = Mod2pi(theta[j][2] - (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] - (theta_i - theta0));
                        }
                        // contribution to incoming flux through internal reflection
                        Intepolation_a(theta_m,dtheta,ns,b.ri[i][j],b.ri2[i][j],ratio_reflection);
                        // contribution from boundary source to incoming flux through refraction
                        Intepolation_a(theta_m2,dtheta,ns,b.si[i][j],b.si2[i][j],ratio_refraction);
                    }
                    else
                    {
                        theta0=Math.Acos(sn);
                        ratio_reflection=Reflection(theta0,index_o,index_i);
                        Refraction(temp,theta0,index_i,index_o);
                        ratio_refraction=temp[0];theta_i=temp[1];

                        if (theta[j][0] * dx + theta[j][1] * dy > 0)// the ONLY place for clockwise (dx,dy)
                        {
                            theta_m  = Mod2pi(theta[j][2] - (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] - (theta_i - theta0));
                        }
                        else
                        {
                            theta_m  = Mod2pi(theta[j][2] + (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] + (theta_i - theta0));
                        }
                        // contribution from boundary source to outgoing flux through reflection
                        Intepolation_a(theta_m,dtheta,ns,b.ro[i][j],b.ro2[i][j],ratio_reflection);
                        // contribution to outgoing flux after refraction
                        Intepolation_a(theta_m2,dtheta,ns,b.so[i][j],b.so2[i][j],ratio_refraction);
                    }
                }
            }         
        }

        public double Reflection(double theta_i, double ni, double no)
        // Purpose: this function is to find the reflection energy ratio for the reflected angle "theta_i" by tracing-back computation.
        {
            double r, theta_t, temp1, temp2;
            if (Math.Abs(theta_i) < 1e-6)
            {
                temp1 = (ni - no) / (ni + no);
                r = temp1 * temp1;
            }
            else
            {
                temp1 = Math.Sin(theta_i) * ni / no;
                if (temp1 < 1)
                {
                    theta_t = Math.Asin(temp1);
                    temp1 = Math.Sin(theta_i - theta_t) / Math.Sin(theta_i + theta_t);
                    temp2 = Math.Tan(theta_i - theta_t) / Math.Tan(theta_i + theta_t);
                    r = 0.5 * (temp1 * temp1 + temp2 * temp2);
                }
                else
                {
                    r = 1;
                }
            }
            return r;
        }


        public void Refraction(double []temp,double theta_r,double ni,double no)
        // Purpose: this function is to find the refraction energy ratio for the refracted angle "theta_r" by tracing-back computation.
        {
            double r,theta_i,temp1,temp2;
            if (Math.Abs(theta_r) < 1e-6)
            {
                temp1=(ni-no)/(ni+no);
                r=1-temp1*temp1;
                theta_i=0;
            }
            else
            {
                temp1 = Math.Sin(theta_r) * ni / no;
                if(temp1<1)
                {
                    theta_i = Math.Asin(temp1);
                    temp1 = Math.Sin(theta_i - theta_r) / Math.Sin(theta_i + theta_r);
                    temp2 = Math.Tan(theta_i - theta_r) / Math.Tan(theta_i + theta_r);
                    r=1-0.5*(temp1*temp1+temp2*temp2);
                }
                else
                {
                    theta_i=Pi/2;
                    r=0;
                }
            }
            temp[0]=r;temp[1]=theta_i;
        }

        public void Intepolation_a(double theta_m,double dtheta,int ns,int[]b, double[]b2,double constant)
        // Purpose: this function is to find two linearly intepolated angles "b" and weights "b2" for the angle "theta_m"
        {
            int theta1,theta2;
            double w1,w2;

            theta1=(int)Math.Floor(theta_m/dtheta)+1;
            w2=(theta_m-(theta1-1)*dtheta)/dtheta;
            w1=1.0-w2;
            if(theta1==ns)
            {theta2=1;}
            else
            {theta2=theta1+1;}
            b[0]=theta1-1;b[1]=theta2-1;
            b2[0]=w1*constant;b2[1]=w2*constant;
            
        }

        private double Mod2pi(double x)
        // Purpose: this function is to transfer angle "x" into [0 2*Pi)
        {   double y;
            if (x<2*Pi)
            {   if(x<0)
                {
                    y=x+2*Pi;
                }
                else
                {
                    y=x;
                }
            }
            else
            {
                y=x-2*Pi;
            }
            return y;
        }
        
    }
}
