using System;

namespace NumericalRecipes.InterpolationAndExtrapolation
{
	/// <summary>
	/// Given x1a, x2a, ya, m, n as described in splie2 and y2a as produced by that routine, and
	/// given a desired interpolating point x1,x2; this routine returns an interpolated function value y
	/// by bicubic spline interpolation.
	/// </summary>
	public class Splin2
	{
		public Splin2()
		{
		}
		private double y;
		public double Y
		{
			get{return y;}
		}
		public void splin2(double[] x1a, double[] x2a, double[,] ya, double[,] y2a,
			int m, int n, double x1, double x2)
		{
			NumericalRecipes.InterpolationAndExtrapolation.Spline obj
				= new NumericalRecipes.InterpolationAndExtrapolation.Spline();
			NumericalRecipes.InterpolationAndExtrapolation.Splint obj1
				= new NumericalRecipes.InterpolationAndExtrapolation.Splint();

			int j,k;
			double[] yah = new Double[n];
			double[] y2ah = new Double[n];
			double[] ytmp = new Double[n];
			double[] yytmp = new Double[n];

			for (j=0;j<m;j++)
			{
				for(k = 0; k < n; k++) 
				{
					yah[k] = ya[j,k];
					y2ah[k] = y2a[j,k];
				}
				obj1.splint(x2a,yah,y2ah,n,x2);
				yytmp[j]=obj1.Y;
			}
			obj.spline(x1a,yytmp,m,1.0e30,1.0e30,ytmp);
			obj1.splint(x1a,yytmp,ytmp,m,x1);
			y=obj1.Y;
    	}     
	}
}
/*
        private void button1_Click(object sender, System.EventArgs e)
		{
			double[] x1a = new Double[4];
			double[] x2a = new Double[4];
			double[,] ya = new Double[4,4];
			double[,] y2a = new Double[4,4];
               					
			x1a[0]=0.0;
			x1a[1]=1.0;
			x1a[2]=2.0;
			x1a[3]=3.0;

			x2a[0]=0.0;
			x2a[1]=1.0;
			x2a[2]=2.0;
			x2a[3]=3.0;

			for(int i = 0 ; i < 4; i++)
				for(int j = 0 ; j < 4; j++)
					ya[i,j] = Convert.ToDouble(i*i+j*j*j);
			
			NR.InterpolationAndExtrapolation.Splie2 obj = 
				new NR.InterpolationAndExtrapolation.Splie2();

			NR.InterpolationAndExtrapolation.Splin2 obj1 = 
				new NR.InterpolationAndExtrapolation.Splin2();

			
			obj.splie2(x1a,x2a,ya,4,4,y2a);
			obj1.splin2(x1a,x2a,ya,y2a,4,4,2.0,5.0);

			textBox1.Text += Convert.ToString(obj1.Y) + "\r\n";
		} 
// result
52.2
*/