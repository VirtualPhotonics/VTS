using System;
using System.Collections.Generic;
using Meta.Numerics;
using Vts.Common.Math;

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

        private MieScatteringParameters MieScattParams;

        private class MieScatteringParameters
        {

            public double[] Q { get; set; }
            public Complex[] S1 { get; set; }
            public Complex[] S2 { get; set; }
            public double[] S11 { get; set; }
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
            double mediumRefractiveIndex)
        {
            ParticleRadius = particleRadius;
            ParticleRefractiveIndexMismatch = particleRefractiveIndex;
            MediumRefractiveIndexMismatch = mediumRefractiveIndex;
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
        /// Returns the anisotropy coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The anisotropy coeffient g</returns>
        public double GetG(double wavelength)
        {
            MieScattParams = new MieScatteringParameters();
            BohrenHuffmanMie(wavelength);

            var n = Integration.IntegrateAdaptiveSimpsonRule(Numerator, 0.0, Math.PI, 0.0001);
            var d = Integration.IntegrateAdaptiveSimpsonRule(Denominator, 0.0, Math.PI, 0.0001);
            double g = n / d;
            return g;
        }

        /// <summary>
        /// Returns the scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The scattering coefficient Mus</returns>
        public double GetMus(double wavelength)
        {
            MieScattParams = new MieScatteringParameters();
            double sizeParameter = GetSizeParameter(wavelength);

            BohrenHuffmanMie(wavelength);

            double Qsca = MieScattParams.Q[0];
            double fv = 0.001;//particle volume fraction

            //Let a = particle radius;
            //fv = particle volume fraction;
            //rho_s = fv/((4/3)*Pi/a^3) = particle number density;
            //sigma_s = Qsca*A = cross-section
            //A = Pi*a^2 = area
            // Then mus = rho_s*sigma_s, which, upon simplifying, produces:
            // mus = (fv*Qsca)/((4/3)*a)
            double mus = fv * Qsca / ((4.0 / 3.0) * ParticleRadius);

            //to convert to mus per millimeter, multiply by 10^3:
            mus *= 1000;
            return mus;
        }

        /// <summary>
        /// Returns the reduced scattering coefficient for a given wavelength
        /// </summary>
        /// <param name="wavelength">Wavelength</param>
        /// <returns>The reduced scattering coefficient Mus'</returns>
        public double GetMusp(double wavelength)
        {
            //MieScattParams = new MieScatteringParameters();
            return GetMus(wavelength) * (1.0 - GetG(wavelength));
        }

        private double GetSizeParameter(double wavelength)
        {
            return 1000 * 2.0 * Math.PI * ParticleRadius * MediumRefractiveIndexMismatch / wavelength;
        }

        private MieScatteringParameters BohrenHuffmanMie(double wavelength)
        {
            double sizeParameter = GetSizeParameter(wavelength);


            int i, j, n, rn, jj, nMx;
            double[] amu = new double[angles];
            double[] theta = new double[angles];
            double[] tau = new double[angles];
            double[] pi = new double[angles];
            double[] pi0 = new double[angles];
            double[] pi1 = new double[angles];

            double nStop = (int)(sizeParameter + 4.0 * Math.Pow(sizeParameter, 1 / 3) + 2.0);//number of terms in the series
            double kMedium = 0.0;
            double kSphere = 0.0;
            double nSphere = ParticleRefractiveIndexMismatch;
            double nMedium = MediumRefractiveIndexMismatch;
            Complex refrel = new Complex(nSphere, kSphere) / (new Complex(nMedium, kMedium));

            double ym = Math.Abs(sizeParameter * ComplexAbs(refrel));
            Complex y = sizeParameter * refrel;
            nMx = (int)(ym > nStop ? (ym + 15) : (nStop + 15));

            Complex[] d = new Complex[nMx + 1];

            theta[0] = 0;
            amu[0] = 1.0;

            for (i = 1; i < angles; i++)  /* theta[0-360] goes from 0 to pi rad. */
            {
                theta[i] = theta[i - 1] + dTheta;
                amu[i] = Math.Cos(theta[i]);
            }

            Complex z = new Complex();
            for (int p = nMx; p > 0; p--)
            {
                z = (Complex)(p + 1) / (sizeParameter * refrel);
                d[p - 1] = z - 1.0 / (d[p] + z);
            }

            Complex[] s1 = new Complex[nAngles];
            Complex[] s2 = new Complex[nAngles];

            for (i = 0; i < angles; i++)  /* pi0[0-360] = 0, pi1[0-360] = 1 */
            {
                pi0[i] = 0.0;
                pi1[i] = 1.0;
            }

            double psi0, psi1, chi0, chi1, psi;
            chi1 = psi0 = Math.Cos(sizeParameter);
            psi1 = Math.Sin(sizeParameter);
            chi0 = -psi1;
            Complex xi0 = new Complex(psi0, -chi0);
            Complex xi1 = new Complex(psi1, -chi1);

            double Qscat = 0.0;
            double Qext = 0.0;
            double Qback = 0.0;

            for (n = 0; n < nStop; n++)  /* Recursive series until nStop is reached */
            {
                double fn, chi;

                Complex an = new Complex();
                Complex bn = new Complex();

                rn = n + 1;
                fn = (2.0 * rn + 1) / (rn * (rn + 1));
                psi = (2.0 * rn - 1) * psi1 / sizeParameter - psi0;
                chi = (2.0 * rn - 1) * chi1 / sizeParameter - chi0;
                Complex xi = new Complex(psi, -chi);
                an = ((d[n] / refrel + rn / sizeParameter) * psi - psi1) /
                    ((d[n] / refrel + rn / sizeParameter) * xi - xi1);
                bn = ((refrel * d[n] + rn / sizeParameter) * psi - psi1) /
                    ((refrel * d[n] + rn / sizeParameter) * xi - xi1);
                Qscat += (2.0 * rn + 1) * (Math.Pow(ComplexAbs(an), 2) + Math.Pow(ComplexAbs(bn), 2));

                for (j = 0; j < angles; j++)  /* j from 0 to 360 */
                {
                    double p, t;
                    jj = nAngles - 1 - j; /* jj = 720-j (i.e. jj from 720 to 360) */
                    pi[j] = pi1[j];
                    tau[j] = rn * amu[j] * pi[j] - (rn + 1) * pi0[j];

                    p = Math.Pow(-1.0, n - 1);
                    t = Math.Pow(-1, n);
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
                for (j = 0; j < angles; j++)
                {
                    pi1[j] = ((2.0 * rn - 1) * amu[j] * pi[j] - rn * pi0[j]) / (rn - 1);
                    pi0[j] = pi[j];
                }
            } /* end of recursive series loop */

            Qscat *= 2.0 / (sizeParameter * sizeParameter);
            Qext = (4.0 * s1[0].Re) / (sizeParameter * sizeParameter);
            Qback = 4.0 / (sizeParameter * sizeParameter) * Math.Pow(ComplexAbs(s1[nAngles - 1]), 2);
            //return the whole class
            //remove definitions from the constructor

            MieScattParams.Q = new double[3];
            MieScattParams.Q[0] = Qscat;
            MieScattParams.Q[1] = Qext;
            MieScattParams.Q[2] = Qback;
            int l = s1.Length;
            MieScattParams.S1 = new Complex[l];
            MieScattParams.S2 = new Complex[l];
            MieScattParams.S11 = new double[l];

            MieScattParams.S1 = s1;
            MieScattParams.S2 = s2;

            for (int k = 0; k < s1.Length; k++)
            {
                MieScattParams.S11[k] = 0.5 * (Math.Pow(ComplexAbs(MieScattParams.S1[k]), 2)
                    + Math.Pow(ComplexAbs(MieScattParams.S2[k]), 2));
            }
            for (int st = 0; st < MieScattParams.S11.Length; st++)
            {

            }
            return MieScattParams;
        }

        private double S11OfTheta(double theta)
        {
            List<double> thetai = new List<double>();
            List<double> S11i = new List<double>();

            for (int i = 0; i < nAngles; i++)
            {
                thetai.Add(i * dTheta);
                S11i.Add(MieScattParams.S11[i]);
            }
            double answ = Interpolation.interp1(thetai, S11i, theta);
            return answ;

        }
        private double Numerator(double theta)
        {
            return S11OfTheta(theta) * Math.Cos(theta) * 2.0 * Math.PI * Math.Sin(theta);
        }
        private double Denominator(double theta)
        {
            return S11OfTheta(theta) * 2.0 * Math.PI * Math.Sin(theta);
        }

        private double ComplexAbs(Complex z)
        {
            double re = z.Re;
            double im = z.Im;
            return Math.Sqrt(re * re + im * im);
        }
    }
}
