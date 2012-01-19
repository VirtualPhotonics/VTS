classdef VtsMonteCarlo < handle
    % static properties
    properties (Constant, GetAccess='private') % can be used like a static constructor for static properties
        Assemblies = loadAssemblies();
    end
    
    % instance methods
    methods (Static)
        % static method to run simulation
        function output = RunSimulation(simulationInput)
            inputNET = SimulationInput.ToInputNET(simulationInput);
            sim = Vts.MonteCarlo.MonteCarloSimulation(inputNET);
            VtsMonteCarlo.MakeNecessaryFolders(inputNET.OutputName,'');
            try
                disp('Running simulation...');
                tstart = tic;
                outputNET = sim.Run();
                ellapsed = toc(tstart);
                disp(['Simulation complete! Run time: ' num2str(ellapsed) ' seconds']);                
                output = SimulationOutput.FromOutputNET(outputNET);
            catch oops
                disp(oops)
            end
        end
        
        % static method to run simulation
        function output = RunPostProcessor(postProcessorInput)
            ppInputNET = PostProcessorInput.ToInputNET(postProcessorInput);
            
            % hard-coding pMC to get started, than need to open this up
            vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'pMCDiffuseReflectance');
            
            ppDatabase = PhotonDatabaseFactory.GetpMCDatabase(... % database filenames are assumed to be convention
                vbTypeNET, ...
                ppInputNET.InputFolder);
            
            simInputNET = Vts.MonteCarlo.SimulationInput.FromFile( ...
                System.IO.Path.Combine( ...
                ppInputNET.InputFolder, ...
                System.String.Concat(ppInputNET.DatabaseSimulationInputFilename, '.xml')));
            
            %             sim = Vts.MonteCarlo.MonteCarloSimulation(ppInputNET);
            disp('Running post-processor...');
            tstart = tic;
            
            PhotonDatabasePostProcessor.GenerateOutput( ...
                vbTypeNET, ...
                ppInputNET.DetectorInputs, ... % not switching based on input type here (yet...)
                ppInputNET.TallySecondMoment, ...
                ppDatabase, ...
                simInputNET ...
                );
            
            ellapsed = toc(tstart);
            
            disp(['Post-processing complete! Run time: ' num2str(ellapsed) ' seconds']);
            
            output = SimulationOutput.FromOutputNET(outputNET);
        end
        
        function MakeNecessaryFolders(outputName, outputFolderPath)
            % locate root folder for output, creating it if necessary
            if System.String.IsNullOrEmpty(outputFolderPath)
                path =  System.IO.Path.GetFullPath(System.IO.Directory.GetCurrentDirectory());
            else
                path =  System.IO.Path.GetFullPath(outputFolderPath);
            end
            if ~System.IO.Directory.Exists(path)
                System.IO.Directory.CreateDirectory(path);
            end
            
            % locate destination folder for output, creating it if necessary
            resultsFolder = System.IO.Path.Combine(path, outputName);
            if ~System.IO.Directory.Exists(resultsFolder)
                System.IO.Directory.CreateDirectory(resultsFolder);
            end
        end
    end
end