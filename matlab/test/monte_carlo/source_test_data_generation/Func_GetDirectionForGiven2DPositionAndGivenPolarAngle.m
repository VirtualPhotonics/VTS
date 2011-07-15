%GetDirectionForGiven2DPositionAndGivenPolarAngle
function [UOUT] = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(V, pAngle)

Radius = sqrt(V(1)*V(1) + V(2)*V(2));

if (Radius == 0)
    UOUT(1) = 0;
    UOUT(2) = 0;
    UOUT(3) = cos(pAngle);     
else
    UOUT(1) = sin(pAngle) * V(1)/Radius;
    UOUT(2) = sin(pAngle) * V(2)/Radius;
    UOUT(3) = cos(pAngle); 
end



