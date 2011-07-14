%GetPositionInACuboidRandomFlat
function [VOUT] = Func_GetPositionInACuboidRandomFlat(V, L, RN1, RN2, RN3)

VOUT(1) = V(1)+ Func_GetPositionOfASymmetricalLineRandomFlat(L(1), RN1);
VOUT(2) = V(2)+ Func_GetPositionOfASymmetricalLineRandomFlat(L(2), RN2);
VOUT(3) = V(3)+ Func_GetPositionOfASymmetricalLineRandomFlat(L(3), RN3);


