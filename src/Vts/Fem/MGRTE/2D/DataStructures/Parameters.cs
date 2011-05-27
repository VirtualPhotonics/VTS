using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.Fem.MGRTE._2D
{
    public class Parameters : BindableObject
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
                this.OnPropertyChanged("G");
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
                this.OnPropertyChanged("L");
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
                this.OnPropertyChanged("Index_i");
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
                this.OnPropertyChanged("Index_o");
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
                this.OnPropertyChanged("Alevel");
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
                this.OnPropertyChanged("Alevel0");
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
                this.OnPropertyChanged("Slevel");
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
                this.OnPropertyChanged("Slevel0");
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
                this.OnPropertyChanged("Whichmg");
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
                this.OnPropertyChanged("N1");
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
                this.OnPropertyChanged("N2");
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
                this.OnPropertyChanged("N3");
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
                this.OnPropertyChanged("Fmg");
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
                this.OnPropertyChanged("N_max");
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
                this.OnPropertyChanged("Tol");
            }
        }
    }
}
