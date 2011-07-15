%UpdateDirectionAfterRotatingByGivenAnglePair
function [UOUT] = Func_UpdateDirectionAfterRotatingByGivenAnglePair(U, AngPair)

ux = U(1);
uy = U(2);
uz = U(3);

cost = cos(AngPair(1));
sint = sqrt(1-cost*cost);
cosp = cos(AngPair(2));
sinp = sin(AngPair(2));

UOUT(1) = ux * cost * cosp - uy * sinp + uz * sint * cosp;
UOUT(2) = ux * cost * sinp + uy * cosp + uz * sint * sinp;
UOUT(3) = -ux * sint + uz * cost;




