using Vts.Gui.Silverlight.Resources;

namespace Vts.Gui.Silverlight.Extensions
{
    /// <summary>
    /// Class for looking up strings from XAML
    /// </summary>
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static Strings _resource = new Strings();

        /// <summary>
        /// MainResource pulls the relevant string from resources
        /// </summary>
        public Strings MainResource { get { return _resource; } }
    }
}
