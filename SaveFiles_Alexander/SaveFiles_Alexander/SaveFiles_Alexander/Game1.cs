using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Current player Score
        /// </summary>
        public int PlayerScore = 30;

        public Tuple<string, int>[] Scorelist = { };


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

            // TODO: use this.Content to load your game content here
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

            // if gamestate is Menu
            if (gameState == GameStates.Menu)
            {
                // Gamestate is menu

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

                    // Load data from file
                    SaveData data = LoadData(FileName);

                    // Move data into an array of tuples
                    for (int i = 0; i < data.Count; i++)
                    {
                        Scorelist[i] = new Tuple<string, int>(data.PlayerName[i], data.Score[i]);
                    }

                    Scorelist = HighScoreSort(Scorelist);

                    Console.WriteLine(Scorelist.ToString());
                }

                // S to save to file
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    if (!File.Exists(FileName))
                    {
                        SaveData data = new SaveData(1);
                        data.PlayerName[0] = "Kalle";
                        data.Score[0] = 0;

                        DoSave(data, FileName);
                    }
                }
            }
            else
            {
                // Gamestate is Scoreboard
                // M to open menu
                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    // Set gameState to Menu
                    gameState = GameStates.Menu;
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

        private void SaveHighScore()
        {
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

               
            }
        }

        /// <summary>
        /// HighScoreSort method 
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns></returns>
        static Tuple<string, int>[] HighScoreSort(Tuple<string, int>[] inputArray)
        {
            for (int i = 0; i < inputArray.Length - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (inputArray[j - 1].Item2 > inputArray[j].Item2)
                    {
                        Tuple<string,int> temp = inputArray[j - 1];
                        inputArray[j - 1] = inputArray[j];
                        inputArray[j] = temp;
                    }
                }
            }
            return inputArray;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
