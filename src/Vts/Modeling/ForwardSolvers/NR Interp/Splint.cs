using System;

namespace NumericalRecipes.InterpolationAndExtrapolation
{
	/// <summary>
	/// Given the arrays xa[0..n-1] and ya[0..n-1], which tabulate a function (with the xa_i¡¯s in order),
	/// and given the array y2a[0..n-1], which is the output from spline above, and given a value of
	/// x, this routine returns a cubic-spline interpolated value y.
	/// </summary>
	public class Splint
	{
		private double y;
		public double Y
		{
			get{return y;}
		}
		public Splint()
		{
		}
        public void splint(double[] xa, double[] ya, double[] y2a, int n, double x)
		{
			int klo,khi,k;
			double h,b,a;

			klo=0;
			khi=n-1;
			while (khi-klo > 1) 
			{
				k=((khi+klo+2) >> 1) - 1;
				if (xa[k] > x) khi=k;
				else klo=k;
			}
			h=xa[khi]-xa[klo];
			if (h == 0.0) try{throw new Exception();}
						  catch (Exception)
						  {
                              //MessageBox.Show("Bad xa input to routine splint",
                              //    "Invalid method",MessageBoxButtons.OK, MessageBoxIcon.Error );
						  }
			a=(xa[khi]-x)/h;
			b=(x-xa[klo])/h;
			y=a*ya[klo]+b*ya[khi]+((a*a*a-a)*y2a[klo]+(b*b*b-b)*y2a[khi])*(h*h)/6.0;
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
			v[2]=4.0;
			v[3]=9.0;
			
			NR.InterpolationAndExtrapolation.Spline ob = 
				new NR.InterpolationAndExtrapolation.Spline();
			
			ob.spline(u,v,4,0.0,6.0,y2);

			NR.InterpolationAndExtrapolation.Splint ob2 = 
				new NR.InterpolationAndExtrapolation.Splint();

			ob2.splint(u,v,y2,4,5.0);
			textBox1.Text += Convert.ToString(ob2.Y) + "\r\n";			 
    	}
// result
25
*/