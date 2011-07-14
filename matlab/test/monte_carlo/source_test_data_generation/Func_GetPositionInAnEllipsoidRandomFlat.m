%GetPositionInAnEllipsoidRandomFlat
function [VOUT] = Func_GetPositionInAnEllipsoidRandomFlat(V, P, RN1, RN2, RN3)

VOUT(1) = V(1)+ P(1) * (2 * RN1-1);
VOUT(2) = V(2)+ P(2) * (2 * RN2-1);
VOUT(3) = V(3)+ P(3) * (2 * RN3-1);
