using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// PhotonStateType is a bitmap of Photon.StateFlag.  Combinations of bits indicate
    /// the current state of the photon.  These states communicate what to do with the photon.
    /// ref: http://www.codeproject.com/Articles/37921/Enums-Flags-and-Csharp-Oh-my-bad-pun.aspx
    /// or http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations
    ///     |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |
    ///   8000 4000 2000 1000 0800 0400 0200 0100 0080 0040 0020 0010 0008 0004 0002 0001
    ///   &lt;- transport flags                                                           -&gt;
    ///   &lt;- virtual flags these with "0000" added in lowest bits                      -&gt;
    /// </summary>
    [Flags]
    public enum PhotonStateType
    {
        /// <summary>
        /// no bits set
        /// </summary>
        None = 0x0,
        // transport flags
        /// <summary>
        /// photon alive
        /// </summary>
        Alive = 0x1,
        /// <summary>
        /// photon exited domain
        /// </summary>
        ExitedDomain = 0x2, // do I need this?
        /// <summary>
        /// photon was absorbed, used only in analog random walk process
        /// </summary>
        Absorbed = 0x4,
        /// <summary>
        /// photon killed because path length too long
        /// </summary>
        KilledOverMaximumPathLength = 0x8,
        /// <summary>
        /// photon killed because number of collisions over maximum
        /// </summary>
        KilledOverMaximumCollisions = 0x10,
        /// <summary>
        /// photon killed by Russian Roulette
        /// </summary>
        KilledRussianRoulette = 0x20,
        // the following get set during photon transport in tissue
        /// <summary>
        /// photon pseudo-collision at reflected tissue boundary
        /// </summary>
        PseudoReflectedTissueBoundary = 0x40,
        /// <summary>
        /// photon pseudo-collision at transmitted tissue boundary
        /// </summary>
        PseudoTransmittedTissueBoundary = 0x80,
        /// <summary>
        /// photon pseudo-collision at specular tissue boundary
        /// </summary>
        PseudoSpecularTissueBoundary = 0x100,
        /// <summary>
        /// photon pseudo-collision at bounding volume boundary
        /// </summary>
        PseudoBoundingVolumeTissueBoundary = 0x200,

        // virtual boundary flags, can we 1-1 map to virtualBoundary "Name"
        // move up to 16th position
        // the following get set when VB hit (after hit tissue boundary)
        /// <summary>
        /// photon pseudo-collision at DiffuseReflectance Virtual Boundary (VB)
        /// </summary>
        PseudoDiffuseReflectanceVirtualBoundary = 0x10000,
        /// <summary>
        /// photon pseudo-collision at DiffuseTransmittance Virtual Boundary (VB)
        /// </summary>
        PseudoDiffuseTransmittanceVirtualBoundary = 0x20000,
        /// <summary>
        /// photon pseudo-collision at SpecularReflectance Virtual Boundary (VB)
        /// </summary>
        PseudoSpecularReflectanceVirtualBoundary = 0x40000,
        /// <summary>
        /// photon pseudo-collision at Generic Volume Virtual Boundary (VB)
        /// </summary>
        PseudoGenericVolumeVirtualBoundary = 0x80000,
        /// <summary>
        /// photon pseudo-collision at Internal Surface Virtual Boundary (VB)
        /// </summary>
        PseudoSurfaceRadianceVirtualBoundary = 0x100000,
        /// <summary>
        /// photon pseudo-collision at Lateral Bounding Virtual Boundary (VB)
        /// </summary>
        PseudoLateralBoundingVirtualBoundary = 0x110000,
    }
}