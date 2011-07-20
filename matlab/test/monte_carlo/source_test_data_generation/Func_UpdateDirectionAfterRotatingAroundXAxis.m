%UpdateDirectionAfterRotatingAroundXAxis
function [UOUT] = Func_UpdateDirectionAfterRotatingAroundXAxis(U, AngRot)

uy = U(2);
uz = U(3);

cost = cos(AngRot);
sint = sin(AngRot);

UOUT(1) = U(1);
UOUT(2) = uy * cost - uz * sint;
UOUT(3) = uy * sint + uz * cost;




