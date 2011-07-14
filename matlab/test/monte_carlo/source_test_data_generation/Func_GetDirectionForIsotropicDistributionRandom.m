%GetDirectionForIsotropicDistributionRandom
function [UOUT] = Func_GetDirectionForIsotropicDistributionRandom(RN1, RN2)

cost = 2*RN1 - 1;
sint = sqrt(1-cost*cost);

phi = 2*pi*RN2;
cosp = cos(phi);
sinp = sin(phi);

UOUT(1) = sint * cosp;
UOUT(2) = sint * sinp;
UOUT(3) = cost;



