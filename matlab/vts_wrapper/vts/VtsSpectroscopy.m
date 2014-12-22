classdef VtsSpectroscopy
    % VTSSPECTROSCOPY  
    properties (Constant, GetAccess='private') % can be used like a static constructor for static properties
         Assemblies = loadAssemblies();
%          ChromEnumInfo = getEnumInfo(Vts.ChromophoreType.Hb.GetType());
    end
    methods (Static) 
%         function chromType = GetChromophoreType(chromName)
%             chromType = VtsSpectroscopy.ChromEnumInfo.TypeValues(VtsSpectroscopy.ChromEnumInfo.TypeIndexMap(chromName));
%         end
        
        function op = GetOP(absorbers, scatterer, wavelengths)
            % get chromophore absorbers
            nchrom = length(absorbers.Names);
            chromophoresNET = NET.createArray('Vts.SpectralMapping.IChromophoreAbsorber', nchrom);      
            
            for i=1:nchrom
                chromType = EnumHelper.GetValueNET('Vts.ChromophoreType', absorbers.Names{i});
%                 chromType = EnumHelper.GetChromophoreType(absorbers.Names{i});
                chromophoresNET(i) = Vts.SpectralMapping.ChromophoreAbsorber(...
                    chromType, absorbers.Concentrations(i));
            end

            % get scatterer
            switch scatterer.Type
                case 'PowerLaw'
                    if(isfield(scatterer, 'A'))
                        scattererNET = Vts.SpectralMapping.PowerLawScatterer(...
                            scatterer.A, ...
                            scatterer.b);
                    else % call default constructor
                        scattererNET = Vts.SpectralMapping.PowerLawScatterer;
                    end
                case 'Intralipid'
                    if(isfield(scatterer, 'vol_frac'))
                        scattererNET = Vts.SpectralMapping.IntralipidScatterer(...
                            scatterer.VolumeFraction);
                    else % call default constructor
                        scattererNET = Vts.SpectralMapping.IntralipidScatterer;
                    end
                case 'Mie'
                    if(isfield(scatterer, 'radius'))
                        scattererNET = Vts.SpectralMapping.MieScatterer(...
                            scatterer.radius, ...
                            scatterer.n, ...
                            scatterer.nMedium);
                    else % call default constructor
                        scattererNET = Vts.SpectralMapping.MieScatterer;
                    end
            end

            % create tissue
            tissue = Vts.SpectralMapping.Tissue(chromophoresNET, scattererNET, '', 1.4);
            
            nwv = length(wavelengths);
            op = zeros([nwv 4]);
            for wvi = 1:nwv
                op(wvi,1) = tissue.GetMua(wavelengths(wvi));
                op(wvi,2) = tissue.GetMusp(wavelengths(wvi));
                op(wvi,3) = tissue.GetG(wavelengths(wvi));
                op(wvi,4) = 1.4;
            end
        end       
    end
end
