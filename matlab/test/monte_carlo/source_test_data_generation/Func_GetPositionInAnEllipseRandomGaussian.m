%GetGaussianEllipsePosition
function [VOUT] =  Func_GetPositionInAnEllipseRandomGaussian(V, P, BDFWHM, RN1, RN2, RN3, RN4)

FactorX = 2 * P(1)/BDFWHM;
FactorY = 2 * P(2)/BDFWHM;

LimitX = Func_GetLowerLimit(FactorX);
LimitY = Func_GetLowerLimit(FactorY);

T1 = P(1)* Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
T2 = P(2)* Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);

VOUT(1) = V(1)+ 0.8493218*BDFWHM*T1;
VOUT(2) = V(2)+ 0.8493218*BDFWHM*T2;
VOUT(3) = V(3);



