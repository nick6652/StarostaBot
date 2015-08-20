using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Telegram;

namespace testBot1
{
    class RateCommand : Command
    {
        public override bool Execute()
        {

            string suggestionId;
            if (string.IsNullOrEmpty(Arguments))
            {
                Console.WriteLine(GetType().Name + ": Sending 'Enter suggestion number' prompt");
                try
                {
                    TelegramApi.SendMessage(Message.From, "Введите номер предложения");
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while sending prompt", e);
                }

                Console.WriteLine(GetType().Name + ": Waiting for a message that contains show title");
                try
                {
                    suggestionId = TelegramApi.WaitForMessage(Message.From).Text;
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while waiting for a message that contains show title", e);
                }
            }
            else
            {
                suggestionId = Arguments;
            }

            ReplyKeyboardMarkup replyRateKB = new ReplyKeyboardMarkup();
            replyRateKB.Keyboard = new string[][] 
                    {
                        new string[] {"/+","/-"},
                        new string[] {"/back"}

                    };
            replyRateKB.OneTimeKeyboard = true;
            replyRateKB.ResizeKeyboard = true;
            replyRateKB.Selective = false;

            int sId = int.Parse(suggestionId);

            using (var db = new AppDbContext())
            {
                Suggestion suggestion;
                //Rating rating;
                try
                {
                    suggestion = db.GetSuggestionById(sId);
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while searching suggestion #" + suggestionId + " in database", e);
                }

                if (suggestion != null)
                {
                    Console.WriteLine(GetType().Name + ": Searching user with TelegramId: " + Message.From.Id + " in database");
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

                    Rating rating;

                    try
                    {
                        var _user = db.Users.Include(u => u.Ratings).FirstOrDefault(r => r.Id==user.Id);
                        rating = _user.Ratings.FirstOrDefault(r => r.Suggestion.Id == suggestion.Id);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(GetType().Name + ": An error occurred while checking for rating", e);
                    }
                    /* if (rating == null)
                     {
                         rating = new Rating
                         {
                             User = user,
                             Suggestion = suggestion,
                             isRatedUp = true
                         }
                     }*/

                    //}


                    //do
                    //{
                    //using (var db = new AppDbContext())
                    //{

                    /*if (suggestion.AddedBy == userRated)
                        replyRateKB.Keyboard[1] = new string[] { "/delete" };*/

                    if (rating != null)
                    {
                        if (rating.isRatedUp)
                        {
                            replyRateKB.Keyboard[0] = new string[] { "/-" };
                        }
                        else
                        {
                            replyRateKB.Keyboard[0] = new string[] { "/+" };
                        }
                    }
                    else
                    {
                        replyRateKB.Keyboard[0] = new string[] { "/+", "/-" };
                    }

                    try
                    {
                        TelegramApi.SendMessage(Message.From, suggestion.ToString(), replyRateKB);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(GetType().Name + ": An error occurred while sending suggest description", e);
                    }

                    Message message;
                    try
                    {
                        message = TelegramApi.WaitForMessage(Message.From);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(GetType().Name + ": An error occurred while waiting for a message with rate action", e);
                    }

                    /*if (suggestion.AddedBy != userRated)
                    {*/
                    if (rating != null)
                    {
                        if (message.Text == "/+" && !rating.isRatedUp)
                        {
                            suggestion.Rate += 1;
                            db.Ratings.Remove(rating);
                        }
                        else if (message.Text == "/-" && rating.isRatedUp)
                        {
                            suggestion.Rate -= 1;
                            db.Ratings.Remove(rating);
                        }
                    }
                    else
                    {
                        if (message.Text == "/+")
                        {
                            suggestion.Rate += 1;
                            rating = new Rating()
                            {
                                User = user,
                                Suggestion = suggestion,
                                isRatedUp = true
                            };
                            db.Ratings.Add(rating);
                        }
                        else if (message.Text == "/-")
                        {
                            suggestion.Rate -= 1;
                            rating = new Rating()
                            {
                                User = user,
                                Suggestion = suggestion,
                                isRatedUp = false
                            };
                            db.Ratings.Add(rating);
                        }
                    }
                    //}                    
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine(GetType().Name + ": Suggestion does not exist");
                    try
                    {
                        TelegramApi.SendMessage(Message.From, "Suggestion does not exist");
                    }
                    catch (Exception e)
                    {
                        throw new Exception(GetType().Name + ": An error occured while sending notification of nonexistent suggestion", e);
                    }
                }
                //} while (message.Text != "/+" && message.Text != "/-" && message.Text != "/delete" && message.Text != "/back");
            }
            Status = true;
            return true;
        }
    }
}


