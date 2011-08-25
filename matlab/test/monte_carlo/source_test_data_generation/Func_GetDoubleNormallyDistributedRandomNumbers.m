%GetDoubleNormallyDistributedRandomNumbers
function [NR] = Func_GetDoubleNormallyDistributedRandomNumbers(RN1, RN2, LimitL, LimitU)

T1 = sqrt(-2.0*(log(LimitL+RN1*(LimitU-LimitL))));
T2 = 2 * pi * RN2;

NR(1) = T1*cos(T2);
NR(2) = T1*sin(T2);




