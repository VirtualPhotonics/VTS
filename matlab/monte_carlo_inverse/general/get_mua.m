% function to generate mua given absorber struct with names, concentrations & wavelengths
function [mua,dmua]=get_mua(absorbers,wavelengths)
persistent data;
if isempty(data)
  data=readtable('SpectralDictionary.xlsx','ReadVariableNames',false,'HeaderLines',1); % file has 1 tab only
end
% the following assumes all columns in table have same length
% read Fat data
wv_fat=data(:,1);
spec_fat=data(:,2);
% read H2O data
wv_h2o=data(:,3);
spec_h2o=data(:,4);
% read Hb data
wv_hb=data(:,5);
spec_hb=data(:,6);
% read HbO2 data
wv_hbo2=data(:,7);
spec_hbo2=data(:,8);
% read melanin data
wv_melanin=data(:,9);
spec_melanin=data(:,10);

mua=zeros(1,length(wavelengths));
dmua=zeros(length(wavelengths),length(absorbers.Names));
values=zeros(1,length(wavelengths));
for i=1:length(absorbers.Names)
  % determine if spectra is in molar absorption coefficients or fractional abs coefficient
  if (strcmp(absorbers.Names(i),'Fat')) % fractional
    values=interp1(wv_fat,spec_fat,wavelengths);
    mua=mua+absorbers.Concentrations(i)*values;
    dmua(:,i)=dmua(:,i)+values';
  elseif (strcmp(absorbers.Names(i),'H2O')) % fractional
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
  elseif (strcmp(absorbers.Names(i),'Melanin')) % fractional
    values=interp1(wv_melanin,spec_melanin,wavelengths);
    mua=mua+absorbers.Concentrations(i)*values;
    dmua(:,i)=dmua(:,i)+values';
  end
end
end
