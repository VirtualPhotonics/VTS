namespace Vts.MonteCarlo
{
    public unsafe static class UnmanagedIO
    {
        public static void Assign1DPointer(int[] input, ref int* pointer)
        {
            fixed (int* px = &input[0])
            {
                pointer = px;
            }
        }
        public static void Assign1DPointer(double[] input, ref double* pointer)
        {
            fixed (double* px = &input[0])
            {
                pointer = px;
            }
        }
        //public static void Assign1DPointer(ref Layer[] input, ref Layer* pointer)
        //{
        //    fixed (Layer* px = &input[0])
        //    {
        //        pointer = px;
        //    }
        //}
        public static void Assign2DPointer(int[,] input, ref int** pointer)
        {
            var temp = new int*[input.GetLength(0)];
            fixed (int** px = &temp[0])
            {
                pointer = px;
            }

            for (int i = 0; i < input.GetLength(0); i++)
            {
                fixed (int* px = &input[i, 0])
                {
                    pointer[i] = px;
                }
            }
        }
        public static void Assign2DPointer(double[,] input, ref double** pointer)
        {
            var temp = new double*[input.GetLength(0)];
            fixed (double** px = &temp[0])
            {
                pointer = px;
            }

            for (int i = 0; i < input.GetLength(0); i++)
            {
                fixed (double* px = &input[i, 0])
                {
                    pointer[i] = px;
                }
            }
        }
        public static void Assign3DPointer(double[,,] input, ref double*** pointer)
        {
            var temptemp = new double **[input.GetLength(0)];
            fixed (double*** px = &temptemp[0])
            {
                pointer = px;
            }
            for (int i = 0; i < input.GetLength(0); i++)
            {
                var temp = new double*[input.GetLength(1)];
                fixed (double** px = &temp[0])
                {
                    pointer[i] = px;
                }

                for (int j = 0; j < input.GetLength(1); j++)
                {
                    fixed (double* py = &input[i,j,0])
                    {
                        pointer[i][j] = py;
                    }
                }
            }
        }
        public static void Assign4DPointer(double[,,,] input, ref double**** pointer)
        {
            var temptemptemp = new double***[input.GetLength(0)];
            fixed (double**** px = &temptemptemp[0])
            {
                pointer = px;
            }
            for (int i = 0; i < input.GetLength(0); i++)
            {
                var temptemp = new double**[input.GetLength(1)];
                fixed (double*** px = &temptemp[0])
                {
                    pointer[i] = px;
                }
                for (int j = 0; j < input.GetLength(1); j++)
                {
                    var temp = new double*[input.GetLength(2)];
                    fixed (double** py = &temp[0])
                    {
                        pointer[i][j] = py;
                    }
                    for (int k = 0; k < input.GetLength(3); k++)
                    {
                        fixed (double* pz = &input[i,j,k,0])
                        {
                            pointer[i][j][k] = pz;
                        }
                    }
                }
            }
        }
    }
}
