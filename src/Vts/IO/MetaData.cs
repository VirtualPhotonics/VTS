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
        public int[] dims { get; set; }
        //[IgnoreDataMember]
        /// <summary>
        /// Current date and time
        /// </summary>
        public DateTime datetime => DateTime.Now;

        /// <summary>
        /// The type of the array
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// Name of the file
        /// </summary>
        public string filename { get; set; }

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
            if (myArray is Array[])
            {
                var array = myArray as Array[];
                var subDims = CreateDimensions(array[0]);

                dims = new int[array.Length + subDims.Length];
                dims[0] = array.Length;
                subDims.CopyTo(dims, 1);
            }
            else //(myArray is Time[] or myArray is Time[,] or myArray is Time[, ,] or myArray is Time[, ,] || myArray is Time[, , ,])
            {
                dims = new int[myArray.Rank];
                for (int i = 0; i < dims.Length; i++)
                {
                    dims[i] = myArray.GetLength(i);
                }
            }
            return dims;
        }

    }

}
