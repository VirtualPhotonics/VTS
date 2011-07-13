%GetPositionInACuboidRandomGaussian
function [VOUT] = Func_GetPositionInACuboidRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4, RN5, RN6)

FactorX = L(1)/BDFWHM;
FactorY = L(2)/BDFWHM;
FactorZ = L(3)/BDFWHM;

LimitX = Func_GetLowerLimit(FactorX);
LimitY = Func_GetLowerLimit(FactorY);
LimitZ = Func_GetLowerLimit(FactorZ);

NRX = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
NRY = Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);
NRZ = Func_GetSingleNormallyDistributedRandomNumber(RN5, RN6, LimitZ);

VOUT(1) = V(1)+ 0.8493218*BDFWHM*NRX;
VOUT(2) = V(2)+ 0.8493218*BDFWHM*NRY;
VOUT(3) = V(3)+ 0.8493218*BDFWHM*NRZ;


