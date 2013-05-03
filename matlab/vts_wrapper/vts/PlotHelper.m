classdef PlotHelper
    % Class containing helper functions
    %   Helper functions for MATLAB for items like creating legends
    %   plot title and other global MATLAB specific functions
    
    properties
    end
    
    methods (Static)
        function CreateLegend(vals, btxt, atxt, options)
            % CreateLegend(VALS, BTXT, ATXT, OPTIONS) creates a legend 
            % dynamically from a list of values
            % 
            %   VALS is an array of values for the legend
            %       eg. VALS = [1, 2, 3, 4];
            %   BTXT is the text that goes before the value
            %       eg. BTXT = 'n = ';
            %   ATXT is the text that goes after the value
            %       eg. BTXT = 'mm';
            %   OPTIONS is an N x 2 matrix of name value pairs for the
            %   legend
            %       eg. OPTIONS = [{'Location', 'NorthWest'}; {'FontSize', 12}];
            %           OPTIONS = {'FontSize', 12};
            %           OPTIONS = '';
            l1 = cell(1,length(vals)); %create a 1xlength(fx) cell array to hold the string values
            for i=1:length(vals)
                str = sprintf('%s%2.2f%s', btxt, vals(i), atxt);
                l1{1,i} = str;
            end
            lgnd = legend(l1);
            % set the options if there are any
            op_len = length(options(:));
            if op_len >= 2
                for j=1:op_len/2
                    set(lgnd, options(j,1), options(j,2)); 
                end
            end
        end
    end    
end

