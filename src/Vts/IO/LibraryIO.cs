using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Vts.Extensions;


namespace Vts.IO
{
    /// <summary>
    /// This class includes methods for dynamically loading DLLs
    /// </summary>
    public static class LibraryIO
    {
#if SILVERLIGHT
        //public object LoadDLL(string filename, string projectName)
        //{

        //}
#else
        //public object LoadDLL(string filename, string typeName)
        //{
        //    byte[] bytes = File.ReadAllBytes(filename);
        //    Assembly LoadedDLL = Assembly.load(bytes);
        //    return LoadedDLL.CreateInstance(typeName);
        //}
#endif
    }
}