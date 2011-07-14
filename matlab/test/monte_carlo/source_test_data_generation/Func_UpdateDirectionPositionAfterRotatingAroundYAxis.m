%UpdateDireactionAndPositionAfterRotatingAroundYAxis
function [UOUT, VOUT] = Func_UpdateDirectionPositionAfterRotatingAroundYAxis(U, V, AngRotY)

ux = U(1);
uz = U(3);

x = V(1);
z = V(3);

cost = cos(AngRotY);
sint = sqrt(1-cost*cost);

UOUT(1) = ux * cost + uz * sint;
UOUT(2) = U(2);
UOUT(3) = -ux * sint + uz * cost;

VOUT(1) = x * cost + z * sint;
VOUT(2) = V(2);
VOUT(3) = -x * sint + z * cost;





