%UpdateDirectionAfterRotatingAroundYAxis
function [UOUT] = Func_UpdateDirectionAfterRotatingAroundYAxis(U, AngRot)

ux = U(1);
uz = U(3);

cost = cos(AngRot);
sint = sin(AngRot);

UOUT(1) = ux * cost + uz * sint;
UOUT(2) = U(2);
UOUT(3) = -ux * sint + uz * cost;




