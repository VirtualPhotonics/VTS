% SIMULATIONOPTIONS Defines options for the Monte Carlo simulation
% 	Designates a random number generator, absorption weighting type 
%   and flags input to the Monte Carlo simulation (e.g. tally second moment 
%   and specify seed for RNG).
classdef SimulationOptions < handle % deriving from handle allows us to keep a singleton around (reference based) - see Doug's post here: http://www.mathworks.com/matlabcentral/newsreader/view_thread/171344
  properties
    % seed of random number generator (-1=randomly selected seed, >=0 reproducible sequence)
    Seed = -1;
    % random number generator type
    RandomNumberGeneratorType = 'MersenneTwister';
    % absorption weighting type
    AbsorptionWeightingType = 'Discrete';
    % phase function type
    PhaseFunctionType  = 'HenyeyGreenstein';
    % list of databases to be written
    Databases = {};
    % flag indicating whether to track statistics about where photon ends up
    TrackStatistics = 0;
    % photon weight threshold to perform Russian Roulette.  Default = 0 means no RR performed.
    RussianRouletteWeightThreshold = 0;
    % simulation index 
    SimulationIndex = 0;
  end
  
    methods (Static)
      function options = FromOptionsNET(optionsNET)
          options = SimulationOptions();
          options.Seed = optionsNET.Seed;
          options.RandomNumberGeneratorType = char(optionsNET.RandomNumberGeneratorType);
          options.AbsorptionWeightingType = char(optionsNET.AbsorptionWeightingType);
          options.PhaseFunctionType = char(optionsNET.PhaseFunctionType);
          options.Databases = {};
          databasesNET = optionsNET.Databases;
          nDatabases = NET.invokeGenericMethod('System.Linq.Enumerable', 'Count', ...
                        {'Vts.MonteCarlo.DatabaseType'}, databasesNET);
          for i=1:nDatabases
              options.Databases{i} = char(databasesNET(i));
          end
          options.RussianRouletteWeightThreshold = optionsNET.RussianRouletteWeightThreshold;
          options.SimulationIndex = optionsNET.SimulationIndex;          
%           input.Databases = NET.createArray('Vts.MonteCarlo.DatabaseType', 0);  
      end
      
      function optionsNET = ToOptionsNET(options)          
          databasesNET = NET.createArray('Vts.MonteCarlo.DatabaseType', length(options.Databases));  
          for i=1:databasesNET.Length;
              databasesNET(i) = EnumHelper.GetValueNET('Vts.MonteCarlo.DatabaseType', char(options.Databases{i}));
          end     
          optionsNET = Vts.MonteCarlo.SimulationOptions( ...
              options.Seed, ... 
              EnumHelper.GetValueNET('Vts.RandomNumberGeneratorType', options.RandomNumberGeneratorType), ...
              EnumHelper.GetValueNET('Vts.AbsorptionWeightingType', options.AbsorptionWeightingType), ...
              EnumHelper.GetValueNET('Vts.PhaseFunctionType', options.PhaseFunctionType), ...
              databasesNET, ...
              logical(options.TrackStatistics), ... % track statistics
              options.RussianRouletteWeightThreshold, ... % RR threshold -> no RR performed
              options.SimulationIndex ...
          );          
      end
  end
end