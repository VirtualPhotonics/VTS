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


U = [ux, uy, uz];
V = [x, y, z];
P = [a, b, c];
L = [l,w, h];
R = [r1, r2];
T = [xt, yt, zt];
AngPair = [theta, phi];
PolRange = [polAng1, polAng2];
AziRange = [aziAng1, aziAng2];
AngRot3 = [AngRotX, AngRotY, AngRotZ];

%Unit TESTS%


%Custom Line Source
%Flat
TestCustomLineSourcePos = Func_GetPositionInALineRandomFlat([0,0,0], L, RN1);
TestCustomLineSourceDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN2, RN3);
%Gaussian
%TestCustomLineSourcePos = Func_GetPositionInALineRandomGaussian([0,0,0], L, BDFWHM, RN1, RN2);
%TestCustomLineSourceDir = Func_GetDirectionForGivenPolaAzimuthalAngleRange(PolRange, AziRange, RN3, RN4);
TestCustomLineSourcePolAzi = Func_GetPolarAzimuthalPairFromDirection([1, 0, 0]);
Flags = [true, true, true];
[TestCustomLineSource_U, TestCustomLineSource_V] = Func_UpdateDirectionAndPositionAfterGivenFlags(TestCustomLineSourceDir, TestCustomLineSourcePos, TestCustomLineSourcePolAzi, [1,2,3], AngPair, Flags);

%Isotropic Line Source
%Flat
% TestIsotropicLineSourcePos = Func_GetPositionInALineRandomFlat([0,0,0], L, RN1);
% TestIsotropicLineSourceDir = Func_GetDirectionForIsotropicDistributionRandom(RN2, RN3);
%Gaussian
TestIsotropicLineSourcePos = Func_GetPositionInALineRandomGaussian([0,0,0], L, BDFWHM, RN1, RN2);
TestIsotropicLineSourceDir = Func_GetDirectionForIsotropicDistributionRandom(RN3, RN4);
TestIsotropicLineSourcePolAzi = Func_GetPolarAzimuthalPairFromDirection([1, 0, 0]);
Flags = [true, true, true];
[TestIsotropicLineSource_U, TestIsotropicLineSource_V] = Func_UpdateDirectionAndPositionAfterGivenFlags(TestIsotropicLineSourceDir, TestIsotropicLineSourcePos, TestIsotropicLineSourcePolAzi, [1,2,3], AngPair, Flags);

