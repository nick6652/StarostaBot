using System;

namespace testBot1
{
    class HelpCommand : Command
    {
        public override bool Execute()
        {
            //Program.Logger.Debug($"{GetType().Name}: Sending help message to {Message.From}");

            try
            {
                TelegramApi.SendMessage(Message.From, "Список команд: \n" +
                "/suggest - предложить идею \n" +
                "/browse - список идей \n" +
                "/help - справка"
                );
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending help message to " + Message.From, e);
            }

            Status = true;
            return true;
        }
    }
}