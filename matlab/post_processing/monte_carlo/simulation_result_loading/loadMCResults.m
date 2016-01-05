function results = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

%json = readAndParseJson([datadir slash 'infile_' dataname '.txt']);
json = readAndParseJson([datadir slash dataname '.txt']);
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
                % x=a+ib: SD=sqrt( E(|x|^2) + |E(x)|^2 ) = since SecondMoment is real = sqrt(SecondMoment+real(Mean)*real(Mean)+imag(Mean)*imag(Mean))
                ROfRhoAndOmega.Stdev = sqrt((ROfRhoAndOmega.SecondMoment + real(ROfRhoAndOmega.Mean) .* real(ROfRhoAndOmega.Mean) + ...
                                                                           imag(ROfRhoAndOmega.Mean) .* imag(ROfRhoAndOmega.Mean)) / json.N);
            end            
            results{di}.ROfRhoAndOmega = ROfRhoAndOmega;
        case 'ROfFx'
            ROfFx.Name = detector.Name;
            tempFx = detector.Fx;
            ROfFx.Fx = linspace((tempFx.Start), (tempFx.Stop), (tempFx.Count));
            ROfFx.Fx_Midpoints = ROfFx.Fx;
            tempData = readBinaryData([datadir slash detector.Name],2*length(ROfFx.Fx));
            ROfFx.Mean = tempData(1:2:end) + 1i*tempData(2:2:end);          
            if(detector.TallySecondMoment && exist([datadir slash detector.Name '_2'],'file'))
                tempData = readBinaryData([datadir slash detector.Name '_2'],2*length(ROfFx.Fx));
                ROfFx.SecondMoment = tempData(1:2:end) + 1i*tempData(2:2:end);
                % x=a+ib: SD=sqrt( E(|x|^2) + |E(x)|^2 ) = since SecondMoment is real = sqrt(SecondMoment+real(Mean)*real(Mean)+imag(Mean)*imag(Mean))
                ROfFx.Stdev = sqrt((ROfFx.SecondMoment + real(ROfFx.Mean) .* real(ROfFx.Mean) + ...
                                                         imag(ROfFx.Mean) .* imag(ROfFx.Mean)) / json.N);
            end
            results{di}.ROfFx = ROfFx;
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
            TOfRho.Rho_Midpoints = (TOfRho.Rho(1:end-1) + TOfRho.Rho(2:end))/2;
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
                AOfXAndYAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(AOfXAndYAndZ.X)-1) * (length(AOfXAndYAndZ.Y)-1) * (length(AOfXAndYAndZ.Z)-1)]);
                AOfXAndYAndZ.SecondMoment = reshape(AOfXAndYAndZ.SecondMoment, ... % column major json binary
                    [length(AOfXAndYAndZ.Z)-1,length(AOfXAndYAndZ.Y)-1,length(AOfXAndYAndZ.X)-1]);
                AOfXAndYAndZ.Stdev = sqrt((AOfXAndYAndZ.SecondMoment - (AOfXAndYAndZ.Mean .* AOfXAndYAndZ.Mean)) / json.N);  
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
                FluenceOfXAndYAndZ.SecondMoment = readBinaryData([datadir slash detector.Name '_2'], ...
                    [(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]);
                FluenceOfXAndYAndZ.SecondMoment = reshape(FluenceOfXAndYAndZ.SecondMoment, ... % column major json binary
                    [length(FluenceOfXAndYAndZ.Z)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.X)-1]);
                FluenceOfXAndYAndZ.Stdev = sqrt((FluenceOfXAndYAndZ.SecondMoment - (FluenceOfXAndYAndZ.Mean .* FluenceOfXAndYAndZ.Mean)) / (json.N));  
            end
            results{di}.FluenceOfXAndYAndZ = FluenceOfXAndYAndZ;
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
                FluenceOfXAndYAndZAndOmega.SecondMoment = tempDataReshape(1:2:end,:,:,:) + 1i*tempDataReshape(2:2:end,:,:,:);
                % x=a+ib: SD=sqrt( E(|x|^2) + |E(x)|^2 ) = since SecondMoment is real = sqrt(SecondMoment+real(Mean)*real(Mean)+imag(Mean)*imag(Mean))
                FluenceOfXAndYAndZAndOmega.Stdev = sqrt((FluenceOfXAndYAndZAndOmega.SecondMoment + real(FluenceOfXAndYAndZAndOmega.Mean) .* real(FluenceOfXAndYAndZAndOmega.Mean) + ...
                                                                           imag(FluenceOfXAndYAndZAndOmega.Mean) .* imag(FluenceOfXAndYAndZAndOmega.Mean)) / json.N);
