%GetPolarAzimuthalPairFromDirection
function [AngPair] = Func_GetPolarAndAzimuthalAnglesFromDirection(U)

ux = U(1);
uy = U(2);
uz = U(3);

AngPair(1) = acos(uz);

if ((ux~= 0.0) || (uy ~= 0.0))
     r = sqrt(ux * ux + uy * uy);
     if (y >= 0.0)
        AngPair(2) = acos(ux / r);
     else
        AngPair(2) = 2 *pi - acos(ux / r);
     end
else
     AngPair(2) = 0.0;        
end






