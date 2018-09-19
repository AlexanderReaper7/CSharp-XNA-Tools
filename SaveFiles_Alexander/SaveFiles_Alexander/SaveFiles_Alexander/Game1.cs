using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

//using Microsoft.Xna.Framework.Storage; TODO

namespace SaveFiles_Alexander
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public readonly string FileName = "save.dat";
        private SpriteFont Font;
        /// <summary>
        /// Current player Score
        /// </summary>
        public int PlayerScore = 30;

        public string PlayerName;
        /// <summary>
        /// boolean used to stop S key spam
        /// </summary>
        bool isSUp;

        /// <summary>
        /// Possible states for game
        /// </summary>
        private enum GameStates
        {
            Scoreboard,
            Menu
        }

        /// <summary>
        /// Current gamestate
        /// </summary>
        private GameStates gameState = GameStates.Menu;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            PlayerName = GetRandomString();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>(@"Font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();
            //back or escape exits the game
            if (gamepad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.End))
                this.Exit();

            //F to toggle fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            switch (gameState)
            {
                case GameStates.Menu:
                {
                    // Up Arrow key to add 1 to score
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        PlayerScore++;
                    }
                    // Down Arrow key to subtract 1 to score
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        PlayerScore--;
                    }
                    // H to open scoreboard 
                    if (Keyboard.GetState().IsKeyDown(Keys.H))
                    {
                        // Set gamestate to scoreboard
                        gameState = GameStates.Scoreboard;
                    }
                    // Check if S is up to enable 
                    if (keyboard.IsKeyUp(Keys.S))
                    {
                        isSUp = true;
                    }
                    // S to save to file
                    if (Keyboard.GetState().IsKeyDown(Keys.S) && isSUp)
                    {

                        isSUp = false;
                        SaveHighScore();
                    }

                    break;
                }
                case GameStates.Scoreboard:
                {
                    // M to open menu
                    if (Keyboard.GetState().IsKeyDown(Keys.M))
                    {
                        // Set gameState to Menu
                        gameState = GameStates.Menu;
                    }

                    break;

                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            base.Update(gameTime);
        }

        [Serializable]
        public struct SaveData
        {
            public string[] PlayerName;
            public int[] Score;

            public int Count;

            public SaveData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];

                Count = count;
            }
        }

        /// <summary>
        /// Opens file and saves data in a XML file
        /// </summary>
        /// <param name="data">data to write</param>
        /// <param name="filename">name of file to write to</param>
        public static void DoSave(SaveData data, string filename)
        {
            // Open or create file
            FileStream stream = File.Open(filename, FileMode.Create);
            try
            {
                // Make to XML and try to open filestream
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close file
                stream.Close();
            }
        }

        public static SaveData LoadData(string FileName)
        {
            SaveData data;

            string fullpath = FileName;

            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                data = (SaveData)serializer.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }

            return data;
        }

        /// <summary>
        /// Public domain method found on the interwebz
        /// </summary>
        /// <returns>Random string of 8 characters</returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveHighScore()
        {
                Console.WriteLine("Saved highscore");
            SaveData data = LoadData(FileName);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (PlayerScore > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                for (int i = 0; i > scoreIndex; i--)
                {
                    data.Score[i] = data.Score[i - 1];
                }

                data.Score[scoreIndex] = PlayerScore;
                data.PlayerName[scoreIndex] = PlayerName;
                DoSave(data, FileName);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameStates.Scoreboard:
                    GraphicsDevice.Clear(Color.Black);

                    for (var index = 0; index < LoadData(FileName).Count; index++)
                    {
                        // Draw Name and Score
                        spriteBatch.DrawString(Font, LoadData(FileName).PlayerName[index] + " " + LoadData(FileName).Score[index], new Vector2(1, Font.MeasureString(LoadData(FileName).PlayerName[index]).Y + 3), Color.White);
                    }

                    break;
                case GameStates.Menu:
                    GraphicsDevice.Clear(Color.HotPink);

                    // Draw Name and Score
                    spriteBatch.DrawString(Font, PlayerName + " " + PlayerScore, Vector2.One, Color.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
