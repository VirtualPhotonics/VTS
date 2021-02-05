%GetPositionInACircleRandomFlat
function [VOUT] = Func_GetPositionAtCirclePerimeter(V, R, RN1)

if (R ~= 0)   
    T1 = 2 * pi * RN1;

    VOUT(1) = V(1)+ R * cos(T1);
    VOUT(2) = V(2)+ R * sin(T1);
    VOUT(3) = V(3);
end


