using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class InvalidMeetingException : ApplicationException
    {
        public string message;
        public InvalidMeetingException(string message)
        {
            this.message = message;
        }

        public InvalidMeetingException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
        {
            message = info.GetString("message");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("message", message);
        }
    }
}
