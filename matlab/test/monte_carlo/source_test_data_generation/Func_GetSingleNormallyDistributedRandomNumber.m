%GetSingleNormallyDistributedRandomNumber
function NR = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, Limit)

T1 = sqrt(-2.0*(log(Limit+RN1*(1.0-Limit))));
T2 = cos(2 * pi * RN2);

NR = T1*T2;





