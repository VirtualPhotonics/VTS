using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class validates whether the fields in PostProcessorInput have been specified
    /// correctly or not.
    /// </summary>
    /// <param name="input"></param>
    public class PostProcessorInputValidation
    {
        public static ValidationResult ValidateInput(PostProcessorInput input)
        {
            //        // read in Simulationinput that generated database
            //        if (input.DatabaseSimulationinputFilename.Length > 0)
            //        {
            //            var SimulationinputFile = path + input.DatabaseSimulationinputFilename + ".xml";

            //            if (System.IO.File.Exists(SimulationinputFile))
            //            {
            //                databaseSimulationinput = Simulationinput.FromFile(SimulationinputFile);
            //            }
            //            else
            //            {
            //                Console.WriteLine("\nThe following input file could not be found: " +
            //                    input.DatabaseSimulationinputFilename + ".xml");
            //                return false;
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("\nNo Simulationinput file specified in PostProcessorinput. ");
            //        }
            //        // read in database names
            //        if (input.DatabaseFilenames != null)
            //        {
            //            // check if pMC databases first
            //            if (input.DatabaseTypes.Contains(DatabaseType.PhotonExitDataPoints) &&
            //                input.DatabaseTypes.Contains(DatabaseType.CollisionInfo))
            //            {
            //                doPMC = true;
            //                int pdindex = input.DatabaseTypes.IndexOf(DatabaseType.PhotonExitDataPoints);
            //                var photonDatabaseName = path + input.DatabaseFilenames[pdindex];
            //                int ciindex = input.DatabaseTypes.IndexOf(DatabaseType.CollisionInfo);
            //                var collisionInfoDatabaseName = path + input.DatabaseFilenames[ciindex];
            //                if (System.IO.File.Exists(photonDatabaseName) &&
            //                    System.IO.File.Exists(collisionInfoDatabaseName))
            //                {
            //                    pmcDatabase = pMCDatabase.FromFile(photonDatabaseName, collisionInfoDatabaseName);
            //                }
            //                else
            //                {
            //                    Console.WriteLine("\nOne of the following database files could not be found: " +
            //                        photonDatabaseName + ".xml or" + collisionInfoDatabaseName + ".xml");
            //                    return false;
            //                }
            //            }
            //            if (input.DatabaseTypes.Contains(DatabaseType.PhotonExitDataPoints) &&
            //                !input.DatabaseTypes.Contains(DatabaseType.CollisionInfo))
            //            {
            //                int index = input.DatabaseTypes.IndexOf(DatabaseType.PhotonExitDataPoints);
            //                var photonDatabaseName = path + input.DatabaseFilenames[index];
            //                if (System.IO.File.Exists(photonDatabaseName))
            //                {
            //                    photonDatabase = PhotonDatabase.FromFile(photonDatabaseName);
            //                }
            //                else
            //                {
            //                    Console.WriteLine("\nThe following database file could not be found: " +
            //                            photonDatabaseName + ".xml");
            //                    return false;
            //                }
            //            }
            //        }
            //        return true;
            //    }


            return new ValidationResult(
                true,
                "PostProcessorinput: file not found");
        }  
    }
}
