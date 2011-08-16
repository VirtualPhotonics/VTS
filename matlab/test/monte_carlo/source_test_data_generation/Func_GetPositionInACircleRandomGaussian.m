%GetPositionInACircleRandomGaussian
function [VOUT] =  Func_GetPositionInACircleRandomGaussian(V, R, BDFWHM, RN1, RN2)

if (R(2) ~= 0)  
    FactorL = R(2)/(0.8493218 * BDFWHM);
    FactorU = R(1)/(0.8493218 * BDFWHM);
    LimitL = Func_GetLimit(FactorL);
    LimitU = Func_GetLimit(FactorU);

    NR = Func_GetDoubleNormallyDistributedRandomNumbers(RN1, RN2, LimitL, LimitU);

    VOUT(1) = V(1)+ 0.8493218 * BDFWHM * NR(1);
    VOUT(2) = V(2)+ 0.8493218 * BDFWHM * NR(2);
    VOUT(3) = V(3);
end


