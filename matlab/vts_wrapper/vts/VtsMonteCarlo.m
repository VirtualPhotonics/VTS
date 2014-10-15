classdef VtsMonteCarlo < handle
    %% VtsMonteCarlo Class containing static methods for running Monte Carlo simulations
    % single or multiple simulations or post processing.
    % For more information about the class see VtsMonteCarlo

    % static properties
    properties (Constant, GetAccess='private') % can be used like a static constructor for static properties
        Assemblies = loadAssemblies();
    end
    
    % instance methods
    methods (Static)
        % static method to run simulation
        function output = RunSimulation(simulationInput, writeDetectors)
            %% RunSimulation Runs a single Monte Carlo simulation
            %
            %   RunSimulation(SIMULATIONINPUT, WRITEDETECTORS)
            %   SIMULATIONINPUT Simulation input class defining the data for 
            %   the simulation. 
            %
            %   see also SIMULATIONINPUT, WRITEDETECTORS
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
                    System.String.Concat(inputNET.OutputName, '.txt')));            
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
                        Vts.MonteCarlo.IO.DetectorIO.WriteDetectorToFile(detectorsNET(i), folderPath)
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
            %% RunSimulations Runs multiple Monte Carlo simulations
            %   RunSimulations(SIMULATIONINPUTS, WRITEDETECTORS)
            %   SIMULATIONINPUTS List of simulation input classes defining the data for 
            %       the simulation. see also SIMULATIONINPUT, WRITEDETECTORS
            if nargin < 2
                writeDetectors = false;
            end
            simulations = NET.createArray('Vts.MonteCarlo.MonteCarloSimulation', length(simulationInputs));
            for si = 1:length(simulationInputs)
                simulationInput = simulationInputs(si);
                
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
                        System.String.Concat(inputNET.OutputName, '.txt')));            
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
                            Vts.MonteCarlo.IO.DetectorIO.WriteDetectorToFile(detectorsNET(i), folderPath)
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
        
        % static method to run post processor
        function output = RunPostProcessor(postProcessorInput, originalSimInput)
            %% RunPostProcessor Runs the post processor for the Monte Carlo simulation
            %   RunPostProcessor(POSTPROCESSORINPUT, ORIGINALSIMINPUT)
            %   POSTPROCESSORINPUT postprocessor input class defining the data for 
            %       post processing.
            %   ORIGINALSIMINPUT original simulation input class. 
            %       see also POSTPROCESSORINPUT, SIMULATIONINPUT
            ppInputNET = PostProcessorInput.ToInputNET(postProcessorInput);
            
            if nargin < 2                
                originalSimInputNET = Vts.MonteCarlo.SimulationInput.FromFile( ...
                    System.IO.Path.Combine( ...
                        ppInputNET.InputFolder, ...
                        System.String.Concat(ppInputNET.DatabaseSimulationInputFilename, '.txt')));
            else
                originalSimInputNET = SimulationInput.ToInputNET(originalSimInput);
            end            

            % the following assumes only one database specified for now
            switch char(originalSimInput.Options.Databases{1})
                case 'DiffuseReflectance'
                vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'DiffuseReflectance');
                % the following assumes database filnames are convention
                ppDatabase = Vts.MonteCarlo.Factories.PhotonDatabaseFactory.GetPhotonDatabase(...
                    vbTypeNET, ...
                    ppInputNET.InputFolder);
                case 'pMCDiffuseReflectance'
                vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'pMCDiffuseReflectance');
                ppDatabase = Vts.MonteCarlo.Factories.PhotonDatabaseFactory.GetpMCDatabase(...
                    vbTypeNET, ...
                    ppInputNET.InputFolder);
            end
            
            %             sim = Vts.MonteCarlo.MonteCarloSimulation(ppInputNET);
            disp('Running post-processor...');
            
            ppNET = Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor( ...
                vbTypeNET, ...
                ppInputNET.DetectorInputs, ...
                ppDatabase, ...
                originalSimInputNET ...
                );
            
            tstart = tic;
            
            outputNET = ppNET.Run();
