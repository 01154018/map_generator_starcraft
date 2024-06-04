using System;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public class SimplexNoise : Noise
    {

        private PermutationTable Perm { get; set; }
        public SimplexNoise(int seed, float frequency, float amplitude = 1.0f)
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
            x = (x + Offset.x) * Frequency * 0.5f;
            y = (y + Offset.y) * Frequency * 0.5f;

            const float F2 = 0.366025403f;
            const float G2 = 0.211324865f;

            float n0, n1, n2;

            float s = (x+y)*F2;
            float xs = x + s;
            float ys = y + s;
            int i = (int)Mathf.Floor(xs);
            int j = (int)Mathf.Floor(ys);

            float t = (i+j)*G2;
            float X0 = i-t;
            float Y0 = j-t;
            float x0 = x-X0;
            float y0 = y-Y0;

            int i1, j1; 
            if(x0>y0) {i1=1; j1=0;} 
            else {i1=0; j1=1;} 


            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2;
            float y2 = y0 - 1.0f + 2.0f * G2;

            float t0 = 0.5f - x0*x0-y0*y0;
            if(t0 < 0.0) n0 = 0.0f;
            else {
                t0 *= t0;
				n0 = t0 * t0 * Grad(Perm[i, j], x0, y0); 
            }

            float t1 = 0.5f - x1*x1-y1*y1;
            if(t1 < 0.0) n1 = 0.0f;
            else {
                t1 *= t1;
				n1 = t1 * t1 * Grad(Perm[i+i1, j+j1], x1, y1);
            }

            float t2 = 0.5f - x2*x2-y2*y2;
            if(t2 < 0.0) n2 = 0.0f;
            else {
                t2 *= t2;
				n2 = t2 * t2 * Grad(Perm[i+1, j+1], x2, y2);
            }

            return 40.0f * (n0 + n1 + n2) * Amplitude; 
        }

        private float Grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }


    }
}




