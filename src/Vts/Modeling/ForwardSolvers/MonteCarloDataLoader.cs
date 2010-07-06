using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Vts.IO;
using Vts;
using Vts.MonteCarlo;

namespace Vts.Modeling
{
    public static class MonteCarloDataLoader
    {
        public static Output GetOutputFromFolder(string folder)
        {
            return Output.FromFolderInResources("Resources/" + folder, "Vts.Modeling");
        }
    }
}
