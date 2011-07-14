%GetGaussianRectanglePosition
function [VOUT] = Func_GetPositionInARectangleRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4)

FactorX = L(1)/BDFWHM;
FactorY = L(2)/BDFWHM;

LimitX = Func_GetLowerLimit(FactorX);
LimitY = Func_GetLowerLimit(FactorY);

NRX = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
NRY = Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);

VOUT(1) = V(1)+ 0.8493218*BDFWHM*NRX;
VOUT(2) = V(2)+ 0.8493218*BDFWHM*NRY;
VOUT(3) = V(3);


