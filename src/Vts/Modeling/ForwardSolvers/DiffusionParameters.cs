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
        private double _A;
        private double _mueff;
        private double _zb;
        private double _zp;
        private double _mutTilde;
        private double _mutr;
        private double _musTilde;
        private double _gTilde;
        private double _D;
        private double _cn;
        private double _mua;

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
            var mua = op.Mua;
            var mutr = op.Mua + op.Musp;
            var cn = GlobalConstants.C / op.N;
            var D = 1 / (3 * mutr);
            var A = CalculatorToolbox.GetCubicAParameter(op.N);
            var mueff = Math.Sqrt(3 * op.Mua * mutr);
            var zb = 2 / (3 * mutr) * A;
            double musTilde;
            double gTilde;
            switch (fm)
            {
                case ForwardModel.SDA:
                default:
                    musTilde = op.Musp;
                    gTilde = op.G;
                    break;
                case ForwardModel.DeltaPOne:
                    musTilde = op.Musp * (1 - op.G * op.G) / (1 - op.G);
                    gTilde = op.G / (op.G + 1);
                    break;
            }
            var mutTilde = op.Mua + musTilde;
            var zp = 1 / mutTilde;

            return new DiffusionParameters(A, mueff, zb, zp, mutTilde, musTilde, mutr, gTilde, D, cn, mua);
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
        public double A
        {
            get { return _A; }
            set { _A = value; }
        }
        /// <summary>
        /// effective attenuation coefficient = sqrt(mua / D)
        /// </summary>
        public double mueff
        {
            get { return _mueff; }
            set { _mueff = value; }
        }
        /// <summary>
        /// extrapolated boundary distance
        /// </summary>
        public double zb
        {
            get { return _zb; }
            set { _zb = value; }
        }
        /// <summary>
        /// ??? z0, better explain
        /// </summary>
        public double zp
        {
            get { return _zp; }
            set { _zp = value; }
        }
        /// <summary>
        /// mean reduced tranport length as a function of the phase function, if Eddington: = mutr, 
        /// if delta-Eddington = mutStar
        /// </summary>
        public double mutTilde
        {
            get { return _mutTilde; }
            set { _mutTilde = value; }
        }
        /// <summary>
        /// mean reduced transport length
        /// </summary>
        public double mutr
        {
            get { return _mutr; }
            set { _mutr = value; }
        }
        /// <summary>
        /// reduced scattering length as a function of the phase function, if Eddington: = musPrime, 
        /// if delta-Eddington = musStar
        /// </summary>
        public double musTilde
        {
            get { return _musTilde; }
            set { _musTilde = value; }
        }
        /// <summary>
        /// 1st moment of the scattering phase function, if Eddington = g, if delta-Eddington = g/(g+1)
        /// </summary>
        public double gTilde
        {
            get { return _gTilde; }
            set { _gTilde = value; }
        }
        /// <summary>
        /// diffusion coefficient = 1/3/mutr
        /// </summary>
        public double D
        {
            get { return _D; }
            set { _D = value; }
        }
        /// <summary>
        /// speed of light adjusted according to the refractive index of the media cn = c / n, where
        /// n is the media refractive index
        /// </summary>
        public double cn
        {
            get { return _cn; }
            set { _cn = value; }
        }
        /// <summary>
        /// absorption coefficient
        /// </summary>
        public double mua
        {
            get { return _mua; }
            set { _mua = value; }
        }
    }
}