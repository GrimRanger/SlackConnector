﻿using SlackConnector.Connections.Clients.Api.Helpers;

namespace SlackConnector.Connections.Clients.Api.Responces.List
{
    [RequestPath("groups.list")]
    internal class GroupListResponse : ApiResponce
    {
        public Models.Group[] Groups { get; set; }
    }
}