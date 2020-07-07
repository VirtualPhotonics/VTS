% function to generate mua given absorber struct with names, concentrations & wavelengths
function [mua,dmua]=get_mua(absorbers,wavelengths)
persistent data;
if isempty(data)
  data=xlsread('SpectralDictionary.xlsx'); % file has 1 tab only
end
% read H2O data
last_line_h2o=209;
wv_h2o=data(2:last_line_h2o,1);
spec_h2o=data(2:last_line_h2o,2);
% read Hb data
last_line_hb=376;
wv_hb=data(2:last_line_hb,3);
spec_hb=data(2:last_line_hb,4);
% read HbO2 data
last_line_hbo2=376;
wv_hbo2=data(2:last_line_hbo2,5);
spec_hbo2=data(2:last_line_hbo2,6);

mua=zeros(1,length(wavelengths));
dmua=zeros(length(wavelengths),length(absorbers.Names));
for i=1:length(absorbers.Names)
  % determine if spectra is in molar absorption coefficients or fractional abs coefficient
  if (strcmp(absorbers.Names(i),'H2O')) % fractional
    values=interp1(wv_h2o,spec_h2o,wavelengths);
    mua=mua+absorbers.Concentrations(i)*values;
    dmua(:,i)=dmua(:,i)+values';
  elseif (strcmp(absorbers.Names(i),'Hb')) % molar
    values=interp1(wv_hb,spec_hb,wavelengths);
    mua=mua+log(10)*absorbers.Concentrations(i)*values;
    dmua(:,i)=dmua(:,i)+log(10)*values';
  elseif (strcmp(absorbers.Names(i),'HbO2')) % molar
    values=interp1(wv_hbo2,spec_hbo2,wavelengths);
    mua=mua+log(10)*absorbers.Concentrations(i)*values;
    dmua(:,i)=dmua(:,i)+log(10)*values';
  end
end
end
