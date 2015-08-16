using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testBot1
{
    class Suggestion : DbEntry
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }
        public virtual User AddedBy { get; set; }

        public override string ToString()
        {
            return Title + "\nдобавлено " + AddedBy.ToString() + "\n рейтинг: " + Rate + "\n" + Text;
        }
    }
}
