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

namespace AlexanderTest01
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics; 
        SpriteBatch spriteBatch; 
        Random rand = new Random(); // declares a new random var
        Texture2D squareTexture; // declares a Texture: square texture 
        Rectangle currentSquare; // declares a rectangle called currentsquare
        int playerScore = 0; // declares an integer playerscore
        float timeRemaining = 0.0f; //declares the timeRemaining float 
        const float timePerSquare = 0.75f; //time between popup
        Color[] colors = new Color[3] // array of colors to shift from
        {
            Color.Red,
            Color.Green,
            Color.Blue
        };

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
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true; //Enables Mouse
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
            squareTexture = Content.Load<Texture2D>(@"Textures/SQUARE"); //Loads the square
                // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();
            //back or escape exits the game
            if (gamepad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            

            // TODO: Add your update logic here

            base.Update(gameTime);
            if (timeRemaining == 0.0f) // cheecks if time has run out and then creates new square
            {
                currentSquare = new Rectangle(
                    rand.Next(0, this.Window.ClientBounds.Width - 25),
                    rand.Next(0, this.Window.ClientBounds.Height - 25),
                    25, 25);
                timeRemaining = timePerSquare;
            }

            MouseState mouse = Mouse.GetState();
            if ((mouse.LeftButton == ButtonState.Pressed) && (currentSquare.Contains(mouse.X, mouse.Y))) // Checks if mouse is on square
            {
                playerScore++; // adds score
                timeRemaining = 0.0f; // skips to next square
            }
            timeRemaining = MathHelper.Max(0, timeRemaining - (float)gameTime.ElapsedGameTime.TotalSeconds);
            this.Window.Title = "score : " + playerScore.ToString(); // displays score
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(); // clears Square
            spriteBatch.Draw(
                squareTexture,
                currentSquare,
                colors[playerScore % 3]); //chages between colors
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