end                 
            results{di}.FluenceOfXAndYAndZAndOmega = FluenceOfXAndYAndZAndOmega;
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
                RadianceOfFxAndZAndAngle.SecondMoment = tempDataReshape(1:2:end,:,:) + 1i*tempDataReshape(2:2:end,:,:);
                % x=a+ib: SD=sqrt( E(|x|^2) + |E(x)|^2 ) = since SecondMoment is real = sqrt(SecondMoment+real(Mean)*real(Mean)+imag(Mean)*imag(Mean))
                RadianceOfFxAndZAndAngle.Stdev = sqrt((RadianceOfFxAndZAndAngle.SecondMoment + real(RadianceOfFxAndZAndAngle.Mean) .* real(RadianceOfFxAndZAndAngle.Mean) + ...
                                                                           imag(RadianceOfFxAndZAndAngle.Mean) .* imag(RadianceOfFxAndZAndAngle.Mean)) / json.N);
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
                N = databaseInputJson.N;
            else   
                N = json.N;
            end
            ReflectedDynamicMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            ReflectedDynamicMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedDynamicMTOfRhoAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            ReflectedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.MTBins(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            ReflectedDynamicMTOfRhoAndSubregionHist.Z_Midpoints = (ReflectedDynamicMTOfRhoAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
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
            tempMTBins = detector.MTBins;              
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                N = databaseInputJson.N;
            else   
                N = json.N;
            end
            ReflectedDynamicMTOfXAndYAndSubregionHist.X = linspace((tempX.Start), (tempX.Stop), (tempX.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.Y = linspace((tempY.Start), (tempY.Stop), (tempY.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            ReflectedDynamicMTOfXAndYAndSubregionHist.X_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.X(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.X(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.Y_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.Y(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.Y(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins(1:end-1) + ReflectedDynamicMTOfXAndYAndSubregionHist.MTBins(2:end))/2;
            ReflectedDynamicMTOfXAndYAndSubregionHist.Z_Midpoints = (ReflectedDynamicMTOfXAndYAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
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
        case 'TransmittedDynamicMTOfRhoAndSubregionHist'
            TransmittedDynamicMTOfRhoAndSubregionHist.Name = detector.Name;
            tempRho = detector.Rho;
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                N = databaseInputJson.N;
            else   
                N = json.N;
            end
            TransmittedDynamicMTOfRhoAndSubregionHist.Rho = linspace((tempRho.Start), (tempRho.Stop), (tempRho.Count));                     
            TransmittedDynamicMTOfRhoAndSubregionHist.MTBins = linspace((tempMTBins.Start), (tempMTBins.Stop), (tempMTBins.Count));
            TransmittedDynamicMTOfRhoAndSubregionHist.Z = linspace((tempZ.Start), (tempZ.Stop), (tempZ.Count));
            TransmittedDynamicMTOfRhoAndSubregionHist.Rho_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.Rho(1:end-1) + TransmittedDynamicMTOfRhoAndSubregionHist.Rho(2:end))/2;
            TransmittedDynamicMTOfRhoAndSubregionHist.MTBins_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.MTBins(1:end-1) + TransmittedDynamicMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            TransmittedDynamicMTOfRhoAndSubregionHist.Z_Midpoints = (TransmittedDynamicMTOfRhoAndSubregionHist.Z(1:end-1) + ReflectedDynamicMTOfRhoAndSubregionHist.Z(2:end))/2;
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
            tempMTBins = detector.MTBins;
            tempFractionalMTBinsLength = detector.FractionalMTBins.Count+1; % +1 due to addition of =0,=1 bins
            if (postProcessorResults)
                N = databaseInputJson.N;
            else   
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
