using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;

namespace Tools_3D_FreeCamera
{
    class CustomModel
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Model Model { get; private set; }

        private Matrix[] modelTransforms;
        private GraphicsDevice graphicsDevice;


        public CustomModel(Model model, Vector3 position, Vector3 rotation, Vector3 scale,
            GraphicsDevice graphicsDevice)
        {
            Model = model;
            // Create a new array of matrices with capacity of total bones in the model
            modelTransforms = new Matrix[Model.Bones.Count];
            // Copy the models bone matrices to the modelTransforms matrix array
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Position = position;
            Rotation = rotation;
            Scale = scale;

            this.graphicsDevice = graphicsDevice;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            // Create new matrix with current scale, rotation and translation
            Matrix baseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(Position);

            // For every mesh in model 
            foreach (ModelMesh mesh in Model.Meshes)
            {
                // Create a local matrix by combining the parent bone´s matrix and baseworld matrix
                Matrix localWorldMatrix = modelTransforms[mesh.ParentBone.Index] * baseWorld;

                // For every mesh part in mesh
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    // Create an effect with the material proprieties
                    BasicEffect effect = (BasicEffect)part.Effect;
                    // Set matrices
                    effect.World = localWorldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                    // enable lighting
                    effect.EnableDefaultLighting();
                }

                // finally draw entire mesh
                mesh.Draw();
            }
        }
    }
}