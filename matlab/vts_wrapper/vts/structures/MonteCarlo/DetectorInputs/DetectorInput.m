% DETECTORINPUT Defines the input data for detector
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
        
        function input = AOfRhoAndZ(rho, z, name)
            if nargin < 3
                name = 'AOfRhoAndZ';
            end
            input.TallyType = 'AOfRhoAndZ';
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
        end 
        
        function input = ATotal(name)
            if nargin < 1
                name = 'ATotal';
            end
            input.TallyType = 'ATotal';
            input.Name = name;
        end
        % set default perturbed optical properties list to be consistent
        % with default infile which has air-tissue-air definition and
        % properties set to default infile properties
        function input = dMCdROfRhodMua(rho, name)
            if nargin < 2
                name = 'dMCdROfRhodMua';
            end
            input.TallyType = 'dMCdROfRhodMua';
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
        function input = dMCdROfRhodMus(rho, name)
            if nargin < 2
                name = 'dMCdROfRhodMus';
            end
            input.TallyType = 'dMCdROfRhodMus';
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
        function input = FluenceOfRhoAndZ(rho, z, name)
            if nargin < 3
                name = 'FluenceOfRhoAndZ';
            end
            input.TallyType = 'FluenceOfRhoAndZ';
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
        end
        
        function input = FluenceOfRhoAndZAndTime(rho, z, time, name)
            if nargin < 4
                name = 'FluenceOfRhoAndZAndTime';
            end
            input.TallyType = 'FluenceOfRhoAndZAndTime';
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
            input.Time = time;
        end
        
        function input = FluenceOfXAndYAndZ(x, y, z, name)
            if nargin < 4
                name = 'FluenceOfXAndYAndZ';
            end
            input.TallyType = 'FluenceOfXAndYAndZ';
            input.Name = name;
            input.X = x;
            input.Y = y;
            input.Z = z;
        end
        
        function input = RadianceOfRho(rho, name)
            if nargin < 2
                name = 'RadianceOfRho';
            end
            input.TallyType = 'RadianceOfRho';
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = RadianceOfRhoAndZAndAngle(rho, z, angle, name)
            if nargin < 4
                name = 'RadianceOfRhoAndZAndAngle';
            end
            input.TallyType = 'RadianceOfRhoAndZAndAngle';
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
            input.Angle = angle;
        end
        
        function input = RDiffuse(name)
            if nargin < 1
                name = 'RDiffuse';
            end
            input.TallyType = 'RDiffuse';
            input.Name = name;
        end
        
        function input = ROfAngle(angle, name)
            if nargin < 2
                name = 'ROfAngle';
            end
            input.TallyType = 'ROfAngle';
            input.Name = name;
            input.Angle = angle;
        end
        
        function input = ROfRho(rho, name)
            if nargin < 2
                name = 'ROfRho';
            end
            input.TallyType = 'ROfRho';
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = ROfRhoAndAngle(rho, angle, name)
            if nargin < 3
                name = 'ROfRhoAndAngle';
            end
            input.TallyType = 'ROfRhoAndAngle';
            input.Name = name;
            input.Rho = rho;
            input.Angle = angle;
        end
        
        function input = ROfRhoAndOmega(rho, omega, name)
            if nargin < 3
                name = 'ROfRhoAndOmega';
            end
            input.TallyType = 'ROfRhoAndOmega';
            input.Name = name;
            input.Rho = rho;
            input.Omega = omega;
        end
        
        function input = ROfRhoAndTime(rho, t, name)
            if nargin < 3
                name = 'ROfRhoAndTime';
            end
            input.TallyType = 'ROfRhoAndTime';
            input.Name = name;
            input.Rho = rho;
            input.Time = t;
        end
        
        function input = ROfFx(fx, name)
            if nargin < 2
                name = 'ROfFx';
            end
            input.TallyType = 'ROfFx';
            input.Name = name;
            input.Fx = fx;
        end
        
        function input = ROfFxAndTime(fx, t, name)
            if nargin < 3
                name = 'ROfFxAndTime';
            end
            input.TallyType = 'ROfFxAndTime';
            input.Name = name;
            input.Fx = fx;
            input.Time = t;
        end
        % set default perturbed optical properties list to be consistent
        % with default infile which has air-tissue-air definition and
        % properties set to default infile properties
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
        
        function input = pMCROfRhoAndTime(rho, t, name)
            if nargin < 3
                name = 'pMCROfRhoAndTime';
            end
            input.TallyType = 'pMCROfRhoAndTime';
            input.Name = name;
            input.Rho = rho;
            input.Time = t;
            input.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.0,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
            input.PerturbedRegionsIndices = [ 1 ];
        end
        
        function input = pMCROfFx(fx, name)
            if nargin < 2
                name = 'pMCROfFx';
            end
            input.TallyType = 'pMCROfFx';
            input.Name = name;
            input.Fx = fx;
            input.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.0,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
            input.PerturbedRegionsIndices = [ 1 ];
        end
        
        function input = pMCROfFxAndTime(fx, t, name)
            if nargin < 3
                name = 'pMCROfFxAndTime';
            end
            input.TallyType = 'pMCROfFxAndTime';
            input.Name = name;
            input.Fx = fx;
            input.Time = t;
            input.PerturbedOps = ...
                [...
                [1e-10, 0.0, 0.0, 1.0]; ...
                [0.0,   1.0, 0.8, 1.4]; ...
                [1e-10, 0.0, 0.0, 1.0]; ...
                ];
            input.PerturbedRegionsIndices = [ 1 ];
        end
        
        function input = ROfXAndYDetectorInput(x, y, name)
            if nargin < 3
                name = 'ROfXAndY';
            end
            input.TallyType = 'ROfXAndY';
            input.Name = name;
            input.X = x;
            input.Y = y;
        end
        
        function input = RSpecularDetectorInput(name)
            if nargin < 1
                name = 'RSpecular';
            end
            input.TallyType = 'RSpecular';
            input.Name = name;
        end
        
        function input = TDiffuseDetectorInput(name)
            if nargin < 1
                name = 'TDiffuse';
            end
            input.TallyType = 'TDiffuse';
            input.Name = name;
        end
        
        function input = TOfAngleDetectorInput(angle, name)
            if nargin < 2
                name = 'TOfAngle';
            end
            input.TallyType = 'TOfAngle';
            input.Name = name;
            input.Angle = angle;
        end
        
        function input = TOfRhoDetectorInput(rho, name)
            if nargin < 2
                name = 'TOfRho';
            end
            input.TallyType = 'TOfRho';
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = TOfRhoAndAngleDetectorInput(rho, angle, name)
            if nargin < 3
                name = 'TOfRhoAndAngle';
            end
            input.TallyType = 'TOfRhoAndAngle';
            input.Name = name;
            input.Rho = rho;
            input.Angle = angle;
        end
        
        function input = FromInputNET(inputNET)
            input.TallyType = char(inputNET.TallyType);
            input.Name = char(inputNET.Name);
            switch input.TallyType
                case 'AOfRhoAndZ'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Z = linspace(inputNET.Z.Start, inputNET.Z.Stop, inputNET.Z.Count);
                case 'ATotal'
                    % nothing to do here?
                case 'dMCdROfRhodMua'
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
                case 'dMCdROfRhodMus'
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
                case 'FluenceOfRhoAndZ'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Z = linspace(inputNET.Z.Start, inputNET.Z.Stop, inputNET.Z.Count);
                case 'FluenceOfRhoAndZAndTime'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Z = linspace(inputNET.Z.Start, inputNET.Z.Stop, inputNET.Z.Count);
                    input.Time = linspace(inputNET.Time.Start, inputNET.Time.Stop, inputNET.Time.Count);
                case 'FluenceOfXAndYAndZ'
                    input.X = linspace(inputNET.X.Start, inputNET.X.Stop, inputNET.X.Count);
                    input.Y = linspace(inputNET.Y.Start, inputNET.Y.Stop, inputNET.Y.Count);
                    input.Z = linspace(inputNET.Z.Start, inputNET.Z.Stop, inputNET.Z.Count);
                case 'RadianceOfRho'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                case 'RadianceOfRhoAndZAndAngle'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Z = linspace(inputNET.Z.Start, inputNET.Z.Stop, inputNET.Z.Count);
                    input.Angle = linspace(inputNET.Angle.Start, inputNET.Angle.Stop, inputNET.Angle.Count);
                case 'RDiffuse'
                    % nothing to do here?
                case 'ROfAngle'
                    input.Angle = linspace(inputNET.Angle.Start, inputNET.Angle.Stop, inputNET.Angle.Count);
                case 'ROfRho'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                case 'ROfRhoAndAngle'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Angle = linspace(inputNET.Angle.Start, inputNET.Angle.Stop, inputNET.Angle.Count);
                case 'ROfRhoAndOmega'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Omega = linspace(inputNET.Omega.Start, inputNET.Omega.Stop, inputNET.Omega.Count);
                case 'ROfRhoAndTime'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Time = linspace(inputNET.Time.Start, inputNET.Time.Stop, inputNET.Time.Count);
                case 'ROfFx'
                    input.Fx = linspace(inputNET.Fx.Start, inputNET.Fx.Stop, inputNET.Fx.Count);
                case 'ROfFxAndTime'
                    input.Fx = linspace(inputNET.Fx.Start, inputNET.Fx.Stop, inputNET.Fx.Count);
                    input.Time = linspace(inputNET.Time.Start, inputNET.Time.Stop, inputNET.Time.Count);
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
                case 'pMCROfRhoAndTime'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Time = linspace(inputNET.Time.Start, inputNET.Time.Stop, inputNET.Time.Count);
                    
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
                case 'pMCROfFx'
                    input.Fx = linspace(inputNET.Fx.Start, inputNET.Fx.Stop, inputNET.Fx.Count);
                    
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
                case 'pMCROfFxAndTime'
                    input.Fx = linspace(inputNET.Fx.Start, inputNET.Fx.Stop, inputNET.Fx.Count);
                    input.Time = linspace(inputNET.Time.Start, inputNET.Time.Stop, inputNET.Time.Count);
                    
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
                case 'ROfXAndY'
                    input.X = linspace(inputNET.X.Start, inputNET.X.Stop, inputNET.X.Count);
                    input.Y = linspace(inputNET.Y.Start, inputNET.Y.Stop, inputNET.Y.Count);
                case 'RSpecular'
                    % nothing to do here?
                case 'TDiffuse'
                    % nothing to do here?
                case 'TOfAngle'
                    input.Angle = linspace(inputNET.Angle.Start, inputNET.Angle.Stop, inputNET.Angle.Count);
                case 'TOfRho'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                case 'TOfRhoAndAngle'
                    input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
                    input.Angle = linspace(inputNET.Angle.Start, inputNET.Angle.Stop, inputNET.Angle.Count);
                otherwise
                    disp('Unknown DetectorInput type');
            end
        end
        
        function inputNET = ToInputNET(input)
            switch input.TallyType
                case 'AOfRhoAndZ'
                    inputNET = Vts.MonteCarlo.AOfRhoAndZDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z)), ...
                        input.Name ...
                        );
                case 'ATotal'
                    inputNET = Vts.MonteCarlo.ATotalDetectorInput( ...
                        input.Name ...
                        );
                case 'dMCdROfRhodMua'
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
                    inputNET = Vts.MonteCarlo.dMCdROfRhodMuaDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                case 'dMCdROfRhodMus'
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
                    inputNET = Vts.MonteCarlo.dMCdROfRhodMusDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                case 'FluenceOfRhoAndZ'
                    inputNET = Vts.MonteCarlo.FluenceOfRhoAndZDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z)), ...
                        input.Name ...
                        );
                case 'FluenceOfRhoAndZAndTime'
                    inputNET = Vts.MonteCarlo.FluenceOfRhoAndZAndTimeDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z)), ...
                        Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time)), ...
                        input.Name ...
                        );
                case 'FluenceOfXAndYAndZ'
                    inputNET = Vts.MonteCarloFluenceOfXAndYAndZDetectorInput( ...
                        Vts.Common.DoubleRange(input.X(1), input.X(end), length(input.X)), ...
                        Vts.Common.DoubleRange(input.Y(1), input.Y(end), length(input.Y)), ...
                        Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z)), ...
                        input.Name ...
                        );
                case 'RadianceOfRho'
                    inputNET = Vts.MonteCarlo.RadianceOfRhoDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        input.Name ...
                        );
                case 'RadianceOfRhoAndZAndAngle'
                    inputNET = Vts.MonteCarlo.RadianceOfRhoAndZAndAngleDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z)), ...
                        Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle)), ...
                        input.Name ...
                        );
                case 'RDiffuse'
                    inputNET = Vts.MonteCarlo.RDiffuseDetectorInput( ...
                        input.Name ...
                        );
                case 'ROfAngle'
                    inputNET = Vts.MonteCarlo.ROfAngleDetectorInput( ...
                        Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle)), ...
                        input.Name ...
                        );
                case 'ROfRho'
                    inputNET = Vts.MonteCarlo.ROfRhoDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        input.Name ...
                        );
                case 'ROfRhoAndAngle'
                    inputNET = Vts.MonteCarlo.ROfRhoAndAngleDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle)), ...
                        input.Name ...
                        );
                case 'ROfRhoAndOmega'
                    inputNET = Vts.MonteCarlo.ROfRhoAndOmegaDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Omega(1), input.Omega(end), length(input.Omega)), ...
                        input.Name ...
                        );
                case 'ROfRhoAndTime'
                    inputNET = Vts.MonteCarlo.ROfRhoAndTimeDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time)), ...
                        input.Name ...
                        );
                case 'ROfFx'
                    inputNET = Vts.MonteCarlo.ROfFxDetectorInput( ...
                        Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx)), ...
                        input.Name ...
                        );
                case 'ROfFxAndTime'
                    inputNET = Vts.MonteCarlo.ROfFxAndTimeDetectorInput( ...
                        Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx)), ...
                        Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time)), ...
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
                case 'pMCROfRhoAndTime'
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
                    inputNET = Vts.MonteCarlo.pMCROfRhoAndTimeDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                    
                case 'pMCROfFx'
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
                    inputNET = Vts.MonteCarlo.pMCROfFxDetectorInput( ...
                        Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                case 'pMCROfFxAndTime'
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
                    inputNET = Vts.MonteCarlo.pMCROfFxAndTimeDetectorInput( ...
                        Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx)), ...
                        Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time)), ...
                        perturbedOpsNET, ...
                        perturbedRegionsIndicesNET, ...
                        input.Name ...
                        );
                case 'ROfXAndY'
                    inputNET = Vts.MonteCarlo.ROfXAndYDetectorInput( ...
                        Vts.Common.DoubleRange(input.X(1), input.X(end), length(input.X)), ...
                        Vts.Common.DoubleRange(input.Y(1), input.Y(end), length(input.Y)), ...
                        input.Name ...
                        );
                case 'RSpecular'
                    inputNET = Vts.MonteCarlo.RSpecularDetectorInput( ...
                        input.Name ...
                        );
                case 'TDiffuse'
                    inputNET = Vts.MonteCarlo.TDiffuseDetectorInput( ...
                        input.Name ...
                        );
                case 'TOfAngle'
                    inputNET = Vts.MonteCarlo.TOfAngleDetectorInput( ...
                        Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle)), ...
                        input.Name ...
                        );
                case 'TOfRho'
                    inputNET = Vts.MonteCarlo.TOfRhoDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        input.Name ...
                        );
                case 'TOfRhoAndAngle'
                    inputNET = Vts.MonteCarlo.TOfRhoAndAngleDetectorInput( ...
                        Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                        Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle)), ...
                        input.Name ...
                        );
                otherwise
                    disp('Unknown DetectorInput type');
            end
            
        end
    end
end