%GetDoubleNormallyDistributedRandomNumbers
function [NR] = Func_GetDoubleNormallyDistributedRandomNumbers(RN1, RN2, Limit)

T1 = sqrt(-2.0*(log(Limit+RN1*(1.0-Limit))));
T2 = 2 * pi * RN2;

NR(1) = T1*cos(T2);
NR(2) = T1*sin(T2);




