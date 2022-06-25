using System;
using System.Runtime.Serialization;

namespace server.Hubs.Services
{
    [Serializable]

    //Validation Exception (General)

//    throw new MessageTextNotValidException("Name doesn't exist.", "John");

//    try 
//	{	        
		
//	}

//    catch (MessageTextNotValidException ex)
//{

//    throw new ValdationException("Message", ex);
//}

    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        // Most Used
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}