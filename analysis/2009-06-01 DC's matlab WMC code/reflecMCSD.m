%======================================================================================
% function y = reflecMCFD(p,f,fx,n,rho,dummy_variable, wt, reim_flag);
%
% Inputs:
% p = [mua musp] - absorption and scattering parameters, in 1/mm
% f = frequency (Mx1 column vector, in MHz)
% rho  = s-d separation (1xN column vector, in mm)
% wt = 2MxN matrix of weights
%
% Output:
% y = Reflectance in frequency vs rho at given optical properties and s-d seperations
%========================================================================================

function R_at_rho = reflecMCSD(p,rho,n,mc_sim_type)%fx,n,f_t,dummy_variable2, wt, reim_flag);
% function y = reflecMCFDfull(p,f,fx,n,rho,dummy_variable, wt, reim_flag);
persistent motherR rho_MC rhofile rho_interval t_MC timefile t_interval;% R_vs_rhoMC_tMC R_vs_f_rhoMC Amp_vs_f_rho Phi_vs_f_rho;
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

rho=reshape(rho,length(rho),1);
c=300; % speed of light in vacuum
v=c/n;  %  speed through medium in mm/ns
R_at_rho = zeros([length(rho) size(p,1)]);
R_at_rhoMC = zeros([length(rho_MC) 1]);

% traditional 'repmat' version
for j=1:size(p,1),
    mua=p(j,1);
    musp=p(j,2);
    
    % Weight for arbitrary mua by applying a pathlength correction => sum[ R(rho,t)*dt ]
    R_at_rhoMC = sum( motherR/(1-fresnel(1,n,0)).*repmat(exp(-mua*v*t_MC/musp).*t_interval/musp, length(rho_MC),1), 2)*musp; %reflectance per m^2 per s

    R_at_rho(:,j) = interp1(rho_MC/musp, R_at_rhoMC*musp^2, rho, 'pchip');
end;