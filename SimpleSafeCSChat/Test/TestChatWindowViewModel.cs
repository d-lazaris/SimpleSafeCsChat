using SimpleSafeCSChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleSafeCSChat.Test
{

    public class TestChatWindowViewModel : ChatScreenViewModel
    {
        public TestChatWindowViewModel()
        {
            AddMessage("Literius", new ChatMessage
            {
                Author = "Literius",
                Message = "Ab altĕro expectes, altĕri quod fecĕris. Amīcus certus in re incertā cernĭtur. Amīcus Plato, sed magis amīca verĭtas. \r\n Ave Caesar, moritūri te salūtant.",
                Time = DateTime.Now,
                IsOriginNative = false
            });
            AddMessage("Literius", new ChatMessage
            {
                Author = "Me",
                Message = "Cetĕrum censeo Carthagĭnem esse delendam.",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            AddMessage("Gentleman-In-Hat", new ChatMessage
            {
                Author = "Gentleman-In-Hat",
                Message = "It's tea time.",
                Time = DateTime.Now,
                IsOriginNative = false
            });
            AddMessage("Gentleman-In-Hat", new ChatMessage
            {
                Author = "Me",
                Message = "Sure.",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            SelectedChatTab = ChatTabs.First();
        }
    public TestChatWindowViewModel(ChatManager cm)
        {
            chatManager = cm;
            AddMessage("Literius", new ChatMessage
            {
                Author = "Literius",
                Message = "Ab altĕro expectes, altĕri quod fecĕris. Amīcus certus in re incertā cernĭtur. Amīcus Plato, sed magis amīca verĭtas. \r\n Ave Caesar, moritūri te salūtant.",
                Time = DateTime.Now,
                IsOriginNative = false
            });
            AddMessage("Literius", new ChatMessage
            {
                Author = "Me",
                Message = "Cetĕrum censeo Carthagĭnem esse delendam.",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            AddMessage("Gentleman-In-Hat", new ChatMessage
            {
                Author = "Gentleman-In-Hat",
                Message = "It's tea time.",
                Time = DateTime.Now,
                IsOriginNative = false
            });
            AddMessage("Gentleman-In-Hat", new ChatMessage
            {
                Author = "Me",
                Message = "Sure.",
                Time = DateTime.Now,
                IsOriginNative = true
            });
            SelectedChatTab = ChatTabs.First();
        }
    }
}
