namespace MocsMessageInterceptor
{
    /// <summary>
    /// <Message>
    ///     <MessageType>Info</MessageType>            
    ///     <DateTime>20:12:00</DateTime>
    ///     <TeamId></TeamId>
    ///     <Category>Test</Category>
    ///     <Text>Dit is een bericht</Text>
    ///     <Forward>0</Forward>
    ///     <Synchronized>0</Synchronized>
    /// </Message>            
    /// </summary>
    public interface IMessageInterceptor
    {
        void ProcessMessage(string message);
    }
}
