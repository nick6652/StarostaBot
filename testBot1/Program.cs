using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Telegram;
using System.Collections.Concurrent;
using System.Threading;

namespace testBot1
{
  
    class Program
    {

        //static int user_id = 116815836;
        static string _token = "93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY";

        static void Main(string[] args)
        {
            
            TelegramApi telegram = new TelegramApi(_token);
            try
            {
                Console.WriteLine("Executing GetMe");
                var botUser = telegram.GetMe();
                Console.WriteLine("GetMe returned \n" + botUser.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("GetMe failed");
                Console.WriteLine(e.Message);
                return;
            }
            /*
            ReplyKeyboardMarkup replyKB = new ReplyKeyboardMarkup();
            replyKB.Keyboard = new string[][] 
                    {
                        new string[] {"Yes","No"}

                    };
            replyKB.OneTimeKeyboard = false;
            replyKB.ResizeKeyboard = false;
            replyKB.Selective = false;*/

            //string json = JsonConvert.SerializeObject(replyKB);

            telegram.StartPolling();

            //var processingCommandUsers = new ConcurrentDictionary<User, bool>();
            //Regex commandRegex = new Regex(@"(/\w+)\s*");

            while (true)
            {
                foreach (var update in telegram.Updates)
                {
                    /*if (processingCommandUsers.ContainsKey(update.Key) &&
                        processingCommandUsers[update.Key])
                    {
                        continue;
                    }*/

                    if (update.Value.Count == 0)
                    {
                        continue;
                    }
                    Message message;
                    update.Value.TryDequeue(out message);

                    /*Logger.Debug($"Received message '{message.Text}' from " +
                                 $"{message.From.FirstName} {message.From.LastName}");*/
                    //string commandTitle = commandRegex.Match(message.Text).Groups[1].Value;

                    string[][] keyboard = new string[][] 
                    {
                        new string[] {"Yes","No"}

                    };


                    ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup() {Keyboard = new string[][] {
                                                                                       new string[] {"Yes","No"}
                                                                                   } };

                    string testText = message.Text.ToUpper();


                    Console.WriteLine("Received a message: " + testText);

                    //Logger.Debug($"Creating command object for '{commandTitle}'");
                    //var command = Command.CreateCommand(commandTitle);
                    //Logger.Info($"Received {command.GetType().Name} from " +
                    //            $"{message.From.FirstName} {message.From.LastName}");

                    //command.TelegramApi = tg;
                    //command.Message = message;

                    //Logger.Debug($"Executing {command.GetType().Name}");
                    //processingCommandUsers[update.Key] = true;
                    try
                    {
                        telegram.SendMessage(message.From, testText, replyKeyboard);
                     //   telegram.SendMessage(message.From, " ", replyKeyboard);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }

                    /*Task commandTask = Task.Run(() =>
                    {
                        try
                        {
                            command.Execute();
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"An error occurred while executing {command.GetType().Name}.\n" +
                                         $"Message: {command.Message.Text}\n" +
                                         $"Arguments: {command.Arguments}\n" +
                                         $"User: {command.Message.From}");
                            Logger.Error(e);
                        }
                    }
                        );
                    commandTask.ContinueWith(task =>
                    {
                        processingCommandUsers[update.Key] = false;
                        Logger.Debug($"{command.GetType().Name} from " +
                                     $"{message.From.FirstName} {message.From.LastName} {(command.Status ? "succeeded" : "failed")}");
                    });*/
                }
                Thread.Sleep(200);
            }

        }

    }
}
