using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Model
{
    public class Ticket
    {
        public int number;
        public string operation;
        public Ticket(int number, string operation)
        {
            this.number = number;
            this.operation = operation;
        }
    }
}
