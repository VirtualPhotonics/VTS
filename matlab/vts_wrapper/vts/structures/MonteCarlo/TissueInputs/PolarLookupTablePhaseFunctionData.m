% POLARLOOKUPTABLEPHASEFUNCTIONDATA used in MultiLayerTissueInput
classdef PolarLookupTablePhaseFunctionData < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties        
        % Lookup table phase function data type 
        LookupTablePhaseFunctionDataType = 'Polar';
        LutAngles = [0, pi/6, pi/3, pi/2, 2*pi/3, pi*5/6, pi]; % theta angles
        LutPdf = [0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5]; % p(theta) pdf
  end
  
  methods (Static)
        function input = PolarLookupTablePhaseFunctionData(lutAngles, lutPdf)
            input.LutAngles = lutAngles;
            input.LutPdf = lutPdf;
        end
      
      function input = FromInputNET(inputNET)
          input.LookupTablePhaseFunctionDataType = char(inputNET.PhaseFunctionType);
      end
      
      function inputNET = ToInputNET(input)  
          LutAngles = input.LutAngles;
          LutPdf = input.LutPdf;
          inputNET = Vts.MonteCarlo.LookupTablePhaseFunctionData.PolarLookupTablePhaseFunctionData(LutAngles, LutPdf);
      end
  end
end