% function to generate mua given absorber struct with names, concentrations & wavelengths
function [mua,dmua]=get_mua(absorbers,wavelengths)
persistent data;
if isempty(data)
  data=xlsread('SpectralDictionary.xlsx'); % file has 1 tab only
end
last_line=1202;
% read Fat data
wv_fat=data(2:last_line,1);
spec_fat=data(2:last_line,2);
% read H2O data
wv_h2o=data(2:last_line,3);
spec_h2o=data(2:last_line,4);
% read Hb data
wv_hb=data(2:last_line,5);
spec_hb=data(2:last_line,6);
% read HbO2 data
wv_hbo2=data(2:last_line,7);
spec_hbo2=data(2:last_line,8);
% read melanin data
wv_melanin=data(2:last_line,9);
spec_melanin=data(2:last_line,10);

mua=zeros(1,length(wavelengths));
dmua=zeros(length(wavelengths),length(absorbers.Names));
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
