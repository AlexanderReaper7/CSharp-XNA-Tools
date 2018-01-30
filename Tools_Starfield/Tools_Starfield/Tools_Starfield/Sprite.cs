using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tools_Starfield
{
    class Sprite
    {
        public Texture2D Texture;

        private int frameWidth;
        private int frameHeight;
        private int currentFrame;
        private float frameTime = 0.1f;
        private float timeForCurrentFrame;

        private Color tintColor = Color.White;
        private float rotation;

        public int collisionRadius;
        public int boundingXPadding;
        public int boundingYPadding;
        protected Vector2 position;
        protected Vector2 velocity;

        protected List<Rectangle> frames = new List<Rectangle>();

        public Sprite(Vector2 position, Texture2D texture, Rectangle initialFrame, Vector2 velocity)
        {
            this.position = position;
            Texture = texture;
            this.velocity = velocity;

            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
        }

        #region Get/Set
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }
        }

        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        public Rectangle Destination
        {
            get { return new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight); }
        }

        public Vector2 Center
        {
            get { return position + new Vector2(frameWidth / 2, frameHeight / 2); }
        }

        public Rectangle BoundingBoxRect
        {
            get { return new Rectangle((int)position.X + boundingXPadding, (int)position.Y + boundingYPadding, frameWidth - (boundingXPadding * 2), frameHeight - (boundingYPadding * 2)); }
        }
        #endregion

        public bool IsBoxColliding(Rectangle OtherBox)
        {
            return BoundingBoxRect.Intersects(OtherBox);
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if (Vector2.Distance(Center, otherCenter) < (collisionRadius + otherRadius))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        public virtual void Update(GameTime gametime)
        {
            float elapsed = (float)gametime.ElapsedGameTime.TotalSeconds;

            timeForCurrentFrame += elapsed;

            if (timeForCurrentFrame >= FrameTime)
            {
                currentFrame = (currentFrame + 1) % (frames.Count);
                timeForCurrentFrame = 0.0f;
            }

            position += (velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Center, Source, tintColor, rotation, new Vector2(frameWidth / 2, frameHeight / 2), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
