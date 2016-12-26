using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hynamick.QaBot
{
    public class RelatedQuestion
    {
        public RelatedQuestion(string question, string command)
        {
            this.Question = question;
            this.Command = command;
        }

        public string Question { get; set; }

        public string Command { get; set; }
    }
}