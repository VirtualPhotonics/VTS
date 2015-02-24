function results = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

%json = readAndParseJson([datadir slash 'infile_' dataname '.txt']);
json = readAndParseJson([datadir slash dataname '.txt'])
postProcessorResults = false;
if (exist([datadir slash dataname '_database_infile.txt'],'file'))
    postProcessorResults = true;
    databaseInputJson = readAndParseJson([datadir slash dataname '_database_infile.txt']);
end
numDetectors = length(json.DetectorInputs);
for di = 1:numDetectors
    detector = json.DetectorInputs{di};
    switch(detector.TallyType)
        case 'RDiffuse'
            RDiffuse.Name = detector.Name;
            RDiffuse_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            RDiffuse.Mean = RDiffuse_txt.Mean;              
            RDiffuse.SecondMoment = RDiffuse_txt.SecondMoment;
            RDiffuse.Stdev = sqrt((RDiffuse.SecondMoment - (RDiffuse.Mean .* RDiffuse.Mean)) / (json.N)); 
            results{di}.RDiffuse = RDiffuse;
        case 'RSpecular'
            RSpecular.Name = detector.Name;
            RSpecular_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            RSpecular.Mean = RSpecular_txt.Mean;              
            RSpecular.SecondMoment = RSpecular_txt.SecondMoment;
            RSpecular.Stdev = sqrt((RSpecular.SecondMoment - (RSpecular.Mean .* RSpecular.Mean)) / (json.N)); 
            results{di}.RSpecular = RSpecular;
        case 'ROfRho'
            ROfRho.Name = detector.Name;
            tempRho = detector.Rho;
            ROfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            ROfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + ROfRho.Rho(2:end))/2;
            ROfRho.Mean = readBinaryData([datadir slash detector.Name],length(ROfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(ROfRho.Rho)-1);
                ROfRho.Stdev = sqrt((ROfRho.SecondMoment - (ROfRho.Mean .* ROfRho.Mean)) / (json.N));
            end
            results{di}.ROfRho = ROfRho;
        case 'ROfAngle'
            ROfAngle.Name = detector.Name;
            tempAngle = detector.Angle;
            ROfAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            ROfAngle.Angle_Midpoints = (ROfAngle.Angle(1:end-1) + ROfAngle.Angle(2:end))/2;
            ROfAngle.Mean = readBinaryData([datadir slash detector.Name],length(ROfAngle.Angle)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                ROfAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(ROfAngle.Angle)-1);
                ROfAngle.Stdev = sqrt((ROfAngle.SecondMoment - (ROfAngle.Mean .* ROfAngle.Mean)) / (json.N));
            end
            results{di}.ROfAngle = ROfAngle;
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
                ROfXAndY.Stdev = sqrt((ROfXAndY.SecondMoment - (ROfXAndY.Mean .* ROfXAndY.Mean)) / (json.N)); 
            end      
            results{di}.ROfXAndY = ROfXAndY;

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
                ROfRhoAndTime.Stdev = sqrt((ROfRhoAndTime.SecondMoment - (ROfRhoAndTime.Mean .* ROfRhoAndTime.Mean)) / (json.N));
            end
            results{di}.ROfRhoAndTime = ROfRhoAndTime;
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
                ROfRhoAndAngle.Stdev = sqrt((ROfRhoAndAngle.SecondMoment - (ROfRhoAndAngle.Mean .* ROfRhoAndAngle.Mean)) / (json.N)); 
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
                ROfRhoAndOmega.SecondMoment =  tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
                ROfRhoAndOmega.Stdev = sqrt((real(ROfRhoAndOmega.SecondMoment) - (real(ROfRhoAndOmega.Mean) .* real(ROfRhoAndOmega.Mean))) / (json.N)) + ...
                    1i*sqrt((imag(ROfRhoAndOmega.SecondMoment) - (imag(ROfRhoAndOmega.Mean) .* imag(ROfRhoAndOmega.Mean))) / (json.N));
            end            
            results{di}.ROfRhoAndOmega = ROfRhoAndOmega;

        case 'TDiffuse'
            TDiffuse.Name = detector.Name;
            TDiffuse_txt = readAndParseJson([datadir slash detector.Name '.txt']);
            TDiffuse.Mean = TDiffuse_txt.Mean;              
            TDiffuse.SecondMoment = TDiffuse_txt.SecondMoment; 
            TDiffuse.Stdev = sqrt((TDiffuse.SecondMoment - (TDiffuse.Mean .* TDiffuse.Mean)) / (json.N));
            results{di}.TDiffuse = TDiffuse;
        case 'TOfRho'
            TOfRho.Name = detector.Name;
            tempRho = detector.Rho;
            TOfRho.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));
            TOfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + TOfRho.Rho(2:end))/2;
            TOfRho.Mean = readBinaryData([datadir slash detector.Name],length(TOfRho.Rho)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfRho.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(TOfRho.Rho)-1);
                TOfRho.Stdev = sqrt((TOfRho.SecondMoment - (TOfRho.Mean .* TOfRho.Mean)) / (json.N));
            end
            results{di}.TOfRho = TOfRho;
        case 'TOfAngle'
            TOfAngle.Name = detector.Name;
            tempAngle = detector.Angle;
            TOfAngle.Angle = linspace((tempAngle.Start), (tempAngle.Stop), (tempAngle.Count));
            TOfAngle.Angle_Midpoints = (TOfAngle.Angle(1:end-1) + TOfAngle.Angle(2:end))/2;
            TOfAngle.Mean = readBinaryData([datadir slash detector.Name],length(ROfAngle.Angle)-1);              
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TOfAngle.SecondMoment = readBinaryData([datadir slash detector.Name '_2'],length(TOfAngle.Angle)-1);
                TOfAngle.Stdev = sqrt((TOfAngle.SecondMoment - (TOfAngle.Mean .* TOfAngle.Mean)) / (json.N));
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
                TOfRhoAndAngle.Stdev = sqrt((TOfRhoAndAngle.SecondMoment - (TOfRhoAndAngle.Mean .* TOfRhoAndAngle.Mean)) / (json.N)); 
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
                TOfXAndY.Stdev = sqrt((TOfXAndY.SecondMoment - (TOfXAndY.Mean .* TOfXAndY.Mean)) / (json.N)); 
            end      
            results{di}.TOfXAndY = TOfXAndY;
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
                AOfRhoAndZ.Stdev = sqrt((AOfRhoAndZ.SecondMoment - (AOfRhoAndZ.Mean .* AOfRhoAndZ.Mean)) / (json.N)); 
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
                AOfXAndAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(AOfXAndYAndZ.X)-1) * (length(AOfXAndYAndZ.Y)-1) * (length(AOfXAndYAndZ.Z)-1)]);
                AOfXAndYAndZ.SecondMoment = reshape(AOfXAndYAndZ.Mean, ... % column major json binary
                    [length(AOfXAndYAndZ.Z)-1,length(AOfXAndYAndZ.Y)-1,length(AOfXAndYAndZ.X)-1]);
                AOfXAndYAndZ.Stdev = sqrt((AOfXAndYAndZ.SecondMoment - (AOfXAndYAndZ.Mean .* AOfXAndYAndZ.Mean)) / (json.N));  
            end
            results{di}.AOfXAndYAndZ = AOfXAndYAndZ
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
                FluenceOfRhoAndZ.Stdev = sqrt((FluenceOfRhoAndZ.SecondMoment - (FluenceOfRhoAndZ.Mean .* FluenceOfRhoAndZ.Mean)) / (json.N));  
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
                FluenceOfRhoAndZAndTime.SecondMoment = reshape(FluenceOfRhoAndZAndTime.Mean, ... % column major json binary
                    [length(FluenceOfRhoAndZAndTime.Time)-1,length(FluenceOfRhoAndZAndTime.Z)-1,length(FluenceOfRhoAndZAndTime.Rho)-1]);
                FluenceOfRhoAndZAndTime.Stdev = sqrt((FluenceOfRhoAndZAndTime.SecondMoment - (FluenceOfRhoAndZAndTime.Mean .* FluenceOfRhoAndZAndTime.Mean)) / (json.N));  
            end
            results{di}.FluenceOfRhoAndZAndTime = FluenceOfRhoAndZAndTime;
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
                FluenceOfXAndAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]);
                FluenceOfXAndYAndZ.SecondMoment = reshape(FluenceOfXAndYAndZ.Mean, ... % column major json binary
                    [length(FluenceOfXAndYAndZ.Z)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.X)-1]);
                FluenceOfXAndYAndZ.Stdev = sqrt((FluenceOfXAndYAndZ.SecondMoment - (FluenceOfXAndYAndZ.Mean .* FluenceOfXAndYAndZ.Mean)) / (json.N));  
            end
            results{di}.FluenceOfXAndYAndZ = FluenceOfXAndYAndZ;
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
                RadianceOfRhoAndZAndAngle.Stdev = sqrt((RadianceOfRhoAndZAndAngle.SecondMoment - (RadianceOfRhoAndZAndAngle.Mean .* RadianceOfRhoAndZAndAngle.Mean)) / (json.N));               
            end
            results{di}.RadianceOfRhoAndZAndAngle = RadianceOfRhoAndZAndAngle;
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
                [length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1,  % read column major json binary
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
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
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
                (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * 10);
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = reshape(ReflectedMTOfRhoAndSubregionHist.FractionalMT, ...            
                [10, length(tempSubregionIndices), length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1, length(ReflectedMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
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
            tempMTBins = detector.MTBins;
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
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
                (length(ReflectedMTOfXAndYAndSubregionHist.X)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.Y)-1) * (length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * 10);
            ReflectedMTOfXAndYAndSubregionHist.FractionalMT = reshape(ReflectedMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [10, length(tempSubregionIndices), length(ReflectedMTOfXAndYAndSubregionHist.MTBins)-1, length(ReflectedMTOfXAndYAndSubregionHist.Y)-1, length(ReflectedMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
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
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
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
                (length(TransmittedMTOfRhoAndSubregionHist.Rho)-1) * (length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * 10);
            TransmittedMTOfRhoAndSubregionHist.FractionalMT = reshape(TransmittedMTOfRhoAndSubregionHist.FractionalMT, ...            
                [10, length(tempSubregionIndices), length(TransmittedMTOfRhoAndSubregionHist.MTBins)-1, length(TransmittedMTOfRhoAndSubregionHist.Rho)-1]); % read column major json binary
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
            tempMTBins = detector.MTBins;
            if (postProcessorResults)
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer')) 
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.Regions));
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(databaseInputJson.TissueInput.LayerRegions)+length(databaseInputJson.TissueInput.EllipsoidRegion));                            
                end
                N = databaseInputJson.N;
            else   
                if (strcmp(json.TissueInput.TissueType, 'MultiLayer'))           
                    tempSubregionIndices = (1:1:length(json.TissueInput.Regions)); 
                elseif (strcmp(json.TissueInput.TissueType, 'SingleEllipsoid'))
                    tempSubregionIndices = (1:1:length(json.TissueInput.LayerRegions)+length(json.TissueInput.EllipsoidRegion));                            
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
                (length(TransmittedMTOfXAndYAndSubregionHist.X)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * 10);
            TransmittedMTOfXAndYAndSubregionHist.FractionalMT = reshape(TransmittedMTOfXAndYAndSubregionHist.FractionalMT, ...            
                [10, length(tempSubregionIndices), length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1, length(TransmittedMTOfXAndYAndSubregionHist.Y)-1, length(TransmittedMTOfXAndYAndSubregionHist.X)-1]); % read column major json binary
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                TransmittedMTOfXAndYAndSubregionHist.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ... 
                (length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.Y)-1) * (length(TransmittedMTOfXAndYAndSubregionHist.X)-1)); % read column major json binary
                TransmittedMTOfXAndYAndSubregionHist.SecondMoment = reshape(TransmittedMTOfXAndYAndSubregionHist.SecondMoment, ...
                [length(TransmittedMTOfXAndYAndSubregionHist.MTBins)-1,length(TransmittedMTOfXAndYAndSubregionHist.Y)-1,length(TransmittedMTOfXAndYAndSubregionHist.X)-1]);  
                TransmittedMTOfXAndYAndSubregionHist.Stdev = sqrt((TransmittedMTOfXAndYAndSubregionHist.SecondMoment - (TransmittedMTOfXAndYAndSubregionHist.Mean .* TransmittedMTOfXAndYAndSubregionHist.Mean)) / (N));               
            end
            results{di}.TransmittedMTOfXAndYAndSubregionHist = TransmittedMTOfXAndYAndSubregionHist;
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
    end %detector.Name switch
end

function json_parsed = readAndParseJson(filename)
json_strings = textread(filename, '%s',  'whitespace', '', 'bufsize', 65536);
json_parsed = loadjson(json_strings{1});
