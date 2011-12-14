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
        double ROfRho(double mua, double musp, double n, double rho);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and time t
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at source-detector separation rho and time t</returns>
        double ROfRhoAndT(double mua, double musp, double n, double rho, double t);

        /// <summary>
        /// Determines reflectance at source-detector separation rho and modulation frequency ft
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="rho">source-detector separation (mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at source-detector separation rho and modulation frequency ft</returns>
        double ROfRhoAndFt(double mua, double musp, double n, double rho, double ft);

        /// <summary>
        /// Determines reflectance at spatial frequency fx
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <returns>reflectance at spatial frequency fx</returns>
        double ROfFx(double mua, double musp, double n, double fx);

        /// <summary>
        /// Determines reflectance at spatial frequency fx and time t
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="t">time (ns)</param>
        /// <returns>reflectance at spatial frequency fx and time t</returns>
        double ROfFxAndT(double mua, double musp, double n, double fx, double t);
        
        /// <summary>
        /// Determines reflectance at spatial frequency fx and modulation frequency ft
        /// </summary>
        /// <param name="mua">absorption coefficient (1/mm)</param>
        /// <param name="musp">reduced scattering coefficient (1/mm)</param>
        /// <param name="n">refractive index</param>
        /// <param name="fx">spatial frequency (1/mm)</param>
        /// <param name="ft">modulation frequency (GHz)</param>
        /// <returns>reflectance at spatial frequency fx and modulation frequency ft</returns>
        double ROfFxAndFt(double mua, double musp, double n, double fx, double ft);

        // should we add these (so we can consolidate the analysis/inversion methods)?
        //double ROfRho(params double[] arguments);
        //double ROfFxAndT(params double[] arguments);
        //double ROfFx(params double[] arguments);
        //double ROfRhoAndT(params double[] arguments);
        //double ROfRhoAndFt(params double[] arguments);
        //double ROfFxAndFt(params double[] arguments);
    }
}
    //double ROfRho(double mua, double musp, double n, double rho);
    //double ROfRhoAndT(double mua, double musp, double n, double rho, double t);
    //double ROfRhoAndFt(double mua, double musp, double n, double rho, double ft);
    //double ROfFx(double mua, double musp, double n, double fx);
    //double ROfFxAndT(double mua, double musp, double n, double fx, double t);
    //double ROfFxAndFt(double mua, double musp, double n, double fx, double ft);

    //double ROfRho(OpticalProperties op, double rho);
    //double ROfRhoAndT(OpticalProperties op, double t);
    //double ROfRhoAndFt(OpticalProperties op, double ft);
    //double ROfFx(OpticalProperties op, double fx);
    //double ROfFxAndT(OpticalProperties op, double t);
    //double ROfFxAndFt(OpticalProperties op, double ft);