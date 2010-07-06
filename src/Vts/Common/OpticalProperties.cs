namespace Vts
{
    /// <summary>
    /// Describes optical properties needed as input for various 
    /// forward solver models.
    /// </summary>
    public class OpticalProperties : BindableObject
    {
        private double _Mua;
        private double _Mus;
        private double _G;
        private double _N;

        /// <summary>
        /// Definition of optical properties
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="mua">absorption coefficient = probability of absorption per unit distance traveled</param>
        /// <param name="musp">scattering coefficient = probability of having scattered per unit distance traveled</param>
        /// <param name="g">anisotropy coefficient = cosine of an average scattering angle (where the angle is relative to the incoming and outgoing unit direction vectors)</param>
        /// <param name="n">refractive index mismatch</param>

        public OpticalProperties(double mua, double musp, double g, double n)
        {
            this._Mua = mua;
            this._Mus = musp / (1 - g);
            this._G = g;
            this._N = n;
        }

        public OpticalProperties()
            : this(0.01, 1.0, 0.8, 1.4) { }
        /// <summary>
        /// Creates a new instance based on values from a previous instance
        /// </summary>
        /// <param name="op"></param>
        public OpticalProperties(OpticalProperties op)
        {
            this.Mua = op.Mua;
            this.Musp = op.Musp;
            this.G = op.G;
            this.N = op.N;
        }


        #region Properties

        public double Mua
        {
            get { return _Mua; }
            set
            {
                if(value>=0)
                {
                    _Mua = value;
                    this.OnPropertyChanged("Mua");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Warning - Setting this value also modifies Mus!</remarks>
        public double Musp
        {
            get { return _Mus*(1-_G); }
            set
            {
                _Mus = value / (1 - _G);
                this.OnPropertyChanged("Musp");
                this.OnPropertyChanged("Mus");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Warning - Setting this value also modifies Musp!</remarks>
        public double Mus
        {
            get { return _Mus; }
            set
            {
                _Mus = value;
                this.OnPropertyChanged("Mus");
                this.OnPropertyChanged("Musp");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Warning - Setting this value also modifies Musp!</remarks>
        public double G
        {
            get { return _G; }
            set
            {
                _G = value;
                this.OnPropertyChanged("G");
                this.OnPropertyChanged("Musp");
            }
        }

        public double N
        {
            get { return _N; }
            set
            {
                _N = value;
                this.OnPropertyChanged("N");
            }
        }
        #endregion

        public override string ToString()
        {
            return string.Format("μa={0:0.####} μs'={1:0.####} g={2:0.##} n={3:0.##}", Mua, Musp, G, N);
        }
    }
}
