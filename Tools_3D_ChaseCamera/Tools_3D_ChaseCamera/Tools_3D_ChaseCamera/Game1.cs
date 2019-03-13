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

namespace Tools_3D_ChaseCamera
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
            Model groundModel = Content.Load<Model>(@"Ground/Ground");

            models.Add( new CustomModel(
                boxModel,
                Vector3.Zero, 
                Vector3.Zero, 
                new Vector3(100f),
                GraphicsDevice));

            models.Add(new CustomModel(groundModel, Vector3.Zero, Vector3.Zero, Vector3.One, GraphicsDevice));

            camera = new ChaseCamera(new Vector3(1000f), Vector3.Zero, Vector3.Zero, GraphicsDevice);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.End))
                this.Exit();

            UpdateModel(gameTime);
            UpdateCamera(gameTime);

            base.Update(gameTime);
        }

        private void UpdateModel(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            Vector3 rotChange = Vector3.Zero;

            // Rotate object
            if (keyState.IsKeyDown(Keys.Q)) { rotChange += new Vector3(1, 0, 0); }
            if (keyState.IsKeyDown(Keys.E)) { rotChange += new Vector3(-1, 0, 0); }
            if (keyState.IsKeyDown(Keys.A)) { rotChange += new Vector3(0, 1, 0); }
            if (keyState.IsKeyDown(Keys.D)) { rotChange += new Vector3(0, -1, 0); }

            models[0].Rotation += rotChange * .025f;

            // Object should only move when space is held down
            if (keyState.IsKeyUp(Keys.S))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X, models[0].Rotation.Z);
                // Move object in direction given by rotation matrix
                models[0].Position += Vector3.Transform(Vector3.Forward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);
            }

            if (keyState.IsKeyUp(Keys.W))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X, models[0].Rotation.Z);
                // Move object in direction given by rotation matrix
                models[0].Position += Vector3.Transform(Vector3.Backward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);

            }
        }

        private void UpdateCamera(GameTime gameTime)
        {
            ((ChaseCamera)camera).Move(models[0].Position, models[0].Rotation);

            camera.Update();
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Create view and projection matrix TODO: move these to load content
            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(100, 300, 600), new Vector3(0, 50, 0), Vector3.Up);
            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000.0f);

            // Draw every model in models with a view and projection matrix
            foreach (CustomModel model in models)
            {
                model.Draw(camera.View, camera.Projection);
            }

            
            base.Draw(gameTime);
        }
    }
}
