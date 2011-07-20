%UpdateDirectionAndPositionAfterGivenFlags
function [UOUT, VOUT] = Func_UpdateDirectionAndPositionAfterGivenFlags(U, V,  SAngPair, T, BAngPair, Flags)

if (Flags(3))    
    U = Func_UpdateDirectionAfterRotatingByGivenAnglePair(U, BAngPair);
end

if (Flags(1))     
    [U, V] = Func_UpdateDirectionPositionAfterRotatingByAnglePair(U, V, SAngPair);
end

if (Flags(2))     
    [V] = Func_UpdatePositionAfterTranslation(V, T);
end

UOUT = U;
VOUT = V;





