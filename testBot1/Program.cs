using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Telegram;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace testBot1
{
        
    class Program
    {

        //static int user_id = 116815836;
        static string _token = "93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY";

        private static int _retryPollingDelay=200;

        private static void StartPolling(TelegramApi tgApi)
        {
            //Logger.Debug("Starting polling");
            Task pollingTask = tgApi.StartPolling();
            pollingTask.ContinueWith(e =>
            {
                //Logger.Error(e.Exception, "An error occurred while retrieving updates");
                new Timer(o =>
                {
                    StartPolling(tgApi);
                }, null, _retryPollingDelay, -1);
            },
          TaskContinuationOptions.OnlyOnFaulted);
        }


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
            
            using (var db = new AppDbContext())
            {
                db.SaveChanges();
                var query = from b in db.Suggestions
                            orderby b.Id
                            select b;
                Console.WriteLine("All suggestions:");
                foreach (var item in query)
                {
                    Console.WriteLine(item.Id + " " + item.Text);
                }
                //Console.ReadKey();
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

            StartPolling(telegram);

            var processingCommandUsers = new ConcurrentDictionary<Telegram.User, bool>();
            Regex commandRegex = new Regex(@"(/\w+)\s*");

            while (true)
            {
                foreach (var update in telegram.Updates)
                {
                    if (processingCommandUsers.ContainsKey(update.Key) &&
                        processingCommandUsers[update.Key])
                    {
                        continue;
                    }

                    if (update.Value.Count == 0)
                    {
                        continue;
                    }
                    Message message;
                    update.Value.TryDequeue(out message);

                    /*Logger.Debug($"Received message '{message.Text}' from " +
                                 $"{message.From.FirstName} {message.From.LastName}");*/
                    string commandTitle = commandRegex.Match(message.Text).Groups[1].Value;

                    /*ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup() {Keyboard = new string[][] {
                                                                                       new string[] {"Yes","No"}
                                                                                   },
                                                                                   OneTimeKeyboard = true
                    };*/

                    string testText = message.Text;


                    Console.WriteLine("Received a message: " + testText);

                    //Logger.Debug($"Creating command object for '{commandTitle}'");
                    var command = Command.CreateCommand(commandTitle);
                    //Logger.Info($"Received {command.GetType().Name} from " +
                    //            $"{message.From.FirstName} {message.From.LastName}");

                    command.TelegramApi = telegram;
                    command.Message = message;

                    //Logger.Debug($"Executing {command.GetType().Name}");
                    processingCommandUsers[update.Key] = true;

                    try
                    {
                       // telegram.SendMessage(message.From, testText, replyKeyboard);
                     //   telegram.SendMessage(message.From, " ", replyKeyboard);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }

                    Task commandTask = Task.Run(() =>
                    {
                        try
                        {
                            command.Execute();
                        }
                        catch (Exception e)
                        {
                            /*Logger.Error($"An error occurred while executing {command.GetType().Name}.\n" +
                                         $"Message: {command.Message.Text}\n" +
                                         $"Arguments: {command.Arguments}\n" +
                                         $"User: {command.Message.From}");
                            Logger.Error(e);*/
                            Console.WriteLine(e.Message);
                        }
                    }
                        );
                    commandTask.ContinueWith(task =>
                    {
                        processingCommandUsers[update.Key] = false;
                        /*Logger.Debug($"{command.GetType().Name} from " +
                                     $"{message.From.FirstName} {message.From.LastName} {(command.Status ? "succeeded" : "failed")}");*/
                    });
                }
                Thread.Sleep(200);
            }

        }

    }
}
