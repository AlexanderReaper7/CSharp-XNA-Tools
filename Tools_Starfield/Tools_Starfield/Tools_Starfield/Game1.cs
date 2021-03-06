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

namespace Tools_Starfield
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Starfield starfield;
        PlayerManager playerSprite;
        Texture2D mixedSprites;
        EnemyManager enemyManager;
        CollisionsManager collisionManager;
        ExplosionManager explosionManager;

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
            // Star texture(alias)
            mixedSprites = Content.Load<Texture2D>(@"mixed");
            // Window size
            Rectangle screenBounds = new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            // Star particles
            starfield = new Starfield(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, 200, new Vector2(0, 30f), mixedSprites, new Rectangle(0, 48, 2, 2));
            // Player Sprite
            playerSprite = new PlayerManager(Content.Load<Texture2D>(@"SpriteSheet"), 1, 32, 48, screenBounds);
            // Player starting position
            playerSprite.Position = new Vector2(400, 300);
            // Enemy sprite
            enemyManager = new EnemyManager(mixedSprites, new Rectangle(0, 200, 50, 50), 6, playerSprite, screenBounds);
            // Collisions
            collisionManager = new CollisionsManager(playerSprite, enemyManager, explosionManager);
            // Explosion particle sprite
            explosionManager = new ExplosionManager(mixedSprites, new Rectangle(0, 100, 50, 50), 3, new Rectangle(0, 450, 2, 2));
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
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();
            // Back or escape exits the game
            if (gamepad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.End))
            {
                this.Exit();
            }
            // Starfield
            starfield.Update(gameTime);
            // Player movement
            playerSprite.HandleSpriteMovement(gameTime);
            playerSprite.update(gameTime);
            // Enemy
            enemyManager.Update(gameTime);
            // Hitbox
            collisionManager.CheckCollisions();
            explosionManager.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            starfield.Draw(spriteBatch);
            playerSprite.Draw(spriteBatch);
            enemyManager.draw(spriteBatch);
            explosionManager.draw(spriteBatch);
            spriteBatch.Draw(playerSprite.Texture, playerSprite.Position, playerSprite.SourceRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
