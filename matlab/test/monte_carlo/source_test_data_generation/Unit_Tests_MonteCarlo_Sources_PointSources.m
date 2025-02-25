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
Flags = [false, true, true];

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

%Custom Point Source
FlatPos = V;
FlatDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN1, RN2);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomPointSourceFlat_U, TestCustomPointSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, [0, 0],T, FlatPolAzi, Flags);

%Directional Point Source
FlatPos = V;
FlatDir = [0, 0, 1];
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirPointSourceFlat_U, TestDirPointSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, [0, 0],T, FlatPolAzi, Flags);

%Isotropic Point Source
FlatPos = V;
FlatDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange([0, pi], [0, 2*pi], RN1, RN2);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestIsoPointSourceFlat_U, TestIsoPointSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, [0,0],T, [0,0], Flags);

%Lambertian Point Source
FlatPos = V;
FlatDir = Func_GetDirectionForLambertianDistributionRandom(RN1, RN2);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestLamPointSourceFlat_U, TestLamPointSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, [0,0],T, [0,0], Flags);

fid = fopen('UnitTests_PointSources.txt', 'w');
fprintf(fid,'%.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, %.10e, ',...
    V,U,T,AngPair, PolRange, AziRange,  polAngle);
fprintf(fid,'%.10e, %.10e, ',...
    TestCustomPointSourceFlat_U,TestCustomPointSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestDirPointSourceFlat_U,TestDirPointSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestIsoPointSourceFlat_U,TestIsoPointSourceFlat_V);
fprintf(fid,'%.10e, %.10e, ',...
    TestLamPointSourceFlat_U,TestLamPointSourceFlat_V);

fclose(fid);
