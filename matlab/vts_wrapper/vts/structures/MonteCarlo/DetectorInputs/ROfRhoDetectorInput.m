classdef ROfRhoDetectorInput < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
    properties
        % detector tally identifier
        TallyType = 'ROfRho';
        % detector name, defaults to 'ROfRho' but can be user specified
        Name = 'ROfRho';
        % detector rho binning
        Rho = linspace(0, 10, 101);
    end
    
    methods
        % "constructor"
        function obj = ROfRhoDetectorInput(rho, name)
            if nargin == 2
                obj.Rho = rho;
                obj.Name = name;
            elseif nargin == 1
                obj.Rho = rho;
            end
        end
    end
    
    methods (Static)
        function input = FromInputNET(inputNET)
            input = ROfRhoDetectorInput();
            input.TallyType = char(inputNET.TallyType);
            input.Name = char(inputNET.Name);
            input.Rho = linspace(inputNET.Rho.Start, inputNET.Rho.Stop, inputNET.Rho.Count);
        end
        
        function inputNET = ToInputNET(input)
            inputNET = Vts.MonteCarlo.ROfRhoDetectorInput( ...
                Vts.Common.DoubleRange(input.Rho(1), input.Rho(end), length(input.Rho)), ...
                input.Name ...
                );
        end
    end
end