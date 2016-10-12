classdef VtsSolvers
    %%  Definitions for the main solvers in the VTS
    %   For more information, see <a href="matlab:doc vtssolvers">VtsSolvers</a>.
    %
    %       see also ROfRho,
    %       ROfFx,
    %       ROfFxAndT,
    %       ROfRhoAndT,
    %       ROfRhoAndFt,
    %       ROfFxAndFt,
    %       FluenceOfRhoAndZ,
    %       PHDOfRhoAndZ,
    %       AbsorbedEnergyOfRhoAndZ
    %       ROfRho for 2-layer tissue
    
    properties (Constant, GetAccess='private')
        Assemblies = loadAssemblies();
        Options = SolverOptions(); % only instantiate this class once
    end
    methods (Static)
        function solverType = GetSolverType()
            solverType = VtsSolvers.Options.SolverType;
        end
        function SetSolverType(solverType)
            o = VtsSolvers.Options; % need to get the reference first
            o.SolverType = solverType; % this HAS to be on a separate line
        end
        function r = ROfRho(op, rho)
            %% ROfRho
            %   ROfRho(OP, RHO) returns the steady-state spatially-resolved
            %   reflectance
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            
            nop = size(op,1);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            r = reshape(double(fs.ROfRho(op_net,rho)),[length(rho) nop]);
        end
        function r = ROfFx(op, fx)
            %% ROfFx
            %   ROfFx(OP, FX) returns the steady-state reflectance in the
            %   spatial frequency domain
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   FX is an 1 x M array of spatial frequencies (in 1/mm)
            %       eg. FX = linspace(0,0.5,11);
            
            nop = size(op,1);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            r = reshape(double(fs.ROfFx(op_net,fx)),[length(fx) nop]);
        end
        
        function r = ROfFxAndT(op, fx, t)
            %% ROfFxAndT
            %   ROfFxAndT(OP, FX, T)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   FX is an 1 x M array of spatial frequencies (in 1/mm)
            %       eg. FX = linspace(0,0.5,11);
            %   T is an 1 x O array of times (in ns)
            %       eg. T = [1:10];
            
            nop = size(op,1);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            r = reshape(double(fs.ROfFxAndTime(op_net,fx,t)),[length(t) length(fx) nop]);
        end
        
        function r = ROfRhoAndT(op, rho, t)
            %% ROfRhoAndT
            %   ROfRhoAndT(OP, RHO, T) returns time-resolved reflectance at
            %   specified detector locations
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   T is an 1 x O array of times (in ns)
            %       eg. T = [1:10];
            
            nop = size(op,1);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties(op(i,1), op(i,2), op(i,3), op(i,4)); % call the constructor with the 4 values
            end;
            
            r = reshape(double(fs.ROfRhoAndTime(op_net,rho,t)),[length(t) length(rho) nop]);
        end
        
        function r = ROfRhoAndFt(op, rho, ft)
            %% ROfRhoAndFt
            %   ROfRhoAndFt(OP, RHO, FT)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   FT is an 1 x M array of modulation frequencies (in GHz)
            %       eg. FT = [0:0.01:0.5];
            
            nop = size(op,1);
            nrho = length(rho);
            nft = length(ft);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties(op(i,1), op(i,2), op(i,3), op(i,4)); % call the constructor with the 4 values
            end
            
            % call the solver, which returns an array of (.NET) Complex structs
            rComplexNET = fs.ROfRhoAndFt(op_net, rho, ft);
            
            % create a native Matlab
            rReal = zeros([nft nrho nop]);
            rImag = zeros([nft nrho nop]);
            ci = 1;
            for i=1:nop
                for j=1:nrho
                    for k=1:nft
                        rReal(k,j,i) = rComplexNET(ci).Real;
                        rImag(k,j,i) = rComplexNET(ci).Imaginary;
                        ci=ci+1;
                    end
                end
            end
            r = complex(rReal, rImag);
        end
        
        function r = ROfFxAndFt(op, fx, ft)
            %% ROfFxAndFt
            %   ROfFxAndFt(OP, FX, FT)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   FX is an 1 x M array of spatial frequencies (in 1/mm)
            %       eg. FX = linspace(0,0.5,11);
            %   FT is an 1 x M array of modulation frequencies (in GHz)
            %       eg. FT = [0:0.01:0.5];
            
            nop = size(op,1);
            nfx = length(fx);
            nft = length(ft);
            
            fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            % fs =  Vts.Modeling.ForwardSolvers.NurbsForwardSolver;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties(op(i,1), op(i,2), op(i,3), op(i,4)); % call the constructor with the 4 values
            end
            
            % call the solver, which returns an array of (.NET) Complex structs
            rComplexNET = fs.ROfFxAndFt(op_net, fx, ft);
            
            % create a native Matlab
            rReal = zeros([nft nfx nop]);
            rImag = zeros([nft nfx nop]);
            ci = 1;
            for i=1:nop
                for j=1:nfx
                    for k=1:nft
                        rReal(k,j,i) = rComplexNET(ci).Real;
                        rImag(k,j,i) = rComplexNET(ci).Imaginary;
                        ci=ci+1;
                    end
                end
            end
            r = complex(rReal, rImag);
        end
        
        function r = FluenceOfRhoAndZ(op, rhos, zs)
            %% FluenceOfRhoAndZ
            %   FluenceOfRhoAndZ(OP, RHOS, ZS)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   Z is a 1 x M array of z values (in mm)
            %       eg. Z = linspace(0.1,19.9,100);
            
            nop = size(op,1);
            
            if strfind(VtsSolvers.Options.SolverType, 'SDA')
                fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            else
                fs =  Vts.Modeling.ForwardSolvers.PointSourceSDAForwardSolver();
            end
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            r = reshape(double(fs.FluenceOfRhoAndZ(op_net,rhos,zs)),[length(zs) length(rhos) nop]);
        end
        
        function r = FluenceOfRhoAndZAndFt(op, rhos, zs, fts)
            %% FluenceOfRhoAndZ
            %   FluenceOfRhoAndZ(OP, RHOS, ZS)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   Z is a 1 x M array of z values (in mm)
            %       eg. Z = linspace(0.1,19.9,100);
            
            nop = size(op,1);
            nrho = length(rhos);
            nft = length(fts);
            nz = length(zs);
            
            if strfind(VtsSolvers.Options.SolverType, 'SDA')
                fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            else
                fs =  Vts.Modeling.ForwardSolvers.PointSourceSDAForwardSolver();
            end
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            % call the solver, which returns an array of (.NET) Complex structs
            fComplexNET = fs.FluenceOfRhoAndZAndFt(op_net,rhos,zs,fts);%,[length(fts) length(zs) length(rhos) nop]);
            
            % create a native Matlab array
            fReal = zeros([nft nz nrho nop]);
            fImag = zeros([nft nz nrho nop]);
            ci = 1;
            for i=1:nop
                for j=1:nrho
                    for k=1:nz
                        for el=1:nft
                            fReal(el,k,j,i) = fComplexNET(ci).Real;
                            fImag(el,k,j,i) = fComplexNET(ci).Imaginary;
                            ci=ci+1;
                        end
                    end
                end
            end
            r = complex(fReal, fImag);
        end
        
        function r = PHDOfRhoAndZ(op, rhos, zs, sd)
            %% PHDOfRhoAndZ
            %   PHDOfRhoAndZ(OP, RHOS, ZS, SD)
            %
            %   OP is an N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   Z is a 1 x M array of z values (in mm)
            %       eg. Z = linspace(0.1,19.9,100);
            %   SD is the source-detector separation in mm
            
            nop = size(op,1);
            
            if strfind(VtsSolvers.Options.SolverType, 'SDA')
                fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            else
                fs =  Vts.Modeling.ForwardSolvers.PointSourceSDAForwardSolver();
            end
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            fluence = double(fs.FluenceOfRhoAndZ(op_net,rhos,zs));
            %phd = Vts.Factories.ComputationFactory.GetPHD(fs, fluence, sd, op_net, rhos, zs);
            phd = Vts.Factories.ComputationFactory.GetPHD(fs, NET.convertArray(fluence,'System.Double'),...
                sd, op_net, NET.convertArray(rhos,'System.Double'), NET.convertArray(zs,'System.Double'));
            
            %r = double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},phd));
            r = reshape(double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},phd)),[length(zs) length(rhos) nop]);
        end
        
        function r = PHDOfRhoAndZTwoLayer(op, rhos, zs, sd, layerThickness)
            %% PHDOfRhoAndZTwoLayer
            %   PHDOfRhoAndZ(OP, RHOS, ZS, SD, LAYERTHICKNESS) returns the
            %   Photon Hitting Density in cylindrical coordinates for a two
            %   layer tissue with specified source-detector separation and top layer thickness
            %
            %   OP is an array of N x 4 matrix of optical properties
            %       eg. OP = [[mua1, mus'1, g1, n1]; [mua2, mus'2, g2, n2]; ...];
            %   RHO is a 1 x M array of x values (in mm)
            %       eg. RHO = linspace(0,10,10);
            %   Z is a 1 x M array of z values (in mm)
            %       eg. Z = linspace(0.1,19.9,100);
            %   SD is the source-detector separation in mm
            %   LAYERTHICKNESS is the thickness of the top layer of tissue
            
            nop = size(op,1);
            
            fs =  Vts.Modeling.ForwardSolvers.TwoLayerSDAForwardSolver();
            solutionDomain = Vts.FluenceSolutionDomainType.FluenceOfRhoAndZ;
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
                      
            regions = NET.createArray('Vts.IOpticalPropertyRegion', 2);
            regions(1) = Vts.Common.LayerOpticalPropertyRegion(Vts.Common.DoubleRange(0, layerThickness), op_net(1));
            regions(2) = Vts.Common.LayerOpticalPropertyRegion(Vts.Common.DoubleRange(layerThickness, Inf), op_net(2));
         
            independentAxes = NET.createArray('Vts.IndependentVariableAxis', 2);
            independentAxes(1) = Vts.IndependentVariableAxis.Rho;
            independentAxes(2) = Vts.IndependentVariableAxis.Z;
            independentValues = NET.createArray('System.Double[]', 2);
            independentValues(1) = rhos;
            independentValues(2) = zs;
            constantValues = NET.createArray('System.Double', 0);
                    
            fluence = double(Vts.Factories.ComputationFactory.ComputeFluence(fs, solutionDomain, independentAxes, independentValues, regions, constantValues));
            %fluence = double(fs.FluenceOfRhoAndZ(op_net,rhos,zs));

            phd = Vts.Factories.ComputationFactory.GetPHD(fs, NET.convertArray(fluence,'System.Double'),...
                sd, op_net, NET.convertArray(rhos,'System.Double'), NET.convertArray(zs,'System.Double'));
            
            %r = double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},phd));
            r = reshape(double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},phd)),[length(zs) length(rhos)]);
        end
        
        function r = AbsorbedEnergyOfRhoAndZ(op, rhos, zs)
            %% AbsorbedEnergyOfRhoAndZ
            %   AbsorbedEnergyOfRhoAndZ(OP, RHOS, ZS)
            %
            %   OP is an 1 x 4 matrix of optical properties
            %       eg. OP = [mua1, mus'1, g1, n1];
            %   RHO is an 1 x M array of detector locations (in mm)
            %       eg. RHO = [1:10];
            %   Z is a 1 x M array of z values (in mm)
            %       eg. Z = linspace(0.1,19.9,100);
            
            nop = size(op,1);
            
            if strfind(VtsSolvers.Options.SolverType, 'SDA')
                fs = Vts.Factories.SolverFactory.GetForwardSolver(VtsSolvers.Options.SolverType);
            else
                fs =  Vts.Modeling.ForwardSolvers.PointSourceSDAForwardSolver();
            end
            
            op_net = NET.createArray('Vts.OpticalProperties', nop);
            
            for i=1:nop
                op_net(i) = Vts.OpticalProperties;
                op_net(i).Mua =  op(i,1);
                op_net(i).Musp = op(i,2);
                op_net(i).G =    op(i,3);
                op_net(i).N =    op(i,4);
            end;
            
            fluence = double(fs.FluenceOfRhoAndZ(op_net,rhos,zs));
            ae = Vts.Factories.ComputationFactory.GetAbsorbedEnergy(NET.convertArray(fluence,'System.Double'),...
                op_net(1).Mua);
            
            %r = double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},phd));
            r = reshape(double(NET.invokeGenericMethod('System.Linq.Enumerable','ToArray',{'System.Double'},ae)),[length(zs) length(rhos) nop]);
        end
        function r = ROfRhoTwoLayer(op, layerThickness, rho)
            %% ROfRhoTwoLayer
            %   ROfRhoTwoLayer(OP, LAYERTHICKNESS, RHO) returns the steady-state spatially-resolved
            %   reflectance for a two layer tissue with specified top layer thickness
            %
            %   OP is an N x 2 x 4 matrix of optical properties
            %       eg. OP = [[mua11, mus'11, g11, n11] [mua12, mus'12, g12, n12]; ... % layer 1 & layer 2 for system 1
            %                 [mua21, mus'21, g21, n21] [mua22, mus'22, g22, n22]; ... % layer 1 & layer 2 for system 2
            %                 ];
            %   LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
            %   RHO is an 1 x M array of detector locations (in mm) eg. RHO = [1:10];
            
            nop = size(op,1);
            
            fs =  Vts.Modeling.ForwardSolvers.TwoLayerSDAForwardSolver;
            
            nLayers = size(op,2);
            r = zeros([nop length(rho)]);
            
            for i=1:nop
                opArray_net = NET.createArray('Vts.IOpticalPropertyRegion', nLayers);
                for j=1:nLayers                    
                    zRangeNET = Vts.Common.DoubleRange(0,layerThickness);
                    opNET = Vts.OpticalProperties(op(i,j,1), op(i,j,2), op(i,j,3), op(i,j,4));
                    tempOpRegion = Vts.Common.LayerOpticalPropertyRegion(zRangeNET, opNET);                    
                    opArray_net(j) = tempOpRegion;
                end                
                reflectanceAtRho = fs.ROfRho(opArray_net, rho);
                r(i,:) = reflectanceAtRho;
            end;
        end
        
        function r = ROfRhoAndTimeTwoLayer(op, layerThickness, rho, time)
            %% ROfRhoAndTimeTwoLayer
            %   ROfRhoAndTimeTwoLayer(OP, LAYERTHICKNESS, RHO, TIME) returns the steady-state spatially-resolved
            %   reflectance for a two layer tissue with top layer thickness
            %
            %   OP is an N x 2 x 4 matrix of optical properties
            %       eg. OP = [[mua11, mus'11, g11, n11] [mua12, mus'12, g12, n12]; ... % layer 1 & layer 2 for system 1
            %                 [mua21, mus'21, g21, n21] [mua22, mus'22, g22, n22]; ... % layer 1 & layer 2 for system 2
            %                 ];
            %   LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
            %   RHO is an 1 x M array of detector locations (in mm) eg. RHO = [1:10];
            %   T is an 1 x P array of times (in ns) eg. T = [1:10]
            
            nop = size(op,1);
            
            fs =  Vts.Modeling.ForwardSolvers.TwoLayerSDAForwardSolver;
            
            nLayers = size(op,2);
            r = zeros([nop length(rho) length(time)]);
            
            for i=1:nop
                opArray_net = NET.createArray('Vts.IOpticalPropertyRegion', nLayers);
                for j=1:nLayers                    
                    zRangeNET = Vts.Common.DoubleRange(0,layerThickness);
                    opNET = Vts.OpticalProperties(op(i,j,1), op(i,j,2), op(i,j,3), op(i,j,4));
                    tempOpRegion = Vts.Common.LayerOpticalPropertyRegion(zRangeNET, opNET);                    
                    opArray_net(j) = tempOpRegion;
                end                
                reflectanceAtRhoAndTime = fs.ROfRhoAndTime(opArray_net, rho, time);
                r(i,:,:) = reflectanceAtRhoAndTime;
            end;
        end
                
        function r = ROfRhoAndFtTwoLayer(op, layerThickness, rho, ft)
            %% ROfRhoAndFtTwoLayer
            %   ROfRhoAndFtTwoLayer(OP, LAYERTHICKNESS, RHO, FT) returns the 
            %   reflectance for a two layer tissue with specified top layer thickness
            %
            %   OP is an N x 2 x 4 matrix of optical properties
            %       eg. OP = [[mua11, mus'11, g11, n11] [mua12, mus'12, g12, n12]; ... % layer 1 & layer 2 for system 1
            %                 [mua21, mus'21, g21, n21] [mua22, mus'22, g22, n22]; ... % layer 1 & layer 2 for system 2
            %                 ];
            %   LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
            %   RHO is an 1 x M array of detector locations (in mm) eg. RHO = [1:10];
            %   FT is an 1 x P array of modulation frequencies (in GHz) eg. FT = [0:0.01:0.5;]
            
            nop = size(op,1);
            nrho = length(rho);
            nft = length(ft);
            
            fs =  Vts.Modeling.ForwardSolvers.TwoLayerSDAForwardSolver;
            
            nLayers = size(op,2);
            r = zeros([nop length(rho) length(ft)]);
            
            for i=1:nop
                opArray_net = NET.createArray('Vts.IOpticalPropertyRegion', nLayers);
                for j=1:nLayers                    
                    zRangeNET = Vts.Common.DoubleRange(0,layerThickness);
                    opNET = Vts.OpticalProperties(op(i,j,1), op(i,j,2), op(i,j,3), op(i,j,4));
                    tempOpRegion = Vts.Common.LayerOpticalPropertyRegion(zRangeNET, opNET);                    
                    opArray_net(j) = tempOpRegion;
                end      
                % call the solver, which returns an array of (.NET) Complex
                rComplexNET = fs.ROfRhoAndFt(opArray_net, rho, ft);
                rReal = zeros([nft nrho]);
                rImag = zeros([nft nrho]);
                ci = 1;
                for j=1:nrho
                    for k=1:nft
                        rReal(k,j) = rComplexNET(ci).Real;
                        rImag(k,j) = rComplexNET(ci).Imaginary;
                        ci=ci+1;
                    end
                end
                r(i,:,:) = complex(rReal, rImag);
            end;
        end
        
        function r = ROfFxTwoLayer(op, layerThickness, fx)
            %% ROfFxTwoLayer
            %   ROfFxTwoLayer(OP, LAYERTHICKNESS, FX) returns the steady-state spatial-frequesncy resolved
            %   reflectance for a two layer tissue with specified top layer thickness
            %
            %   OP is an N x 2 x 4 matrix of optical properties
            %       eg. OP = [[mua11, mus'11, g11, n11] [mua12, mus'12, g12, n12]; ... % layer 1 & layer 2 for system 1
            %                 [mua21, mus'21, g21, n21] [mua22, mus'22, g22, n22]; ... % layer 1 & layer 2 for system 2
            %                 ];
            %   LAYERTHICKNESS is the tissue top layer thickness.  Needs to be > lstar = 1/(mua+mus')
            %   FX is an 1 x M array of frequencies (in mm) eg. FX = [1:10];
            
            nop = size(op,1);
            
            fs =  Vts.Modeling.ForwardSolvers.TwoLayerSDAForwardSolver;
            
            nLayers = size(op,2);
            r = zeros([nop length(fx)]);
            
            for i=1:nop
                opArray_net = NET.createArray('Vts.IOpticalPropertyRegion', nLayers);
                for j=1:nLayers                    
                    zRangeNET = Vts.Common.DoubleRange(0,layerThickness);
                    opNET = Vts.OpticalProperties(op(i,j,1), op(i,j,2), op(i,j,3), op(i,j,4));
                    tempOpRegion = Vts.Common.LayerOpticalPropertyRegion(zRangeNET, opNET);                    
                    opArray_net(j) = tempOpRegion;
                end                
                reflectanceAtFx = fs.ROfFx(opArray_net, fx);
                r(i,:) = reflectanceAtFx;
            end;
        end
        
    end
end