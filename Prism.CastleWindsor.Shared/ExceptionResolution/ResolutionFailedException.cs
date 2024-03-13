using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Prism.CastleWindsor.ExceptionResolution
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResolutionFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeRequested"></param>
        /// <param name="nameRequested"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResolutionFailedException(Type typeRequested,
                                         string nameRequested,
                                         string message,
                                         Exception innerException = null) : base(message, innerException)
        {
            Type type = typeRequested;

            if ((object) type == null)
                throw new ArgumentNullException(nameof (typeRequested));

            this.TypeRequested = type.GetTypeInfo().Name;
            this.NameRequested = nameRequested;

            this.RegisterSerializationHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        public string TypeRequested { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string NameRequested { get; private set; }

        private void RegisterSerializationHandler()
        {
            this.SerializeObjectState += (EventHandler<SafeSerializationEventArgs>) ((s, e) =>
                e.AddSerializedState(
                    (ISafeSerializationData) new ResolutionFailedException.ResolutionFailedExceptionSerializationData(
                        this.TypeRequested, this.NameRequested)));
        }

        [Serializable]
        private struct ResolutionFailedExceptionSerializationData : ISafeSerializationData
        {
            private readonly string _typeRequested;
            private readonly string _nameRequested;

            public ResolutionFailedExceptionSerializationData(string typeRequested, string nameRequested)
            {
                this._typeRequested = typeRequested;
                this._nameRequested = nameRequested;
            }

            public void CompleteDeserialization(object deserialized)
            {
                ResolutionFailedException resolutionFailedException = (ResolutionFailedException) deserialized;
                resolutionFailedException.TypeRequested = this._typeRequested;
                resolutionFailedException.NameRequested = this._nameRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected ResolutionFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public ResolutionFailedException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ResolutionFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ResolutionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
