using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools_DynamicCharacter;

namespace Tools_DynamicCharacter
{
    public class CharacterPart : CollidableObject
    {
        /// <summary>
        /// Position from TopLeft
        /// </summary>
        public Vector2 GripPosition { get; set; }

        public Vector2 AttachmentPosition { get; set; }

        public CharacterPart(Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 origin, float rotation, Vector2 gripPosition) : base(texture, position, sourceRectangle, origin, rotation)
        {
            GripPosition = gripPosition;

        }
    }

    class HumanoidCharacter
    {
        // Body Parts
        private CharacterPart _head;
        private CharacterPart _torso;
        private CharacterPart _leftArm;
        private CharacterPart _rightArm;
        private CharacterPart _leftLeg;
        private CharacterPart _rightLeg;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }
    }
}
