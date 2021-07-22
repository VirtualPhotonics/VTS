%UpdateDirectionAndPositionAfterGivenFlags
function [UOUT, VOUT] = Func_UpdateDirectionAndPositionAfterGivenFlags(U, V,  SAngPair, T, BAngPair, Flags)
% V is position, U is direction
if (Flags(3))    
   [U] = Func_UpdateDirectionAfterRotatingByGivenAnglePair(U, [BAngPair(1), 0]);
    if (BAngPair(2)~=0)
        [U, V] = Func_UpdateDirectionPositionAfterRotatingByAnglePair(U, V, [0 BAngPair(2)]);
    end
end

if (Flags(1))     
    [U, V] = Func_UpdateDirectionPositionAfterRotatingByAnglePair(U, V, SAngPair);
end

if (Flags(2))     
    [V] = Func_UpdatePositionAfterTranslation(V, T);
end

UOUT = U;
VOUT = V;





