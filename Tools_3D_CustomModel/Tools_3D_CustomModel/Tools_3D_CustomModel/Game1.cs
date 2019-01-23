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
using Microsoft.Xna.Framework.Graphics;

namespace Tools_3D_CustomModel
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private List<CustomModel> models = new List<CustomModel>();
        private Camera camera;

        private MouseState lastMouseState;

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
            Model boxModel = Content.Load<Model>(@"test");

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    Vector3 position = new Vector3(-200 + x * 200, -200 + y * 300, 0);

                    models.Add( new CustomModel(
                        boxModel,
                        position,
                        new Vector3(0, MathHelper.ToRadians(90) * (y * 3 + x), 0),
                        new Vector3(10f),
                        GraphicsDevice));


                }
            }

            models.Add(new CustomModel(boxModel, Vector3.Zero, Vector3.Zero, new Vector3(100f), GraphicsDevice));

            camera = new ArcBallCamera(Vector3.Zero, 0, 0, 0, MathHelper.PiOver2, 1200, 1000, 2000, GraphicsDevice);

            lastMouseState = Mouse.GetState();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateCamera(gameTime);

            base.Update(gameTime);
        }

        public void UpdateCamera(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            // Calculate how much the camera should rotate
            float deltaX = lastMouseState.X - mouseState.X;
            float deltaY = lastMouseState.Y - mouseState.Y;

            // Rotate camera
            ((ArcBallCamera)camera).Rotate(deltaX * 0.01f, deltaY * 0.01f);

            // Calculate scroll wheel 
            float scrollDelta = lastMouseState.ScrollWheelValue - (float) mouseState.ScrollWheelValue;

            // Move camera
            ((ArcBallCamera)camera).Move(scrollDelta);

            // Update camera
            camera.Update();

            // Update lastMouseState
            lastMouseState = mouseState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(100, 300, 600), new Vector3(0, 50, 0), Vector3.Up);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f);

            foreach (CustomModel model in models)
            {
                model.Draw(camera.View, camera.Projection);
            }

            
            base.Draw(gameTime);
        }
    }
}
