%GetGaussianEllipsePosition
function [VOUT] =  Func_GetPositionInAnEllipseRandomGaussian(V, P, BDFWHM, RN1, RN2, RN3, RN4)

FactorX = P(1)/(BDFWHM * 0.8493218);
FactorY = P(2)/(BDFWHM * 0.8493218);

LimitX = Func_GetLimit(FactorX);
LimitY = Func_GetLimit(FactorY);

% The x and y position should be from the center of the circle or radius,
% so beamDiameterFwhm should be beamRadiusFwhm = (beamDiameterFwhm/2)
T1 = 0.8493218*(BDFWHM/2)*Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, LimitX);
T2 = 0.8493218*(BDFWHM/2)*Func_GetSingleNormallyDistributedRandomNumber(RN3, RN4, LimitY);

VOUT(1) = V(1)+ T1;
VOUT(2) = V(2)+ T2;
VOUT(3) = V(3);



