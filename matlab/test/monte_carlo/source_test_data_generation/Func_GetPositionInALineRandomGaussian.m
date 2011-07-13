%GetGaussianLinePosition
function [V] = Func_GetPositionInALineRandomGaussian(V, L, BDFWHM, RN1, RN2)

FactorX = L(1)/BDFWHM;

LimitX = Func_GetLowerLimit(FactorX);

NR = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);

V(1) = V(1)+ 0.8493218*BDFWHM*NR;
V(2) = V(2);
V(3) = V(3);



