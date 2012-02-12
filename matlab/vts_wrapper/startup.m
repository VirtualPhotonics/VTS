function [ output_args ] = startup( input_args )

v = genpath(getFullPath('vts'));
addpath(v);

t = genpath(getFullPath('vts_tests'));
addpath(t);

loadAssemblies();

% Lisa, let's talk about this...don't know if it's an issue w/o more unit
% tests to confirm our behavior is what we want
warning off MATLAB:class:cannotUpdateClass:Change; 

% addpath hankel_transforms
% addpath('gridlayout');
% addpath('gridlayout\Layout');
% addpath('gridlayout\Parse');

set(0,'DefaultAxesFontName','Times New Roman')
set(0,'DefaultAxesFontSize',18)
set(0,'DefaultAxesFontWeight','bold')
set(0,'DefaultAxesLineWidth',2)
set(0,'DefaultLineLineWidth',2)
set(0,'DefaultAxesColor','white')
set(0,'DefaultLineMarkerSize',2)
set(0,'DefaultFigureColor','white')
% 
% Defaults: 
% FontName = Helvetica
% FontSize = [10]
% FontWeight = normal
% LineWidth = [0.5]
% 
% ALim = [0.1 10]
% 	ALimMode = auto
% 	AmbientLightColor = [1 1 1]
% 	Box = off
% 	CameraPosition = [35 35 9.16025]
% 	CameraPositionMode = auto
% 	CameraTarget = [35 35 0.5]
% 	CameraTargetMode = auto
% 	CameraUpVector = [0 1 0]
% 	CameraUpVectorMode = auto
% 	CameraViewAngle = [6.60861]
% 	CameraViewAngleMode = auto
% 	CLim = [3.61232 31.2923]
% 	CLimMode = auto
% 	Color = [1 1 1]
% 	CurrentPoint = [ (2 by 3) double array]
% 	ColorOrder = [ (7 by 3) double array]
% 	DataAspectRatio = [70 70 1]
% 	DataAspectRatioMode = auto
% 	DrawMode = normal
% 	FontAngle = normal
% 	FontName = Helvetica
% 	FontSize = [10]
% 	FontUnits = points
% 	FontWeight = normal
% 	GridLineStyle = :
% 	Layer = bottom
% 	LineStyleOrder = -
% 	LineWidth = [0.5]
% 	MinorGridLineStyle = :
% 	NextPlot = replace
% 	PlotBoxAspectRatio = [1 1 1]
% 	PlotBoxAspectRatioMode = auto
% 	Projection = orthographic
% 	Position = [0.13 0.11 0.775 0.815]
% 	TickLength = [0.01 0.025]
% 	TickDir = in
% 	TickDirMode = auto
% 	Title = [102]
% 	Units = normalized
% 	View = [0 90]
% 	XColor = [0 0 0]
% 	XDir = normal
% 	XGrid = off
% 	XLabel = [103]
% 	XAxisLocation = bottom
% 	XLim = [0 70]
% 	XLimMode = auto
% 	XMinorGrid = off
% 	XMinorTick = off
% 	XScale = linear
% 	XTick = [ (1 by 8) double array]
% 	XTickLabel = 
% 		0 
% 		10
% 		20
% 		30
% 		40
% 		50
% 		60
% 		70
% 	XTickLabelMode = auto
% 	XTickMode = auto
% 	YColor = [0 0 0]
% 	YDir = normal
% 	YGrid = off
% 	YLabel = [104]
% 	YAxisLocation = left
% 	YLim = [0 70]
% 	YLimMode = auto
% 	YMinorGrid = off
% 	YMinorTick = off
% 	YScale = linear
% 	YTick = [ (1 by 8) double array]
% 	YTickLabel = 
% 		0 
% 		10
% 		20
% 		30
% 		40
% 		50
% 		60
% 		70
% 	YTickLabelMode = auto
% 	YTickMode = auto
% 	ZColor = [0 0 0]
% 	ZDir = normal
% 	ZGrid = off
% 	ZLabel = [105]
% 	ZLim = [0 1]
% 	ZLimMode = auto
% 	ZMinorGrid = off
% 	ZMinorTick = off
% 	ZScale = linear
% 	ZTick = [0 0.5 1]
% 	ZTickLabel = 
% 	ZTickLabelMode = auto
% 	ZTickMode = auto
% 
% 	BeingDeleted = off
% 	ButtonDownFcn = 
% 	Children = [3.00098]
% 	Clipping = on
% 	CreateFcn = 
% 	DeleteFcn = 
% 	BusyAction = queue
% 	HandleVisibility = on
% 	HitTest = on
% 	Interruptible = on
% 	Parent = [1]
% 	Selected = off
% 	SelectionHighlight = on
% 	Tag = 
% 	Type = axes
% 	UIContextMenu = []
% 	UserData = []
% 	Visible = on