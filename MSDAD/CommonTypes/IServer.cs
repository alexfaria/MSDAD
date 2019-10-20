using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IServer
    {
        void WriteLine(string message);
        List<Meeting> GetMeetings();

        void CreateMeeting(Meeting m);

        void JoinMeeting(string user, Meeting meeting, Slot slot);
    }
}
