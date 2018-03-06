using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_Starfield
{
    class Enemy
    {
        public Sprite EnemySprite;
        public Vector2 gunOffset = new Vector2(25, 25);
        private Queue<Vector2> waypoints = new Queue<Vector2>();
        private Vector2 currentWaypoint = Vector2.Zero;
        private float speed = 120f;
        public bool Destroyed = false;
        private int enemyRadius = 15;
        private Vector2 previousPosition = Vector2.Zero;

        public Enemy(Texture2D texture, Vector2 Position, Rectangle initialFrame, int frameCount)
        {
            EnemySprite = new Sprite(Position, texture, initialFrame, Vector2.Zero);

            for(int i = 1; i < frameCount; i++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X = (initialFrame.Width * i), initialFrame.Y, initialFrame.Width, initialFrame.Height));
            }
            previousPosition = Position;
            currentWaypoint = Position;
            EnemySprite.collisionRadius = enemyRadius;
        }

        public void AddWaypoint(Vector2 waypoint)
        {
            waypoints.Enqueue(waypoint);
        }

        public bool WaypointReached()
        {
            if (Vector2.Distance(EnemySprite.Position, currentWaypoint) < (float)EnemySprite.Source.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsActive()
        {
            if (Destroyed)
            {
                return false;
            }

            if(waypoints.Count > 0)
            {
                return true;
            }

            if(WaypointReached())
            {
                return true;
            }

            return true;
        }

        public void Update(GameTime gametime)
        {
            if (IsActive())
            {
                Vector2 heading = currentWaypoint - EnemySprite.Position;
                if (heading != Vector2.Zero)
                {
                    heading.Normalize();
                }
                heading *= speed;
                EnemySprite.Velocity = heading;
                previousPosition = EnemySprite.Position;
                EnemySprite.Update(gametime);
                EnemySprite.Rotation = (float)Math.Atan2(EnemySprite.Position.Y - previousPosition.Y, EnemySprite.Position.X - previousPosition.X);

                if (WaypointReached())
                {
                    if (waypoints.Count > 0)
                    {
                        currentWaypoint = waypoints.Dequeue();
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(IsActive())
            {
                EnemySprite.Draw(spriteBatch);
            }
        }
    }
}
