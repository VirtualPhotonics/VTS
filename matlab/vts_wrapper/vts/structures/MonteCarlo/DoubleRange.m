classdef DoubleRange
    methods (Static)
        function range = Default()        
            range.Start = 0;            
            range.Stop = 1;            
            range.Count = 2;        
            range.Delta = 1;
        end
        function range = FromRangeNET(rangeNET)            
            range = linspace(rangeNET.Start, rangeNET.Stop, rangeNET.Count);
        end        
        function rangeNET = ToRangeNET(rangeArray)
            rangeNET = Vts.MonteCarlo.DoubleRange(rangeArray(1), rangeArray(end), length(rangeArray));
        end
    end
end