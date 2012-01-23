classdef DetectorInput
    methods (Static)
        function input = Default()
            % detector tally identifier
            input.TallyType = 'ROfRho';
            % detector name, defaults to 'ROfRho' but can be user specified
            input.Name = 'ROfRho';
            % detector rho binning
            input.Rho = linspace(0, 10, 101);
        end
        
        function input = ROfRho(rho, name)
            if nargin < 2
                name = 'ROfRho';
            end
            input.TallyType = 'ROfRho';
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = pMCROfRho(rho, name)
            if nargin < 2
                name = 'pMCROfRho';
            end
            input.TallyType = 'pMCROfRho';
            input.Name = name;
            input.Rho = rho;
            input.PerturbedOps = ...
                [...
                 [1e-10, 0.0, 0.0, 1.0]; ...
                 [0.0,   1.0, 0.8, 1.4]; ...
                 [1e-10, 0.0, 0.0, 1.0]; ...
                ];
            input.PerturbedRegionsIndices = [ 1 ];
        end
        
        function input = FromInputNET(inputNET)
            input.TallyType = char(inputNET.TallyType);
            input.Name = char(inputNET.Name);
            switch input.TallyType
                case 'ROfRho'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                case 'pMCROfRho'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    
                    % for some reason, if the underlying object is an array
                    % (vs List), "Count" method will fail (and vice versa).
                    % So, I'm using Linq to make sure I get the right
                    % one...should probably make this a global helper
                    nPerturbedOps = NET.invokeGenericMethod('System.Linq.Enumerable', 'Count', ...
                        {'Vts.OpticalProperties'}, inputNET.PerturbedOps);
                    nPerturbedRegionsIndices = NET.invokeGenericMethod('System.Linq.Enumerable', 'Count', ...
                        {'System.Int32'}, inputNET.PerturbedRegionsIndices);              
                    
                    input.PerturbedOps = zeros([nPerturbedOps 4]);
                    input.PerturbedRegionsIndices = zeros([nPerturbedRegionsIndices 1]);   
                    perturbedOpsNET = inputNET.PerturbedOps;                 
                    for i=1:size(input.PerturbedOps,1)
                        input.PerturbedOps(i, 1) = perturbedOpsNET(i).Mua;
                        input.PerturbedOps(i, 2) = perturbedOpsNET(i).Musp;
                        input.PerturbedOps(i, 3) = perturbedOpsNET(i).G;
                        input.PerturbedOps(i, 4) = perturbedOpsNET(i).N;
                    end
                    perturbedRegionsIndicesNET = inputNET.PerturbedRegionsIndices;
                    for i=1:length(input.PerturbedRegionsIndices)
                        input.PerturbedRegionsIndices(i) = perturbedRegionsIndicesNET(i);
                    end                    
                otherwise
                    disp('Unknown DetectorInput type');
            end
        end
        
        function inputNET = ToInputNET(input)
            switch input.TallyType
                case 'ROfRho'
                    inputNET = Vts.MonteCarlo.ROfRhoDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        input.Name ...
                        );
                case 'pMCROfRho'
                    perturbedOpsNET = NET.createArray('Vts.OpticalProperties', size(input.PerturbedOps,1));
                    perturbedRegionsIndicesNET = NET.createArray('System.Int32', length(input.PerturbedRegionsIndices));
                    for i=1:size(input.PerturbedOps,1)
                        perturbedOpsNET(i) = Vts.OpticalProperties( ...
                            input.PerturbedOps(i,1), ...
                            input.PerturbedOps(i,2), ...
                            input.PerturbedOps(i,3), ...
                            input.PerturbedOps(i,4) ...
                            );
                    end
                    for i=1:length(input.PerturbedRegionsIndices)
                        perturbedRegionsIndicesNET(i) = input.PerturbedRegionsIndices(i);
                    end
                    inputNET = Vts.MonteCarlo.pMCROfRhoDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                otherwise
                    disp('Unknown DetectorInput type');
            end
            
        end
    end
end