using System;
using Meta.Numerics;

namespace Vts.SpectralMapping
{
    /// <summary>
    /// This class implements the formulation by Craig F. Bohren, Donald R. Huffman
    /// "Absorption and Scattering of Light by Small Particles", Wiley Sci., 1983
    /// Additional information can be found at Oregon Medical Center web-page:
    /// http://omlc.ogi.edu/classroom/ece532/class3/mie.html
    /// </summary>
    public class MieScatterer : BindableObject, IScatterer
    {
        const int angles = 361;
        const int nAngles = 2 * angles - 1;
        const double dTheta = (Math.PI / 2) / (angles - 1); /* dtheta: .25 deg. (in radian) */
        //private double _Wavelength;
        private double _ParticleRadius;
        private double _ParticleRefractiveIndexMismatch;
        private double _MediumRefractiveIndexMismatch;
        private double _VolumeFraction;

        private MieScatteringParameters MieScattParams;

        private class MieScatteringParameters
        {
            public double[] Q { get; set; }
            public Complex[] S1 { get; set; }
            public Complex[] S2 { get; set; }
            public double[] S11 { get; set; }
            public double G { get; set; }
        }

        /// <summary>
        /// Creates a MieScatterer with the specified values for particle radius, particle refractive index and medium refractive index
        /// </summary>
        /// <param name="particleRadius">Particle radius</param>
        /// <param name="particleRefractiveIndex">Particle refractive index</param>
        /// <param name="mediumRefractiveIndex">Medium refractive index</param>
        public MieScatterer(
            double particleRadius,
            double particleRefractiveIndex,
            double mediumRefractiveIndex,
            double volumeFraction)
        {
            ParticleRadius = particleRadius;
            ParticleRefractiveIndexMismatch = particleRefractiveIndex;
            MediumRefractiveIndexMismatch = mediumRefractiveIndex;
            VolumeFraction = volumeFraction;
        }

        /// <summary>
        /// Creates a MieScatterer with the specified Mie scatterer type
        /// </summary>
        /// <param name="scattererType">The Mie scatterer type</param>
        public MieScatterer(MieScattererType scattererType)
        {
            switch (scattererType)
            {
                case MieScattererType.PolystyreneSphereSuspension:
                default:
                    ParticleRadius = 0.5;
                    ParticleRefractiveIndexMismatch = 1.4;
                    MediumRefractiveIndexMismatch = 1.0;
                    VolumeFraction = 0.01;
                    break;
            }
        }

        /// <summary>
        /// Creates a MieScatterer with a MieScattererType of PolystyreneSphereSuspension
        /// </summary>
        public MieScatterer()
            : this(MieScattererType.PolystyreneSphereSuspension)
        {
        }

        /// <summary>
        /// Scatterer type, set to Mie
        /// </summary>
        public ScatteringType ScattererType { get { return ScatteringType.Mie; } }

        /// <summary>
        /// Partice radius
        /// </summary>
        public double ParticleRadius
        {
            get { return _ParticleRadius; }

            set
            {
                _ParticleRadius = value;
                OnPropertyChanged("ParticleRadius");
            }
        }

        /// <summary>
        /// Particle refractive index mismatch
        /// </summary>
        public double ParticleRefractiveIndexMismatch
        {
            get { return _ParticleRefractiveIndexMismatch; }

            set
            {
                _ParticleRefractiveIndexMismatch = value;
                OnPropertyChanged("ParticleRefractiveIndexMismatch");
            }
        }

        /// <summary>
        /// Medium refractive index mismatch
        /// </summary>
        public double MediumRefractiveIndexMismatch
        {
            get { return _MediumRefractiveIndexMismatch; }
            set
            {
                _MediumRefractiveIndexMismatch = value;
                OnPropertyChanged("MediumRefractiveIndexMismatch");
            }
        }

        /// <summary>
        /// Volume fraction
        /// </summary>
        public double VolumeFraction
        {
            get { return _VolumeFraction; }
            set
            {
                _VolumeFraction = value;
            }
        }

        /// <summary>
        /// Returns the anisotropy coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The anisotropy coeffient g</returns>
        public double GetG(double wavelength)
        {
            MieScattParams = new MieScatteringParameters();
            BohrenHuffmanMie(wavelength);
            return MieScattParams.G;
        }

        /// <summary>
        /// Returns the scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The scattering coefficient Mus</returns>
        public double GetMus(double wavelength)
        {
            MieScattParams = new MieScatteringParameters();
            BohrenHuffmanMie(wavelength);
            double qSca = MieScattParams.Q[0];

            //Let a = particle radius;
            //fv = particle volume fraction;
            //rho_s = fv/((4/3)*Pi/a^3) = particle number density;
            //sigma_s = Qsca*A = cross-section
            //A = Pi*a^2 = area
            // Then mus = rho_s*sigma_s, which, upon simplifying, produces:
            // mus = (fv*Qsca)/((4/3)*a)
            double mus = VolumeFraction * qSca / ((4.0 / 3.0) * ParticleRadius*1e-3);  //radius in m
            return mus;
        }

        /// <summary>
        /// Returns the reduced scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            var mus = GetMus(wavelength);
            var g = GetG(wavelength);   //This call may not be required. GetMus calculate both mus and g. Fix later
            return mus * (1 - g);
        }

        private double GetSizeParameter(double wavelength)
        {
            return 1000 * 2.0 * Math.PI * ParticleRadius * MediumRefractiveIndexMismatch / wavelength;
        }

