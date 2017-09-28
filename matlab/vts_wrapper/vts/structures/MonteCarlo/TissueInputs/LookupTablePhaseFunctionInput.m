% LOOKUPTABLEPHASEFUNCTIONINPUT used in MultiLayerTissueInput.m
classdef LookupTablePhaseFunctionInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties        
        % Lookup table phase function type 
        PhaseFunctionType = 'LookupTable';
        RegionPhaseFunctionData = Vts.MonteCarlo.LookupTablePhaseFunctionData.PolarLookupTablePhaseFunctionData();
  end
  
  methods (Static)
      function input = FromInputNET(inputNET)
          input.PhaseFunctionType = char(inputNET.PhaseFunctionType);
      end
      
      function inputNET = ToInputNET(input)   
          PhaseFunctionType = input.PhaseFunctionType;
          RegionPhaseFunctionData = input.RegionPhaseFunctionData;
          if (RegionPhaseFunctionData.LookupTablePhaseFunctionDataType == 'Polar')
            inputNET = Vts.MonteCarlo.LookupTablePhaseFunctionInput(...
                Vts.MonteCarlo.LookupTablePhaseFunctionData.PolarLookupTablePhaseFunctionData(...
                    RegionPhaseFunctionData.LutAngles, RegionPhaseFunctionData.LutPdf));
          %elseif (RegionPhaseFunctionData.LookupTablePhaseFunctionDataType == 'PolarAndAzimuthal') % not sure this C# code is working
          end
      end
  end
end