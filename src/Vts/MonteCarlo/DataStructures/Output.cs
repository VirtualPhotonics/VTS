using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Numerics;
using Vts.IO;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Houses output from Monte Carlo simulation.  This includes the various
    /// tallies (e.g. reflectance, transmittance, fluence) and the transfer of
    /// this data to/from files.
    /// </summary>
    public class Output
    {
        public Output(SimulationInput si)
        {
            Input = si;

            R_rw = new Complex[Input.DetectorInput.Rho.Count, Input.DetectorInput.Omega.Count];
            A_z = new double[Input.DetectorInput.Z.Count];
            A_layer = new double[Input.TissueInput.Regions.Count() + 1];
            Flu_rz = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count];
            Flu_z = new double[Input.DetectorInput.Z.Count];

            R_ra = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Angle.Count];

            R_xy = new double[Input.DetectorInput.X.Count, Input.DetectorInput.Y.Count];

            R_r = new double[Input.DetectorInput.Rho.Count];
            R_r2 = new double[Input.DetectorInput.Rho.Count];

            R_rt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count]; /* R(r,t) */
            R_rt2 = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count]; /* second moment R(r,t) */

            R_a = new double[Input.DetectorInput.Angle.Count];
            T_ra = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Angle.Count];
            T_r = new double[Input.DetectorInput.Rho.Count];
            T_a = new double[Input.DetectorInput.Angle.Count];
            D_rt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count];

            A_rzt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count, Input.DetectorInput.Time.Count]; 
            Flu_rzt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count, Input.DetectorInput.Time.Count]; 

        }

        public Output()
            : this(new SimulationInput())
        {
        }

        [IgnoreDataMember]
        public double[,] A_rz { get; set; }
        [IgnoreDataMember]
        public double[,] Flu_rz { get; set; }
        [IgnoreDataMember]
        public double[, ,] A_rzt { get; set; } //TODO: DC - Add to unmanaged code
        [IgnoreDataMember]
        public double[, ,] Flu_rzt { get; set; } //TODO: DC - Add to unmanaged code 
        [IgnoreDataMember]
        public double[,] R_ra { get; set; }
        [IgnoreDataMember]
        public double[,] T_ra { get; set; }
        [IgnoreDataMember]
        public double[,] R_xy { get; set; }
        [IgnoreDataMember]
        public double[,] R_rt { get; set; }
        [IgnoreDataMember]
        public double[,] R_rt2 { get; set; }
        [IgnoreDataMember]
        public double[, , ,] in_side_allvox { get; set; }
        [IgnoreDataMember]
        public double[, , ,] out_side_allvox { get; set; }
        [IgnoreDataMember]
        public Complex[,] R_rw { get; set; }
        [IgnoreDataMember]
        public double[,] D_rt { get; set; }  // average distance out bin for scaled MC
        [IgnoreDataMember]
        public double Atot { get; set; }
        [IgnoreDataMember]
        public double[] A_z { get; set; }
        [IgnoreDataMember]
        public double[] A_layer { get; set; }
        [IgnoreDataMember]
        public double[] Flu_z { get; set; }
        [IgnoreDataMember]
        public double[] R_r { get; set; }
        [IgnoreDataMember]
        public double[] R_a { get; set; }
        [IgnoreDataMember]
        public double[] R_r2 { get; set; }
        [IgnoreDataMember]
        public double[] T_r { get; set; }
        [IgnoreDataMember]
        public double[] T_a { get; set; }
        [IgnoreDataMember]
        public double Rd { get; set; }
        [IgnoreDataMember]
        public double Rtot { get; set; }
        [IgnoreDataMember]
        public double Td { get; set; }

        public SimulationInput Input { get; set; }

        public static Output FromFolderInResources(string folderPath, string projectName)
        {
            // Write the whole Output object including the multidimensional arrays
            Output outptr = FileIO.ReadFromXMLInResources<Output>(folderPath + @"output.xml", projectName);

            // Read the multidimensional arrays as binaries (and accompanying .xml MetaData files0
            outptr.A_rz = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"A_rz", projectName);
            outptr.Flu_rz = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"Flu_rz", projectName);
            outptr.R_ra = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"R_ra", projectName);
            outptr.R_xy = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"R_xy", projectName);
            outptr.T_ra = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"T_ra", projectName);
            outptr.R_rt = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"R_rt", projectName);
            outptr.R_rt2 = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"R_rt2", projectName);
            //outptr.Amp_rw = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"Amp_rw", projectName);
            //outptr.Phase_rw = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"Phase_rw", projectName);
            outptr.D_rt = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"D_rt", projectName);

                outptr.A_rzt = (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"A_rzt", projectName);
                outptr.Flu_rzt = (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(folderPath + @"Flu_rzt", projectName);

            return outptr;
        }

        public static Output FromFile(string folderPath)
        {
            // Write the whole Output object, sans the multidimensional arrays
            Output outptr = FileIO.ReadFromXML<Output>(folderPath + @"/output.xml");

            // Write the multidimensional arrays as binaries (and accompanying .xml MetaData files0
            outptr.A_rz = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/A_rz");
            outptr.Flu_rz = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/Flu_rz");
            outptr.R_ra = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/R_ra");
            outptr.R_xy = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/R_xy");
            outptr.T_ra = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/T_ra");
            outptr.R_rt = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/R_rt");
            outptr.R_rt2 = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/R_rt2");
            outptr.R_rw = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(folderPath + @"/R_rw");
            //outptr.Amp_rw = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/Amp_rw");
            //outptr.Phase_rw = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/Phase_rw");
            outptr.D_rt = (double[,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/D_rt");
            outptr.A_rzt = (double[, ,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/A_rzt");
            outptr.Flu_rzt = (double[, ,])FileIO.ReadArrayFromBinary<double>(folderPath + @"/Flu_rzt");

            return outptr;
        }

        public bool ToFile(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Write the whole Output object, sans the multi dimensional arrays
                FileIO.WriteToXML(this, folderPath + @"/output.xml");

                // Write the multidimensional arrays as binaries (and accompanying .xml MetaData files0
                // quick fix until we figure out redesign of Output
                if (Input.DetectorInput.TallyTypeList.Contains(TallyType.AOfRhoAndZ))
                {
                    FileIO.WriteArrayToBinary<double>(A_rz, folderPath + @"/A_rz");
                }
                if (Input.DetectorInput.TallyTypeList.Contains(TallyType.FluenceOfRhoAndZ))
                {
                    FileIO.WriteArrayToBinary<double>(Flu_rz, folderPath + @"/Flu_rz");
                }

                if (Input.DetectorInput.TallyTypeList.Contains(TallyType.FluenceOfRhoAndZAndTime))
                {
                    if (A_rzt != null) FileIO.WriteArrayToBinary<double>(A_rzt, folderPath + @"/A_rzt");
                    if (Flu_rzt != null) FileIO.WriteArrayToBinary<double>(Flu_rzt, folderPath + @"/Flu_rzt");
                }

                FileIO.WriteArrayToBinary<double>(R_ra, folderPath + @"/R_ra");
                FileIO.WriteArrayToBinary<double>(R_xy, folderPath + @"/R_xy");
                FileIO.WriteArrayToBinary<double>(T_ra, folderPath + @"/T_ra");
                FileIO.WriteArrayToBinary<double>(R_rt, folderPath + @"/R_rt");
                FileIO.WriteArrayToBinary<double>(R_rt2, folderPath + @"/R_rt2");
                FileIO.WriteArrayToBinary<Complex>(R_rw, folderPath + @"/R_rw");
                //FileIO.WriteArrayToBinary<double>(folderPath + @"/Amp_rw", Amp_rw);
                //FileIO.WriteArrayToBinary<double>(folderPath + @"/Phase_rw", Phase_rw);

                FileIO.WriteArrayToBinary<double>(D_rt, folderPath + @"/D_rt");

                if (A_rzt != null) FileIO.WriteArrayToBinary<double>(A_rzt, folderPath + @"/A_rzt");
                if (Flu_rzt != null) FileIO.WriteArrayToBinary<double>(Flu_rzt, folderPath + @"/Flu_rzt");


                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
                //return false;
            }
        }

        //public void Initialize( SimulationInput infile) // removed "out Banana" 3/10/2010
        //{
        //    Input = infile;
        //    R_rw = new Complex[Input.DetectorInput.Rho.Count, Input.DetectorInput.Omega.Count];
        //    //Amp_rw = new double[Input.DetectorInput.Rho.Count, 50]; /* FIX added Amp_w, Phase_w */
        //    //Phase_rw = new double[Input.DetectorInput.Rho.Count, Amp_rw.GetLength(1)];
        //    //re_rw = new double[Input.DetectorInput.Rho.Count, Amp_rw.GetLength(1)];
        //    //im_rw = new double[Input.DetectorInput.Rho.Count, Amp_rw.GetLength(1)];

        //    A_rz = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count];
        //    A_z = new double[Input.DetectorInput.Z.Count];
        //    A_layer = new double[Input.TissueInput.Regions.Count() + 1];
        //    Flu_rz = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count];
        //    Flu_z = new double[Input.DetectorInput.Z.Count];

        //    R_ra = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Angle.Count];

        //    // dcfix: todo: the following is a bad idea. allocation logic
        //    // should be done from higher-level constructs. here, it should 
        //    // just use nx, ny, etc...
        //    R_xy = new double[2 * Input.DetectorInput.X.Count, 2 * Input.DetectorInput.Y.Count];

        //    R_r = new double[Input.DetectorInput.Rho.Count];
        //    R_r2 = new double[Input.DetectorInput.Rho.Count];

        //    R_rt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count]; /* R(r,t) */
        //    R_rt2 = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count]; /* second moment R(r,t) */

        //    R_a = new double[Input.DetectorInput.Angle.Count];
        //    T_ra = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Angle.Count];
        //    T_r = new double[Input.DetectorInput.Rho.Count];
        //    T_a = new double[Input.DetectorInput.Angle.Count];
        //    D_rt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Time.Count];

        //    if (MonteCarloSimulation.DO_TIME_RESOLVED_FLUENCE)
        //    {
        //        A_rzt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count, Input.DetectorInput.Time.Count]; //TODO: DC - Add init to unmanaged code
        //        Flu_rzt = new double[Input.DetectorInput.Rho.Count, Input.DetectorInput.Z.Count, Input.DetectorInput.Time.Count]; //TODO: DC - Add init to unmanaged code
        //    }

        //    if (MonteCarloSimulation.DO_ALLVOX)
        //    {
        //        out_side_allvox = new double[6, 2 * Input.DetectorInput.X.Count, 2 * Input.DetectorInput.Y.Count, Input.DetectorInput.Z.Count]; // CKH switched side to first arg
        //        in_side_allvox = new double[6, 2 * Input.DetectorInput.X.Count, 2 * Input.DetectorInput.Y.Count, Input.DetectorInput.Z.Count];
        //    }
        //}
    }
}
