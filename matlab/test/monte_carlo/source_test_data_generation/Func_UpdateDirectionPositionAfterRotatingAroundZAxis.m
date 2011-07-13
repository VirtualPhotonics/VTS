%UpdateDireactionAndPositionAfterRotatingAroundZAxis
function [UOUT, VOUT] = Func_UpdateDirectionPositionAfterRotatingAroundZAxis(U, V, AngRotZ)

ux = U(1);
uy = U(2);

x = V(1);
y = V(2);

cost = cos(AngRotZ);
sint = sqrt(1-cost*cost);

UOUT(1) = ux * cost - uy * sint;
UOUT(2) = ux * sint + uy * cost;
UOUT(3) = U(3);

VOUT(1) = x * cost - y * sint;
VOUT(2) = x * sint + y * cost;
VOUT(3) = V(3);




