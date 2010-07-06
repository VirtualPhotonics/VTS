namespace Vts
{
    public interface IOldForwardSolver
    {
        /// <summary>
        /// Determines reflectance at source-detector separation rho
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <returns>reflectance at source-detector separation rho</returns>
        double RofRho(double mua, double musp, double n, double rho);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and time t
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at source-detector separation rho and time t</returns>
        double RofRhoAndT(double mua, double musp, double n, double rho, double t);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and modulation frequency ft
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at source-detector separation rho and modulation frequency ft</returns>
        double RofRhoAndFt(double mua, double musp, double n, double rho, double ft);

        /// <summary>
        /// Determines reflectance at spatial frequency fx
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at spatial frequency fx</returns>
        double RofFx(double mua, double musp, double n, double fx);

        /// <summary>
        /// Determines reflectance at spatial frequency fx and time t
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at spatial frequency fx and time t</returns>
        double RofFxAndT(double mua, double musp, double n, double fx, double t);
        
        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at spatial frequency fx and modulation frequency ft</returns>
        double RofFxAndFt(double mua, double musp, double n, double fx, double ft);

        // should we add these (so we can consolidate the analysis/inversion methods)?
        //double RofRho(params double[] arguments);
        //double RofFxAndT(params double[] arguments);
        //double RofFx(params double[] arguments);
        //double RofRhoAndT(params double[] arguments);
        //double RofRhoAndFt(params double[] arguments);
        //double RofFxAndFt(params double[] arguments);
    }
}
    //double RofRho(double mua, double musp, double n, double rho);
    //double RofRhoAndT(double mua, double musp, double n, double rho, double t);
    //double RofRhoAndFt(double mua, double musp, double n, double rho, double ft);
    //double RofFx(double mua, double musp, double n, double fx);
    //double RofFxAndT(double mua, double musp, double n, double fx, double t);
    //double RofFxAndFt(double mua, double musp, double n, double fx, double ft);

    //double RofRho(OpticalProperties op, double rho);
    //double RofRhoAndT(OpticalProperties op, double t);
    //double RofRhoAndFt(OpticalProperties op, double ft);
    //double RofFx(OpticalProperties op, double fx);
    //double RofFxAndT(OpticalProperties op, double t);
    //double RofFxAndFt(OpticalProperties op, double ft);