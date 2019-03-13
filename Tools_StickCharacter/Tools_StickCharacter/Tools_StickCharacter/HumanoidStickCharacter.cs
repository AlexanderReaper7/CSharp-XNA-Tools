using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_StickCharacter
{
    class HumanoidStickCharacter
    {
        public static Texture2D GenerateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color, float sharpness)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new Texture2D(graphicsDevice, diameter, diameter, false, SurfaceFormat.Color);
            // Set colorData size
            Color[] colorData = new Color[circleTexture.Width * circleTexture.Height];
            // Center of texture derived from radius
            Vector2 center = new Vector2(radius);
            for (int colIndex = 0; colIndex < circleTexture.Width; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < circleTexture.Height; rowIndex++)
                {
                    Vector2 position = new Vector2(colIndex, rowIndex);
                    float distance = Vector2.Distance(center, position);

                    // hermite iterpolation
                    float x = distance / diameter;
                    float edge0 = (radius * sharpness) / (float)diameter;
                    float edge1 = radius / (float)diameter;
                    float temp = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
                    float result = temp * temp * (3.0f - 2.0f * temp);

                    colorData[rowIndex * circleTexture.Width + colIndex] = color * (1f - result);
                }
            }
            circleTexture.SetData<Color>(colorData);

            return circleTexture;
        }


        public static Texture2D GenerateRoundedRectangleTexture(
            GraphicsDevice graphicsDevice, int width, int height, Color color, float sharpness)
        {
            int radius = height / 2;

            // Create a circle with radius of height / 2
            Texture2D circle = GenerateCircleTexture(graphicsDevice, height / 2, color, sharpness);
            Texture2D texture = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);

            Color[] tempCircleData = new Color[circle.Width * circle.Height];  
            circle.GetData(tempCircleData);

            Color[,] circleData = tempCircleData.ConvertTo2D(width, height);
            Color[,] textureData = new Color[width, height];

            // Create mid part
            for (int x = radius; x < width - radius; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    textureData[x, y] = circleData[radius, y];
                }
            }

            // Create beginning part
            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    textureData[x, y] = circleData[x, y];
                }
            }

            // Create end part
            for (int x = width - radius; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    
                }
            }

            Color[] data = new Color[textureData.Length];

            texture.SetData(data);
            return texture;
        }
    }
}
