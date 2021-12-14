using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_MultipleCamera
{
    class ChaseCamera : Camera
    {

        public Vector3 CameraRotation { get; set; }

        /// <summary>
        /// Coordinate the camera is looking at
        /// </summary>
        public Vector3 Target { get; private set; }

        /// <summary>
        /// The target´s position
        /// </summary>
        public Vector3 FollowTargetPosition { get; private set; }

        /// <summary>
        /// The target´s rotation
        /// </summary>
        public Vector3 FollowTargetRotation { get; private set; }

        /// <summary>
        /// position offset
        /// </summary>
        public Vector3 PositionOffset { get; set; }

        /// <summary>
        /// position offset from target
        /// </summary>
        public Vector3 TargetOffset { get; set; }



        private float springiness = .15f;

        /// <summary>
        /// Strength of interpolation of position
        /// </summary>
        public float Springiness
        {
            get { return springiness; }
            set { springiness = MathHelper.Clamp(value, 0, 1); }
        }

        /// <summary>
        /// instantiates a new ChaseCamera
        /// </summary>
        /// <param name="positionOffset"></param>
        /// <param name="targetOffset"></param>
        /// <param name="cameraRotation"></param>
        /// <param name="graphicsDevice"></param>
        public ChaseCamera(Vector3 positionOffset, Vector3 targetOffset, Vector3 cameraRotation, GraphicsDevice graphicsDevice) 
            : base(graphicsDevice)
        {
            PositionOffset = positionOffset;
            TargetOffset = targetOffset;
            CameraRotation = cameraRotation;
        }

        /// <summary>
        /// Set new target position and rotation
        /// </summary>
        /// <param name="newFollowTargetPosition"></param>
        /// <param name="newFollowTargetRotation"></param>
        public void Move(Vector3 newFollowTargetPosition, Vector3 newFollowTargetRotation)
        {
            FollowTargetPosition = newFollowTargetPosition;
            FollowTargetRotation = newFollowTargetRotation;
        }

        /// <summary>
        /// Sets a new rotation
        /// </summary>
        /// <param name="rotationChange"></param>
        public void Rotate(Vector3 rotationChange)
        {
            CameraRotation = rotationChange;
        }

        public override void Update()
        {
            // add rotation of target and camera together
            Vector3 combinedRotation = FollowTargetRotation + CameraRotation;
            // Calculate rotation matrix for camera
            Matrix rotation = Matrix.CreateFromYawPitchRoll(combinedRotation.Y, combinedRotation.X, combinedRotation.Z);

            // calculate new position to move camera to
            Vector3 desiredPosition = FollowTargetPosition + Vector3.Transform(PositionOffset, rotation);
            // Interpolate between desiredPosition and Position
            Position = Vector3.Lerp(Position, desiredPosition, Springiness);

            // Calculate new target from rotation matrix
            Target = FollowTargetPosition + Vector3.Transform(TargetOffset, rotation);

            // Calculate up vector from rotation
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            // Calculate view matrix
            View = Matrix.CreateLookAt(Position, Target, up);
        }
    }
}
