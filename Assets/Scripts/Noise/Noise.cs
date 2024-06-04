using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public abstract class Noise : INoise
	{
        public float Frequency { get; set; }
        public float Amplitude { get; set; }
        public Vector3 Offset { get; set; }
		public Noise()
		{
            
		}
		public abstract float Sample2D(float x, float y);
        public abstract void UpdateSeed(int seed);
		
	}

}