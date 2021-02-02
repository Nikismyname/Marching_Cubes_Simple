﻿namespace Assets.Scripts.MarchingCubes
{
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;
    using Assets.Scripts.MarchingCubesMine;
    using Assets.Cameras;

    public class Main : MonoBehaviour
    {
        private List<Chunk> chunks = new List<Chunk>();
        /// <summary>
        /// ALL vertecies
        /// </summary>
        private float[,,] verts;
        /// <summary>
        /// the dimensions of each individual chunk - again we are sticking to cubes
        /// </summary>
        private int chunkSize;
        /// <summary>
        /// the dimensions of chunk grid - cubes again 
        /// if say 3 - this means we will have 9 chunks 
        /// </summary>
        private int chunkGridSize;
        /// <summary>
        /// how many vertecies there are in each dimension, we are working with cubes here
        /// if chunkSize is 3 and chunkGridSize is 2 = this means we will have 4 chunks with 3 x 3 x 3 vertecies each 
        /// the length in each dimesion would be 3 * 2 = 6 
        /// </summary>
        private int length;

        private void Start()
        {
            this.chunkSize = 16;
            this.chunkGridSize = 3;
            this.length = chunkGridSize * chunkSize;

            /// setting up camera
            GameObject camTarget = new GameObject("Cam Target");
            camTarget.transform.position = new Vector3((float)length / 2, (float)length / 2, (float)length / 2);
            Camera cam = Camera.main;
            var cs = cam.gameObject.AddComponent<Standard>();
            cs.target = camTarget.transform;
            ///...
            
            this.verts = new float[length, length, length];

            /// manipulata this to get different scale of random noise
            float randomNoiseMulty = 0.05f;

            /// generating the initial values based on some 3d noise
            for (int x = 0; x < length; x++)
                for (int y = 0; y < length; y++)
                    for (int z = 0; z < length; z++)
                        verts[x, y, z] = Perlin.Noise(x * randomNoiseMulty, y * randomNoiseMulty, z * randomNoiseMulty);

            /// generating the chunks themselves
            for (int x = 0; x < chunkGridSize; x++)
                for (int y = 0; y < chunkGridSize; y++)
                    for (int z = 0; z < chunkGridSize; z++)
                        chunks.Add(new Chunk(new int3(x * chunkSize, y * chunkSize, z * chunkSize), new int3(chunkSize, chunkSize, chunkSize), new int3(x, y, z), length));

            this.GenerateMesh();
        }

        /// <summary>
        /// If chunksToUpdate is null - updates all chunks, otherwise only updates selected chunks 
        /// </summary>
        /// <param name="chunksToUpdate"> chunk coordinates to update </param>
        private void GenerateMesh(HashSet<int3> chunksToUpdate = null)
        {
            if (chunksToUpdate == null)
            {
                foreach (Chunk chunk in this.chunks)
                {
                    chunk.ApplyMeshChanges(this.verts);
                }
            }
            else
            {
                foreach (Chunk chunk in this.chunks)
                {
                    if (chunksToUpdate.Contains(chunk.GridIndecies))
                    {
                        chunk.ApplyMeshChanges(this.verts);
                    }
                }
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                int directionMulty = Input.GetKey(KeyCode.LeftShift) ? -1 : 1;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit info))
                {
                    Vector3 point = info.point;

                    float xMin = point.x - 3;
                    float xMax = point.x + 3;

                    float yMin = point.y - 3;
                    float yMax = point.y + 3;

                    float zMin = point.z - 3;
                    float zMax = point.z + 3;

                    List<int3> affectedVerts = new List<int3>();
                    for (int x = (int)math.floor(xMin); x <= (int)math.ceil(xMax); x++)
                        for (int y = (int)math.floor(yMin); y <= (int)math.ceil(yMax); y++)
                            for (int z = (int)math.floor(zMin); z <= (int)math.ceil(zMax); z++)
                                if ((x >= xMin && x <= xMax) && (y >= yMin && y <= yMax) && (z >= zMin && z <= zMax))
                                    if ((x >= 0 && x <= this.length - 1) && (y >= 0 && y <= this.length - 1) && (z >= 0 && z <= this.length - 1))
                                        if (math.distance(new int3(x, y, z), point) < 3)
                                            affectedVerts.Add(new int3(x, y, z));

                    HashSet<int3> chunkByGridIndex = new HashSet<int3>();

                    foreach (var vert in affectedVerts)
                    {
                        /// offsetting the vertecies scalar value with amount proportianal to the distance so that we get a spherical extrusion
                        float dist = math.distance(vert, point); /// TODO: Optimize the calculation of dist to once - it is being calculated twice in this method!
                        float change = 3 / dist * directionMulty * Time.deltaTime;
                        float newAmount = verts[vert.x, vert.y, vert.z] - change;
                        newAmount = math.clamp(newAmount, -1f, 1f);
                        verts[vert.x, vert.y, vert.z] = newAmount;
                        
                        /// collecting all the unique grid positions in which a vertex has changed so we can selectively update those only
                        int x = (int)math.floor((float)vert.x / this.chunkSize);
                        int y = (int)math.floor((float)vert.y / this.chunkSize);
                        int z = (int)math.floor((float)vert.z / this.chunkSize);
                        chunkByGridIndex.Add(new int3(x, y, z));
                    }

                    GenerateMesh(chunkByGridIndex);
                }
            }
        }
    }
}