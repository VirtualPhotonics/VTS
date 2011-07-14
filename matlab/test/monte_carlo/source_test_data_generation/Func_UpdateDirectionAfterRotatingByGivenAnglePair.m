%UpdateDirectionAfterRotatingByGivenAnglePair
function [UOUT] = Func_UpdateDirectionAfterRotatingByGivenAnglePair(U, AngPair)

ux = U(1);
uy = U(2);
uz = U(3);

cost = cos(AngPair(1));
sint = sqrt(1-cost*cost);
cosp = cos(AngPair(2));
sinp = sin(AngPair(2));

UOUT(1) = ux * cosp * cost - uy * sint + uz * sinp * cost;
UOUT(2) = ux * cosp * sint + uy * cost + uz * sinp * sint;
UOUT(3) = -ux * sinp + uz * cost;




