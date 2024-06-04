using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public class ValueNoise : Noise
	{

        private PermutationTable Perm { get; set; }

        public ValueNoise(int seed, float frequency, float amplitude = 1.0f) 
        {

            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Perm = new PermutationTable(1024, 255, seed);

        }
        public override void UpdateSeed(int seed)
        {
            Perm.Build(seed);
        }
        public override float Sample2D(float x, float y)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            int ix0, iy0;
            float fx0, fy0, s, t, nx0, nx1, n0, n1;

            ix0 = (int)Mathf.Floor(x);
            iy0 = (int)Mathf.Floor(y);

            fx0 = x - ix0;
            fy0 = y - iy0;

            t = FADE(fy0);
            s = FADE(fx0);

            nx0 = Perm[ix0, iy0];
            nx1 = Perm[ix0, iy0 + 1];

            n0 = LERP(t, nx0, nx1);

            nx0 = Perm[ix0 + 1, iy0];
            nx1 = Perm[ix0 + 1, iy0 + 1];

            n1 = LERP(t, nx0, nx1);

            float n = LERP(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        private float FADE(float t) { return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f); }

        private float LERP(float t, float a, float b) { return a + t * (b - a); }

	}

}





