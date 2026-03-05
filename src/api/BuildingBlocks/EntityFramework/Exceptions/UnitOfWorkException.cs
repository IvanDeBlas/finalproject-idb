using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WePlayRises.BuildingBlocks.EntityFramework.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException() { }
        public UnitOfWorkException(string message) : base(message) { }
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException) { }
    }
}
