using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Vts
{
    /// <summary>
    /// Defines contract for a forward solver 
    /// </summary>
    public interface IForwardSolver : INotifyPropertyChanged
    {
        /// <summary>
        /// Beam Diameter
        /// </summary>
        double BeamDiameter { get; set; }

        #region ROfRho
        /// <summary>
        /// Determines reflectance at optical properties 'op' and
        /// source-detector separation 'rho'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>Reflectance at source-detector separation rho</returns>
        double ROfRho(OpticalProperties op, double rho);

        /// <summary>
        /// Determines reflectance at optical properties 'op' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        double[] ROfRho(OpticalProperties op, double[] rhos);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and
        /// source-detector separation 'rho'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>Reflectance as a function of rho</returns>
        double[] ROfRho(OpticalProperties[] ops, double rho);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        double[] ROfRho(OpticalProperties[] ops, double[] rhos);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and
        /// source-detector separation 'rho'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>Reflectance at source-detector separation rho</returns>
        double ROfRho(IOpticalPropertyRegion[] regions, double rho);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        double[] ROfRho(IOpticalPropertyRegion[] regions, double[] rhos);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        IEnumerable<double> ROfRho(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and
        /// source-detector separations 'rhos'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <returns>Reflectance as a function of rhos</returns>
        double[] ROfRho(IOpticalPropertyRegion[][] regions, double[] rhos);
        #endregion

        #region ROfRhoAndTime
        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// source-detector separation 'rho' and time 'time'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance at source-detector separation rho and time</returns>
        double ROfRhoAndTime(OpticalProperties op, double rho, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// source-detector separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rhos and times</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'op', source-detector
        /// separation 'rho' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rho and times</returns>
        double[] ROfRhoAndTime(OpticalProperties op, double rho, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// source-detector separations 'rhos' and time 'time'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of rhos and time</returns>
        double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// source-detector separation 'rho' and time 'time'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of rho and time</returns>
        double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// source-detector separations 'rhos' and time 'time'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as function of rhos and times</returns>
        double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// source-detector separation 'rho' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rho and time</returns>
        double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// source-detector separations 'rhos' and times 
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rho and time</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// source-detector separations 'rhos' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rhos and times</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separation 'rho' and time 'time'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance at source-detector separation rho</returns>
        double ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double time);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separations 'rhos', and time 'time'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of rhos and time</returns>
        double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double time);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separation 'rho', and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rho">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rho and times</returns>
        double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double[] times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separations 'rhos', and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as function of rhos and times</returns>
        double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double[] times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separations 'rhos', and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rhos and times</returns>
        IEnumerable<double> ROfRhoAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos,
            IEnumerable<double> times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// source-detector separations 'rhos', and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rhos and times</returns>
        double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double[] times);
        #endregion

        #region ROfRhoAndFt
        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// source-detector separation 'rho' and modulation frequency 'ft'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance at source-detector separation rho and modulation frequency ft</returns>
        Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'op', source-detector
        /// separations 'rhos' and modulation frequency 'ft'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of rhos and ft</returns>
        Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'op', source-detector
        /// separation 'rho' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rho and fts</returns>
        Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'op', source-detector
        /// separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops', source-detector
        /// separation 'rho' and modulation frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of rho and ft</returns>
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'ops', source-detector
        /// separations 'rhos' and modulation frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of rhos and ft</returns>
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'ops', source-detector
        /// separation 'rho' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rho and fts</returns>
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops', source-detector
        /// separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops', source-detector
        /// separations 'rhos' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts);

        /// <summary>
        /// Determines reflectance at tissue regions 'regions', source-detector
        /// separation 'rho' and temporal frequency 'ft'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>Reflectance at source-detector separation rho</returns>
        Complex ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double ft);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions', source-detector 
        /// separations 'rhos' and temporal frequency 'ft'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separation (mm)</param>
        /// <param name="ft">temporal frequencies</param>
        /// <returns>Reflectance as a function of rhos and ft</returns>
        Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double ft);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions', source-detector 
        /// separation 'rho' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of rho and fts</returns>
        Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double[] fts);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions', source-detector 
        /// separations 'rhos' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double[] fts);

        /// <summary>
        /// Determines reflectance at tissue regions 'regions', source-detector 
        /// separations 'rhos' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        IEnumerable<Complex> ROfRhoAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> fts);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions', source-detector 
        /// separations 'rhos', and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of rhos and fts</returns>
        Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] fts);
        #endregion

        #region ROfFx
        /// <summary>
        /// Determines reflectance at optical properties 'op' and spatial frequency 'fx'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>Reflectance at spatial frequency fx</returns>
        double ROfFx(OpticalProperties op, double fx);

        /// <summary>
        /// Determines reflectance at optical properties 'op' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>Reflectance as a function of fxs</returns>
        double[] ROfFx(OpticalProperties op, double[] fxs);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and spatial frequency 'fx'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>Reflectance as a function of fx</returns>
        double[] ROfFx(OpticalProperties[] ops, double fx);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>Reflectance as a function of fxs</returns>
        double[] ROfFx(OpticalProperties[] ops, double[] fxs);

        /// <summary>
        /// Determines reflectance at optical properties 'ops' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <returns>Reflectance as a function of fxs</returns>
        IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and spatial frequency 'fx'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="fx">spatial frequency</param>
        /// <returns>Reflectance at spatial frequency fx</returns>
        double ROfFx(IOpticalPropertyRegion[] regions, double fx);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <returns>Reflectance at spatial frequency</returns>
        double[] ROfFx(IOpticalPropertyRegion[] regions, double[] fxs);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <returns>Reflectance as a function of fxs</returns>
        IEnumerable<double> ROfFx(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions' and spatial frequencies 'fxs'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <returns>Reflectance at spatial frequency</returns>
        double[] ROfFx(IOpticalPropertyRegion[][] regions, double[] fxs);
        #endregion

        #region ROfFxAndTime
        /// <summary>
        /// Determines reflectance at optical properties 'op', spatial frequency 'fx' and time 'time'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance at spatial frequency fx and time</returns>
        double ROfFxAndTime(OpticalProperties op, double fx, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequencies 'fxs' and time 'time'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequency (1/mm)</param>
        /// <param name="time">times (ns)</param>
        /// <returns>Reflectance as a function of fxs and time</returns>
        double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequency 'fx' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fx and times</returns>
        double[] ROfFxAndTime(OpticalProperties op, double fx, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fxs and times</returns>
        double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequency 'fx' and time 'time'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of fx and time</returns>
        double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequencies 'fxs' and time 'time'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of fxs and time</returns>
        double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double time);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequency 'fx' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fx and times</returns>
        double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fxs and times</returns>
        double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double[] times);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequencies 'fxs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fx and time</returns>
        IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequency 'fx' and time 'time'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance at spatial frequency fx</returns>
        double ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double time);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs', and time 'time'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="time">time (ns)</param>
        /// <returns>Reflectance as a function of fxs and time</returns>
        double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double time);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequency 'fx', and times 'times'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fx and times</returns>
        double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double[] times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs', and times 'times'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance at spatial freq. and time</returns>
        double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double[] times);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs', and times 'times'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance at spatial frequency and time</returns>
        IEnumerable<double> ROfFxAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs,
            IEnumerable<double> times);

        /// <summary>
        /// Determines reflectance given tissue regions 'regions',
        /// spatial frequencies 'fxs', and times 'times'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance at spatial freq. and time</returns>
        double[] ROfFxAndTime(IOpticalPropertyRegion[][] regions, double[] fxs, double[] times);
        #endregion

        #region ROfFxAndFt
        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequency 'fx' and modulation frequency 'ft'
        /// </summary>
        /// <param name="op">optical properties of the medium</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance at spatial frequency fx and modulation frequency ft</returns>
        Complex ROfFxAndFt(OpticalProperties op, double fx, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequency 'fxs' and modulation frequency 'ft'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of fxs and ft</returns>
        Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequency 'fx' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fx and fts</returns>
        Complex[] ROfFxAndFt(OpticalProperties op, double fx, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'op',
        /// spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="op">medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fxs and fts</returns>
        Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequency 'fx' and modulation frequencies 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of fx and ft</returns>
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequency 'fxs' and modulation frequency 'ft'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>Reflectance as a function of fxs and ft</returns>
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double ft);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequency 'fx' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fx and fts</returns>
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fxs and fts</returns>
        Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts);

        /// <summary>
        /// Determines reflectance at optical properties 'ops',
        /// spatial frequencies 'fxs' and time frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fxs and fts</returns>
        IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequency 'fx' and temporal frequency 'ft'
        /// </summary>
        /// <param name="regions">optical and geometrical properties of the medium for each sub-region</param>
        /// <param name="fx">spatial frequency</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>Reflectance at spatial freq. and temporal freq.</returns>
        Complex ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double ft);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs' and temporal frequency 'ft'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="ft">temporal frequencies</param>
        /// <returns>Reflectance as a function of fxs and ft</returns>
        Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double ft);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequency 'fx' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fx">spatial-frequency</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of fx and fts</returns>
        Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double[] fts);

        /// <summary>
        ///Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance at spatial freq. and temporal freq.</returns>
        Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double[] fts);

        /// <summary>
        /// Determines reflectance at tissue regions 'regions',
        /// spatial frequencies 'fxs' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance as a function of fxs and fts</returns>
        IEnumerable<Complex> ROfFxAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> fts);

        /// <summary>
        /// Determines reflectance at given tissue regions 'regions',
        /// spatial frequencies 'fxs', and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">sets of medium optical and geometrical properties for each sub-region</param>
        /// <param name="fxs">spatial frequencies</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Reflectance at spatial freq. and temporal freq.</returns>
        Complex[] ROfFxAndFt(IOpticalPropertyRegion[][] regions, double[] fxs, double[] fts);
        #endregion

        #region FluenceOfRhoAndZ
        /// <summary>
        /// Determines fluence at optical properties 'ops', source-detector separations 'rhos' and z-values 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Reflectance as a function of rhos and zs</returns>
        double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs);

        /// <summary>
        /// Determines fluence at optical properties 'ops', source-detector separations 'rhos' and z-values 'zs'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Fluence as a function of rhos and zs</returns>
        IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs);

        /// <summary>
        /// Determines fluence of tissue regions 'regions', source-detector separations 'rhos' and depth bins 'zs'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">depth bins</param>
        /// <returns>Fluence as a function of rhos and zs</returns>
        IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs);

        /// <summary>
        /// Determines fluence of tissue regions 'regions', source-detector separations 'rhos' and depth bins 'zs'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">depth bins</param>
        /// <returns>Fluence as a function of rhos and zs</returns>
        double[] FluenceOfRhoAndZ(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs);
        #endregion

        #region FluenceOfRhoAndZAndTime
        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// source-detector separations 'rhos', z-values 'zs' and times 'times'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of rhos and zs</returns>
        double[] FluenceOfRhoAndZAndTime(OpticalProperties[] ops, double[] rhos, double[] zs, double[] times);

        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// source-detector separations 'rhos', z-values 'zs' and times 'times'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Fluence as a function of rhos, zs and times</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> times);
        #endregion

        #region FluenceOfRhoAndZAndFt
        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// source-detector separations 'rhos', z-values 'zs' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties </param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of rhos, zs and fts</returns>
        Complex[] FluenceOfRhoAndZAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts);

        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// source-detector separations 'rhos', z-values 'zs' and time frequencies 'fts'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="rhos">source-detector separations (mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Fluence as a function of rhos, zs and fts</returns>
        /// <remarks>IEnumerable can be one or more values - use the .AsEnumerable() extension method (in Vts.Extensions)
        /// on single items</remarks>
        IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);

        /// <summary>
        ///  Determines fluence of tissue regions 'regions',
        /// source-detector separations 'rhos', z-values 'zs' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">depth bins</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Fluence as a function of rhos, zs and fts</returns>
        IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts);

        /// <summary>
        /// Determines fluence of tissue regions 'regions',
        /// source-detector separations 'rhos', z-values 'zs' and temporal frequencies 'fts'
        /// </summary>
        /// <param name="regions">tissue regions</param>
        /// <param name="rhos">source-detector separations</param>
        /// <param name="zs">depth bins</param>
        /// <param name="fts">temporal frequencies</param>
        /// <returns>Fluence as a function of rhos, zs and fts</returns>
        Complex[] FluenceOfRhoAndZAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs, double[] fts);
        #endregion

        #region FluenceOfFxAndZ
        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs' and z-values 'zs'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Reflectance as a function of fxs and zs</returns>
        double[] FluenceOfFxAndZ(OpticalProperties[] ops, double[] fxs, double[] zs);

        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs' and z-values 'zs'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <returns>Fluence as a function of fxs and zs</returns>
        IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs);
        #endregion

        #region FluenceOfFxAndZAndTime
        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs', z-values 'zs' and times 'times' 
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Reflectance as a function of fxs, zs and times</returns>
        double[] FluenceOfFxAndZAndTime(OpticalProperties[] ops, double[] fxs, double[] zs, double[] times);

        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs', z-values 'zs' and times 'times'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="times">times (ns)</param>
        /// <returns>Fluence as a function of fxs, zs and times</returns>
        IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> times);
        #endregion

        #region FluenceOfFxAndZAndFt
        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs', z-values 'zs' and modulation frequencies 'fts'
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fx">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Reflectance as a function of fx, zs and fts</returns>
        Complex[] FluenceOfFxAndZAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts);

        /// <summary>
        /// Determines fluence at optical properties 'ops',
        /// spatial frequencies 'fxs', z-values 'zs' and time frequencies 'fts'
        /// This method is designed to return values based on iteration with the leftmost IEnumerable&lt;double&gt;
        /// input being the top-level, and so-on such that the right-most input is at the inner-most loop
        /// </summary>
        /// <param name="ops">sets of medium optical properties</param>
        /// <param name="fxs">spatial frequencies (1/mm)</param>
        /// <param name="zs">z values (mm)</param>
        /// <param name="fts">modulation frequencies (GHz)</param>
        /// <returns>Fluence as a function of fxs, zs and fts</returns>
        IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts);
        #endregion
    }
}
