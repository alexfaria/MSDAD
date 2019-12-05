using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class VectorClock
    {
        string sender_url;
        Dictionary<string, int> vc;

        public Dictionary<string, int> vector => vc;
        public int Count => vc.Count;
        public int this[string key]
        {
            get {
                int v;
                vc.TryGetValue(key, out v);
                vc[key] = v;
                return v;
            }
            set => vc[key] = value;
        }
        public VectorClock()
        {
            this.sender_url = String.Empty;
            this.vc = new Dictionary<string, int>();
        }
        public VectorClock(string id)
        {
            this.sender_url = id;
            this.vc = new Dictionary<string, int>();
            this.vc.Add(id, 0);
        }
        public VectorClock(string id, Dictionary<string, int> vc)
        {
            this.vc = vc;
            this.sender_url = id;
        }
        public void Init(Dictionary<string, int>.KeyCollection ids)
        {
            foreach (string id in ids)
            {
                this.vc.Add(id, 0);
            }
        }
        public void Add(string id, int value)
        {
            this.vc.Add(id, value);
        }

        public void TryGetValue(string sender_url, out int currentCount)
        {
            vc.TryGetValue(sender_url, out currentCount);
        }

        public void PrettyPrint()
        {
            Console.WriteLine("Vector Clock:");
            foreach (KeyValuePair<string, int> seq in vc)
            {
                Console.WriteLine($"  {seq.Key} -> {seq.Value}");
            }
        }

        public bool Delay(VectorClock other)
        {
            string sender = other.sender_url;

            if (this[sender] != other[sender] - 1)
            {
                return false;
            }

            foreach (string key in other.vc.Keys)
            {
                if (key != this.sender_url)
                {
                    if (this[key] < other[key])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool Caused(VectorClock other)
        {
            bool caused = true;
            string id = other.sender_url;

            if (vc[id] == (other.vc[id] - 1))
            {
                foreach (KeyValuePair<string, int> entry in vc)
                {
                    if ((entry.Key != id) && (vc[entry.Key] < other.vc[entry.Key]))
                    {
                        caused = false;
                    }
                }
            }
            else
            {
                caused = false;
            }

            return caused;
        }
    }
}
