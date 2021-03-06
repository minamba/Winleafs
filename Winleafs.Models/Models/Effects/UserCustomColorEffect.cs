﻿using System.Drawing;

namespace Winleafs.Models.Models.Effects
{
    public class UserCustomColorEffect
    {
        /// <summary>
        /// The name of the effect supplied by the user.
        /// </summary>
        public string EffectName { get; set; }

        /// <summary>
        /// The color that the nanoleaf device should be.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Effects are equal when their names are equal, since
        /// names are unique identifiers
        /// </summary>
        public override bool Equals(object obj)
        {
            return EffectName == ((UserCustomColorEffect)obj).EffectName;
        }

        public override int GetHashCode()
        {
            return EffectName.GetHashCode();
        }

        public static string DisplayName(string effectName) => $"{UserSettings.CustomColorNamePreface}{effectName}";
    }
}
