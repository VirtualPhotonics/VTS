%UpdateDirectionPositionAfterRotatingByAnglePair
function [UOUT, VOUT] = Func_UpdateDirectionPositionAfterRotatingByAnglePair(U, V, AngPair)

ux = U(1);
uy = U(2);
uz = U(3);

x = V(1);
y = V(2);
z = V(3);

cost = cos(AngPair(1));
sint = sin(AngPair(1));
cosp = cos(AngPair(2));
sinp = sin(AngPair(2));


UOUT(1) = ux * cost * cosp - uy * sinp + uz * sint * cosp;
UOUT(2) = ux * cost * sinp + uy * cosp + uz * sint * sinp;
UOUT(3) = -ux * sint + uz * cost;

VOUT(1) = x * cost * cosp - y * sinp + z * sint * cosp;
VOUT(2) = x * cost * sinp + y * cosp + z * sint * sinp;
VOUT(3) = -x * sint + z * cost;







