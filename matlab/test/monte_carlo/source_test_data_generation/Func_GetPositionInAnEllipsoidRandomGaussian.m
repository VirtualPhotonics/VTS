%GetPositionInAnEllipsoidRandomGaussian
function [VOUT] = Func_GetPositionInAnEllipsoidRandomGaussian(V, P, BDFWHM, RN1, RN2, RN3, RN4, RN5, RN6)

FactorX = 2 * P(1)/BDFWHM;
FactorY = 2 * P(2)/BDFWHM;
FactorZ = 2 * P(3)/BDFWHM;

LimitX = Func_GetLowerLimit(FactorX);
LimitY = Func_GetLowerLimit(FactorY);
LimitZ = Func_GetLowerLimit(FactorZ);

T1 = P(1)* Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
T2 = P(2)* Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);
T3 = P(3)* Func_GetSingleNormallyDistributedRandomNumber(RN5, RN6, LimitZ);

VOUT(1) = V(1)+ 0.8493218*BDFWHM*T1;
VOUT(2) = V(2)+ 0.8493218*BDFWHM*T2;
VOUT(3) = V(3)+ 0.8493218*BDFWHM*T3;