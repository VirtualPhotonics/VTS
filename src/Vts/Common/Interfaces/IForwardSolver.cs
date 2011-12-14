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
        #region double ROfRho(OpticalProperties op, double rho);
        /// <summary>
        /// Determines reflectance at source-detector separation rho
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>reflectance at source-detector separation rho</returns>
        double ROfRho(OpticalProperties op, double rho);
        #endregion

        #region double ROfRhoAndT(OpticalProperties op, double rho, double t);
        /// <summary>
        /// Determines reflectance at source-detector separation rho and time t
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at source-detector separation rho and time t</returns>
        double ROfRhoAndT(OpticalProperties op, double rho, double t);
        #endregion

        #region double ROfRhoAndFt(OpticalProperties op, double rho, double ft);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and modulation frequency ft
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at source-detector separation rho and modulation frequency ft</returns>
        Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft);
        #endregion

        #region double ROfFx(OpticalProperties op, double fx);
        /// <summary>
        /// Determines reflectance at spatial frequency fx
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at spatial frequency fx</returns>
        double ROfFx(OpticalProperties op, double fx);
        #endregion

        #region double ROfFxAndT(OpticalProperties op, double fx, double t);
        /// <summary>
        /// Determines reflectance at spatial frequency fx and time t
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at spatial frequency fx and time t</returns>
        double ROfFxAndT(OpticalProperties op, double fx, double t);
        #endregion

        #region double ROfFxAndFt(OpticalProperties op, double fx, double ft);

        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at spatial frequency fx and modulation frequency ft</returns>
        Complex ROfFxAndFt(OpticalProperties op, double fx, double ft);
        #endregion

        // The following methods are designed to return values based on iteration 
        // with the leftmost IEnumerable<T> input being the top-level, and so-on
        // such that the right-most input is at the inner-most loop

        #region IEnumerable<double> FluenceOfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs);
        /// <summary>
        /// Overload of scalar ROfRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceOfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs);
        #endregion

        #region IEnumerable<double> FluenceOfRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar ROfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> FluenceOfRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> ts);
        #endregion

        #region IEnumerable<double> FluenceOfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);
        /// <summary>
        /// Overload of scalar ROfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> FluenceOfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);
        #endregion

        #region IEnumerable<double> FluenceOfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs);
        /// <summary>
        /// Overload of scalar ROfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceOfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs);
        #endregion
        
        #region IEnumerable<double> FluenceOfFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar ROfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        IEnumerable<double> FluenceOfFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> ts);
        #endregion

        #region IEnumerable<double> FluenceOfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts);
        /// <summary>
        /// Overload of scalar ROfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        IEnumerable<double> FluenceOfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts);
        #endregion

        #region IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos);
        /// <summary>
        /// Overload of scalar ROfRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos);

        #endregion


        #region IEnumerable<double> ROfRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar ROfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<double> ROfRhoAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> ts);


        #endregion

        #region IEnumerable<double> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts);

        /// <summary>
        /// Overload of scalar ROfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts);

        #endregion

        #region IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs);
        /// <summary>
        /// Overload of scalar ROfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs);

        
        #region IEnumerable<double> ROfFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts);
        /// <summary>
        /// Overload of scalar ROfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        IEnumerable<double> ROfFxAndT(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> ts);

        #endregion

        #region IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts);

        /// <summary>
        /// Overload of scalar ROfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts);

        #endregion

        #endregion

        double BeamDiameter { get; set; } // temp - should go in ISourceConfiguration or something

        #region Convenience Overloads - default implementation handled by ForwardSolverBase (todo: these could alternatively be IForwardSolverExtensions instead of part of the contract)

        /// <summary>
        /// Overload of scalar ROfRho function. Determines reflectances at optical properties 'op' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="op">medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        double[] ROfRho(OpticalProperties op, double[] rhos);

        /// <summary>
        /// Overload of scalar ROfRho function. Determines reflectances at optical properties 'ops' and source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns></returns>
        double[] ROfRho(OpticalProperties[] ops, double[] rhos);

        /// <summary>
        /// Overload of scalar ROfRhoAndT function. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        double[] ROfRhoAndT(OpticalProperties op, double[] rhos, double[] ts);

        /// <summary>
        /// Overload of scalar ROfRhoAndT function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ts">times (ns)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        double[]ROfRhoAndT(OpticalProperties[] ops, double[] rhos, double[] ts);

        /// <summary>
        /// Overload of scalar ROfRhoAndFt function. Determines reflectances at optical properties 'op', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="os">medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        Complex[]ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts);

        /// <summary>
        /// Overload of scalar ROfRhoAndFt function. Determines reflectances at optical properties 'ops', source-detector separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        /// <remarks>IEnumerables can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions) on single items</remarks>
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts);

        /// <summary>
        /// Overload of scalar ROfFx function. Determines reflectances at optical properties 'op' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="op">medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        double[] ROfFx(OpticalProperties op, double[] fxs);

        /// <summary>
        /// Overload of scalar ROfFx function. Determines reflectances at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns></returns>
        double[] ROfFx(OpticalProperties[] ops, double[] fxs);

        /// <summary>
        /// Overload of scalar ROfFxAndT function. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="op">medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        double[] ROfFxAndT(OpticalProperties op, double[] fxs, double[] ts);

        /// <summary>
        /// Overload of scalar ROfFxAndT function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and times 'ts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ts"></param>
        /// <returns></returns>
        double[] ROfFxAndT(OpticalProperties[] ops, double[] fxs, double[] ts);

        /// <summary>
        /// Overload of scalar ROfFxAndFt function. Determines reflectances at optical properties 'op', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double[] fts);

        /// <summary>
        /// Overload of scalar ROfFxAndFt function. Determines reflectances at optical properties 'ops', spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns></returns>
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts);

        // others...need comments...again, these could be extensions
        double[] ROfRho(OpticalProperties[] ops, double rho);
        double[] ROfRhoAndT(OpticalProperties[] ops, double rho, double[] ts);
        double[] ROfRhoAndT(OpticalProperties[] ops, double[] rhos, double t);
        double[] ROfRhoAndT(OpticalProperties op, double rho, double[] ts);
        double[] ROfRhoAndT(OpticalProperties op, double[] rhos, double t);
        double[] ROfRhoAndT(OpticalProperties[] ops, double rho, double t);
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts);
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft);
        Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts);
        Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft);
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double ft);
        double[] ROfFx(OpticalProperties[] ops, double fx);
        double[] ROfFxAndT(OpticalProperties[] ops, double fx, double[] ts);
        double[] ROfFxAndT(OpticalProperties[] ops, double[] fxs, double t);
        double[] ROfFxAndT(OpticalProperties op, double fx, double[] ts);
        double[] ROfFxAndT(OpticalProperties op, double[] fxs, double t);
        double[] ROfFxAndT(OpticalProperties[] ops, double fx, double t);
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double[] fts);
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double ft);
        Complex[] ROfFxAndFt(OpticalProperties op, double fx, double[] fts);
        Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double ft);
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft);
        
        double[] FluenceOfRho(OpticalProperties[] ops, double[] rhos, double[] zs);
        double[] FluenceOfRhoAndT(OpticalProperties[] ops, double[] rhos, double[] zs, double[] ts);
        double[] FluenceOfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts);
        double[] FluenceOfFx(OpticalProperties[] ops, double[] fxs, double[] zs);
        double[] FluenceOfFxAndT(OpticalProperties[] ops, double[] fxs, double[] zs, double[] ts);
        double[] FluenceOfFxAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts);

        #endregion
    }
}
