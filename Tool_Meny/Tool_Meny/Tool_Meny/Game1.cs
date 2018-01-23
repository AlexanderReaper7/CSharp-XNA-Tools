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

namespace Tool_Meny
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //---Variables---___________________________________________________________________________________________________
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //--Assets--_______________________________________________________________________________________________
        Texture2D logoImage;
        Texture2D backgroundImage;

        //__________________________________________________________________________________________________________

        //--Positions--__________________________________________________________________________________________
        Vector2 logoPos = new Vector2(1920 / 2 - 512 / 2,0);



        //_______________________________________________________________________________________________________

        //GameStates_____________________________________________________________________________________
        enum GameStates
        {
            TitleScreen, Playing, Gameover
        };

        //starting gamestate
        GameStates gameState = GameStates.TitleScreen;
        //_______________________________________________________________________________________________



        //_________________________________________________________________________________________________________________

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;



        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            logoImage = Content.Load<Texture2D>(@"Images/logoImage");
            backgroundImage = Content.Load<Texture2D>(@"Images/backgroundImage");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region Game Exit
            // Allows the game to exit
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();
            //back or escape exits the game

            if (gamepad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            //F to toggle fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }
            #endregion
            #region Game States
            switch (gameState)
            {
                case GameStates.TitleScreen:
                    //nextscreen on A
                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        gameState = GameStates.Playing;
                    }

                    break;

                case GameStates.Playing:
                    //next screen on S
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        gameState = GameStates.Gameover;
                    }

                    break;

                case GameStates.Gameover:
                    //next screen on D
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        gameState = GameStates.TitleScreen;
                    }

                    break;

                    #endregion
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            //---TitleScreen--________________________________________________________________________________
            if (gameState == GameStates.TitleScreen)
            {
                GraphicsDevice.Clear(Color.White);
                spriteBatch.Draw(backgroundImage, new Rectangle(0,0,this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                spriteBatch.Draw(logoImage, logoPos, Color.White);
            }
            //________________________________________________________________________________________________

            //---Playing--____________________________________________________________________________________
            if (gameState == GameStates.Playing)
            {
                GraphicsDevice.Clear(Color.Gray);
            }
            //________________________________________________________________________________________________

            //---GameOver--___________________________________________________________________________________
            if (gameState == GameStates.Gameover)
            {
                GraphicsDevice.Clear(Color.Black);
            }
            //________________________________________________________________________________________________

            spriteBatch.End();





            base.Draw(gameTime);
        }
    }
}
