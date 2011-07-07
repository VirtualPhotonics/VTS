function results = loadMCResults(outdir, dataname)

datadir = [outdir '\' dataname];

xml = xml_load([datadir '\' dataname '.xml']);
numDetectors = length(xml.DetectorInputs);

for di = 1:numDetectors
    detectorName = xml.DetectorInputs(di).anyType.Name;
    switch(detectorName)
        
        case 'RDiffuse'
            RDiffuse.Name = detectorName;
            RDiffuse_xml = xml_load([datadir '\' detectorName '.xml']);
            RDiffuse.Mean = str2num(RDiffuse_xml.Mean);              
            RDiffuse.SecondMoment = str2num(RDiffuse_xml.SecondMoment); 
%             disp(['Total reflectance captured by RDiffuse detector: ' num2str(RDiffuse.Mean)]);
            results.RDiffuse = RDiffuse;
        case 'ROfRho'
            ROfRho.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            ROfRho.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRho.Rho_Midpoints = (ROfRho.Rho(1:end-1) + ROfRho.Rho(2:end))/2;
            ROfRho.Mean = readBinaryData([datadir '\' detectorName],length(ROfRho.Rho)-1);              
            if(exist([datadir '\' detectorName '_2'],'file'))
                ROfRho.SecondMoment = readBinaryData([datadir '\' detectorName '_2'],length(ROfRho.Rho)-1);
            end
%             figname = ['log(' detectorName ')']; figure; imagesc(log(ROfRho.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfRho detector: ' num2str(sum(ROfRho.Mean(:)))]);
            results.ROfRho = ROfRho;
            
        case 'ROfAngle'
            ROfAngle.Name = detectorName;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            ROfAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            ROfAngle.Angle_Midpoints = (ROfAngle.Angle(1:end-1) + ROfAngle.Angle(2:end))/2;
            ROfAngle.Mean = readBinaryData([datadir '\' detectorName],length(ROfAngle.Angle)-1);              
            if(exist([datadir '\' detectorName '_2'],'file'))
                ROfAngle.SecondMoment = readBinaryData([datadir '\' detectorName '_2'],length(ROfAngle.Angle)-1);
            end
%             figname = ['log(' detectorName ')']; figure; imagesc(log(ROfAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfAngle detector: ' num2str(sum(ROfAngle.Mean(:)))]);
            results.ROfAngle = ROfAngle;
        
        case 'ROfXAndY'
            ROfXAndY.Name = detectorName;
            tempX = xml.DetectorInputs(di).anyType.X;
            tempY = xml.DetectorInputs(di).anyType.Y;
            ROfXAndY.X = linspace(str2num(tempX.Start), str2num(tempX.Stop), str2num(tempX.Count));
            ROfXAndY.Y = linspace(str2num(tempY.Start), str2num(tempY.Stop), str2num(tempY.Count));
            ROfXAndY.X_Midpoints = (ROfXAndY.X(1:end-1) + ROfXAndY.X(2:end))/2;
            ROfXAndY.Y_Midpoints = (ROfXAndY.Y(1:end-1) + ROfXAndY.Y(2:end))/2;
            ROfXAndY.Mean = readBinaryData([datadir '\' detectorName],[length(ROfXAndY.X)-1,length(ROfXAndY.Y)-1]);    
            if(exist([datadir '\' detectorName '_2'],'file'))
                ROfXAndY.SecondMoment = readBinaryData([datadir '\' detectorName '_2'],[length(ROfXAndY.X)-1,length(ROfXAndY.Y)-1]);  
            end            
%             figname = ['log(' detectorName ')']; figure; imagesc(log(ROfXAndY.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfXAndY detector: ' num2str(sum(ROfXAndY.Mean(:)))]);
            results.ROfXAndY = ROfXAndY;
            
        case 'ROfRhoAndTime'
            ROfRhoAndTime.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempTime = xml.DetectorInputs(di).anyType.Time;
            ROfRhoAndTime.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndTime.Time = linspace(str2num(tempTime.Start), str2num(tempTime.Stop), str2num(tempTime.Count));
            ROfRhoAndTime.Rho_Midpoints = (ROfRhoAndTime.Rho(1:end-1) + ROfRhoAndTime.Rho(2:end))/2;
            ROfRhoAndTime.Time_Midpoints = (ROfRhoAndTime.Time(1:end-1) + ROfRhoAndTime.Time(2:end))/2;
            ROfRhoAndTime.Mean = readBinaryData([datadir '\' detectorName],[length(ROfRhoAndTime.Rho)-1,length(ROfRhoAndTime.Time)-1]);              
            if(exist([datadir '\' detectorName '_2'],'file'))
                ROfRhoAndTime.SecondMoment = readBinaryData([datadir '\' detectorName '_2'],[length(ROfRhoAndTime.Rho)-1,length(ROfRhoAndTime.Time)-1]);
            end
%             figname = ['log(' detectorName ')']; figure; imagesc(log(ROfRhoAndTime.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfRhoAndTime detector: ' num2str(sum(ROfRhoAndTime.Mean(:)))]);
            results.ROfRhoAndTime = ROfRhoAndTime;
            
        case 'ROfRhoAndAngle'
            ROfRhoAndAngle.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempAngle = xml.DetectorInputs(di).anyType.Angle;
            ROfRhoAndAngle.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndAngle.Angle = linspace(str2num(tempAngle.Start), str2num(tempAngle.Stop), str2num(tempAngle.Count));
            ROfRhoAndAngle.Rho_Midpoints = (ROfRhoAndAngle.Rho(1:end-1) + ROfRhoAndAngle.Rho(2:end))/2;
            ROfRhoAndAngle.Angle_Midpoints = (ROfRhoAndAngle.Angle(1:end-1) + ROfRhoAndAngle.Angle(2:end))/2;
            ROfRhoAndAngle.Mean = readBinaryData([datadir '\' detectorName],[length(ROfRhoAndAngle.Rho)-1,length(ROfRhoAndAngle.Angle)-1]);
            if(exist([datadir '\' detectorName '_2'],'file'))
                ROfRhoAndAngle.SecondMoment = readBinaryData([datadir '\' detectorName '_2'],[length(ROfRhoAndAngle.Rho)-1,length(ROfRhoAndAngle.Angle)-1]);  
            end
%             figname = ['log(' detectorName ')']; figure; imagesc(log(ROfRhoAndAngle.Mean)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfRhoAndAngle detector: ' num2str(sum(ROfRhoAndAngle.Mean(:)))]);
            results.ROfRhoAndAngle = ROfRhoAndAngle;
            
        case 'ROfRhoAndOmega'
            ROfRhoAndOmega.Name = detectorName;
            tempRho = xml.DetectorInputs(di).anyType.Rho;
            tempOmega = xml.DetectorInputs(di).anyType.Omega;
            ROfRhoAndOmega.Rho = linspace(str2num(tempRho.Start), str2num(tempRho.Stop), str2num(tempRho.Count));
            ROfRhoAndOmega.Omega = linspace(str2num(tempOmega.Start), str2num(tempOmega.Stop), str2num(tempOmega.Count));
            ROfRhoAndOmega.Rho_Midpoints = (ROfRhoAndOmega.Rho(1:end-1) + ROfRhoAndOmega.Rho(2:end))/2;
            ROfRhoAndOmega.Omega_Midpoints = (ROfRhoAndOmega.Omega(1:end-1) + ROfRhoAndOmega.Omega(2:end))/2;
            tempData = readBinaryData([datadir '\' detectorName],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)-1]);  
            ROfRhoAndOmega.Mean = tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            ROfRhoAndOmega.Amplitude = abs(ROfRhoAndOmega.Mean);
            ROfRhoAndOmega.Phase = -angle(ROfRhoAndOmega.Mean);
            if(exist([datadir '\' detectorName '_2'],'file'))
                tempData = readBinaryData([datadir '\' detectorName '_2'],[2*(length(ROfRhoAndOmega.Rho)-1),length(ROfRhoAndOmega.Omega)-1]);  
                ROfRhoAndOmega.SecondMoment =  tempData(1:2:end,:) + 1i*tempData(2:2:end,:);
            end            
%             figname = ['log(Amplitude(' detectorName ') ']; figure; imagesc(log(ROfRhoAndOmega.Amplitude)); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             figname = [detectorName ' Phase']; figure; imagesc(ROfRhoAndOmega.Phase); axis image; axis off; colorbar; title(figname); set(gcf,'Name', figname);
%             disp(['Total reflectance captured by ROfRhoAndOmega detector: ' num2str(sum(ROfRhoAndOmega.Amplitude(:,1)))]);
            results.ROfRhoAndOmega = ROfRhoAndOmega;
    end
end

