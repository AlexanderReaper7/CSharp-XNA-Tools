using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tool_Network
{
    public static class NetworkTest
    {
        private static SpriteFont _font;

        // Textures for network icon
        private static Texture2D _networkNullIcon;
        private static Texture2D _networkOnIcon;
        private static Texture2D _networkOffIcon;

        private static bool? _isConnected;
        private static bool? _successfullySentMail;

        public static void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>(@"Font");

            _networkNullIcon = content.Load<Texture2D>(@"NetworkNull");
            _networkOnIcon = content.Load<Texture2D>(@"NetworkOn");
            _networkOffIcon = content.Load<Texture2D>(@"NetworkOff");
        }

        public static void Update()
        {
            // Press space to get network connectivity status
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _isConnected = NetworkInterface.GetIsNetworkAvailable();               
            }
            // Press Enter to send test mail if message was not to sent yet
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && _successfullySentMail == null)
            {
                _successfullySentMail = SendTestMail();
            }
        }

        /// <summary>
        /// Sends a test mail to Stativetroller7@gmail.com from alexander.ohberg.a1@gmail.com
        /// </summary>
        /// <returns></returns>'
        private static bool SendTestMail()
        {
            try
            {
                // Create smtp client
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Credentials = new NetworkCredential("alexander.ohberg.a1@gmail.com", ""),
                    EnableSsl = true
                };
                // Create message
                MailMessage message = new MailMessage("alexander.ohberg.a1@gmail.com", "stativetroller7@gmail.com")
                {
                    Subject = "Test", Body = "This is a test mail"
                };
                // Send message with smtp client
                client.Send(message);
            }
            catch (Exception e)
            {
                // Write error to console
                Console.WriteLine(e);
                return false;
            }
            // If no error was catch-ed, return true (success)
            return true;
        }

        /// <summary>
        /// Draws network elements
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // Draw tool tip for network connection check
            spriteBatch.DrawString(_font,
                "Press space to check for a network connection",
                new Vector2(200f),
                Color.White);
            // Draw connection icon
            spriteBatch.Draw(_isConnected == null ? _networkNullIcon : _isConnected == true ? _networkOnIcon : _networkOffIcon,
                Vector2.One,
                Color.White);
            // Draw 
            spriteBatch.DrawString(_font,
                _successfullySentMail == null ? "Press Enter to send test mail" : _successfullySentMail == true ? "Mail successfully sent!" : "Error when attempting to send mail, check console for error.",
                new Vector2(1, 100),
                Color.White);

            spriteBatch.End();
        }
    }
}
