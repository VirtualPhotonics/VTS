namespace System
{
    /// <summary>
    /// Additional Func which allows for five input arguments
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="arg4"></param>
    /// <param name="arg5"></param>
    /// <returns></returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    /// <summary>
    /// Additional Func which allows for six input arguments
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="arg4"></param>
    /// <param name="arg5"></param>
    /// <param name="arg6"></param>
    /// <returns></returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    
    /// <summary>
    /// Additional Func which allows for unlimited params TParams[] arguments (all inputs must be the same type)
    /// </summary>
    /// <typeparam name="TParams"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="paramters"></param>
    /// <returns></returns>
    public delegate TResult FuncWithParams<TParams, TResult>(params TParams[] paramters);

    /// <summary>
    /// Additional Func which allows for unlimited params TParams[] arguments (all inputs must be the same type)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TParams"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="paramters"></param>
    /// <returns></returns>
    public delegate TResult FuncWithParams<T1, TParams, TResult>(T1 arg1, params TParams[] paramters);

    /// <summary>
    /// Additional Func which allows for unlimited params TParams[] arguments (all inputs must be the same type)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TParams"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="paramters"></param>
    /// <returns></returns>
    public delegate TResult FuncWithParams<T1, T2, TParams, TResult>(T1 arg1, T2 arg2, params TParams[] paramters);
}
