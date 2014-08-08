% Defines the post processor input for a Monte Carlo simulation
classdef PostProcessorInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties
    DetectorInputs = {...
        DetectorInput.ROfRho(linspace(0,40,201))...
    };
      TallySecondMoment = false;
      InputFolder = 'results';
      DatabaseSimulationInputFilename = 'infile';
      OutputName = 'ppresults';
  end
  
  methods (Static)
      function input = FromInputNET(inputNET)
          input = PostProcessorInput();
                    
          detectorInputsNET = inputNET.DetectorInputs;
          for i=1:inputNET.DetectorInputs.Length
              input.DetectorInputs{i} = DetectorInput.FromInputNET(detectorInputsNET(i));
          end                    
          input.TallySecondMoment = logical(inputNET.TallySecondMoment);
          input.InputFolder = char(inputNET.InputFolder);
          input.DatabaseSimulationInputFilename = char(inputNET.DatabaseSimulationInputFilename);
          input.OutputName = char(inputNET.OutputName);
      end
      
      function inputNET = ToInputNET(input)
          detectorInputsNET = NET.createArray('Vts.MonteCarlo.IDetectorInput', length(input.DetectorInputs));  
          for i=1:length(input.DetectorInputs)
              detectorInputsNET(i) = DetectorInput.ToInputNET(input.DetectorInputs{i});
          end
          
          inputNET = Vts.MonteCarlo.PostProcessorInput( ...
              detectorInputsNET, ...
              logical(input.TallySecondMoment), ...
              input.InputFolder, ...
              input.DatabaseSimulationInputFilename, ...
              input.OutputName...
          );
      end
  end
end