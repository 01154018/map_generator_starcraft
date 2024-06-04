using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;

namespace ProceduralNoiseProject
{
    public enum NOISE_TYPE { PERLIN, VALUE, SIMPLEX, VORONOI, WORLEY }

    public class Map_Generator : MonoBehaviour
    {
        public NOISE_TYPE noiseType = NOISE_TYPE.PERLIN;
        public int seed = 0;
        public int octaves = 4;
        public float frequency = 2.0f;
        public float magnification = 8.0f;

        Dictionary<int, GameObject> tileset;
        Dictionary<int, GameObject> tile_groups;
        public GameObject prefab_floor;
        public GameObject prefab_mineral;
        public GameObject prefab_dirt;
        public GameObject prefab_wall;
        public GameObject prefab_water;
        public GameObject prefab_gas;
        public GameObject prefab_ash_dirt;

        int map_width = 64;
        int map_height = 64;

        List<List<int>> noise_grid = new List<List<int>>();
        List<List<GameObject>> tile_grid = new List<List<GameObject>>();
        

        int x_offset = 0; 
        int y_offset = 0; 

        void Update()
        {

        }

        void Start()
        {
            CreateTileset();
            CreateTileGroups();
            GenerateMap();
        }

        void CreateTileset()
        {
            tileset = new Dictionary<int, GameObject>();
            tileset.Add(0, prefab_floor);
            tileset.Add(1, prefab_dirt);
            tileset.Add(2, prefab_ash_dirt);
            tileset.Add(3, prefab_wall);
            tileset.Add(4, prefab_water);
            tileset.Add(5, prefab_mineral);
        }

        void CreateTileGroups()
        {

            tile_groups = new Dictionary<int, GameObject>();
            foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
            {
                GameObject tile_group = new GameObject(prefab_pair.Value.name);
                tile_group.transform.parent = gameObject.transform;
                tile_group.transform.localPosition = new Vector3(0, 0, 0);
                tile_groups.Add(prefab_pair.Key, tile_group);
            }
        }

        void GenerateMap()
        {

            for (int x = 0; x < map_width; x++)
            {
                noise_grid.Add(new List<int>());
                tile_grid.Add(new List<GameObject>());

                for (int y = 0; y < map_height; y++)
                {
                    int tile_id = GetIdUsing(x, y);
                   
                        noise_grid[x ].Add(tile_id);
                        CreateTile(tile_id, x , y);
                }
            }
        }


        int GetIdUsingPerlin(int x, int y)
        {
            //metoda PerlinNoise z Unity3D
            float raw_perlin = Mathf.PerlinNoise(
                (x - x_offset) / magnification,
                (y - y_offset) / magnification
            );
            Debug.Log(raw_perlin);
            float clamp_perlin = Mathf.Clamp01(raw_perlin);
            float scaled_perlin = clamp_perlin * tileset.Count;

            if (scaled_perlin == tileset.Count)
            {
                scaled_perlin = (tileset.Count - 1);
            }
            return Mathf.FloorToInt(scaled_perlin);
        }

        int GetIdUsing(int x, int y)
        {
            INoise noise = GetNoise();

            FractalNoise fractal = new FractalNoise(noise, octaves, frequency);

            float fx = x / magnification;
            float fy = y / magnification;
            float raw_perlin = fractal.Sample2D(fx, fy);

           
            switch (noiseType)
            {
                case NOISE_TYPE.PERLIN:
                    if (raw_perlin < 0.01) return 5;
                    else if (raw_perlin < 0.5) return 0;
                    else if (raw_perlin < 0.6) return 1;
                    else if (raw_perlin < 0.7) return 2;
                    else if (raw_perlin < 0.8) return 3;
                    else return 4;

                case NOISE_TYPE.VALUE:
                    if (raw_perlin < 0.01) return 5;
                    else if (raw_perlin < 0.5) return 0;
                    else if (raw_perlin < 0.6) return 1;
                    else if (raw_perlin < 0.7) return 2;
                    else if (raw_perlin < 0.8) return 3;
                    else return 4;

                case NOISE_TYPE.SIMPLEX:
                    if (raw_perlin < 0.01) return 5;
                    else if (raw_perlin < 0.5) return 0;
                    else if (raw_perlin < 0.6) return 1;
                    else if (raw_perlin < 0.7) return 2;
                    else if (raw_perlin < 0.8) return 3;
                    else return 4;

                case NOISE_TYPE.VORONOI:
                    if (raw_perlin < 0.51) return 5;
                    else if (raw_perlin < 0.55) return 0;
                    else if (raw_perlin < 0.57) return 1;
                    else if (raw_perlin < 0.58) return 2;
                    else if (raw_perlin < 0.60) return 3;
                    else return 4;

                case NOISE_TYPE.WORLEY:
                    if (raw_perlin < 0.565) return 5;
                    else if (raw_perlin < 0.76) return 0;
                    else if (raw_perlin < 0.80) return 1;
                    else if (raw_perlin < 0.92) return 2;
                    else if (raw_perlin < 0.99) return 3;
                    else return 4;

                default:
                    if (raw_perlin < 0.01) return 5;
                    else if (raw_perlin < 0.5) return 0;
                    else if (raw_perlin < 0.6) return 1;
                    else if (raw_perlin < 0.7) return 2;
                    else if (raw_perlin < 0.8) return 3;
                    else return 4;
            }
        }

        private INoise GetNoise()
        {
            
            switch (noiseType)
            {
                case NOISE_TYPE.PERLIN:
                    return new PerlinNoise(seed, 20);

                case NOISE_TYPE.VALUE:
                    return new ValueNoise(seed, 20);

                case NOISE_TYPE.SIMPLEX:
                    return new SimplexNoise(seed, 20);

                case NOISE_TYPE.VORONOI:
                    return new VoronoiNoise(seed, 20);

                case NOISE_TYPE.WORLEY:
                    return new WorleyNoise(seed, 20, 1.0f);

                default:
                    return new PerlinNoise(seed, 20);
            }
        }



        void CreateTile(int tile_id, int x, int y)
        {

            GameObject tile_prefab = tileset[tile_id];
            GameObject tile_group = tile_groups[tile_id];
            GameObject tile = Instantiate(tile_prefab, tile_group.transform);

            tile.name = string.Format("tile_x{0}_y{1}", x, y);
            tile.transform.localPosition = new Vector3(x, y, 0);

            tile_grid[x].Add(tile);
        }
    }

}