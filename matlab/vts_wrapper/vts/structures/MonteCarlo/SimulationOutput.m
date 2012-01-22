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
                % need variability with types here (DetectorOutput static
                % methods)
                detectorNames{i} = char(detectorNamesNET(i));
                nValues = valuesNET(i).Mean.Length;
                detectorOutput.Mean = zeros([nValues 1]);
                if valuesNET(1).Mean(1).GetType().ToString() == 'System.Numerics.Complex'
                    for j=1:nValues
                        re = valuesNET(i).Mean(j).Real;
                        im = valuesNET(i).Mean(j).Imaginary;
                        detectorOutput.Mean(j) = sqrt(re.^2 + im.^2);
                    end
                else
                    for j=1:nValues
                        detectorOutput.Mean(j) = valuesNET(i).Mean(j);
                    end
                end
%                 detectorOutput.Mean = NET.convertArray(valuesNET(i).Mean, 'System.Double');
                if(outputNET.Input.Options.TallySecondMoment && ~isempty(valuesNET(1).SecondMoment))
                    detectorOutput.SecondMoment = zeros([nValues 1]);
                    for j=1:nValues
                        detectorOutput.SecondMoment(j) = valuesNET(i).SecondMoment(j);
                    end
%                     detectorOutput.SecondMoment = NET.convertArray(valuesNET(i).SecondMoment, 'System.Double');
                end                   
%                 switch outputNET.TallyType %%%%%
%                     otherwise 
                  try
                        rho_endpoints = input.DetectorInputs{i}.Rho; 
                        detectorOutput.Rho = (rho_endpoints(1:end-1) + rho_endpoints(2:end))/2;
                  catch, end
                  
                  try
                        detectorOutput.Fx = input.DetectorInputs{i}.Fx;
                  catch, end
                  
                  try
                        detectorOutput.Time = input.DetectorInputs{i}.Time;
                  catch, end
                  
                  detectorOutputs{i} = detectorOutput;
%                 end
            end
            
            output.Input = input;
            output.DetectorNames = detectorNames;
            if ~isempty(detectorNames)
                output.Detectors = containers.Map(output.DetectorNames, detectorOutputs);
            end
        end
    end
end