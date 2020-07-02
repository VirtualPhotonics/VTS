%% Monte Carlo Demo
% This script is the equivalent to Example 7 in vts_mc_demo.m but
% does not require MATLAB interop code to run
%%
clear all
clc
% ======================================================================= %
% Example 7: run a Monte Carlo simulation with pMC post-processing enabled
% Use generated database to solve inverse problem with measured data
% generated using Nurbs
% NOTE: convergence to measured data optical properties affected by:
% 1) number of photons launched in baseline simulation, N
% 2) placement and number of rho
% 3) distance of initial guess from actual
% 4) normalization of chi2
% 5) optimset options selected
%% read in baseline OPs from database gen infile
infile_db_gen='infile_pMC_db_gen.txt';
[status,muastr]=system(sprintf('./get_ops.sh Mua %s',infile_db_gen));
muaBaseline=str2num(muastr);
[status,musstr]=system(sprintf('./get_ops.sh Mus %s',infile_db_gen));
musBaseline=str2num(musstr);
% generate database
system('./mc infile=infile_pMC_db_gen.txt');
x0 = [muaBaseline, musBaseline];
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('pMC_db_gen');
R_ig=R(1:end)';
%% use unconstrained optimization lb=[-inf -inf]; ub=[inf inf];
lb=[]; ub=[];
x0=[0.01 5.0];
% input rho
rhoMidpoints=[0.5 1.5 2.5 3.5 4.5 5.5];
% input measData
measData = [0.0331 0.0099 0.0042 0.0020 0.0010 0.0005];
% input measData  

% option: divide measured data and forward model by measured data
% this counters log decay of data and relative importance of small rho data
% NOTE: if use option here, need to use option in pmc_F_dmc_J.m 
% measDataNorm = measData./measData;

% run lsqcurvefit if have Optimization Toolbox because it makes use of
% dMC differential Monte Carlo predictions
% if don't have Optimization Toolbox, run non-gradient, non-constrained
% fminsearch
if(exist('lsqcurvefit','file'))
    options = optimset('Jacobian','on','diagnostics','on','largescale','on');
    [recoveredOPs,resnorm] = lsqcurvefit('pmc_F_dmc_J',x0,rhoMidpoints,measData,lb,ub,...
        options,measData);
else
%     options = optimset('diagnostics','on','largescale','on');
    options = [];
    recoveredOPs = fminsearch('pmc_Chi2',x0,options,rhoMidpoints,measData);
end
[R,pmcR,dmcRmua,dmcRmus]=load_for_inv_results('PP_pMC_est');
R_conv=pmcR(1:end);
f = figure; semilogy(rhoMidpoints,measData,'r.',...
    rhoMidpoints,R_ig,'g-',...
    rhoMidpoints,R_conv,'b:','LineWidth',2);
xlabel('\rho [mm]');
ylabel('log10(R(\rho))');
legend('Meas','IG','Converged','Location','SouthWest');
title('Inverse solution using pMC/dMC'); 
set(f, 'Name', 'Inverse solution using pMC/dMC');
disp(sprintf('Conv =    [%f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
    (measData-R_conv')*(measData-R_conv')'));

