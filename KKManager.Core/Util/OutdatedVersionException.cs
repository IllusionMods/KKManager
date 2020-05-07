using System;

namespace KKManager.Util
{
    /// <summary>
    /// Thrown if KK Manager needs to be updated
    /// </summary>
    public sealed class OutdatedVersionException : Exception
    {
        public OutdatedVersionException(string message) : base(message)
        {
        }
    }
}