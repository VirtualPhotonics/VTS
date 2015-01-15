% DIRECTIONALPOINTSOURCEINPUT Replaced by SourceInput
classdef DirectionalPointSourceInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties        
        % Point source type 
        SourceType = 'DirectionalPoint';
        % New position 
        PointLocation = [0 0 0];    
        % Point source emitting direction
        Direction = [0 0 1];  
        % Initial tissue region index        
        InitialTissueRegionIndex = 0;
  end
  
  methods (Static)
      function input = FromInputNET(inputNET)
          input = DirectionalPointSourceInput;
          input.SourceType = char(inputNET.SourceType);
          input.PointLocation = [inputNET.PointLocation.X inputNET.PointLocation.Y inputNET.PointLocation.Z];
          input.Direction = [inputNET.Direction.Ux inputNET.Direction.Uy inputNET.Direction.Uz]; 
          input.InitialTissueRegionIndex = inputNET.InitialTissueRegionIndex;
      end
      
      function inputNET = ToInputNET(input)                    
          inputNET = Vts.MonteCarlo.Sources.DirectionalPointSourceInput( ...
              Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
              Vts.Common.Direction(input.Direction(1), input.Direction(2), input.Direction(3)), ...
              input.InitialTissueRegionIndex ...
          );
      end
  end
end