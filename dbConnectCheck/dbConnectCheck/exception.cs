using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbConnectCheck
{
    /// <summary>
    /// Thrown when the program is unable to find a configuration file.
    /// </summary>
    [global::System.Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invalid configuration file is loaded.
    /// </summary>
    [global::System.Serializable]
    public class NotAConfigurationFileException : Exception
    {
        public NotAConfigurationFileException() { }
        public NotAConfigurationFileException(string message) : base(message) { }
        public NotAConfigurationFileException(string message, Exception inner) : base(message, inner) { }
        protected NotAConfigurationFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an invalid connection string is found.
    /// </summary>
    [global::System.Serializable]
    public class ConnectionStringErrorException : Exception
    {
        const string MESSAGE_PREFIX = "Connection string errors found:\n";

        public ConnectionStringErrorException() { }
        public ConnectionStringErrorException(string message) : base(MESSAGE_PREFIX + message) { }
        public ConnectionStringErrorException(string message, Exception inner) : base(MESSAGE_PREFIX + message, inner) { }
        protected ConnectionStringErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
