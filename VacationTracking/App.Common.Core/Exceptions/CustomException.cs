using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace App.Common.Core.Exceptions
{
    public class CustomException : Exception
    {
        public string Code { get; set; }

        public List<string> Codes { get; set; }

        public CustomException(string code, string message = null) : base(message)
        {
            this.Code = code;
        }

        public CustomException(List<string> codes) : base()
        {
            this.Codes = codes;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                Code = this.Code,
                Codes = this.Codes,
                Message = this.Message
            });
        }
    }
}
