classdef SolverOptions < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties
    SolverType = 'Nurbs';
  end
end