using System;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Modeling
{
    public interface ISource
    {
        Func<double,double> SetSourceFunction(SourceType st, DiffusionParameters dps);

        //Func<Func<double>, double,
    }
}
