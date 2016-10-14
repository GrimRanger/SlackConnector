namespace SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal interface IMessageInterpreter
    {
        InboundData InterpretMessage(string json);
    }
}