namespace Vts
{
    /// <summary>
    /// Available choices for mapping grayscale intensity.  These names taken from matlab.
    /// </summary>
    public enum ColormapType
    {
        /// <summary>
        /// varies smoothly from black through shades of red, orange, and yellow, to white
        /// </summary>
        Hot,
        /// <summary>
        /// ranges from blue to red, and passes through the colors cyan, yellow, and orange
        /// </summary>
        Jet,
        /// <summary>
        /// linear grayscale
        /// </summary>
        Gray,
        /// <summary>
        /// varies the hue component of the hue-saturation-value color model
        /// </summary>
        HSV,
        /// <summary>
        /// grayscale colormap with a higher value for the blue component
        /// </summary>
        Bone,
        /// <summary>
        /// varies smoothly from black to bright copper
        /// </summary>
        Copper,
        /// <summary>
        /// map is digitize to two colors (white and black)
        /// </summary>
        Binary,
    }
}