using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversationApp.Models
{
    public class Message
    {
        public string Id { get; set; }

        public string Sender { get; set; }

        public string Messagetext { get; set; }

        public string UserImageUrl { get; set; }
    }
}
