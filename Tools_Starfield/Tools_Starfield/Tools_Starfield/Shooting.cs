using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tools_Starfield
{
    class Shooting
    {
        public List<Sprite> Shots = new List<Sprite>();
        private Rectangle screenBounds;

        private static Texture2D Texture;
        private static Rectangle InitialFrame;
        private static int FrameCount;
        private float shotSpeed;
        private static int CollisionRadius;

        public Shooting(Texture2D texture, Rectangle initialFrame, int frameCount, int collisionRadius, float shotSpeed, Rectangle screenBounds)
        {
            Texture = texture;
            InitialFrame = initialFrame;
            FrameCount = frameCount;
            CollisionRadius = collisionRadius;
            this.shotSpeed = shotSpeed;
            this.screenBounds = screenBounds;
        }

        public void FireShot(Vector2 position, Vector2 velocity, bool playerFired)
        {
            Sprite thisShot = new Sprite(position, Texture, InitialFrame, velocity);

            thisShot.Velocity *= shotSpeed;

            for (int i = 1; i < FrameCount; i++)
            {
                thisShot.AddFrame(new Rectangle(InitialFrame.X + (InitialFrame.Width * i), InitialFrame.Y, InitialFrame.Width, InitialFrame.Height));
            }
            thisShot.collisionRadius = CollisionRadius;
            Shots.Add(thisShot);
        }

        public void Update(GameTime gametime)
        {
            for (int i = Shots.Count - 1; i >= 0; i--)
            {
                Shots[i].Update(gametime);
                if (!screenBounds.Intersects(Shots[i].Destination))
                {
                    Shots.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (Sprite shot in Shots)
            {
                shot.Draw(spritebatch);
            }
        }

    }
}
