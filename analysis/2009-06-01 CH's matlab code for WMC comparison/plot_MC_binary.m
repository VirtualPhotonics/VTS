figure;
id='mua0.1musp0.9';

scaled_fid = fopen(sprintf('%s%s','scaled_MC_',id),'rb');
 % this is in isolated storage
valid_fid =  fopen(sprintf('Vts.Modeling.Test/N1e7G0.8dt2.5/N1e7%sg0.8dr0.2dt2.5/%s','R_rt'),'rb');

scaled_MC = fread(scaled_fid,[50,800],'double');

valid_MC = fread(valid_fid,[50,800],'double')/100; % convert to mm-2

pcolor((scaled_MC-valid_MC)./valid_MC);

axis([0 400,0 50]);

caxis([0,1]);

shading('flat');

grid off;

h=colorbar;

colormap('default');

set(gca,'TickDir','out','FontSize',20);

xlabel('time','FontSize',20);

ylabel('rho','FontSize',20);

saveas(gca,sprintf('%s%s',id,'.eps'),'epsc');
