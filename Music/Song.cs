using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace Music
{
    [Serializable]
    public class Song
    {
        public List<Note> noteList { get; set; } = new List<Note>();
        public Song ()
        {
        }
        public Song (List<Note> noteList)
        {
            this.noteList = noteList;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(var note in noteList)
            {
                builder.AppendFormat("{0}, ", note);
            }
            return builder.ToString();
        }
    }
}
