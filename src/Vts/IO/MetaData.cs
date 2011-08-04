using System;

namespace Vts.IO
{

    /// <summary>
    /// This class will create metadata objects that will aid in the reading of binary files created 
    /// with the BinaryWriter tools
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// The dimensions of the array
        /// </summary>
        public int[] dims;
        //[IgnoreDataMember]
        /// <summary>
        /// Current data and time
        /// </summary>
        public DateTime datetime = System.DateTime.Now;
        /// <summary>
        /// The type of the array
        /// </summary>
        public string ObjectType;
        /// <summary>
        /// Name of the file
        /// </summary>
        public string filename;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MetaData() { }

        /// <summary>
        /// Constructor that passes the array and sets the object type and dimensions
        /// </summary>
        /// <param name="myArray">Array for which meta data is needed</param>
        public MetaData(Array myArray)
        {
            ObjectType = myArray.GetType().ToString();
            dims = CreateDimensions(myArray);
        }

        private int[] CreateDimensions(Array myArray)
        {
            //if (myArray is Array[][])
            //{
            //    var array = myArray as Array[];
            //    foreach (var item in collection)
            //    {

            //    }
            //}
            //else 
            if (myArray is Array[])
            {
                var array = myArray as Array[];
                var subDims = CreateDimensions(array[0]);

                dims = new int[array.Length + subDims.Length];
                dims[0] = array.Length;
                subDims.CopyTo(dims, 1);
            }
            else //if (myArray is T[] || myArray is T[,] || myArray is T[, ,] || myArray is T[, ,] || myArray is T[, , ,])
            {
                dims = new int[myArray.Rank];
                for (int i = 0; i < dims.Length; i++)
                {
                    dims[i] = myArray.GetLength(i);
                }
            }
            return dims;
        }
        //private MetaData() { }

        //public MetaData(Array myArray)
        //{
        //    ObjectType = myArray.GetType().ToString();
        //    dims = CreateDimensions(myArray);
        //}

        //private int[] CreateDimensions(Array myArray)
        //{
        //    //if (myArray is Array[][])
        //    //{
        //    //    var array = myArray as Array[];
        //    //    foreach (var item in collection)
        //    //    {
                    
        //    //    }
        //    //}
        //    //else 
        //    if (myArray is Array[])
        //    {
        //        var array = myArray as Array[];
        //        var subDims = CreateDimensions(array[0]);

        //        dims = new int[array.Length + subDims.Length];
        //        dims[0] = array.Length;
        //        subDims.CopyTo(dims, 1);
        //    }
        //    else if (myArray is T[] || myArray is T[,] || myArray is T[, ,] || myArray is T[, ,] || myArray is T[, , ,])
        //    {
        //        dims = new int[myArray.Rank];
        //        for (int i = 0; i < dims.Length; i++)
        //        {
        //            dims[i] = myArray.GetLength(i);
        //        }
        //    }
        //    return dims;
        //}

        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[1] { input.Length };
        //}
        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[,] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[2] { input.GetLength(0), 
        //                        input.GetLength(1) };
        //}
        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[, ,] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[3] { input.GetLength(0), 
        //                        input.GetLength(1), 
        //                        input.GetLength(2)  };
        //}
        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[, , ,] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[4] { input.GetLength(0), 
        //                        input.GetLength(1), 
        //                        input.GetLength(2),   
        //                        input.GetLength(3)  };
        //}

        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[][] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[2] { input.Length, 
        //                        input[0].Length};
        //}
        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[][,] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[3] { input.Length, 
        //                        input[0].GetLength(0), 
        //                        input[0].GetLength(1) };
        //}
        ///// <summary>
        ///// Overload of the constructor to handle creation of Metadata for 3D jagged arrays
        ///// </summary>
        ///// <param name="input"></param>
        //public MetaData(T[][][,] input)
        //{
        //    ObjectType = input.GetType().ToString();
        //    dims = new int[4] { input.Length, 
        //                        input[0].Length,
        //                        input[0][0].GetLength(0), 
        //                        input[0][0].GetLength(1) };
        //}
    }

}
