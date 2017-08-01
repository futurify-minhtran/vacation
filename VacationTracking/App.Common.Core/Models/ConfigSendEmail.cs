using System;
using System.Collections.Generic;
using System.Text;

namespace App.Common.Core.Models
{
    public class ConfigSendEmail
    {
        public string Sender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }

        public string Receiver { get; set; }

        public ConfigSendEmail()
        {

        }

        public ConfigSendEmail(ConfigSendEmail config)
        {
            this.Sender = config.Sender;
            this.Username = config.Username;
            this.Password = config.Password;
            this.Host = config.Host;
            this.Port = config.Port;
        }
    }
}
