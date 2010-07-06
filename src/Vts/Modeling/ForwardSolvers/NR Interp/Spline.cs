using System;

namespace NumericalRecipes.InterpolationAndExtrapolation
{
	/// <summary>
	/// Given arrays x[0..n-1] and y[0..n-1] containing a tabulated function, i.e., y_i = f(x_i), with
	/// x_0 less than x_1 less than ... less than x_n-1, and given values yp_0 and yp_n-1 for the first 
	/// derivative of the interpolating function at points 0 and n-1, respectively, this routine returns 
	/// an array y2[0..n-1] that contains the second derivatives of the interpolating function at the 
	/// tabulated points x_i. If yp_0 and/or yp_n-1 are equal to 1 \times 10^30 or larger, the routine is signaled 
	/// to set the corresponding boundary condition for a natural spline, with zero second derivative 
	/// on that boundary.
	/// </summary>
	public class Spline
	{
		public Spline()
		{
		}
        public void spline(double[] x, double[] y, int n, double yp1, double ypn, double[] y2)
		{
			int i,k;
			double p,qn,sig,un;
			double[] u = new Double[n-1];

			if (yp1 > 0.99e30)
				y2[0]=u[0]=0.0;
			else 
			{
				y2[0] = -0.5;
				u[0]=(3.0/(x[1]-x[0]))*((y[1]-y[0])/(x[1]-x[0])-yp1);
			}
			for (i=1;i<n-1;i++) 
			{
				sig=(x[i]-x[i-1])/(x[i+1]-x[i-1]);
				p=sig*y2[i-1]+2.0;
				y2[i]=(sig-1.0)/p;
				u[i]=(y[i+1]-y[i])/(x[i+1]-x[i]) - (y[i]-y[i-1])/(x[i]-x[i-1]);
				u[i]=(6.0*u[i]/(x[i+1]-x[i-1])-sig*u[i-1])/p;
			}
			if (ypn > 0.99e30)
				qn=un=0.0;
			else 
			{
				qn=0.5;
				un=(3.0/(x[n-1]-x[n-2]))*(ypn-(y[n-1]-y[n-2])/(x[n-1]-x[n-2]));
			}
			y2[n-1]=(un-qn*u[n-2])/(qn*y2[n-2]+1.0);
			for (k=n-2;k>=0;k--)
				y2[k]=y2[k]*y2[k+1]+u[k];
		}  
	}
}
/*
        private void button1_Click(object sender, System.EventArgs e)
		{
			double[] u = new Double[4];
			double[] v = new Double[4];
			double[] y2 = new Double[4];
               					
			u[0]=0.0;
			u[1]=1.0;
			u[2]=2.0;
			u[3]=3.0;

			v[0]=0.0;
			v[1]=1.0;
			v[2]=8.0;
			v[3]=27.0;
			
			NR.InterpolationAndExtrapolation.Spline ob = 
				new NR.InterpolationAndExtrapolation.Spline();
			
			ob.spline(u,v,4,0.0,27.0,y2);

			for(int i=0; i<4; i++)
			textBox1.Text += Convert.ToString(y2[i]) + "\r\n";			 
    	} 
// result
0
6
12
18
*/