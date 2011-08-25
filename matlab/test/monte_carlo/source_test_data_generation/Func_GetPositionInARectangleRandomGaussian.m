%GetGaussianRectanglePosition
function [VOUT] = Func_GetPositionInARectangleRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4)

FactorX = 0.5*L(1)/(BDFWHM * 0.8493218);
FactorY = 0.5*L(2)/(BDFWHM * 0.8493218);

LimitX = Func_GetLimit(FactorX);
LimitY = Func_GetLimit(FactorY);

NRX = 0.8493218*BDFWHM*Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
NRY = 0.8493218*BDFWHM*Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);

VOUT(1) = V(1)+ NRX;
VOUT(2) = V(2)+ NRY;
VOUT(3) = V(3);


