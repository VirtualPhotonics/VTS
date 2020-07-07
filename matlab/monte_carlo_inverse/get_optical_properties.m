% function to generate mua given absorber struct with names, concentrations and lambdas
function [ops,dmua]=get_optical_properties(absorbers,scatterers,wavelengths)
nwv = length(wavelengths);
ops = zeros([nwv 4]);
[ops(:,1),dmua] = get_mua(absorbers,wavelengths);
ops(:,2) = get_musp(scatterers,wavelengths);
ops(:,3) = 0.8;
ops(:,4) = 1.4;
end
