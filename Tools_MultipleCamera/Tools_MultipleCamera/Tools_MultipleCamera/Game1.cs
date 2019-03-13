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

namespace Tools_MultipleCamera
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Contains all custom models
        /// </summary>
        private List<CustomModel> models = new List<CustomModel>();

        private ArcBallCamera _arcBallCamera;
        private ChaseCamera _chaseCamera;
        private FreeCamera _freeCamera;

        private MouseState _lastMouseState;

        private enum Cameras
        {
            ArcBall,
            Chase,
            Free
        }

        /// <summary>
        /// Currently active camera
        /// </summary>
        private Cameras _selectedCamera = Cameras.Chase;

        /// <summary>
        /// returns currently selected camera based on _selectedCamera
        /// </summary>
        public Camera CurrentCamera
        {
            get
            {
                switch (_selectedCamera)
                {
                    case Cameras.ArcBall:
                        return _arcBallCamera;
                    case Cameras.Chase:
                        return _chaseCamera;
                    case Cameras.Free:
                        return _freeCamera;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load models
            Model boxModel = Content.Load<Model>(@"test");
            Model groundModel = Content.Load<Model>(@"Ground/Ground");

            // Add new CustomModels to models List
            // Box
            models.Add( new CustomModel(
                boxModel,
                Vector3.Zero, 
                Vector3.Zero, 
                new Vector3(100f),
                GraphicsDevice));
            // Ground
            models.Add(new CustomModel(groundModel, Vector3.Zero, Vector3.Zero, Vector3.One, GraphicsDevice));


            // Load cameras
            _chaseCamera = new ChaseCamera(new Vector3(1000f), Vector3.Zero, Vector3.Zero, GraphicsDevice);
            _arcBallCamera = new ArcBallCamera(Vector3.Zero, 0, 0, 0, MathHelper.PiOver2, 1200, 1000, 2000, GraphicsDevice);
            _freeCamera = new FreeCamera(GraphicsDevice, 0f, 0f, Vector3.Zero);


            // Initialize _lastMouseState with current mouse state
            _lastMouseState = Mouse.GetState();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
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

            // Update selected 
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) _selectedCamera = Cameras.Chase;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) _selectedCamera = Cameras.Free;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) _selectedCamera = Cameras.ArcBall;

            // Update the selected camera´s relevant methods
            switch (_selectedCamera)
            {
                case Cameras.ArcBall:
                    UpdateModel(gameTime);
                    UpdateArcBallCamera();
                    break;
                case Cameras.Chase:
                    UpdateModel(gameTime);
                    UpdateChaseCamera();
                    break;
                case Cameras.Free:
                    UpdateFreeCamera(gameTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates model movement
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateModel(GameTime gameTime)
        {
            // Get keyboard state
            KeyboardState keyState = Keyboard.GetState();

            // Reset rotation delta
            Vector3 rotChange = Vector3.Zero;

            // Rotate object
            if (keyState.IsKeyDown(Keys.Q)) { rotChange += Vector3.Right; }
            if (keyState.IsKeyDown(Keys.E)) { rotChange += Vector3.Left; }
            if (keyState.IsKeyDown(Keys.A)) { rotChange += Vector3.Up; }  
            if (keyState.IsKeyDown(Keys.D)) { rotChange += Vector3.Down; }
            // Set new rotation
            models[0].Rotation += rotChange * .025f;

            // Move back on S
            if (keyState.IsKeyUp(Keys.S))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X, models[0].Rotation.Z);
                // Move object in direction given by rotation matrix
                models[0].Position += Vector3.Transform(Vector3.Forward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);
            }

            // Move forward on W
            if (keyState.IsKeyUp(Keys.W))
            {
                // Calculate what direction the object should move in
                Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X, models[0].Rotation.Z);
                // Move object in direction given by rotation matrix
                models[0].Position += Vector3.Transform(Vector3.Backward, rotation * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4);
            }
        }

        /// <summary>
        /// Updates Free camera movement
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFreeCamera(GameTime gameTime)
        {
            // Get mouse and keyboard state
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            // Calculate how much the camera should rotate
            float deltaX = _lastMouseState.X - mouseState.X;
            float deltaY = _lastMouseState.Y - mouseState.Y;

            // Rotate camera
            _freeCamera.Rotate(deltaX * 0.01f, deltaY * 0.01f);

            Vector3 translation = Vector3.Zero;
            // Get camera movement
            if (keyState.IsKeyDown(Keys.W)) translation += Vector3.Forward * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.S)) translation += Vector3.Backward * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.A)) translation += Vector3.Left * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.D)) translation += Vector3.Right * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.Space)) translation += Vector3.Up * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (keyState.IsKeyDown(Keys.LeftShift)) translation += Vector3.Down * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Move camera
            _freeCamera.Move(translation);

            // Update camera
            _freeCamera.Update();

            // Update lastMouseState
            _lastMouseState = mouseState;
        }

        /// <summary>
        /// Updates chase camera
        /// </summary>
        private void UpdateChaseCamera()
        {
            // Move camera position and rotation relative to box 
            _chaseCamera.Move(models[0].Position, models[0].Rotation);

            // Update camera
            _chaseCamera.Update();
        }

        /// <summary>
        /// Updates ArcBall camera movement
        /// </summary>
        public void UpdateArcBallCamera()
        {
            // Get mouse state
            MouseState mouseState = Mouse.GetState();

            // Calculate how much the camera should rotate
            float deltaX = _lastMouseState.X - mouseState.X;
            float deltaY = _lastMouseState.Y - mouseState.Y;

            // Rotate camera
            _arcBallCamera.Rotate(deltaX * 0.01f, deltaY * 0.01f);

            // Calculate scroll wheel delta from previous mouse state
            float scrollDelta = _lastMouseState.ScrollWheelValue - (float)mouseState.ScrollWheelValue;

            // Move camera
            _arcBallCamera.Move(scrollDelta);

            // Move cameras target to box´s position
            _arcBallCamera.Target = models[0].Position;

            // Update camera
            _arcBallCamera.Update();

            // Update lastMouseState to 
            _lastMouseState = mouseState;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw every model in models with the current camera´s view and projection matrix
            foreach (CustomModel model in models)
            {
                model.Draw(CurrentCamera.View, CurrentCamera.Projection);
            }
            
            base.Draw(gameTime);
        }
    }
}
