using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SimpleSafeCSChat.ViewModel;

namespace SimpleSafeCSChat
{
    //Converts data from inner presentation to outer and vice versa
    class ChatTabsToXmlConverter
    {
        public static ObservableCollection<ChatTab> ParseUserData(string rawStringData)
        {
            ObservableCollection<ChatTab> chatTabs = new ObservableCollection<ChatTab>();
            XDocument xDoc = XDocument.Parse(rawStringData);
            foreach (XElement chatElement in xDoc.Element("Chats").Elements("PersonalChat"))
            {
                XAttribute nameAttribute = chatElement.Attribute("name");
                if (nameAttribute != null)
                {
                    ObservableCollection<ChatMessage> messages = new ObservableCollection<ChatMessage>();
                    ChatTab chatTab = new ChatTab();
                    foreach (XElement messageElement in chatElement.Elements("Message"))
                    {
                        XElement timeElement = messageElement.Element("time");
                        XElement senderElement = messageElement.Element("sender");
                        XElement textElement = messageElement.Element("text");
                        if (nameAttribute != null && timeElement != null && senderElement != null && textElement != null)
                        {

                            messages.Add(new ChatMessage
                            {
                                Time = DateTime.Parse(timeElement.Value),
                                Author = senderElement.Value,
                                Message = textElement.Value
                            });
                        }
                    }
                    chatTab.Name = nameAttribute.Value;
                    chatTab.ChatMessages = messages;
                    chatTab.chatType = ChatInstanceType.personal;
                    chatTabs.Add(chatTab);
                }
            }
            foreach (XElement chatElement in xDoc.Element("Chats").Elements("GroupChat"))
            {
                XAttribute nameAttribute = chatElement.Attribute("name");
                XAttribute passwordAttribute = chatElement.Attribute("password");
                if (nameAttribute != null && passwordAttribute != null)
                {
                    ChatTab chatTab = new ChatTab();
                    chatTab.Name = nameAttribute.Value;
                    chatTab.Password = passwordAttribute.Value;
                    chatTab.chatType = ChatInstanceType.group;
                    chatTabs.Add(chatTab);
                }
            }
            return chatTabs;
        }

        public static string GetPlainTextUserData(ref ObservableCollection<ChatTab> chatTabs)
        {
            XDocument xdoc = new XDocument();
            XElement xChats = new XElement("Chats");
            foreach (ChatTab element in chatTabs)
            {
                XElement xChat = new XElement("Chat", new XAttribute("name", element.Name));
                foreach (ChatMessage message in element.ChatMessages)
                { 
                    xChat.Add(
                    new XElement("Message", 
                    new XElement("time", message.Time.ToString()),
                    new XElement("sender", message.Author),
                    new XElement("text", message.Message)
                    ));
                }
                xChats.Add(xChat);
            }
            foreach (ChatTab element in chatTabs)
            {
                xChats.Add(
                    new XElement(
                        "GroupChat", 
                        new XAttribute("name", element.Name), 
                        new XAttribute("password", element.Password)
                        ));
            }
            xdoc.Add(xChats);
            return xdoc.ToString();
        }
    }
}
