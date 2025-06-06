%GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom
function [UOUT] = Func_GetDirectionForGivenPolarAzimuthalAngleRange(PolRange, AziRange, RN1, RN2)

if ((PolRange(1) - PolRange(2) == 0.0) && (AziRange(1) - AziRange(2)== 0.0))
    UOUT(1)  = 0.0;
    UOUT(2)  = 0.0;
    UOUT(3)  = 1.0;
else
    cosmax = cos(PolRange(1));
    cosmin = cos(PolRange(2));

    cost = cosmin + RN1*(cosmax - cosmin);
    sint = sqrt(1 - cost * cost);

    phi = AziRange(1) + RN2 * (AziRange(2) - AziRange(1));
    cosp = cos(phi);
    sinp = sin(phi);

    UOUT(1) = sint * cosp;
    UOUT(2) = sint * sinp;
    UOUT(3) = cost;        
end


