function R = fresnel(n_in,n_out,theta);

theta_prime = (n_in/n_out)*sin(theta);
cos_theta = cos(theta);
cos_theta_prime = cos(theta_prime);
R = 0.5*((n_in*cos_theta_prime - n_out*cos_theta)./(n_in*cos_theta_prime+n_out*cos_theta)).^2 + 0.5*((n_in*cos_theta - n_out*cos_theta_prime)./(n_in*cos_theta+n_out*cos_theta_prime)).^2;