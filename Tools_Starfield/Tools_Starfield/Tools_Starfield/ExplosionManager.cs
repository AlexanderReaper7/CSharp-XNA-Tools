﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Tools_Starfield
{
    class ExplosionManager
    {
        private Texture2D texture;
        private List<Rectangle> pieceRectangles = new List<Rectangle>();
        private Rectangle pointRectangle;

        private int minPieceCount = 3;
        private int maxPieceCount = 6;
        private int minPointCount = 20;
        private int maxPointCount = 30;

        private int durationCount = 90;
        private float explosionMaxSpeed = 30f;

        private float pieceSpeedScale = 6f;
        private int pointSpeedMin = 15;
        private int pointSpeedMax = 30;

        private Color initalColor = new Color(1.0f, 0.3f, 0f) * 0.5f;
        private Color finalColor = new Color(0f, 0f, 0f, 0f);

        Random rand = new Random();

        private List<Particles> ExplosionParticles = new List<Particles>();

        public ExplosionManager(Texture2D texture, Rectangle initalFrame, int peiceCount, Rectangle pointRectangle)
        {
            this.texture = texture;
            for (int i = 0; i < peiceCount; i++)
            {
                pieceRectangles.Add(new Rectangle(initalFrame.X + (initalFrame.Width * i), initalFrame.Y, initalFrame.Width, initalFrame.Height));
            }
            this.pointRectangle = pointRectangle;
        }

        public Vector2 randomDirection(float scale)
        {
            Vector2 direction;
            do
            {
                direction = new Vector2(rand.Next(0, 101) - 50, rand.Next(0, 101) - 50);
            } while (direction.Length() == 0);
            direction.Normalize();
            direction *= scale;

            return direction;
        }

        public void AddExplosion(Vector2 location, Vector2 momentum)
        {
            Vector2 pieceLocation = location - new Vector2(pieceRectangles[0].Width / 2, pieceRectangles[0].Height / 2);

            int pieces = rand.Next(minPieceCount, maxPieceCount + 1);
            for (int i = 0; i < pieces; i++)
            {
                ExplosionParticles.Add(new Particles(pieceLocation, texture, pieceRectangles[rand.Next(0, pieceRectangles.Count)], randomDirection(pieceSpeedScale) + momentum, Vector2.Zero, explosionMaxSpeed, durationCount, initalColor, finalColor));
            }

            int points = rand.Next(minPointCount, maxPointCount + 1);
            for (int i = 0; i < points; i++)
            {
                ExplosionParticles.Add(new Particles(location, texture, pointRectangle, randomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax)) + momentum, Vector2.Zero, explosionMaxSpeed, durationCount, initalColor, finalColor));
            }
        }

        public void update(GameTime gameTime)
        {
            for (int i = ExplosionParticles.Count - 1; i >= 0; i--)
            {
                if (ExplosionParticles[i].IsActive)
                {
                    ExplosionParticles[i].Update(gameTime);
                }
                else
                {
                    ExplosionParticles.RemoveAt(i);
                }
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Particles particle in ExplosionParticles)
            {
                particle.Draw(spriteBatch);
            }
        }
    }
}
