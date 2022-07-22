namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Methods to help with the determination of polar azimuthal angles.
    /// </summary>
    public class PolarAzimuthalAngles
    {
        private readonly int _hashCode;

        /// <summary>
        /// Returns polar azimuthal angle angle in spherical coordinate system
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        public PolarAzimuthalAngles(double theta, double phi)
        {
            Theta = theta;
            Phi = phi;
            _hashCode = new {theta, phi}.GetHashCode();
        }
        /// <summary>
        /// Theta (polar) and Phi (azimuthal) angles default constructor
        /// </summary>
        public PolarAzimuthalAngles() : this(0, 0)   { }

        /// <summary>
        /// Polar Angle
        /// </summary>
        public double Theta { get; set; }

        /// <summary>
        /// Azimuthal Angle
        /// </summary>
        public double Phi { get; set; }        

        /// <summary>
        /// Equality overload for polar azimuthal angle pair
        /// </summary>
        /// <param name="pa1">polar angle 1</param>
        /// <param name="pa2">polar angle 2</param>
        /// <returns>boolean</returns>
        public static bool operator ==(PolarAzimuthalAngles pa1, PolarAzimuthalAngles pa2)
        {
            if (object.ReferenceEquals(pa1, pa2))
            {
                // handles if both are null as well as object identity
                return true;
            }

            if (object.Equals(pa1, pa2))
            {
                return true;
            }
            
            if ((object)pa1 == null || (object)pa2 == null)
            {
                return false;
            }

            return pa1.Equals(pa2);
        }

        /// <summary>
        /// Inequality overload for polar azimuthal angle pair
        /// </summary>
        /// <param name="pa1">polar angle 1</param>
        /// <param name="pa2">polar angle 2</param>
        /// <returns>boolean</returns>
        public static bool operator !=(PolarAzimuthalAngles pa1, PolarAzimuthalAngles pa2)
        {
            return !(pa1 == pa2);
        }      
  
        /// <summary>
        /// Instance member for equality comparison
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is PolarAzimuthalAngles)
            {
                var pa = obj as PolarAzimuthalAngles;
                return  Theta == pa.Theta && Phi == pa.Phi;
            }
            return false;
        }

        /// <summary>
        /// method to clone class
        /// </summary>
        /// <returns>PolarAzimuthalAngles class</returns>
        public PolarAzimuthalAngles Clone()
        {
            return new PolarAzimuthalAngles(this.Theta, this.Phi);
        }
        
        /// <summary>
        /// Override of GetHashCode to allow the type to work correctly in a hash table
        /// </summary>
        /// <returns>The hashcode as an integer</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

    }
}
