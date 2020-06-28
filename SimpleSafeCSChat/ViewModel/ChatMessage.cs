using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCSChat.ViewModel
{

    public class ChatMessage
    {
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public bool IsOriginNative 
        {
            get
            {
                if (Author == "Me")
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            set
            { 
                if(value)
                {
                    Author = "Me";
                }
            }
        }
        public ChatMessage()
        {
            Message = "";
            Author = "Me";
            Time = DateTime.Now;
        }
        public ChatMessage(string message, string author, DateTime time)
        {
            Message = message;
            Author = author;
            Time = time;
        }
    }
}
