using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.PhaseFunctions
{
    /// <summary>
    /// An abstract class from which other phase function classes may inherit from.  Subclasses must be able to sample polar and azimuthal angles.
    /// </summary>
    public abstract class PolarAndAzimuthalPhaseFunction
    {

        /// <summary>
        /// An abstract constructor for this class.
        /// </summary>
        public abstract PolarAndAzimuthalPhaseFunction();

        /// <summary>
        /// Rotates the vector incomingDirectionToModify by the polar angle theta and azimuthal angle phi.  
        /// </summary>
        /// <param name="incomingDirectionToModify">The input direction</param>
        /// <param name="theta">The polar angle.</param>
        /// <param name="phi">The azimuthalangle.</param>
        public void Scatter(Direction incomingDirectionToModify, double theta, double phi)
        {
            double cosPsi = Math.Cos(phi);
            double sinPsi = Math.Sin(phi);

            double cosTh = Math.Cos(theta);
            double sinTh = Math.Sin(theta);
            double squz = Math.Sqrt(1 - incomingDirectionToModify.Uz * incomingDirectionToModify.Uz);
            double uxp, uyp, uzp;

            if (squz < 0.0001)
            {
                uxp = sinTh * cosPsi;
                uyp = sinTh * sinPsi;
                uzp = incomingDirectionToModify.Uz > 0 ? cosTh : -cosTh;
            }
            else
            {
                uxp = sinTh / squz * (incomingDirectionToModify.Ux * incomingDirectionToModify.Uz * cosPsi - incomingDirectionToModify.Uy * sinPsi) + incomingDirectionToModify.Ux * cosTh;
                uyp = sinTh / squz * (incomingDirectionToModify.Uy * incomingDirectionToModify.Uz * cosPsi + incomingDirectionToModify.Ux * sinPsi) + incomingDirectionToModify.Uy * cosTh;
                uzp = -sinTh * cosPsi * squz + incomingDirectionToModify.Uz * cosTh;
            }
            incomingDirectionToModify.Ux = uxp;
            incomingDirectionToModify.Uy = uyp;
            incomingDirectionToModify.Uz = uzp;
        }
    }
}
