classdef VtsMonteCarlo < handle
    % static properties
    properties (Constant, GetAccess='private') % can be used like a static constructor for static properties
        Assemblies = loadAssemblies();
    end
    
    % instance methods
    methods (Static)
        % static method to run simulation
        function output = RunSimulation(simulationInput, writeDetectors)
            if nargin < 2
                writeDetectors = false;
            end
            
            inputNET = SimulationInput.ToInputNET(simulationInput);
            sim = Vts.MonteCarlo.MonteCarloSimulation(inputNET);
            outputPath = '';
            folderPath = VtsMonteCarlo.MakeNecessaryFolders(inputNET.OutputName, outputPath);
            sim.SetOutputPathForDatabases(outputPath);
            
            % write the original input
            if writeDetectors || ~isempty(simulationInput.DetectorInputs) || length(simulationInput.Options.Databases) > 0
                inputNET.ToFile( ...
                    System.IO.Path.Combine( ...
                    inputNET.OutputName, ...
                    System.String.Concat(inputNET.OutputName, '.xml')));            
            end
            
            try
                disp('Running simulation...');
                tstart = tic;
                outputNET = sim.Run();
                ellapsed = toc(tstart);
                disp(['Simulation complete! Run time: ' num2str(ellapsed) ' seconds']);
                if writeDetectors                    
                    % create a .NET array of detector results (tallies)
                    detectorsNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                        'ToArray', {'Vts.MonteCarlo.IDetector'}, outputNET.ResultsDictionary.Values);
                    for i=1:detectorsNET.Length
                        Vts.IO.DetectorIO.WriteDetectorToFile(detectorsNET(i), folderPath)
                    end
                end
                output = SimulationOutput.FromOutputNET(outputNET);
            catch oops
                disp(oops);
                for j=1:length(oops.stack)
                    disp(oops.stack(j));
                end;
            end
        end
        
        function outputs = RunSimulations(simulationInputs, writeDetectors)
            if nargin < 2
                writeDetectors = false;
            end
            simulations = NET.createArray('Vts.MonteCarlo.MonteCarloSimulation', length(simulationInputs));
            for si = 1:length(simulationInputs)
                simulationInput = simulationInputs{si};
                
                inputNET = SimulationInput.ToInputNET(simulationInput);
                sim = Vts.MonteCarlo.MonteCarloSimulation(inputNET);
                outputPath = '';
                folderPath = VtsMonteCarlo.MakeNecessaryFolders(inputNET.OutputName, outputPath);
                sim.SetOutputPathForDatabases(outputPath);

                % write the original input
                if writeDetectors || ~isempty(simulationInput.DetectorInputs) || length(simulationInput.Options.Databases) > 0
                    inputNET.ToFile( ...
                        System.IO.Path.Combine( ...
                        inputNET.OutputName, ...
                        System.String.Concat(inputNET.OutputName, '.xml')));            
                end
                simulations(si) = sim;
            end

            try
                disp('Running simulations...');
                tstart = tic;
                outputsNET = Vts.MonteCarlo.MonteCarloSimulation.RunAll(simulations);
                ellapsed = toc(tstart);
                disp(['Simulations complete! Run time: ' num2str(ellapsed) ' seconds']);
                for si = 1:length(simulationInputs)
                    if writeDetectors                    
                        % create a .NET array of detector results (tallies)
                        detectorsNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                            'ToArray', {'Vts.MonteCarlo.IDetector'}, outputsNET(si).ResultsDictionary.Values);
                        for i=1:detectorsNET.Length
                            Vts.IO.DetectorIO.WriteDetectorToFile(detectorsNET(i), folderPath)
                        end
                    end
                    outputs{si} = SimulationOutput.FromOutputNET(outputsNET(si));
                end
            catch oops
                disp(oops);
                for j=1:length(oops.stack)
                    disp(oops.stack(j));
                end;
            end
        end
        
        % static method to run simulation
        function output = RunPostProcessor(postProcessorInput, originalSimInput)
            ppInputNET = PostProcessorInput.ToInputNET(postProcessorInput);
            
            if nargin < 2                
                originalSimInputNET = Vts.MonteCarlo.SimulationInput.FromFile( ...
                    System.IO.Path.Combine( ...
                        ppInputNET.InputFolder, ...
                        System.String.Concat(ppInputNET.DatabaseSimulationInputFilename, '.xml')));
            else
                originalSimInputNET = SimulationInput.ToInputNET(originalSimInput);
            end            
            
            % hard-coding pMC to get started, than need to open this up
            vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'pMCDiffuseReflectance');
            
            ppDatabase = Vts.MonteCarlo.Factories.PhotonDatabaseFactory.GetpMCDatabase(... % database filenames are assumed to be convention
                vbTypeNET, ...
                ppInputNET.InputFolder);
            
            
            %             sim = Vts.MonteCarlo.MonteCarloSimulation(ppInputNET);
            disp('Running post-processor...');
            tstart = tic;
            
            outputNET = Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor.GenerateOutput( ...
                vbTypeNET, ...
                ppInputNET.DetectorInputs, ... % not switching based on input type here (yet...)
                ppInputNET.TallySecondMoment, ...
                ppInputNET.TrackStatistics, ...
                ppDatabase, ...
                originalSimInputNET ...
                );
            
            ellapsed = toc(tstart);
            disp(['Post-processing complete! Run time: ' num2str(ellapsed) ' seconds']);
            
            output = SimulationOutput.FromOutputNET(outputNET);
            output.PostProcessorInput = postProcessorInput;
        end
    end
    
    methods (Static, Access='private')
        function resultsFolder = MakeNecessaryFolders(outputName, outputFolderPath)
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