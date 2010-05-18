using System.Media;
using System.Reflection;
using System.Xml;
using MocsMessageInterceptor;
using System.Collections.Generic;
using System;

namespace MocSampleMessageInterceptors
{
    public class PlaySoundMessageInterceptor : IMessageInterceptor
    {
        private Random _random = new Random((int)DateTime.Now.Ticks);

        private List<string> _succesList = new List<string>
        {            
            "MocsSampleMessageInterceptors.Success.alweereenwinnaar.wav",
            "MocsSampleMessageInterceptors.Success.applause2.wav",
            "MocsSampleMessageInterceptors.Success.josbrink.wav", 
            "MocsSampleMessageInterceptors.Success.jostiband.wav", 
            "MocsSampleMessageInterceptors.Success.kikmgoan.wav", 
            "MocsSampleMessageInterceptors.Success.tarzanyell.wav",       
            "MocsSampleMessageInterceptors.Success.wearethechampions.wav",       
            "MocsSampleMessageInterceptors.Success.wearethegreatest.wav",
            "MocsSampleMessageInterceptors.Success.yoursmile.wav",
            "MocsSampleMessageInterceptors.Success.business.wav",
            "MocsSampleMessageInterceptors.Success.lets_rock.wav"
        };

        private List<string> _errorList = new List<string>
        {            
            "MocsSampleMessageInterceptors.Error.allesoptilt.wav",
            "MocsSampleMessageInterceptors.Error.allwrong.wav",
            "MocsSampleMessageInterceptors.Error.drama.wav",       
            "MocsSampleMessageInterceptors.Error.gloeiende.wav",       
            "MocsSampleMessageInterceptors.Error.hadofnie.wav",       
            "MocsSampleMessageInterceptors.Error.nooo.wav",       
            "MocsSampleMessageInterceptors.Error.ouch.wav",
            "MocsSampleMessageInterceptors.Error.politiewagen.wav",
            "MocsSampleMessageInterceptors.Error.sirene.wav",
            "MocsSampleMessageInterceptors.Error.smash.wav",
            "MocsSampleMessageInterceptors.Error.out_of_gum_x.wav",
            "MocsSampleMessageInterceptors.Error.back_2_work_y.wav",
            "MocsSampleMessageInterceptors.Error.mess.wav"
        };

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
            try
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
                            case "firstplace":
                                soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_succesList[_random.Next(0, _succesList.Count)]);
                                soundPlayer.Play();
                                break;
                        }
                        break;
                    case "error":
                        soundPlayer.Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_errorList[_random.Next(0, _errorList.Count)]);
                        soundPlayer.Play();
                        break;

                }
            }
            catch
            { }
        }
    }
}
