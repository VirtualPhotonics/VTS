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

%volume parameters
l = 1.0;
w = 2.0;
h = 3.0;

%elliptical parameters
a = 0.8;
b = 1.2;
c = 1.5;

%circular parameters
ro = 2.0;
ri = 1.0;

%Gaussian parameters
BDFWHM = 0.8;
Limit = 0.5;
Factor = 0.5;

%Define vectors
V = [x, y, z];
U = [ux, uy, uz];
T = [xt, yt, zt];
L = [l, w, h];
P = [a, b, c];
R = [ri, ro];

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

%Custom Flat Circular Source
FlatPos = Func_GetPositionInACircleRandomFlat(V, R, RN1, RN2);
FlatDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomCirSourceFlat_U, TestCustomCirSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Custom Gaussian Circular Source
GaussPos = Func_GetPositionInACircleRandomGaussian(V, R, BDFWHM, RN1, RN2);
GaussDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomCirSourceGauss_U, TestCustomCirSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Directional Flat Circular Source
FlatPos = Func_GetPositionInACircleRandomFlat(V, R, RN1, RN2);
CurLength = sqrt(FlatPos(1)*FlatPos(1)+FlatPos(2)*FlatPos(2));
FlatPolarAngle = Func_UpdatePolarAngleForDirectionalSources(R(2), CurLength, polAngle);
FlatDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(FlatPos, FlatPolarAngle);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirCirSourceFlat_U, TestDirCirSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Directional Gaussian Circular Source
GaussPos = Func_GetPositionInACircleRandomGaussian(V, R, BDFWHM, RN1, RN2);
CurLength = sqrt(GaussPos(1)*GaussPos(1)+GaussPos(2)*GaussPos(2));
GaussPolarAngle = Func_UpdatePolarAngleForDirectionalSources(R(2), CurLength, polAngle);
GaussDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(GaussPos, GaussPolarAngle);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirCirSourceGauss_U, TestDirCirSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);


%Custom Flat Elliptical Source
FlatPos = Func_GetPositionInAnEllipseRandomFlat(V, P, RN1, RN2);
FlatDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomEllipticalSourceFlat_U, TestCustomEllipticalSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Custom Gaussian Elliptical Source
GaussPos = Func_GetPositionInAnEllipseRandomGaussian(V, P, BDFWHM, RN1, RN2, RN3, RN4);
GaussDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN5, RN6);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomEllipticalSourceGauss_U, TestCustomEllipticalSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Directional Flat Elliptical Source
FlatPos = Func_GetPositionInAnEllipseRandomFlat(V, P, RN1, RN2);
CurLength = sqrt(FlatPos(1)*FlatPos(1)+FlatPos(2)*FlatPos(2));
FlatPolarAngle = Func_UpdatePolarAngleForDirectionalSources(P(1), CurLength, polAngle);
FlatDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(FlatPos, FlatPolarAngle);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirEllipticalSourceFlat_U, TestDirEllipticalSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Directional Gaussian Elliptical Source
GaussPos = Func_GetPositionInAnEllipseRandomGaussian(V, P, BDFWHM, RN1, RN2, RN3, RN4);
CurLength = sqrt(GaussPos(1)*GaussPos(1)+GaussPos(2)*GaussPos(2));
GaussPolarAngle = Func_UpdatePolarAngleForDirectionalSources(P(1), CurLength, polAngle);
GaussDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(GaussPos, GaussPolarAngle);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirEllipticalSourceGauss_U, TestDirEllipticalSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Custom Flat Rectangular Source
FlatPos = Func_GetPositionInARectangleRandomFlat(V, L, RN1, RN2);
FlatDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomRectanSourceFlat_U, TestCustomRectanSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Custom Gaussian Rectangular Source
GaussPos = Func_GetPositionInARectangleRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4);
GaussDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN5, RN6);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestCustomRectanSourceGauss_U, TestCustomRectanSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);

%Directional Flat Rectangular Source
FlatPos = Func_GetPositionInARectangleRandomFlat(V, L, RN1, RN2);
CurLength = sqrt(FlatPos(1)*FlatPos(1)+FlatPos(2)*FlatPos(2));
FlatPolarAngle = Func_UpdatePolarAngleForDirectionalSources(L(1), CurLength, polAngle);
FlatDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(FlatPos, FlatPolarAngle);
FlatPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirRectanSourceFlat_U, TestDirRectanSourceFlat_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (FlatDir, FlatPos, FlatPolAzi,T, AngPair, Flags);

%Directional Gaussian Elliptical Source
GaussPos = Func_GetPositionInARectangleRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4);
CurLength = sqrt(GaussPos(1)*GaussPos(1)+GaussPos(2)*GaussPos(2));
GaussPolarAngle = Func_UpdatePolarAngleForDirectionalSources(L(1), CurLength, polAngle);
GaussDir = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(GaussPos, GaussPolarAngle);
GaussPolAzi = Func_GetPolarAzimuthalPairFromDirection(U);
[TestDirRectanSourceGauss_U, TestDirRectanSourceGauss_V] = Func_UpdateDirectionAndPositionAfterGivenFlags...
    (GaussDir, GaussPos, GaussPolAzi, T, AngPair, Flags);


fid = fopen('UnitTests_SurfaceEmitting2DSources.txt', 'w');
fprintf(fid,'%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t',...
    V,U,T,AngPair, PolRange, AziRange, L,P, R, BDFWHM, polAngle);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomCirSourceFlat_U,TestCustomCirSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomCirSourceGauss_U,TestCustomCirSourceGauss_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirCirSourceFlat_U,TestDirCirSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirCirSourceGauss_U,TestDirCirSourceGauss_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomEllipticalSourceFlat_U,TestCustomEllipticalSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomEllipticalSourceGauss_U,TestCustomEllipticalSourceGauss_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirEllipticalSourceFlat_U,TestDirEllipticalSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirEllipticalSourceGauss_U,TestDirEllipticalSourceGauss_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomRectanSourceFlat_U,TestCustomRectanSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestCustomRectanSourceGauss_U,TestCustomRectanSourceGauss_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirRectanSourceFlat_U,TestDirRectanSourceFlat_V);
fprintf(fid,'%.10e\t%.10e\t',...
    TestDirRectanSourceGauss_U,TestDirRectanSourceGauss_V);

fclose(fid);
