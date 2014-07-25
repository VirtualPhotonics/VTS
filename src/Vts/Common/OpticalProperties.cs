// copy from before change:

using System.Runtime.Serialization;

namespace Vts
{
    /// <summary>
    /// Describes optical properties needed as input for various 
    /// forward solver models.
    /// </summary>
    [DataContract]
    public class OpticalProperties : BindableObject
    {
        private double _Mua;
        private double _Mus;
        private double _G;
        private double _N;

        /// <summary>
        /// Definition of optical properties
        /// </summary>
        /// <param name="mua">absorption coefficient = probability of absorption per unit distance traveled</param>
        /// <param name="musp">scattering coefficient = probability of having scattered per unit distance traveled</param>
        /// <param name="g">anisotropy coefficient = cosine of an average scattering angle (where the angle is relative to the incoming and outgoing unit direction vectors)</param>
        /// <param name="n">refractive index mismatch</param>
        public OpticalProperties(double mua, double musp, double g, double n)
        {
            Mua = mua;
            G = g;
            N = n;
            Musp = musp; // sets _Mus under the hood
        }

        /// <summary>
        /// Creates optical properties with values 0.01, 1.0, 0.8, 1.4 for mua, musp, g and n respectively
        /// </summary>
        public OpticalProperties()
            : this(0.01, 1.0, 0.8, 1.4) { }

        /// <summary>
        /// Creates a new instance based on values from a previous instance
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        public OpticalProperties(OpticalProperties op)
            : this(op.Mua, op.Musp, op.G, op.N)
        {
        }

        /// <summary>
        /// Creates a new instance based on values in an array [mua, musp, g, n]
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        public OpticalProperties(double[] op)
            : this(op[0], op[1], op[2], op[3])
        {
        }


        /// <summary>
        /// absorption coefficient = probability of absorption per unit distance traveled
        /// </summary>
        [DataMember]
        public double Mua
        {
            get { return _Mua; }
            set
            {
                // DC: This was creating problems with the optimization library
                // constraints like this should be done at the optimizer/GUI level 
                //if (value >= 0) 
                //{
                    _Mua = value;
                    this.OnPropertyChanged("Mua");
                //}
            }
        }

        /// <summary>
        /// scattering coefficient = probability of having scattered per unit distance traveled
        /// </summary>
        /// <remarks>Warning - Setting this value also modifies Musp!</remarks>
        [DataMember]
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
        /// anisotropy coefficient = cosine of an average scattering angle (where the angle is relative to the incoming and outgoing unit direction vectors)
        /// </summary>
        /// <remarks>Warning - Setting this value also modifies Mus!</remarks>
        [DataMember]
        public double G
        {
            get { return _G; }
            set
            {
                var previousMusp = Musp;
                _G = value;
                this.OnPropertyChanged("G");
                Musp = previousMusp; // convention that, if g changes, musp should stay the same (forcing mus to shift)
            }
        }

        /// <summary>
        /// refractive index mismatch
        /// </summary>
        [DataMember]
        public double N
        {
            get { return _N; }
            set
            {
                _N = value;
                this.OnPropertyChanged("N");
            }
        }

        /// <summary>
        /// reduced scattering coefficient = probability of having scattered per unit distance traveled
        /// </summary>
        /// <remarks>
        /// Warning - Setting this value also modifies Mus!
        /// Warning - This must be defined AFTER Mus and G to deserialize correctly
        ///           (or alternatively, we should use [IgnoreDataMember] either here or for Mus)
        /// </remarks>
        [DataMember]
        public double Musp
        {
            get
            {
                if (_G == 1)
                {
                    return _Mus;
                }
                else
                {
                    return _Mus * (1 - _G);
                }
            }
            set
            {
                if (_G == 1)
                {
                    _Mus = value;
                }
                else
                {
                    _Mus = value / (1 - _G);
                }
                this.OnPropertyChanged("Musp");
                this.OnPropertyChanged("Mus");
            }
        }

        /// <summary>
        /// Writes the optical properties to a string in the order Mua, Musp, G and N
        /// </summary>
        /// <returns>String of optical properties</returns>
        public override string ToString()
        {
            return string.Format("μa={0:0.####} μs'={1:0.####} g={2:0.##} n={3:0.##}", Mua, Musp, G, N);
        }
    }
}