        private MieScatteringParameters BohrenHuffmanMie(double wavelength)
        {
            int rn;
            double[] amu = new double[angles];
            double[] theta = new double[angles];
            double[] tau = new double[angles];
            double[] pi =  new double[angles];
            double[] pi0 = new double[angles];
            double[] pi1 = new double[angles];
            Complex[] s1 = new Complex[nAngles];
            Complex[] s2 = new Complex[nAngles];

            double sizeParameter = GetSizeParameter(wavelength);
            double nStop = (int)(sizeParameter + 4.0 * Math.Pow(sizeParameter, 1 / 3) + 2.0);//number of terms in the series
            double kMedium = 0.0;
            double kSphere = 0.0;
            double nSphere = ParticleRefractiveIndexMismatch;
            double nMedium = MediumRefractiveIndexMismatch;
            Complex refrel = new Complex(nSphere, kSphere) / (new Complex(nMedium, kMedium));

            double ym = Math.Abs(sizeParameter * ComplexAbs(refrel));
            int nMx = (int)(ym > nStop ? (ym + 15) : (nStop + 15));

            Complex[] d = new Complex[nMx + 1];
            for (int p = nMx; p > 0; p--)
            {
                Complex z = (p + 1) / (sizeParameter * refrel);
                d[p - 1] = z - 1.0 / (d[p] + z);
            }
            
            for (int i = 0; i < angles; i++)  /* theta[0-360] goes from 0 to pi rad. */
            {
                theta[i] = i*dTheta;
                amu[i] = Math.Cos(theta[i]);
                pi0[i] = 0.0;
                pi1[i] = 1.0;
            }
            
            double psi0 = Math.Cos(sizeParameter);
            double psi1 = Math.Sin(sizeParameter);
            double chi0 = -psi1;
            double chi1 = psi0;
            var xi1 = new Complex(psi1, -chi1);

            double qSca = 0.0;
            
            for (int n = 0; n < nStop; n++)  /* Recursive series until nStop is reached */
            {
                rn = n + 1;
                double fn = (2.0 * rn + 1) / (rn * (rn + 1));
                double psi = (2.0 * rn - 1) * psi1 / sizeParameter - psi0;
                double chi = (2.0 * rn - 1) * chi1 / sizeParameter - chi0;
                var xi = new Complex(psi, -chi);
                Complex an = ((d[n] / refrel + rn / sizeParameter) * psi - psi1) /
                             ((d[n] / refrel + rn / sizeParameter) * xi - xi1);
                Complex bn = ((refrel * d[n] + rn / sizeParameter) * psi - psi1) /
                             ((refrel * d[n] + rn / sizeParameter) * xi - xi1);
                qSca += (2.0 * rn + 1) * (Math.Pow(ComplexAbs(an), 2) + Math.Pow(ComplexAbs(bn), 2));

                double p = Math.Pow(-1.0, rn - 1);
                double t = Math.Pow(-1.0, rn);
                for (int j = 0; j < angles; j++)  /* j from 0 to 360 */
                {
                    int jj = nAngles - 1 - j; /* jj = 720-j (i.e. jj from 720 to 360) */
                    pi[j] = pi1[j];
                    tau[j] = rn * amu[j] * pi[j] - (rn + 1) * pi0[j];
                   
                    s1[j] += fn * (an * pi[j] + bn * tau[j]);
                    s2[j] += fn * (an * tau[j] + bn * pi[j]);
                    if (j == jj)
                        continue;
                    s1[jj] += fn * (p * an * pi[j] + t * bn * tau[j]);
                    s2[jj] += fn * (t * an * tau[j] + p * bn * pi[j]);
                }
                psi0 = psi1;
                psi1 = psi;
                chi0 = chi1;
                chi1 = chi;

                xi1.Re = psi1;
                xi1.Im = -chi1;
                rn += 1;
                for (int j = 0; j < angles; j++)
                {
                    pi1[j] = ((2.0 * rn - 1) * amu[j] * pi[j] - rn * pi0[j]) / (rn - 1);
                    pi0[j] = pi[j];
                }
            } /* end of recursive series loop */

            //return the whole class
            //remove definitions from the constructor
            MieScattParams.Q = new double[3];
            MieScattParams.Q[0] = qSca*2.0 / (sizeParameter * sizeParameter);;
            MieScattParams.Q[1] = (4.0 * s1[0].Re) / (sizeParameter * sizeParameter);
            MieScattParams.Q[2] = 4.0 / (sizeParameter * sizeParameter) * Math.Pow(ComplexAbs(s1[nAngles - 1]), 2);

            MieScattParams.S1 = new Complex[nAngles];
            MieScattParams.S2 = new Complex[nAngles];
            MieScattParams.S11 = new double[nAngles];

            MieScattParams.S1 = s1;
            MieScattParams.S2 = s2;

            //Calculate unpolarized amplitude and g (anisotropy)
            double gNumerator = 0.0;
            double gDenominator = 0.0;
            for (int j = 0; j < nAngles; j++)
            {
                double cost = Math.Cos(j*dTheta);
                double sint = Math.Sqrt(1 - cost*cost);
                MieScattParams.S11[j] = 0.5 * (Math.Pow(ComplexAbs(MieScattParams.S1[j]), 2)
                    + Math.Pow(ComplexAbs(MieScattParams.S2[j]), 2));
                gNumerator  += MieScattParams.S11[j] * cost * 2 * Math.PI * sint * dTheta;
                gDenominator += MieScattParams.S11[j] * 2 * Math.PI * sint* dTheta;
            }
            MieScattParams.G = gNumerator/gDenominator;
            
            return MieScattParams;
        }
        
        private double ComplexAbs(Complex z)
        {
            double re = z.Re;
            double im = z.Im;
            return Math.Sqrt(re * re + im * im);
        }
    }
}
