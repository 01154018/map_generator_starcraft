using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{
	public interface INoise 
	{
        float Frequency { get; set; }
        float Amplitude { get; set; }
        Vector3 Offset { get; set; }
		float Sample2D(float x, float y);
        void UpdateSeed(int seed);

	}

}
