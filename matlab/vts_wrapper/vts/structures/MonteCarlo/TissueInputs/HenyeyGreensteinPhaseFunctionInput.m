% HENYEYGREENSTEINPHASEFUNCTIONINPUT used in MultiLayerTissueInput.m
classdef HenyeyGreensteinPhaseFunctionInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties        
        % HenyeyGreenstein phase function type 
        PhaseFunctionType = 'HenyeyGreenstein';
  end
  
  methods (Static)
      function input = FromInputNET(inputNET)
          input.PhaseFunctionType = char(inputNET.PhaseFunctionType);
      end
      
      function inputNET = ToInputNET(input)                    
          inputNET = Vts.MonteCarlo.HenyeyGreensteinPhaseFunctionInput();
      end
  end
end