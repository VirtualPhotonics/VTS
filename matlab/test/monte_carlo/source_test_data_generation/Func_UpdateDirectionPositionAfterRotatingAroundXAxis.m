%UpdateDireactionAndPositionAfterRotatingAroundXAxis
function [UOUT, VOUT] = Func_UpdateDirectionPositionAfterRotatingAroundXAxis(U, V, AngRotX)

uy = U(2);
uz = U(3);

y = V(2);
z = V(3);

cost = cos(AngRotX);
sint = sqrt(1-cost*cost);

UOUT(1) = U(1);
UOUT(2) = uy * cost - uz * sint;
UOUT(3) = uy * sint + uz * cost;

VOUT(1) = V(1);
VOUT(2) = y * cost - z * sint;
VOUT(3) = y * sint + z * cost;





