using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_Starfield
{
    class Starfield
    {
        private List<Sprite> stars = new List<Sprite>();
        private int screenWidth = 800;
        private int screenHeight = 600;
        private Random rand = new Random();
        private Color[] colors = { Color.White, Color.Yellow, Color.Wheat, Color.WhiteSmoke, Color.SlateGray, };
   
        public Starfield(int screenWidth, int screenHeight, int starCount, Vector2 starVelocity, Texture2D texture, Rectangle frameRectangle)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            for (int i = 0; i < starCount; i++)
            {
                stars.Add(new Sprite(new Vector2(rand.Next(0, screenWidth), rand.Next(0, screenHeight)), texture, frameRectangle, starVelocity));
                Color starColor = colors[rand.Next(0, colors.Count())];
                starColor *= (float)(rand.Next(30, 80) / 100f);
                stars[stars.Count() - 1].TintColor = starColor;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach ( Sprite star in stars)
            {
                star.Update(gameTime);
                if ( star.Position.Y > screenHeight)
                {
                    star.Position = new Vector2(rand.Next(0, screenWidth), 0);
                }
            }
        } 

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ( Sprite star in stars)
            {
                star.Draw(spriteBatch);
            }
        }
    }
}
