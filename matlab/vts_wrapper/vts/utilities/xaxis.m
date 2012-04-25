function xaxis( bounds )
%XAXIS Summary of this function goes here
%   Detailed explanation goes here
ax = axis;
axis([bounds(1) bounds(2) ax(3) ax(4)]);

end

