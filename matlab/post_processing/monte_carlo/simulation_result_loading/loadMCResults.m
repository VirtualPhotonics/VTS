function results = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

xml = xml_load([datadir slash dataname '.xml']);
numDetectors = length(xml.DetectorInputs);
for di = 1:numDetectors
    detectorName = xml.DetectorInputs(di).anyType.Name;
    detectorType = xml.DetectorInputs(di).anyType.TallyType;
    switch(detectorType)
        case 'RDiffuse'
            RDiffuse.Name = detectorName;
            RDiffuse_xml = xml_load([datadir slash detectorName '.xml']);
            RDiffuse.Mean = str2num(RDiffuse_xml.Mean);              
            RDiffuse.SecondMoment = str2num(RDiffuse_xml.SecondMoment);
            RDiffuse.Stdev = sqrt((RDiffuse.SecondMoment - (RDiffuse.Mean .* RDiffuse.Mean)) / str2num(xml.N)); 
            results{di}.RDiffuse = RDiffuse;
        case 'ROfRho'
            ROfRho.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            ROfRho.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + ROfRho.Rho(2:end))/2;
            ROfRho.Mean = readBinaryData([datadir slash detectorName],length(ROfRho.Rho)-1);              
            if(exist([datadir slash detectorName '_2'],'file'))
                ROfRho.SecondMoment = readBinaryData([datadir slash detectorName '_2'],length(ROfRho.Rho)-1);
                ROfRho.Stdev = sqrt((ROfRho.SecondMoment - (ROfRho.Mean .* ROfRho.Mean)) / str2num(xml.N));
            end
            results{di}.ROfRho = ROfRho;
        case 'ROfAngle'
            ROfAngle.Name = detectorName;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            ROfAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            ROfAngle.Angle_Midpoints = (ROfAngle.Angle(1:end-1) + ROfAngle.Angle(2:end))/2;
            ROfAngle.Mean = readBinaryData([datadir slash detectorName],length(ROfAngle.Angle)-1);              
            if(exist([datadir slash detectorName '_2'],'file'))
                ROfAngle.SecondMoment = readBinaryData([datadir slash detectorName '_2'],length(ROfAngle.Angle)-1);
                ROfAngle.Stdev = sqrt((ROfAngle.SecondMoment - (ROfAngle.Mean .* ROfAngle.Mean)) / str2num(xml.N));
            end
            results{di}.ROfAngle = ROfAngle;
        case 'ROfXAndY'
            ROfXAndY.Name = detectorName;
            tempX = xml.DetectorInputs(di).anyType.X;
            tempY = xml.DetectorInputs(di).anyType.Y;
            ROfXAndY.X = linspace(str2num(tempX.Start), str2num(tempX.Stop), str2num(tempX.Count));
            ROfXAndY.Y = linspace(str2num(tempY.Start), str2num(tempY.Stop), str2num(tempY.Count));
            ROfXAndY.X_Midpoints = (ROfXAndY.X(1:end-1) + ROfXAndY.X(2:end))/2;
            ROfXAndY.Y_Midpoints = (ROfXAndY.Y(1:end-1) + ROfXAndY.Y(2:end))/2;
            ROfXAndY.Mean = readBinaryData([datadir slash detectorName],[length(ROfXAndY.X)-1,length(ROfXAndY.Y)-1]);    
            if(exist([datadir slash detectorName '_2'],'file'))
                ROfXAndY.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(ROfXAndY.X)-1,length(ROfXAndY.Y)-1]); 
                ROfXAndY.Stdev = sqrt((ROfXAndY.SecondMoment - (ROfXAndY.Mean .* ROfXAndY.Mean)) / str2num(xml.N)); 
            end      
            results{di}.ROfXAndY = ROfXAndY;

        case 'ROfRhoAndTime'
            ROfRhoAndTime.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempTime = xml.DetectorInputs(di).anyType.Time;
            ROfRhoAndTime.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndTime.Time = linspace(str2num(tempTime.Start), str2num(tempTime.Stop), str2num(tempTime.Count));
            ROfRhoAndTime.Rho_Midpoints = (ROfRhoAndTime.Rho(1:end-1) + ROfRhoAndTime.Rho(2:end))/2;
            ROfRhoAndTime.Time_Midpoints = (ROfRhoAndTime.Time(1:end-1) + ROfRhoAndTime.Time(2:end))/2;
            ROfRhoAndTime.Mean = readBinaryData([datadir slash detectorName],[length(ROfRhoAndTime.Rho)-1,length(ROfRhoAndTime.Time)-1]);              
            if(exist([datadir slash detectorName '_2'],'file'))
                ROfRhoAndTime.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(ROfRhoAndTime.Rho)-1,length(ROfRhoAndTime.Time)-1]);
                ROfRhoAndTime.Stdev = sqrt((ROfRhoAndTime.SecondMoment - (ROfRhoAndTime.Mean .* ROfRhoAndTime.Mean)) / str2num(xml.N));
            end
            results{di}.ROfRhoAndTime = ROfRhoAndTime;
        case 'ROfRhoAndAngle'
            ROfRhoAndAngle.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            ROfRhoAndAngle.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            ROfRhoAndAngle.Rho_Midpoints = (ROfRhoAndAngle.Rho(1:end-1) + ROfRhoAndAngle.Rho(2:end))/2;
            ROfRhoAndAngle.Angle_Midpoints = (ROfRhoAndAngle.Angle(1:end-1) + ROfRhoAndAngle.Angle(2:end))/2;
            ROfRhoAndAngle.Mean = readBinaryData([datadir slash detectorName],[length(ROfRhoAndAngle.Rho)-1,length(ROfRhoAndAngle.Angle)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                ROfRhoAndAngle.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(ROfRhoAndAngle.Rho)-1,length(ROfRhoAndAngle.Angle)-1]); 
                ROfRhoAndAngle.Stdev = sqrt((ROfRhoAndAngle.SecondMoment - (ROfRhoAndAngle.Mean .* ROfRhoAndAngle.Mean)) / str2num(xml.N)); 
            end
            results{di}.ROfRhoAndAngle = ROfRhoAndAngle;
        case 'ROfRhoAndOmega'
            ROfRhoAndOmega.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempOmega = xml.DetectorInputs(di).anyType.Omega;
            ROfRhoAndOmega.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndOmega.Omega = linspace(str2num(tempOmega.Start), str2num(tempOmega.Stop), str2num(tempOmega.Count));
            ROfRhoAndOmega.Omega_Midpoints = (ROfRhoAndOmega.Omega(1:end-1) + ROfRhoAndOmega.Omega(2:end))/2;
            ROfRhoAndOmega.Rho_Midpoints = (ROfRhoAndOmega.Rho(1:end-1) + ROfRhoAndOmega.Rho(2:end))/2;
            tempData = readBinaryData([datadir slash detectorName],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)]);  
            ROfRhoAndOmega.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            ROfRhoAndOmega.Amplitude = abs(ROfRhoAndOmega.Mean);
            ROfRhoAndOmega.Phase = -angle(ROfRhoAndOmega.Mean);
            if(exist([datadir slash detectorName '_2'],'file'))
                tempData = readBinaryData([datadir slash detectorName '_2'],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)]);  
                ROfRhoAndOmega.SecondMoment =  tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
                ROfRhoAndOmega.Stdev = sqrt((real(ROfRhoAndOmega.SecondMoment) - (real(ROfRhoAndOmega.Mean) .* real(ROfRhoAndOmega.Mean))) / str2num(xml.N)) + ...
                    1i*sqrt((imag(ROfRhoAndOmega.SecondMoment) - (imag(ROfRhoAndOmega.Mean) .* imag(ROfRhoAndOmega.Mean))) / str2num(xml.N));
            end            
            results{di}.ROfRhoAndOmega = ROfRhoAndOmega;

        case 'TDiffuse'
            TDiffuse.Name = detectorName;
            TDiffuse_xml = xml_load([datadir slash detectorName '.xml']);
            TDiffuse.Mean = str2num(TDiffuse_xml.Mean);              
            TDiffuse.SecondMoment = str2num(TDiffuse_xml.SecondMoment); 
            TDiffuse.Stdev = sqrt((TDiffuse.SecondMoment - (TDiffuse.Mean .* TDiffuse.Mean)) / str2num(xml.N));
            results{di}.TDiffuse = TDiffuse;
        case 'TOfRho'
            TOfRho.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            TOfRho.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            TOfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + TOfRho.Rho(2:end))/2;
            TOfRho.Mean = readBinaryData([datadir slash detectorName],length(TOfRho.Rho)-1);              
            if(exist([datadir slash detectorName '_2'],'file'))
                TOfRho.SecondMoment = readBinaryData([datadir slash detectorName '_2'],length(TOfRho.Rho)-1);
                TOfRho.Stdev = sqrt((TOfRho.SecondMoment - (TOfRho.Mean .* TOfRho.Mean)) / str2num(xml.N));
            end
            results{di}.TOfRho = TOfRho;
        case 'TOfAngle'
            TOfAngle.Name = detectorName;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            TOfAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            TOfAngle.Angle_Midpoints = (TOfAngle.Angle(1:end-1) + TOfAngle.Angle(2:end))/2;
            TOfAngle.Mean = readBinaryData([datadir slash detectorName],length(ROfAngle.Angle)-1);              
            if(exist([datadir slash detectorName '_2'],'file'))
                TOfAngle.SecondMoment = readBinaryData([datadir slash detectorName '_2'],length(TOfAngle.Angle)-1);
                TOfAngle.Stdev = sqrt((TOfAngle.SecondMoment - (TOfAngle.Mean .* TOfAngle.Mean)) / str2num(xml.N));
            end
            results{di}.TOfAngle = TOfAngle;
        case 'TOfRhoAndAngle'
            TOfRhoAndAngle.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            TOfRhoAndAngle.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            TOfRhoAndAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            TOfRhoAndAngle.Rho_Midpoints = (TOfRhoAndAngle.Rho(1:end-1) + TOfRhoAndAngle.Rho(2:end))/2;
            TOfRhoAndAngle.Angle_Midpoints = (TOfRhoAndAngle.Angle(1:end-1) + TOfRhoAndAngle.Angle(2:end))/2;
            TOfRhoAndAngle.Mean = readBinaryData([datadir slash detectorName],[length(ROfRhoAndAngle.Rho)-1,length(TOfRhoAndAngle.Angle)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                TOfRhoAndAngle.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(TOfRhoAndAngle.Rho)-1,length(TOfRhoAndAngle.Angle)-1]); 
                TOfRhoAndAngle.Stdev = sqrt((TOfRhoAndAngle.SecondMoment - (TOfRhoAndAngle.Mean .* TOfRhoAndAngle.Mean)) / str2num(xml.N)); 
            end
            results{di}.TOfRhoAndAngle = TOfRhoAndAngle;

        case 'ATotal'
            ATotal.Name = detectorName;
            ATotal_xml = xml_load([datadir slash detectorName '.xml']);
            ATotal.Mean = str2num(ATotal_xml.Mean);              
            ATotal.SecondMoment = str2num(ATotal_xml.SecondMoment); 
            ATotal.Stdev = sqrt((ATotal.SecondMoment - (ATotal.Mean .* ATotal.Mean)) / str2num(xml.N));
            results{di}.ATotal = ATotal;
        case 'AOfRhoAndZ'
            AOfRhoAndZ.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            AOfRhoAndZ.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            AOfRhoAndZ.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));
            AOfRhoAndZ.Rho_Midpoints = (AOfRhoAndZ.Rho(1:end-1) + AOfRhoAndZ.Rho(2:end))/2;
            AOfRhoAndZ.Z_Midpoints = (AOfRhoAndZ.Z(1:end-1) + AOfRhoAndZ.Z(2:end))/2;
            AOfRhoAndZ.Mean = readBinaryData([datadir slash detectorName],[length(AOfRhoAndZ.Rho)-1,length(AOfRhoAndZ.Z)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                AOfRhoAndZ.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(AOfRhoAndZ.Rho)-1,length(AOfRhoAndZ.Z)-1]); 
                AOfRhoAndZ.Stdev = sqrt((AOfRhoAndZ.SecondMoment - (AOfRhoAndZ.Mean .* AOfRhoAndZ.Mean)) / str2num(xml.N)); 
            end
            results{di}.AOfRhoAndZ = AOfRhoAndZ;
        case 'FluenceOfRhoAndZ'
            FluenceOfRhoAndZ.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            FluenceOfRhoAndZ.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            FluenceOfRhoAndZ.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));
            FluenceOfRhoAndZ.Rho_Midpoints = (FluenceOfRhoAndZ.Rho(1:end-1) + FluenceOfRhoAndZ.Rho(2:end))/2;
            FluenceOfRhoAndZ.Z_Midpoints = (FluenceOfRhoAndZ.Z(1:end-1) + FluenceOfRhoAndZ.Z(2:end))/2;
            FluenceOfRhoAndZ.Mean = readBinaryData([datadir slash detectorName],[length(FluenceOfRhoAndZ.Rho)-1,length(FluenceOfRhoAndZ.Z)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                FluenceOfRhoAndZ.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(FluenceOfRhoAndZ.Rho)-1,length(FluenceOfRhoAndZ.Z)-1]);
                FluenceOfRhoAndZ.Stdev = sqrt((FluenceOfRhoAndZ.SecondMoment - (FluenceOfRhoAndZ.Mean .* FluenceOfRhoAndZ.Mean)) / str2num(xml.N));  
            end
            results{di}.FluenceOfRhoAndZ = FluenceOfRhoAndZ;
        case 'FluenceOfRhoAndZAndTime'
            FluenceOfRhoAndZAndTime.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            tempTime = xml.DetectorInputs(di).anyType.Time;
            FluenceOfRhoAndZAndTime.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            FluenceOfRhoAndZAndTime.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));
            FluenceOfRhoAndZAndTime.Time = linspace(str2num(tempTime.Start), str2num(tempTime.Stop), str2num(tempTime.Count));
            FluenceOfRhoAndZAndTime.Rho_Midpoints = (FluenceOfRhoAndZAndTime.Rho(1:end-1) + FluenceOfRhoAndZAndTime.Rho(2:end))/2;
            FluenceOfRhoAndZAndTime.Z_Midpoints = (FluenceOfRhoAndZAndTime.Z(1:end-1) + FluenceOfRhoAndZAndTime.Z(2:end))/2;
            FluenceOfRhoAndZAndTime.Time_Midpoints = (FluenceOfRhoAndZAndTime.Time(1:end-1) + FluenceOfRhoAndZAndTime.Time(2:end))/2;
            FluenceOfRhoAndZAndTime.Mean = readBinaryData([datadir slash detectorName], ...
                [(length(FluenceOfRhoAndZAndTime.Rho)-1)*(length(FluenceOfRhoAndZAndTime.Z)-1)*(length(FluenceOfRhoAndZAndTime.Time)-1)]);
            FluenceOfRhoAndZAndTime.Mean = reshape(FluenceOfRhoAndZAndTime.Mean, ...
                [length(FluenceOfRhoAndZAndTime.Rho)-1,length(FluenceOfRhoAndZAndTime.Z)-1,length(FluenceOfRhoAndZAndTime.Time)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                FluenceOfRhoAndZAndTime.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ...
                    [(length(FluenceOfRhoAndZAndTime.Rho)-1)*(length(FluenceOfRhoAndZAndTime.Z)-1)*(length(FluenceOfRhoAndZAndTime.Time)-1)]);
                FluenceOfRhoAndZAndTime.SecondMoment = reshape(FluenceOfRhoAndZAndTime.Mean, ...
                    [length(FluenceOfRhoAndZAndTime.Rho)-1,length(FluenceOfRhoAndZAndTime.Z)-1,length(FluenceOfRhoAndZAndTime.Time)-1]);
                FluenceOfRhoAndZAndTime.Stdev = sqrt((FluenceOfRhoAndZAndTime.SecondMoment - (FluenceOfRhoAndZAndTime.Mean .* FluenceOfRhoAndZAndTime.Mean)) / str2num(xml.N));  
            end
            results{di}.FluenceOfRhoAndZAndTime = FluenceOfRhoAndZAndTime;
        case 'FluenceOfXAndYAndZ'
            FluenceOfXAndYAndZ.Name = detectorName;
            tempX = xml.DetectorInputs(di).anyType.X;
            tempY = xml.DetectorInputs(di).anyType.Y;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            FluenceOfXAndYAndZ.X = linspace(str2num(tempX.Start), str2num(tempX.Stop), str2num(tempX.Count));
            FluenceOfXAndYAndZ.Y = linspace(str2num(tempY.Start), str2num(tempY.Stop), str2num(tempY.Count));
            FluenceOfXAndYAndZ.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));
            FluenceOfXAndYAndZ.X_Midpoints = (FluenceOfXAndYAndZ.X(1:end-1) + FluenceOfXAndYAndZ.X(2:end))/2;
            FluenceOfXAndYAndZ.Y_Midpoints = (FluenceOfXAndYAndZ.Y(1:end-1) + FluenceOfXAndYAndZ.Y(2:end))/2;
            FluenceOfXAndYAndZ.Z_Midpoints = (FluenceOfXAndYAndZ.Z(1:end-1) + FluenceOfXAndYAndZ.Z(2:end))/2;
            FluenceOfXAndYAndZ.Mean = readBinaryData([datadir slash detectorName],[(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]);
            FluenceOfXAndYAndZ.Mean = reshape(FluenceOfXAndYAndZ.Mean, ...
                [length(FluenceOfXAndYAndZ.X)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.Z)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                FluenceOfXAndAndZ.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ...
                    [(length(FluenceOfXAndYAndZ.X)-1) * (length(FluenceOfXAndYAndZ.Y)-1) * (length(FluenceOfXAndYAndZ.Z)-1)]);
                FluenceOfXAndYAndZ.SecondMoment = reshape(FluenceOfXAndYAndZ.Mean, ...
                    [length(FluenceOfXAndYAndZ.X)-1,length(FluenceOfXAndYAndZ.Y)-1,length(FluenceOfXAndYAndZ.Z)-1]);
                FluenceOfXAndYAndZ.Stdev = sqrt((FluenceOfXAndYAndZ.SecondMoment - (FluenceOfXAndYAndZ.Mean .* FluenceOfXAndYAndZ.Mean)) / str2num(xml.N));  
            end
            results{di}.FluenceOfXAndYAndZ = FluenceOfXAndYAndZ;
        case 'RadianceOfRhoAndZAndAngle'
            RadianceOfRhoAndZAndAngle.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            RadianceOfRhoAndZAndAngle.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            RadianceOfRhoAndZAndAngle.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));                      
            RadianceOfRhoAndZAndAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            RadianceOfRhoAndZAndAngle.Rho_Midpoints = (RadianceOfRhoAndZAndAngle.Rho(1:end-1) + RadianceOfRhoAndZAndAngle.Rho(2:end))/2;
            RadianceOfRhoAndZAndAngle.Z_Midpoints = (RadianceOfRhoAndZAndAngle.Z(1:end-1) + RadianceOfRhoAndZAndAngle.Z(2:end))/2;
            RadianceOfRhoAndZAndAngle.Angle_Midpoints = (RadianceOfRhoAndZAndAngle.Angle(1:end-1) + RadianceOfRhoAndZAndAngle.Angle(2:end))/2;
            RadianceOfRhoAndZAndAngle.Mean = readBinaryData([datadir slash detectorName], ... 
                [(length(RadianceOfRhoAndZAndAngle.Rho)-1) * (length(RadianceOfRhoAndZAndAngle.Z)-1) * (length(RadianceOfRhoAndZAndAngle.Angle)-1)]);
                %[length(RadianceOfRhoAndZAndAngle.Rho)-1,length(RadianceOfRhoAndZAndAngle.Z)-1,length(RadianceOfRhoAndZAndAngle.Angle)-1]);
            RadianceOfRhoAndZAndAngle.Mean = reshape(RadianceOfRhoAndZAndAngle.Mean, ...
                [length(RadianceOfRhoAndZAndAngle.Rho)-1,length(RadianceOfRhoAndZAndAngle.Z)-1,length(RadianceOfRhoAndZAndAngle.Angle)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                RadianceOfRhoAndZAndAngle.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ... 
                [(length(RadianceOfRhoAndZAndAngle.Rho)-1) * (length(RadianceOfRhoAndZAndAngle.Z)-1) * (length(RadianceOfRhoAndZAndAngle.Angle)-1)]); 
                RadianceOfRhoAndZAndAngle.SecondMoment = reshape(RadianceOfRhoAndZAndAngle.SecondMoment, ...
                [length(RadianceOfRhoAndZAndAngle.Rho)-1,length(RadianceOfRhoAndZAndAngle.Z)-1,length(RadianceOfRhoAndZAndAngle.Angle)-1]);  
                RadianceOfRhoAndZAndAngle.Stdev = sqrt((RadianceOfRhoAndZAndAngle.SecondMoment - (RadianceOfRhoAndZAndAngle.Mean .* RadianceOfRhoAndZAndAngle.Mean)) / str2num(xml.N));               
            end
            results{di}.RadianceOfRhoAndZAndAngle = RadianceOfRhoAndZAndAngle;
        case 'RadianceOfXAndYAndZAndThetaAndPhi'
            RadianceOfXAndYAndZAndThetaAndPhi.Name = detectorName;
            tempX = xml.DetectorInputs(di).anyType.X;
            tempY = xml.DetectorInputs(di).anyType.Y;
            tempZ = xml.DetectorInputs(di).anyType.Z;
            tempTheta = xml.DetectorInputs(di).anyType.Theta;
            tempPhi = xml.DetectorInputs(di).anyType.Phi;
            RadianceOfXAndYAndZAndThetaAndPhi.X = linspace(str2num(tempX.Start), str2num(tempX.Stop), str2num(tempX.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.Y = linspace(str2num(tempY.Start), str2num(tempY.Stop), str2num(tempY.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.Z = linspace(str2num(tempZ.Start), str2num(tempZ.Stop), str2num(tempZ.Count));                      
            RadianceOfXAndYAndZAndThetaAndPhi.Theta = linspace(str2num(tempTheta.Start), str2num(tempTheta.Stop), str2num(tempTheta.Count));    
            RadianceOfXAndYAndZAndThetaAndPhi.Phi = linspace(str2num(tempPhi.Start), str2num(tempPhi.Stop), str2num(tempPhi.Count));
            RadianceOfXAndYAndZAndThetaAndPhi.X_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.X(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.X(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Y_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Y(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Y(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Z_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Z(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Z(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Theta_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Theta(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Theta(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Phi_Midpoints = (RadianceOfXAndYAndZAndThetaAndPhi.Phi(1:end-1) + RadianceOfXAndYAndZAndThetaAndPhi.Phi(2:end))/2;
            RadianceOfXAndYAndZAndThetaAndPhi.Mean = readBinaryData([datadir slash detectorName], ... 
                (length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1));
                %[length(RadianceOfXAndYAndZAndThetaAndPhi.Rho)-1,length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1,length(RadianceOfXAndYAndZAndThetaAndPhi.Angle)-1]);
            RadianceOfXAndYAndZAndThetaAndPhi.Mean = reshape(RadianceOfXAndYAndZAndThetaAndPhi.Mean, ...
                [length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ... 
                (length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1) * ...
                (length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1));
                RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment = reshape(RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment, ...
                [length(RadianceOfXAndYAndZAndThetaAndPhi.X)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Y)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Z)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Theta)-1, ...
                length(RadianceOfXAndYAndZAndThetaAndPhi.Phi)-1]);
                RadianceOfXAndYAndZAndThetaAndPhi.Stdev = sqrt((RadianceOfXAndYAndZAndThetaAndPhi.SecondMoment - (RadianceOfXAndYAndZAndThetaAndPhi.Mean .* RadianceOfXAndYAndZAndThetaAndPhi.Mean)) / str2num(xml.N));               
            end
            results{di}.RadianceOfXAndYAndZAndThetaAndPhi = RadianceOfXAndYAndZAndThetaAndPhi;
        case 'ReflectedMTOfRhoAndSubregionHist'
            ReflectedMTOfRhoAndSubregionHist.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempMTBins = xml.DetectorInputs(di).anyType.MTBins;
            tempSubregionIndices = (1:1:length(xml.TissueInput.Regions));
            ReflectedMTOfRhoAndSubregionHist.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));                     
            ReflectedMTOfRhoAndSubregionHist.MTBins = linspace(str2num(tempMTBins.Start), str2num(tempMTBins.Stop), str2num(tempMTBins.Count));
            ReflectedMTOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            ReflectedMTOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedMTOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedMTOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedMTOfRhoAndSubregionHist.MTBins_Midpoints = (ReflectedMTOfRhoAndSubregionHist.MTBins(1:end-1) + ReflectedMTOfRhoAndSubregionHist.MTBins(2:end))/2;
            ReflectedMTOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detectorName], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1));
            ReflectedMTOfRhoAndSubregionHist.Mean = reshape(ReflectedMTOfRhoAndSubregionHist.Mean, ...
                [length(ReflectedMTOfRhoAndSubregionHist.Rho)-1,length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1]);            
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = readBinaryData([datadir slash detectorName '_FractionalMT'], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1) * (length(tempSubregionIndices)) * 10);
            ReflectedMTOfRhoAndSubregionHist.FractionalMT = reshape(ReflectedMTOfRhoAndSubregionHist.FractionalMT, ...            
                [length(ReflectedMTOfRhoAndSubregionHist.Rho)-1, length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1, length(tempSubregionIndices), 10]);
            if(exist([datadir slash detectorName '_2'],'file'))
                ReflectedMTOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ... 
                (length(ReflectedMTOfRhoAndSubregionHist.Rho)-1) * (length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1)); 
                ReflectedMTOfRhoAndSubregionHist.SecondMoment = reshape(ReflectedMTOfRhoAndSubregionHist.SecondMoment, ...
                [length(ReflectedMTOfRhoAndSubregionHist.Rho)-1,length(ReflectedMTOfRhoAndSubregionHist.MTBins)-1]);  
                ReflectedMTOfRhoAndSubregionHist.Stdev = sqrt((ReflectedMTOfRhoAndSubregionHist.SecondMoment - (ReflectedMTOfRhoAndSubregionHist.Mean .* ReflectedMTOfRhoAndSubregionHist.Mean)) / str2num(xml.N));               
            end
            results{di}.ReflectedMTOfRhoAndSubregionHist = ReflectedMTOfRhoAndSubregionHist;
        case 'ReflectedTimeOfRhoAndSubregionHist'
            ReflectedTimeOfRhoAndSubregionHist.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempTime = xml.DetectorInputs(di).anyType.Time;
            tempSubregionIndices = (1:1:length(xml.TissueInput.Regions));
            ReflectedTimeOfRhoAndSubregionHist.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));                     
            ReflectedTimeOfRhoAndSubregionHist.Time = linspace(str2num(tempTime.Start), str2num(tempTime.Stop), str2num(tempTime.Count));
            ReflectedTimeOfRhoAndSubregionHist.SubregionIndices = tempSubregionIndices;
            ReflectedTimeOfRhoAndSubregionHist.Rho_Midpoints = (ReflectedTimeOfRhoAndSubregionHist.Rho(1:end-1) + ReflectedTimeOfRhoAndSubregionHist.Rho(2:end))/2;
            ReflectedTimeOfRhoAndSubregionHist.Time_Midpoints = (ReflectedTimeOfRhoAndSubregionHist.Time(1:end-1) + ReflectedTimeOfRhoAndSubregionHist.Time(2:end))/2;
            ReflectedTimeOfRhoAndSubregionHist.Mean = readBinaryData([datadir slash detectorName], ... 
                (length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1) * (length(tempSubregionIndices)) * (length(ReflectedTimeOfRhoAndSubregionHist.Time)-1));
            ReflectedTimeOfRhoAndSubregionHist.Mean = reshape(ReflectedTimeOfRhoAndSubregionHist.Mean, ...
                [length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1,length(tempSubregionIndices),length(ReflectedTimeOfRhoAndSubregionHist.Time)-1]);
            ReflectedTimeOfRhoAndSubregionHist.FractionalTime = readBinaryData([datadir slash detectorName '_FractionalTime'], ... 
                [length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1, length(tempSubregionIndices)]);
            if(exist([datadir slash detectorName '_2'],'file'))
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ... 
                [(length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1) * (length(tempSubregionIndices)) * (length(ReflectedTimeOfRhoAndSubregionHist.Time)-1)]); 
                ReflectedTimeOfRhoAndSubregionHist.SecondMoment = reshape(ReflectedTimeOfRhoAndSubregionHist.SecondMoment, ...
                [length(ReflectedTimeOfRhoAndSubregionHist.Rho)-1,length(tempSubregionIndices),length(ReflectedTimeOfRhoAndSubregionHist.Time)-1]);  
                ReflectedTimeOfRhoAndSubregionHist.Stdev = sqrt((ReflectedTimeOfRhoAndSubregionHist.SecondMoment - (ReflectedTimeOfRhoAndSubregionHist.Mean .* ReflectedTimeOfRhoAndSubregionHist.Mean)) / str2num(xml.N));               
            end
            results{di}.ReflectedTimeOfRhoAndSubregionHist = ReflectedTimeOfRhoAndSubregionHist;
      case 'pMCROfRho'
            pMCROfRho.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            pMCROfRho.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            pMCROfRho.Rho_Midpoints = (pMCROfRho.Rho(1:end-1) + pMCROfRho.Rho(2:end))/2;
            pMCROfRho.Mean = readBinaryData([datadir slash detectorName],length(pMCROfRho.Rho)-1);              
            if(exist([datadir slash detectorName '_2'],'file'))
                databaseInputxml = xml_load([datadir slash dataname '_database_infile.xml']);
                pMCROfRho.SecondMoment = readBinaryData([datadir slash detectorName '_2'],length(pMCROfRho.Rho)-1);
                pMCROfRho.Stdev = sqrt((pMCROfRho.SecondMoment - (pMCROfRho.Mean .* pMCROfRho.Mean)) / str2num(databaseInputxml.N));
            end
            results{di}.pMCROfRho = pMCROfRho;
        case 'pMCROfRhoAndTime'
            pMCROfRhoAndTime.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempTime = xml.DetectorInputs(di).anyType.Time;
            pMCROfRhoAndTime.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            pMCROfRhoAndTime.Time = linspace(str2num(tempTime.Start), str2num(tempTime.Stop), str2num(tempTime.Count));
            pMCROfRhoAndTime.Rho_Midpoints = (pMCROfRhoAndTime.Rho(1:end-1) + pMCROfRhoAndTime.Rho(2:end))/2;
            pMCROfRhoAndTime.Time_Midpoints = (pMCROfRhoAndTime.Time(1:end-1) + pMCROfRhoAndTime.Time(2:end))/2;
            pMCROfRhoAndTime.Mean = readBinaryData([datadir slash detectorName],[length(pMCROfRhoAndTime.Rho)-1,length(pMCROfRhoAndTime.Time)-1]);              
            if(exist([datadir slash detectorName '_2'],'file'))
                databaseInputxml = xml_load([datadir slash dataname '_database_infile.xml']);
                pMCROfRhoAndTime.SecondMoment = readBinaryData([datadir slash detectorName '_2'],[length(pMCROfRhoAndTime.Rho)-1,length(pMCROfRhoAndTime.Time)-1]);
                pMCROfRhoAndTime.Stdev = sqrt((pMCROfRhoAndTime.SecondMoment - (pMCROfRhoAndTime.Mean .* pMCROfRhoAndTime.Mean)) / str2num(databaseInputxml.N));
            end
            results{di}.pMCROfRhoAndTime = pMCROfRhoAndTime;
    end %detectorName switch
end
