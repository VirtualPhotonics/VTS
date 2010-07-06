%======================================================================================
% function y = reflecMCFD(p,f,n,mc_sim_type);
%
% Inputs:
% p = [mua musp] - absorption and scattering parameters, in 1/mm
% f = spatial frequency (Mx1 column vector, in 1/mm)
% n = index of refraction
% mc_sim_type = 'motherMC_n1p4_g0p9' or 'motherMC_n1p33_g0p71'
% 
% mc_sim_type is optional and mainly for testing. otherwise reflecMCSFD
% uses one of two simulations: g = 0.71 if n<1.4 or g = 0.9 if n>=1.4 
%
% Output:
% y = Reflectance in frequency vs rho at given optical properties and s-d seperations
%========================================================================================

function R_at_fx = reflecMCSFD(p,f,n,mc_sim_type)

persistent motherR rho_MC rho_interval t_MC t_interval;
if nargin<4, 
    if n<1.4,
        mc_sim_type = 'motherMC_n1p33_g0p71';
    else
        mc_sim_type = 'motherMC_n1p4_g0p9';
    end;
end;
% load MC simulation - > motherR(rho_MC,t_MC)
%   rho_MC - source-detector separation
%   rho_interval - thickness of detection ring for corresponding rho_MC
%   t_MC - time of photon detection
%   t_interval - detection time interval for corresponding t_MC
%   mother R - reflectance array (rho_MC down vs t_MC across)
if isempty(motherR), load(mc_sim_type); end;

f=reshape(f,length(f),1);
c=300; % speed of light in vacuum
v=c/n;  %  speed through medium in mm/ns
R_at_fx = zeros([length(f) size(p,1)]);
R_at_rhoMC = zeros([length(rho_MC) 1]);

% traditional 'repmat' version
for j=1:size(p,1), 
    % for each pair of optical properties
    mua=p(j,1);
    musp=p(j,2);
    
    % Weight for arbitrary mua by applying a pathlength correction => sum[ R(rho,t)*dt ] 
    R_at_rhoMC = sum( motherR/(1-fresnel(1,n,0)).*repmat(exp(-mua*v*t_MC/musp).*t_interval/musp,length(rho_MC),1), 2)*musp^3; %reflectance per m^2 per s

    % Apply Spatial Fourier Transform => sum[ R(rho)*e^(i*2*pi*f)*2*pi*rho*drho ]
    R_at_fx(:,j) = sum( repmat(R_at_rhoMC.',[length(f),1]) .* ...
        besselj( 0, repmat(2*pi*f,[1,length(rho_MC)]).*repmat((rho_MC.')./musp,[length(f),1]) ) .* ...
        repmat(2*pi*(rho_MC.')./musp,[length(f),1]) .* ...
        repmat((rho_interval.')/musp,[length(f),1]), 2);

end;

% % 'loop' version
% for j=1:size(p,1),
%     mua=p(j,1);
%     musp=p(j,2);
%     
%     % Weight for arbitrary mua by applying a pathlength correction => sum[ R(rho,t)*dt ]
%     for r_idx = 1:length(R_at_rhoMC)
%         R_at_rhoMC(r_idx) = sum( motherR(r_idx,:).*exp(-mua*v*t_MC/musp).*t_interval/musp)*musp^3/(1-fresnel(1,n,0)); %reflectance per m^2 per s
%     end;
% %     kernel = besselj( 0, 2*pi*f(f_idx).*rho_MC/musp ).*(2*pi*rho_MC) .*
% %     rho_interval/musp^2;
%     for f_idx = 1:size(R_at_fx,1)
%         % Apply Spatial Fourier Transform => sum[ R(rho)*e^(i*2*pi*f)*2*pi*rho*drho ]
%         R_at_fx(f_idx,j) = sum( R_at_rhoMC.* besselj( 0, 2*pi*f(f_idx).*rho_MC/musp ).*(2*pi*rho_MC) .* rho_interval/musp^2);
%     end;
% end;
