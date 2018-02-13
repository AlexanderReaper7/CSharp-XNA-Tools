using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tools_Starfield
{
    class PlayerManager
    {
        // Sprite
        Texture2D playerSprite;
        float timer = 0f;
        // Time between frames
        readonly float interval = 200f;
        int currentFrame;
        int spriteWidth = 32;
        int spriteHeight = 48;
        // Player speed
        readonly int spriteSpeed = 2;
        Rectangle sourceRect;
        Vector2 position;
        Vector2 velocity;
        // Controll
        KeyboardState currentKBState;
        KeyboardState previousKBState;

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

        public Texture2D Texture
        {
            get { return playerSprite; }
            set { playerSprite = value; }
        }

        public Rectangle SourceRect
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        public PlayerManager(Texture2D texture, int currentFrame, int spriteWidth, int spriteheight) // Controlls what´s allowed as input
        {
            this.playerSprite = texture;
            this.currentFrame = currentFrame;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
        }

        public void HandleSpriteMovement(GameTime gametime)
        {
            previousKBState = currentKBState; // Saves last keyboard state
            currentKBState = Keyboard.GetState(); // Renews keyboard state

            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            // makes player stop animating and sets currentFrame to the corrispoding frame when all keys are released
            if (currentKBState.GetPressedKeys().Length == 0)
            {
                if (currentFrame > 0 && currentFrame < 4)
                {
                    currentFrame = 0;
                }
                if (currentFrame > 4 && currentFrame < 8)
                {
                    currentFrame = 4;
                }
                if (currentFrame > 8 && currentFrame < 12)
                {
                    currentFrame = 8;
                }
                if (currentFrame > 12 && currentFrame < 16)
                {
                    currentFrame = 12;
                }
            }
            if (currentKBState.IsKeyDown(Keys.Right))
            {
                AnimateRight(gametime);
                if (position.X < 780) // Move player right if inside window
                {
                    position.X += spriteSpeed;
                }
            }
            if (currentKBState.IsKeyDown(Keys.Left))
            {
                AnimateLeft(gametime);
                if (position.X > 20) // Move player left if inside window
                {
                    position.X -= spriteSpeed;
                }
            }
            if (currentKBState.IsKeyDown(Keys.Down))
            {
                AnimateDown(gametime);
                if (position.Y < 575) // Move player down if inside window
                {
                    position.Y += spriteSpeed;
                }
            }
            if (currentKBState.IsKeyDown(Keys.Up))
            {
                AnimateUp(gametime);
                if (position.Y > 25) // Move player up if inside window
                {
                    position.Y -= spriteSpeed;
                }
            }

            velocity = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2); // Not used for anything
        }

        public void AnimateRight(GameTime gametime)
        {
            if (currentKBState != previousKBState) // if new KBState, return to idle frame for the specific direction
            {
                currentFrame = 9;  // Idle frame
            }

            timer += (float)gametime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++; // Next frame

                if (currentFrame > 11)
                {
                    currentFrame = 8;
                }
                timer = 0f; // Reset Timer
            }
        }

        public void AnimateUp(GameTime gametime)
        {
            if (currentKBState != previousKBState) // if new KBState, return to idle frame for the specific direction
            {
                currentFrame = 13; // Idle frame
            }

            timer += (float)gametime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++; // Next frame

                if (currentFrame > 15)
                {
                    currentFrame = 12;
                }
                timer = 0f; // Reset Timer
            }
        }

        public void AnimateDown(GameTime gametime)
        {
            if (currentKBState != previousKBState) // if new KBState, return to idle frame for the specific direction
            {
                currentFrame = 1; // Idle frame
            }

            timer += (float)gametime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++; // Next frame

                if (currentFrame > 3)
                {
                    currentFrame = 0;
                }
                timer = 0f; // Reset Timer
            }
        }

        public void AnimateLeft(GameTime gametime)
        {
            if (currentKBState != previousKBState) // if new KBState, return to idle frame for the specific direction
            {
                currentFrame = 5; // Idle frame
            }

            timer += (float)gametime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++; // Next frame

                if (currentFrame > 7)
                {
                    currentFrame = 4;
                }
                timer = 0f; // Reset Timer
            }
        }

    }
}
