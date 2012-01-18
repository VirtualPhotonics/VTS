classdef EnumHelper
    properties (Constant, GetAccess='private') % can be used like a static constructor for static properties
        Assemblies = loadAssemblies();
%         
%         % todo - change this growing list to a single dictionary (Map) type        
%         ChromEnumInfo = EnumHelper.GetEnumInfo(Vts.ChromophoreType.Hb.GetType());
%         SolverEnumInfo = EnumHelper.GetEnumInfo(Vts.ForwardSolverType.Nurbs.GetType());        
%         RandomNumberGeneratorEnumInfo = EnumHelper.GetEnumInfo(Vts.RandomNumberGeneratorType.MersenneTwister.GetType());        
%         AbsorptionWeightingEnumInfo = EnumHelper.GetEnumInfo(Vts.AbsorptionWeightingType.Discrete.GetType());        
%         PhaseFunctionEnumInfo = EnumHelper.GetEnumInfo(Vts.PhaseFunctionType.HenyeyGreenstein.GetType());        
%         DatabseEnumInfo = EnumHelper.GetEnumInfo(Vts.MonteCarlo.DatabaseType.DiffuseReflectance.GetType());

        % create a dictionary that maps a .NET Enum name (with namespace)
        % to a collection of instances of its underlying .NET values
        TypeMapper = containers.Map(...
            { ...
                'Vts.ChromophoreType',...
                'Vts.ForwardSolverType', ...        
                'Vts.RandomNumberGeneratorType', ...        
                'Vts.AbsorptionWeightingType', ...        
                'Vts.PhaseFunctionType', ...        
                'Vts.MonteCarlo.DatabaseType', ...
            }, ...
            { ...
                EnumHelper.GetEnumInfo(Vts.ChromophoreType.Hb.GetType()),...
                EnumHelper.GetEnumInfo(Vts.ForwardSolverType.Nurbs.GetType()), ...        
                EnumHelper.GetEnumInfo(Vts.RandomNumberGeneratorType.MersenneTwister.GetType()), ...        
                EnumHelper.GetEnumInfo(Vts.AbsorptionWeightingType.Discrete.GetType()), ...        
                EnumHelper.GetEnumInfo(Vts.PhaseFunctionType.HenyeyGreenstein.GetType()), ...        
                EnumHelper.GetEnumInfo(Vts.MonteCarlo.DatabaseType.DiffuseReflectance.GetType()), ...
            });
    end
    
    % helper methods to return a .NET enum instance based on a string input
    methods (Static)  
        % return a .NET enum value from the Enum name (with namespace) and
        % value name (eg GetValueNET('Vts.ForwardSolverType', 'Nurbs'))
        function value = GetValueNET(typeName, valueName)
            info = EnumHelper.TypeMapper(typeName);   
            mapIndex = info.TypeIndexMap(valueName);
            value = info.TypeValues(mapIndex);
        end
        
%         function chromType = GetChromophoreType(chromName)
%             chromType = EnumHelper.GetType(EnumHelper.ChromEnumInfo, chromName);
%         end
%         
%         function solverType = GetForwardSolverType(solverName)
%             solverType = EnumHelper.GetType(EnumHelper.SolverEnumInfo, solverName);
%         end
%         
%         function rngType = GetRandomNumberGeneratorType(rngName)
%             rngType = EnumHelper.GetType(EnumHelper.RandomNumberGeneratorEnumInfo, rngName);
%         end
%         
%         function awType = GetAbsorptionWeightingType(awName)
%             awType = EnumHelper.GetType(EnumHelper.AbsorptionWeightingEnumInfo, awName);
%         end
%         
%         function pfType = GetPhaseFunctionType(pfName)
%             pfType = EnumHelper.GetType(EnumHelper.PhaseFunctionEnumInfo, pfName);
%         end        
%         
%         function dbType = GetDatabaseType(dbName)
%             dbType = EnumHelper.GetType(EnumHelper.DatabseEnumInfo, dbName);
%         end
    end
    
    methods (Static, Access='private')
        % create a makeshift dictionary capable of holding .NET values
        function info = GetEnumInfo(enumType)
            
            namesNET = System.Enum.GetNames(enumType);
            names = cell(1, namesNET.Length);
            
            for i=1:namesNET.Length
                names{i} = char(namesNET(i));
            end
            
            % get the underlying Vts.ChromophoreType values
            info.TypeValues = System.Enum.GetValues(enumType);
            
            % create a dictionary based on the chromophore name
            info.TypeIndexMap = containers.Map(names, num2cell(1:namesNET.Length));
        end       
        
%         % on-demand, lookup types stored in enumInfo
%         function value = GetValue(enumInfo, name)
%             value = enumInfo.TypeValues(enumInfo.TypeIndexMap(name));
%         end
    end
    
end