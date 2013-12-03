% Unit tests for the data structures
disp('Running unit tests for data structures...');

% create a Matlab-wrapped source input DTO
di = DirectionalPointSourceInput;

% mutate the state of the Matlab DTO
di.InitialTissueRegionIndex = 1;

% convert that DTO to a native .NET DTO
diNET = DirectionalPointSourceInput.ToInputNET(di);

% check to see that the value was correctly assigned in the .NET class
assert(diNET.InitialTissueRegionIndex == 1);

disp('Done!');