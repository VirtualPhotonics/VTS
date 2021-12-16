using System;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// This class allows creation and copying of a diffusion parameters object, which contains fields
    /// necessary for solution of diffusion models. The creation is made by passing a opticalproperties
    /// object.
    /// </summary>
    public class DiffusionParameters
    {
        private DiffusionParameters(double A, double mueff, double zb, double zp, double mutTilde,
            double musTilde, double mutr, double gTilde, double D, double cn, double mua)
        {
            this.A = A;
            this.mueff = mueff;
            this.zb = zb;
            this.zp = zp;
            this.mutTilde = mutTilde;
            this.musTilde = musTilde;
            this.mutr = mutr;
            this.gTilde = gTilde;
            this.D = D;
            this.cn = cn;
            this.mua = mua;
        }

        /// <summary>
        /// Create a DiffusionParameters object from OpticalProperties object and a ForwardModel
        /// choice.
        /// </summary>
        /// <param name="op">OpticalProperties object</param>
        /// <param name="fm">ForwardModel enum</param>
        /// <returns>new DiffusionParameters object</returns>
        public static DiffusionParameters Create(OpticalProperties op, ForwardModel fm)
        {
            var tempMua = op.Mua;
            var tempMutr = op.Mua + op.Musp;
            var tempCn = GlobalConstants.C / op.N;
            var tempD = 1 / (3 * tempMutr);
            var tempA = CalculatorToolbox.GetCubicAParameter(op.N);
            var tempMueff = Math.Sqrt(3 * op.Mua * tempMutr);
            var tempZb = 2 / (3 * tempMutr) * tempA;
            double tempMusTilde;
            double tempGTilde;
            switch (fm)
            {
                case ForwardModel.SDA:
                default:
                    tempMusTilde = op.Musp;
                    tempGTilde = op.G;
                    break;
                case ForwardModel.DeltaPOne:
                    tempMusTilde = op.Musp * (1 - op.G * op.G) / (1 - op.G);
                    tempGTilde = op.G / (op.G + 1);
                    break;
            }
            var tempMutTilde = op.Mua + tempMusTilde;
            var tempZp = 1 / tempMutTilde;

            return new DiffusionParameters(tempA, tempMueff, tempZb, tempZp, tempMutTilde, tempMusTilde, tempMutr, tempGTilde, tempD, tempCn, tempMua);
        }

        /// <summary>
        /// Create a new copy of a DiffusionParameters object
        /// </summary>
        /// <param name="dp">DiffusionParameters object</param>
        /// <returns>new copy of DiffusionParameters object</returns>
        public static DiffusionParameters Copy(DiffusionParameters dp)
        {
            return new DiffusionParameters(dp.A, dp.mueff, dp.zb, dp.zp, dp.mutTilde,
                dp.musTilde, dp.mutr, dp.gTilde, dp.D, dp.cn, dp.mua);
        }
        // properties

        /// <summary>
        /// Diffusion boundary parameter A = (1 + R2)/(1 - R1)
        /// </summary>
        public double A { get; set; }
        /// <summary>
        /// effective attenuation coefficient = sqrt(mua / D)
        /// </summary>
        public double mueff { get; set; }
        /// <summary>
        /// extrapolated boundary distance
        /// </summary>
        public double zb { get; set; }
        /// <summary>
        /// ??? z0, better explain
        /// </summary>
        public double zp { get; set; }
        /// <summary>
        /// mean reduced tranport length as a function of the phase function, if Eddington: = mutr, 
        /// if delta-Eddington = mutStar
        /// </summary>
        public double mutTilde { get; set; }
        /// <summary>
        /// mean reduced transport length
        /// </summary>
        public double mutr { get; set; }
        /// <summary>
        /// reduced scattering length as a function of the phase function, if Eddington: = musPrime, 
        /// if delta-Eddington = musStar
        /// </summary>
        public double musTilde { get; set; }
        /// <summary>
        /// 1st moment of the scattering phase function, if Eddington = g, if delta-Eddington = g/(g+1)
        /// </summary>
        public double gTilde { get; set; }
        /// <summary>
        /// diffusion coefficient = 1/3/mutr
        /// </summary>
        public double D { get; set; }
        /// <summary>
        /// speed of light adjusted according to the refractive index of the media cn = c / n, where
        /// n is the media refractive index
        /// </summary>
        public double cn { get; set; }
        /// <summary>
        /// absorption coefficient
        /// </summary>
        public double mua { get; set; }
    }
}