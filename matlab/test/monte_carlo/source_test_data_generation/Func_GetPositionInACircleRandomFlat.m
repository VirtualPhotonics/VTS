%GetPositionInACircleRandomFlat
function [VOUT] = Func_GetPositionInACircleRandomFlat(V, R, RN1, RN2)

if (R(2) ~= 0)   
    T1 = 2 * pi * RN1;
    T2 = sqrt(R(1) * R(1) + (R(2) * R(2) - R(1) * R(1)) * RN2);

    VOUT(1) = V(1)+ T2 * cos(T1);
    VOUT(2) = V(2)+ T2 * sin(T1);
    VOUT(3) = V(3);
end


