using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public class FractalNoise
    {
        public int Octaves { get; set; }
        public float Frequency { get; set; }
        public float Amplitude { get; set; }
        public Vector3 Offset { get; set; }
        public float Lacunarity { get; set; }
        public float Gain { get; set; }
        public INoise[] Noises { get; set; }
        public float[] Amplitudes { get; set; }
        public float[] Frequencies { get; set; }
	
        public FractalNoise(INoise noise, int octaves, float frequency, float amplitude = 1.0f)
        {

            Octaves = octaves;
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;
            Lacunarity = 2.0f;
            Gain = 0.5f;

            UpdateTable(new INoise[] { noise });
        }

        protected virtual void UpdateTable(INoise[] noises)
		{
			Amplitudes = new float[Octaves];
			Frequencies = new float[Octaves];
            Noises = new INoise[Octaves];

            int numNoises = noises.Length;
			
			float amp = 0.5f;
			float frq = Frequency;
			for(int i = 0; i < Octaves; i++)
			{
                Noises[i] = noises[Math.Min(i, numNoises - 1)];
				Frequencies[i] = frq;
				Amplitudes[i] = amp;
				amp *= Gain;
				frq *= Lacunarity;
			}

		}
        public virtual float Sample2D(float x, float y)
        {
			x = x + Offset.x;
			y = y + Offset.y;

	        float sum = 0, frq;
	        for(int i = 0; i < Octaves; i++) 
	        {
				frq = Frequencies[i];

                if (Noises[i] != null)
                    sum += Noises[i].Sample2D(x * frq, y * frq) * Amplitudes[i];
			}
            sum= sum + 0.5f;

            return sum * Amplitude;
        }

    }

}














