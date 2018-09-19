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

namespace Tools_Normalmap
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// Brickwall texture
        /// </summary>
        private Texture2D background;
        /// <summary>
        /// Brickwall normalmap
        /// </summary>
        private Texture2D backgroundNormals;

        private RenderTarget2D colorMapRenderTarget;
        private RenderTarget2D normalMapRenderTarget;
        private RenderTarget2D shadowMapRenderTarget;

        private Color ambientLight = new Color(.1f, .1f, .1f, 1);

        private Effect lightEffect;
        private Effect lightCombinedEffect;

        private EffectParameter lightEffectParameterScreenWidth;
        private EffectParameter lightEffectParameterScreenHeight;
        private EffectParameter lightEffectParameterNormalMap;

        private EffectTechnique lightCombinedEffectTechnique;
        private EffectParameter lightCombinedEffectParamAmbient;
        private EffectParameter lightCombinedEffectParamLightAmbient;
        private EffectParameter lightCombinedEffectParamAmbientColor;
        private EffectParameter lightCombinedEffectParamColorMap;
        private EffectParameter lightCombinedEffectParamNormalMap;
        private EffectParameter lightCombinedEffectParamShadowMap;

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
            // Change resolution to 1280x720
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

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

            // Load brickwall texture
            background = Content.Load<Texture2D>(@"Brickwall_Texture");
            // Load brickwall normalmap
            backgroundNormals = Content.Load<Texture2D>(@"Brickwall_Normalmap");

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            colorMapRenderTarget = new RenderTarget2D(GraphicsDevice, width, height);
            normalMapRenderTarget = new RenderTarget2D(GraphicsDevice, width, height);
            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, format,
                pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            lightEffect = Content.Load<Effect>(@"MultiTarget");
            lightCombinedEffect = Content.Load<Effect>(@"DeferredCombined");

            lightEffectParameterNormalMap = lightEffect.Parameters["NormalMap"];
            lightEffectParameterScreenHeight = lightEffect.Parameters["screenHeight"];
            lightEffectParameterScreenWidth = lightEffect.Parameters["screenWidth"];

            lightCombinedEffectTechnique = lightCombinedEffect.Techniques["DeferredCombined2"];
            lightCombinedEffectParamAmbient = lightCombinedEffect.Parameters["ambient"];
            lightCombinedEffectParamLightAmbient = lightCombinedEffect.Parameters["lightAmbient"];
            lightCombinedEffectParamAmbientColor = lightCombinedEffect.Parameters["ambientColor"];
            lightCombinedEffectParamColorMap = lightCombinedEffect.Parameters["ColorMap"];
            lightCombinedEffectParamNormalMap = lightCombinedEffect.Parameters["NormalMap"];
            lightCombinedEffectParamShadowMap = lightCombinedEffect.Parameters["ShadingMap"];

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
            if (gamepad.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.End))
                this.Exit();

            //F to toggle fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
            }

            base.Update(gameTime);
        }

        private void DrawColorMap()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background,Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        private void DrawNormalMap()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundNormals,Vector2.Zero, Color.White);

            spriteBatch.End();

            // Deactivate the render targets to resolve them
            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawCombinedMaps()
        {
            lightCombinedEffect.CurrentTechnique = lightCombinedEffectTechnique;
            lightCombinedEffectParamAmbient.SetValue(1f);
            lightCombinedEffectParamLightAmbient.SetValue(4);
            lightCombinedEffectParamAmbientColor.SetValue(ambientLight.ToVector4());
            lightCombinedEffectParamColorMap.SetValue(colorMapRenderTarget);
            lightCombinedEffectParamNormalMap.SetValue(normalMapRenderTarget);
            lightCombinedEffectParamShadowMap.SetValue(shadowMapRenderTarget);
            lightCombinedEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null,null,lightCombinedEffect);
            spriteBatch.Draw(colorMapRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set the render targets
            GraphicsDevice.SetRenderTarget(colorMapRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            DrawColorMap();

            // Clear all render targets
            GraphicsDevice.SetRenderTarget(null);

            // Set the render targets
            GraphicsDevice.SetRenderTarget(normalMapRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            DrawNormalMap();

            // Clear all render targets
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            DrawCombinedMaps();

            base.Draw(gameTime);
        }
    }
}
