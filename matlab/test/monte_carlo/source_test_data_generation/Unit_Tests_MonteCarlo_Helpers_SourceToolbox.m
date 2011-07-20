clear all;

%Input parameters%

%_position
x = 1.0;
y = 2.0;
z = 3.0;

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

% ellipse parameters
a = 2.0;
b = 2.5;
c = 3.0;

%length , width & height
l = 1.0;
w = 2.0;
h = 3.0;

%outer radius and inner radius
r1 = 1.0;
r2 = 2.0;

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
AngRot3 = [AngRotX, AngRotY, AngRotZ];
P = [a, b, c];
L = [l,w, h];
R = [r1, r2];
G = [BDFWHM, Limit, Factor];


%_polarAngle
polAngle = 0.25*pi;

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

%Unit TESTS%

% unit test for all static methods in Func 
Test01 = Func_GetDirectionForGiven2DPositionAndGivenPolarAngle(V, polAngle);
Test02 = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN1, RN2);
Test03 = Func_GetDirectionForIsotropicDistributionRandom(RN1, RN2);
Test04 = Func_GetDoubleNormallyDistributedRandomNumbers(RN1, RN2, Limit);
Test05 = Func_GetLowerLimit(Factor);
Test06 = Func_GetPolarAzimuthalPairForGivenAngleRangeRandom(PolRange, AziRange, RN1, RN2);
Test07 = Func_GetPolarAzimuthalPairFromDirection(U);
Test08 = Func_GetPositionInACircleRandomFlat(V, R, RN1, RN2);
Test09 = Func_GetPositionInACircleRandomGaussian(V, R, BDFWHM, RN1, RN2);
Test10 = Func_GetPositionInACuboidRandomFlat(V, L, RN1, RN2, RN3);
Test11 = Func_GetPositionInACuboidRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4, RN5, RN6);
Test12 = Func_GetPositionInALineRandomFlat(V, L, RN1);
Test13 = Func_GetPositionInALineRandomGaussian(V, L, BDFWHM, RN1, RN2);
Test14 = Func_GetPositionInAnEllipseRandomFlat(V, P, RN1, RN2);
Test15 = Func_GetPositionInAnEllipseRandomGaussian(V, P, BDFWHM, RN5, RN6, RN7, RN8);
Test16 = Func_GetPositionInAnEllipsoidRandomFlat(V, P, RN1, RN2, RN3);
Test17 = Func_GetPositionInAnEllipsoidRandomGaussian(V, P, BDFWHM, RN13, RN14, RN15, RN16, RN17, RN18);
Test18 = Func_GetPositionInARectangleRandomFlat(V, L, RN1, RN2);
Test19 = Func_GetPositionInARectangleRandomGaussian(V, L, BDFWHM, RN1, RN2, RN3, RN4);
Test20 = Func_GetPositionOfASymmetricalLineRandomFlat(L(1), RN1);
Test21 = Func_GetSingleNormallyDistributedRandomNumber(RN1, RN2, Limit);
Test22 = Func_UpdateDirectionAfterRotatingAroundThreeAxis(U, AngRot3);
Test23 = Func_UpdateDirectionAfterRotatingAroundXAxis(U, AngRotX);
Test24 = Func_UpdateDirectionAfterRotatingAroundYAxis(U, AngRotY);
Test25 = Func_UpdateDirectionAfterRotatingAroundZAxis(U, AngRotZ);
Test26 = Func_UpdateDirectionAfterRotatingByGivenAnglePair(U, AngPair);
Flags = [true, true, false];
[Test27_U, Test27_V] = Func_UpdateDirectionAndPositionAfterGivenFlags(U, V, AngPair, T, AngPair, Flags);
Flags = [true, true, true];
[Test28_U, Test28_V] = Func_UpdateDirectionAndPositionAfterGivenFlags(U, V, AngPair, T, AngPair, Flags);
[Test29_U, Test29_V] = Func_UpdateDirectionPositionAfterRotatingAroundXAxis(U, V, AngRotX);
[Test30_U, Test30_V] = Func_UpdateDirectionPositionAfterRotatingAroundYAxis(U, V, AngRotY);
[Test31_U, Test31_V] = Func_UpdateDirectionPositionAfterRotatingAroundZAxis(U, V, AngRotZ);
[Test32_U, Test32_V] = Func_UpdateDirectionPositionAfterRotatingByAnglePair(U, V, AngPair);
Test33 = Func_UpdatePolarAngleForDirectionalSources(r2, r1, polAngle);
Test34 = Func_UpdatePositionAfterTranslation(V, T);

fid = fopen('UnitTests_SourceToolbox.txt', 'w');
fprintf(fid,'%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t',...
    V,U,T,AngPair, PolRange, AziRange, AngRot3, P, L, R, G, polAngle);
fprintf(fid,'%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t',...
    Test01,Test02,Test03,Test04,Test05,Test06,Test07,Test08,...
    Test09,Test10,Test11,Test12,Test13);
fprintf(fid,'%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t',...
    Test14,Test15,Test16,Test17,Test18,Test19,Test20,Test21,...
    Test22,Test23,Test24,Test25,Test26);
fprintf(fid,'%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t%.10e\t',...
    Test27_U,Test27_V,Test28_U,Test28_V,Test29_U,Test29_V,...
    Test30_U,Test30_V,Test31_U,Test31_V,Test32_U,Test32_V,Test33, Test34);
fclose(fid);


