using System;
using System.Linq;
using System.Data.Entity;

namespace testBot1
{
    class AppDbContext : DbContext
    {
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public AppDbContext() : base("DbConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        public User GetUserByTelegramId(int telegramId)
        {
            return Users.FirstOrDefault(e => e.TelegramUserId == telegramId);
        }

        public Suggestion GetSuggestionById(int sId)
        {
            return Suggestions.Include(s => s.AddedBy).FirstOrDefault(s => s.Id == sId);
        }

        /*public Rating GetRatingByUserAndSuggestion(User user, Suggestion suggestion)
        {
            var _rating = Ratings.
            return null;
        }*/

    }
}
