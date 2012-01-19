classdef SimulationOutput    
    methods (Static)
        function output = FromOutputNET(outputNET)
            % create a .NET array of detector names
            detectorNamesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                'ToArray', {'System.String'}, outputNET.ResultsDictionary.Keys);
            % create a .NET array of detector results (tallies)
            valuesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                'ToArray', {'Vts.MonteCarlo.IDetector'}, outputNET.ResultsDictionary.Values);
                        
            input = SimulationInput.FromInputNET(outputNET.Input);
            
            detectorNames = cell(1, detectorNamesNET.Length); 
            detectorOutputs = cell(1, detectorNamesNET.Length);             
            for i=1:detectorNamesNET.Length
                detectorNames{i} = char(detectorNamesNET(i));
                nValues = valuesNET(i).Mean.Length;
                detectorOutput.Mean = double([nValues 1]);
                for j=1:nValues
                    detectorOutput.Mean(j) = valuesNET(i).Mean(j);
                end
%                 detectorOutput.Mean = NET.convertArray(valuesNET(i).Mean, 'System.Double');
                if(outputNET.Input.Options.TallySecondMoment)
                    detectorOutput.SecondMoment = double([nValues 1]);
                    for j=1:nValues
                        detectorOutput.SecondMoment(j) = valuesNET(i).SecondMoment(j);
                    end
%                     detectorOutput.SecondMoment = NET.convertArray(valuesNET(i).SecondMoment, 'System.Double');
                end    
                
                rho_endpoints = input.DetectorInputs{i}.Rho; 
                detectorOutput.Rho = (rho_endpoints(1:end-1) + rho_endpoints(2:end))/2;
                
                detectorOutputs{i} = detectorOutput;
            end
            
            output.Input = input;
            output.DetectorNames = detectorNames;
            if ~isempty(detectorNames)
                output.Detectors = containers.Map(output.DetectorNames, detectorOutputs);
            end
        end
    end
end