using System;
using System.Linq;

namespace testBot1
{
    public class SuggestCommand : Command
    {
        public override bool Execute()
        {
            string suggestionTitle;
            string suggestionText;

            try
            {
                TelegramApi.SendMessage(Message.From, "Введите название для идеи");
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending prompt", e);
            }
            try
            {
                suggestionTitle = TelegramApi.WaitForMessage(Message.From).Text;
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while waiting for a message that contains show title", e);
            }
            try
            {
                TelegramApi.SendMessage(Message.From, "Введите текст самой идеи");
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending prompt", e);
            }
            try
            {
                suggestionText = TelegramApi.WaitForMessage(Message.From).Text;
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while waiting for a message that contains show title", e);
            }


            string response;
            using (AppDbContext db = new AppDbContext())
            {
                Suggestion suggestion;

                /*if (suggestion.Title != "" && suggestion.Text != "")
                {*/
                User user;
                try
                {
                    user = db.GetUserByTelegramId(Message.From.Id);
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while searching user in database", e);
                }
                bool newUser = false;
                if (user == null)
                {
                    user = new User
                    {
                        TelegramUserId = Message.From.Id,
                        FirstName = Message.From.FirstName,
                        LastName = Message.From.LastName,
                        Username = Message.From.Username
                    };
                    newUser = true;
                }

                if (newUser)
                {
                    Console.WriteLine(GetType().Name + ": " + user + " is new User");
                }
                else
                {
                    Console.WriteLine(GetType().Name + ": User " + user + " already exists");
                }


                suggestion = new Suggestion
                {
                    Title = suggestionTitle,
                    Text = suggestionText,
                    Rate = 0,
                    AddedBy = user
                };

                if (newUser)
                {
                    user.Suggestions.Add(suggestion);
                    db.Users.Add(user);
                }
                else
                {
                    db.Suggestions.Add(suggestion);
                }
                response = "Ваша идея добавлена";

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while saving changes to database", e);
                }
            }



            try
            {
                TelegramApi.SendMessage(Message.From, response);
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending response to " + Message.From, e);
            }
            Status = true;
            return true;
        }
    }
}
