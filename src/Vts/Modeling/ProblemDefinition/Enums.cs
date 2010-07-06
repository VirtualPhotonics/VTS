namespace Vts.Modeling
{
    public enum SourceType
    {
        Point,
        DistributedLine,
        DistributedGaussian,
        Gaussian,
    }

    public enum MediumType
    {
        SemiInfinite,
        Infinite,
        Slab,
        Layered,
    }

    public enum BoundaryConditionType
    {
        Extrapolated,
        Zero,
        PartialCurrent,
    }

    public enum MeasurementType
    {
        Reflectance,
        Transmittance,
        Fluence,
        EnergyDensity,
    }

}
