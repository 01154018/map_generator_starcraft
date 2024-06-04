using System;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public class PerlinNoise : Noise
	{
        private PermutationTable Perm { get; set; }

        public PerlinNoise(int seed, float frequency, float amplitude = 1.0f)
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
		public override float Sample2D( float x, float y )
		{
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

		    int ix0, iy0;
		    float fx0, fy0, fx1, fy1, s, t, nx0, nx1, n0, n1;
		
			ix0 = (int)Mathf.Floor(x);
			iy0 = (int)Mathf.Floor(y);

		    fx0 = x - ix0;
		    fy0 = y - iy0;
		    fx1 = fx0 - 1.0f;
		    fy1 = fy0 - 1.0f;
		    
		    t = FADE( fy0 );
		    s = FADE( fx0 );

			nx0 = Grad(Perm[ix0, iy0], fx0, fy0);
            nx1 = Grad(Perm[ix0, iy0 + 1], fx0, fy1);

		    n0 = LERP( t, nx0, nx1 );

		    nx0 = Grad(Perm[ix0 + 1, iy0], fx1, fy0);
		    nx1 = Grad(Perm[ix0 + 1, iy0 + 1], fx1, fy1);

		    n1 = LERP(t, nx0, nx1);

            return 0.66666f * LERP(s, n0, n1) * Amplitude;
		}

        private float FADE(float t) { return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f); }

        private float LERP(float t, float a, float b) { return a + t * (b - a); }

        private float Grad(int hash, float x, float y)
        {
            int h = hash & 7; 
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }

    }

}













