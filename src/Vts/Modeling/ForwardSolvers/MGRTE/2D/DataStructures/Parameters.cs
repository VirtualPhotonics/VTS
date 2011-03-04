using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.Modeling.ForwardSolvers.MGRTE._2D
{
    public class Parameters
    {
        private double g, l, index_i, index_o;
        private int alevel, alevel0;
        private int slevel, slevel0;
        private int whichmg;
        private int n1, n2, n3, fmg, n_max;
        private double tol;

        public double G
        {
            get 
            {
                return g;
            }

            set
            {
                g = value;
            }
        }

        public double L
        {
            get
            {
                return l;
            }

            set
            {
                l = value;
            }
        }

        public double Index_i
        {
            get
            {
                return index_i;
            }

            set
            {
                index_i = value;
            }
        }

        public double Index_o
        {
            get
            {
                return index_o;
            }

            set
            {
                index_o = value;
            }
        }


        public int Alevel
        {
            get
            {
                return alevel;
            }

            set
            {
                alevel = value;
            }
        }

        public int Alevel0
        {
            get
            {
                return alevel0;
            }

            set
            {
                alevel0 = value;
            }
        }        

        public int Slevel
        {
            get
            {
                return slevel;
            }

            set
            {
                slevel = value;
            }
        }

        public int Slevel0
        {
            get
            {
                return slevel0;
            }

            set
            {
                slevel0 = value;
            }
        }

        public int Whichmg
        {
            get
            {
                return whichmg;
            }

            set
            {
                whichmg = value;
            }
        }        
       

        public int N1
        {
            get
            {
                return n1;
            }

            set
            {
                n1 = value;
            }
        }

        public int N2
        {
            get
            {
                return n2;
            }

            set
            {
                n2 = value;
            }
        }

        public int N3
        {
            get
            {
                return n3;
            }

            set
            {
                n3 = value;
            }
        }

        public int Fmg
        {
            get
            {
                return fmg;
            }

            set
            {
                fmg = value;
            }
        }

        public int N_max
        {
            get
            {
                return n_max;
            }

            set
            {
                n_max = value;
            }
        }
        
        public double Tol   
        {
            get
            {
                return tol;
            }

            set
            {
                tol = value;
            }
        }









    }
}
