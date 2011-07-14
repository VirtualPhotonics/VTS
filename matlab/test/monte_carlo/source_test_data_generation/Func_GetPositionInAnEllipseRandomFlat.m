%GetFlatCirclePosition
function [VOUT] = Func_GetPositionInAnEllipseRandomFlat(V, P, RN1, RN2)

VOUT(1) = V(1)+ P(1) * (2 * RN1-1);
VOUT(2) = V(2)+ P(2) * (2 * RN2-1);
VOUT(3) = V(3);


