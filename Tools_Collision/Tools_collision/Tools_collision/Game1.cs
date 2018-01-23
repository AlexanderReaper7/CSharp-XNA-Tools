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

namespace Tools_collision
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Ball image
        Texture2D ballSprite;
        //Ball Placement
        Vector2 ballPos = Vector2.Zero;
        //Default speed
        protected const int defBallSpeed = 150;
        //Ball Speed
        Vector2 ballSpeed = new Vector2(150, 150);
        //Player sprite
        Texture2D playerSprite;
        //Player Placement
        Vector2 playePos = new Vector2(0,350);
        //default lives
        int defLives = 3;
        //number of lives this session
        int lives;
        //lives text position
        Vector2 livesTextPos = new Vector2(0,0);
        //score
        int score;
        //total score this sesion
        int totalScore;
        //score text position
        Vector2 scoreTextPos = new Vector2(200, 10);
        //totalScore text position
        Vector2 totalScoreTextPos = new Vector2(450, 10);
        //font
        SpriteFont Arial36;
        //sounds
        SoundEffect ballBounce;
        //cheats
        bool cheatsActive = false;
        //dead?
        bool dead = false;
        //currently pressed?
        bool pressing = false;

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
            int screenmid = GraphicsDevice.Viewport.Width / 2;
            playePos.X = screenmid;
            lives = defLives;
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
            ballSprite = Content.Load<Texture2D>(@"res/Ball");
            playerSprite = Content.Load<Texture2D>(@"res/Player");
            Arial36 = Content.Load<SpriteFont>("res/Arial36");
            livesTextPos = new Vector2(0, 10);
            ballBounce = Content.Load<SoundEffect>(@"res/ballBounce");

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

        protected void Reset()
        {
            //resetball
            ballPos.X = 0;
            ballPos.Y = 0;
            ballSpeed.X = defBallSpeed;
            ballSpeed.Y = defBallSpeed;
            //Resetplayer
            int screenmid = GraphicsDevice.Viewport.Width / 2;
            playePos.X = screenmid;
            //reset current score
            totalScore = totalScore + score;
            score = 0;

        }

        void Die()
        {
            //Stop ball
            ballSpeed.X = 0;
            ballSpeed.Y = 0;

        }

        void Continue()
        {
            Reset();
            dead = false;
            //reset lives to 3
            lives = defLives;
            //clear session score
            totalScore = 0;
        }

        protected override void Update(GameTime gameTime)
        {
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
            // TODO: Add your update logic here

            //move the ball through speed dependent on time
            ballPos += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int maxX = GraphicsDevice.Viewport.Width - ballSprite.Width;
            int maxY = GraphicsDevice.Viewport.Height - ballSprite.Height;

            //controll if the ball should reverse movment
            if (ballPos.X > maxX || ballPos.X < 0)  // collision with horisantal
            {
                ballSpeed.X *= -1;
                ballBounce.Play();
            }

            if (ballPos.Y > maxY || ballPos.Y < 0) //collison with vertical walls
            {
                ballSpeed.Y *= -1;
                ballBounce.Play();
            }
            //if ball hits lower edge,die.
            if(ballPos.Y >= maxY)
            {
                Reset();
                lives--;
                //if no lives are remaining,die
                if (lives <= 0)
                {
                   dead = true;
                   Die();
                }

            }

            //start game again on space
            if (dead && keyboard.IsKeyDown(Keys.Enter))
            {
                Continue();
            }

            //hinders player from leaving screen
            if (playePos.X < 0)
            {
                playePos.X = 0;
            }
            if (playePos.X > maxX)
            {
                playePos.X = maxX;
            }


            //update player position

            if (!dead)
            {
                if (keyboard.IsKeyDown(Keys.Right))
                {
                    playePos.X += 2 * (score + 1);

                }

                else if (keyboard.IsKeyDown(Keys.Left))
                {
                    playePos.X -= 2 * (score + 1);

                } 
            }

            //update player position with controller
            playePos.X += (int)((gamepad.ThumbSticks.Left.X * 3) * 2);

            //controll collision
            Rectangle ballRect = new Rectangle((int)ballPos.X, (int)ballPos.Y, ballSprite.Width, ballSprite.Height);
            Rectangle playerRect = new Rectangle((int)playePos.X, (int)playePos.Y, playerSprite.Width, playerSprite.Height);

            if (ballRect.Intersects(playerRect) && ballSpeed.Y > 0) //collison w/ player
            {
                //give point
                score++;
                //increase ball speed
                ballSpeed.Y = 50 * score;
                if (ballSpeed.X < 0)
                {
                    ballSpeed.X -= 50 / score;
                }
                else
                    ballSpeed.X += 50 * score;

                ballSpeed.Y *= -1;
            }

            //Cheats
            if(keyboard.IsKeyDown(Keys.NumPad1) && keyboard.IsKeyDown(Keys.NumPad2) && keyboard.IsKeyDown(Keys.NumPad2))
            {
                cheatsActive = true;
            }
            if(cheatsActive)
            {
                if (keyboard.IsKeyDown(Keys.O) && !pressing) { lives++; pressing = true; }
                if (keyboard.IsKeyDown(Keys.L) && !pressing) { lives--; pressing = true; }
                if (keyboard.IsKeyDown(Keys.I) && !pressing) { score++; pressing = true; }
                if (keyboard.IsKeyDown(Keys.K) && !pressing) { score--; pressing = true; }
                if (keyboard.IsKeyUp(Keys.O) && keyboard.IsKeyUp(Keys.L) && keyboard.IsKeyUp(Keys.I) && keyboard.IsKeyUp(Keys.K))
                {
                    pressing = false;
                }
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            //Draw the Ball
            spriteBatch.Begin();
            spriteBatch.DrawString(Arial36, "Lives : " + lives.ToString(), livesTextPos, Color.Black);
            spriteBatch.DrawString(Arial36, "Score : " + score.ToString(), scoreTextPos, Color.Black);
            spriteBatch.DrawString(Arial36, "Total Score : " + totalScore.ToString(), totalScoreTextPos, Color.Black);
            spriteBatch.Draw(ballSprite, ballPos, Color.Black);
            spriteBatch.Draw(playerSprite,playePos, Color.Black);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
