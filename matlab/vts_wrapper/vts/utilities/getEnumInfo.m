function info = getEnumInfo(enumType)
% GETENUMINFO 
namesNET = System.Enum.GetNames(enumType);
names = cell(1, namesNET.Length);

for i=1:namesNET.Length
    names{i} = char(namesNET(i)); 
end

% get the underlying Vts.ChromophoreType values
info.TypeValues = System.Enum.GetValues(enumType);

% create a dictionary based on the chromophore name
info.TypeIndexMap = containers.Map(names, num2cell(1:namesNET.Length));  