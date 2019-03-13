using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_MultipleCamera
{
    public class ArcBallCamera : Camera
    {
        // Rotation around X and Y
        public float RotationX { get; set; }
        public float RotationY { get; set; }

        /// <summary>
        /// minimum camera rotation on y axis
        /// </summary>
        public float MinRotationY { get; set; }

        /// <summary>
        /// maximum camera rotation on y axis
        /// </summary>
        public float MaxRotationY { get; set; }

        /// <summary>
        /// Distance from camera to target
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Minimum distance from camera to target
        /// </summary>
        public float MinDistance { get; set; }
        /// <summary>
        /// maximum distance from camera to target
        /// </summary>
        public float MaxDistance { get; set; }

        public Vector3 Position { get; set; }

        /// <summary>
        /// Coordinate the camera should face
        /// </summary>
        public Vector3 Target { get; set; }

        /// <summary>
        /// instantiates a new ArcBallCamera
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rotationX"></param>
        /// <param name="rotationY"></param>
        /// <param name="minRotationY"></param>
        /// <param name="maxRotationY"></param>
        /// <param name="distance"></param>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        /// <param name="graphicsDevice"></param>
        public ArcBallCamera(Vector3 target, float rotationX, float rotationY, float minRotationY, float maxRotationY, float distance, float minDistance, float maxDistance, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            RotationX = rotationX;
            // set boundaries for Y rotation
            MinRotationY = minRotationY;
            MaxRotationY = maxRotationY;
            // Clamp RotationY
            RotationY = MathHelper.Clamp(rotationY, minRotationY, maxRotationY);
            // set boundaries for distance
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            // Clamp Distance
            Distance = MathHelper.Clamp(distance, minDistance, maxDistance);

            Target = target;
        }

        /// <summary>
        /// moves distance from camera to target
        /// </summary>
        /// <param name="distanceChange"></param>
        public void Move(float distanceChange)
        {
            Distance += distanceChange;
            // clamp distance
            Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }
        /// <summary>
        /// add rotation around target
        /// </summary>
        /// <param name="rotationXChange"></param>
        /// <param name="rotationYChange"></param>
        public void Rotate(float rotationXChange, float rotationYChange)
        {
            RotationX += rotationXChange;
            RotationY += -rotationYChange;

            // Clamp RotationY to min and max
            RotationY = MathHelper.Clamp(RotationY, MinRotationY, MaxRotationY);
        }

        /// <summary>
        /// Updates view 
        /// </summary>
        public override void Update()
        {
            // Calculate rotation matrix from rotation
            Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);
            // Convert distance to a Vector3
            Vector3 translation = new Vector3(0, 0, Distance);
            // Transform translation with the rotation matrix
            translation = Vector3.Transform(translation, rotation);

            // Update position with new target and distance
            Position = Target + translation;

            // Up vector from rotation matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            // Calculate view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
