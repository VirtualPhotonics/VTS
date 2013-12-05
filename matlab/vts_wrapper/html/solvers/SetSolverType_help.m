%% SetSolverType
% Sets the solver type
%
%% Syntax
%  SetSolverType(SOLVERTYPE) 
%
%       SOLVERTYPE is a string representing the type of solver
%           eg. SOLVERTYPE = 'PointSourceSDA';
%
%% Description
% Sets the solver type to one of the following:
%
% * *PointSourceSDA* - Standard Diffusion Approximation (SDA) with point source forward solver  
% * *DistributedPointSourceSDA* - Standard Diffusion Approximation (SDA) with distributed point source forward solver  
% * *DistributedGaussianSourceSDA* - Standard Diffusion Approximation (SDA) with Gaussian distributed source forward solver  
% * *DeltaPOne* - delta-P1 forward solver *(NOT IMPLEMENTED)*
% * *MonteCarlo* - scaled Monte Carlo forward solver  
% * *Nurbs* - scaled Monte Carlo forward solver with non-uniform rational b-splines forward solver  
%
%% Examples
%       op = [0.01 1 0.8 1.4]; % optical properties
%       rho = 0.5:0.05:9.5; %s-d separation, in mm
%       VtsSolvers.SetSolverType('PointSourceSDA'); % set solver type
%       reflectance = VtsSolvers.ROfRho(op, rho);
%
%% See Also
% <VtsSolvers_help.html VtsSolvers> | 
% <AbsorbedEnergyOfRhoAndZ_help.html AbsorbedEnergyOfRhoAndZ> | 
% <FluenceOfRhoAndZ_help.html FluenceOfRhoAndZ> | 
% <PHDOfRhoAndZ_help.html PHDOfRhoAndZ> | 
% <ROfFx_help.html ROfFx> | 
% <ROfFxAndFt_help.html ROfFxAndFt> |
% <ROfFxAndT_help.html ROfFxAndT> | 
% <ROfRho_help.html ROfRho> |
% <ROfRhoAndFt_help.html ROfRhoAndFt> |
% <ROfRhoAndT_help.html ROfRhoAndT>