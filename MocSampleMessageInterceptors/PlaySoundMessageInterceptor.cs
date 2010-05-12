using System.Media;
using System.Reflection;
using System.Xml;
using MocsMessageInterceptor;

namespace MocSampleMessageInterceptors
{
    public class PlaySoundMessageInterceptor : IMessageInterceptor
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
        public void ProcessMessage(string message)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(message);
            string messageType = xmlDocument.SelectSingleNode("Message/MessageType").InnerText;
            string category = xmlDocument.SelectSingleNode("Message/Category").InnerText;

            SoundPlayer soundPlayer = new SoundPlayer();

            switch (messageType.ToLower())
            {
                case "info":
                    switch (category.ToLower())
                    {
                        case "success":
                            soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.tarzanyell.wav");
                            soundPlayer.Play();
                            break;
                        case "submitted":
                            break;
                        case "processing":
                            break;
                    }
                    break;
                case "error":
                    switch (category.ToLower())
                    {
                        case "errorcompilation":
                            soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.smash.wav");
                            soundPlayer.Play();
                            break;
                        case "errorvalidation":
                            soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.allwrong.wav");
                            soundPlayer.Play();
                            break;
                        case "errortesting":
                            soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.sirene.wav");
                            soundPlayer.Play();
                            break;
                        case "errorserver":
                             soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.gloeiende.wav");
                            soundPlayer.Play();
                            break;
                        case "errorunknown":
                             soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MocSampleMessageInterceptors.ouch.wav");
                            soundPlayer.Play();
                            break;
                    }
                    break;
            }        
        }
    }
}
