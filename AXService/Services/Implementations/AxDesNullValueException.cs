using System;
using System.Runtime.Serialization;

[Serializable]
public class AxDesNullValueException : Exception
{
    public AxDesNullValueException() { }

    public AxDesNullValueException(string message)
        : base(message) { }


    public AxDesNullValueException(string message, Exception innerException)
        : base(message, innerException) { }

    protected AxDesNullValueException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
