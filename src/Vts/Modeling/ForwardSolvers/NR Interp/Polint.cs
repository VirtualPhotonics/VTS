using System;

namespace NumericalRecipes.InterpolationAndExtrapolation
{
	/// <summary>
	/// Given arrays xa[0..n-1] and ya[0..n-1], and given a value x, this routine returns a value y, and
	/// an error estimate dy. If P(x) is the polynomial of degree n - 1 such that P(xai) = yai, i =
	/// 0,..., n-1, then the returned value y = P(x).
	/// </summary>
	public class Polint
	{

		private double y;
		private double dy;
		public Polint()
		{
		}
		public double Y  // output
		{
			get{return y;}
		}
		public double Error  // output
		{
			get{return dy;}
		}
		public void polint(double[] xa, double[] ya, int n, double x)   // input 
		{
			int i,m,ns=0;
			double den,dif,dift,ho,hp,w;
			double[] c = new Double[n];
			double[] d = new Double[n];
			dif=Math.Abs(x-xa[0]);

			for (i=0;i<n;i++) 
			{
                if ( (dift=Math.Abs(x-xa[i])) < dif) 
				{
                     ns=i;
                     dif=dift;
                }
                c[i]=ya[i];  
                d[i]=ya[i];
            }
            y=ya[ns--];     
            for (m=0;m<n-1;m++)
			{
                for (i=0;i<n-m-1;i++)
                { 
                     ho=xa[i]-x;
                     hp=xa[i+m+1]-x;
                     w=c[i+1]-d[i];
				     if( (den=ho-hp) == 0.0) 
						try{throw new Exception();}
						catch (Exception)
						{
                            //MessageBox.Show("Error in routine polint",
                            //"Invalid method",MessageBoxButtons.OK, MessageBoxIcon.Error );
						}
                     den=w/den;
                     d[i]=hp*den; 
                     c[i]=ho*den;
                }
                y += (dy=(2*(ns+1) < (n-m-1) ? c[ns+1] : d[ns--]));
            }
		}
	}
}
/*
        private void button1_Click(object sender, System.EventArgs e)
		{
			double[] xa = new Double[5];
			double[] ya = new Double[5];

			xa[0]=1.0;
			xa[1]=2.0;
			xa[2]=3.0;
			xa[3]=4.0;
			xa[4]=5.0;
			ya[0]=1.8;
			ya[1]=2.27;
			ya[2]=3.18;
			ya[3]=4.01;
			ya[4]=4.91;
			
			NR.InterpolationAndExtrapolation.Polint bd = new NR.InterpolationAndExtrapolation.Polint();
			bd.polint(xa,ya,5,2.25);

			textBox1.Text = Convert.ToString(bd.Y)+"/"+Convert.ToString(bd.Error);
		}
// result
2.4880126953125/0.0114501953125
*/