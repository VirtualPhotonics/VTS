classdef SimulationOutput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
    properties
        Input = SimulationInput();
        DetectorNames;
        Detectors;
    end
    
%     methods (Static, Access='private')
%         function map = GetDetectorMap(output)
%             map = containers.Map(output.DetectorNames, output.DetectorOutputs);
%         end
%     end
%     
    methods (Static)
        function output = FromOutputNET(outputNET)
            % create a .NET array of detector names
            detectorNamesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                'ToArray', {'System.String'}, outputNET.ResultsDictionary.Keys);
            % create a .NET array of detector results (tallies)
            valuesNET = NET.invokeGenericMethod('System.Linq.Enumerable', ...
                'ToArray', {'Vts.MonteCarlo.IDetector'}, outputNET.ResultsDictionary.Values);
            
            detectorNames = cell(1, detectorNamesNET.Length);            
            for i=1:detectorNamesNET.Length
                detectorNames{i} = char(detectorNamesNET(i));
                detectorOutput = DetectorOutput();
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
                detectorOutputs{i} = detectorOutput;
            end
                        
            output = SimulationOutput();
            output.Input = SimulationInput.FromInputNET(outputNET.Input);
            output.DetectorNames = detectorNames;
            output.Detectors = containers.Map(output.DetectorNames, detectorOutputs);
        end
    end
end