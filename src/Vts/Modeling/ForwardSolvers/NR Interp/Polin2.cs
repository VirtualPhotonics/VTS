using System;

namespace NumericalRecipes.InterpolationAndExtrapolation
{
	/// <summary>
	/// Given arrays x1a[0..m-1] and x2a[0..n-1] of independent variables, and a submatrix of function
	/// values ya[0..m-1,0..n-1], tabulated at the grid points defined by x1a and x2a; and given values
	/// x1 and x2 of the independent variables; this routine returns an interpolated function value y,
	/// and an accuracy indication dy (based only on the interpolation in the x1 direction, however).
	/// </summary>
	public class Polin2
	{
		public Polin2()
		{
		}
		private double y;
		private double dy;
		public double Y
		{
			get{return y;}
		}
		public double Error
		{
			get{return dy;}
		}
		public void polin2(double[] x1a, double[] x2a, double[,] ya, int m, int n, double x1, double x2)
		{
			NumericalRecipes.InterpolationAndExtrapolation.Polint obj
				= new NumericalRecipes.InterpolationAndExtrapolation.Polint();
			int j,k;
			double[] ymtmp = new Double[m];
			double[] yah = new Double[n];

			for (j=0;j<m;j++) 
			{
				for(k = 0; k < n; k++) yah[k] = ya[j,k];
				obj.polint(x2a,yah,n,x2);
				ymtmp[j] = obj.Y;
			}
			obj.polint(x1a,ymtmp,m,x1);
			y = obj.Y;
			dy = obj.Error;
		}  
	}
}
/*
        private void button1_Click(object sender, System.EventArgs e)
		{
			double[] x1a = new Double[3];
			double[] x2a = new Double[3];
			double[,] ya = new Double[3,3];
			
			x1a[0]=1.0;
			x1a[1]=2.0;
			x1a[2]=7.0;
			
			x2a[0]=1.0;
			x2a[1]=2.0;
			x2a[2]=7.0;
			
			ya[0,0]=1.0;
			ya[0,1]=2.0;
			ya[0,2]=7.0;
			
			ya[1,0]=1.0;
			ya[1,1]=2.0;
			ya[1,2]=7.0;
			
			ya[2,0]=1.0;
			ya[2,1]=2.0;
			ya[2,2]=7.0;
			
			
			NR.InterpolationAndExtrapolation.Polin2 bd = new NR.InterpolationAndExtrapolation.Polin2();
			bd.polin2(x1a,x2a,ya,3,3,0.5,0.5);

			textBox1.Text = Convert.ToString(bd.Y)+"/"+Convert.ToString(bd.Error);
		}
// result
0.5/0
*/