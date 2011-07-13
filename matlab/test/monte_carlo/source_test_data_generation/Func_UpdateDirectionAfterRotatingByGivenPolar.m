%UpdateDirectionAfterRotatingByGivenPolarAndAzimuthalAnglePair
function [U] = Func_UpdateDirectionAfterRotatingByGivenPolar(U, AngPair)

ux = U(1);
uy = U(2);
uz = U(3);

cost = cos(AngPair(1));
sint = sin(AngPair(1));
cosp = cos(AngPair(2));
sinp = sin(AngPair(2));

U(1) = ux * cosp * cost - uy * sint + uz * sinp * cost;
U(2) = ux * cosp * sint + uy * cost + uz * sinp * sint;
U(3) = -ux * sinp + uz * cost;





