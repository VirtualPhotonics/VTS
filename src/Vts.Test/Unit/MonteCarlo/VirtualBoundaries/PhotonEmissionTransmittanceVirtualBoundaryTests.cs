using MathNet.Numerics;
using NUnit.Framework;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.Test.Unit.MonteCarlo.VirtualBoundaries;

[TestFixture]
public class PhotonEmissionTransmittanceVirtualBoundaryTests
{
    private IVirtualBoundary _virtualBoundary;

    /// <summary>
    /// setup tissue
    /// </summary>
    [OneTimeSetUp]
    public void SetUp()
    {
        var tissue = new MultiLayerTissue(
            new ITissueRegion[]
            {
                new LayerTissueRegion(
                    new DoubleRange(double.NegativeInfinity, 0.0),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                new LayerTissueRegion(
                    new DoubleRange(0.0, 100.0),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                new LayerTissueRegion(
                    new DoubleRange(100.0, double.PositiveInfinity),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
            }
        );
        var detectorController = new DetectorController(new List<IDetector>());
        var virtualBoundaryName = VirtualBoundaryType.PhotonEmissionTransmittance.ToString();

        _virtualBoundary = new PhotonEmissionTransmittanceVirtualBoundary(
            tissue, detectorController, virtualBoundaryName);

    }

    /// <summary>
    /// Test to verify that constructor is working correctly for this virtual boundary.
    /// </summary>
    [Test]
    public void Validate_constructor()
    {
        Assert.That(_virtualBoundary.VirtualBoundaryType, Is.EqualTo(VirtualBoundaryType.PhotonEmissionTransmittance));
        Assert.That(_virtualBoundary.Name, Is.EqualTo(VirtualBoundaryType.PhotonEmissionTransmittance.ToString()));
        Assert.That(_virtualBoundary.DetectorController.Detectors, Is.Empty);
    }

    /// <summary>
    /// Test to verify method GetDistanceToVirtualBoundary
    /// </summary>
    [Test]
    public void Validate_GetDistanceToVirtualBoundary()
    {
        // put photon at bottom of tissue pointing down
        var photonDataPoint = new PhotonDataPoint(
            new Position(0, 0, 100),
            new Direction(0, 0, 1),
            1.0,
            0.5,
            PhotonStateType.PseudoTransmittedTissueBoundary);
        var distance = _virtualBoundary.GetDistanceToVirtualBoundary(photonDataPoint);
        Assert.That(distance, Is.EqualTo(0.0));
        // put photon at bottom of tissue pointing up
        photonDataPoint = new PhotonDataPoint(
            new Position(0, 0, 100),
            new Direction(0, 0, -1),
            1.0,
            0.5,
            PhotonStateType.PseudoTransmittedTissueBoundary);
        distance = _virtualBoundary.GetDistanceToVirtualBoundary(photonDataPoint);
        Assert.That(distance, Is.EqualTo(double.PositiveInfinity));
        // put photon at surface pointing up
        photonDataPoint = new PhotonDataPoint(
            new Position(0, 0, 0),
            new Direction(0, 0, -1),
            1.0,
            0.5,
            PhotonStateType.PseudoReflectedTissueBoundary);
        distance = _virtualBoundary.GetDistanceToVirtualBoundary(photonDataPoint);
        Assert.That(distance, Is.EqualTo(double.PositiveInfinity));
    }
}