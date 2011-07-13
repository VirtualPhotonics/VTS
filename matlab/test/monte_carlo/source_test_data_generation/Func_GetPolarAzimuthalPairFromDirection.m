%GetPolarAndAzimuthalAnglesFromDirection
function [AngPair] = Func_GetPolarAzimuthalPairFromDirection(U)

x = U(1);
y = U(2);
z = U(3);

AngPair(1) = acos(z);

if ((x~= 0.0) || (y ~= 0.0))
     r = sqrt(x * x + y * y);
     if (y >= 0.0)
        AngPair(2) = acos(x / r);
     else
        AngPair(2) = 2 *pi - acos(x / r);
     end
else
     AngPair(2) = 0.0;
end





