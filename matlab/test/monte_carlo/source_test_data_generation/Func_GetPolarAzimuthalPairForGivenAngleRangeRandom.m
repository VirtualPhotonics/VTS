%GetPolarAzimuthalPairForGivenAngleRangeRandom
function [AngPair] = Func_GetPolarAzimuthalPairForGivenAngleRangeRandom(PolRange, AziRange, RN1, RN2)

cosmax = cos(PolRange(1));
cosmin = cos(PolRange(2));

AngPair(1) = acos(cosmin + RN1*(cosmax - cosmin));
AngPair(2) = AziRange(1) + RN2 * (AziRange(2) - AziRange(1));


