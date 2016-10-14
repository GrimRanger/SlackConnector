namespace SlackConnector.Connections.Sockets.Messages.Inbound
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