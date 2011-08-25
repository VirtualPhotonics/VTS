%GetPositionInACuboidRandomGaussian
function [VOUT] = Func_GetPositionInACuboidRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4, RN5, RN6)

FactorX = 0.5*L(1)/(BDFWHM * 0.8493218);
FactorY = 0.5*L(2)/(BDFWHM * 0.8493218);
FactorZ = 0.5*L(3)/(BDFWHM * 0.8493218);

LimitX = Func_GetLimit(FactorX);
LimitY = Func_GetLimit(FactorY);
LimitZ = Func_GetLimit(FactorZ);

NRX = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
NRY = Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);
NRZ = Func_GetSingleNormallyDistributedRandomNumber(RN5, RN6, LimitZ);

VOUT(1) = V(1)+ 0.8493218*BDFWHM*NRX;
VOUT(2) = V(2)+ 0.8493218*BDFWHM*NRY;
VOUT(3) = V(3)+ 0.8493218*BDFWHM*NRZ;


