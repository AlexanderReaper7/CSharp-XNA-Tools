using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_MultipleCamera
{
    public abstract class Camera
    {
        private Matrix view;
        private Matrix projection;

        public Matrix Projection
        {
            get { return projection; }

            protected set
            {
                projection = value;
                // Update the frustum
                GenerateFrustum();
            }
        }

        public Matrix View
        {
            get { return view; }

            set
            {
                view = value;
                // Update the frustum
                GenerateFrustum();
            }
        }

        public Vector3 Position { get; set; }

        /// <summary>
        /// frustum of camera view
        /// </summary>
        public BoundingFrustum Frustum { get; private set; }

        protected GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        /// Instantiates a new camera
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public Camera(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;

            // Calculate projection matrix
            GeneratePerspectiveProjectionMatrix(MathHelper.PiOver4);
        }
        /// <summary>
        /// Creates a new projection matrix with a field of view
        /// </summary>
        /// <param name="fieldOfView"></param>
        private void GeneratePerspectiveProjectionMatrix(float fieldOfView)
        {
            // Get presentation parameters from GraphicsDevice
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            // Calculate aspect ratio
            float aspectRatio = (float) pp.BackBufferWidth / (float) pp.BackBufferHeight;
            // Create projection matrix
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000000.0f);
        }

        public virtual void Update()
        {

        }
        /// <summary>
        /// Calculates a new Bounding Frustum 
        /// </summary>
        private void GenerateFrustum()
        {
            // Combine view and projection
            Matrix viewProjection = View * Projection;
            // Calculate frustum of what the camera sees
            Frustum = new BoundingFrustum(viewProjection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool BoundingVolumeIsInView(BoundingSphere sphere)
        {
            return (Frustum.Contains(sphere) != ContainmentType.Disjoint);
        }

        public bool BoundingVolumeIsInView(BoundingBox box)
        {
            return (Frustum.Contains(box) != ContainmentType.Disjoint);
        }
    }
}