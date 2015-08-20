using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testBot1
{
    class Rating : DbEntry
    {
        public virtual User User { get; set; }
        public virtual Suggestion Suggestion { get; set; }
        public bool isRatedUp { get; set; }

    }
}