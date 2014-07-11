classdef DoubleRange
    methods (Static)
        function range = Default()      
            range = linspace(0, 1, 2);
        end
        function range = FromRangeNET(rangeNET)            
            range = linspace(rangeNET.Start, rangeNET.Stop, rangeNET.Count);
        end        
        function rangeNET = ToRangeNET(rangeArray)
            rangeNET = Vts.MonteCarlo.DoubleRange(rangeArray(1), rangeArray(end), length(rangeArray));
        end
    end
end