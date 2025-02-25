%GetDirectionForLambertianDistributionRandom
function [UOUT] = Func_GetDirectionForLambertianDistributionRandom(RN1, RN2)

lambertOrder = 1;
cost = power(RN1, 1.0 / (lambertOrder + 1));
sint = sqrt(1-cost*cost);

phi = 2*pi*RN2;
cosp = cos(phi);
sinp = sin(phi);

UOUT(1) = sint * cosp;
UOUT(2) = sint * sinp;
UOUT(3) = cost;



