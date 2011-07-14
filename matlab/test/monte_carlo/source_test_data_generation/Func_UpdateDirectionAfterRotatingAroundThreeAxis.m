%UpdateDirectionAfterRotatingAroundThreeAxisClockwiseLeftHanded
function [UOUT] = Func_UpdateDirectionAfterRotatingAroundThreeAxis(U, AngRot3)

ux = U(1);
uy = U(2);
uz = U(3);

cosx = cos(AngRot3(1));
sinx = sin(AngRot3(1));
cosy = cos(AngRot3(2));
siny = sin(AngRot3(2));
cosz = cos(AngRot3(3));
sinz = sin(AngRot3(3));

UOUT(1) = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
UOUT(2) = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
UOUT(3) = ux * siny + uy * sinx * cosy + uz * cosx * cosy;





