%UpdateDirectionAfterRotatingAroundZAxis
function [UOUT] = Func_UpdateDirectionAfterRotatingAroundZAxis(U, AngRot)

ux = U(1);
uy = U(2);

cost = cos(AngRot);
sint = sin(AngRot);

UOUT(1) = ux * cost - uy * sint;
UOUT(2) = ux * sint + uy * cost;
UOUT(3) = U(3);




