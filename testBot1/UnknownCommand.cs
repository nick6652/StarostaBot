﻿using System;

namespace testBot1
{
    class UnknownCommand : Command
    {
        public override bool Execute()
        {
            //Program.Logger.Debug($"{GetType().Name}: Sending message to {Message.From}");
            try
            {
                TelegramApi.SendMessage(Message.From, "Неизвестная команда");
            }
            catch (Exception e)
            {
                throw new Exception(GetType().Name + ": An error occurred while sending message to " + Message.From, e);
            }

            Status = true;
            return true;
        }
    }
}