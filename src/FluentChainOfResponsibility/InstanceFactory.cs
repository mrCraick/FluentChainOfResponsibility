using System;

namespace FluentChainOfResponsibility
{
    /// <summary>
    /// Factory method used to resolve an instance
    /// </summary>
    /// <param name="instanceType">Type of instance to resolve</param>
    /// <returns></returns>
    public delegate object InstanceFactory(Type instanceType);

    /// <summary>
    /// <see cref="InstanceFactory"/> extension
    /// </summary>
    public static class InstanceFactoryExtensions
    {
        /// <summary>
        /// Get an instance
        /// </summary>
        /// <typeparam name="T">Type of instance to resolve</typeparam>
        /// <param name="factory">Factory method</param>
        /// <returns>An instance</returns>
        public static T GetInstance<T>(this InstanceFactory factory)
        {
            return (T) factory(typeof(T));
        }

        /// <summary>
        /// Get an instance or null if it cannot resolve it
        /// </summary>
        /// <typeparam name="T">Type of instance to resolve</typeparam>
        /// <param name="factory">Factory method</param>
        /// <returns>An instance or null</returns>
        public static T? GetOptionInstance<T>(this InstanceFactory factory)
            where T : class
        {
            try
            {
                return factory.GetInstance<T>();
            }
            catch
            {
                return default;
            }
        }
    }
}