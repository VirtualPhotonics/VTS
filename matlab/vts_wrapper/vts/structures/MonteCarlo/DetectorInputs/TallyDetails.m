classdef TallyDetails
    methods (Static)
        function details = Default()
            details.IsReflectanceTally = 0;
            details.IsTransmittanceTally = 0;
            details.IsSpecularReflectanceTally = 0;
            details.IsInternalSurfaceTally = 0;
            details.IspMCReflectanceTally = 0;
            details.IsDosimetryTally = 0;
            details.IsVolumeTally = 0;
            details.IsCylindricalTally = 0;
            details.IsNotImplementedForDAW = 0;
            details.IsNotImplementedForCAW = 0;
            details.IsNotImplementedYet = 0;
        end
        function details = FromDetailsNET(detailsNET)            
            details.IsReflectanceTally = detailsNET.IsReflectanceTally;
            details.IsTransmittanceTally = detailsNET.IsTransmittanceTally;
            details.IsSpecularReflectanceTally = detailsNET.IsSpecularReflectanceTally;
            details.IsInternalSurfaceTally = detailsNET.IsInternalSurfaceTally;
            details.IspMCReflectanceTally = detailsNET.IspMCReflectanceTally;
            details.IsDosimetryTally = detailsNET.IsDosimetryTally;
            details.IsVolumeTally = detailsNET.IsVolumeTally;
            details.IsCylindricalTally = detailsNET.IsCylindricalTally;
            details.IsNotImplementedForDAW = detailsNET.IsNotImplementedForDAW;
            details.IsNotImplementedForCAW = detailsNET.IsNotImplementedForCAW;
            details.IsNotImplementedYet = detailsNET.IsNotImplementedYet;
        end        
        function detailsNET = ToDetailsNET(details)
            detailsNET = Vts.MonteCarlo.TallyDetails;
            detailsNET.IsReflectanceTally = details.IsReflectanceTally;
            detailsNET.IsTransmittanceTally = details.IsTransmittanceTally;
            detailsNET.IsSpecularReflectanceTally = details.IsSpecularReflectanceTally;
            detailsNET.IsInternalSurfaceTally = details.IsInternalSurfaceTally;
            detailsNET.IspMCReflectanceTally = details.IspMCReflectanceTally;
            detailsNET.IsDosimetryTally = details.IsDosimetryTally;
            detailsNET.IsVolumeTally = details.IsVolumeTally;
            detailsNET.IsCylindricalTally = details.IsCylindricalTally;
            detailsNET.IsNotImplementedForDAW = details.IsNotImplementedForDAW;
            detailsNET.IsNotImplementedForCAW = details.IsNotImplementedForCAW;
            detailsNET.IsNotImplementedYet = details.IsNotImplementedYet;
        end
    end
end