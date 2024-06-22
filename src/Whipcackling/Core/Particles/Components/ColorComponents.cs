using Arch.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Whipcackling.Core.Particles.Components
{
    public struct LinearAlphaFade(int amount)
    {
        public int DecreaseAmount = amount;
    }

    public struct ExponentialAlphaFade(float amount)
    {
        public float DecreaseAmount = amount;
    }

    public struct LinearColorFade(int amount)
    {
        public int DecreaseAmount = amount;
    }

    public struct ExponentialColorFade(float amount)
    {
        public float DecreaseAmount = amount;
    }

    public struct ShiftColorThree(Color color1, Color color2, Color color3)
    {
        public Color FirstColor = color1;
        public Color SecondColor = color2;
        public Color ThirdColor = color3;
    }

    public struct AlphaFadeInOut(int fadeIn, int fadeOut)
    {
        public int FadeInTime = fadeIn;
        public int FadeOutTime = fadeOut;
    }
}
