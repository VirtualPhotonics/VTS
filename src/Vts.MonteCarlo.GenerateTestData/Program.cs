using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.MonteCarlo.GenerateTestData
{
    class Program
    {
        static void Main(string[] args)
        {
            //PointSourceTwoLayer.GeneratePointSourceTwoLayerManagedData();
            LineSourceTwoRegionSphere.GenerateLineSourceTwoRegionSphereManagedData();
            //LineSourceTwoRegionSphere.GenerateLineSourceTwoRegionSphereUnManagedData();
        }
    }
}
