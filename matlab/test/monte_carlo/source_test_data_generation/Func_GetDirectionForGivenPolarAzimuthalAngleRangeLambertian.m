%GetDirectionForGivenPolarAndAzimuthalAngleRangeLambertianRandom
function [UOUT] = Func_GetDirectionForGivenPolarAzimuthalAngleRangeLambertian(PolRange, AziRange, LambertOrder, RN1, RN2)

if ((PolRange(1) - PolRange(2) == 0.0) && (AziRange(1) - AziRange(2)== 0.0))
    UOUT(1)  = 0.0;
    UOUT(2)  = 0.0;
    UOUT(3)  = 1.0;
else
    cost = power(RN1, 1.0 / (LambertOrder + 1));
    sint = sqrt(1 - cost * cost);

    phi = AziRange(1) + RN2 * (AziRange(2) - AziRange(1));
    cosp = cos(phi);
    sinp = sin(phi);

    UOUT(1) = sint * cosp;
    UOUT(2) = sint * sinp;
    UOUT(3) = cost;        
end


