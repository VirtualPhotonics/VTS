using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using System.ComponentModel;


namespace Vts.Modeling.ProblemDefinition
{
    public interface IDiffusionProblemDefinition
    {
        Func<OpticalProperties, IList<double>, double>
            GetDiffuseDistributionFunction(MediumType mt, BoundaryConditionType bct);
        

     
    }
}
