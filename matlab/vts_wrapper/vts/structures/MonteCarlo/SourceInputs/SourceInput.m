classdef SourceInput
    %SOURCEINPUTS Source inputs
    %   
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
            input.PolarAngleEmission = polarAngle;
            input.AzimuthalAngleEmmision = azimuthalAngle;
            input.PointLocation = pointLocation;    
            input.Direction = direction;  
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
                    input.PolarAngleEmission = linspace(inputNET.PolarAngleEmissionRange.Start, inputNET.PolarAngleEmissionRange.Stop, inputNET.PolarAngleEmissionRange.Count);
                    input.AzimuthalAngleEmmision = linspace(inputNET.AzimuthalAngleEmmisionRange.Start, inputNET.AzimuthalAngleEmmisionRange.Stop, inputNET.AzimuthalAngleEmmisionRange.Count);
                    input.PointLocation = [inputNET.PointLocation.X inputNET.PointLocation.Y inputNET.PointLocation.Z];
                    input.Direction = [inputNET.Direction.Ux inputNET.Direction.Uy inputNET.Direction.Uz]; 
                otherwise
                    disp('Unknown SourceInput type');
            end
        end
      
      function inputNET = ToInputNET(input)
          switch input.SourceType           
              case 'DirectionalPoint'
                  inputNET = Vts.MonteCarlo.DirectionalPointSourceInput( ...
                      Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
                      Vts.Common.Direction(input.Direction(1), input.Direction(2), input.Direction(3)), ...
                      input.InitialTissueRegionIndex ...
                      );
              case 'CustomPoint'
                  inputNET = Vts.MonteCarlo.CustomPointSourceInput( ...
                      Vts.Common.DoubleRange(input.PolarAngleEmission(1), input.PolarAngleEmission(end), length(input.PolarAngleEmission)), ...
                      Vts.Common.DoubleRange(input.AzimuthalAngleEmmision(1), input.AzimuthalAngleEmmision(end), length(input.AzimuthalAngleEmmision)), ...
                      Vts.Common.Position(input.PointLocation(1), input.PointLocation(2), input.PointLocation(3)), ...
                      Vts.Common.Direction(input.Direction(1), input.Direction(2), input.Direction(3)), ...
                      input.InitialTissueRegionIndex ...
                      );
                otherwise
                    disp('Unknown SourceInput type');
          end
      end
    end
    
end

