using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using MathNet.Numerics;

namespace Vts
{
    /// <summary>
    /// Defines contract for a forward solver 
    /// </summary>
    public interface IForwardSolver : INotifyPropertyChanged
    {
        #region double RofRho(OpticalProperties op, double rho);
        /// <summary>
        /// Determines reflectance at source-detector separation rho
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>reflectance at source-detector separation rho</returns>
        double RofRho(OpticalProperties op, double rho);
        #endregion

        #region double RofRhoAndT(OpticalProperties op, double rho, double t);
        /// <summary>
        /// Determines reflectance at source-detector separation rho and time t
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at source-detector separation rho and time t</returns>
        double RofRhoAndT(OpticalProperties op, double rho, double t);
        #endregion

        #region double RofRhoAndFt(OpticalProperties op, double rho, double ft);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and modulation frequency ft
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at source-detector separation rho and modulation frequency ft</returns>
        Complex RofRhoAndFt(OpticalProperties op, double rho, double ft);
        #endregion

        #region double RofFx(OpticalProperties op, double fx);
        /// <summary>
        /// Determines reflectance at spatial frequency fx
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at spatial frequency fx</returns>
        double RofFx(OpticalProperties op, double fx);
        #endregion

        #region double RofFxAndT(OpticalProperties op, double fx, double t);
        /// <summary>
        /// Determines reflectance at spatial frequency fx and time t
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at spatial frequency fx and time t</returns>
        double RofFxAndT(OpticalProperties op, double fx, double t);
        #endregion

        #region double RofFxAndFt(OpticalProperties op, double fx, double ft);

        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at spatial frequency fx and modulation frequency ft</returns>
        Complex RofFxAndFt(OpticalProperties op, double fx, double ft);
        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<T> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop

        #region IEnumerable<double> FluenceofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs);
        /// <summary>
        /// Overload of scalar RofRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs);
        #endregion

        #region IEnumerable<double> FluenceofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar RofRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> FluenceofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts);
        #endregion

        #region IEnumerable<double> FluenceofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);
        /// <summary>
        /// Overload of scalar RofRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> FluenceofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);
        #endregion

        #region IEnumerable<double> FluenceofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs);
        /// <summary>
        /// Overload of scalar RofFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs);
        #endregion
        
        #region IEnumerable<double> FluenceofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar RofFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        IEnumerable<double> FluenceofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts);
        #endregion

        #region IEnumerable<double> FluenceofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts);
        /// <summary>
        /// Overload of scalar RofFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts);
        #endregion

        #region IEnumerable<double> RofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos);
        /// <summary>
        /// Overload of scalar RofRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        IEnumerable<double> RofRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos);
        #endregion

        #region IEnumerable<double> RofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar RofRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> RofRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts);
        #endregion

        #region IEnumerable<double> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts);

        /// <summary>
        /// Overload of scalar RofRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<Complex> RofRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts);
        #endregion

        #region IEnumerable<double> RofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs);
        /// <summary>
        /// Overload of scalar RofFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        IEnumerable<double> RofFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs);
        
        #region
        /// <summary>
        /// Overload of scalar RofFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        IEnumerable<double> RofFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts);
        #endregion

        #region

        /// <summary>
        /// Overload of scalar RofFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        IEnumerable<Complex> RofFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts);
        #endregion

        #endregion

        double BeamDiameter { get; set; } // temp - should go in ISourceConfiguration or something
    }
}
