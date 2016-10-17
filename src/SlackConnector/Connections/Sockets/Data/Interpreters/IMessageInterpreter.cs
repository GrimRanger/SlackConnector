using SlackConnector.Connections.Sockets.Data.Inbound;

namespace SlackConnector.Connections.Sockets.Data.Interpreters
{
    internal interface IMessageInterpreter
    {
        IInboundMessage InterpretMessage(string json);
    }
}