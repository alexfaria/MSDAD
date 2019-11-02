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
        public string meetingTopic;
        public InvalidMeetingException(string meetingTopic)
        {
            this.meetingTopic = meetingTopic;
        }

        public InvalidMeetingException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
        {
            meetingTopic = info.GetString("meetingTopic");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("meetintTopic", meetingTopic);
        }
    }
}
