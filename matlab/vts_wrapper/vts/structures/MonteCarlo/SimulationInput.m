% SIMULATIONINPUT Defines input to the Monte Carlo simulation
% This includes the output file name, number of photons to execute (N), 
% source, tissue and detector definitions.
classdef SimulationInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
    properties
        N = 100;
        OutputName = 'results';
        Options = SimulationOptions();
        SourceInput = SourceInput.DirectionalPoint([0 0 0], [0 0 1], 0);
        %SourceInput = DirectionalPointSourceInput();
        %TissueInput = TissueInput.MultiLayer();
        TissueInput = MultiLayerTissueInput();
        DetectorInputs = {...
            DetectorInput.ROfRho(linspace(0,40,201))...
            };
    end
    
    methods (Static)
        function input = FromInputNET(inputNET)
            input = SimulationInput();
            input.N = inputNET.N;
            input.OutputName = char(inputNET.OutputName);
            input.Options = SimulationOptions.FromOptionsNET(inputNET.Options);
            input.SourceInput = SourceInput.FromInputNET(inputNET.SourceInput);
            input.TissueInput = MultiLayerTissueInput.FromInputNET(inputNET.TissueInput);
            detectorInputsNET = inputNET.DetectorInputs; % not sure why I can't inline this, but whatevs
            for i=1:inputNET.DetectorInputs.Length
                input.DetectorInputs{i} = DetectorInput.FromInputNET(detectorInputsNET(i));
            end
        end
        
        function inputNET = ToInputNET(input)
            optionsNET = SimulationOptions.ToOptionsNET(input.Options);
            detectorInputsNET = NET.createArray('Vts.MonteCarlo.IDetectorInput', length(input.DetectorInputs));
            for i=1:length(input.DetectorInputs)
                detectorInputsNET(i) = DetectorInput.ToInputNET(input.DetectorInputs{i});
            end
            
            inputNET = Vts.MonteCarlo.SimulationInput( ...
                input.N, ...
                input.OutputName, ...
                optionsNET, ...
                SourceInput.ToInputNET(input.SourceInput), ...
                MultiLayerTissueInput.ToInputNET(input.TissueInput), ...
                detectorInputsNET ...
                );
%             inputNET = Vts.MonteCarlo.SimulationInput( ...
%                 input.N, ...
%                 input.OutputName, ...
%                 optionsNET, ...
%                 SourceInput.ToInputNET(input.SourceInput), ...
%                 TissueInput.ToInputNET(input.TissueInput), ...
%                 detectorInputsNET ...
%                 );
        end
        
        function ToFile(input, path)
            inputNET =  SimulationInput.ToInputNET(input);
            inputNET.ToFile(path);
        end
        
        function input = FromFile(path)
            inputNET = Vts.MonteCarlo.SimulationInput.FromFile(path);
            input =  SimulationInput.FromInputNET(inputNET);
        end
    end
end