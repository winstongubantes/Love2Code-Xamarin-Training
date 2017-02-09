using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ConversationApp.Models;
using ConversationApp.Services;
using Xamarin.Forms;

namespace ConversationApp.ViewModels
{
    public class ConversationPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Message> _messages;
        private string _userName;
        private string _textMessage;
        private ICommand _sendMessageCommand;
        private AzureDataService _azureDataService = new AzureDataService();

        public event PropertyChangedEventHandler PropertyChanged;

        public ConversationPageViewModel()
        {
            Messages = new ObservableCollection<Message>();

            //Messages.Add(new Message {Messagetext = "Hello", Sender = "Sender", UserImageUrl = "me_user.png"});

            InitializeMessage();
        }

        private async void InitializeMessage()
        {
            var message = await _azureDataService.GetMessages();

            foreach (var message1 in message)
            {
                Messages.Add(message1);
            }
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value; 
                OnPropertyChanged("Messages");
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string TextMessage
        {
            get { return _textMessage; }
            set
            {
                _textMessage = value;
                OnPropertyChanged("TextMessage");
            }
        }

        public ICommand SendMessageCommand
        {
            get { return _sendMessageCommand = _sendMessageCommand ?? new Command(async () =>
            {
                var msg = new Message
                {
                    Messagetext = TextMessage,
                    Sender = UserName,
                    UserImageUrl = "me_user.png"
                };

                Messages.Add(msg);

                await _azureDataService.AddMessage(msg);
            }); }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
