using SLExtensions.Input;

namespace Vts.Modeling.Input
{
    public static class Commands
    {
        static Commands()
        {
            Modeling_SetGaussianBeamSize = new Command("Modeling_SetGaussianBeamSize");
        }

        public static Command Modeling_SetGaussianBeamSize { get; private set; }

    }
}
