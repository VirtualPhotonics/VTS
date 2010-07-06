using System;
using System.Collections.Generic;

namespace Vts.Modeling
{
    public interface IBoundaryCondition
    {
        Func<IEnumerable<double>, IEnumerable<double>> SetBoundaryCondition(BoundaryConditionType bct);
        //FuncWithParams<IList<double>,IEnumerable<double>>
        //    ReturnBoundaryConditionFunction(BoundaryConditionType bct, 
        //    MediumType mt);
    }
}
