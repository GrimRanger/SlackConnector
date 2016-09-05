﻿using System.Collections.Generic;

namespace SlackConnector.Models
{
    public class SlackUser
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string FormattedUserId
        {
            get
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    return "<@" + Id + ">";
                }
                return string.Empty;
            }
        }

        public Dictionary<string, string> Icons { get; set; }

        //public bool IsSlackbot
        //{
        //    get { return Id == "USLACKBOT"; }
        //}
    }
}