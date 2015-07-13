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

namespace testBot1
{
    public class TeleGetMeResponse
    {
        public bool ok;
        public TeleUser result;
    }

    public class TeleGetUpdatesResponse
    {
        public bool ok;
        public TeleUpdate[] result;
    }

    public class TeleSendMessageResponse
    {
        public bool ok;
        public TeleMessage result;
    }

    public class TeleUser
    {
        public int id;
        public string first_name;
        public string last_name;
        public string username;
    }

    public class TeleMessage
    {
        public int message_id;
        public TeleUser from;
        public int date;
        public TeleUser chat;
        public string text;
    }

    public class TeleUpdate
    {
        public int update_id;
        public TeleMessage message;
    }


    
    class Program
    {

        WebClient wc = new WebClient();
        int user_id = 116815836;

        static void Main(string[] args)
        {
            Program prog = new Program();
            
            //Console.Write("Hello ");

            //prog.teleGetUpdates();
            prog.teleSendMessage();

            //teleRequest();
            Console.ReadKey();
        }

        void teleGetMe()
        {
            string request = "https://api.telegram.org/bot93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY/getMe";

            string json = wc.DownloadString(request);

            dynamic tu = new JObject();
            tu = JsonConvert.DeserializeObject<TeleGetMeResponse>(json);
        }

        void teleGetUpdates()
        {
            string request = "https://api.telegram.org/bot93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY/getUpdates";

            string json = wc.DownloadString(request);

            var tResponse = JsonConvert.DeserializeObject<TeleGetUpdatesResponse>(json);
            
            foreach(TeleUpdate tResult in tResponse.result)
            {
                Console.WriteLine(tResult.message.text);
            }
        }

        void teleSendMessage()
        {
            string chat_id = Uri.EscapeDataString(user_id.ToString());
            string text = Uri.EscapeDataString("Hello!");
            string request = "https://api.telegram.org/bot93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY/sendMessage?chat_id="+chat_id+"&text="+text;
            
            string json = wc.DownloadString(request);
            //dynamic tu = new JObject();
            //tu = JsonConvert.DeserializeObject<TeleResponse>(json);
        }

        static void teleRequest()
        {

            // Get URL and proxy
            // from the text boxes.
            string getMe = "https://api.telegram.org/bot93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY/getMe";
            string getUpdates = "https://api.telegram.org/bot93529147:AAF5qsEr-4s2nyxKeVIjYROrIL0MA8zxyEY/getUpdates";
            //string proxy = txtProxy.Text;

            try
            {

                //Code here VVVVV

                WebClient wc = new WebClient();

                String json = wc.DownloadString(getUpdates);

                //TeleResponse tu = JsonConvert.DeserializeObject<TeleResponse>(json);

                dynamic tu = new JObject();
                tu = JsonConvert.DeserializeObject(json);
                

                foreach (TeleUpdate tuResult in tu.result)
                {

                        Console.WriteLine(tuResult.message.text);

                }
                

            }
            catch (WebException ex)
            {
                string message = ex.Message;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (null != response)
                {
                    message = response.StatusDescription;
                    response.Close();
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

        }


    }
}
