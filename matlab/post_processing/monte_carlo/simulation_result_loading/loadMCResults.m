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
        case 'ReflectedMTOfRhoAndSubRegionHist'
            ReflectedMTOfRhoAndSubRegionHist.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempSubRegionsCount = length(xml.TissueInput.Regions);
            tempSubRegions = zeros(1, tempSubRegionsCount + 1);
            tempSubRegions(1:end) = xml.TissueInput.Regions(1).anyType.ZRange.Start;
            tempMTBins = xml.DetectorInputs(di).anyType.MTBins;
            ReflectedMTOfRhoAndSubRegionHist.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));                     
            ReflectedMTOfRhoAndSubRegionHist.MTBins = linspace(str2num(tempMTBins.Start), str2num(tempMTBins.Stop), str2num(tempMTBins.Count));
            ReflectedMTOfRhoAndSubRegionHist.Rho_Midpoints = (ReflectedMTOfRhoAndSubRegionHist.Rho(1:end-1) + ReflectedMTOfRhoAndSubRegionHist.Rho(2:end))/2;
            ReflectedMTOfRhoAndSubRegionHist.SubRegions_Midpoints = (tempSubRegions(1:end-1) + tempSubRegions(2:end))/2;
            ReflectedMTOfRhoAndSubRegionHist.MTBins_Midpoints = (ReflectedMTOfRhoAndSubRegionHist.MTBins(1:end-1) + ReflectedMTOfRhoAndSubRegionHist.MTBins(2:end))/2;
            ReflectedMTOfRhoAndSubRegionHist.Mean = readBinaryData([datadir slash detectorName], ... 
                [(length(ReflectedMTOfRhoAndSubRegionHist.Rho)-1) * (length(tempSubRegions)-1) * (length(ReflectedMTOfRhoAndSubRegionHist.MTBins)-1)]);
            ReflectedMTOfRhoAndSubRegionHist.Mean = reshape(ReflectedMTOfRhoAndSubRegionHist.Mean, ...
                [length(ReflectedMTOfRhoAndSubRegionHist.Rho)-1,length(tempSubRegions)-1,length(ReflectedMTOfRhoAndSubRegionHist.MTBins)-1]);
            if(exist([datadir slash detectorName '_2'],'file'))
                ReflectedMTOfRhoAndSubRegionHist.SecondMoment = readBinaryData([datadir slash detectorName '_2'], ... 
                [(length(ReflectedMTOfRhoAndSubRegionHist.Rho)-1) * (length(tempSubRegions)-1) * (length(ReflectedMTOfRhoAndSubRegionHist.MTBins)-1)]); 
                ReflectedMTOfRhoAndSubRegionHist.SecondMoment = reshape(ReflectedMTOfRhoAndSubRegionHist.SecondMoment, ...
                [length(ReflectedMTOfRhoAndSubRegionHist.Rho)-1,length(tempSubRegions)-1,length(ReflectedMTOfRhoAndSubRegionHist.MTBins)-1]);  
                ReflectedMTOfRhoAndSubRegionHist.Stdev = sqrt((ReflectedMTOfRhoAndSubRegionHist.SecondMoment - (ReflectedMTOfRhoAndSubRegionHist.Mean .* ReflectedMTOfRhoAndSubRegionHist.Mean)) / str2num(xml.N));               
            end
            results{di}.ReflectedMTOfRhoAndSubRegionHist = ReflectedMTOfRhoAndSubRegionHist;
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
