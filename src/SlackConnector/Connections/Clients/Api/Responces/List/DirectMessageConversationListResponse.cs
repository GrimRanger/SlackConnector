﻿using SlackConnector.Connections.Clients.Api.Helpers;
using SlackConnector.Connections.Models;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("im.list")]
    internal class DirectMessageConversationListResponse : ApiResponceWithoutParametres
    {
        public Im[] Ims { get; set; }
    }
}
