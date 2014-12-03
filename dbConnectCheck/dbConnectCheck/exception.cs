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
    /// Thrown when problems are encountered with the configuration file.
    /// </summary>
    [global::System.Serializable]
    public class ConfigurationFileException : Exception
    {
        public ConfigurationFileException() { }
        public ConfigurationFileException(string message) : base(message) { }
        public ConfigurationFileException(string message, Exception inner) : base(message, inner) { }
        protected ConfigurationFileException(
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
