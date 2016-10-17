namespace SlackConnector.Connections.Sockets.Data.Inbound
{
    internal enum MessageType
    {
        Unknown = 0,
        Message,
        mpim_joined,
        group_joined,
        channel_joined,
    }
}