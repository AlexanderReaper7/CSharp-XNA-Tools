using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Tools_DynamicLights
{
    public enum LightType
    {
        Point
    }


    public abstract class Light
    {

        protected float initialPower;

        public Vector3 Position;

        public Vector4 Color;

        public float ActualPower { get; set; }

        public float Power
        {
            get { return initialPower; }
            set
            {
                initialPower = value;
                ActualPower = initialPower;
            }
        }

        public int LightDecay { get; set; }

        public LightType LightType { get; private set; }

        public bool IsEnabled { get; set; }

        public Vector3 Direction { get; set; }

        protected Light(LightType lightType)
        {
            LightType = lightType;
        }

        public void EnableLight(bool enabled, float timeToEnable)
        {
            // If the light is to be On
            IsEnabled = enabled;
        }

        // Updates light
        public virtual void Update(GameTime gameTime)
        {
            if (!IsEnabled)
            {
                return;
            }
        }

        protected void CopyBaseFields(Light light)
        {
            light.Color = Color;
            light.IsEnabled = IsEnabled;
            light.LightDecay = LightDecay;
            light.LightType = LightType;
            light.Position = Position;
            light.Power = Power;
        }

        public abstract Light DeepCopy();
    }
}
