% DETECTORINPUT Defines the input data for detector
classdef DetectorInput
    methods (Static)
        function input = Default()
            % detector tally identifier
            input.TallyType = 'ROfRho';
            % detector name, defaults to 'ROfRho' but can be user specified
            input.Name = 'ROfRho';
            % boolean to specify if second moment should also be calculated
            input.TallySecondMoment = false;
            % boolean to specify if second moment should also be calculated
            input.TallyDetails = TallyDetails.Default();
            % detector rho binning
            input.Rho = linspace(0, 10, 101);
        end
        
        function input = AOfRhoAndZ(rho, z, name)
            if nargin < 3
                name = 'AOfRhoAndZ';
            end
            sampleInput = Vts.MonteCarlo.Detectors.AOfRhoAndZDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
        end 
        
        function input = ATotal(name)
            if nargin < 1
                name = 'ATotal';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ATotalDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
        end
        % set default perturbed optical properties list to be consistent
        % with default infile which has air-tissue-air definition and
        % properties set to default infile properties
        function input = dMCdROfRhodMua(rho, name)
            if nargin < 2
                name = 'dMCdROfRhodMua';
            end
            sampleInput = Vts.MonteCarlo.Detectors.dMCdROfRhodMuaDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.dMCdROfRhodMusDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.FluenceOfRhoAndZDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
        end
        
        function input = FluenceOfRhoAndZAndTime(rho, z, time, name)
            if nargin < 4
                name = 'FluenceOfRhoAndZAndTime';
            end
            sampleInput = Vts.MonteCarlo.Detectors.FluenceOfRhoAndZAndTimeDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
            input.Time = time;
        end
        
        function input = FluenceOfXAndYAndZ(x, y, z, name)
            if nargin < 4
                name = 'FluenceOfXAndYAndZ';
            end
            sampleInput = Vts.MonteCarlo.Detectors.FluenceOfXAndYAndZDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.X = x;
            input.Y = y;
            input.Z = z;
        end
        
        function input = RadianceOfRho(rho, name)
            if nargin < 2
                name = 'RadianceOfRho';
            end
            sampleInput = Vts.MonteCarlo.Detectors.RadianceOfRhoDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = RadianceOfRhoAndZAndAngle(rho, z, angle, name)
            if nargin < 4
                name = 'RadianceOfRhoAndZAndAngle';
            end
            sampleInput = Vts.MonteCarlo.Detectors.RadianceOfRhoAndZAndAngleDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Z = z;
            input.Angle = angle;
        end
        
        function input = RDiffuse(name)
            if nargin < 1
                name = 'RDiffuse';
            end
            sampleInput = Vts.MonteCarlo.Detectors.RDiffuseDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
        end
        
        function input = ROfAngle(angle, name)
            if nargin < 2
                name = 'ROfAngle';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfAngleDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Angle = angle;
        end
        
        function input = ROfRho(rho, name)
            if nargin < 2
                name = 'ROfRho';
            end
            input.Name = name;
            input.Rho = rho;
            sampleInput = Vts.MonteCarlo.Detectors.ROfRhoDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
        end
        
        function input = ROfRhoAndAngle(rho, angle, name)
            if nargin < 3
                name = 'ROfRhoAndAngle';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfRhoAndAngleDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Angle = angle;
        end
        
        function input = ROfRhoAndOmega(rho, omega, name)
            if nargin < 3
                name = 'ROfRhoAndOmega';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfRhoAndOmegaDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Omega = omega;
        end
        
        function input = ROfRhoAndTime(rho, t, name)
            if nargin < 3
                name = 'ROfRhoAndTime';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfRhoAndTimeDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Time = t;
        end
        
        function input = ROfFx(fx, name)
            if nargin < 2
                name = 'ROfFx';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfFxDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Fx = fx;
        end
        
        function input = ROfFxAndTime(fx, t, name)
            if nargin < 3
                name = 'ROfFxAndTime';
            end
            sampleInput = Vts.MonteCarlo.Detectors.ROfFxAndTimeDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.pMCROfRhoDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.pMCROfRhoAndTimeDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.pMCROfFxDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.pMCROfFxAndTimeDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
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
            sampleInput = Vts.MonteCarlo.Detectors.ROfXAndYDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.X = x;
            input.Y = y;
        end
        
        function input = RSpecularDetectorInput(name)
            if nargin < 1
                name = 'RSpecular';
            end
            sampleInput = Vts.MonteCarlo.Detectors.RSpecularDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
        end
        
        function input = TDiffuseDetectorInput(name)
            if nargin < 1
                name = 'TDiffuse';
            end
            sampleInput = Vts.MonteCarlo.Detectors.TDiffuseDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
        end
        
        function input = TOfAngleDetectorInput(angle, name)
            if nargin < 2
                name = 'TOfAngle';
            end
            sampleInput = Vts.MonteCarlo.Detectors.TOfAngleDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Angle = angle;
        end
        
        function input = TOfRhoDetectorInput(rho, name)
            if nargin < 2
                name = 'TOfRho';
            end
            sampleInput = Vts.MonteCarlo.Detectors.TOfRhoDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
        end
        
        function input = TOfRhoAndAngleDetectorInput(rho, angle, name)
            if nargin < 3
                name = 'TOfRhoAndAngle';
            end
            sampleInput = Vts.MonteCarlo.Detectors.TOfRhoAndAngleDetectorInput;
            input.TallyType = sampleInput.TallyType;
            input.TallySecondMoment = sampleInput.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(sampleInput.TallyDetails);
            input.Name = name;
            input.Rho = rho;
            input.Angle = angle;
        end
        
        function input = FromInputNET(inputNET)
            input.TallyType = char(inputNET.TallyType);
            input.Name = char(inputNET.Name);
            input.TallySecondMoment = inputNET.TallySecondMoment;
            input.TallyDetails = TallyDetails.FromDetailsNET(inputNET.TallyDetails);
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
            % convert to string with char fix for certain matlab versions
            tallyType = char(input.TallyType);
            switch tallyType
                case 'AOfRhoAndZ'
                    inputNET = Vts.MonteCarlo.Detectors.AOfRhoAndZDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Z = Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ATotal'
                    inputNET = Vts.MonteCarlo.ATotalDetectorInput;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
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
                    inputNET = Vts.MonteCarlo.Detectors.dMCdROfRhodMuaDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
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
                    inputNET = Vts.MonteCarlo.Detectors.dMCdROfRhodMusDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'FluenceOfRhoAndZ'
                    inputNET = Vts.MonteCarlo.Detectors.FluenceOfRhoAndZDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Z = Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'FluenceOfRhoAndZAndTime'
                    inputNET = Vts.MonteCarlo.Detectors.FluenceOfRhoAndZAndTimeDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Z = Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z));
                    inputNET.Time = Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'FluenceOfXAndYAndZ'
                    inputNET = Vts.MonteCarlo.Detectors.FluenceOfXAndYAndZDetectorInput;
                    inputNET.X = Vts.Common.DoubleRange(input.X(1), input.X(end), length(input.X));
                    inputNET.Y = Vts.Common.DoubleRange(input.Y(1), input.Y(end), length(input.Y));
                    inputNET.Z = Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'RadianceOfRho'
                    inputNET = Vts.MonteCarlo.Detectors.RadianceOfRhoDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'RadianceOfRhoAndZAndAngle'
                    inputNET = Vts.MonteCarlo.Detectors.RadianceOfRhoAndZAndAngleDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Z = Vts.Common.DoubleRange(input.Z(1), input.Z(end), length(input.Z));
                    inputNET.Angle = Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'RDiffuse'
                    inputNET = Vts.MonteCarlo.Detectors.RDiffuseDetectorInput;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfAngle'
                    inputNET = Vts.MonteCarlo.Detectors.ROfAngleDetectorInput;
                    inputNET.Angle = Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfRho'
                    inputNET = Vts.MonteCarlo.Detectors.ROfRhoDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfRhoAndAngle'
                    inputNET = Vts.MonteCarlo.Detectors.ROfRhoAndAngleDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Angle = Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfRhoAndOmega'
                    inputNET = Vts.MonteCarlo.Detectors.ROfRhoAndOmegaDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Omega = Vts.Common.DoubleRange(input.Omega(1), input.Omega(end), length(input.Omega));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfRhoAndTime'
                    inputNET = Vts.MonteCarlo.Detectors.ROfRhoAndTimeDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Time = Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfFx'
                    inputNET = Vts.MonteCarlo.Detectors.ROfFxDetectorInput;
                    inputNET.Fx = Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfFxAndTime'
                    inputNET = Vts.MonteCarlo.Detectors.ROfFxAndTimeDetectorInput;
                    inputNET.Fx = Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx));
                    inputNET.Time = Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
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
                    inputNET = Vts.MonteCarlo.Detectors.pMCROfRhoDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
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
                    inputNET = Vts.MonteCarlo.Detectors.pMCROfRhoAndTimeDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Time = Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                    
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
                    inputNET = Vts.MonteCarlo.Detectors.pMCROfFxDetectorInput;
                    inputNET.Fx = Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
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
                    inputNET = Vts.MonteCarlo.Detectors.pMCROfFxAndTimeDetectorInput;
                    inputNET.Fx = Vts.Common.DoubleRange(input.Fx(1), input.Fx(end), length(input.Fx));
                    inputNET.Time = Vts.Common.DoubleRange(input.Time(1), input.Time(end), length(input.Time));
                    inputNET.PerturbedOps = perturbedOpsNET;
                    inputNET.PerturbedRegionsIndices = perturbedRegionsIndicesNET;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'ROfXAndY'
                    inputNET = Vts.MonteCarlo.Detectors.ROfXAndYDetectorInput;
                    inputNET.X = Vts.Common.DoubleRange(input.X(1), input.X(end), length(input.X));
                    inputNET.Y = Vts.Common.DoubleRange(input.Y(1), input.Y(end), length(input.Y));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'RSpecular'
                    inputNET = Vts.MonteCarlo.Detectors.RSpecularDetectorInput;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'TDiffuse'
                    inputNET = Vts.MonteCarlo.Detectors.TDiffuseDetectorInput;
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'TOfAngle'
                    inputNET = Vts.MonteCarlo.Detectors.TOfAngleDetectorInput;
                    inputNET.Angle = Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'TOfRho'
                    inputNET = Vts.MonteCarlo.Detectors.TOfRhoDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                case 'TOfRhoAndAngle'
                    inputNET = Vts.MonteCarlo.Detectors.TOfRhoAndAngleDetectorInput;
                    inputNET.Rho = Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho));
                    inputNET.Angle = Vts.Common.DoubleRange(input.Angle(1), input.Angle(end), length(input.Angle));
                    inputNET.Name = input.Name;
                    inputNET.TallySecondMoment = input.TallySecondMoment;
                    inputNET.TallyDetails = TallyDetails.ToDetailsNET(input.TallyDetails);
                otherwise
                    disp('Unknown DetectorInput type');
            end
            
        end
    end
end