% SIMULATIONOUTPUT Output from a Monte Carlo simulation
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
            for n=1:detectorNamesNET.Length
                % need variability with types here (DetectorOutput static
                % methods)
                localValuesNET=valuesNET(n);
                meanValuesNET=localValuesNET.Mean;
                secondMomentValuesNET=localValuesNET.SecondMoment;
                detectorNames{n} = char(detectorNamesNET(n));
                type = char(valuesNET(n).Mean.GetType().ToString());
                switch type
                    case {'MathNet.Numerics.Complex[]'}
                        nValues = valuesNET(n).Mean.Length;               
                        detectorOutput.Mean = zeros([nValues 1]);
                        
                        for j=1:nValues
                            re = meanValuesNET(j).Real;
                            im = meanValuesNET(j).Imaginary;
                            %detectorOutput.Mean(j) = sqrt(re.^2 + im.^2);
                            % return complex values not amp CKH 3/26/13
                            detectorOutput.Mean(j) = re + 1i * im;
                        end
                    case {'MathNet.Numerics.Complex[,]'}
                        dim1 = valuesNET(n).Mean.GetLength(0);
                        dim2 = valuesNET(n).Mean.GetLength(1);
                        detectorOutput.Mean = zeros([dim1 dim2]);
                        for j=1:dim1
                            for k=1:dim2
                                re = meanValuesNET(j,k).Real;
                                im = meanValuesNET(j,k).Imaginary;
                                %detectorOutput.Mean(j,k) = sqrt(re.^2 + im.^2);
                                % return complex values not amp CKH 3/26/13
                                detectorOutput.Mean(j,k) = re + 1i * im;
                            end
                        end
                    case {'System.Double[,]'}
                        dim1 = valuesNET(n).Mean.GetLength(0);
                        dim2 = valuesNET(n).Mean.GetLength(1);
                        detectorOutput.Mean = zeros([dim1 dim2]);
                        for j=1:dim1
                            for k=1:dim2
                                detectorOutput.Mean(j,k) = meanValuesNET(j,k);
                            end
                        end
                    case {'System.Double[,,]'}
                        dim1 = valuesNET(n).Mean.GetLength(0);
                        dim2 = valuesNET(n).Mean.GetLength(1);
                        dim3 = valuesNET(n).Mean.GetLength(2);
                        detectorOutput.Mean = zeros([dim1 dim2 dim3]);
                        for j=1:dim1
                            for k=1:dim2
                                for l=1:dim3
                                    detectorOutput.Mean(j,k,l) = meanValuesNET(j,k,l);
                                end
                            end
                        end
                    otherwise
                        nValues = valuesNET(n).Mean.Length;               
                        detectorOutput.Mean = zeros([nValues 1]);
                        for j=1:nValues
                            detectorOutput.Mean(j) = meanValuesNET(j);
                        end
                end
%                 detectorOutput.Mean = NET.convertArray(valuesNET(i).Mean, 'System.Double');
                if(outputNET.Input.Options.TallySecondMoment && ~isempty(valuesNET(1).SecondMoment))
%                     detectorOutput.SecondMoment = zeros([nValues 1]);
%                     for j=1:nValues
%                         detectorOutput.SecondMoment(j) = valuesNET(i).SecondMoment(j);
%                     end
                    type = char(valuesNET(n).SecondMoment.GetType().ToString());
                    switch type
                        case {'MathNet.Numerics.Complex[]'}
                            nValues = valuesNET(n).SecondMoment.Length;               
                             detectorOutput.SecondMoment = zeros([nValues 1]);
                             for j=1:nValues
                                 re = valuesNET(n).SecondMoment(j).Real;
                                 im = valuesNET(n).SecondMoment(j).Imaginary;
                                 detectorOutput.SecondMoment(j) = re + 1i * im;
                             end
                        case {'MathNet.Numerics.Complex[,]'}
                            dim1 = valuesNET(n).SecondMoment.GetLength(0);
                            dim2 = valuesNET(n).SecondMoment.GetLength(1);
                            detectorOutput.SecondMoment = zeros([dim1 dim2]);
                            for j=1:dim1
                                for k=1:dim2
                                    detectorOutput.SecondMoment(j,k) = re + 1i * im;
                                end
                            end
                        case {'System.Double[,]'}
                            dim1 = valuesNET(n).SecondMoment.GetLength(0);
                            dim2 = valuesNET(n).SecondMoment.GetLength(1);
                            detectorOutput.SecondMoment = zeros([dim1 dim2]);
                            for j=1:dim1
                                for k=1:dim2
                                    detectorOutput.SecondMoment(j,k) = secondMomentValuesNET(j,k);
                                end
                            end
                        case {'System.Double[,,]'}
                            dim1 = valuesNET(n).SecondMoment.GetLength(0);
                            dim2 = valuesNET(n).SecondMoment.GetLength(1);
                            dim3 = valuesNET(n).SecondMoment.GetLength(2);
                            detectorOutput.SecondMoment = zeros([dim1 dim2 dim3]);
                            for j=1:dim1
                                for k=1:dim2
                                    for l=1:dim3
                                        detectorOutput.SecondMoment(j,k,l) = secondMomentValuesNET(j,k,l);
                                    end
                                end
                            end
                        otherwise
                            nValues = valuesNET(n).SecondMoment.Length;               
                            detectorOutput.SecondMoment = zeros([nValues 1]);
                            for j=1:nValues
                                detectorOutput.SecondMoment(j) = secondMomentValuesNET(j);
                            end
                    end
%                     detectorOutput.SecondMoment = NET.convertArray(valuesNET(i).SecondMoment, 'System.Double');
                end                   
%                 switch outputNET.TallyType %%%%%
%                     otherwise 
                  try
                        rho_endpoints = input.DetectorInputs{n}.Rho; 
                        detectorOutput.Rho = (rho_endpoints(1:end-1) + rho_endpoints(2:end))/2;
                  catch, end
                                    
                  try
                        time_endpoints = input.DetectorInputs{n}.Time;
                        detectorOutput.Time = (time_endpoints(1:end-1) + time_endpoints(2:end))/2;
                  catch, end
                  
                  % Fx and Omega results are determined at specified Fx , Omega values not binned
                  % so no midpoint calculation needed I think (CKH)
                  try
                        detectorOutput.Fx = input.DetectorInputs{n}.Fx;
                  catch, end
                  
                  try 
                        detectorOutput.Omega = input.DetectorInputs{n}.Omega;
                  catch, end
                  
                  detectorOutputs{n} = detectorOutput;
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