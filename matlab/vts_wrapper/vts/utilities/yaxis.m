function yaxis( bounds )
%YAXIS Summary of this function goes here
%   Detailed explanation goes here
ax = axis;
axis([ax(1) ax(2) bounds(1) bounds(2)]);

end