%             
%             outputNET = Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor.GenerateOutput( ...
%                 vbTypeNET, ...
%                 ppInputNET.DetectorInputs, ... % not switching based on input type here (yet...)
%                 ppInputNET.TallySecondMoment, ...
%                 ppInputNET.TrackStatistics, ...
%                 ppDatabase, ...
%                 originalSimInputNET ...
%                 );
            
            ellapsed = toc(tstart);
            disp(['Post-processing complete! Run time: ' num2str(ellapsed) ' seconds']);
            
            output = SimulationOutput.FromOutputNET(outputNET);
            output.PostProcessorInput = postProcessorInput;
        end
        function outputs = RunPostProcessors(postProcessorInputs, originalSimInputs)
            %% RunPostProcessors Runs multiple post processors for the Monte Carlo simulations
            %   RunPostProcessors(POSTPROCESSORINPUTS, ORIGINALSIMINPUTS)
            %   POSTPROCESSORINPUTS postprocessor input classes defining the data for 
            %       post processing.
            %   ORIGINALSIMINPUTS original simulation input classes. 
            %       see also POSTPROCESSORINPUT, SIMULATIONINPUT
            npp = length(postProcessorInputs);
            postProcessors = NET.createArray('Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor', npp);
            for ppi = 1:npp
                ppInputNET = PostProcessorInput.ToInputNET(postProcessorInputs{ppi});

                if nargin < 2                
                    originalSimInputNET = Vts.MonteCarlo.SimulationInput.FromFile( ...
                        System.IO.Path.Combine( ...
                            ppInputNET.InputFolder, ...
                            System.String.Concat(ppInputNET.DatabaseSimulationInputFilename, '.xml')));
                else
                    originalSimInputNET = SimulationInput.ToInputNET(originalSimInputs{ppi});
                end            

                % the following assumes only one database specified for now
                switch char(originalSimInputs{ppi}.Options.Databases{1}) 
                    case 'DiffuseReflectance'
                    vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'DiffuseReflectance');
                    % the following assumes database filnames are convention
                    ppDatabase = Vts.MonteCarlo.Factories.PhotonDatabaseFactory.GetPhotonDatabase(...
                        vbTypeNET, ...
                        ppInputNET.InputFolder);
                    case 'pMCDiffuseReflectance'
                    vbTypeNET = EnumHelper.GetValueNET('Vts.MonteCarlo.VirtualBoundaryType', 'pMCDiffuseReflectance');
                    ppDatabase = Vts.MonteCarlo.Factories.PhotonDatabaseFactory.GetpMCDatabase(...
                        vbTypeNET, ...
                        ppInputNET.InputFolder);
                end

                %             sim = Vts.MonteCarlo.MonteCarloSimulation(ppInputNET);

                ppNET = Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor( ...
                    vbTypeNET, ...
                    ppInputNET.DetectorInputs, ... % not switching based on input type here (yet...)
                    ppDatabase, ...
                    originalSimInputNET ...
                    );
                
                postProcessors(ppi) = ppNET;
            end
            
            disp('Running post-processor...');
            tstart = tic;
            
            outputsNET = ppNET.RunAll(postProcessors);
%             
%             outputNET = Vts.MonteCarlo.PostProcessing.PhotonDatabasePostProcessor.GenerateOutput( ...
%                 vbTypeNET, ...
%                 ppInputNET.DetectorInputs, ... % not switching based on input type here (yet...)
%                 ppInputNET.TallySecondMoment, ...
%                 ppInputNET.TrackStatistics, ...
%                 ppDatabase, ...
%                 originalSimInputNET ...
%                 );
            
            ellapsed = toc(tstart);
            disp([int2str(npp) ' post-processing simulations complete! Total run time: ' num2str(ellapsed) ' seconds']);
            
            for ppi = 1:npp
                output = SimulationOutput.FromOutputNET(outputsNET(ppi));
                output.PostProcessorInput = postProcessorInputs{ppi};
                outputs{ppi} = output;
            end
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