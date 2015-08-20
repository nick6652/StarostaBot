using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testBot1
{
    class User : DbEntry
    {
        public int TelegramUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        public virtual ICollection<Suggestion> Suggestions { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public User()
        {
            Ratings = new List<Rating>();
        }

        public override string ToString()
        {
            return
                (string.IsNullOrEmpty(FirstName) ? "" : FirstName) +
                (string.IsNullOrEmpty(LastName) ? "" : LastName) +
                (string.IsNullOrEmpty(Username) ? "" : Username);
        }
    }
}