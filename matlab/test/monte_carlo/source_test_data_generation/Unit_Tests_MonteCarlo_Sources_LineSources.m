clear all;

%Input parameters%

%_position
x = 0.0;
y = 0.0;
z = 0.0;

%_direction
ux = 0.707106781186548;
uy = 0.707106781186548;
uz = 0.0;

%_translation
xt = 1.0;
yt = -2.5;
zt = 1.2;

%emission angPair
theta = 0.25*pi;
phi = 0.25*pi;

%_polRange
polAng1 = 0;
polAng2 = 0.5*pi;

%_aziRange
aziAng1 = 0;
aziAng2 = pi;

%_angRot
AngRotX = 0.5*pi;
AngRotY = 0.5*pi;
AngRotZ = 0.25*pi;

%length 
l = 1.0;

%Gaussian parameters
BDFWHM = 0.8;
Limit = 0.5;
Factor = 0.5;

%Define vectors
V = [x, y, z];
U = [ux, uy, uz];
T = [xt, yt, zt];
AngPair = [theta, phi];
PolRange = [polAng1, polAng2];
AziRange = [aziAng1, aziAng2];

%_polarAngle
polAngle = 0.25*pi;
Flags = [true, true, true];

%First 18 random numbers for SEED = 0
RN1 = 0.54881350243203653;
RN2 = 0.59284461652693443;
RN3 = 0.715189365138111;
RN4 = 0.84426574428665124;
RN5 = 0.60276337051828466;
RN6 = 0.85794562004924413;
RN7 = 0.544883177509737;
RN8 = 0.84725173745473192;
RN9 = 0.42365479688710878;
RN10 = 0.6235636965426532;
RN11 = 0.64589411524261675;
RN12 = 0.38438170831286855;
RN13 = 0.43758721007909329;
RN14 = 0.29753460532462567;
RN15 = 0.89177300196415121;
RN16 = 0.0567129757387361;
RN17 = 0.96366276428188724;
RN18 = 0.27265629458070179;

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%Unit TESTS%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%Custom Flat Line Source
FlatPos = Func_GetPositionInALineRandomFlat(V, l, RN1);
FlatDir = Func_GetDirectionForGivenPolarAzimuthalAngleRange(PolRange, AziRange, RN2, RN3);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomLineSourceFlat_U, TestCustomLineSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Custom Gaussian Line Source
GaussPos = Func_GetPositionInALineRandomGaussian(V, l, BDFWHM, RN1, RN2);
GaussDir = Func_GetDirectionForGivenPolarAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomLineSourceGauss_U, TestCustomLineSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Directional Flat Line Source
FlatPos = Func_GetPositionInALineRandomFlat(V, l, RN1);
FlatPolarAngle = Func_UpdatePolarAngleForDirectionalSources(0.5*l, FlatPos(1), polAngle);
FlatDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(FlatPos, FlatPolarAngle);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirLineSourceFlat_U, TestDirLineSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Directional Gaussian Line Source
GaussPos = Func_GetPositionInALineRandomGaussian(V, l, BDFWHM, RN1, RN2);
GaussPolarAngle = Func_UpdatePolarAngleForDirectionalSources(0.5*l, GaussPos(1), polAngle);
GaussDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(GaussPos, GaussPolarAngle);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirLineSourceGauss_U, TestDirLineSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Isotropic Flat Line Source
FlatPos = Func_GetPositionInALineRandomFlat(V, l, RN1);
FlatDir = Func_GetDirectionForGivenPolarAzimuthalAngleRange([0, pi], [0, 2*pi], RN2, RN3);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestIsoLineSourceFlat_U, TestIsoLineSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Isotropic Gaussian Line Source
GaussPos = Func_GetPositionInALineRandomGaussian(V, l, BDFWHM, RN1, RN2);
GaussDir = Func_GetDirectionForGivenPolarAzimuthalAngleRange([0, pi], [0, 2*pi], RN3, RN4);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestIsoLineSourceGauss_U, TestIsoLineSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Lambertian Flat Line Source
FlatPos = Func_GetPositionInALineRandomFlat(V, l, RN1);
FlatDir = Func_GetDirectionForLambertianDistributionRandom(RN2, RN3);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestLamLineSourceFlat_U, TestLamLineSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Lambertian Gaussian Line Source
GaussPos = Func_GetPositionInALineRandomGaussian(V, l, BDFWHM, RN1, RN2);
GaussDir = Func_GetDirectionForLambertianDistributionRandom(RN3, RN4);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestLamLineSourceGauss_U, TestLamLineSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);


fid = fopen('UnitTests_LineSources.txt', 'w');
fprintf(fid,'%.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, ',...
    V,U,T,AngPair, PolRange, AziRange,  l, BDFWHM, polAngle);
fprintf(fid,'%.10e, %.10e, ',...
    TestCustomLineSourceFlat_U,TestCustomLineSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestCustomLineSourceGauss_U,TestCustomLineSourceGauss_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestDirLineSourceFlat_U,TestDirLineSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestDirLineSourceGauss_U,TestDirLineSourceGauss_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestIsoLineSourceFlat_U,TestIsoLineSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestIsoLineSourceGauss_U,TestIsoLineSourceGauss_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestLamLineSourceFlat_U,TestLamLineSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestLamLineSourceGauss_U,TestLamLineSourceGauss_V);
fclose(fid);
