using System.Collections.Generic;
using System.Numerics;
namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Defines the methods and properties that need to be implemented by the NurbsGenerator 
    /// class and by its stub version, StubNurbsGenerator used for Unit Testing.  
    /// </summary>
    public interface INurbs
    {
        /// <summary>
        /// Defines signature for a method used to consider cousality of the photon migration.
        /// </summary>
        /// <param name="rho">source detector separation</param>
        /// <returns>minimal valid time</returns>
        double GetMinimumValidTime(double rho);

        /// <summary>
        /// Defines the signature of the method used to compute a point on a NURBS curve.
        /// </summary>
        /// <param name="point">physical coordinate of the point</param>
        /// <param name="dimension">physical dimension represented by the NURBS curve</param>
        /// <returns>Value of a point on the curve</returns>
        double ComputeCurvePoint(double point, NurbsValuesDimensions dimension);

        /// <summary>
        /// Defines the signature of the method used to compute the point on a NURBS surface.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate(rho or fx)</param>
        /// <returns>Value of a point  on the surface </returns>
        double ComputeSurfacePoint(double time, double space);

        /// <summary>
        /// Defines the signature of the method used to compute the point outside the surface range.
        /// </summary>
        /// <param name="time">time coordinate</param>
        /// <param name="space">space coordinate(rho or fx)</param>
        /// <param name="edgeValue">point calculated on a position on the limit of the surface</param>
        /// <returns>Value of a point outside the surface</returns>
        double ComputePointOutOfSurface(double time, double space, double edgeValue);

        /// <summary>
        /// Defines the signature of the method used to calculate the integral value of an
        /// isoparametric NURBS curve multiplied by an exponential function analitically.
        /// </summary>
        /// <param name="space">radial position or spatial frequency mapped to the reference spatial value</param>
        /// <param name="exponentialTerm">exponential decay due to absorption</param>
        /// <returns>integral value of an isoparametric NURBS curve</returns>
        double EvaluateNurbsCurveIntegral(double space, double exponentialTerm);

        /// <summary>
        /// Defines the signature for the method used to evaluate the FT of an isoparametric curve.
        /// </summary>
        /// <param name="space">spatial coordinate</param>
        /// <param name="expTerm">exponential coefficient</param>
        /// <param name="ft">temporal frequency</param>
        /// <returns>R(ft) at  fixed rho</returns>
        Complex EvaluateNurbsCurveFourierTransform(double space, double expTerm, double ft);

        /// <summary>
        /// Defines the signature of the method used to evaluate the tensor product control points
        /// necessary to evaluate the integral of an isoparametric curve.
        /// </summary>
        /// <param name="space_ref">spatial coordinate mapped to teh reference space</param>
        /// <returns>Tensor product control points for an isoparametric curve on a surface</returns>
        List<double[]> EvaluateTensorProductControlPoints(double space_ref);

        /// <summary>
        /// Gets the NurbsValues along the time dimension.
        /// </summary>
        NurbsValues TimeValues { get; }

        /// <summary>
        /// Gets the NurbsValues along the space dimension.
        /// </summary>
        NurbsValues SpaceValues { get; }

        /// <summary>
        /// Gets or sets the coefficients of the non vanishing B-splines over each knot span.
        /// </summary>
        List<BSplinesCoefficients> TimeKnotSpanPolynomialCoefficients { get; set; }

        double[] NativeTimes { get; }
    }
}
