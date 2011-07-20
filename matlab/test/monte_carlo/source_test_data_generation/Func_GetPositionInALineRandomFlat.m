%GetFlatLinePosition
function [VOUT] = Func_GetPositionInALineRandomFlat(V, L, RN1)

VOUT(1) = V(1)+ Func_GetPositionOfASymmetricalLineRandomFlat(L(1), RN1);
VOUT(2) = V(2);
VOUT(3) = V(3);

