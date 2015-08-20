using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.Entity;
using Telegram;

namespace testBot1
{
    class BrowseCommand : Command
    {
        private const int MaxPageSize = 50;

        private static readonly Regex SuggestionNumRegex = new Regex(@"[0-9]+");

        public BrowseCommand(TelegramApi telegramApi, Message message) : base(telegramApi, message)
        {
        }

        public BrowseCommand()
        {
        }

        public override bool Execute()
        {
            //Program.Logger.Debug($"{GetType().Name}: Parsing message size. Arguments: {Arguments}");
            int messageSize;
            int.TryParse(Arguments, out messageSize);
            if (messageSize == 0)
            {
                messageSize = MaxPageSize;
            }
            messageSize = Math.Min(messageSize, MaxPageSize);
            //Program.Logger.Debug($"{GetType().Name}: Message size: {messageSize}");

            List<string> suggestions;

            //Program.Logger.Debug($"{GetType().Name}: Retrieving shows list");
            using (var db = new AppDbContext())
            {
                try
                {
                    suggestions = db.Suggestions.Select(s => "/" + s.Id + " " + s.Title + "\nрейтинг: "+ s.Rate  + "\nпредложено " + s.AddedBy.FirstName + " " + s.AddedBy.LastName).ToList();
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while retrieving shows list", e);
                }
            }


            List<string> pagesList = new List<string>();
            for (int i = 0; i < suggestions.Count; i += messageSize)
            {
                if (i > suggestions.Count)
                {
                    break;
                }

                int count = Math.Min(suggestions.Count - i, messageSize);
                pagesList.Add(
                    suggestions.GetRange(i, count)
                    .Aggregate("", (s, s1) => s + "\n" + s1)
                    );
            }
      
            try
            {
                //Program.Logger.Debug($"{GetType().Name}: Sending shows list");

                for (int i = 0; i < pagesList.Count; i++)
                {
                    string page = pagesList[i];
                    ReplyKeyboardMarkup replyKB = new ReplyKeyboardMarkup();
                    replyKB.Keyboard = new string[][] 
                    {
                        new string[] {"/next","/stop"}

                    };
                    replyKB.OneTimeKeyboard = true;
                    replyKB.ResizeKeyboard = true;
                    replyKB.Selective = false;

                    if (i != pagesList.Count - 1)
                    {
                        page += "\n/next or /stop";
                        replyKB.Keyboard[0]= new string[] {"/next","/stop"};
                    }
                                     
                    if (i == pagesList.Count - 1)
                    {
                        page += "\n/stop";
                        replyKB.Keyboard[0]= new string[] {"/stop"};
                    }
                    TelegramApi.SendMessage(Message.From, page,replyKB);
                    Message message;
                    /*ReplyKeyboardMarkup replyKB = new ReplyKeyboardMarkup();
                    replyKB.Keyboard = new string[][] 
                    {
                        new string[] {"/next","/stop"}

                    };
                    replyKB.OneTimeKeyboard = true;
                    replyKB.ResizeKeyboard = true;
                    replyKB.Selective = false;*/
                    ReplyKeyboardMarkup replyRateKB = new ReplyKeyboardMarkup();
                    replyRateKB.Keyboard = new string[][] 
                    {
                        new string[] {"/+","/-"}

                    };
                    replyRateKB.OneTimeKeyboard = true;
                    replyRateKB.ResizeKeyboard = true;
                    replyRateKB.Selective = false;
                    string footerText = "";
                    
                    do
                    {
                        message = TelegramApi.WaitForMessage(Message.From);
                        if (message.Text != "/stop" && (i == pagesList.Count - 1 || message.Text != "/next"  ))
                        {
                            //if (message.Text = )
                            string someMatch = SuggestionNumRegex.Match(message.Text).Groups[0].Value;
                            if (someMatch == "" || someMatch == null)
                            {
                                if (i == pagesList.Count - 1)
                                {
                                    footerText = "\n/stop or /+";
                                    replyKB.Keyboard[0]= new string[] {"/stop"};
                                }
                                else
                                {
                                    footerText = "\n/next or /stop or /+";
                                    replyKB.Keyboard[0]= new string[] {"/next","/stop"};
                                }
                                TelegramApi.SendMessage(Message.From, footerText, replyKB);
                            }
                            else
                            {
                                using (var db = new AppDbContext())
                                {
                                    try
                                    {
                                        int sId = int.Parse(someMatch);
                                        Suggestion fullSuggestion = db.Suggestions.Include(s => s.AddedBy).FirstOrDefault(s => s.Id == sId);

                                        /*Console.WriteLine(fullSuggestion.Title + "\nдобавлено " + fullSuggestion.AddedBy.FirstName +
                                            " " + fullSuggestion.AddedBy.LastName +  "\n рейтинг: " + fullSuggestion.Rate + "\n" + fullSuggestion.Text);*/
                                        TelegramApi.SendMessage(Message.From, fullSuggestion.ToString(), replyRateKB);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(GetType().Name + ": An error occurred while sending suggest description", e);
                                    }
                                }
                            }


                        }
                    } while (message.Text != "/stop" && (i == pagesList.Count - 1 || message.Text != "/next"));

                    if (message.Text == "/stop")
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending suggestions list", e);
            }

            Status = true;
            return true;
        }
    }
}