namespace Agrin.UI.Helper.SingleApplication
{
    #region Namespaces
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    #endregion

    /// <summary>
    ///   Class used to define an exception raised when something went bad during a <see cref = "InstanceAwareApplication" /> startup.
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UnexpectedInstanceAwareApplicationException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnexpectedInstanceAwareApplicationException" /> class.
        /// </summary>
        public UnexpectedInstanceAwareApplicationException()
                : base("Unexpected exception while starting up instance aware application")
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnexpectedInstanceAwareApplicationException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        public UnexpectedInstanceAwareApplicationException(string message)
                : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnexpectedInstanceAwareApplicationException" /> class.
        /// </summary>
        /// <param name = "info">The <see cref = "System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name = "context">The <see cref = "System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "System.ArgumentNullException">The <paramref name = "info" /> parameter is null. </exception>
        /// <exception cref = "System.Runtime.Serialization.SerializationException">The class name is null or <see cref = "System.Exception.HResult" /> is zero (0). </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected UnexpectedInstanceAwareApplicationException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UnexpectedInstanceAwareApplicationException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <param name = "inner">The inner.</param>
        public UnexpectedInstanceAwareApplicationException(string message, Exception inner)
                : base(message, inner)
        {
        }
    }

    /// <summary>
    ///   Class used to define an exception raised when no application <see cref = "Guid" /> was defined.
    /// </summary>
    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UndefinedApplicationGuidException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "UndefinedApplicationGuidException" /> class.
        /// </summary>
        public UndefinedApplicationGuidException()
                : base("No application Guid was defined")
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UndefinedApplicationGuidException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        public UndefinedApplicationGuidException(string message)
                : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UndefinedApplicationGuidException" /> class.
        /// </summary>
        /// <param name = "info">The <see cref = "System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name = "context">The <see cref = "System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref = "System.ArgumentNullException">The <paramref name = "info" /> parameter is null. </exception>
        /// <exception cref = "System.Runtime.Serialization.SerializationException">The class name is null or <see cref = "System.Exception.HResult" /> is zero (0). </exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected UndefinedApplicationGuidException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UndefinedApplicationGuidException" /> class.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <param name = "inner">The inner.</param>
        public UndefinedApplicationGuidException(string message, Exception inner)
                : base(message, inner)
        {
        }
    }
}