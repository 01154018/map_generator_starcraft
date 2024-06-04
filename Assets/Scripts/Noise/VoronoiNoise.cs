using System;
using System.Collections;
using UnityEngine;

namespace ProceduralNoiseProject
{

    public enum VORONOI_DISTANCE { EUCLIDIAN, MANHATTAN, CHEBYSHEV };

    public enum VORONOI_COMBINATION { D0, D1_D0, D2_D0 };

    public class VoronoiNoise : Noise
    {

        public VORONOI_DISTANCE Distance { get; set; }

        public VORONOI_COMBINATION Combination { get; set; }

        private PermutationTable Perm { get; set; }

        public VoronoiNoise(int seed, float frequency, float amplitude = 1.0f)
        {

            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Distance = VORONOI_DISTANCE.EUCLIDIAN;
            Combination = VORONOI_COMBINATION.D1_D0;

            Perm = new PermutationTable(1024, int.MaxValue, seed);

        }

        public override void UpdateSeed(int seed)
        {
            Perm.Build(seed);
        }

        public override float Sample2D(float x, float y)
        {
            x = (x + Offset.x) * Frequency * 0.75f;
            y = (y + Offset.y) * Frequency * 0.75f;

            int lastRandom, numberFeaturePoints;
            float randomDiffX, randomDiffY;
            float featurePointX, featurePointY;
            int cubeX, cubeY;

            Vector3 distanceArray = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

            int evalCubeX = (int)Mathf.Floor(x);
            int evalCubeY = (int)Mathf.Floor(y);

            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    cubeX = evalCubeX + i;
                    cubeY = evalCubeY + j;

                    lastRandom = Perm[cubeX, cubeY];
                    numberFeaturePoints = ProbLookup(lastRandom * Perm.Inverse);
                    for (int l = 0; l < numberFeaturePoints; ++l)
                    {
                        lastRandom = Perm[lastRandom];
                        randomDiffX = lastRandom * Perm.Inverse;

                        lastRandom = Perm[lastRandom];
                        randomDiffY = lastRandom * Perm.Inverse;

                        featurePointX = randomDiffX + cubeX;
                        featurePointY = randomDiffY + cubeY;

                        distanceArray = Insert(distanceArray, Distance2(x, y, featurePointX, featurePointY));
                    }

                }
            }

            return Combine(distanceArray) * Amplitude;
        }

        private float Distance2(float p1x, float p1y, float p2x, float p2y)
        {
            switch(Distance)
            {
                case VORONOI_DISTANCE.EUCLIDIAN:
                    return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y);

                case VORONOI_DISTANCE.MANHATTAN:
                    return Math.Abs(p1x - p2x) + Math.Abs(p1y - p2y);

                case VORONOI_DISTANCE.CHEBYSHEV:
                    return Math.Max(Math.Abs(p1x - p2x), Math.Abs(p1y - p2y));
            }

            return 0;
        }

        private float Combine(Vector3 arr)
        {
            switch(Combination)
            {
                case VORONOI_COMBINATION.D0:
                    return arr[0];

                case VORONOI_COMBINATION.D1_D0:
                    return arr[1] - arr[0];

                case VORONOI_COMBINATION.D2_D0:
                    return arr[2] - arr[0];
            }

            return 0;
        }

        int ProbLookup(float value)
        {
            if (value < 0.0915781944272058) return 1;
            if (value < 0.238103305510735) return 2;
            if (value < 0.433470120288774) return 3;
            if (value < 0.628836935299644) return 4;
            if (value < 0.785130387122075) return 5;
            if (value < 0.889326021747972) return 6;
            if (value < 0.948866384324819) return 7;
            if (value < 0.978636565613243) return 8;

            return 9;
        }
        Vector3 Insert(Vector3 arr, float value)
        {
            float temp;
            for (int i = 3 - 1; i >= 0; i--)
            {
                if (value > arr[i]) break;
                temp = arr[i];
                arr[i] = value;
                if (i + 1 < 3) arr[i + 1] = temp;
            }

            return arr;
        }

    }


}