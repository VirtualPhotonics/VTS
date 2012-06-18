function assemblies = loadAssemblies(varargin)

persistent VTS_ASSEMBLIES;

if isempty(VTS_ASSEMBLIES)
    
    if nargin == 3
        [assembly_names, directory, verbose] = deal(varargin{1}, varargin{2}, varargin{3});
    else
        if nargin == 0,
            verbose = 0;
        end
        
        directory = getFullPath('vts_libraries\');
        
        assembly_names = { ...
            'mc.exe', ...
            'Ionic.Zip.dll', ...
            'MathNet.Numerics.dll', ...
            'Meta.Numerics.dll', ...
            'Microsoft.Practices.ServiceLocation.dll', ...
            'Microsoft.Practices.Unity.dll', ...
            'MPFitLib.dll', ...
            'NLog.dll', ...
            'System.CoreEx.dll', ...
            'System.Reactive.dll', ...
            'Vts.dll'...
            'zlib.net.dll', ...
            'System.Core.dll', ... % this one should have been loaded...
            };
    end
    
    for i = 1:length(assembly_names)
        library = assembly_names{i};
        try
            VTS_ASSEMBLIES{i} = NET.addAssembly([directory library]);
        catch e
            if (verbose)
                e.message
                if(isa(e, 'NET.NetException'))
                    e.ExceptionObject
                end
            end
        end
    end
end

assemblies = VTS_ASSEMBLIES;