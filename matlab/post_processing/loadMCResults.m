function results = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

json = readAndParseJson([datadir slash dataname '.txt']);

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
            SurfaceFiber_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            SurfaceFiber.Mean = SurfaceFiber_txt.Mean;              
            SurfaceFiber.SecondMoment = SurfaceFiber_txt.SecondMoment;
            SurfaceFiber.Stdev = sqrt((SurfaceFiber.SecondMoment - (SurfaceFiber.Mean .* SurfaceFiber.Mean)) / (json.N)); 
            results{di}.SurfaceFiber = SurfaceFiber;
        case 'SlantedRecessedFiber'
            SlantedRecessedFiber.Name = detector.Name;
            SlantedRecessedFiber_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            SlantedRecessedFiber.Mean = SlantedRecessedFiber_txt.Mean;              
            SlantedRecessedFiber.SecondMoment = SlantedRecessedFiber_txt.SecondMoment;
            SlantedRecessedFiber.Stdev = sqrt((SlantedRecessedFiber.SecondMoment - (SlantedRecessedFiber.Mean .* SlantedRecessedFiber.Mean)) / (json.N)); 
            results{di}.SlantedRecessedFiber = SlantedRecessedFiber;
        case 'RDiffuse'
            RDiffuse.Name = detector.Name;
            RDiffuse_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            RDiffuse.Mean = RDiffuse_txt.Mean;              
            RDiffuse.SecondMoment = RDiffuse_txt.SecondMoment;
            RDiffuse.Stdev = sqrt((RDiffuse.SecondMoment - (RDiffuse.Mean .* RDiffuse.Mean)) / json.N); 
            results{di}.RDiffuse = RDiffuse;
        case 'RSpecular'
            RSpecular.Name = detector.Name;
            RSpecular_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            RSpecular.Mean = RSpecular_txt.Mean;              
            RSpecular.SecondMoment = RSpecular_txt.SecondMoment;
            RSpecular.Stdev = sqrt((RSpecular.SecondMoment - (RSpecular.Mean .* RSpecular.Mean)) / json.N); 
            results{di}.RSpecular = RSpecular;
        case 'ROfRho'
            ROfRho.Name = detector.Name;
            tempRho = detector.Rho;
            ROfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + ROfRho.Rho(2:end))/2;
            ROfRho.Mean = readBinaryData([datadir slash detector.Name],length(ROfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(ROfRho.Rho)-1);
                ROfRho.Stdev = sqrt((ROfRho.SecondMoment - (ROfRho.Mean .* ROfRho.Mean)) / json.N);
            end
            results{di}.ROfRho = ROfRho;
        case 'ROfRhoRecessed'
            ROfRhoRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            ROfRhoRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoRecessed.Rho_Midpoints = (ROfRhoRecessed.Rho(1:end-1) + ROfRhoRecessed.Rho(2:end))/2;
            ROfRhoRecessed.Mean = readBinaryData([datadir slash detector.Name],length(ROfRhoRecessed.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRhoRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(ROfRhoRecessed.Rho)-1);
                ROfRhoRecessed.Stdev = sqrt((ROfRhoRecessed.SecondMoment - (ROfRhoRecessed.Mean .* ROfRhoRecessed.Mean)) / json.N);
            end
            results{di}.ROfRhoRecessed = ROfRhoRecessed;
        case 'ROfAngle'
            ROfAngle.Name = detector.Name;
            tempAngle = detector.Angle;
            ROfAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            ROfAngle.Angle_Midpoints = (ROfAngle.Angle(1:end-1) + ROfAngle.Angle(2:end))/2;
            ROfAngle.Mean = readBinaryData([datadir slash detector.Name],length(ROfAngle.Angle)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(ROfAngle.Angle)-1);
                ROfAngle.Stdev = sqrt((ROfAngle.SecondMoment - (ROfAngle.Mean .* ROfAngle.Mean)) / json.N);
            end
            results{di}.ROfAngle = ROfAngle;

        case 'ROfRhoAndTime'
            ROfRhoAndTime.Name = detector.Name;
            tempRho = detector.Rho;
            tempTime = detector.Time;
            ROfRhoAndTime.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfRhoAndTime.Rho_Midpoints = (ROfRhoAndTime.Rho(1:end-1) + ROfRhoAndTime.Rho(2:end))/2;
            ROfRhoAndTime.Time_Midpoints = (ROfRhoAndTime.Time(1:end-1) + ROfRhoAndTime.Time(2:end))/2;
            ROfRhoAndTime.Mean = readBinaryData([datadir slash detector.Name],[length(ROfRhoAndTime.Time)-1,length(ROfRhoAndTime.Rho)-1]); % read column major json binary             
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRhoAndTime.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfRhoAndTime.Time)-1,length(ROfRhoAndTime.Rho)-1]);
                ROfRhoAndTime.Stdev = sqrt((ROfRhoAndTime.SecondMoment - (ROfRhoAndTime.Mean .* ROfRhoAndTime.Mean)) / json.N);
            end
            results{di}.ROfRhoAndTime = ROfRhoAndTime;
        case 'ROfRhoAndMaxDepth'
            ROfRhoAndMaxDepth.Name = detector.Name;
            tempRho = detector.Rho;
            tempMaxDepth = detector.MaxDepth;
            ROfRhoAndMaxDepth.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoAndMaxDepth.MaxDepth = linspace((tempMaxDepth.Start), (tempMaxDepth.Stop), (tempMaxDepth.Count));
            ROfRhoAndMaxDepth.Rho_Midpoints = (ROfRhoAndMaxDepth.Rho(1:end-1) + ROfRhoAndMaxDepth.Rho(2:end))/2;
            ROfRhoAndMaxDepth.MaxDepth_Midpoints = (ROfRhoAndMaxDepth.MaxDepth(1:end-1) + ROfRhoAndMaxDepth.MaxDepth(2:end))/2;
            ROfRhoAndMaxDepth.Mean = readBinaryData([datadir slash detector.Name],[length(ROfRhoAndMaxDepth.MaxDepth)-1,length(ROfRhoAndMaxDepth.Rho)-1]); % read column major json binary             
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRhoAndMaxDepth.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfRhoAndMaxDepth.MaxDepth)-1,length(ROfRhoAndMaxDepth.Rho)-1]);
                ROfRhoAndMaxDepth.Stdev = sqrt((ROfRhoAndMaxDepth.SecondMoment - (ROfRhoAndMaxDepth.Mean .* ROfRhoAndMaxDepth.Mean)) / json.N);
            end
            results{di}.ROfRhoAndMaxDepth = ROfRhoAndMaxDepth;
        case 'ROfRhoAndMaxDepthRecessed'
            ROfRhoAndMaxDepthRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            tempMaxDepth = detector.MaxDepth;
            ROfRhoAndMaxDepthRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoAndMaxDepthRecessed.MaxDepth = linspace((tempMaxDepth.Start), (tempMaxDepth.Stop), (tempMaxDepth.Count));
            ROfRhoAndMaxDepthRecessed.Rho_Midpoints = (ROfRhoAndMaxDepthRecessed.Rho(1:end-1) + ROfRhoAndMaxDepthRecessed.Rho(2:end))/2;
            ROfRhoAndMaxDepthRecessed.MaxDepth_Midpoints = (ROfRhoAndMaxDepthRecessed.MaxDepth(1:end-1) + ROfRhoAndMaxDepthRecessed.MaxDepth(2:end))/2;
            ROfRhoAndMaxDepthRecessed.Mean = readBinaryData([datadir slash detector.Name],[length(ROfRhoAndMaxDepthRecessed.MaxDepth)-1,length(ROfRhoAndMaxDepthRecessed.Rho)-1]); % read column major json binary             
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRhoAndMaxDepthRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfRhoAndMaxDepthRecessed.MaxDepth)-1,length(ROfRhoAndMaxDepthRecessed.Rho)-1]);
                ROfRhoAndMaxDepthRecessed.Stdev = sqrt((ROfRhoAndMaxDepthRecessed.SecondMoment - (ROfRhoAndMaxDepthRecessed.Mean .* ROfRhoAndMaxDepthRecessed.Mean)) / json.N);
            end
            results{di}.ROfRhoAndMaxDepthRecessed = ROfRhoAndMaxDepthRecessed;
        case 'ROfRhoAndAngle'
            ROfRhoAndAngle.Name = detector.Name;
            tempRho = detector.Rho;
            tempAngle = detector.Angle;
            ROfRhoAndAngle.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoAndAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            ROfRhoAndAngle.Rho_Midpoints = (ROfRhoAndAngle.Rho(1:end-1) + ROfRhoAndAngle.Rho(2:end))/2;
            ROfRhoAndAngle.Angle_Midpoints = (ROfRhoAndAngle.Angle(1:end-1) + ROfRhoAndAngle.Angle(2:end))/2;
            ROfRhoAndAngle.Mean = readBinaryData([datadir slash detector.Name],[length(ROfRhoAndAngle.Angle)-1,length(ROfRhoAndAngle.Rho)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRhoAndAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfRhoAndAngle.Angle)-1,length(ROfRhoAndAngle.Rho)-1]); 
                ROfRhoAndAngle.Stdev = sqrt((ROfRhoAndAngle.SecondMoment - (ROfRhoAndAngle.Mean .* ROfRhoAndAngle.Mean)) / json.N); 
            end
            results{di}.ROfRhoAndAngle = ROfRhoAndAngle;
        case 'ROfRhoAndOmega'
            ROfRhoAndOmega.Name = detector.Name;
            tempRho = detector.Rho;
            tempOmega = detector.Omega;
            ROfRhoAndOmega.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRhoAndOmega.Omega = linspace((tempOmega.Start), (tempOmega.Stop), (tempOmega.Count));
            ROfRhoAndOmega.Omega_Midpoints = ROfRhoAndOmega.Omega; % omega is not binned, value is used
            ROfRhoAndOmega.Rho_Midpoints = (ROfRhoAndOmega.Rho(1:end-1) + ROfRhoAndOmega.Rho(2:end))/2;
            tempData = readBinaryData([datadir slash detector.Name],[2*length(ROfRhoAndOmega.Omega),(length(ROfRhoAndOmega.Rho)-1)]); % read column major json binary
            ROfRhoAndOmega.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            ROfRhoAndOmega.Amplitude = abs(ROfRhoAndOmega.Mean);
            ROfRhoAndOmega.Phase = -angle(ROfRhoAndOmega.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],[2*length(ROfRhoAndOmega.Omega),(length(ROfRhoAndOmega.Rho)-1)]); % 2x omega for re and im  
                ROfRhoAndOmega.SecondMoment =  tempData(1:2:end,:) + tempData(2:2:end,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfRhoAndOmega.Stdev = sqrt((ROfRhoAndOmega.SecondMoment - real(ROfRhoAndOmega.Mean) .* real(ROfRhoAndOmega.Mean) ...
                                                                         - imag(ROfRhoAndOmega.Mean) .* imag(ROfRhoAndOmega.Mean)) / json.N);
            end            
            results{di}.ROfRhoAndOmega = ROfRhoAndOmega;
         case 'ROfXAndY'
            ROfXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            ROfXAndY.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndY.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndY.X_Midpoints = (ROfXAndY.X(1:end-1) + ROfXAndY.X(2:end))/2;
            ROfXAndY.Y_Midpoints = (ROfXAndY.Y(1:end-1) + ROfXAndY.Y(2:end))/2;
            ROfXAndY.Mean = readBinaryData([datadir slash detector.Name],[length(ROfXAndY.Y)-1,length(ROfXAndY.X)-1]);  % read column major json binary  
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndY.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfXAndY.Y)-1,length(ROfXAndY.X)-1]); 
                ROfXAndY.Stdev = sqrt((ROfXAndY.SecondMoment - (ROfXAndY.Mean .* ROfXAndY.Mean)) / json.N); 
            end      
            results{di}.ROfXAndY = ROfXAndY;
        case 'ROfXAndYRecessed'
            ROfXAndYRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            ROfXAndYRecessed.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYRecessed.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYRecessed.X_Midpoints = (ROfXAndYRecessed.X(1:end-1) + ROfXAndYRecessed.X(2:end))/2;
            ROfXAndYRecessed.Y_Midpoints = (ROfXAndYRecessed.Y(1:end-1) + ROfXAndYRecessed.Y(2:end))/2;
            ROfXAndYRecessed.Mean = readBinaryData([datadir slash detector.Name],[length(ROfXAndYRecessed.Y)-1,length(ROfXAndYRecessed.X)-1]);  % read column major json binary  
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(ROfXAndYRecessed.Y)-1,length(ROfXAndYRecessed.X)-1]); 
                ROfXAndYRecessed.Stdev = sqrt((ROfXAndYRecessed.SecondMoment - (ROfXAndYRecessed.Mean .* ROfXAndYRecessed.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYRecessed = ROfXAndYRecessed;
         case 'ROfXAndYAndTime'
            ROfXAndYAndTime.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            ROfXAndYAndTime.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndTime.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfXAndYAndTime.X_Midpoints = (ROfXAndYAndTime.X(1:end-1) + ROfXAndYAndTime.X(2:end))/2;
            ROfXAndYAndTime.Y_Midpoints = (ROfXAndYAndTime.Y(1:end-1) + ROfXAndYAndTime.Y(2:end))/2;
            ROfXAndYAndTime.Time_Midpoints = (ROfXAndYAndTime.Time(1:end-1) + ROfXAndYAndTime.Time(2:end))/2;
            ROfXAndYAndTime.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(ROfXAndYAndTime.X)-1)*(length(ROfXAndYAndTime.Y)-1)*(length(ROfXAndYAndTime.Time)-1)]); 
            ROfXAndYAndTime.Mean = reshape(ROfXAndYAndTime.Mean, ...% column major json binary
                [length(ROfXAndYAndTime.Time)-1,length(ROfXAndYAndTime.Y)-1,length(ROfXAndYAndTime.X)-1]); 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndTime.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [(length(ROfXAndYAndTime.Time)-1)*(length(ROfXAndYAndTime.Y)-1)*(length(ROfXAndYAndTime.X)-1)]); 
                ROfXAndYAndTime.SecondMoment = reshape(ROfXAndYAndTime.SecondMoment, ...% column major json binary
                  [length(ROfXAndYAndTime.Time)-1,length(ROfXAndYAndTime.Y)-1,length(ROfXAndYAndTime.X)-1]);
                ROfXAndYAndTime.Stdev = sqrt((ROfXAndYAndTime.SecondMoment - (ROfXAndYAndTime.Mean .* ROfXAndYAndTime.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYAndTime = ROfXAndYAndTime;
        case 'ROfXAndYAndTimeRecessed'
            ROfXAndYAndTimeRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            ROfXAndYAndTimeRecessed.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndTimeRecessed.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndTimeRecessed.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfXAndYAndTimeRecessed.X_Midpoints = (ROfXAndYAndTimeRecessed.X(1:end-1) + ROfXAndYAndTimeRecessed.X(2:end))/2;
            ROfXAndYAndTimeRecessed.Y_Midpoints = (ROfXAndYAndTimeRecessed.Y(1:end-1) + ROfXAndYAndTimeRecessed.Y(2:end))/2;
            ROfXAndYAndTimeRecessed.Time_Midpoints = (ROfXAndYAndTimeRecessed.Time(1:end-1) + ROfXAndYAndTimeRecessed.Time(2:end))/2;
            ROfXAndYAndTimeRecessed.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(ROfXAndYAndTimeRecessed.X)-1)*(length(ROfXAndYAndTimeRecessed.Y)-1)*(length(ROfXAndYAndTimeRecessed.Time)-1)]); 
            ROfXAndYAndTimeRecessed.Mean = reshape(ROfXAndYAndTimeRecessed.Mean, ...% column major json binary
                [length(ROfXAndYAndTimeRecessed.Time)-1,length(ROfXAndYAndTimeRecessed.Y)-1,length(ROfXAndYAndTimeRecessed.X)-1]); 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndTimeRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [(length(ROfXAndYAndTimeRecessed.Time)-1)*(length(ROfXAndYAndTimeRecessed.Y)-1)*(length(ROfXAndYAndTimeRecessed.X)-1)]); 
                ROfXAndYAndTimeRecessed.SecondMoment = reshape(ROfXAndYAndTimeRecessed.SecondMoment, ...% column major json binary
                  [length(ROfXAndYAndTimeRecessed.Time)-1,length(ROfXAndYAndTimeRecessed.Y)-1,length(ROfXAndYAndTimeRecessed.X)-1]);
                ROfXAndYAndTimeRecessed.Stdev = sqrt((ROfXAndYAndTimeRecessed.SecondMoment - (ROfXAndYAndTimeRecessed.Mean .* ROfXAndYAndTimeRecessed.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYAndTimeRecessed = ROfXAndYAndTimeRecessed;
        case 'ROfXAndYAndTimeAndSubregion'
            ROfXAndYAndTimeAndSubregion.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            tempNumberOfRegions = length(json.TissueInput.Regions); % get tissue region count from json
            ROfXAndYAndTimeAndSubregion.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndTimeAndSubregion.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndTimeAndSubregion.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfXAndYAndTimeAndSubregion.X_Midpoints = (ROfXAndYAndTimeAndSubregion.X(1:end-1) + ROfXAndYAndTimeAndSubregion.X(2:end))/2;
            ROfXAndYAndTimeAndSubregion.Y_Midpoints = (ROfXAndYAndTimeAndSubregion.Y(1:end-1) + ROfXAndYAndTimeAndSubregion.Y(2:end))/2;
            ROfXAndYAndTimeAndSubregion.Time_Midpoints = (ROfXAndYAndTimeAndSubregion.Time(1:end-1) + ROfXAndYAndTimeAndSubregion.Time(2:end))/2;
            ROfXAndYAndTimeAndSubregion.NumberOfRegions = tempNumberOfRegions;
            ROfXAndYAndTimeAndSubregion.Mean = readBinaryData([datadir slash detector.Name],...
                [tempNumberOfRegions*(length(ROfXAndYAndTimeAndSubregion.Time)-1)*(length(ROfXAndYAndTimeAndSubregion.Y)-1)*(length(ROfXAndYAndTimeAndSubregion.X)-1)]); % read column major json binary            
            ROfXAndYAndTimeAndSubregion.Mean = reshape(ROfXAndYAndTimeAndSubregion.Mean,...
                [tempNumberOfRegions,length(ROfXAndYAndTimeAndSubregion.Time)-1,length(ROfXAndYAndTimeAndSubregion.Y)-1,length(ROfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
            ROfXAndYAndTimeAndSubregion.ROfXAndY = readBinaryData([datadir slash detector.Name '_ROfXAndY'],[length(ROfXAndYAndTimeAndSubregion.Y)-1,length(ROfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary          
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndTimeAndSubregion.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [tempNumberOfRegions*(length(ROfXAndYAndTimeAndSubregion.Time)-1)*(length(ROfXAndYAndTimeAndSubregion.Y)-1)*(length(ROfXAndYAndTimeAndSubregion.X)-1)]);
                ROfXAndYAndTimeAndSubregion.SecondMoment = reshape(ROfXAndYAndTimeAndSubregion.SecondMoment,...
                  [tempNumberOfRegions,length(ROfXAndYAndTimeAndSubregion.Time)-1,length(ROfXAndYAndTimeAndSubregion.Y)-1,length(ROfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
                ROfXAndYAndTimeAndSubregion.Stdev = sqrt((ROfXAndYAndTimeAndSubregion.SecondMoment - (ROfXAndYAndTimeAndSubregion.Mean .* ROfXAndYAndTimeAndSubregion.Mean)) / (json.N));
                ROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment = readBinaryData([datadir slash detector.Name '_ROfXAndY_2'],[length(ROfXAndYAndTimeAndSubregion.Y)-1,length(ROfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary          
                ROfXAndYAndTimeAndSubregion.ROfXAndYStdev = sqrt((ROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment - (ROfXAndYAndTimeAndSubregion.ROfXAndY .* ROfXAndYAndTimeAndSubregion.ROfXAndY)) / (json.N));
            end
            results{di}.ROfXAndYAndTimeAndSubregion = ROfXAndYAndTimeAndSubregion;
        case 'ROfXAndYAndTimeAndSubregionRecessed'
            ROfXAndYAndTimeAndSubregionRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            tempNumberOfRegions = length(json.TissueInput.Regions); % get tissue region count from json
            ROfXAndYAndTimeAndSubregionRecessed.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndTimeAndSubregionRecessed.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndTimeAndSubregionRecessed.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfXAndYAndTimeAndSubregionRecessed.X_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.X(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.X(2:end))/2;
            ROfXAndYAndTimeAndSubregionRecessed.Y_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.Y(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.Y(2:end))/2;
            ROfXAndYAndTimeAndSubregionRecessed.Time_Midpoints = (ROfXAndYAndTimeAndSubregionRecessed.Time(1:end-1) + ROfXAndYAndTimeAndSubregionRecessed.Time(2:end))/2;
            ROfXAndYAndTimeAndSubregionRecessed.NumberOfRegions = tempNumberOfRegions;
            ROfXAndYAndTimeAndSubregionRecessed.Mean = readBinaryData([datadir slash detector.Name],...
                [tempNumberOfRegions*(length(ROfXAndYAndTimeAndSubregionRecessed.Time)-1)*(length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1)*(length(ROfXAndYAndTimeAndSubregionRecessed.X)-1)]); % read column major json binary            
            ROfXAndYAndTimeAndSubregionRecessed.Mean = reshape(ROfXAndYAndTimeAndSubregionRecessed.Mean,...
                [tempNumberOfRegions,length(ROfXAndYAndTimeAndSubregionRecessed.Time)-1,length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(ROfXAndYAndTimeAndSubregionRecessed.X)-1]); % read column major json binary            
            ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY = readBinaryData([datadir slash detector.Name '_ROfXAndY'],[length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(ROfXAndYAndTimeAndSubregionRecessed.X)-1]);  % read column major json binary               
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndTimeAndSubregionRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [tempNumberOfRegions*(length(ROfXAndYAndTimeAndSubregionRecessed.Time)-1)*(length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1)*(length(ROfXAndYAndTimeAndSubregionRecessed.X)-1)]);
                ROfXAndYAndTimeAndSubregionRecessed.SecondMoment = reshape(ROfXAndYAndTimeAndSubregionRecessed.SecondMoment,...
                  [tempNumberOfRegions,length(ROfXAndYAndTimeAndSubregionRecessed.Time)-1,length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(ROfXAndYAndTimeAndSubregionRecessed.X)-1]); % read column major json binary            
                ROfXAndYAndTimeAndSubregionRecessed.Stdev = sqrt((ROfXAndYAndTimeAndSubregionRecessed.SecondMoment - (ROfXAndYAndTimeAndSubregionRecessed.Mean .* ROfXAndYAndTimeAndSubregionRecessed.Mean)) / (json.N));
                ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment = readBinaryData([datadir slash detector.Name '_ROfXAndY_2'],[length(ROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(ROfXAndYAndTimeAndSubregionRecessed.X)-1]);  % read column major json binary          
                ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYStdev = sqrt((ROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment - (ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY .* ROfXAndYAndTimeAndSubregionRecessed.ROfXAndY)) / (json.N));
            end
            results{di}.ROfXAndYAndTimeAndSubregionRecessed = ROfXAndYAndTimeAndSubregionRecessed;
        case 'ROfXAndYAndThetaAndPhi'
            ROfXAndYAndThetaAndPhi.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTheta = detector.Theta;
            tempPhi = detector.Phi;
            ROfXAndYAndThetaAndPhi.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndThetaAndPhi.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndThetaAndPhi.Theta = linspace((tempTheta.Start), (tempTheta.Stop), (tempTheta.Count));
            ROfXAndYAndThetaAndPhi.Phi = linspace((tempPhi.Start), (tempPhi.Stop), (tempPhi.Count));
            ROfXAndYAndThetaAndPhi.X_Midpoints = (ROfXAndYAndThetaAndPhi.X(1:end-1) + ROfXAndYAndThetaAndPhi.X(2:end))/2;
            ROfXAndYAndThetaAndPhi.Y_Midpoints = (ROfXAndYAndThetaAndPhi.Y(1:end-1) + ROfXAndYAndThetaAndPhi.Y(2:end))/2;
            ROfXAndYAndThetaAndPhi.Theta_Midpoints = (ROfXAndYAndThetaAndPhi.Theta(1:end-1) + ROfXAndYAndThetaAndPhi.Theta(2:end))/2;
            ROfXAndYAndThetaAndPhi.Phi_Midpoints = (ROfXAndYAndThetaAndPhi.Phi(1:end-1) + ROfXAndYAndThetaAndPhi.Phi(2:end))/2;
            ROfXAndYAndThetaAndPhi.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(ROfXAndYAndThetaAndPhi.X)-1)*(length(ROfXAndYAndThetaAndPhi.Y)-1)* ...
                (length(ROfXAndYAndThetaAndPhi.Theta)-1)*(length(ROfXAndYAndThetaAndPhi.Phi)-1)]); 
            ROfXAndYAndThetaAndPhi.Mean = reshape(ROfXAndYAndThetaAndPhi.Mean, ...% column major json binary
                [length(ROfXAndYAndThetaAndPhi.Phi)-1,length(ROfXAndYAndThetaAndPhi.Theta)-1,...
                length(ROfXAndYAndThetaAndPhi.Y)-1,length(ROfXAndYAndThetaAndPhi.X)-1]); 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndThetaAndPhi.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [(length(ROfXAndYAndThetaAndPhi.Phi)-1)*(length(ROfXAndYAndThetaAndPhi.Theta)-1)*...
                  (length(ROfXAndYAndThetaAndPhi.Y)-1)*(length(ROfXAndYAndThetaAndPhi.X)-1)]); 
                ROfXAndYAndThetaAndPhi.SecondMoment = reshape(ROfXAndYAndThetaAndPhi.SecondMoment, ...% column major json binary
                  [length(ROfXAndYAndThetaAndPhi.Phi)-1,length(ROfXAndYAndThetaAndPhi.Theta)-1,...
                  length(ROfXAndYAndThetaAndPhi.Y)-1,length(ROfXAndYAndThetaAndPhi.X)-1]);
                ROfXAndYAndThetaAndPhi.Stdev = sqrt((ROfXAndYAndThetaAndPhi.SecondMoment - (ROfXAndYAndThetaAndPhi.Mean .* ROfXAndYAndThetaAndPhi.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYAndThetaAndPhi = ROfXAndYAndThetaAndPhi;
        case 'ROfXAndYAndMaxDepth'
            ROfXAndYAndMaxDepth.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempMaxDepth = detector.MaxDepth;
            ROfXAndYAndMaxDepth.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndMaxDepth.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndMaxDepth.MaxDepth = linspace((tempMaxDepth.Start), (tempMaxDepth.Stop), (tempMaxDepth.Count));
            ROfXAndYAndMaxDepth.X_Midpoints = (ROfXAndYAndMaxDepth.X(1:end-1) + ROfXAndYAndMaxDepth.X(2:end))/2;
            ROfXAndYAndMaxDepth.Y_Midpoints = (ROfXAndYAndMaxDepth.Y(1:end-1) + ROfXAndYAndMaxDepth.Y(2:end))/2;
            ROfXAndYAndMaxDepth.MaxDepth_Midpoints = (ROfXAndYAndMaxDepth.MaxDepth(1:end-1) + ROfXAndYAndMaxDepth.MaxDepth(2:end))/2;
            ROfXAndYAndMaxDepth.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(ROfXAndYAndMaxDepth.X)-1)*(length(ROfXAndYAndMaxDepth.Y)-1)*(length(ROfXAndYAndMaxDepth.MaxDepth)-1)]); 
            ROfXAndYAndMaxDepth.Mean = reshape(ROfXAndYAndMaxDepth.Mean, ...% column major json binary
                [length(ROfXAndYAndMaxDepth.MaxDepth)-1,length(ROfXAndYAndMaxDepth.Y)-1,length(ROfXAndYAndMaxDepth.X)-1]); 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndMaxDepth.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [(length(ROfXAndYAndMaxDepth.MaxDepth)-1)*(length(ROfXAndYAndMaxDepth.Y)-1)*(length(ROfXAndYAndMaxDepth.X)-1)]); 
                ROfXAndYAndMaxDepth.SecondMoment = reshape(ROfXAndYAndMaxDepth.SecondMoment, ...% column major json binary
                  [length(ROfXAndYAndMaxDepth.MaxDepth)-1,length(ROfXAndYAndMaxDepth.Y)-1,length(ROfXAndYAndMaxDepth.X)-1]);
                ROfXAndYAndMaxDepth.Stdev = sqrt((ROfXAndYAndMaxDepth.SecondMoment - (ROfXAndYAndMaxDepth.Mean .* ROfXAndYAndMaxDepth.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYAndMaxDepth = ROfXAndYAndMaxDepth;
        case 'ROfXAndYAndMaxDepthRecessed'
            ROfXAndYAndMaxDepthRecessed.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempMaxDepth = detector.MaxDepth;
            ROfXAndYAndMaxDepthRecessed.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ROfXAndYAndMaxDepthRecessed.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ROfXAndYAndMaxDepthRecessed.MaxDepth = linspace((tempMaxDepth.Start), (tempMaxDepth.Stop), (tempMaxDepth.Count));
            ROfXAndYAndMaxDepthRecessed.X_Midpoints = (ROfXAndYAndMaxDepthRecessed.X(1:end-1) + ROfXAndYAndMaxDepthRecessed.X(2:end))/2;
            ROfXAndYAndMaxDepthRecessed.Y_Midpoints = (ROfXAndYAndMaxDepthRecessed.Y(1:end-1) + ROfXAndYAndMaxDepthRecessed.Y(2:end))/2;
            ROfXAndYAndMaxDepthRecessed.MaxDepth_Midpoints = (ROfXAndYAndMaxDepthRecessed.MaxDepth(1:end-1) + ROfXAndYAndMaxDepthRecessed.MaxDepth(2:end))/2;
            ROfXAndYAndMaxDepthRecessed.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(ROfXAndYAndMaxDepthRecessed.X)-1)*(length(ROfXAndYAndMaxDepthRecessed.Y)-1)*(length(ROfXAndYAndMaxDepthRecessed.MaxDepth)-1)]); 
            ROfXAndYAndMaxDepthRecessed.Mean = reshape(ROfXAndYAndMaxDepthRecessed.Mean, ...% column major json binary
                [length(ROfXAndYAndMaxDepthRecessed.MaxDepth)-1,length(ROfXAndYAndMaxDepthRecessed.Y)-1,length(ROfXAndYAndMaxDepthRecessed.X)-1]); 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfXAndYAndMaxDepthRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [(length(ROfXAndYAndMaxDepthRecessed.MaxDepth)-1)*(length(ROfXAndYAndMaxDepthRecessed.Y)-1)*(length(ROfXAndYAndMaxDepthRecessed.X)-1)]); 
                ROfXAndYAndMaxDepthRecessed.SecondMoment = reshape(ROfXAndYAndMaxDepthRecessed.SecondMoment, ...% column major json binary
                  [length(ROfXAndYAndMaxDepthRecessed.MaxDepth)-1,length(ROfXAndYAndMaxDepthRecessed.Y)-1,length(ROfXAndYAndMaxDepthRecessed.X)-1]);
                ROfXAndYAndMaxDepthRecessed.Stdev = sqrt((ROfXAndYAndMaxDepthRecessed.SecondMoment - (ROfXAndYAndMaxDepthRecessed.Mean .* ROfXAndYAndMaxDepthRecessed.Mean)) / json.N); 
            end      
            results{di}.ROfXAndYAndMaxDepthRecessed = ROfXAndYAndMaxDepthRecessed;
        case 'ROfFx'
            ROfFx.Name = detector.Name;
            tempFx = detector.Fx;
            ROfFx.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            ROfFx.Fx_Midpoints = ROfFx.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(ROfFx.Fx));
            ROfFx.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);  
            ROfFx.Amplitude = abs(ROfFx.Mean);
            ROfFx.Phase = -angle(ROfFx.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],2*length(ROfFx.Fx));
                ROfFx.SecondMoment = tempData(1:2:end) + tempData(2:2:end); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfFx.Stdev = sqrt((ROfFx.SecondMoment - real(ROfFx.Mean) .* real(ROfFx.Mean) ...
                                                       - imag(ROfFx.Mean) .* imag(ROfFx.Mean)) / json.N);
            end
            results{di}.ROfFx = ROfFx;
        case 'ROfFxAndTime'
            ROfFxAndTime.Name = detector.Name;
            tempFx = detector.Fx;
            tempTime = detector.Time;
            ROfFxAndTime.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            ROfFxAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            ROfFxAndTime.Fx_Midpoints = ROfFxAndTime.Fx;
            ROfFxAndTime.Time_Midpoints = (ROfFxAndTime.Time(1:end-1) + ROfFxAndTime.Time(2:end))/2;
            tempData = readBinaryData([datadir slash detector.Name],[2*(length(ROfFxAndTime.Time)-1), length(ROfFxAndTime.Fx)]); % column major but complex
            ROfFxAndTime.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);  
            ROfFxAndTime.Amplitude = abs(ROfFxAndTime.Mean);
            ROfFxAndTime.Phase = -angle(ROfFxAndTime.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],[2*(length(ROfFxAndTime.Time)-1), length(ROfFxAndTime.Fx)]);
                ROfFxAndTime.SecondMoment = tempData(1:2:end,:) + tempData(2:2:end,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfFxAndTime.Stdev = sqrt((ROfFxAndTime.SecondMoment - real(ROfFxAndTime.Mean) .* real(ROfFxAndTime.Mean) ...
                                                       - imag(ROfFxAndTime.Mean) .* imag(ROfFxAndTime.Mean)) / json.N);
            end
            results{di}.ROfFxAndTime = ROfFxAndTime;
        case 'ROfFxAndAngle'
            ROfFxAndAngle.Name = detector.Name;
            tempFx = detector.Fx;
            tempAngle = detector.Angle;
            ROfFxAndAngle.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            ROfFxAndAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            ROfFxAndAngle.Fx_Midpoints = ROfFxAndAngle.Fx;
            ROfFxAndAngle.Angle_Midpoints = (ROfFxAndAngle.Angle(1:end-1) + ROfFxAndAngle.Angle(2:end))/2;
            tempData = readBinaryData([datadir slash detector.Name],[2*(length(ROfFxAndAngle.Angle)-1), length(ROfFxAndAngle.Fx)]); % column major but complex
            ROfFxAndAngle.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);  
            ROfFxAndAngle.Amplitude = abs(ROfFxAndAngle.Mean);
            ROfFxAndAngle.Phase = -angle(ROfFxAndAngle.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],[2*(length(ROfFxAndAngle.Angle)-1), length(ROfFxAndAngle.Fx)]);
                ROfFxAndAngle.SecondMoment = tempData(1:2:end,:) + tempData(2:2:end,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfFxAndAngle.Stdev = sqrt((ROfFxAndAngle.SecondMoment - real(ROfFxAndAngle.Mean) .* real(ROfFxAndAngle.Mean) ...
                                                       - imag(ROfFxAndAngle.Mean) .* imag(ROfFxAndAngle.Mean)) / json.N);
            end
            results{di}.ROfFxAndAngle = ROfFxAndAngle;
        case 'ROfFxAndMaxDepth'
            ROfFxAndMaxDepth.Name = detector.Name;
            tempFx = detector.Fx;
            tempMaxDepth = detector.MaxDepth;
            ROfFxAndMaxDepth.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            ROfFxAndMaxDepth.MaxDepth = linspace((tempMaxDepth.Start), (tempMaxDepth.Stop), (tempMaxDepth.Count));
            ROfFxAndMaxDepth.Fx_Midpoints = ROfFxAndMaxDepth.Fx;
            ROfFxAndMaxDepth.MaxDepth_Midpoints = (ROfFxAndMaxDepth.MaxDepth(1:end-1) + ROfFxAndMaxDepth.MaxDepth(2:end))/2;
            tempData = readBinaryData([datadir slash detector.Name],[2*(length(ROfFxAndMaxDepth.MaxDepth)-1), length(ROfFxAndMaxDepth.Fx)]); % column major but complex
            ROfFxAndMaxDepth.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);  
            ROfFxAndMaxDepth.Amplitude = abs(ROfFxAndMaxDepth.Mean);
            ROfFxAndMaxDepth.Phase = -angle(ROfFxAndMaxDepth.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],[2*(length(ROfFxAndMaxDepth.MaxDepth)-1), length(ROfFxAndMaxDepth.Fx)]);
                ROfFxAndMaxDepth.SecondMoment = tempData(1:2:end,:) + tempData(2:2:end,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                ROfFxAndMaxDepth.Stdev = sqrt((ROfFxAndMaxDepth.SecondMoment - real(ROfFxAndMaxDepth.Mean) .* real(ROfFxAndMaxDepth.Mean) ...
                                                       - imag(ROfFxAndMaxDepth.Mean) .* imag(ROfFxAndMaxDepth.Mean)) / json.N);
            end
            results{di}.ROfFxAndMaxDepth = ROfFxAndMaxDepth;
       case 'TDiffuse'
            TDiffuse.Name = detector.Name;
            TDiffuse_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            TDiffuse.Mean = TDiffuse_txt.Mean;              
            TDiffuse.SecondMoment = TDiffuse_txt.SecondMoment; 
            TDiffuse.Stdev = sqrt((TDiffuse.SecondMoment - (TDiffuse.Mean .* TDiffuse.Mean)) / json.N);
            results{di}.TDiffuse = TDiffuse;
        case 'TOfRho'
            TOfRho.Name = detector.Name;
            tempRho = detector.Rho;
            TOfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            TOfRho.Rho_Midpoints = (TOfRho.Rho(1:end-1) + TOfRho.Rho(2:end))/2;
            TOfRho.Mean = readBinaryData([datadir slash detector.Name],length(TOfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(TOfRho.Rho)-1);
                TOfRho.Stdev = sqrt((TOfRho.SecondMoment - (TOfRho.Mean .* TOfRho.Mean)) / json.N);
            end
            results{di}.TOfRho = TOfRho;
        case 'TOfAngle'
            TOfAngle.Name = detector.Name;
            tempAngle = detector.Angle;
            TOfAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            TOfAngle.Angle_Midpoints = (TOfAngle.Angle(1:end-1) + TOfAngle.Angle(2:end))/2;
            TOfAngle.Mean = readBinaryData([datadir slash detector.Name],length(TOfAngle.Angle)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(TOfAngle.Angle)-1);
                TOfAngle.Stdev = sqrt((TOfAngle.SecondMoment - (TOfAngle.Mean .* TOfAngle.Mean)) / json.N);
            end
            results{di}.TOfAngle = TOfAngle;
        case 'TOfRhoAndAngle'
            TOfRhoAndAngle.Name = detector.Name;
            tempRho = detector.Rho;
            tempAngle = detector.Angle;
            TOfRhoAndAngle.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            TOfRhoAndAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            TOfRhoAndAngle.Rho_Midpoints = (TOfRhoAndAngle.Rho(1:end-1) + TOfRhoAndAngle.Rho(2:end))/2;
            TOfRhoAndAngle.Angle_Midpoints = (TOfRhoAndAngle.Angle(1:end-1) + TOfRhoAndAngle.Angle(2:end))/2;
            TOfRhoAndAngle.Mean = readBinaryData([datadir slash detector.Name],[length(ROfRhoAndAngle.Angle)-1,length(TOfRhoAndAngle.Rho)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfRhoAndAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(TOfRhoAndAngle.Angle)-1,length(TOfRhoAndAngle.Rho)-1]); 
                TOfRhoAndAngle.Stdev = sqrt((TOfRhoAndAngle.SecondMoment - (TOfRhoAndAngle.Mean .* TOfRhoAndAngle.Mean)) / json.N); 
            end
            results{di}.TOfRhoAndAngle = TOfRhoAndAngle;
        case 'TOfXAndY'
            TOfXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            TOfXAndY.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            TOfXAndY.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            TOfXAndY.X_Midpoints = (TOfXAndY.X(1:end-1) + TOfXAndY.X(2:end))/2;
            TOfXAndY.Y_Midpoints = (TOfXAndY.Y(1:end-1) + TOfXAndY.Y(2:end))/2;
            TOfXAndY.Mean = readBinaryData([datadir slash detector.Name],[length(TOfXAndY.Y)-1,length(TOfXAndY.X)-1]);  % read column major json binary  
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfXAndY.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(TOfXAndY.Y)-1,length(TOfXAndY.X)-1]); 
                TOfXAndY.Stdev = sqrt((TOfXAndY.SecondMoment - (TOfXAndY.Mean .* TOfXAndY.Mean)) / json.N); 
            end      
            results{di}.TOfXAndY = TOfXAndY;
        case 'TOfXAndYAndTimeAndSubregion'
            TOfXAndYAndTimeAndSubregion.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempTime = detector.Time;
            tempNumberOfRegions = length(json.TissueInput.Regions); % get tissue region count from json
            TOfXAndYAndTimeAndSubregion.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            TOfXAndYAndTimeAndSubregion.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            TOfXAndYAndTimeAndSubregion.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            TOfXAndYAndTimeAndSubregion.X_Midpoints = (TOfXAndYAndTimeAndSubregion.X(1:end-1) + TOfXAndYAndTimeAndSubregion.X(2:end))/2;
            TOfXAndYAndTimeAndSubregion.Y_Midpoints = (TOfXAndYAndTimeAndSubregion.Y(1:end-1) + TOfXAndYAndTimeAndSubregion.Y(2:end))/2;
            TOfXAndYAndTimeAndSubregion.Time_Midpoints = (TOfXAndYAndTimeAndSubregion.Time(1:end-1) + TOfXAndYAndTimeAndSubregion.Time(2:end))/2;
            TOfXAndYAndTimeAndSubregion.NumberOfRegions = tempNumberOfRegions;
            TOfXAndYAndTimeAndSubregion.Mean = readBinaryData([datadir slash detector.Name],...
                [tempNumberOfRegions*(length(TOfXAndYAndTimeAndSubregion.Time)-1)*(length(TOfXAndYAndTimeAndSubregion.Y)-1)*(length(TOfXAndYAndTimeAndSubregion.X)-1)]); % read column major json binary            
            TOfXAndYAndTimeAndSubregion.Mean = reshape(TOfXAndYAndTimeAndSubregion.Mean,...
                [tempNumberOfRegions,length(TOfXAndYAndTimeAndSubregion.Time)-1,length(TOfXAndYAndTimeAndSubregion.Y)-1,length(TOfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
            TOfXAndYAndTimeAndSubregion.TOfXAndY = readBinaryData([datadir slash detector.Name '_TOfXAndY'],[length(TOfXAndYAndTimeAndSubregion.Y)-1,length(TOfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary          
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfXAndYAndTimeAndSubregion.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [tempNumberOfRegions*(length(TOfXAndYAndTimeAndSubregion.Time)-1)*(length(TOfXAndYAndTimeAndSubregion.Y)-1)*(length(TOfXAndYAndTimeAndSubregion.X)-1)]);
                TOfXAndYAndTimeAndSubregion.SecondMoment = reshape(TOfXAndYAndTimeAndSubregion.SecondMoment,...
                  [tempNumberOfRegions,length(TOfXAndYAndTimeAndSubregion.Time)-1,length(TOfXAndYAndTimeAndSubregion.Y)-1,length(TOfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
                TOfXAndYAndTimeAndSubregion.Stdev = sqrt((TOfXAndYAndTimeAndSubregion.SecondMoment - (TOfXAndYAndTimeAndSubregion.Mean .* TOfXAndYAndTimeAndSubregion.Mean)) / (json.N));
                TOfXAndYAndTimeAndSubregion.TOfXAndYSecondMoment = readBinaryData([datadir slash detector.Name '_TOfXAndY_2'],[length(TOfXAndYAndTimeAndSubregion.Y)-1,length(TOfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary          
                TOfXAndYAndTimeAndSubregion.TOfXAndYStdev = sqrt((TOfXAndYAndTimeAndSubregion.TOfXAndYSecondMoment - (TOfXAndYAndTimeAndSubregion.TOfXAndY .* TOfXAndYAndTimeAndSubregion.TOfXAndY)) / (json.N));
            end
            results{di}.TOfXAndYAndTimeAndSubregion = TOfXAndYAndTimeAndSubregion;
		case 'TOfFx'
            TOfFx.Name = detector.Name;
            tempFx = detector.Fx;
            TOfFx.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            TOfFx.Fx_Midpoints = TOfFx.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(TOfFx.Fx));
            TOfFx.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);     
            TOfFx.Amplitude = abs(TOfFx.Mean);
            TOfFx.Phase = -angle(TOfFx.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],2*length(TOfFx.Fx));
                TOfFx.SecondMoment = tempData(1:2:end) + tempData(2:2:end); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                TOfFx.Stdev = sqrt((TOfFx.SecondMoment - real(TOfFx.Mean) .* real(TOfFx.Mean) ...
                                                       - imag(TOfFx.Mean) .* imag(TOfFx.Mean)) / json.N);
            end
            results{di}.TOfFx = TOfFx;
        case 'ATotal'
            ATotal.Name = detector.Name;
            ATotal_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            ATotal.Mean = ATotal_txt.Mean;              
            ATotal.SecondMoment = ATotal_txt.SecondMoment; 
            ATotal.Stdev = sqrt((ATotal.SecondMoment - (ATotal.Mean .* ATotal.Mean)) / (json.N));
            results{di}.ATotal = ATotal;
        case 'AOfRhoAndZ'
            AOfRhoAndZ.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            AOfRhoAndZ.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            AOfRhoAndZ.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            AOfRhoAndZ.Rho_Midpoints = (AOfRhoAndZ.Rho(1:end-1) + AOfRhoAndZ.Rho(2:end))/2;
            AOfRhoAndZ.Z_Midpoints = (AOfRhoAndZ.Z(1:end-1) + AOfRhoAndZ.Z(2:end))/2;
            AOfRhoAndZ.Mean = readBinaryData([datadir slash detector.Name],[length(AOfRhoAndZ.Z)-1,length(AOfRhoAndZ.Rho)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                AOfRhoAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(AOfRhoAndZ.Z)-1,length(AOfRhoAndZ.Rho)-1]); 
                AOfRhoAndZ.Stdev = sqrt((AOfRhoAndZ.SecondMoment - (AOfRhoAndZ.Mean .* AOfRhoAndZ.Mean)) / json.N); 
            end
            results{di}.AOfRhoAndZ = AOfRhoAndZ;
        case 'AOfXAndYAndZ'
            AOfXAndYAndZ.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            tempZ = detector.Z;
            AOfXAndYAndZ.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            AOfXAndYAndZ.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            AOfXAndYAndZ.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            AOfXAndYAndZ.X_Midpoints = (AOfXAndYAndZ.X(1:end-1) + AOfXAndYAndZ.X(2:end))/2;
            AOfXAndYAndZ.Y_Midpoints = (AOfXAndYAndZ.Y(1:end-1) + AOfXAndYAndZ.Y(2:end))/2;
            AOfXAndYAndZ.Z_Midpoints = (AOfXAndYAndZ.Z(1:end-1) + AOfXAndYAndZ.Z(2:end))/2;
            AOfXAndYAndZ.Mean = readBinaryData([datadir slash detector.Name],[(length(AOfXAndYAndZ.X)-1) * (length(AOfXAndYAndZ.Y)-1) * (length(AOfXAndYAndZ.Z)-1)]); 
            AOfXAndYAndZ.Mean = reshape(AOfXAndYAndZ.Mean, ...% read column major json binary
                [length(AOfXAndYAndZ.Z)-1,length(AOfXAndYAndZ.Y)-1,length(AOfXAndYAndZ.X)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                AOfXAndYAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(AOfXAndYAndZ.X)-1) * (length(AOfXAndYAndZ.Y)-1) * (length(AOfXAndYAndZ.Z)-1)]);
                AOfXAndYAndZ.SecondMoment = reshape(AOfXAndYAndZ.SecondMoment, ... % column major json binary
                    [length(AOfXAndYAndZ.Z)-1,length(AOfXAndYAndZ.Y)-1,length(AOfXAndYAndZ.X)-1]);
                AOfXAndYAndZ.Stdev = sqrt((AOfXAndYAndZ.SecondMoment - (AOfXAndYAndZ.Mean .* AOfXAndYAndZ.Mean)) / json.N);  
            end
            results{di}.AOfXAndYAndZ = AOfXAndYAndZ;
        case 'ATotalBoundingVolume'
            ATotalBoundingVolume.Name = detector.Name;
            ATotalBoundingVolume_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            ATotalBoundingVolume.Mean = ATotalBoundingVolume_txt.Mean;              
            ATotalBoundingVolume.SecondMoment = ATotalBoundingVolume_txt.SecondMoment; 
            ATotalBoundingVolume.Stdev = sqrt((ATotalBoundingVolume.SecondMoment - (ATotalBoundingVolume.Mean .* ATotalBoundingVolume.Mean)) / (json.N));
            results{di}.ATotalBoundingVolume = ATotalBoundingVolume;
        case 'FluenceOfRhoAndZ'
            FluenceOfRhoAndZ.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            FluenceOfRhoAndZ.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            FluenceOfRhoAndZ.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfRhoAndZ.Rho_Midpoints = (FluenceOfRhoAndZ.Rho(1:end-1) + FluenceOfRhoAndZ.Rho(2:end))/2;
            FluenceOfRhoAndZ.Z_Midpoints = (FluenceOfRhoAndZ.Z(1:end-1) + FluenceOfRhoAndZ.Z(2:end))/2;
            FluenceOfRhoAndZ.Mean = readBinaryData([datadir slash detector.Name],[length(FluenceOfRhoAndZ.Z)-1,length(FluenceOfRhoAndZ.Rho)-1]); % read column major binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                FluenceOfRhoAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(FluenceOfRhoAndZ.Z)-1,length(FluenceOfRhoAndZ.Rho)-1]);
                FluenceOfRhoAndZ.Stdev = sqrt((FluenceOfRhoAndZ.SecondMoment - (FluenceOfRhoAndZ.Mean .* FluenceOfRhoAndZ.Mean)) / json.N);  
            end
            results{di}.FluenceOfRhoAndZ = FluenceOfRhoAndZ;
        case 'FluenceOfRhoAndZAndTime'
            FluenceOfRhoAndZAndTime.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempTime = detector.Time;
            FluenceOfRhoAndZAndTime.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            FluenceOfRhoAndZAndTime.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfRhoAndZAndTime.Time = linspace((tempTime.Start), (tempTime.Stop), (tempTime.Count));
            FluenceOfRhoAndZAndTime.Rho_Midpoints = (FluenceOfRhoAndZAndTime.Rho(1:end-1) + FluenceOfRhoAndZAndTime.Rho(2:end))/2;
            FluenceOfRhoAndZAndTime.Z_Midpoints = (FluenceOfRhoAndZAndTime.Z(1:end-1) + FluenceOfRhoAndZAndTime.Z(2:end))/2;
            FluenceOfRhoAndZAndTime.Time_Midpoints = (FluenceOfRhoAndZAndTime.Time(1:end-1) + FluenceOfRhoAndZAndTime.Time(2:end))/2;
            FluenceOfRhoAndZAndTime.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(FluenceOfRhoAndZAndTime.Rho)-1)*(length(FluenceOfRhoAndZAndTime.Z)-1)*(length(FluenceOfRhoAndZAndTime.Time)-1)]); 
            FluenceOfRhoAndZAndTime.Mean = reshape(FluenceOfRhoAndZAndTime.Mean, ...% column major json binary
                [length(FluenceOfRhoAndZAndTime.Time)-1,length(FluenceOfRhoAndZAndTime.Z)-1,length(FluenceOfRhoAndZAndTime.Rho)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                FluenceOfRhoAndZAndTime.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(FluenceOfRhoAndZAndTime.Rho)-1)*(length(FluenceOfRhoAndZAndTime.Z)-1)*(length(FluenceOfRhoAndZAndTime.Time)-1)]);
                FluenceOfRhoAndZAndTime.SecondMoment = reshape(FluenceOfRhoAndZAndTime.SecondMoment, ... % column major json binary
                    [length(FluenceOfRhoAndZAndTime.Time)-1,length(FluenceOfRhoAndZAndTime.Z)-1,length(FluenceOfRhoAndZAndTime.Rho)-1]);
                FluenceOfRhoAndZAndTime.Stdev = sqrt((FluenceOfRhoAndZAndTime.SecondMoment - (FluenceOfRhoAndZAndTime.Mean .* FluenceOfRhoAndZAndTime.Mean)) / json.N);  
            end
            results{di}.FluenceOfRhoAndZAndTime = FluenceOfRhoAndZAndTime;        
        case 'FluenceOfRhoAndZAndOmega'
            FluenceOfRhoAndZAndOmega.Name = detector.Name;
            tempRho = detector.Rho;
            tempZ = detector.Z;
            tempOmega = detector.Omega;
            FluenceOfRhoAndZAndOmega.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            FluenceOfRhoAndZAndOmega.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            FluenceOfRhoAndZAndOmega.Omega = linspace((tempOmega.Start), (tempOmega.Stop), (tempOmega.Count));
            FluenceOfRhoAndZAndOmega.Rho_Midpoints = (FluenceOfRhoAndZAndOmega.Rho(1:end-1) + FluenceOfRhoAndZAndOmega.Rho(2:end))/2;
            FluenceOfRhoAndZAndOmega.Z_Midpoints = (FluenceOfRhoAndZAndOmega.Z(1:end-1) + FluenceOfRhoAndZAndOmega.Z(2:end))/2;
            FluenceOfRhoAndZAndOmega.Omega_Midpoints = FluenceOfRhoAndZAndOmega.Omega; % omega is not binned, value is used
            tempData = readBinaryData([datadir slash detector.Name], ...
                [2*(length(FluenceOfRhoAndZAndOmega.Rho)-1)*(length(FluenceOfRhoAndZAndOmega.Z)-1)*(length(FluenceOfRhoAndZAndOmega.Omega))]); 
            tempDataReshape = reshape(tempData, ...% column major json binary
                [2*length(FluenceOfRhoAndZAndOmega.Omega),length(FluenceOfRhoAndZAndOmega.Z)-1,length(FluenceOfRhoAndZAndOmega.Rho)-1]);
            FluenceOfRhoAndZAndOmega.Mean = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
            FluenceOfRhoAndZAndOmega.Amplitude = abs(FluenceOfRhoAndZAndOmega.Mean);
            FluenceOfRhoAndZAndOmega.Phase = -angle(FluenceOfRhoAndZAndOmega.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ...
                    [2*(length(FluenceOfRhoAndZAndOmega.Rho)-1)*(length(FluenceOfRhoAndZAndOmega.Z)-1)*(length(FluenceOfRhoAndZAndOmega.Omega))]);
                tempDataReshape = reshape(tempData, ... % column major json binary
                    [2*length(FluenceOfRhoAndZAndOmega.Omega),length(FluenceOfRhoAndZAndOmega.Z)-1,length(FluenceOfRhoAndZAndOmega.X)-1]);
                FluenceOfRhoAndZAndOmega.SecondMoment = tempDataReshape(1:2:end,:,:) + tempDataReshape(2:2:end,:,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                FluenceOfRhoAndZAndOmega.Stdev = sqrt((FluenceOfRhoAndZAndOmega.SecondMoment - real(FluenceOfRhoAndZAndOmega.Mean) .* real(FluenceOfRhoAndZAndOmega.Mean) ...
                                                                           - imag(FluenceOfRhoAndZAndOmega.Mean) .* imag(FluenceOfRhoAndZAndOmega.Mean)) / json.N);
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
            FluenceOfXAndYAndZ.Mean = readBinaryData([datadir slash detector.Name],[(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]); 
            FluenceOfXAndYAndZ.Mean = reshape(FluenceOfXAndYAndZ.Mean, ...% read column major json binary
                [length(FluenceOfXAndYAndZ.Z)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.X)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                FluenceOfXAndYAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]);
                FluenceOfXAndYAndZ.SecondMoment = reshape(FluenceOfXAndYAndZ.SecondMoment, ... % column major json binary
                    [length(FluenceOfXAndYAndZ.Z)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.X)-1]);
                FluenceOfXAndYAndZ.Stdev = sqrt((FluenceOfXAndYAndZ.SecondMoment - (FluenceOfXAndYAndZ.Mean .* FluenceOfXAndYAndZ.Mean)) / json.N);  
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
            FluenceOfXAndYAndZAndTime.Mean = readBinaryData([datadir slash detector.Name], ...
                [(length(FluenceOfXAndYAndZAndTime.X)-1)*(length(FluenceOfXAndYAndZAndTime.Y)-1)*(length(FluenceOfXAndYAndZAndTime.Z)-1)*(length(FluenceOfXAndYAndZAndTime.Time)-1)]); 
            FluenceOfXAndYAndZAndTime.Mean = reshape(FluenceOfXAndYAndZAndTime.Mean, ...% column major json binary
                [length(FluenceOfXAndYAndZAndTime.Time)-1,length(FluenceOfXAndYAndZAndTime.Z)-1,length(FluenceOfXAndYAndZAndTime.Y)-1,length(FluenceOfXAndYAndZAndTime.X)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                FluenceOfXAndYAndZAndTime.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(FluenceOfXAndYAndZAndTime.X)-1)*(length(FluenceOfXAndYAndZAndTime.Y)-1)*(length(FluenceOfXAndYAndZAndTime.Z)-1)*(length(FluenceOfXAndYAndZAndTime.Time))]);
                FluenceOfXAndYAndZAndTime.SecondMoment = reshape(tempData, ... % column major json binary
                    [length(FluenceOfXAndYAndZAndTime.Time),length(FluenceOfXAndYAndZAndTime.Z)-1,length(FluenceOfXAndYAndZAndTime.Y)-1,length(FluenceOfXAndYAndZAndTime.X)-1]);
                FluenceOfXAndYAndZAndTime.Stdev = sqrt((FluenceOfXAndYAndZAndTime.SecondMoment - (FluenceOfXAndYAndZAndTime.Mean) .* (FluenceOfXAndYAndZAndTime.Mean)) / json.N);
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
            tempData = readBinaryData([datadir slash detector.Name], ...
                [2*(length(FluenceOfXAndYAndZAndOmega.X)-1)*(length(FluenceOfXAndYAndZAndOmega.Y)-1)*(length(FluenceOfXAndYAndZAndOmega.Z)-1)*(length(FluenceOfXAndYAndZAndOmega.Omega))]); 
            tempDataReshape = reshape(tempData, ...% column major json binary
                [2*length(FluenceOfXAndYAndZAndOmega.Omega),length(FluenceOfXAndYAndZAndOmega.Z)-1,length(FluenceOfXAndYAndZAndOmega.Y)-1,length(FluenceOfXAndYAndZAndOmega.X)-1]);
            FluenceOfXAndYAndZAndOmega.Mean = tempDataReshape(1:2:end,:,:,:) + 1i*tempDataReshape(2:2:end,:,:,:);
            FluenceOfXAndYAndZAndOmega.Amplitude = abs(FluenceOfXAndYAndZAndOmega.Mean);
            FluenceOfXAndYAndZAndOmega.Phase = -angle(FluenceOfXAndYAndZAndOmega.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ...
                    [2*(length(FluenceOfXAndYAndZAndOmega.X)-1)*(length(FluenceOfXAndYAndZAndOmega.Y)-1)*(length(FluenceOfXAndYAndZAndOmega.Z)-1)*(length(FluenceOfXAndYAndZAndOmega.Omega))]);
                tempDataReshape = reshape(tempData, ... % column major json binary
                    [2*length(FluenceOfXAndYAndZAndOmega.Omega),length(FluenceOfXAndYAndZAndOmega.Z)-1,length(FluenceOfXAndYAndZAndOmega.Y)-1,length(FluenceOfXAndYAndZAndOmega.X)-1]);
                FluenceOfXAndYAndZAndOmega.SecondMoment = tempDataReshape(1:2:end,:,:,:) + tempDataReshape(2:2:end,:,:,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                FluenceOfXAndYAndZAndOmega.Stdev = sqrt((FluenceOfXAndYAndZAndOmega.SecondMoment - real(FluenceOfXAndYAndZAndOmega.Mean) .* real(FluenceOfXAndYAndZAndOmega.Mean) ...
                                                                           - imag(FluenceOfXAndYAndZAndOmega.Mean) .* imag(FluenceOfXAndYAndZAndOmega.Mean)) / json.N);
            end                 
            results{di}.FluenceOfXAndYAndZAndOmega = FluenceOfXAndYAndZAndOmega;
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
            tempData = readBinaryData([datadir slash detector.Name], ...
                [(length(FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1)*...
                 (length(FluenceOfXAndYAndZAndStartingXAndY.X)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.Y)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.Z)-1)]); 
            FluenceOfXAndYAndZAndStartingXAndY.Mean = reshape(tempData, ...% column major json binary
                [length(FluenceOfXAndYAndZAndStartingXAndY.Z)-1,length(FluenceOfXAndYAndZAndStartingXAndY.Y)-1,length(FluenceOfXAndYAndZAndStartingXAndY.X)-1,...
                 length(FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1,length(FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1]);
            FluenceOfXAndYAndZAndStartingXAndY.StartingXYCount = readBinaryData([datadir slash detector.Name '_StartingXYCount'],...
                [length(FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1,length(FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1]); % read column major json binary                      
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ...
                   [(length(FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1)*...
                 (length(FluenceOfXAndYAndZAndStartingXAndY.X)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.Y)-1)*(length(FluenceOfXAndYAndZAndStartingXAndY.Z)-1)]); 
                FluenceOfXAndYAndZAndStartingXAndY.SecondMoment = reshape(tempData, ... % column major json binary
                 [length(FluenceOfXAndYAndZAndStartingXAndY.Z)-1,length(FluenceOfXAndYAndZAndStartingXAndY.Y)-1,length(FluenceOfXAndYAndZAndStartingXAndY.X)-1,...
                 length(FluenceOfXAndYAndZAndStartingXAndY.StartingY)-1,length(FluenceOfXAndYAndZAndStartingXAndY.StartingX)-1]);
                FluenceOfXAndYAndZAndStartingXAndY.Stdev = sqrt((FluenceOfXAndYAndZAndStartingXAndY.SecondMoment -  ...
                      (FluenceOfXAndYAndZAndStartingXAndY.Mean .* FluenceOfXAndYAndZAndStartingXAndY.Mean)) / json.N);
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
            RadianceOfRhoAndZAndAngle.Mean = readBinaryData([datadir slash detector.Name], ... 
                [(length(RadianceOfRhoAndZAndAngle.Rho)-1) * (length(RadianceOfRhoAndZAndAngle.Z)-1) * (length(RadianceOfRhoAndZAndAngle.Angle)-1)]); 
            RadianceOfRhoAndZAndAngle.Mean = reshape(RadianceOfRhoAndZAndAngle.Mean, ...% read column major json binary
                [length(RadianceOfRhoAndZAndAngle.Angle)-1,length(RadianceOfRhoAndZAndAngle.Z)-1,length(RadianceOfRhoAndZAndAngle.Rho)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                RadianceOfRhoAndZAndAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                [(length(RadianceOfRhoAndZAndAngle.Rho)-1) * (length(RadianceOfRhoAndZAndAngle.Z)-1) * (length(RadianceOfRhoAndZAndAngle.Angle)-1)]); 
                RadianceOfRhoAndZAndAngle.SecondMoment = reshape(RadianceOfRhoAndZAndAngle.SecondMoment, ...% read column major json binary
                [length(RadianceOfRhoAndZAndAngle.Angle)-1,length(RadianceOfRhoAndZAndAngle.Z)-1,length(RadianceOfRhoAndZAndAngle.Rho)-1]);  
                RadianceOfRhoAndZAndAngle.Stdev = sqrt((RadianceOfRhoAndZAndAngle.SecondMoment - (RadianceOfRhoAndZAndAngle.Mean .* RadianceOfRhoAndZAndAngle.Mean)) / json.N);               
            end
            results{di}.RadianceOfRhoAndZAndAngle = RadianceOfRhoAndZAndAngle;
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
            tempData = readBinaryData([datadir slash detector.Name], ... 
                [2*(length(RadianceOfFxAndZAndAngle.Fx))*(length(RadianceOfFxAndZAndAngle.Z)-1)*(length(RadianceOfFxAndZAndAngle.Angle)-1)]); 
            tempDataReshape = reshape(tempData, ...% read column major json binary
                [2*(length(RadianceOfFxAndZAndAngle.Angle)-1),length(RadianceOfFxAndZAndAngle.Z)-1,length(RadianceOfFxAndZAndAngle.Fx)]);
            RadianceOfFxAndZAndAngle.Mean = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
            RadianceOfFxAndZAndAngle.Amplitude = abs(RadianceOfFxAndZAndAngle.Mean);
            RadianceOfFxAndZAndAngle.Phase = -angle(RadianceOfFxAndZAndAngle.Mean);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ... 
                    [2*(length(RadianceOfFxAndZAndAngle.Fx)) * (length(RadianceOfFxAndZAndAngle.Z)-1) * (length(RadianceOfFxAndZAndAngle.Angle)-1)]); 
                tempDataReshape = reshape(tempData, ...% read column major json binary
                    [2*(length(RadianceOfFxAndZAndAngle.Angle)-1),length(RadianceOfFxAndZAndAngle.Z)-1,length(RadianceOfFxAndZAndAngle.Fx)]);
                RadianceOfFxAndZAndAngle.SecondMoment = tempDataReshape(1:2:end,:,:) + tempDataReshape(2:2:end,:,:); % SecondMoment=E[re^2]+E[im^2] is real
                % SD=sqrt( SecondMoment - E[re]^2 - E[im]^2 )
                RadianceOfFxAndZAndAngle.Stdev = sqrt((RadianceOfFxAndZAndAngle.SecondMoment - real(RadianceOfFxAndZAndAngle.Mean) .* real(RadianceOfFxAndZAndAngle.Mean)...
                                                                           - imag(RadianceOfFxAndZAndAngle.Mean) .* imag(RadianceOfFxAndZAndAngle.Mean)) / json.N);
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
            RadianceOfXAndYAndZAndThetaAndPhi.Mean = readBinaryData([datadir slash detector.Name], ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1));
            RadianceOfXAndYAndZAndThetaAndPhi.Mean = reshape(RadianceOfXAndYAndZAndThetaAndPhi.Mean, ... % read column major json binary
                [length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1) * ...  
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1));
                RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment = reshape(RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment, ...
                [length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1, ... % read column major json binary
                length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1]);
                RadianceOfXAndYAndZAndThetaAndPhi.Stdev = sqrt((RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment - (RadianceOfXAndYAndZAndThetaAndPhi.Mean .* RadianceOfXAndYAndZAndThetaAndPhi.Mean)) / (json.N));               
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
            ReflectedMTOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1) * (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1));  
            ReflectedMTOfRhoAndSubregionHist.Mean = reshape(ReflectedMTOfRhoAndSubregionHist.Mean, ...
                [length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1,length(ReflectedMTOfRhoAndSubregionHist.Rho)-1]);  % read column major json binary          
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * ...
                (tempFractionalMTBinsLength)); 
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = reshape(ReflectedMTOfRhoAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(tempSubregionIndices), length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1, length(ReflectedMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ReflectedMTOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1) * (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                ReflectedMTOfRhoAndSubregionHist.SecondMoment = reshape(ReflectedMTOfRhoAndSubregionHist.SecondMoment, ...
                [length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1,length(ReflectedMTOfRhoAndSubregionHist.Rho)-1]);  
                ReflectedMTOfRhoAndSubregionHist.Stdev = sqrt((ReflectedMTOfRhoAndSubregionHist.SecondMoment - (ReflectedMTOfRhoAndSubregionHist.Mean .* ReflectedMTOfRhoAndSubregionHist.Mean)) / (N));               
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
            ReflectedMTOfXAndYAndSubregionHist.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ReflectedMTOfXAndYAndSubregionHist.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ReflectedMTOfXAndYAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            ReflectedMTOfXAndYAndSubregionHist.X_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.X(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.X(2:end))/2;
            ReflectedMTOfXAndYAndSubregionHist.Y_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.Y(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.Y(2:end))/2;
            ReflectedMTOfXAndYAndSubregionHist.MTBins_Midpoints = (ReflectedMTOfXAndYAndSubregionHist.MTBins(1:end-1) + ReflectedMTOfXAndYAndSubregionHist.MTBins(2:end))/2;
            ReflectedMTOfXAndYAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.X)-1));  
            ReflectedMTOfXAndYAndSubregionHist.Mean = reshape(ReflectedMTOfXAndYAndSubregionHist.Mean, ...
                [length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1,length(ReflectedMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedMTOfXAndYAndSubregionHist.X)-1]);  % read column major json binary          
            ReflectedMTOfXAndYAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(ReflectedMTOfXAndYAndSubregionHist.X)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * ...
                (tempFractionalMTBinsLength));
            ReflectedMTOfXAndYAndSubregionHist.FractionalMT = reshape(ReflectedMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(tempSubregionIndices), length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1, length(ReflectedMTOfXAndYAndSubregionHist.Y)-1, length(ReflectedMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ReflectedMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                ReflectedMTOfXAndYAndSubregionHist.SecondMoment = reshape(ReflectedMTOfXAndYAndSubregionHist.SecondMoment, ...
                [length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1,length(ReflectedMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedMTOfXAndYAndSubregionHist.X)-1]);  
                ReflectedMTOfXAndYAndSubregionHist.Stdev = sqrt((ReflectedMTOfXAndYAndSubregionHist.SecondMoment - (ReflectedMTOfXAndYAndSubregionHist.Mean .* ReflectedMTOfXAndYAndSubregionHist.Mean)) / (N));               
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
            TransmittedMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            TransmittedMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            TransmittedMTOfRhoAndSubregionHist.Rho_Midpoints = (TransmittedMTOfRhoAndSubregionHist.Rho(1:end-1) + TransmittedMTOfRhoAndSubregionHist.Rho(2:end))/2;
            TransmittedMTOfRhoAndSubregionHist.MTBins_Midpoints = (TransmittedMTOfRhoAndSubregionHist.MTBins(1:end-1) + TransmittedMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            TransmittedMTOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1) * (length(TransmittedMTOfRhoAndSubregionHist.Rho)-1));  
            TransmittedMTOfRhoAndSubregionHist.Mean = reshape(TransmittedMTOfRhoAndSubregionHist.Mean, ...
                [length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1,length(TransmittedMTOfRhoAndSubregionHist.Rho)-1]);  % read column major json binary          
            TransmittedMTOfRhoAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(TransmittedMTOfRhoAndSubregionHist.Rho)-1) * (length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * ...
                (tempFractionalMTBinsLength)); 
            TransmittedMTOfRhoAndSubregionHist.FractionalMT = reshape(TransmittedMTOfRhoAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(tempSubregionIndices), length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1, length(TransmittedMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TransmittedMTOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1) * (length(TransmittedMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                TransmittedMTOfRhoAndSubregionHist.SecondMoment = reshape(TransmittedMTOfRhoAndSubregionHist.SecondMoment, ...
                [length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1,length(TransmittedMTOfRhoAndSubregionHist.Rho)-1]);  
                TransmittedMTOfRhoAndSubregionHist.Stdev = sqrt((TransmittedMTOfRhoAndSubregionHist.SecondMoment - (TransmittedMTOfRhoAndSubregionHist.Mean .* TransmittedMTOfRhoAndSubregionHist.Mean)) / (N));               
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
            TransmittedMTOfXAndYAndSubregionHist.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            TransmittedMTOfXAndYAndSubregionHist.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            TransmittedMTOfXAndYAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedMTOfXAndYAndSubregionHist.SubregionIndices = tempSubregionIndices;
            TransmittedMTOfXAndYAndSubregionHist.X_Midpoints = (TransmittedMTOfXAndYAndSubregionHist.X(1:end-1) + TransmittedMTOfXAndYAndSubregionHist.X(2:end))/2;
            TransmittedMTOfXAndYAndSubregionHist.Y_Midpoints = (TransmittedMTOfXAndYAndSubregionHist.Y(1:end-1) + TransmittedMTOfXAndYAndSubregionHist.Y(2:end))/2;
            TransmittedMTOfXAndYAndSubregionHist.MTBins_Midpoints = (TransmittedMTOfXAndYAndSubregionHist.MTBins(1:end-1) + TransmittedMTOfXAndYAndSubregionHist.MTBins(2:end))/2;
            TransmittedMTOfXAndYAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.X)-1));  
            TransmittedMTOfXAndYAndSubregionHist.Mean = reshape(TransmittedMTOfXAndYAndSubregionHist.Mean, ...
                [length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1,length(TransmittedMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedMTOfXAndYAndSubregionHist.X)-1]);  % read column major json binary          
            TransmittedMTOfXAndYAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(TransmittedMTOfXAndYAndSubregionHist.X)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * ...
                (tempFractionalMTBinsLength)); 
            TransmittedMTOfXAndYAndSubregionHist.FractionalMT = reshape(TransmittedMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(tempSubregionIndices), length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1, length(TransmittedMTOfXAndYAndSubregionHist.Y)-1, length(TransmittedMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TransmittedMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                TransmittedMTOfXAndYAndSubregionHist.SecondMoment = reshape(TransmittedMTOfXAndYAndSubregionHist.SecondMoment, ...
                [length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1,length(TransmittedMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedMTOfXAndYAndSubregionHist.X)-1]);  
                TransmittedMTOfXAndYAndSubregionHist.Stdev = sqrt((TransmittedMTOfXAndYAndSubregionHist.SecondMoment - (TransmittedMTOfXAndYAndSubregionHist.Mean .* TransmittedMTOfXAndYAndSubregionHist.Mean)) / (N));               
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
            ReflectedDynamicMTOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1));  
            ReflectedDynamicMTOfRhoAndSubregionHist.Mean = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.Mean, ...
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1,length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  % read column major json binary          
            ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * ...
                (tempFractionalMTBinsLength)); 
            ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1, length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1)); 
            ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ, ...            
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1, length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1)); 
            ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ, ...            
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1, length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary    
            ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment, ...
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.MTBins)-1,length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                ReflectedDynamicMTOfRhoAndSubregionHist.Stdev = sqrt((ReflectedDynamicMTOfRhoAndSubregionHist.SecondMoment - (ReflectedDynamicMTOfRhoAndSubregionHist.Mean .* ReflectedDynamicMTOfRhoAndSubregionHist.Mean)) / (N));               
                % depth dependent output
                ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment, ...
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1,length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZStdev = sqrt((ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment - (ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ .* ReflectedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ)) / (N));               
                ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                (length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = reshape(ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment, ...
                [length(ReflectedDynamicMTOfRhoAndSubregionHist.Z)-1,length(ReflectedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZStdev = sqrt((ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment - (ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ .* ReflectedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ)) / (N));               
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
            ReflectedDynamicMTOfXAndYAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1));  
            ReflectedDynamicMTOfXAndYAndSubregionHist.Mean = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.Mean, ...
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]);  % read column major json binary          
            ReflectedDynamicMTOfXAndYAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * ...
                (tempFractionalMTBinsLength));
            ReflectedDynamicMTOfXAndYAndSubregionHist.FractionalMT = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1, length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1, length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * ...
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1)); 
            ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ, ...            
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1, length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1, ...
                length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * ...
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1)); 
            ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ, ...            
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1, length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1,...
                length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary    
            ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment, ...
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                ReflectedDynamicMTOfXAndYAndSubregionHist.Stdev = sqrt((ReflectedDynamicMTOfXAndYAndSubregionHist.SecondMoment - (ReflectedDynamicMTOfXAndYAndSubregionHist.Mean .* ReflectedDynamicMTOfXAndYAndSubregionHist.Mean)) / (N));               
                % depth dependent output
                ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment, ...
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZStdev = sqrt((ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment - (ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ .* ReflectedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ)) / (N));               
                ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1) *length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1) % read column major json binary
                ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = reshape(ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment, ...
                [length(ReflectedDynamicMTOfXAndYAndSubregionHist.Z)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(ReflectedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZStdev = sqrt((ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment - (ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ .* ReflectedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ)) / (N));               
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
            tempData = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1)*(2*length(ReflectedDynamicMTOfFxAndSubregionHist.Fx))); 
            % NOTE! reshape with 2x dim of var in inner loop binaryWrite
            tempDataReshape = reshape(tempData, ...
                [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1),length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]);
            ReflectedDynamicMTOfFxAndSubregionHist.Mean = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);           
            tempData = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                2* length(ReflectedDynamicMTOfFxAndSubregionHist.Fx) * (length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1) * ...
                tempFractionalMTBinsLength); 
            tempDataReshape = reshape(tempData, ...            
                [2*tempFractionalMTBinsLength, length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1, length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]); % read column major json binary
            ReflectedDynamicMTOfFxAndSubregionHist.FractionalMT = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
            tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                (2*length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)) * (length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1)); 
            tempDataReshape = reshape(tempData, ...            
                [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1), length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]);  
            ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
            tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                2* length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)*(length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1)); 
            tempDataReshape = reshape(tempData, ...            
                [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1), length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]);
            ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);   
            ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions = reshape(ReflectedDynamicMTOfFxAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'], ... 
                  (length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1) * 2* length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.MTBins)-1),length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]);  
                ReflectedDynamicMTOfFxAndSubregionHist.SecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
                ReflectedDynamicMTOfFxAndSubregionHist.Stdev = sqrt((abs(ReflectedDynamicMTOfFxAndSubregionHist.SecondMoment) - ...
                    (abs(ReflectedDynamicMTOfFxAndSubregionHist.Mean) .* abs(ReflectedDynamicMTOfFxAndSubregionHist.Mean))) / (N));               
                % depth dependent output
                tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                  (length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1) * 2 * length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1),length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]); 
                ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
                ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZStdev = sqrt((abs(ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZSecondMoment) - ...
                    (abs(ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ) .* abs(ReflectedDynamicMTOfFxAndSubregionHist.TotalMTOfZ))) / (N));               
                tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                  (length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1) * 2*length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)); % read column major json binary
                tempDataReshape = reshape(tempData, ...
                  [2*(length(ReflectedDynamicMTOfFxAndSubregionHist.Z)-1),length(ReflectedDynamicMTOfFxAndSubregionHist.Fx)]);  
                ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
                ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZStdev = sqrt((ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZSecondMoment - (ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ .* ReflectedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ)) / (N));               
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
            TransmittedDynamicMTOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1));  
            TransmittedDynamicMTOfRhoAndSubregionHist.Mean = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.Mean, ...
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1,length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  % read column major json binary          
            TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * ...
                (tempFractionalMTBinsLength)); 
            TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1, length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1)); 
            TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ, ...            
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1, length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
            TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1)); 
            TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ, ...            
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1, length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary    
            TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment, ...
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.MTBins)-1,length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                TransmittedDynamicMTOfRhoAndSubregionHist.Stdev = sqrt((TransmittedDynamicMTOfRhoAndSubregionHist.SecondMoment - (TransmittedDynamicMTOfRhoAndSubregionHist.Mean .* TransmittedDynamicMTOfRhoAndSubregionHist.Mean)) / (N));               
                % depth dependent output
                TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment, ...
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1,length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZStdev = sqrt((TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZSecondMoment - (TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ .* TransmittedDynamicMTOfRhoAndSubregionHist.TotalMTOfZ)) / (N));               
                TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                (length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1)); % read column major json binary
                TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment = reshape(TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment, ...
                [length(TransmittedDynamicMTOfRhoAndSubregionHist.Z)-1,length(TransmittedDynamicMTOfRhoAndSubregionHist.Rho)-1]);  
                TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZStdev = sqrt((TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZSecondMoment - (TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ .* TransmittedDynamicMTOfRhoAndSubregionHist.DynamicMTOfZ)) / (N));               
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
            TransmittedDynamicMTOfXAndYAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1));  
            TransmittedDynamicMTOfXAndYAndSubregionHist.Mean = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.Mean, ...
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]);  % read column major json binary          
            TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * ...
                (tempFractionalMTBinsLength)); 
            TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [tempFractionalMTBinsLength, length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1, length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1, length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * ...
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1)); 
            TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ, ...            
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1, length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1, ...
                length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * ...
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1)); 
            TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ, ...            
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1, length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1,...
                length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary    
            TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment, ...
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.MTBins)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                TransmittedDynamicMTOfXAndYAndSubregionHist.Stdev = sqrt((TransmittedDynamicMTOfXAndYAndSubregionHist.SecondMoment - (TransmittedDynamicMTOfXAndYAndSubregionHist.Mean .* TransmittedDynamicMTOfXAndYAndSubregionHist.Mean)) / (N));               
                % depth dependent output
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_TotalMTOfZ_2'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment, ...
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZStdev = sqrt((TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZSecondMoment - (TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ .* TransmittedDynamicMTOfXAndYAndSubregionHist.TotalMTOfZ)) / (N));               
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ_2'], ... 
                (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1) * (length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1) *length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1) % read column major json binary
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment = reshape(TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment, ...
                [length(TransmittedDynamicMTOfXAndYAndSubregionHist.Z)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedDynamicMTOfXAndYAndSubregionHist.X)-1]);  
                TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZStdev = sqrt((TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZSecondMoment - (TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ .* TransmittedDynamicMTOfXAndYAndSubregionHist.DynamicMTOfZ)) / (N));               
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
            tempData = readBinaryData([datadir slash detector.Name], ... 
                (length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1) * 2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx));  
            tempDataReshape = reshape(tempData, ...
                [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1),length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]);  % read column major json binary          
            TransmittedDynamicMTOfFxAndSubregionHist.Mean = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
            tempData = readBinaryData([datadir slash detector.Name '_FractionalMT'], ... 
                2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx) * (length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1) * ...
                (tempFractionalMTBinsLength)); 
            tempDataReshape = reshape(tempData, ...            
                [2*tempFractionalMTBinsLength, length(TransmittedDynamicMTOfFxAndSubregionHist.MTBins)-1, length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.FractionalMT = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
            tempData = readBinaryData([datadir slash detector.Name '_TotalMTOfZ'], ... 
                2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx) * (length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1)); 
            tempDataReshape = reshape(tempData, ...            
                [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1), length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]); % read column major json binary
            TransmittedDynamicMTOfFxAndSubregionHist.TotalMTOfZ = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
            tempData = readBinaryData([datadir slash detector.Name '_DynamicMTOfZ'], ... 
                2*length(TransmittedDynamicMTOfFxAndSubregionHist.Fx) * (length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1)); 
            tempDataReshape = reshape(tempData, ...            
                [2*(length(TransmittedDynamicMTOfFxAndSubregionHist.Z)-1), length(TransmittedDynamicMTOfFxAndSubregionHist.Fx)]); % read column major json binary    
            TransmittedDynamicMTOfFxAndSubregionHist.DynamicMTOfZ = tempDataReshape(1:2:end,:) + 1i*tempDataReshape(2:2:end,:);
            TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions = readBinaryData([datadir slash detector.Name '_SubregionCollisions'], ... 
                (length(tempSubregionIndices) * 2)); % 2 for static vs dynamic tallies
            TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions = reshape(TransmittedDynamicMTOfFxAndSubregionHist.SubregionCollisions, ...            
                [2, length(tempSubregionIndices)]); % read column major json binary    
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
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
            ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedTimeOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedTimeOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints = (ReflectedTimeOfRhoAndSubregionHist.Time(1:end-1) + ReflectedTimeOfRhoAndSubregionHist.Time(2:end))/2;
            ReflectedTimeOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detector.Name], ... 
                (length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1) * (length(tempSubregionIndices)) * (length(ReflectedTimeOfRhoAndSubregionHist.Time)-1));
            ReflectedTimeOfRhoAndSubregionHist.Mean = reshape(ReflectedTimeOfRhoAndSubregionHist.Mean, ...
                [length(ReflectedTimeOfRhoAndSubregionHist.Time)-1,length(tempSubregionIndices),length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1]); % column major json binary
            ReflectedTimeOfRhoAndSubregionHist.FractionalTime = readBinaryData([datadir slash detector.Name '_FractionalTime'], ... 
                [length(tempSubregionIndices), length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1]);
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                [(length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1) * (length(tempSubregionIndices)) * (length(ReflectedTimeOfRhoAndSubregionHist.Time)-1)]); 
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = reshape(ReflectedTimeOfRhoAndSubregionHist.SecondMoment, ...
                [length(ReflectedTimeOfRhoAndSubregionHist.Time)-1,length(tempSubregionIndices),length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1]);  % column major json binary
                ReflectedTimeOfRhoAndSubregionHist.Stdev = sqrt((ReflectedTimeOfRhoAndSubregionHist.SecondMoment - (ReflectedTimeOfRhoAndSubregionHist.Mean .* ReflectedTimeOfRhoAndSubregionHist.Mean)) / (N));               
            end
            results{di}.ReflectedTimeOfRhoAndSubregionHist = ReflectedTimeOfRhoAndSubregionHist;
      case 'pMCATotal'
            pMCAtotal.Name = detector.Name;           
            pMCATotal_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            pMCATotal.Mean = pMCATotal_txt.Mean;              
            pMCATotal.SecondMoment = pMCATotal_txt.SecondMoment; 
            pMCATotal.Stdev = sqrt((pMCATotal.SecondMoment - (pMCATotal.Mean .* pMCATotal.Mean)) / (databaseInputJson.N));
            results{di}.pMCATotal = pMCATotal;           
        case 'pMCROfRho'
            pMCROfRho.Name = detector.Name;
            tempRho = detector.Rho;
            pMCROfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRho.Rho_Midpoints = (pMCROfRho.Rho(1:end-1) + pMCROfRho.Rho(2:end))/2;
            pMCROfRho.Mean = readBinaryData([datadir slash detector.Name],length(pMCROfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(pMCROfRho.Rho)-1);
                pMCROfRho.Stdev = sqrt((pMCROfRho.SecondMoment - (pMCROfRho.Mean .* pMCROfRho.Mean)) / (databaseInputJson.N));
            end
            results{di}.pMCROfRho = pMCROfRho;
        case 'pMCROfRhoRecessed'
            pMCROfRhoRecessed.Name = detector.Name;
            tempRho = detector.Rho;
            pMCROfRhoRecessed.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCROfRhoRecessed.Rho_Midpoints = (pMCROfRhoRecessed.Rho(1:end-1) + pMCROfRhoRecessed.Rho(2:end))/2;
            pMCROfRhoRecessed.Mean = readBinaryData([datadir slash detector.Name],length(pMCROfRhoRecessed.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfRhoRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(pMCROfRhoRecessed.Rho)-1);
                pMCROfRhoRecessed.Stdev = sqrt((pMCROfRhoRecessed.SecondMoment - (pMCROfRhoRecessed.Mean .* pMCROfRhoRecessed.Mean)) / (databaseInputJson.N));
            end
            results{di}.pMCROfRhoRecessed = pMCROfRhoRecessed;
      case 'dMCdROfRhodMua'
            dMCdROfRhodMua.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhodMua.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhodMua.Rho_Midpoints = (dMCdROfRhodMua.Rho(1:end-1) + dMCdROfRhodMua.Rho(2:end))/2;
            dMCdROfRhodMua.Mean = readBinaryData([datadir slash detector.Name],length(dMCdROfRhodMua.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                dMCdROfRhodMua.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(dMCdROfRhodMua.Rho)-1);
                dMCROfRhodMua.Stdev = sqrt((dMCdROfRhodMua.SecondMoment - (dMCdROfRhodMua.Mean .* dMCdROfRhodMua.Mean)) / (databaseInputJson.N));
            end
            results{di}.dMCdROfRhodMua = dMCdROfRhodMua;
      case 'dMCdROfRhodMus'
            dMCdROfRhodMus.Name = detector.Name;
            tempRho = detector.Rho;
            dMCdROfRhodMus.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            dMCdROfRhodMus.Rho_Midpoints = (dMCdROfRhodMus.Rho(1:end-1) + dMCdROfRhodMus.Rho(2:end))/2;
            dMCdROfRhodMus.Mean = readBinaryData([datadir slash detector.Name],length(dMCdROfRhodMus.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                dMCdROfRhodMus.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(dMCdROfRhodMus.Rho)-1);
                dMCROfRhodMus.Stdev = sqrt((dMCdROfRhodMus.SecondMoment - (dMCdROfRhodMus.Mean .* dMCdROfRhodMus.Mean)) / (databaseInputJson.N));
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
            pMCROfRhoAndTime.Mean = readBinaryData([datadir slash detector.Name],[length(pMCROfRhoAndTime.Time)-1,length(pMCROfRhoAndTime.Rho)-1]); % read column major json binary            
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfRhoAndTime.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(pMCROfRhoAndTime.Time)-1,length(pMCROfRhoAndTime.Rho)-1]);
                pMCROfRhoAndTime.Stdev = sqrt((pMCROfRhoAndTime.SecondMoment - (pMCROfRhoAndTime.Mean .* pMCROfRhoAndTime.Mean)) / (databaseInputJson.N));
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
            pMCROfRhoAndTimeRecessed.Mean = readBinaryData([datadir slash detector.Name],[length(pMCROfRhoAndTimeRecessed.Time)-1,length(pMCROfRhoAndTimeRecessed.Rho)-1]); % read column major json binary            
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfRhoAndTimeRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(pMCROfRhoAndTimeRecessed.Time)-1,length(pMCROfRhoAndTimeRecessed.Rho)-1]);
                pMCROfRhoAndTimeRecessed.Stdev = sqrt((pMCROfRhoAndTimeRecessed.SecondMoment - (pMCROfRhoAndTimeRecessed.Mean .* pMCROfRhoAndTimeRecessed.Mean)) / (databaseInputJson.N));
            end
            results{di}.pMCROfRhoAndTimeRecessed = pMCROfRhoAndTimeRecessed;
         case 'pMCROfXAndY'
            pMCROfXAndY.Name = detector.Name;
            tempX = detector.X;
            tempY = detector.Y;
            pMCROfXAndY.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            pMCROfXAndY.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            pMCROfXAndY.X_Midpoints = (pMCROfXAndY.X(1:end-1) + pMCROfXAndY.X(2:end))/2;
            pMCROfXAndY.Y_Midpoints = (pMCROfXAndY.Y(1:end-1) + pMCROfXAndY.Y(2:end))/2;
            pMCROfXAndY.Mean = readBinaryData([datadir slash detector.Name],[length(pMCROfXAndY.Y)-1,length(pMCROfXAndY.X)-1]); % read column major json binary            
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfXAndY.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],[length(pMCROfXAndY.Y)-1,length(pMCROfXAndY.X)-1]);
                pMCROfXAndY.Stdev = sqrt((pMCROfXAndY.SecondMoment - (pMCROfXAndY.Mean .* pMCROfXAndY.Mean)) / (databaseInputJson.N));
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
            pMCROfXAndYAndTimeAndSubregion.Mean = readBinaryData([datadir slash detector.Name],...
                [tempNumberOfRegions*(length(pMCROfXAndYAndTimeAndSubregion.Time)-1)*(length(pMCROfXAndYAndTimeAndSubregion.Y)-1)*(length(pMCROfXAndYAndTimeAndSubregion.X)-1)]); % read column major json binary            
            pMCROfXAndYAndTimeAndSubregion.Mean = reshape(pMCROfXAndYAndTimeAndSubregion.Mean,...
                [tempNumberOfRegions,length(pMCROfXAndYAndTimeAndSubregion.Time)-1,length(pMCROfXAndYAndTimeAndSubregion.Y)-1,length(pMCROfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
            pMCROfXAndYAndTimeAndSubregion.ROfXAndY = readBinaryData([datadir slash detector.Name '_ROfXAndY'],[length(pMCROfXAndYAndTimeAndSubregion.Y)-1,length(pMCROfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary                 
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfXAndYAndTimeAndSubregion.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [tempNumberOfRegions*(length(pMCROfXAndYAndTimeAndSubregion.Time)-1)*(length(pMCROfXAndYAndTimeAndSubregion.Y)-1)*(length(pMCROfXAndYAndTimeAndSubregion.X)-1)]);
                pMCROfXAndYAndTimeAndSubregion.SecondMoment = reshape(pMCROfXAndYAndTimeAndSubregion.SecondMoment,...
                  [tempNumberOfRegions,length(pMCROfXAndYAndTimeAndSubregion.Time)-1,length(pMCROfXAndYAndTimeAndSubregion.Y)-1,length(pMCROfXAndYAndTimeAndSubregion.X)-1]); % read column major json binary            
                pMCROfXAndYAndTimeAndSubregion.Stdev = sqrt((pMCROfXAndYAndTimeAndSubregion.SecondMoment - (pMCROfXAndYAndTimeAndSubregion.Mean .* pMCROfXAndYAndTimeAndSubregion.Mean)) / (databaseInputJson.N));
                pMCROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment = readBinaryData([datadir slash detector.Name '_ROfXAndY_2'],[length(pMCROfXAndYAndTimeAndSubregion.Y)-1,length(pMCROfXAndYAndTimeAndSubregion.X)-1]);  % read column major json binary          
                pMCROfXAndYAndTimeAndSubregion.ROfXAndYStdev = sqrt((pMCROfXAndYAndTimeAndSubregion.ROfXAndYSecondMoment - (pMCROfXAndYAndTimeAndSubregion.ROfXAndY .* pMCROfXAndYAndTimeAndSubregion.ROfXAndY)) / (databaseInputJson.N));
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
            pMCROfXAndYAndTimeAndSubregionRecessed.Mean = readBinaryData([datadir slash detector.Name],...
                [tempNumberOfRegions*(length(pMCROfXAndYAndTimeAndSubregionRecessed.Time)-1)*(length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1)*(length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1)]); % read column major json binary            
            pMCROfXAndYAndTimeAndSubregionRecessed.Mean = reshape(pMCROfXAndYAndTimeAndSubregionRecessed.Mean,...
                [tempNumberOfRegions,length(pMCROfXAndYAndTimeAndSubregionRecessed.Time)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1]); % read column major json binary            
            pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY = readBinaryData([datadir slash detector.Name '_ROfXAndY'],[length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1]);  % read column major json binary          
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],...
                  [tempNumberOfRegions*(length(pMCROfXAndYAndTimeAndSubregionRecessed.Time)-1)*(length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1)*(length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1)]);
                pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment = reshape(pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment,...
                  [tempNumberOfRegions,length(pMCROfXAndYAndTimeAndSubregionRecessed.Time)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1]); % read column major json binary            
                pMCROfXAndYAndTimeAndSubregionRecessed.Stdev = sqrt((pMCROfXAndYAndTimeAndSubregionRecessed.SecondMoment - (pMCROfXAndYAndTimeAndSubregionRecessed.Mean .* pMCROfXAndYAndTimeAndSubregionRecessed.Mean)) / (databaseInputJson.N));
                pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment = readBinaryData([datadir slash detector.Name '_ROfXAndY_2'],[length(pMCROfXAndYAndTimeAndSubregionRecessed.Y)-1,length(pMCROfXAndYAndTimeAndSubregionRecessed.X)-1]);  % read column major json binary          
                pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYStdev = sqrt((pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndYSecondMoment - (pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY .* pMCROfXAndYAndTimeAndSubregionRecessed.ROfXAndY)) / (databaseInputJson.N));
           end
            results{di}.pMCROfXAndYAndTimeAndSubregionRecessed = pMCROfXAndYAndTimeAndSubregionRecessed;
        case 'pMCROfFx'
            pMCROfFx.Name = detector.Name;
            tempFx = detector.Fx;
            pMCROfFx.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            pMCROfFx.Fx_Midpoints = pMCROfFx.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(pMCROfFx.Fx)); 
            pMCROfFx.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);                       
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],2*length(pMCROfFx.Fx));
                pMCROfFx.SecondMoment = tempData(1:2:end) + 1i*tempData(2:2:end);
                pMCROfFx.Stdev = sqrt((pMCROfFx.SecondMoment + real(pMCROfFx.Mean) .* real(pMCROfFx.Mean) + ...
                    imag(pMCROfFx.Mean) .* imag(pMCROfFx.Mean)) / databaseInputJson.N);
            end
            results{di}.pMCROfFx = pMCROfFx;
        case 'pMCTOfRho'
            pMCTOfRho.Name = detector.Name;
            tempRho = detector.Rho;
            pMCTOfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            pMCTOfRho.Rho_Midpoints = (pMCTOfRho.Rho(1:end-1) + pMCTOfRho.Rho(2:end))/2;
            pMCTOfRho.Mean = readBinaryData([datadir slash detector.Name],length(pMCTOfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                pMCTOfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(pMCTOfRho.Rho)-1);
                pMCTOfRho.Stdev = sqrt((pMCTOfRho.SecondMoment - (pMCTOfRho.Mean .* pMCTOfRho.Mean)) / (databaseInputJson.N));
            end
            results{di}.pMCTOfRho = pMCTOfRho;
    end %detector.Name switch
end

function json_parsed = readAndParseJson(filename)
json_parsed = loadjson(filename);

