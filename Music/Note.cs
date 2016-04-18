using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace Music
{
    [Serializable]
    public class Note
    {
        public int frequency { get; set; }
        public int duration { get; set; }
        public Note(int frequency, int duration)
        {
            this.frequency = frequency;
            this.duration = duration;
        }

        public override string ToString()
        {
            return String.Format("Frequency {0} , Duration {1}", frequency, duration);
        }
    }
}
