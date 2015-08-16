using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Telegram;

namespace testBot1
{
    class RateCommand : Command
    {
        public override bool Execute()
        {
            ReplyKeyboardMarkup replyRateKB = new ReplyKeyboardMarkup();
                    replyRateKB.Keyboard = new string[][] 
                    {
                        new string[] {"/+","/-"},
                        new string[] {"/back"}

                    };
                    replyRateKB.OneTimeKeyboard = true;
                    replyRateKB.ResizeKeyboard = true;
                    replyRateKB.Selective = false;
            
            int sId = int.Parse(Arguments);
            Message message;
            Suggestion fullSuggestion;

            using (var db = new AppDbContext())
            {
                try
                {
                    fullSuggestion = db.Suggestions.Include(s => s.AddedBy).FirstOrDefault(s => s.Id == sId);

                    TelegramApi.SendMessage(Message.From, fullSuggestion.ToString(), replyRateKB);
                }
                catch (Exception e)
                {
                    throw new Exception(GetType().Name + ": An error occurred while sending suggest description", e);
                }
            }

            do
            {
                message = TelegramApi.WaitForMessage(Message.From);
                using (var db = new AppDbContext())
                {
                    if (message.Text == "/+")
                        db.Suggestions.Find(sId).Rate += 1;
                    if (message.Text == "/-")
                        db.Suggestions.Find(sId).Rate -= 1;

                    db.SaveChanges();
                }
            } while (message.Text != "/+" && message.Text != "/-" && message.Text != "/back");
            Status = true;
            return true;
        }
    }
}


