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
            disp('Running simulation...'); 
            tstart = tic; 
            outputNET = sim.Run();
            ellapsed = toc(tstart);
            disp(['Simulation complete! Run time: ' num2str(ellapsed) ' seconds']);
            
            output = SimulationOutput.FromOutputNET(outputNET);
                                   
        end    
    end
%     
%     methods (Static, Access='private')
%         function info = FromDetectorNET(output)
%             
%             detectorNamesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
%                 'ToArray', {'System.String'}, output.ResultsDictionary.Keys);            
%             valuesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
%                 'ToArray', {'Vts.MonteCarlo.IDetector'}, output.ResultsDictionary.Values);
%             
%             detectorNames = cell(1, detectorNamesNET.Length);
%             
%             for i=1:detectorNamesNET.Length
%                 names{i} = char(detectorNamesNET(i));
%                 
%             end
%             
%             % get the underlying Vts.ChromophoreType values
%             info.TypeValues = System.Enum.GetValues(enumType);
%             
%             % create a dictionary based on the chromophore name
%             info.TypeIndexMap = containers.Map(names, num2cell(1:namesNET.Length));
%         end        
%     end
end