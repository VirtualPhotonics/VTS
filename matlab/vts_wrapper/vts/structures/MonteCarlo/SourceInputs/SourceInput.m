%SOURCEINPUT Defines input data for Source implementation
%   
classdef SourceInput
    methods (Static)
        function input = Default()
            % Point source type 
            input.SourceType = 'DirectionalPoint';
            % New position 
            input.PointLocation = [0 0 0];    
            % Point source emitting direction
            input.Direction = [0 0 1];  
            % Initial tissue region index        
            input.InitialTissueRegionIndex = 0;
        end

        function input = DirectionalPoint(location, direction, index)
            input.SourceType = 'DirectionalPoint';
            input.PointLocation = location;    
            input.Direction = direction;  
            input.InitialTissueRegionIndex = index;
        end 

        function input = CustomPoint(polarAngle, azimuthalAngle, pointLocation, direction, index)
            input.SourceType = 'CustomPoint';
            input.PolarAngleEmissionRange = polarAngle;
            input.AzimuthalAngleEmissionRange = azimuthalAngle;
            input.PointLocation = pointLocation;    
            input.Direction = direction;  
            input.InitialTissueRegionIndex = index;
        end 

        function input = IsotropicPoint(location, index)
            input.SourceType = 'IsotropicPoint';
            input.PointLocation = location;    
            input.InitialTissueRegionIndex = index;
        end 

        function input = FromInputNET(inputNET)
            input.SourceType = char(inputNET.SourceType);
            input.InitialTissueRegionIndex = inputNET.InitialTissueRegionIndex;
            switch input.SourceType
                case 'DirectionalPoint'
                    input.PointLocation = [inputNET.PointLocation.X inputNET.PointLocation.Y inputNET.PointLocation.Z];
                    input.Direction = [inputNET.Direction.Ux inputNET.Direction.Uy inputNET.Direction.Uz]; 
                case 'CustomPoint'
                    input.PolarAngleEmissionRange = linspace(inputNET.PolarAngleEmissionRange.Start, inputNET.PolarAngleEmissionRange.Stop, inputNET.PolarAngleEmissionRange.Count);
                    input.AzimuthalAngleEmissionRange = linspace(inputNET.AzimuthalAngleEmissionRange.Start, inputNET.AzimuthalAngleEmissionRange.Stop, inputNET.AzimuthalAngleEmissionRange.Count);
                    input.PointLocation = [inputNET.PointLocation.X inputNET.PointLocation.Y inputNET.PointLocation.Z];
                    input.Direction = [inputNET.Direction.Ux inputNET.Direction.Uy inputNET.Direction.Uz]; 
                case 'IsotropicPoint'
                    input.PointLocation = [inputNET.PointLocation.X inputNET.PointLocation.Y inputNET.PointLocation.Z];
                otherwise
                    disp('Unknown SourceInput type');
            end
        end
      
      function inputNET = ToInputNET(input)
          switch input.SourceType           
              case 'DirectionalPoint'
                  inputNET = Vts.MonteCarlo.Sources.DirectionalPointSourceInput( ...
                      Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
                      Vts.Common.Direction(input.Direction(1), input.Direction(2), input.Direction(3)), ...
                      input.InitialTissueRegionIndex ...
                      );
              case 'CustomPoint'
                  inputNET = Vts.MonteCarlo.Sources.CustomPointSourceInput( ...
                      Vts.Common.DoubleRange(input.PolarAngleEmissionRange(1), input.PolarAngleEmissionRange(end), length(input.PolarAngleEmissionRange)), ...
                      Vts.Common.DoubleRange(input.AzimuthalAngleEmissionRange(1), input.AzimuthalAngleEmissionRange(end), length(input.AzimuthalAngleEmissionRange)), ...
                      Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
                      Vts.Common.Direction(input.Direction(1), input.Direction(2), input.Direction(3)), ...
                      input.InitialTissueRegionIndex ...
                      );
              case 'IsotropicPoint'
                  inputNET = Vts.MonteCarlo.Sources.IsotropicPointSourceInput( ...
                      Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
                      input.InitialTissueRegionIndex ...
                      );
                otherwise
                    disp('Unknown SourceInput type');
          end
      end
    end
    
end

