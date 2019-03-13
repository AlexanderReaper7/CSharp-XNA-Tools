using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArcBallCamera
{
    public class ArcBallCamera : Camera
    {
        // Rotation around X and Y
        public float RotationX { get; set; }
        public float RotationY { get; set; }

        // Y axis boundary
        public float MinRotationY { get; set; }
        public float MaxRotationY { get; set; }

        // Distance between target and camera
        public float Distance { get; set; }

        // Boundaries for distance
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }

        public ArcBallCamera(Vector3 target, float rotationX, float rotationY, float minRotationY, float maxRotationY, float distance, float minDistance, float maxDistance, GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            RotationX = rotationX;
            MinRotationY = minRotationY;
            MaxRotationY = maxRotationY;
            // Clamp RotationY
            RotationY = MathHelper.Clamp(rotationY, minRotationY, maxRotationY);
            MinDistance = minDistance;
            MaxDistance = maxDistance;
            // Clamp Distance
            Distance = MathHelper.Clamp(distance, minDistance, maxDistance);
            Target = target;
        }

        public void Move(float distanceChange)
        {
            Distance += distanceChange;
            Distance = MathHelper.Clamp(Distance, MinDistance, MaxDistance);
        }

        public void Rotate(float rotationXChange, float rotationYChange)
        {
            RotationX += rotationXChange;
            RotationY += -rotationYChange;

            RotationY = MathHelper.Clamp(RotationY, MinRotationY, MaxRotationY);
        }

        public void Translate(Vector3 positionChange)
        {
            Position = positionChange;
        }

        public override void Update()
        {
            // Calculate rotation matrix from rotation
            Matrix rotation = Matrix.CreateFromYawPitchRoll(RotationX, -RotationY, 0);

            Vector3 translation = new Vector3(0, 0, Distance);
            translation = Vector3.Transform(translation, rotation);

            Position = Target + translation;

            // Up vector from rotation matrix
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
