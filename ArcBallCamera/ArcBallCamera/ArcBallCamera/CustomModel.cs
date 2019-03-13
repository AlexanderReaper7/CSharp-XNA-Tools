using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;

namespace ArcBallCamera
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

            modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Position = position;
            Rotation = rotation;
            Scale = scale;

            this.graphicsDevice = graphicsDevice;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix baseWorld = Matrix.CreateScale(Scale)
                * Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z)
                * Matrix.CreateTranslation(Position);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                Matrix localWorldMatrix = modelTransforms[mesh.ParentBone.Index] * baseWorld;

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)part.Effect;

                    effect.World = localWorldMatrix;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}