function results = loadMCResults(outdir, dataname)

slash = filesep;  % get correct path delimiter for platform
datadir = [outdir slash dataname];

xml = xml_load([datadir slash dataname '.xml']);
numDetectors = length(xml.DetectorInputs);
for di = 1:numDetectors
    detectorName = xml.DetectorInputs(di).anyType.Name;
    switch(detectorName)
        case 'RDiffuse'
            RDiffuse.Name = detectorName;
            RDiffuse_xml = xml_load([datadir slash detectorName '.xml']);
            RDiffuse.Mean = str2num(RDiffuse_xml.Mean);              
            RDiffuse.SecondMoment = str2num(RDiffuse_xml.SecondMoment);
            RDiffuse.Stdev = sqrt((RDiffuse.SecondMoment - (RDiffuse.Mean .* RDiffuse.Mean)) / str2num(xml.N)); 
            results.RDiffuse = RDiffuse;
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
            results.ROfRho = ROfRho;
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
            results.ROfAngle = ROfAngle;
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
            results.ROfXAndY = ROfXAndY;

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
            results.ROfRhoAndTime = ROfRhoAndTime;
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
            results.ROfRhoAndAngle = ROfRhoAndAngle;
        case 'ROfRhoAndOmega'
            ROfRhoAndOmega.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempOmega = xml.DetectorInputs(di).anyType.Omega;
            ROfRhoAndOmega.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndOmega.Omega = linspace(str2num(tempOmega.Start), str2num(tempOmega.Stop), str2num(tempOmega.Count));
            ROfRhoAndOmega.Rho_Midpoints = (ROfRhoAndOmega.Rho(1:end-1) + ROfRhoAndOmega.Rho(2:end))/2;
            ROfRhoAndOmega.Omega_Midpoints = (ROfRhoAndOmega.Omega(1:end-1) + ROfRhoAndOmega.Omega(2:end))/2;
            tempData = readBinaryData([datadir slash detectorName],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)-1]);  
            ROfRhoAndOmega.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            ROfRhoAndOmega.Amplitude = abs(ROfRhoAndOmega.Mean);
            ROfRhoAndOmega.Phase = -angle(ROfRhoAndOmega.Mean);
            if(exist([datadir slash detectorName '_2'],'file'))
                tempData = readBinaryData([datadir slash detectorName '_2'],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)-1]);  
                ROfRhoAndOmega.SecondMoment =  tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
                ROfRhoAndOmega.Stdev = sqrt((real(ROfRhoAndOmega.SecondMoment) - (real(ROfRhoAndOmega.Mean) .* real(ROfRhoAndOmega.Mean))) / str2num(xml.N)) + ...
                    1i*sqrt((imag(ROfRhoAndOmega.SecondMoment) - (imag(ROfRhoAndOmega.Mean) .* imag(ROfRhoAndOmega.Mean))) / str2num(xml.N));
            end            
            results.ROfRhoAndOmega = ROfRhoAndOmega;

        case 'TDiffuse'
            TDiffuse.Name = detectorName;
            TDiffuse_xml = xml_load([datadir slash detectorName '.xml']);
            TDiffuse.Mean = str2num(TDiffuse_xml.Mean);              
            TDiffuse.SecondMoment = str2num(TDiffuse_xml.SecondMoment); 
            TDiffuse.Stdev = sqrt((TDiffuse.SecondMoment - (TDiffuse.Mean .* TDiffuse.Mean)) / str2num(xml.N));
            results.TDiffuse = TDiffuse;
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
            results.TOfRho = ROfRho;
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
            results.ROfAngle = ROfAngle;
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
            results.TOfRhoAndAngle = TOfRhoAndAngle;

        case 'ATotal'
            ATotal.Name = detectorName;
            ATotal_xml = xml_load([datadir slash detectorName '.xml']);
            ATotal.Mean = str2num(ATotal_xml.Mean);              
            ATotal.SecondMoment = str2num(ATotal_xml.SecondMoment); 
            ATotal.Stdev = sqrt((ATotal.SecondMoment - (ATotal.Mean .* ATotal.Mean)) / str2num(xml.N));
            results.ATotal = ATotal;
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
            results.AOfRhoAndZ = AOfRhoAndZ;
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
            results.FluenceOfRhoAndZ = FluenceOfRhoAndZ;
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
            results.RadianceOfRhoAndZAndAngle = RadianceOfRhoAndZAndAngle;
    end %detectorName switch
end
