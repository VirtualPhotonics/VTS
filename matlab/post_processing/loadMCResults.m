function [results, N] = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

json = readAndParseJson([datadir slash dataname '.txt']);
N = json.N;

postProcessorResults = false;
if (exist([datadir slash dataname '_database_infile.txt'],'file'))
    postProcessorResults = true;
    databaseInputJson = readAndParseJson([datadir slash dataname '_database_infile.txt']);
    json.N = databaseInputJson.N;  % set this in case MCPP has mix of MC and pMC detectors
end
numDetectors = length(json.DetectorInputs);
for di = 1:numDetectors
    detector = json.DetectorInputs{di};
    switch(detector.TallyType)
        case 'SurfaceFiber'
            SurfaceFiber.Name = detector.Name;
            data = readAndParseJson([datadir slash detector.Name '.txt']);
            SurfaceFiber.Mean = data.Mean;
            SurfaceFiber.SecondMoment = data.SecondMoment;
            SurfaceFiber.Stdev = sqrt((SurfaceFiber.SecondMoment - SurfaceFiber.Mean.^2) / json.N);
            results{di}.SurfaceFiber = SurfaceFiber;
        case 'SlantedRecessedFiber'
            SlantedRecessedFiber.Name = detector.Name;
            data = readAndParseJson([datadir slash detector.Name '.txt']);
            SlantedRecessedFiber.Mean = data.Mean;
            SlantedRecessedFiber.SecondMoment = data.SecondMoment;
            SlantedRecessedFiber.Stdev = sqrt( ...
                (data.SecondMoment - data.Mean.^2) / json.N);
            results{di}.SlantedRecessedFiber = SlantedRecessedFiber;
        case 'RDiffuse'
            RDiffuse.Name = detector.Name;
            data = readAndParseJson([datadir slash detector.Name '.txt']);
            RDiffuse.Mean = data.Mean;
            RDiffuse.SecondMoment = data.SecondMoment;
            RDiffuse.Stdev = sqrt((data.SecondMoment - data.Mean.^2) / json.N);
            results{di}.RDiffuse = RDiffuse;
        case 'RSpecular'
            RSpecular.Name = detector.Name;
            data = readAndParseJson([datadir slash detector.Name '.txt']);
            RSpecular.Mean = data.Mean;
            RSpecular.SecondMoment = data.SecondMoment;
            RSpecular.Stdev = sqrt((RSpecular.SecondMoment - RSpecular.Mean.^2) / json.N);
            results{di}.RSpecular = RSpecular;
        case 'ROfRho'
            ROfRho.Name = detector.Name;
            tempRho = detector.Rho;
            ROfRho.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            ROfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + ROfRho.Rho(2:end)) / 2;
            ROfRho.Mean = readBinaryData( ...
                [datadir slash detector.Name], numel(ROfRho.Rho) - 1);
            secondMomentFile = [datadir slash detector.Name '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRho.SecondMoment = readBinaryData( ...
                    secondMomentFile, numel(ROfRho.Rho) - 1);
                ROfRho.Stdev = sqrt( ...
                    (ROfRho.SecondMoment - ROfRho.Mean.^2) / json.N);
            end
            results{di}.ROfRho = ROfRho;
        case 'ROfRhoRecessed'
            ROfRhoRecessed.Name = detector.Name;
            rho = detector.Rho;
            ROfRhoRecessed.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoRecessed.Rho_Midpoints =  (ROfRhoRecessed.Rho(1:end-1) + ROfRhoRecessed.Rho(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            ROfRhoRecessed.Mean = readBinaryData( ...
                dataFile, numel(ROfRhoRecessed.Rho) - 1);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoRecessed.SecondMoment = readBinaryData( ...
                    secondMomentFile, numel(ROfRhoRecessed.Rho) - 1);
                ROfRhoRecessed.Stdev = sqrt( ...
                    (ROfRhoRecessed.SecondMoment - ...
                    ROfRhoRecessed.Mean.^2) / json.N);
            end
            results{di}.ROfRhoRecessed = ROfRhoRecessed;
        case 'ROfAngle'
            ROfAngle.Name = detector.Name;
            ang = detector.Angle; % shortened so no confusion with Builtin function 'angle'
            ROfAngle.Angle = linspace(ang.Start, ang.Stop, ang.Count);
            ROfAngle.Angle_Midpoints = ...
                (ROfAngle.Angle(1:end-1) + ROfAngle.Angle(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            ROfAngle.Mean = readBinaryData( dataFile, numel(ROfAngle.Angle) - 1);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfAngle.SecondMoment = readBinaryData(secondMomentFile, numel(ROfAngle.Angle) - 1);
                ROfAngle.Stdev = sqrt((ROfAngle.SecondMoment - ROfAngle.Mean.^2) / json.N);
            end
            results{di}.ROfAngle = ROfAngle;
        case 'ROfRhoAndTime'
            ROfRhoAndTime.Name = detector.Name;
            rho = detector.Rho;
            time = detector.Time;
            ROfRhoAndTime.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndTime.Time = linspace(time.Start, time.Stop, time.Count);
            ROfRhoAndTime.Rho_Midpoints = (ROfRhoAndTime.Rho(1:end-1) + ROfRhoAndTime.Rho(2:end)) / 2;
            ROfRhoAndTime.Time_Midpoints = (ROfRhoAndTime.Time(1:end-1) + ROfRhoAndTime.Time(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfRhoAndTime.Time) - 1, numel(ROfRhoAndTime.Rho) - 1];
            ROfRhoAndTime.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoAndTime.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfRhoAndTime.Stdev = sqrt( ...
                    (ROfRhoAndTime.SecondMoment - ...
                    ROfRhoAndTime.Mean.^2) / json.N);
            end
            results{di}.ROfRhoAndTime = ROfRhoAndTime;
        case 'ROfRhoAndTimeAndMaxDepth'
            ROfRhoAndTimeAndMaxDepth.Name = detector.Name;
            rho = detector.Rho;
            time = detector.Time;
            maxDepth = detector.MaxDepth;
            ROfRhoAndTimeAndMaxDepth.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndTimeAndMaxDepth.Time = linspace(time.Start, time.Stop, time.Count);
            ROfRhoAndTimeAndMaxDepth.MaxDepth = linspace(maxDepth.Start, maxDepth.Stop, maxDepth.Count);
            ROfRhoAndTimeAndMaxDepth.Rho_Midpoints = (ROfRhoAndTimeAndMaxDepth.Rho(1:end-1) +  ROfRhoAndTimeAndMaxDepth.Rho(2:end)) / 2;
            ROfRhoAndTimeAndMaxDepth.Time_Midpoints = (ROfRhoAndTimeAndMaxDepth.Time(1:end-1) + ROfRhoAndTimeAndMaxDepth.Time(2:end)) / 2;
            ROfRhoAndTimeAndMaxDepth.MaxDepth_Midpoints = (ROfRhoAndTimeAndMaxDepth.MaxDepth(1:end-1) + ROfRhoAndTimeAndMaxDepth.MaxDepth(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfRhoAndTimeAndMaxDepth.MaxDepth) - 1, ...
                        numel(ROfRhoAndTimeAndMaxDepth.Time) - 1, ...
                        numel(ROfRhoAndTimeAndMaxDepth.Rho) - 1];
            dataCount = prod(dataSize);
            ROfRhoAndTimeAndMaxDepth.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoAndTimeAndMaxDepth.SecondMoment = reshape( ...
                    readBinaryData(dataFile, prod(dataSize)), dataSize);
                ROfRhoAndTimeAndMaxDepth.Stdev = sqrt( ...
                    (ROfRhoAndTimeAndMaxDepth.SecondMoment - ...
                    ROfRhoAndTimeAndMaxDepth.Mean.^2) / json.N);
            end
            results{di}.ROfRhoAndTimeAndMaxDepth = ROfRhoAndTimeAndMaxDepth;
        case 'ROfRhoAndMaxDepth'
            ROfRhoAndMaxDepth.Name = detector.Name;
            rho = detector.Rho;
            maxDepth = detector.MaxDepth;
            ROfRhoAndMaxDepth.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndMaxDepth.MaxDepth = linspace(maxDepth.Start, maxDepth.Stop, maxDepth.Count);
            ROfRhoAndMaxDepth.Rho_Midpoints =  (ROfRhoAndMaxDepth.Rho(1:end-1) + ROfRhoAndMaxDepth.Rho(2:end)) / 2;
            ROfRhoAndMaxDepth.MaxDepth_Midpoints = (ROfRhoAndMaxDepth.MaxDepth(1:end-1) + ROfRhoAndMaxDepth.MaxDepth(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfRhoAndMaxDepth.MaxDepth) - 1, ...
                        numel(ROfRhoAndMaxDepth.Rho) - 1];
            ROfRhoAndMaxDepth.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoAndMaxDepth.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfRhoAndMaxDepth.Stdev = sqrt( ...
                    (ROfRhoAndMaxDepth.SecondMoment - ...
                    ROfRhoAndMaxDepth.Mean.^2) / json.N);
            end
            results{di}.ROfRhoAndMaxDepth = ROfRhoAndMaxDepth;
        case 'ROfRhoAndMaxDepthRecessed'
            ROfRhoAndMaxDepthRecessed.Name = detector.Name;
            rho = detector.Rho;
            maxDepth = detector.MaxDepth;
            ROfRhoAndMaxDepthRecessed.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndMaxDepthRecessed.MaxDepth = linspace(maxDepth.Start, maxDepth.Stop, maxDepth.Count);
            ROfRhoAndMaxDepthRecessed.Rho_Midpoints = (ROfRhoAndMaxDepthRecessed.Rho(1:end-1) + ROfRhoAndMaxDepthRecessed.Rho(2:end)) / 2;
            ROfRhoAndMaxDepthRecessed.MaxDepth_Midpoints = (ROfRhoAndMaxDepthRecessed.MaxDepth(1:end-1) + ROfRhoAndMaxDepthRecessed.MaxDepth(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfRhoAndMaxDepthRecessed.MaxDepth) - 1, ...
                        numel(ROfRhoAndMaxDepthRecessed.Rho) - 1];
            ROfRhoAndMaxDepthRecessed.Mean = readBinaryData( ...
                dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoAndMaxDepthRecessed.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfRhoAndMaxDepthRecessed.Stdev = sqrt( ...
                    (ROfRhoAndMaxDepthRecessed.SecondMoment - ...
                    ROfRhoAndMaxDepthRecessed.Mean.^2) / json.N);
            end
            results{di}.ROfRhoAndMaxDepthRecessed = ...
                ROfRhoAndMaxDepthRecessed;
        case 'ROfRhoAndAngle'
            ROfRhoAndAngle.Name = detector.Name;
            rho = detector.Rho;
            ang = detector.Angle; % shortened so that Builtin 'angle' function not confused
            ROfRhoAndAngle.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndAngle.Angle = linspace(ang.Start, ang.Stop, ang.Count);
            ROfRhoAndAngle.Rho_Midpoints = (ROfRhoAndAngle.Rho(1:end-1) + ROfRhoAndAngle.Rho(2:end)) / 2;
            ROfRhoAndAngle.Angle_Midpoints = (ROfRhoAndAngle.Angle(1:end-1) + ROfRhoAndAngle.Angle(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfRhoAndAngle.Angle) - 1, ...
                        numel(ROfRhoAndAngle.Rho) - 1];
            ROfRhoAndAngle.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfRhoAndAngle.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfRhoAndAngle.Stdev = sqrt( ...
                    (ROfRhoAndAngle.SecondMoment - ...
                    ROfRhoAndAngle.Mean.^2) / json.N);
            end
            results{di}.ROfRhoAndAngle = ROfRhoAndAngle;
        case 'ROfRhoAndOmega'
            ROfRhoAndOmega.Name = detector.Name;
            rho = detector.Rho;
            omega = detector.Omega;
            ROfRhoAndOmega.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            ROfRhoAndOmega.Omega = linspace(omega.Start, omega.Stop, omega.Count);
            ROfRhoAndOmega.Omega_Midpoints = ROfRhoAndOmega.Omega; % omega is not binned, value is used
            ROfRhoAndOmega.Rho_Midpoints = (ROfRhoAndOmega.Rho(1:end-1) + ROfRhoAndOmega.Rho(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [2 * numel(ROfRhoAndOmega.Omega), ...
                        numel(ROfRhoAndOmega.Rho) - 1];
            tempData = readBinaryData(dataFile, dataSize); % read column major json binary
            ROfRhoAndOmega.Mean = tempData(1:2:end, :) + 1i * tempData(2:2:end, :);
            ROfRhoAndOmega.Amplitude = abs(ROfRhoAndOmega.Mean);
            ROfRhoAndOmega.Phase = -angle(ROfRhoAndOmega.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                tempData = readBinaryData(secondMomentFile, dataSize); % 2x omega for re and im
                ROfRhoAndOmega.SecondMoment = tempData(1:2:end, :) + ...
                    tempData(2:2:end, :); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfRhoAndOmega.Stdev = sqrt((ROfRhoAndOmega.SecondMoment - ...
                    abs(ROfRhoAndOmega.Mean).^2) / json.N);
            end
            results{di}.ROfRhoAndOmega = ROfRhoAndOmega;
         case 'ROfXAndY'
            ROfXAndY.Name = detector.Name;
            x = detector.X;
            y = detector.Y;
            ROfXAndY.X = linspace(x.Start, x.Stop, x.Count);
            ROfXAndY.Y = linspace(y.Start, y.Stop, y.Count);
            ROfXAndY.X_Midpoints = (ROfXAndY.X(1:end-1) + ROfXAndY.X(2:end)) / 2;
            ROfXAndY.Y_Midpoints = (ROfXAndY.Y(1:end-1) + ROfXAndY.Y(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfXAndY.Y) - 1, numel(ROfXAndY.X) - 1];
            ROfXAndY.Mean = readBinaryData(dataFile, dataSize); % read column-major JSON binary
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndY.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfXAndY.Stdev = sqrt( ...
                    (ROfXAndY.SecondMoment - ROfXAndY.Mean.^2) / json.N);
            end
            results{di}.ROfXAndY = ROfXAndY;
        case 'ROfXAndYRecessed'
            ROfXAndYRecessed.Name = detector.Name;
            x = detector.X;
            y = detector.Y;
            ROfXAndYRecessed.X = linspace(x.Start, x.Stop, x.Count);
            ROfXAndYRecessed.Y = linspace(y.Start, y.Stop, y.Count);
            ROfXAndYRecessed.X_Midpoints = (ROfXAndYRecessed.X(1:end-1) + ROfXAndYRecessed.X(2:end)) / 2;
            ROfXAndYRecessed.Y_Midpoints = (ROfXAndYRecessed.Y(1:end-1) + ROfXAndYRecessed.Y(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(ROfXAndYRecessed.Y) - 1, ...
                        numel(ROfXAndYRecessed.X) - 1];
            ROfXAndYRecessed.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYRecessed.SecondMoment = readBinaryData( ...
                    secondMomentFile, dataSize);
                ROfXAndYRecessed.Stdev = sqrt( ...
                    (ROfXAndYRecessed.SecondMoment - ...
                    ROfXAndYRecessed.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYRecessed = ROfXAndYRecessed;
         case 'ROfXAndYAndTime'
            ROfXAndYAndTime.Name = detector.Name;
            x = detector.X;
            y = detector.Y;
            time = detector.Time;
            ROfXAndYAndTime.X = linspace(x.Start, x.Stop, x.Count);
            ROfXAndYAndTime.Y = linspace(y.Start, y.Stop, y.Count);
            ROfXAndYAndTime.Time = linspace(time.Start, time.Stop, time.Count);
            ROfXAndYAndTime.X_Midpoints = (ROfXAndYAndTime.X(1:end-1) + ROfXAndYAndTime.X(2:end)) / 2;
            ROfXAndYAndTime.Y_Midpoints = (ROfXAndYAndTime.Y(1:end-1) + ROfXAndYAndTime.Y(2:end)) / 2;
            ROfXAndYAndTime.Time_Midpoints = (ROfXAndYAndTime.Time(1:end-1) + ROfXAndYAndTime.Time(2:end)) / 2;
            dataSize = [numel(ROfXAndYAndTime.Time) - 1, ...
                        numel(ROfXAndYAndTime.Y) - 1, ...
                        numel(ROfXAndYAndTime.X) - 1];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndTime.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndTime.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndTime.Stdev = sqrt( ...
                    (ROfXAndYAndTime.SecondMoment - ...
                    ROfXAndYAndTime.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYAndTime = ROfXAndYAndTime;
        case 'ROfXAndYAndTimeRecessed'
            ROfXAndYAndTimeRecessed.Name = detector.Name;
            x = detector.X;
            y = detector.Y;
            time = detector.Time;
            ROfXAndYAndTimeRecessed.X = linspace(x.Start, x.Stop, x.Count);
            ROfXAndYAndTimeRecessed.Y = linspace(y.Start, y.Stop, y.Count);
            ROfXAndYAndTimeRecessed.Time = linspace( ...
                time.Start, time.Stop, time.Count);
            ROfXAndYAndTimeRecessed.X_Midpoints = (ROfXAndYAndTimeRecessed.X(1:end-1) + ROfXAndYAndTimeRecessed.X(2:end)) / 2;
            ROfXAndYAndTimeRecessed.Y_Midpoints = (ROfXAndYAndTimeRecessed.Y(1:end-1) + ROfXAndYAndTimeRecessed.Y(2:end)) / 2;
            ROfXAndYAndTimeRecessed.Time_Midpoints = (ROfXAndYAndTimeRecessed.Time(1:end-1) + ROfXAndYAndTimeRecessed.Time(2:end)) / 2;
            dataSize = [numel(ROfXAndYAndTimeRecessed.Time) - 1, ...
                        numel(ROfXAndYAndTimeRecessed.Y) - 1, ...
                        numel(ROfXAndYAndTimeRecessed.X) - 1];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndTimeRecessed.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndTimeRecessed.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndTimeRecessed.Stdev = sqrt( ...
                    (ROfXAndYAndTimeRecessed.SecondMoment - ...
                    ROfXAndYAndTimeRecessed.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYAndTimeRecessed = ROfXAndYAndTimeRecessed;
        case 'ROfXAndYAndTimeAndSubregion'
            ROfXAndYAndTimeAndSubregion.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            nRegions = numel(json.TissueInput.Regions);
            ROfXAndYAndTimeAndSubregion.X = linspace(tempX.Start, tempX.Stop, tempX.Count);
            ROfXAndYAndTimeAndSubregion.Y = linspace(tempY.Start, tempY.Stop, tempY.Count);
            ROfXAndYAndTimeAndSubregion.Time = linspace(tempTime.Start, tempTime.Stop, tempTime.Count);
            ROfXAndYAndTimeAndSubregion.X_Midpoints = (ROfXAndYAndTimeAndSubregion.X(1:end-1) + ROfXAndYAndTimeAndSubregion.X(2:end)) / 2;
            ROfXAndYAndTimeAndSubregion.Y_Midpoints = (ROfXAndYAndTimeAndSubregion.Y(1:end-1) + ROfXAndYAndTimeAndSubregion.Y(2:end)) / 2;
            ROfXAndYAndTimeAndSubregion.Time_Midpoints = (ROfXAndYAndTimeAndSubregion.Time(1:end-1) + ROfXAndYAndTimeAndSubregion.Time(2:end)) / 2;
            nX = tempX.Count - 1;
            nY = tempY.Count - 1;
            nTime = tempTime.Count - 1;
            dataSize = [nRegions, nTime, nY, nX];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndTimeAndSubregion.NumberOfRegions = nRegions;
            ROfXAndYAndTimeAndSubregion.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            ROfXAndYAndTimeAndSubregion.ROfXAndY = readBinaryData( ...
                [dataFile '_ROfXAndY'], [nY, nX]);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndTimeAndSubregion.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndTimeAndSubregion.Stdev = sqrt( ...
                    (ROfXAndYAndTimeAndSubregion.SecondMoment - ...
                    ROfXAndYAndTimeAndSubregion.Mean.^2) / json.N);
                xySecondMomentFile = [dataFile '_ROfXAndY_2'];
                ROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment = ...
                    readBinaryData(xySecondMomentFile, [nY, nX]);
                ROfXAndYAndTimeAndSubregion.ROfXAndYStdev = sqrt( ...
                    (ROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment - ...
                    ROfXAndYAndTimeAndSubregion.ROfXAndY.^2) / json.N);
            end
            results{di}.ROfXAndYAndTimeAndSubregion = ROfXAndYAndTimeAndSubregion;
        case 'ROfXAndYAndTimeAndSubregionRecessed'
            ROfXAndYAndTimeAndSubregionRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            nRegions = numel(json.TissueInput.Regions);
            nX = tempX.Count - 1;
            nY = tempY.Count - 1;
            nTime = tempTime.Count - 1;
            ROfXAndYAndTimeAndSubregionRecessed.X = linspace(tempX.Start, tempX.Stop, tempX.Count);
            ROfXAndYAndTimeAndSubregionRecessed.Y = linspace(tempY.Start, tempY.Stop, tempY.Count);
            ROfXAndYAndTimeAndSubregionRecessed.Time = linspace(tempTime.Start, tempTime.Stop, tempTime.Count);
            ROfXAndYAndTimeAndSubregionRecessed.X_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.X(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.X(2:end)) / 2;
            ROfXAndYAndTimeAndSubregionRecessed.Y_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.Y(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.Y(2:end)) / 2;
            ROfXAndYAndTimeAndSubregionRecessed.Time_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.Time(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.Time(2:end)) / 2;
            dataSize = [nRegions, nTime, nY, nX];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions = nRegions;
            ROfXAndYAndTimeAndSubregionRecessed.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY = readBinaryData( ...
                [dataFile '_ROfXAndY'], [nY, nX]);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndTimeAndSubregionRecessed.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndTimeAndSubregionRecessed.Stdev = sqrt( ...
                    (ROfXAndYAndTimeAndSubregionRecessed.SecondMoment - ...
                    ROfXAndYAndTimeAndSubregionRecessed.Mean.^2) / json.N);
                xySecondMomentFile = [dataFile '_ROfXAndY_2'];
                ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment = ...
                    readBinaryData(xySecondMomentFile, [nY, nX]);
                ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYStdev = sqrt( ...
                    (ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment - ...
                    ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY.^2) / json.N);
            end
            results{di}.ROfXAndYAndTimeAndSubregionRecessed = ROfXAndYAndTimeAndSubregionRecessed;
        case 'ROfXAndYAndThetaAndPhi'
            ROfXAndYAndThetaAndPhi.Name = detector.Name;
            nX = detector.X.Count - 1;
            nY = detector.Y.Count - 1;
            nTheta = detector.Theta.Count - 1;
            nPhi = detector.Phi.Count - 1;
            ROfXAndYAndThetaAndPhi.X = linspace(detector.X.Start, detector.X.Stop, detector.X.Count);
            ROfXAndYAndThetaAndPhi.Y = linspace(detector.Y.Start, detector.Y.Stop, detector.Y.Count);
            ROfXAndYAndThetaAndPhi.Theta = linspace(detector.Theta.Start, detector.Theta.Stop, detector.Theta.Count);
            ROfXAndYAndThetaAndPhi.Phi = linspace(detector.Phi.Start, detector.Phi.Stop, detector.Phi.Count);
            ROfXAndYAndThetaAndPhi.X_Midpoints = (ROfXAndYAndThetaAndPhi.X(1:end-1) + ROfXAndYAndThetaAndPhi.X(2:end)) / 2;
            ROfXAndYAndThetaAndPhi.Y_Midpoints = (ROfXAndYAndThetaAndPhi.Y(1:end-1) + ROfXAndYAndThetaAndPhi.Y(2:end)) / 2;
            ROfXAndYAndThetaAndPhi.Theta_Midpoints = (ROfXAndYAndThetaAndPhi.Theta(1:end-1) + ROfXAndYAndThetaAndPhi.Theta(2:end)) / 2;
            ROfXAndYAndThetaAndPhi.Phi_Midpoints = (ROfXAndYAndThetaAndPhi.Phi(1:end-1) + ROfXAndYAndThetaAndPhi.Phi(2:end)) / 2;
            dataSize = [nPhi, nTheta, nY, nX];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndThetaAndPhi.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndThetaAndPhi.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndThetaAndPhi.Stdev = sqrt( ...
                    (ROfXAndYAndThetaAndPhi.SecondMoment - ...
                    ROfXAndYAndThetaAndPhi.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYAndThetaAndPhi = ROfXAndYAndThetaAndPhi;
        case 'ROfXAndYAndMaxDepth'
            ROfXAndYAndMaxDepth.Name = detector.Name;
            x = linspace(detector.X.Start, detector.X.Stop, detector.X.Count);
            y = linspace(detector.Y.Start, detector.Y.Stop, detector.Y.Count);
            maxDepth = linspace(detector.MaxDepth.Start, detector.MaxDepth.Stop, ...
                detector.MaxDepth.Count);
            ROfXAndYAndMaxDepth.X = x;
            ROfXAndYAndMaxDepth.Y = y;
            ROfXAndYAndMaxDepth.MaxDepth = maxDepth;
            ROfXAndYAndMaxDepth.X_Midpoints = (x(1:end-1) + x(2:end)) / 2;
            ROfXAndYAndMaxDepth.Y_Midpoints = (y(1:end-1) + y(2:end)) / 2;
            ROfXAndYAndMaxDepth.MaxDepth_Midpoints = movmean( ...
                maxDepth, 2, 'Endpoints', 'discard');
            dataSize = [numel(maxDepth) - 1, numel(y) - 1, numel(x) - 1];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndMaxDepth.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndMaxDepth.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndMaxDepth.Stdev = sqrt( ...
                    (ROfXAndYAndMaxDepth.SecondMoment - ...
                    ROfXAndYAndMaxDepth.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYAndMaxDepth = ROfXAndYAndMaxDepth;
        case 'ROfXAndYAndMaxDepthRecessed'
            ROfXAndYAndMaxDepthRecessed.Name = detector.Name;
            x = linspace(detector.X.Start, detector.X.Stop, detector.X.Count);
            y = linspace(detector.Y.Start, detector.Y.Stop, detector.Y.Count);
            maxDepth = linspace(detector.MaxDepth.Start, detector.MaxDepth.Stop, ...
                detector.MaxDepth.Count);
            ROfXAndYAndMaxDepthRecessed.X = x;
            ROfXAndYAndMaxDepthRecessed.Y = y;
            ROfXAndYAndMaxDepthRecessed.MaxDepth = maxDepth;
            ROfXAndYAndMaxDepthRecessed.X_Midpoints = (x(1:end-1) + x(2:end)) / 2;
            ROfXAndYAndMaxDepthRecessed.Y_Midpoints = (y(1:end-1) + y(2:end)) / 2;
            ROfXAndYAndMaxDepthRecessed.MaxDepth_Midpoints = (maxDepth(1:end-1) + maxDepth(2:end)) / 2;
            dataSize = [numel(maxDepth) - 1, numel(y) - 1, numel(x) - 1];
            dataFile = [datadir slash detector.Name];
            ROfXAndYAndMaxDepthRecessed.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                ROfXAndYAndMaxDepthRecessed.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                ROfXAndYAndMaxDepthRecessed.Stdev = sqrt( ...
                    (ROfXAndYAndMaxDepthRecessed.SecondMoment - ...
                    ROfXAndYAndMaxDepthRecessed.Mean.^2) / json.N);
            end
            results{di}.ROfXAndYAndMaxDepthRecessed = ROfXAndYAndMaxDepthRecessed;
        case 'ROfFx'
            ROfFx.Name = detector.Name;
            ROfFx.Fx = linspace(detector.Fx.Start, detector.Fx.Stop, detector.Fx.Count);
            ROfFx.Fx_Midpoints = ROfFx.Fx;
            dataFile = [datadir slash detector.Name];
            dataSize = 2 * numel(ROfFx.Fx);
            tempData = readBinaryData(dataFile, dataSize);
            ROfFx.Mean = complex(tempData(1:2:end), tempData(2:2:end));
            ROfFx.Amplitude = abs(ROfFx.Mean);
            ROfFx.Phase = -angle(ROfFx.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                tempData = readBinaryData(secondMomentFile, dataSize);
                ROfFx.SecondMoment = tempData(1:2:end) + tempData(2:2:end);
                ROfFx.Stdev = sqrt((ROfFx.SecondMoment ...
                    - real(ROfFx.Mean).^2 - imag(ROfFx.Mean).^2) / json.N);
            end
            results{di}.ROfFx = ROfFx;
        case 'ROfFxAndTime'
            ROfFxAndTime.Name = detector.Name;
            fx = detector.Fx;
            time = detector.Time;
            ROfFxAndTime.Fx = linspace(fx.Start, fx.Stop, fx.Count);
            ROfFxAndTime.Time = linspace(time.Start, time.Stop, time.Count);
            ROfFxAndTime.Fx_Midpoints = ROfFxAndTime.Fx;
            ROfFxAndTime.Time_Midpoints = movmean(ROfFxAndTime.Time, 2, ...
                'Endpoints', 'discard');
            dataFile = [datadir slash detector.Name];
            dataSize = [2 * (numel(ROfFxAndTime.Time) - 1), ...
                numel(ROfFxAndTime.Fx)];
            tempData = readBinaryData(dataFile, dataSize);
            ROfFxAndTime.Mean = complex(tempData(1:2:end, :), ...
                tempData(2:2:end, :));
            ROfFxAndTime.Amplitude = abs(ROfFxAndTime.Mean);
            ROfFxAndTime.Phase = -angle(ROfFxAndTime.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                tempData = readBinaryData(secondMomentFile, dataSize);
                ROfFxAndTime.SecondMoment = tempData(1:2:end, :) + ...
                    tempData(2:2:end, :);
                ROfFxAndTime.Stdev = sqrt((ROfFxAndTime.SecondMoment ...
                    - real(ROfFxAndTime.Mean).^2 ...
                    - imag(ROfFxAndTime.Mean).^2) / json.N);
            end
            results{di}.ROfFxAndTime = ROfFxAndTime;
        case 'ROfFxAndAngle'
            ROfFxAndAngle.Name = detector.Name;
            ROfFxAndAngle.Fx = linspace(detector.Fx.Start, detector.Fx.Stop, detector.Fx.Count);
            ROfFxAndAngle.Angle = linspace(detector.Angle.Start, detector.Angle.Stop, detector.Angle.Count);
            ROfFxAndAngle.Fx_Midpoints = ROfFxAndAngle.Fx;
            ROfFxAndAngle.Angle_Midpoints = (ROfFxAndAngle.Angle(1:end-1) + ROfFxAndAngle.Angle(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [2 * (numel(ROfFxAndAngle.Angle) - 1), numel(ROfFxAndAngle.Fx)];
            tempData = readBinaryData(dataFile, dataSize);
            ROfFxAndAngle.Mean = complex(tempData(1:2:end, :), tempData(2:2:end, :));
            ROfFxAndAngle.Amplitude = abs(ROfFxAndAngle.Mean);
            ROfFxAndAngle.Phase = -angle(ROfFxAndAngle.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && isfile(secondMomentFile)
                tempData = readBinaryData(secondMomentFile, dataSize);
                ROfFxAndAngle.SecondMoment = tempData(1:2:end, :) + tempData(2:2:end, :);
                ROfFxAndAngle.Stdev = sqrt((ROfFxAndAngle.SecondMoment ...
                    - real(ROfFxAndAngle.Mean).^2 ...
                    - imag(ROfFxAndAngle.Mean).^2) / json.N);
            end
            results{di}.ROfFxAndAngle = ROfFxAndAngle;
        case 'ROfFxAndMaxDepth'
            ROfFxAndMaxDepth.Name = detector.Name;
            tempFx = detector.Fx;
            tempMaxDepth = detector.MaxDepth;
            ROfFxAndMaxDepth.Fx = linspace(tempFx.Start, tempFx.Stop, tempFx.Count);
            ROfFxAndMaxDepth.MaxDepth = linspace(tempMaxDepth.Start, tempMaxDepth.Stop, tempMaxDepth.Count);
            ROfFxAndMaxDepth.Fx_Midpoints = ROfFxAndMaxDepth.Fx;
            ROfFxAndMaxDepth.MaxDepth_Midpoints = (ROfFxAndMaxDepth.MaxDepth(1:end-1) + ROfFxAndMaxDepth.MaxDepth(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [2 * (numel(ROfFxAndMaxDepth.MaxDepth) - 1), numel(ROfFxAndMaxDepth.Fx)];
            tempData = readBinaryData(dataFile, dataSize);
            ROfFxAndMaxDepth.Mean = complex(tempData(1:2:end, :), tempData(2:2:end, :));
            ROfFxAndMaxDepth.Amplitude = abs(ROfFxAndMaxDepth.Mean);
            ROfFxAndMaxDepth.Phase = -angle(ROfFxAndMaxDepth.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && isfile(secondMomentFile)
                tempData = readBinaryData(secondMomentFile, dataSize);
                ROfFxAndMaxDepth.SecondMoment = ...
                    tempData(1:2:end, :) + tempData(2:2:end, :);
                ROfFxAndMaxDepth.Stdev = sqrt((ROfFxAndMaxDepth.SecondMoment ...
                    - real(ROfFxAndMaxDepth.Mean).^2 ...
                    - imag(ROfFxAndMaxDepth.Mean).^2) / json.N);
            end
            results{di}.ROfFxAndMaxDepth = ROfFxAndMaxDepth;
        case 'TDiffuse'
            TDiffuse.Name = detector.Name;
            dataFile = [datadir slash detector.Name '.txt'];
            data = readAndParseJson(dataFile);
            TDiffuse.Mean = data.Mean;
            TDiffuse.SecondMoment = data.SecondMoment;
            TDiffuse.Stdev = sqrt((TDiffuse.SecondMoment -  TDiffuse.Mean.^2) / json.N);
            results{di}.TDiffuse = TDiffuse;
        case 'TDiffuseBoundingVolume'
            TDiffuseBoundingVolume.Name = detector.Name;
            dataFile = [datadir slash detector.Name '.txt'];
            data = readAndParseJson(dataFile);
            TDiffuseBoundingVolume.Mean = data.Mean;
            TDiffuseBoundingVolume.SecondMoment = data.SecondMoment;
            TDiffuseBoundingVolume.Stdev = sqrt((TDiffuseBoundingVolume.SecondMoment -  TDiffuseBoundingVolume.Mean.^2) / json.N);
            results{di}.TDiffuseBoundingVolume = TDiffuseBoundingVolume;
        case 'TOfRho'
            TOfRho.Name = detector.Name;
            tempRho = detector.Rho;
            TOfRho.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            TOfRho.Rho_Midpoints = (TOfRho.Rho(1:end-1) + TOfRho.Rho(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = numel(TOfRho.Rho) - 1;
            TOfRho.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && isfile(secondMomentFile)
                TOfRho.SecondMoment = readBinaryData(secondMomentFile, dataSize);
                TOfRho.Stdev = sqrt((TOfRho.SecondMoment - TOfRho.Mean.^2) / json.N);
            end
            results{di}.TOfRho = TOfRho;
        case 'TOfAngle'
            TOfAngle.Name = detector.Name;
            TOfAngle.Angle = linspace(detector.Angle.Start, detector.Angle.Stop, ...
                detector.Angle.Count);
            TOfAngle.Angle_Midpoints = (TOfAngle.Angle(1:end-1) + TOfAngle.Angle(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = numel(TOfAngle.Angle) - 1;
            TOfAngle.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && isfile(secondMomentFile)
                TOfAngle.SecondMoment = readBinaryData(secondMomentFile, dataSize);
                TOfAngle.Stdev = sqrt((TOfAngle.SecondMoment - TOfAngle.Mean.^2) / json.N);
            end
            results{di}.TOfAngle = TOfAngle;
        case 'TOfRhoAndAngle'
            TOfRhoAndAngle.Name = detector.Name;
            tempRho = detector.Rho;
            tempAngle = detector.Angle;
            TOfRhoAndAngle.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            TOfRhoAndAngle.Angle = linspace(tempAngle.Start, tempAngle.Stop, tempAngle.Count);
            TOfRhoAndAngle.Rho_Midpoints = (TOfRhoAndAngle.Rho(1:end-1) + TOfRhoAndAngle.Rho(2:end)) / 2;
            TOfRhoAndAngle.Angle_Midpoints = (TOfRhoAndAngle.Angle(1:end-1) + TOfRhoAndAngle.Angle(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(TOfRhoAndAngle.Angle) - 1, numel(TOfRhoAndAngle.Rho) - 1];
            TOfRhoAndAngle.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                TOfRhoAndAngle.SecondMoment = readBinaryData(secondMomentFile, dataSize);
                TOfRhoAndAngle.Stdev = sqrt( ...
                    (TOfRhoAndAngle.SecondMoment - TOfRhoAndAngle.Mean.^2) / json.N);
            end
            results{di}.TOfRhoAndAngle = TOfRhoAndAngle;
        case 'TOfXAndY'
            TOfXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            TOfXAndY.X = linspace(tempX.Start, tempX.Stop, tempX.Count);
            TOfXAndY.Y = linspace(tempY.Start, tempY.Stop, tempY.Count);
            TOfXAndY.X_Midpoints = (TOfXAndY.X(1:end-1) + TOfXAndY.X(2:end)) / 2;
            TOfXAndY.Y_Midpoints = (TOfXAndY.Y(1:end-1) + TOfXAndY.Y(2:end)) / 2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(TOfXAndY.Y) - 1, numel(TOfXAndY.X) - 1];
            TOfXAndY.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                TOfXAndY.SecondMoment = readBinaryData(secondMomentFile, dataSize);
                TOfXAndY.Stdev = sqrt( ...
                    (TOfXAndY.SecondMoment - TOfXAndY.Mean.^2) / json.N);
            end
            results{di}.TOfXAndY = TOfXAndY;
        case 'TOfXAndYAndTimeAndSubregion'
            TOfXAndYAndTimeAndSubregion.Name = detector.Name;
            x = detector.X;
            y = detector.Y;
            time = detector.Time;
            nRegions = numel(json.TissueInput.Regions);
            TOfXAndYAndTimeAndSubregion.X = linspace(x.Start, x.Stop, x.Count);
            TOfXAndYAndTimeAndSubregion.Y = linspace(y.Start, y.Stop, y.Count);
            TOfXAndYAndTimeAndSubregion.Time = linspace(time.Start, time.Stop, time.Count);
            TOfXAndYAndTimeAndSubregion.X_Midpoints = (TOfXAndYAndTimeAndSubregion.X(1:end-1) + TOfXAndYAndTimeAndSubregion.X(2:end)) / 2;
            TOfXAndYAndTimeAndSubregion.Y_Midpoints = (TOfXAndYAndTimeAndSubregion.Y(1:end-1) + TOfXAndYAndTimeAndSubregion.Y(2:end)) / 2;
            TOfXAndYAndTimeAndSubregion.Time_Midpoints = (TOfXAndYAndTimeAndSubregion.Time(1:end-1) + TOfXAndYAndTimeAndSubregion.Time(2:end)) / 2;
            TOfXAndYAndTimeAndSubregion.NumberOfRegions = nRegions;
            nx = numel(TOfXAndYAndTimeAndSubregion.X) - 1;
            ny = numel(TOfXAndYAndTimeAndSubregion.Y) - 1;
            nt = numel(TOfXAndYAndTimeAndSubregion.Time) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = [nRegions, nt, ny, nx];
            TOfXAndYAndTimeAndSubregion.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            TOfXAndYAndTimeAndSubregion.TOfXAndY = ...
                readBinaryData([dataFile '_TOfXAndY'], [ny, nx]);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && isfile(secondMomentFile)
                TOfXAndYAndTimeAndSubregion.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                TOfXAndYAndTimeAndSubregion.Stdev = sqrt( ...
                    (TOfXAndYAndTimeAndSubregion.SecondMoment - ...
                    TOfXAndYAndTimeAndSubregion.Mean.^2) / json.N);
                secondMomentFile = [dataFile '_TOfXAndY_2'];
                if isfile(secondMomentFile)
                    TOfXAndYAndTimeAndSubregion.TOfXAndYSecondMoment = ...
                        readBinaryData(secondMomentFile, [ny, nx]);
                    TOfXAndYAndTimeAndSubregion.TOfXAndYStdev = sqrt( ...
                        (TOfXAndYAndTimeAndSubregion.TOfXAndYSecondMoment - ...
                        TOfXAndYAndTimeAndSubregion.TOfXAndY.^2) / json.N);
                end
            end
            results{di}.TOfXAndYAndTimeAndSubregion = TOfXAndYAndTimeAndSubregion;
        case 'TOfFx'
            TOfFx.Name = detector.Name;
            fx = detector.Fx;
            TOfFx.Fx = linspace(fx.Start, fx.Stop, fx.Count);
            TOfFx.Fx_Midpoints = TOfFx.Fx;
            dataFile = [datadir slash detector.Name];
            tempData = readBinaryData(dataFile, 2 * numel(TOfFx.Fx));
            TOfFx.Mean = complex(tempData(1:2:end), tempData(2:2:end));
            TOfFx.Amplitude = abs(TOfFx.Mean);
            TOfFx.Phase = -angle(TOfFx.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                tempData = readBinaryData(secondMomentFile, 2 * numel(TOfFx.Fx));
                TOfFx.SecondMoment = tempData(1:2:end) + tempData(2:2:end);
                TOfFx.Stdev = sqrt((TOfFx.SecondMoment - abs(TOfFx.Mean).^2) / json.N);
            end
            results{di}.TOfFx = TOfFx;
        case 'ATotal'
            ATotal.Name = detector.Name;
            ATotalData = readAndParseJson([datadir slash detector.Name '.txt']);
            ATotal.Mean = ATotalData.Mean;
            ATotal.SecondMoment = ATotalData.SecondMoment;
            ATotal.Stdev = sqrt((ATotal.SecondMoment - ATotal.Mean.^2) / json.N);
            results{di}.ATotal = ATotal;
            AOfRhoAndZ.Name = detector.Name;
            dataFile = [datadir slash detector.Name];
        case 'AOfRhoAndZ'
            AOfRhoAndZ.Name = detector.Name;
            dataFile = [datadir slash detector.Name];
            rho = detector.Rho;
            z = detector.Z;
            AOfRhoAndZ.Rho = linspace(rho.Start, rho.Stop, rho.Count);
            AOfRhoAndZ.Z = linspace(z.Start, z.Stop, z.Count);
            AOfRhoAndZ.Rho_Midpoints = diff(AOfRhoAndZ.Rho) / 2 + AOfRhoAndZ.Rho(1:end-1);
            AOfRhoAndZ.Z_Midpoints = diff(AOfRhoAndZ.Z) / 2 + AOfRhoAndZ.Z(1:end-1);
            dataSize = [numel(AOfRhoAndZ.Z) - 1, numel(AOfRhoAndZ.Rho) - 1];
            AOfRhoAndZ.Mean = readBinaryData(dataFile, dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                AOfRhoAndZ.SecondMoment = readBinaryData(secondMomentFile, dataSize);
                AOfRhoAndZ.Stdev = sqrt( ...
                    (AOfRhoAndZ.SecondMoment - AOfRhoAndZ.Mean.^2) / json.N);
            end
            results{di}.AOfRhoAndZ = AOfRhoAndZ;
        case 'AOfXAndYAndZ'
            AOfXAndYAndZ.Name = detector.Name;
            dataFile = [datadir slash detector.Name];
            x = detector.X;
            y = detector.Y;
            z = detector.Z;
            AOfXAndYAndZ.X = linspace(x.Start, x.Stop, x.Count);
            AOfXAndYAndZ.Y = linspace(y.Start, y.Stop, y.Count);
            AOfXAndYAndZ.Z = linspace(z.Start, z.Stop, z.Count);
            AOfXAndYAndZ.X_Midpoints = (AOfXAndYAndZ.X(1:end-1) + AOfXAndYAndZ.X(2:end)) / 2;
            AOfXAndYAndZ.Y_Midpoints = (AOfXAndYAndZ.Y(1:end-1) + AOfXAndYAndZ.Y(2:end)) / 2;
            AOfXAndYAndZ.Z_Midpoints = (AOfXAndYAndZ.Z(1:end-1) + AOfXAndYAndZ.Z(2:end)) / 2;
            dataSize = [numel(AOfXAndYAndZ.Z) - 1, ...
                        numel(AOfXAndYAndZ.Y) - 1, ...
                        numel(AOfXAndYAndZ.X) - 1];
            dataCount = prod(dataSize);
            AOfXAndYAndZ.Mean = reshape( ...
                readBinaryData(dataFile, dataCount), dataSize); % read column major json binary
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                AOfXAndYAndZ.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, dataCount), dataSize); % read column major json binary
                AOfXAndYAndZ.Stdev = sqrt( ...
                    (AOfXAndYAndZ.SecondMoment - AOfXAndYAndZ.Mean.^2) / json.N);
            end
            results{di}.AOfXAndYAndZ = AOfXAndYAndZ;
        case 'ATotalBoundingVolume'
            ATotalBoundingVolume.Name = detector.Name;
            data = readAndParseJson([datadir slash detector.Name '.txt']);
            ATotalBoundingVolume.Mean = data.Mean;
            ATotalBoundingVolume.SecondMoment = data.SecondMoment;
            ATotalBoundingVolume.Stdev = sqrt( ...
                (data.SecondMoment - data.Mean.^2) / json.N);
            results{di}.ATotalBoundingVolume = ATotalBoundingVolume;
        case 'FluenceOfRhoAndZ'
            FluenceOfRhoAndZ.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            FluenceOfRhoAndZ.Rho = linspace(tempRho.Start,tempRho.Stop,tempRho.Count);
            FluenceOfRhoAndZ.Z = linspace(tempZ.Start,tempZ.Stop,tempZ.Count);
            FluenceOfRhoAndZ.Rho_Midpoints = (FluenceOfRhoAndZ.Rho(1:end-1) + FluenceOfRhoAndZ.Rho(2:end)) / 2;
            FluenceOfRhoAndZ.Z_Midpoints = (FluenceOfRhoAndZ.Z(1:end-1) + FluenceOfRhoAndZ.Z(2:end)) / 2;
            dataSize = [numel(FluenceOfRhoAndZ.Z)-1,numel(FluenceOfRhoAndZ.Rho)-1];
            dataFile = [datadir slash detector.Name];
            FluenceOfRhoAndZ.Mean = readBinaryData(dataFile,dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile,'file')
                FluenceOfRhoAndZ.SecondMoment = readBinaryData(secondMomentFile,dataSize);
                FluenceOfRhoAndZ.Stdev = sqrt((FluenceOfRhoAndZ.SecondMoment - FluenceOfRhoAndZ.Mean.^2) / json.N);
            end
            results{di}.FluenceOfRhoAndZ = FluenceOfRhoAndZ;
        case 'FluenceOfRhoAndZAndTime'
            FluenceOfRhoAndZAndTime.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempTime = detector.Time;
            FluenceOfRhoAndZAndTime.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            FluenceOfRhoAndZAndTime.Z = linspace(tempZ.Start, tempZ.Stop, tempZ.Count);
            FluenceOfRhoAndZAndTime.Time = linspace(tempTime.Start, tempTime.Stop, tempTime.Count);
            FluenceOfRhoAndZAndTime.Rho_Midpoints = (FluenceOfRhoAndZAndTime.Rho(1:end-1) + FluenceOfRhoAndZAndTime.Rho(2:end)) / 2;
            FluenceOfRhoAndZAndTime.Z_Midpoints =  (FluenceOfRhoAndZAndTime.Z(1:end-1) + FluenceOfRhoAndZAndTime.Z(2:end)) / 2;
            FluenceOfRhoAndZAndTime.Time_Midpoints = (FluenceOfRhoAndZAndTime.Time(1:end-1) + FluenceOfRhoAndZAndTime.Time(2:end)) / 2;
            dataSize = [numel(FluenceOfRhoAndZAndTime.Time) - 1, ...
                        numel(FluenceOfRhoAndZAndTime.Z) - 1, ...
                        numel(FluenceOfRhoAndZAndTime.Rho) - 1];
            dataFile = [datadir slash detector.Name];
            FluenceOfRhoAndZAndTime.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                FluenceOfRhoAndZAndTime.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfRhoAndZAndTime.Stdev = sqrt( ...
                    (FluenceOfRhoAndZAndTime.SecondMoment - ...
                    FluenceOfRhoAndZAndTime.Mean.^2) / json.N);
            end
            results{di}.FluenceOfRhoAndZAndTime = FluenceOfRhoAndZAndTime;
        case 'FluenceOfRhoAndZAndOmega'
            FluenceOfRhoAndZAndOmega.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempOmega = detector.Omega;
            FluenceOfRhoAndZAndOmega.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            FluenceOfRhoAndZAndOmega.Z = linspace(tempZ.Start, tempZ.Stop, tempZ.Count);
            FluenceOfRhoAndZAndOmega.Omega = linspace(tempOmega.Start, tempOmega.Stop, tempOmega.Count);
            FluenceOfRhoAndZAndOmega.Rho_Midpoints = (FluenceOfRhoAndZAndOmega.Rho(1:end-1) + FluenceOfRhoAndZAndOmega.Rho(2:end)) / 2;
            FluenceOfRhoAndZAndOmega.Z_Midpoints = (FluenceOfRhoAndZAndOmega.Z(1:end-1) + FluenceOfRhoAndZAndOmega.Z(2:end)) / 2;
            FluenceOfRhoAndZAndOmega.Omega_Midpoints = FluenceOfRhoAndZAndOmega.Omega;
            nRho = numel(FluenceOfRhoAndZAndOmega.Rho) - 1;
            nZ = numel(FluenceOfRhoAndZAndOmega.Z) - 1;
            nOmega = numel(FluenceOfRhoAndZAndOmega.Omega);
            dataSize = [2 * nOmega, nZ, nRho];
            dataFile = [datadir slash detector.Name];
            data = reshape(readBinaryData(dataFile, prod(dataSize)), dataSize);
            FluenceOfRhoAndZAndOmega.Mean = data(1:2:end,:,:) + 1i * data(2:2:end,:,:);
            FluenceOfRhoAndZAndOmega.Amplitude = abs(FluenceOfRhoAndZAndOmega.Mean);
            FluenceOfRhoAndZAndOmega.Phase = -angle(FluenceOfRhoAndZAndOmega.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                data = reshape(readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfRhoAndZAndOmega.SecondMoment = data(1:2:end,:,:) + data(2:2:end,:,:);
                meanReal = real(FluenceOfRhoAndZAndOmega.Mean);
                meanImag = imag(FluenceOfRhoAndZAndOmega.Mean);
                FluenceOfRhoAndZAndOmega.Stdev = sqrt( ...
                    (FluenceOfRhoAndZAndOmega.SecondMoment - meanReal.^2 - meanImag.^2) / json.N);
            end
            results{di}.FluenceOfRhoAndZAndOmega = FluenceOfRhoAndZAndOmega;
        case 'FluenceOfXAndYAndZ'
            FluenceOfXAndYAndZ.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            FluenceOfXAndYAndZ.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            FluenceOfXAndYAndZ.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            FluenceOfXAndYAndZ.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfXAndYAndZ.X_Midpoints = (FluenceOfXAndYAndZ.X(1:end-1) + FluenceOfXAndYAndZ.X(2:end))/2;
            FluenceOfXAndYAndZ.Y_Midpoints = (FluenceOfXAndYAndZ.Y(1:end-1) + FluenceOfXAndYAndZ.Y(2:end))/2;
            FluenceOfXAndYAndZ.Z_Midpoints = (FluenceOfXAndYAndZ.Z(1:end-1) + FluenceOfXAndYAndZ.Z(2:end))/2;
            nX = numel(FluenceOfXAndYAndZ.X) - 1;
            nY = numel(FluenceOfXAndYAndZ.Y) - 1;
            nZ = numel(FluenceOfXAndYAndZ.Z) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = [nZ, nY, nX];
            FluenceOfXAndYAndZ.Mean = reshape(readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                FluenceOfXAndYAndZ.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfXAndYAndZ.Stdev = sqrt( ...
                    (FluenceOfXAndYAndZ.SecondMoment - ...
                    FluenceOfXAndYAndZ.Mean.^2) / json.N);
            end
            results{di}.FluenceOfXAndYAndZ = FluenceOfXAndYAndZ;
        case 'FluenceOfXAndYAndZAndTime'
            FluenceOfXAndYAndZAndTime.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempTime = detector.Time;
            FluenceOfXAndYAndZAndTime.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            FluenceOfXAndYAndZAndTime.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            FluenceOfXAndYAndZAndTime.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfXAndYAndZAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            FluenceOfXAndYAndZAndTime.X_Midpoints = (FluenceOfXAndYAndZAndTime.X(1:end-1) + FluenceOfXAndYAndZAndTime.X(2:end))/2;
            FluenceOfXAndYAndZAndTime.Y_Midpoints = (FluenceOfXAndYAndZAndTime.Y(1:end-1) + FluenceOfXAndYAndZAndTime.Y(2:end))/2;
            FluenceOfXAndYAndZAndTime.Z_Midpoints = (FluenceOfXAndYAndZAndTime.Z(1:end-1) + FluenceOfXAndYAndZAndTime.Z(2:end))/2;
            FluenceOfXAndYAndZAndTime.Time_Midpoints = (FluenceOfXAndYAndZAndTime.Time(1:end-1) + FluenceOfXAndYAndZAndTime.Time(2:end))/2;
            nX = numel(FluenceOfXAndYAndZAndTime.X) - 1;
            nY = numel(FluenceOfXAndYAndZAndTime.Y) - 1;
            nZ = numel(FluenceOfXAndYAndZAndTime.Z) - 1;
            nTime = numel(FluenceOfXAndYAndZAndTime.Time) - 1;
            dataSize = [nTime, nZ, nY, nX];
            dataFile = [datadir slash detector.Name];
            FluenceOfXAndYAndZAndTime.Mean = reshape(readBinaryData(dataFile, prod(dataSize)), dataSize);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                FluenceOfXAndYAndZAndTime.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfXAndYAndZAndTime.Stdev = sqrt( ...
                    (FluenceOfXAndYAndZAndTime.SecondMoment - ...
                    FluenceOfXAndYAndZAndTime.Mean.^2) / json.N);
            end
            results{di}.FluenceOfXAndYAndZAndTime = FluenceOfXAndYAndZAndTime;
        case 'FluenceOfXAndYAndZAndOmega'
            FluenceOfXAndYAndZAndOmega.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempOmega = detector.Omega;
            FluenceOfXAndYAndZAndOmega.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            FluenceOfXAndYAndZAndOmega.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            FluenceOfXAndYAndZAndOmega.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfXAndYAndZAndOmega.Omega = linspace((tempOmega.Start), (tempOmega.Stop), (tempOmega.Count));
            FluenceOfXAndYAndZAndOmega.X_Midpoints = (FluenceOfXAndYAndZAndOmega.X(1:end-1) + FluenceOfXAndYAndZAndOmega.X(2:end))/2;
            FluenceOfXAndYAndZAndOmega.Y_Midpoints = (FluenceOfXAndYAndZAndOmega.Y(1:end-1) + FluenceOfXAndYAndZAndOmega.Y(2:end))/2;
            FluenceOfXAndYAndZAndOmega.Z_Midpoints = (FluenceOfXAndYAndZAndOmega.Z(1:end-1) + FluenceOfXAndYAndZAndOmega.Z(2:end))/2;
            FluenceOfXAndYAndZAndOmega.Omega_Midpoints = FluenceOfXAndYAndZAndOmega.Omega; % omega is not binned, value is used
            nX = numel(FluenceOfXAndYAndZAndOmega.X) - 1;
            nY = numel(FluenceOfXAndYAndZAndOmega.Y) - 1;
            nZ = numel(FluenceOfXAndYAndZAndOmega.Z) - 1;
            nOmega = numel(FluenceOfXAndYAndZAndOmega.Omega);
            dataFile = [datadir slash detector.Name];
            dataSize = [2 * nOmega, nZ, nY, nX];
            tempData = reshape(readBinaryData(dataFile, prod(dataSize)), dataSize);
            FluenceOfXAndYAndZAndOmega.Mean = ...
                tempData(1:2:end,:,:,:) + 1i * tempData(2:2:end,:,:,:);
            FluenceOfXAndYAndZAndOmega.Amplitude =  abs(FluenceOfXAndYAndZAndOmega.Mean);
            FluenceOfXAndYAndZAndOmega.Phase = -angle(FluenceOfXAndYAndZAndOmega.Mean);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                tempData = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfXAndYAndZAndOmega.SecondMoment = ...
                    tempData(1:2:end,:,:,:) + tempData(2:2:end,:,:,:);
                FluenceOfXAndYAndZAndOmega.Stdev = sqrt( ...
                    (FluenceOfXAndYAndZAndOmega.SecondMoment - ...
                    abs(FluenceOfXAndYAndZAndOmega.Mean).^2) / json.N);
            end
            results{di}.FluenceOfXAndYAndZAndOmega = ...
                FluenceOfXAndYAndZAndOmega;
        case 'FluenceOfXAndYAndZAndStartingXAndY'
            FluenceOfXAndYAndZAndStartingXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempSX = detector.StartingX;
            tempSY = detector.StartingY;
            FluenceOfXAndYAndZAndStartingXAndY.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            FluenceOfXAndYAndZAndStartingXAndY.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            FluenceOfXAndYAndZAndStartingXAndY.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfXAndYAndZAndStartingXAndY.StartingX = linspace((tempSX.Start),(tempSX.Stop),(tempSX.Count));
            FluenceOfXAndYAndZAndStartingXAndY.StartingY = linspace((tempSY.Start),(tempSY.Stop),(tempSY.Count));
            FluenceOfXAndYAndZAndStartingXAndY.X_Midpoints = (FluenceOfXAndYAndZAndStartingXAndY.X(1:end-1) + FluenceOfXAndYAndZAndStartingXAndY.X(2:end))/2;
            FluenceOfXAndYAndZAndStartingXAndY.Y_Midpoints = (FluenceOfXAndYAndZAndStartingXAndY.Y(1:end-1) + FluenceOfXAndYAndZAndStartingXAndY.Y(2:end))/2;
            FluenceOfXAndYAndZAndStartingXAndY.Z_Midpoints = (FluenceOfXAndYAndZAndStartingXAndY.Z(1:end-1) + FluenceOfXAndYAndZAndStartingXAndY.Z(2:end))/2;
            FluenceOfXAndYAndZAndStartingXAndY.StartingX_Midpoints = (FluenceOfXAndYAndZAndStartingXAndY.StartingX(1:end-1) + FluenceOfXAndYAndZAndStartingXAndY.StartingX(2:end))/2;
            FluenceOfXAndYAndZAndStartingXAndY.StartingY_Midpoints = (FluenceOfXAndYAndZAndStartingXAndY.StartingY(1:end-1) + FluenceOfXAndYAndZAndStartingXAndY.StartingY(2:end))/2;
            nX = numel(FluenceOfXAndYAndZAndStartingXAndY.X) - 1;
            nY = numel(FluenceOfXAndYAndZAndStartingXAndY.Y) - 1;
            nZ = numel(FluenceOfXAndYAndZAndStartingXAndY.Z) - 1;
            nSX = numel(FluenceOfXAndYAndZAndStartingXAndY.StartingX) - 1;
            nSY = numel(FluenceOfXAndYAndZAndStartingXAndY.StartingY) - 1;
            dataSize = [nZ, nY, nX, nSY, nSX];
            dataFile = [datadir slash detector.Name];
            FluenceOfXAndYAndZAndStartingXAndY.Mean = reshape( ...
                readBinaryData(dataFile, prod(dataSize)), dataSize);
            FluenceOfXAndYAndZAndStartingXAndY.StartingXYCount = ...
                readBinaryData([dataFile '_StartingXYCount'], [nSY, nSX]);
            secondMomentFile = [dataFile '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                FluenceOfXAndYAndZAndStartingXAndY.SecondMoment = reshape( ...
                    readBinaryData(secondMomentFile, prod(dataSize)), dataSize);
                FluenceOfXAndYAndZAndStartingXAndY.Stdev = sqrt( ...
                    (FluenceOfXAndYAndZAndStartingXAndY.SecondMoment - ...
                    abs(FluenceOfXAndYAndZAndStartingXAndY.Mean).^2) / json.N);
            end
            results{di}.FluenceOfXAndYAndZAndStartingXAndY = FluenceOfXAndYAndZAndStartingXAndY;
        case 'FluenceOfFxAndZ'
            FluenceOfFxAndZ.Name = detector.Name;
            tempFx = detector.Fx;
            tempZ = detector.Z;
            FluenceOfFxAndZ.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            FluenceOfFxAndZ.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfFxAndZ.Fx_Midpoints = FluenceOfFxAndZ.Fx;
            FluenceOfFxAndZ.Z_Midpoints = (FluenceOfFxAndZ.Z(1:end-1) + FluenceOfFxAndZ.Z(2:end))/2;
            tempData = readBinaryData([datadir slash detector.Name], ...
                [2*(length(FluenceOfFxAndZ.Z)-1),length(FluenceOfFxAndZ.Fx)]); % column major but with complex
            FluenceOfFxAndZ.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            FluenceOfFxAndZ.Amplitude = abs(FluenceOfFxAndZ.Mean);
            FluenceOfFxAndZ.Phase = -angle(FluenceOfFxAndZ.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ...
                    [2*(length(FluenceOfFxAndZ.Z)-1),length(FluenceOfFxAndZ.Fx)]);
                FluenceOfFxAndZ.SecondMoment = tempData(1:2:end,:) + tempData(2:2:end,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                FluenceOfFxAndZ.Stdev = sqrt((FluenceOfFxAndZ.SecondMoment - real(FluenceOfFxAndZ.Mean) .* real(FluenceOfFxAndZ.Mean) ...
                                                                           - imag(FluenceOfFxAndZ.Mean) .* imag(FluenceOfFxAndZ.Mean)) / json.N);
            end                 
            results{di}.FluenceOfFxAndZ = FluenceOfFxAndZ;
     case 'RadianceOfRhoAndZAndAngle'
            RadianceOfRhoAndZAndAngle.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempAngle = detector.Angle;
            RadianceOfRhoAndZAndAngle.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            RadianceOfRhoAndZAndAngle.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));                      
            RadianceOfRhoAndZAndAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            RadianceOfRhoAndZAndAngle.Rho_Midpoints = (RadianceOfRhoAndZAndAngle.Rho(1:end-1) + RadianceOfRhoAndZAndAngle.Rho(2:end))/2;
            RadianceOfRhoAndZAndAngle.Z_Midpoints = (RadianceOfRhoAndZAndAngle.Z(1:end-1) + RadianceOfRhoAndZAndAngle.Z(2:end))/2;
            RadianceOfRhoAndZAndAngle.Angle_Midpoints = (RadianceOfRhoAndZAndAngle.Angle(1:end-1) + RadianceOfRhoAndZAndAngle.Angle(2:end))/2;
            nRho = length(RadianceOfRhoAndZAndAngle.Rho) - 1;
            nZ = length(RadianceOfRhoAndZAndAngle.Z) - 1;
            nAngle = length(RadianceOfRhoAndZAndAngle.Angle) - 1;
            dataSize = [nAngle, nZ, nRho];
            RadianceOfRhoAndZAndAngle.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], prod(dataSize)), dataSize);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                RadianceOfRhoAndZAndAngle.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], prod(dataSize)), dataSize);
                RadianceOfRhoAndZAndAngle.Stdev = sqrt( ...
                    (RadianceOfRhoAndZAndAngle.SecondMoment - ...
                    abs(RadianceOfRhoAndZAndAngle.Mean).^2) / json.N);
            end
            results{di}.RadianceOfRhoAndZAndAngle = RadianceOfRhoAndZAndAngle;
        case 'RadianceOfRhoAtZ'
            RadianceOfRhoAtZ.Name = detector.Name;
            tempRho = detector.Rho;
            RadianceOfRhoAtZ.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            RadianceOfRhoAtZ.Rho_Midpoints = (RadianceOfRhoAtZ.Rho(1:end-1) + RadianceOfRhoAtZ.Rho(2:end)) / 2;
            RadianceOfRhoAtZ.Mean = readBinaryData( ...
                [datadir slash detector.Name], numel(RadianceOfRhoAtZ.Rho) - 1);
            secondMomentFile = [datadir slash detector.Name '_2'];
            if detector.TallySecondMoment && exist(secondMomentFile, 'file')
                RadianceOfRhoAtZ.SecondMoment = readBinaryData( ...
                    secondMomentFile, numel(RadianceOfRhoAtZ.Rho) - 1);
                RadianceOfRhoAtZ.Stdev = sqrt( ...
                    (RadianceOfRhoAtZ.SecondMoment - RadianceOfRhoAtZ.Mean.^2) / json.N);
            end
            results{di}.RadianceOfRhoAtZ = RadianceOfRhoAtZ;
        case 'RadianceOfFxAndZAndAngle'
            RadianceOfFxAndZAndAngle.Name = detector.Name;
            tempFx = detector.Fx;
            tempZ = detector.Z;
            tempAngle = detector.Angle;
            RadianceOfFxAndZAndAngle.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            RadianceOfFxAndZAndAngle.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));                      
            RadianceOfFxAndZAndAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            RadianceOfFxAndZAndAngle.Fx_Midpoints = RadianceOfFxAndZAndAngle.Fx(1:end);
            RadianceOfFxAndZAndAngle.Z_Midpoints = (RadianceOfFxAndZAndAngle.Z(1:end-1) + RadianceOfFxAndZAndAngle.Z(2:end))/2;
            RadianceOfFxAndZAndAngle.Angle_Midpoints = (RadianceOfFxAndZAndAngle.Angle(1:end-1) + RadianceOfFxAndZAndAngle.Angle(2:end))/2;
            nFx = numel(RadianceOfFxAndZAndAngle.Fx);
            nZ = numel(RadianceOfFxAndZAndAngle.Z) - 1;
            nAngle = numel(RadianceOfFxAndZAndAngle.Angle) - 1;
            dataSize = [nAngle, nZ, nFx];
            tempData = reshape(readBinaryData([datadir slash detector.Name], ...
                2 * prod(dataSize)), [2 * nAngle, nZ, nFx]);
            RadianceOfFxAndZAndAngle.Mean = ...
                complex(tempData(1:2:end,:,:), tempData(2:2:end,:,:));
            RadianceOfFxAndZAndAngle.Amplitude = abs(RadianceOfFxAndZAndAngle.Mean);
            RadianceOfFxAndZAndAngle.Phase = -angle(RadianceOfFxAndZAndAngle.Mean);
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                tempData = reshape(readBinaryData( ...
                    [datadir slash detector.Name '_2'], 2 * prod(dataSize)), ...
                    [2 * nAngle, nZ, nFx]);
                RadianceOfFxAndZAndAngle.SecondMoment = ...
                    tempData(1:2:end,:,:) + tempData(2:2:end,:,:);
                meanData = RadianceOfFxAndZAndAngle.Mean;
                RadianceOfFxAndZAndAngle.Stdev = sqrt( ...
                    (RadianceOfFxAndZAndAngle.SecondMoment - ...
                    real(meanData).^2 - imag(meanData).^2) / json.N);
            end
            results{di}.RadianceOfFxAndZAndAngle = RadianceOfFxAndZAndAngle;
        case 'RadianceOfXAndYAndZAndThetaAndPhi'
            RadianceOfXAndYAndZAndThetaAndPhi.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempTheta = detector.Theta;
            tempPhi = detector.Phi;
            RadianceOfXAndYAndZAndThetaAndPhi.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));                      
            RadianceOfXAndYAndZAndThetaAndPhi.Theta = linspace((tempTheta.Start), (tempTheta.Stop), (tempTheta.Count));    
            RadianceOfXAndYAndZAndThetaAndPhi.Phi = linspace((tempPhi.Start), (tempPhi.Stop), (tempPhi.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.X_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.X(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.X(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Y_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Y(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Y(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Z_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Z(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Z(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Theta_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Theta(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Theta(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Phi_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Phi(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Phi(2:end))/2;
            dims = [numel(RadianceOfXAndYAndZAndThetaAndPhi.Phi) - 1, ...
                numel(RadianceOfXAndYAndZAndThetaAndPhi.Theta) - 1, ...
                numel(RadianceOfXAndYAndZAndThetaAndPhi.Z) - 1, ...
                numel(RadianceOfXAndYAndZAndThetaAndPhi.Y) - 1, ...
                numel(RadianceOfXAndYAndZAndThetaAndPhi.X) - 1];
            fileName = [datadir slash detector.Name];
            RadianceOfXAndYAndZAndThetaAndPhi.Mean = reshape(readBinaryData(fileName, prod(dims)), dims);
            if detector.TallySecondMoment && exist([fileName '_2'], 'file')
                RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment = reshape( ...
                    readBinaryData([fileName '_2'], prod(dims)), dims);
                RadianceOfXAndYAndZAndThetaAndPhi.Stdev = sqrt( ...
                    (RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment - ...
                    RadianceOfXAndYAndZAndThetaAndPhi.Mean.^2) / json.N);
            end
            results{di}.RadianceOfXAndYAndZAndThetaAndPhi = RadianceOfXAndYAndZAndThetaAndPhi;
        case 'ReflectedMTOfRhoAndSubregionHist'
            ReflectedMTOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.VoxelRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = json.N;
            end
            ReflectedMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            ReflectedMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedMTOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedMTOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints = (ReflectedMTOfRhoAndSubregionHist.MTBins(1:end-1) + ReflectedMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            nRho = numel(ReflectedMTOfRhoAndSubregionHist.Rho) - 1;
            nMT = numel(ReflectedMTOfRhoAndSubregionHist.MTBins) - 1;
            nSubregions = numel(tempSubregionIndices);
            ReflectedMTOfRhoAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nMT * nRho), [nMT, nRho]);
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                nRho * nMT * nSubregions * tempFractionalMTBinsLength), ...
                [tempFractionalMTBinsLength, nSubregions, nMT, nRho]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                ReflectedMTOfRhoAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nMT * nRho), [nMT, nRho]);
                ReflectedMTOfRhoAndSubregionHist.Stdev = sqrt( ...
                    (ReflectedMTOfRhoAndSubregionHist.SecondMoment - ...
                    ReflectedMTOfRhoAndSubregionHist.Mean.^2) / N);
            end
            results{di}.ReflectedMTOfRhoAndSubregionHist = ReflectedMTOfRhoAndSubregionHist;
        case 'ReflectedMTOfXAndYAndSubregionHist'
            ReflectedMTOfXAndYAndSubregionHist.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempMTBins = detector.MTBins;              
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                           
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = json.N;
            end
            ReflectedMTOfXAndYAndSubregionHist.X = linspace(tempX.Start, tempX.Stop, tempX.Count);
            ReflectedMTOfXAndYAndSubregionHist.Y = linspace(tempY.Start, tempY.Stop, tempY.Count);
            ReflectedMTOfXAndYAndSubregionHist.MTBins = linspace(tempMTBins.Start, tempMTBins.Stop, tempMTBins.Count);
            ReflectedMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            xBins = numel(ReflectedMTOfXAndYAndSubregionHist.X) - 1;
            yBins = numel(ReflectedMTOfXAndYAndSubregionHist.Y) - 1;
            mtBins = numel(ReflectedMTOfXAndYAndSubregionHist.MTBins) - 1;
            nSubregions = numel(tempSubregionIndices);
            ReflectedMTOfXAndYAndSubregionHist.X_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.X(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.X(2:end)) / 2;
            ReflectedMTOfXAndYAndSubregionHist.Y_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.Y(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.Y(2:end)) / 2;
            ReflectedMTOfXAndYAndSubregionHist.MTBins_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.MTBins(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.MTBins(2:end)) / 2;
            nValues = mtBins * yBins * xBins;
            ReflectedMTOfXAndYAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nValues), ...
                [mtBins, yBins, xBins]);
            ReflectedMTOfXAndYAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                tempFractionalMTBinsLength * nSubregions * nValues), ...
                [tempFractionalMTBinsLength, nSubregions, mtBins, yBins, xBins]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                ReflectedMTOfXAndYAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nValues), ...
                    [mtBins, yBins, xBins]);
                ReflectedMTOfXAndYAndSubregionHist.Stdev = sqrt( ...
                    (ReflectedMTOfXAndYAndSubregionHist.SecondMoment - ...
                    ReflectedMTOfXAndYAndSubregionHist.Mean.^2) / N);
            end
            results{di}.ReflectedMTOfXAndYAndSubregionHist = ReflectedMTOfXAndYAndSubregionHist;
        case 'TransmittedMTOfRhoAndSubregionHist'
            TransmittedMTOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = json.N;
            end
            rhoBins = tempRho.Count - 1;
            mtBins = tempMTBins.Count - 1;
            nSubregions = numel(tempSubregionIndices);
            TransmittedMTOfRhoAndSubregionHist.Rho = linspace(tempRho.Start, tempRho.Stop, tempRho.Count);
            TransmittedMTOfRhoAndSubregionHist.MTBins = linspace(tempMTBins.Start, tempMTBins.Stop, tempMTBins.Count);
            TransmittedMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            TransmittedMTOfRhoAndSubregionHist.Rho_Midpoints = (TransmittedMTOfRhoAndSubregionHist.Rho(1:end-1) + TransmittedMTOfRhoAndSubregionHist.Rho(2:end)) / 2;
            TransmittedMTOfRhoAndSubregionHist.MTBins_Midpoints = (TransmittedMTOfRhoAndSubregionHist.MTBins(1:end-1) + TransmittedMTOfRhoAndSubregionHist.MTBins(2:end)) / 2;
            nValues = rhoBins * mtBins;
            TransmittedMTOfRhoAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nValues), ...
                [mtBins, rhoBins]);
            TransmittedMTOfRhoAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                tempFractionalMTBinsLength * nSubregions * nValues), ...
                [tempFractionalMTBinsLength, nSubregions, mtBins, rhoBins]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                TransmittedMTOfRhoAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nValues), ...
                    [mtBins, rhoBins]);
                TransmittedMTOfRhoAndSubregionHist.Stdev = sqrt( ...
                    (TransmittedMTOfRhoAndSubregionHist.SecondMoment - ...
                     TransmittedMTOfRhoAndSubregionHist.Mean.^2) / N);
            end
            results{di}.TransmittedMTOfRhoAndSubregionHist = TransmittedMTOfRhoAndSubregionHist;
        case 'TransmittedMTOfXAndYAndSubregionHist'
            TransmittedMTOfXAndYAndSubregionHist.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
                elseif (strcmp(json.TissueInput.TissueType, 'SingleVoxel'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.VoxelRegion));                            
                end
                N = json.N;
            end
            X = linspace(tempX.Start, tempX.Stop, tempX.Count);
            Y = linspace(tempY.Start, tempY.Stop, tempY.Count);
            MTBins = linspace(tempMTBins.Start, tempMTBins.Stop, tempMTBins.Count);
            nX = numel(X) - 1;
            nY = numel(Y) - 1;
            nMT = numel(MTBins) - 1;
            nSubregions = numel(tempSubregionIndices);
            TransmittedMTOfXAndYAndSubregionHist.X = X;
            TransmittedMTOfXAndYAndSubregionHist.Y = Y;
            TransmittedMTOfXAndYAndSubregionHist.MTBins = MTBins;
            TransmittedMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            TransmittedMTOfXAndYAndSubregionHist.X_Midpoints = (X(1:end-1) + X(2:end)) / 2;
            TransmittedMTOfXAndYAndSubregionHist.Y_Midpoints = (Y(1:end-1) + Y(2:end)) / 2;
            TransmittedMTOfXAndYAndSubregionHist.MTBins_Midpoints = (MTBins(1:end-1) + MTBins(2:end)) / 2;
            nValues = nMT * nY * nX;
            TransmittedMTOfXAndYAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nValues), [nMT, nY, nX]);
            TransmittedMTOfXAndYAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                tempFractionalMTBinsLength * nSubregions * nValues), ...
                [tempFractionalMTBinsLength, nSubregions, nMT, nY, nX]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                TransmittedMTOfXAndYAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nValues), ...
                    [nMT, nY, nX]);
                TransmittedMTOfXAndYAndSubregionHist.Stdev = sqrt( ...
                    (TransmittedMTOfXAndYAndSubregionHist.SecondMoment - ...
                     TransmittedMTOfXAndYAndSubregionHist.Mean.^2) / N);
            end
            results{di}.TransmittedMTOfXAndYAndSubregionHist = TransmittedMTOfXAndYAndSubregionHist;
        case 'ReflectedDynamicMTOfRhoAndSubregionHist'
            ReflectedDynamicMTOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempMTBins = detector.MTBins;
            tempZ = detector.Z;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            ReflectedDynamicMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            ReflectedDynamicMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedDynamicMTOfRhoAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.MTBins(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            ReflectedDynamicMTOfRhoAndSubregionHist.Z_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
            ReflectedDynamicMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nRho = numel(ReflectedDynamicMTOfRhoAndSubregionHist.Rho) - 1;
            nMT = numel(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins) - 1;
            nZ = numel(ReflectedDynamicMTOfRhoAndSubregionHist.Z) - 1;
            ReflectedDynamicMTOfRhoAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nMT * nRho), [nMT, nRho]);
            ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                tempFractionalMTBinsLength * nMT * nRho), ...
                [tempFractionalMTBinsLength, nMT, nRho]);
            ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = reshape( ...
                readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], nZ * nRho), ...
                [nZ, nRho]);
            ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = reshape( ...
                readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], nZ * nRho), ...
                [nZ, nRho]);
            ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = reshape( ...
                readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ...
                2 * numel(tempSubregionIndices)), [2, numel(tempSubregionIndices)]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nMT * nRho), ...
                    [nMT, nRho]);
                ReflectedDynamicMTOfRhoAndSubregionHist.Stdev = sqrt( ...
                    (ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment - ...
                     ReflectedDynamicMTOfRhoAndSubregionHist.Mean.^2) / N);
                ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], nZ * nRho), ...
                    [nZ, nRho]);
                ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZStdev = sqrt( ...
                    (ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment - ...
                     ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ.^2) / N);
                ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], nZ * nRho), ...
                    [nZ, nRho]);
                ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZStdev = sqrt( ...
                    (ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment - ...
                     ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ.^2) / N);
            end
            results{di}.ReflectedDynamicMTOfRhoAndSubregionHist = ReflectedDynamicMTOfRhoAndSubregionHist;
        case 'ReflectedDynamicMTOfXAndYAndSubregionHist'
            ReflectedDynamicMTOfXAndYAndSubregionHist.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempMTBins = detector.MTBins;              
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            ReflectedDynamicMTOfXAndYAndSubregionHist.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.X(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.X(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.Y(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.Y(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.Z(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nMT = numel(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins) - 1;
            nY = numel(ReflectedDynamicMTOfXAndYAndSubregionHist.Y) - 1;
            nX = numel(ReflectedDynamicMTOfXAndYAndSubregionHist.X) - 1;
            nZ = numel(ReflectedDynamicMTOfXAndYAndSubregionHist.Z) - 1;
            readAndReshape = @(suffix, dims) reshape( ...
                readBinaryData([datadir slash detector.Name suffix], prod(dims)), dims);
            ReflectedDynamicMTOfXAndYAndSubregionHist.Mean = ...
                readAndReshape('', [nMT, nY, nX]);
            ReflectedDynamicMTOfXAndYAndSubregionHist.FractionalMT = ...
                readAndReshape('_FractionalMT', [tempFractionalMTBinsLength, nMT, nY, nX]);
            ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = ...
                readAndReshape('_TotalMTOfZ', [nZ, nY, nX]);
            ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = ...
                readAndReshape('_DynamicMTOfZ', [nZ, nY, nX]);
            ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = reshape( ...
                readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ...
                2 * numel(tempSubregionIndices)), [2, numel(tempSubregionIndices)]);    
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                stdevFromMoments = @(secondMoment, meanValue) ...
                    sqrt((secondMoment - meanValue.^2) / N);
                ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment = ...
                    readAndReshape('_2', [nMT, nY, nX]);
                ReflectedDynamicMTOfXAndYAndSubregionHist.Stdev = ...
                    stdevFromMoments( ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment, ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.Mean);
                ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = ...
                    readAndReshape('_TotalMTOfZ_2', [nZ, nY, nX]);
                ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZStdev = ...
                    stdevFromMoments( ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment, ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ);
                ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = ...
                    readAndReshape('_DynamicMTOfZ_2', [nZ, nY, nX]);
                ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZStdev = ...
                    stdevFromMoments( ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment, ...
                    ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ);
            end
            results{di}.ReflectedDynamicMTOfXAndYAndSubregionHist = ReflectedDynamicMTOfXAndYAndSubregionHist;
        case 'ReflectedDynamicMTOfFxAndSubregionHist'
            ReflectedDynamicMTOfFxAndSubregionHist.Name = detector.Name;
            tempFx = detector.Fx;
            tempMTBins = detector.MTBins;
            tempZ = detector.Z;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            ReflectedDynamicMTOfFxAndSubregionHist.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));                     
            ReflectedDynamicMTOfFxAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedDynamicMTOfFxAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            ReflectedDynamicMTOfFxAndSubregionHist.Fx_Midpoints = ReflectedDynamicMTOfFxAndSubregionHist.Fx;
            ReflectedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints = (ReflectedDynamicMTOfFxAndSubregionHist.MTBins(1:end-1) + ReflectedDynamicMTOfFxAndSubregionHist.MTBins(2:end))/2;
            ReflectedDynamicMTOfFxAndSubregionHist.Z_Midpoints = (ReflectedDynamicMTOfFxAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfFxAndSubregionHist.Z(2:end))/2;
            ReflectedDynamicMTOfFxAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nFx = numel(ReflectedDynamicMTOfFxAndSubregionHist.Fx);
            nMT = numel(ReflectedDynamicMTOfFxAndSubregionHist.MTBins) - 1;
            nZ = numel(ReflectedDynamicMTOfFxAndSubregionHist.Z) - 1;
            tempData = readBinaryData([datadir slash detector.Name], 2*nMT*nFx);
            % NOTE! reshape with 2x dim of var in inner loop binaryWrite
            tempDataReshape = reshape(tempData, [2*nMT, nFx]);
            ReflectedDynamicMTOfFxAndSubregionHist.Mean = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            tempData = readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                2*nFx*nMT*tempFractionalMTBinsLength);
            tempDataReshape = reshape(tempData, ...
                [2*tempFractionalMTBinsLength, nMT, nFx]); % read column major json binary
            ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT = ...
                complex(tempDataReshape(1:2:end,:,:), tempDataReshape(2:2:end,:,:));
            tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], 2*nFx*nZ);
            tempDataReshape = reshape(tempData, [2*nZ, nFx]);
            ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], 2*nFx*nZ);
            tempDataReshape = reshape(tempData, [2*nZ, nFx]);
            ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions = ...
                readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ...
                2*numel(tempSubregionIndices)); % 2 for static vs dynamic tallies
            ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions = ...
                reshape(ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions, ...
                [2, numel(tempSubregionIndices)]); % read column major json binary
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                nFx = numel(ReflectedDynamicMTOfFxAndSubregionHist.Fx);
                nMT = numel(ReflectedDynamicMTOfFxAndSubregionHist.MTBins) - 1;
                nZ = numel(ReflectedDynamicMTOfFxAndSubregionHist.Z) - 1;
                tempData = readBinaryData([datadir slash detector.Name '_2'], 2*nMT*nFx); % read column major json binary
                tempDataReshape = reshape(tempData, [2*nMT, nFx]);
                ReflectedDynamicMTOfFxAndSubregionHist.SecondMoment = ...
                    complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
                ReflectedDynamicMTOfFxAndSubregionHist.Stdev = sqrt( ...
                    (abs(ReflectedDynamicMTOfFxAndSubregionHist.SecondMoment) - ...
                    abs(ReflectedDynamicMTOfFxAndSubregionHist.Mean).^2) / N);
                % depth dependent output
                tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], 2*nZ*nFx); % read column major json binary
                tempDataReshape = reshape(tempData, [2*nZ, nFx]);
                ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment = ...
                    complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
                ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZStdev = sqrt( ...
                    (abs(ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment) - ...
                    abs(ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ).^2) / N);
                tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], 2*nZ*nFx); % read column major json binary
                tempDataReshape = reshape(tempData, [2*nZ, nFx]);
                ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment = ...
                    complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
                ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZStdev = sqrt( ...
                    (ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment - ...
                    ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ.^2) / N);
            end
            results{di}.ReflectedDynamicMTOfFxAndSubregionHist = ReflectedDynamicMTOfFxAndSubregionHist;
        case 'TransmittedDynamicMTOfRhoAndSubregionHist'
            TransmittedDynamicMTOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            TransmittedDynamicMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            TransmittedDynamicMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedDynamicMTOfRhoAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.Rho(1:end-1) + TransmittedDynamicMTOfRhoAndSubregionHist.Rho(2:end))/2;
            TransmittedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.MTBins(1:end-1) + TransmittedDynamicMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            TransmittedDynamicMTOfRhoAndSubregionHist.Z_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
            TransmittedDynamicMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nMT = numel(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins) - 1;
            nRho = numel(TransmittedDynamicMTOfRhoAndSubregionHist.Rho) - 1;
            nZ = numel(TransmittedDynamicMTOfRhoAndSubregionHist.Z) - 1;
            TransmittedDynamicMTOfRhoAndSubregionHist.Mean = reshape( ...
                readBinaryData([datadir slash detector.Name], nMT * nRho), [nMT, nRho]);
            TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT = reshape( ...
                readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                nRho * nMT * tempFractionalMTBinsLength), ...
                [tempFractionalMTBinsLength, nMT, nRho]);
            TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = reshape( ...
                readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], nRho * nZ), ...
                [nZ, nRho]);
            TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = reshape( ...
                readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], nRho * nZ), ...
                [nZ, nRho]);
            TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = reshape( ...
                readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ...
                2 * numel(tempSubregionIndices)), [2, numel(tempSubregionIndices)]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                nMT = numel(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins) - 1;
                nRho = numel(TransmittedDynamicMTOfRhoAndSubregionHist.Rho) - 1;
                nZ = numel(TransmittedDynamicMTOfRhoAndSubregionHist.Z) - 1;
                TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_2'], nMT * nRho), ...
                    [nMT, nRho]); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.Stdev = sqrt( ...
                    (TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment - ...
                    TransmittedDynamicMTOfRhoAndSubregionHist.Mean.^2) / N);
                % depth dependent output
                TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], nZ * nRho), ...
                    [nZ, nRho]); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZStdev = sqrt( ...
                    (TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment - ...
                    TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ.^2) / N);
                TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = reshape( ...
                    readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], nZ * nRho), ...
                    [nZ, nRho]); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZStdev = sqrt( ...
                    (TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment - ...
                    TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ.^2) / N);
            end
            results{di}.TransmittedDynamicMTOfRhoAndSubregionHist = TransmittedDynamicMTOfRhoAndSubregionHist;
        case 'TransmittedDynamicMTOfXAndYAndSubregionHist'
            TransmittedDynamicMTOfXAndYAndSubregionHist.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            TransmittedDynamicMTOfXAndYAndSubregionHist.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            TransmittedDynamicMTOfXAndYAndSubregionHist.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedDynamicMTOfXAndYAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            TransmittedDynamicMTOfXAndYAndSubregionHist.X_Midpoints = (TransmittedDynamicMTOfXAndYAndSubregionHist.X(1:end-1) + TransmittedDynamicMTOfXAndYAndSubregionHist.X(2:end))/2;
            TransmittedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints = (TransmittedDynamicMTOfXAndYAndSubregionHist.Y(1:end-1) + TransmittedDynamicMTOfXAndYAndSubregionHist.Y(2:end))/2;
            TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints = (TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins(1:end-1) + TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins(2:end))/2;
            TransmittedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints = (TransmittedDynamicMTOfXAndYAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
            TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nMTBins = length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins) - 1;
            nY = length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y) - 1;
            nX = length(TransmittedDynamicMTOfXAndYAndSubregionHist.X) - 1;
            nZ = length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z) - 1;
            TransmittedDynamicMTOfXAndYAndSubregionHist.Mean = readBinaryData( ...
                [datadir slash detector.Name], nMTBins * nY * nX);
            TransmittedDynamicMTOfXAndYAndSubregionHist.Mean = reshape( ...
                TransmittedDynamicMTOfXAndYAndSubregionHist.Mean, [nMTBins, nY, nX]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT = readBinaryData( ...
                [datadir slash detector.Name '_FractionalMT'], ...
                nX * nY * nMTBins * tempFractionalMTBinsLength);
            TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT = reshape( ...
                TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT, ...
                [tempFractionalMTBinsLength, nMTBins, nY, nX]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = readBinaryData( ...
                [datadir slash detector.Name '_TotalMTOfZ'], nZ * nY * nX);
            TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = reshape( ...
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ, [nZ, nY, nX]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = readBinaryData( ...
                [datadir slash detector.Name '_DynamicMTOfZ'], nZ * nY * nX);
            TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = reshape( ...
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ, [nZ, nY, nX]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = readBinaryData( ...
                [datadir slash detector.Name '_SubregionCollisions'], 2 * length(tempSubregionIndices)); % 2 for static vs dynamic tallies
            TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = reshape( ...
                TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions, [2, length(tempSubregionIndices)]); % read column major json binary
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], nMTBins * nY * nX);
                TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment = reshape( ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment, [nMTBins, nY, nX]);
                TransmittedDynamicMTOfXAndYAndSubregionHist.Stdev = sqrt( ...
                    (TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment - ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.Mean.^2) / N);
                % depth dependent output
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = ...
                    readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], nZ * nY * nX);
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = reshape( ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment, [nZ, nY, nX]);
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZStdev = sqrt( ...
                    (TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment - ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ.^2) / N);
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = ...
                    readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], nZ * nY * nX);
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = reshape( ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment, [nZ, nY, nX]);
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZStdev = sqrt( ...
                    (TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment - ...
                    TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ.^2) / N);
            end
            results{di}.TransmittedDynamicMTOfXAndYAndSubregionHist = TransmittedDynamicMTOfXAndYAndSubregionHist;
        case 'TransmittedDynamicMTOfFxAndSubregionHist'
            TransmittedDynamicMTOfFxAndSubregionHist.Name = detector.Name;
            tempFx = detector.Fx;
            tempMTBins = detector.MTBins;
            tempZ = detector.Z;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            TransmittedDynamicMTOfFxAndSubregionHist.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));                     
            TransmittedDynamicMTOfFxAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedDynamicMTOfFxAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            TransmittedDynamicMTOfFxAndSubregionHist.Fx_Midpoints = TransmittedDynamicMTOfFxAndSubregionHist.Fx;
            TransmittedDynamicMTOfFxAndSubregionHist.MTBins_Midpoints = (TransmittedDynamicMTOfFxAndSubregionHist.MTBins(1:end-1) + TransmittedDynamicMTOfFxAndSubregionHist.MTBins(2:end))/2;
            TransmittedDynamicMTOfFxAndSubregionHist.Z_Midpoints = (TransmittedDynamicMTOfFxAndSubregionHist.Z(1:end-1) + TransmittedDynamicMTOfFxAndSubregionHist.Z(2:end))/2;
            TransmittedDynamicMTOfFxAndSubregionHist.SubregionIndices = tempSubregionIndices;
            nFx = numel(TransmittedDynamicMTOfFxAndSubregionHist.Fx);
            nMTBins = numel(TransmittedDynamicMTOfFxAndSubregionHist.MTBins) - 1;
            nZ = numel(TransmittedDynamicMTOfFxAndSubregionHist.Z) - 1;
            tempData = readBinaryData([datadir slash detector.Name], 2 * nMTBins * nFx);
            tempDataReshape = reshape(tempData, [2 * nMTBins, nFx]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.Mean = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            tempData = readBinaryData([datadir slash detector.Name '_FractionalMT'], ...
                2 * nFx * nMTBins * tempFractionalMTBinsLength);
            tempDataReshape = reshape(tempData, ...
                [2 * tempFractionalMTBinsLength, nMTBins, nFx]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT = ...
                complex(tempDataReshape(1:2:end,:,:), tempDataReshape(2:2:end,:,:));
            tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], 2 * nFx * nZ);
            tempDataReshape = reshape(tempData, [2 * nZ, nFx]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZ = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], 2 * nFx * nZ);
            tempDataReshape = reshape(tempData, [2 * nZ, nFx]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ = ...
                complex(tempDataReshape(1:2:end,:), tempDataReshape(2:2:end,:));
            TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions = ...
                reshape(readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ...
                2 * numel(tempSubregionIndices)), [2, numel(tempSubregionIndices)]); % read column major json binary
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file')
                tempData = readBinaryData([datadir slash detector.Name '_2'], ... 
                  (length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1) * 2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1),length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]);  
                TransmittedDynamicMTOfFxAndSubregionHist.SecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);    
                TransmittedDynamicMTOfFxAndSubregionHist.Stdev = sqrt((abs(TransmittedDynamicMTOfFxAndSubregionHist.SecondMoment) - ...
                    (abs(TransmittedDynamicMTOfFxAndSubregionHist.Mean) .* abs(TransmittedDynamicMTOfFxAndSubregionHist.Mean))) / (N));               
                % depth dependent output
                tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                  (length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1) * 2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1),length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]);  
                TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);   
                TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZStdev = sqrt((abs(TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment) - ...
                    (abs(TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZ) .* abs(TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZ))) / (N));               
                tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                  (length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1) * 2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1),length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]);  
                TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);    
                TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZStdev = sqrt((abs(TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment) - ...
                  (abs(TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ) .* abs(TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ))) / (N));               
            end
            results{di}.TransmittedDynamicMTOfFxAndSubregionHist = TransmittedDynamicMTOfFxAndSubregionHist;
        case 'ReflectedTimeOfRhoAndSubregionHist'
            ReflectedTimeOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            if (postProcessorResults)        
                tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                N = databaseInputJson.N;
            else   
                tempSubregionIndices = (1:1:length(json.TissueInput.Regions));
                N = json.N;
            end
            ReflectedTimeOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            ReflectedTimeOfRhoAndSubregionHist.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ReflectedTimeOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints = ...
                (ReflectedTimeOfRhoAndSubregionHist.Rho(1:end-1) + ...
                 ReflectedTimeOfRhoAndSubregionHist.Rho(2:end)) / 2;
            ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints = ...
                (ReflectedTimeOfRhoAndSubregionHist.Time(1:end-1) + ...
                 ReflectedTimeOfRhoAndSubregionHist.Time(2:end)) / 2;
            nRho = numel(ReflectedTimeOfRhoAndSubregionHist.Rho) - 1;
            nTime = numel(ReflectedTimeOfRhoAndSubregionHist.Time) - 1;
            nSubregions = numel(tempSubregionIndices);
            ReflectedTimeOfRhoAndSubregionHist.Mean = readBinaryData( ...
                [datadir slash detector.Name], nRho * nSubregions * nTime);
            ReflectedTimeOfRhoAndSubregionHist.Mean = reshape( ...
                ReflectedTimeOfRhoAndSubregionHist.Mean, [nTime, nSubregions, nRho]); % column major json binary
            ReflectedTimeOfRhoAndSubregionHist.FractionalTime = readBinaryData( ...
                [datadir slash detector.Name '_FractionalTime'], [nSubregions, nRho]);
            if detector.TallySecondMoment && exist([datadir slash detector.Name '_2'], 'file')
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], nRho * nSubregions * nTime);
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = reshape( ...
                    ReflectedTimeOfRhoAndSubregionHist.SecondMoment, ...
                    [nTime, nSubregions, nRho]); % column major json binary
                ReflectedTimeOfRhoAndSubregionHist.Stdev = sqrt( ...
                    (ReflectedTimeOfRhoAndSubregionHist.SecondMoment - ...
                     ReflectedTimeOfRhoAndSubregionHist.Mean.^2) / N);
            end
            results{di}.ReflectedTimeOfRhoAndSubregionHist = ReflectedTimeOfRhoAndSubregionHist;
        case 'pMCATotal'
            pMCAtotal.Name = detector.Name;           
            pMCATotal_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            pMCATotal.Mean = pMCATotal_txt.Mean;              
            pMCATotal.SecondMoment = pMCATotal_txt.SecondMoment; 
            pMCATotal.Stdev = sqrt( ...
                (pMCATotal.SecondMoment - pMCATotal.Mean.^2) / ...
                databaseInputJson.N);
            results{di}.pMCATotal = pMCATotal;
        case 'pMCROfRho'
            pMCROfRho.Name = detector.Name;
            tempRho = detector.Rho;
            pMCROfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRho.Rho_Midpoints = (pMCROfRho.Rho(1:end-1) + pMCROfRho.Rho(2:end))/2;
            pMCROfRho.Mean = readBinaryData([datadir slash detector.Name],length(pMCROfRho.Rho)-1);              
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                rhoCount = numel(pMCROfRho.Rho) - 1;
                pMCROfRho.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], rhoCount);
                pMCROfRho.Stdev = sqrt( ...
                    (pMCROfRho.SecondMoment - pMCROfRho.Mean.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfRho = pMCROfRho;
        case 'pMCROfRhoRecessed'
            pMCROfRhoRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            pMCROfRhoRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoRecessed.Rho_Midpoints = (pMCROfRhoRecessed.Rho(1:end-1) + pMCROfRhoRecessed.Rho(2:end))/2;
            rhoCount = numel(pMCROfRhoRecessed.Rho) - 1;
            pMCROfRhoRecessed.Mean = readBinaryData( ...
                [datadir slash detector.Name], rhoCount);
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                pMCROfRhoRecessed.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], rhoCount);
                pMCROfRhoRecessed.Stdev = sqrt( ...
                    (pMCROfRhoRecessed.SecondMoment - ...
                     pMCROfRhoRecessed.Mean.^2) / databaseInputJson.N);
            end
            results{di}.pMCROfRhoRecessed = pMCROfRhoRecessed;
        case 'dMCdROfRhodMua'
            dMCdROfRhodMua.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhodMua.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhodMua.Rho_Midpoints = (dMCdROfRhodMua.Rho(1:end-1) + dMCdROfRhodMua.Rho(2:end))/2;
            rhoCount = numel(dMCdROfRhodMua.Rho) - 1;
            dMCdROfRhodMua.Mean = readBinaryData( ...
                [datadir slash detector.Name], rhoCount);
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                dMCdROfRhodMua.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], rhoCount);
                dMCdROfRhodMua.Stdev = sqrt( ...
                    (dMCdROfRhodMua.SecondMoment - ...
                     dMCdROfRhodMua.Mean.^2) / databaseInputJson.N);
            end
            results{di}.dMCdROfRhodMua = dMCdROfRhodMua;
        case 'dMCdROfRhodMus'
            dMCdROfRhodMus.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhodMus.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhodMus.Rho_Midpoints = (dMCdROfRhoAndTimedMus.Rho(1:end-1) + dMCdROfRhodMus.Rho(2:end))/2;
            rhoCount = numel(dMCdROfRhodMus.Rho) - 1;
            dMCdROfRhodMus.Mean = readBinaryData( ...
                [datadir slash detector.Name], rhoCount);
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                dMCdROfRhodMus.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], rhoCount);
                dMCdROfRhodMus.Stdev = sqrt( ...
                    (dMCdROfRhodMus.SecondMoment - ...
                     dMCdROfRhodMus.Mean.^2) / databaseInputJson.N);
            end
            results{di}.dMCdROfRhodMus = dMCdROfRhodMus;
        case 'pMCROfRhoAndTime'
            pMCROfRhoAndTime.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            pMCROfRhoAndTime.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfRhoAndTime.Rho_Midpoints = (pMCROfRhoAndTime.Rho(1:end-1) + pMCROfRhoAndTime.Rho(2:end))/2;
            pMCROfRhoAndTime.Time_Midpoints = (pMCROfRhoAndTime.Time(1:end-1) + pMCROfRhoAndTime.Time(2:end))/2;
            dataSize = [numel(pMCROfRhoAndTime.Time) - 1, ...
                        numel(pMCROfRhoAndTime.Rho) - 1];
            pMCROfRhoAndTime.Mean = readBinaryData( ...
                [datadir slash detector.Name], dataSize); % read column major json binary
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                pMCROfRhoAndTime.SecondMoment = readBinaryData( ...
                    [datadir slash detector.Name '_2'], dataSize);
                pMCROfRhoAndTime.Stdev = sqrt( ...
                    (pMCROfRhoAndTime.SecondMoment - ...
                     pMCROfRhoAndTime.Mean.^2) / databaseInputJson.N);
            end
            results{di}.pMCROfRhoAndTime = pMCROfRhoAndTime;
        case 'pMCROfRhoAndTimeRecessed'
            pMCROfRhoAndTimeRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            pMCROfRhoAndTimeRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoAndTimeRecessed.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfRhoAndTimeRecessed.Rho_Midpoints = (pMCROfRhoAndTimeRecessed.Rho(1:end-1) + pMCROfRhoAndTimeRecessed.Rho(2:end))/2;
            pMCROfRhoAndTimeRecessed.Time_Midpoints = (pMCROfRhoAndTimeRecessed.Time(1:end-1) + pMCROfRhoAndTimeRecessed.Time(2:end))/2;
            dataSize = [numel(pMCROfRhoAndTimeRecessed.Time) - 1, ...
                        numel(pMCROfRhoAndTimeRecessed.Rho) - 1];
            dataFile = [datadir slash detector.Name];
            pMCROfRhoAndTimeRecessed.Mean = readBinaryData( ...
                dataFile, dataSize); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfRhoAndTimeRecessed.SecondMoment = readBinaryData( ...
                    [dataFile '_2'], dataSize);
                pMCROfRhoAndTimeRecessed.Stdev = sqrt( ...
                    (pMCROfRhoAndTimeRecessed.SecondMoment - ...
                     pMCROfRhoAndTimeRecessed.Mean.^2) / databaseInputJson.N);
            end
            results{di}.pMCROfRhoAndTimeRecessed = pMCROfRhoAndTimeRecessed;
        case 'pMCROfRhoAndTimeAndSubregion'
            pMCROfRhoAndTimeAndSubregion.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            tempNumberOfRegions = size(detector.PerturbedOps,2); % get number of tissue regions indirectly
            pMCROfRhoAndTimeAndSubregion.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoAndTimeAndSubregion.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfRhoAndTimeAndSubregion.Rho_Midpoints = (pMCROfRhoAndTimeAndSubregion.Rho(1:end-1) + pMCROfRhoAndTimeAndSubregion.Rho(2:end))/2;
            pMCROfRhoAndTimeAndSubregion.Time_Midpoints = (pMCROfRhoAndTimeAndSubregion.Time(1:end-1) + pMCROfRhoAndTimeAndSubregion.Time(2:end))/2;
            pMCROfXAndYAndTimeAndSubregion.NumberOfRegions = tempNumberOfRegions;
            nTime = numel(pMCROfRhoAndTimeAndSubregion.Time) - 1;
            nRho = numel(pMCROfRhoAndTimeAndSubregion.Rho) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = [tempNumberOfRegions * nTime * nRho];
            pMCROfRhoAndTimeAndSubregion.Mean = readBinaryData( ...
                dataFile, dataSize); % read column major json binary
            pMCROfRhoAndTimeAndSubregion.Mean = reshape( ...
                pMCROfRhoAndTimeAndSubregion.Mean, ...
                [tempNumberOfRegions, nTime, nRho]); % reshape
            pMCROfRhoAndTimeAndSubregion.ROfRho = readBinaryData( ...
                [dataFile '_ROfRho'], nRho); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfRhoAndTimeAndSubregion.SecondMoment = reshape( ...
                    readBinaryData([dataFile '_2'], dataSize), ...
                    [tempNumberOfRegions, nTime, nRho]); % read column major json binary
                pMCROfRhoAndTimeAndSubregion.Stdev = sqrt( ...
                    (pMCROfRhoAndTimeAndSubregion.SecondMoment - ...
                     pMCROfRhoAndTimeAndSubregion.Mean.^2) / ...
                    databaseInputJson.N);
                pMCROfRhoAndTimeAndSubregion.ROfRhoSecondMoment = ...
                    readBinaryData([dataFile '_ROfRho_2'], nRho); % read column major json binary
                pMCROfRhoAndTimeAndSubregion.ROfRhoStdev = sqrt( ...
                    (pMCROfRhoAndTimeAndSubregion.ROfRhoSecondMoment - ...
                     pMCROfRhoAndTimeAndSubregion.ROfRho.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfRhoAndTimeAndSubregion = pMCROfRhoAndTimeAndSubregion;
        case 'pMCROfRhoAndTimeAndSubregionRecessed'
            pMCROfRhoAndTimeAndSubregionRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            tempNumberOfRegions = size(detector.PerturbedOps,2); % get number of tissue regions indirectly
            pMCROfRhoAndTimeAndSubregionRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoAndTimeAndSubregionRecessed.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfRhoAndTimeAndSubregionRecessed.Rho_Midpoints = (pMCROfRhoAndTimeAndSubregionRecessed.Rho(1:end-1) + pMCROfRhoAndTimeAndSubregionRecessed.Rho(2:end))/2;
            pMCROfRhoAndTimeAndSubregionRecessed.Time_Midpoints = (pMCROfRhoAndTimeAndSubregionRecessed.Time(1:end-1) + pMCROfRhoAndTimeAndSubregionRecessed.Time(2:end))/2;
            pMCROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions = tempNumberOfRegions;
            nTime = numel(pMCROfRhoAndTimeAndSubregionRecessed.Time) - 1;
            nRho = numel(pMCROfRhoAndTimeAndSubregionRecessed.Rho) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = tempNumberOfRegions * nTime * nRho;
            pMCROfRhoAndTimeAndSubregionRecessed.Mean = reshape( ...
                readBinaryData(dataFile, dataSize), ...
                [tempNumberOfRegions, nTime, nRho]); % read column major json binary
            pMCROfRhoAndTimeAndSubregionRecessed.ROfRho = readBinaryData( ...
                [dataFile '_ROfRho'], nRho); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfRhoAndTimeAndSubregionRecessed.SecondMoment = reshape( ...
                    readBinaryData([dataFile '_2'], dataSize), ...
                    [tempNumberOfRegions, nTime, nRho]); % read column major json binary
                pMCROfRhoAndTimeAndSubregionRecessed.Stdev = sqrt( ...
                    (pMCROfRhoAndTimeAndSubregionRecessed.SecondMoment - ...
                     pMCROfRhoAndTimeAndSubregionRecessed.Mean.^2) / ...
                    databaseInputJson.N);
                pMCROfRhoAndTimeAndSubregionRecessed.ROfRhoSecondMoment = ...
                    readBinaryData([dataFile '_ROfRho_2'], nRho); % read column major json binary
                pMCROfRhoAndTimeAndSubregionRecessed.ROfRhoStdev = sqrt( ...
                    (pMCROfRhoAndTimeAndSubregionRecessed.ROfRhoSecondMoment - ...
                     pMCROfRhoAndTimeAndSubregionRecessed.ROfRho.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfRhoAndTimeAndSubregionRecessed = pMCROfRhoAndTimeAndSubregionRecessed;
        case 'dMCdROfRhoAndTimedMua'
            dMCdROfRhoAndTimedMua.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhoAndTimedMua.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhoAndTimedMua.Rho_Midpoints = (dMCdROfRhoAndTimedMua.Rho(1:end-1) + dMCdROfRhoAndTimedMua.Rho(2:end))/2;
            tempTime = detector.Time;
            dMCdROfRhoAndTimedMua.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            dMCdROfRhoAndTimedMua.Time_Midpoints = (dMCdROfRhoAndTimedMua.Time(1:end-1) + dMCdROfRhoAndTimedMua.Time(2:end))/2;
            nTime = numel(dMCdROfRhoAndTimedMua.Time) - 1;
            nRho = numel(dMCdROfRhoAndTimedMua.Rho) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = [nTime, nRho];
            dMCdROfRhoAndTimedMua.Mean = readBinaryData(dataFile, dataSize);
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                dMCdROfRhoAndTimedMua.SecondMoment = ...
                    readBinaryData([dataFile '_2'], dataSize);
                dMCdROfRhoAndTimedMua.Stdev = sqrt( ...
                    (dMCdROfRhoAndTimedMua.SecondMoment - ...
                     dMCdROfRhoAndTimedMua.Mean.^2) / databaseInputJson.N);
            end
            results{di}.dMCdROfRhoAndTimedMua = dMCdROfRhoAndTimedMua;
        case 'dMCdROfRhoAndTimedMus'
            dMCdROfRhoAndTimedMus.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhoAndTimedMus.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhoAndTimedMus.Rho_Midpoints = (dMCdROfRhoAndTimedMus.Rho(1:end-1) + dMCdROfRhoAndTimedMus.Rho(2:end))/2;
            tempTime = detector.Time;
            dMCdROfRhoAndTimedMus.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            dMCdROfRhoAndTimedMus.Time_Midpoints = (dMCdROfRhoAndTimedMus.Time(1:end-1) + dMCdROfRhoAndTimedMus.Time(2:end))/2;
            nTime = numel(dMCdROfRhoAndTimedMus.Time) - 1;
            nRho = numel(dMCdROfRhoAndTimedMus.Rho) - 1;
            dataFile = [datadir slash detector.Name];
            dataSize = [nTime, nRho];
            dMCdROfRhoAndTimedMus.Mean = readBinaryData(dataFile, dataSize);
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                dMCdROfRhoAndTimedMus.SecondMoment = ...
                    readBinaryData([dataFile '_2'], dataSize);
                dMCdROfRhoAndTimedMus.Stdev = sqrt( ...
                    (dMCdROfRhoAndTimedMus.SecondMoment - ...
                     dMCdROfRhoAndTimedMus.Mean.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.dMCdROfRhoAndTimedMus = dMCdROfRhoAndTimedMus;
        case 'pMCROfXAndY'
            pMCROfXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            pMCROfXAndY.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            pMCROfXAndY.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            pMCROfXAndY.X_Midpoints = (pMCROfXAndY.X(1:end-1) + pMCROfXAndY.X(2:end))/2;
            pMCROfXAndY.Y_Midpoints = (pMCROfXAndY.Y(1:end-1) + pMCROfXAndY.Y(2:end))/2;
            dataFile = [datadir slash detector.Name];
            dataSize = [numel(pMCROfXAndY.Y) - 1, numel(pMCROfXAndY.X) - 1];
            pMCROfXAndY.Mean = readBinaryData(dataFile, dataSize); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfXAndY.SecondMoment = readBinaryData([dataFile '_2'], dataSize);
                pMCROfXAndY.Stdev = sqrt( ...
                    (pMCROfXAndY.SecondMoment - pMCROfXAndY.Mean.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfXAndY = pMCROfXAndY;
        case 'pMCROfXAndYAndTimeAndSubregion'
            pMCROfXAndYAndTimeAndSubregion.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            tempNumberOfRegions = size(detector.PerturbedOps,2); % get number of tissue regions indirectly
            pMCROfXAndYAndTimeAndSubregion.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            pMCROfXAndYAndTimeAndSubregion.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            pMCROfXAndYAndTimeAndSubregion.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfXAndYAndTimeAndSubregion.X_Midpoints = (pMCROfXAndYAndTimeAndSubregion.X(1:end-1) + pMCROfXAndYAndTimeAndSubregion.X(2:end))/2;
            pMCROfXAndYAndTimeAndSubregion.Y_Midpoints = (pMCROfXAndYAndTimeAndSubregion.Y(1:end-1) + pMCROfXAndYAndTimeAndSubregion.Y(2:end))/2;
            pMCROfXAndYAndTimeAndSubregion.Time_Midpoints = (pMCROfXAndYAndTimeAndSubregion.Time(1:end-1) + pMCROfXAndYAndTimeAndSubregion.Time(2:end))/2;
            pMCROfXAndYAndTimeAndSubregion.NumberOfRegions = tempNumberOfRegions;
            nX = numel(pMCROfXAndYAndTimeAndSubregion.X) - 1;
            nY = numel(pMCROfXAndYAndTimeAndSubregion.Y) - 1;
            nTime = numel(pMCROfXAndYAndTimeAndSubregion.Time) - 1;
            nValues = tempNumberOfRegions * nTime * nY * nX;
            dataFile = [datadir slash detector.Name];
            pMCROfXAndYAndTimeAndSubregion.Mean = readBinaryData(dataFile, nValues); % read column major json binary
            pMCROfXAndYAndTimeAndSubregion.Mean = reshape( ...
                pMCROfXAndYAndTimeAndSubregion.Mean, ...
                [tempNumberOfRegions, nTime, nY, nX]); % read column major json binary
            pMCROfXAndYAndTimeAndSubregion.ROfXAndY = readBinaryData( ...
                [dataFile '_ROfXAndY'], [nY, nX]); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfXAndYAndTimeAndSubregion.SecondMoment = reshape( ...
                    readBinaryData([dataFile '_2'], nValues), ...
                    [tempNumberOfRegions, nTime, nY, nX]); % read column major json binary
                pMCROfXAndYAndTimeAndSubregion.Stdev = sqrt( ...
                    (pMCROfXAndYAndTimeAndSubregion.SecondMoment - ...
                    pMCROfXAndYAndTimeAndSubregion.Mean.^2) / databaseInputJson.N);
                pMCROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment = ...
                    readBinaryData([dataFile '_ROfXAndY_2'], [nY, nX]); % read column major json binary
                pMCROfXAndYAndTimeAndSubregion.ROfXAndYStdev = sqrt( ...
                    (pMCROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment - ...
                    pMCROfXAndYAndTimeAndSubregion.ROfXAndY.^2) / databaseInputJson.N);
            end
            results{di}.pMCROfXAndYAndTimeAndSubregion = pMCROfXAndYAndTimeAndSubregion;
        case 'pMCROfXAndYAndTimeAndSubregionRecessed'
            pMCROfXAndYAndTimeAndSubregionRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            tempNumberOfRegions = size(detector.PerturbedOps,2); % get number of tissue regions indirectly
            pMCROfXAndYAndTimeAndSubregionRecessed.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            pMCROfXAndYAndTimeAndSubregionRecessed.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            pMCROfXAndYAndTimeAndSubregionRecessed.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            pMCROfXAndYAndTimeAndSubregionRecessed.X_Midpoints = (pMCROfXAndYAndTimeAndSubregionRecessed.X(1:end-1) + pMCROfXAndYAndTimeAndSubregionRecessed.X(2:end))/2;
            pMCROfXAndYAndTimeAndSubregionRecessed.Y_Midpoints = (pMCROfXAndYAndTimeAndSubregionRecessed.Y(1:end-1) + pMCROfXAndYAndTimeAndSubregionRecessed.Y(2:end))/2;
            pMCROfXAndYAndTimeAndSubregionRecessed.Time_Midpoints = (pMCROfXAndYAndTimeAndSubregionRecessed.Time(1:end-1) + pMCROfXAndYAndTimeAndSubregionRecessed.Time(2:end))/2;
            pMCROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions = tempNumberOfRegions;
            nTime = numel(pMCROfXAndYAndTimeAndSubregionRecessed.Time) - 1;
            nY = numel(pMCROfXAndYAndTimeAndSubregionRecessed.Y) - 1;
            nX = numel(pMCROfXAndYAndTimeAndSubregionRecessed.X) - 1;
            nValues = tempNumberOfRegions * nTime * nY * nX;
            dataFile = [datadir slash detector.Name];
            pMCROfXAndYAndTimeAndSubregionRecessed.Mean = ...
                reshape(readBinaryData(dataFile, nValues), ...
                [tempNumberOfRegions, nTime, nY, nX]); % read column major json binary
            pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY = ...
                readBinaryData([dataFile '_ROfXAndY'], [nY, nX]); % read column major json binary
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment = ...
                    reshape(readBinaryData([dataFile '_2'], nValues), ...
                    [tempNumberOfRegions, nTime, nY, nX]); % read column major json binary
                pMCROfXAndYAndTimeAndSubregionRecessed.Stdev = sqrt( ...
                    (pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment - ...
                    pMCROfXAndYAndTimeAndSubregionRecessed.Mean.^2) / ...
                    databaseInputJson.N);
                pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment = ...
                    readBinaryData([dataFile '_ROfXAndY_2'], [nY, nX]); % read column major json binary
                pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYStdev = sqrt( ...
                    (pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment - ...
                    pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY.^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfXAndYAndTimeAndSubregionRecessed = pMCROfXAndYAndTimeAndSubregionRecessed;
        case 'pMCROfFx'
            pMCROfFx.Name = detector.Name;
            tempFx = detector.Fx;
            pMCROfFx.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            pMCROfFx.Fx_Midpoints = pMCROfFx.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(pMCROfFx.Fx)); 
            pMCROfFx.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);  
            pMCROfFx.Amplitude = abs(pMCROfFx.Mean);
            pMCROfFx.Phase = -angle(pMCROfFx.Mean);
            dataFile = [datadir slash detector.Name];
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                tempData = readBinaryData([dataFile '_2'], 2*numel(pMCROfFx.Fx));
                pMCROfFx.SecondMoment = tempData(1:2:end) + 1i*tempData(2:2:end);
                pMCROfFx.Stdev = sqrt((pMCROfFx.SecondMoment + abs(pMCROfFx.Mean).^2) / ...
                    databaseInputJson.N);
            end
            results{di}.pMCROfFx = pMCROfFx;
        case 'pMCROfFxAndTime'
            pMCROfFxAndTime.Name = detector.Name;
            tempFx = detector.Fx;
            pMCROfFxAndTime.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            pMCROfFxAndTime.Fx_Midpoints = pMCROfFxAndTime.Fx;
            tempTime = detector.Time;
            pMCROfFxAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (TempTime.Count));
            pMCROfFxAndTime.Time_Midpoints = pMCROfFxAndTime;
            tempData = readBinaryData([datadir slash detector.Name],2*length(pMCROfFxAndTime.Fx)); 
            pMCROfFxAndTime.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);  
            pMCROfFxAndTime.Amplitude = abs(pMCROfFxAndTime.Mean);
            pMCROfFxAndTime.Phase = -angle(pMCROfFxAndTime.Mean);
            dataFile = [datadir slash detector.Name];
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                tempData = readBinaryData([dataFile '_2'], 2 * numel(pMCROfFxAndTime.Fx));
                pMCROfFxAndTime.SecondMoment = tempData(1:2:end) + ...
                    1i * tempData(2:2:end);
                pMCROfFxAndTime.Stdev = sqrt( ...
                    (pMCROfFxAndTime.SecondMoment + ...
                    abs(pMCROfFxAndTime.Mean).^2) / databaseInputJson.N);
            end
            results{di}.pMCROfFxAndTime = pMCROfFxAndTime;
        case 'dMCdROfFxdMua'
            dMCdROfFxdMua.Name = detector.Name;
            tempFx = detector.Fx;
            dMCdROfFxdMua.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            dMCdROfFxdMua.Fx_Midpoints = dMCdROfFxdMua.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(dMCdROfFxdMua.Fx)); 
            dMCdROfFxdMua.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);  
            dMCdROfFxdMua.Amplitude = abs(dMCdROfFxdMua.Mean);
            dMCdROfFxdMua.Phase = -angle(dMCdROfFxdMua.Mean);
            dataFile = [datadir slash detector.Name];
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                tempData = readBinaryData([dataFile '_2'], 2 * numel(dMCdROfFxdMua.Fx));
                dMCdROfFxdMua.SecondMoment = tempData(1:2:end) + ...
                    1i * tempData(2:2:end);
                dMCdROfFxdMua.Stdev = sqrt( ...
                    (dMCdROfFxdMua.SecondMoment + ...
                    abs(dMCdROfFxdMua.Mean).^2) / databaseInputJson.N);
            end
            results{di}.dMCdROfFxdMua = dMCdROfFxdMua;
        case 'dMCdROfFxdMus'
            dMCdROfFxdMus.Name = detector.Name;
            tempFx = detector.Fx;
            dMCdROfFxdMus.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            dMCdROfFxdMus.Fx_Midpoints = dMCdROfFxdMus.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(dMCdROfFxdMus.Fx)); 
            dMCdROfFxdMus.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);   
            dMCdROfFxdMus.Amplitude = abs(dMCdROfFxdMus.Mean);
            dMCdROfFxdMus.Phase = -angle(dMCdROfFxdMus.Mean);               
            if detector.TallySecondMoment && ...
                    exist([datadir slash detector.Name '_2'], 'file')
                dataFile = [datadir slash detector.Name '_2'];
                tempData = readBinaryData(dataFile, 2 * numel(dMCdROfFxdMus.Fx));
                dMCdROfFxdMus.SecondMoment = tempData(1:2:end) + ...
                    1i * tempData(2:2:end);
                dMCdROfFxdMus.Stdev = sqrt( ...
                    (dMCdROfFxdMus.SecondMoment + ...
                    abs(dMCdROfFxdMus.Mean).^2) / databaseInputJson.N);
            end
            results{di}.dMCdROfFxdMus = dMCdROfFxdMus;
        case 'pMCTOfRho'
            pMCTOfRho.Name = detector.Name;
            tempRho = detector.Rho;
            pMCTOfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCTOfRho.Rho_Midpoints = (pMCTOfRho.Rho(1:end-1) + pMCTOfRho.Rho(2:end))/2;
            pMCTOfRho.Mean = readBinaryData([datadir slash detector.Name],length(pMCTOfRho.Rho)-1);              
            dataFile = [datadir slash detector.Name];
            nRho = numel(pMCTOfRho.Rho) - 1;
            if detector.TallySecondMoment && exist([dataFile '_2'], 'file')
                pMCTOfRho.SecondMoment = readBinaryData([dataFile '_2'], nRho);
                pMCTOfRho.Stdev = sqrt((pMCTOfRho.SecondMoment - ...
                    pMCTOfRho.Mean.^2) / databaseInputJson.N);
            end
            results{di}.pMCTOfRho = pMCTOfRho;
    end % switch on TallyType
end % for all detectors

function json_parsed = readAndParseJson(filename)
json_parsed = loadjson(filename);

