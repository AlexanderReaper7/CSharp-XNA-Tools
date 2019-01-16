using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_DynamicLights
{
    class DynamicLight
    {
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

        public List<Light> lights = new List<Light>();
        private float specularStrength = 1.0f;

        private EffectTechnique lightEffectTechniquePointLight;
        private EffectParameter lightEffectParameterStrength;
        private EffectParameter lightEffectParameterPosition;
        private EffectParameter lightEffectParameterConeDirection;
        private EffectParameter lightEffectParameterLightColor;
        private EffectParameter lightEffectParameterLightDecay;

        public VertexPositionColorTexture[] Vertices;
        public VertexBuffer VertexBuffer;

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            PresentationParameters pp = graphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            Vertices = new VertexPositionColorTexture[4];

            Vertices[0] = new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.Red, new Vector2(0, 0));
            Vertices[1] = new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.Blue, new Vector2(1, 0));
            Vertices[2] = new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.Yellow, new Vector2(0, 1));
            Vertices[3] = new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1));
            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), Vertices.Length, BufferUsage.None);
            VertexBuffer.SetData(Vertices);


            colorMapRenderTarget = new RenderTarget2D(graphicsDevice, width, height);
            normalMapRenderTarget = new RenderTarget2D(graphicsDevice, width, height);
            shadowMapRenderTarget = new RenderTarget2D(graphicsDevice, width, height, false, format,
                pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            lightEffect = content.Load<Effect>(@"MultiTarget");
            lightCombinedEffect = content.Load<Effect>(@"DeferredCombined");

            lightEffectParameterNormalMap = lightEffect.Parameters["NormalMap"];
            lightEffectParameterScreenHeight = lightEffect.Parameters["screenHeight"];
            lightEffectParameterScreenWidth = lightEffect.Parameters["screenWidth"];

            lightEffectTechniquePointLight = lightEffect.Techniques["DeferredPointLight"];
            lightEffectParameterConeDirection = lightEffect.Parameters["coneDirection"];
            lightEffectParameterLightColor = lightEffect.Parameters["lightColor"];
            lightEffectParameterLightDecay = lightEffect.Parameters["lightDecay"];
            lightEffectParameterPosition = lightEffect.Parameters["lightPosition"];
            lightEffectParameterStrength = lightEffect.Parameters["lightStrength"];

            lightCombinedEffectTechnique = lightCombinedEffect.Techniques["DeferredCombined2"];
            lightCombinedEffectParamAmbient = lightCombinedEffect.Parameters["ambient"];
            lightCombinedEffectParamLightAmbient = lightCombinedEffect.Parameters["lightAmbient"];
            lightCombinedEffectParamAmbientColor = lightCombinedEffect.Parameters["ambientColor"];
            lightCombinedEffectParamColorMap = lightCombinedEffect.Parameters["ColorMap"];
            lightCombinedEffectParamNormalMap = lightCombinedEffect.Parameters["NormalMap"];
            lightCombinedEffectParamShadowMap = lightCombinedEffect.Parameters["ShadingMap"];

            lights.Add(new PointLight()
            {
                IsEnabled = true,
                Color = new Vector4(1f, 1f, 1f, 1f),
                Power = .7f,
                LightDecay = 200,
                Position = new Vector3(500, 400, 80)
            });

        }

        public void DrawCombinedMaps(SpriteBatch spriteBatch)
        {
            lightCombinedEffect.CurrentTechnique = lightCombinedEffectTechnique;
            lightCombinedEffectParamAmbient.SetValue(1f);
            lightCombinedEffectParamLightAmbient.SetValue(4);
            lightCombinedEffectParamAmbientColor.SetValue(ambientLight.ToVector4());
            lightCombinedEffectParamColorMap.SetValue(colorMapRenderTarget);
            lightCombinedEffectParamNormalMap.SetValue(normalMapRenderTarget);
            lightCombinedEffectParamShadowMap.SetValue(shadowMapRenderTarget);
            lightCombinedEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, lightCombinedEffect);
            spriteBatch.Draw(colorMapRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public Texture2D GenerateShadowMap(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(shadowMapRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            foreach (Light light in lights)
            {
                if (!light.IsEnabled) { continue; }

                graphicsDevice.SetVertexBuffer(VertexBuffer);

                // Draw all the light sources
                lightEffectParameterStrength.SetValue(light.ActualPower);
                lightEffectParameterPosition.SetValue(light.Position);
                lightEffectParameterLightColor.SetValue(light.Color);
                lightEffectParameterLightDecay.SetValue(light.LightDecay);

                lightEffect.Parameters["specularStrength"].SetValue(specularStrength);

                if (light.LightType == LightType.Point)
                {
                    lightEffect.CurrentTechnique = lightEffectTechniquePointLight;
                }

                lightEffectParameterScreenWidth.SetValue(graphicsDevice.Viewport.Width);
                lightEffectParameterScreenHeight.SetValue(graphicsDevice.Viewport.Height);
                lightEffect.Parameters["ambientColor"].SetValue(ambientLight.ToVector4());
                lightEffectParameterNormalMap.SetValue(normalMapRenderTarget);
                lightEffect.Parameters["ColorMap"].SetValue(colorMapRenderTarget);
                lightEffect.CurrentTechnique.Passes[0].Apply();

                // Add Blending (black background)
                graphicsDevice.BlendState = BlendBlack;

                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, 2);

            }

            // Reset Render targets
            graphicsDevice.SetRenderTarget(null);

            return shadowMapRenderTarget;
        }

        public static BlendState BlendBlack = new BlendState()
        {
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,

            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.One
        };

        public void DrawColorMap(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(colorMapRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }

        public void DrawNormalMap(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(normalMapRenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }
    }
}
