namespace Assets.Scripts.MarchingCubesMine
{
    using Assets.Scripts.MarchingCubes;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Mathematics;
    using UnityEngine;

    public class Chunk
    {
        /// <summary>
        /// the indecies of the chunk, NOT the indecies of the top left near vertex that is <see cref="tlnv">
        /// this indecies are used to identify the chunk when we decide which meshes need updating after deformation 
        /// </summary>
        public int3 GridIndecies { get; private set; }

        /// <summary>
        /// top left near vertex coordinates for this chunk;
        /// </summary>
        private int3 tlnv;
        /// <summary>
        /// how many vertecies there are in the three dimentions, for now I have it as a cube always
        /// </summary>
        private int3 dimensions;
        /// <summary>
        /// the number of vertecies for the whole grid; the gris is composed of Chunks and represents the entire "play field"
        /// </summary>
        private int wholeGridLength;

        /// <summary>
        /// this is what we assign the meshes to, once we have generated them
        /// </summary>
        private MeshFilter meshFilter;
        /// <summary>
        /// we need collider so we can cast rays to the mouse location and deform the terrain
        /// </summary>
        private MeshCollider meshCollider;

        public Chunk(int3 topLeftNearVertex, int3 dimensions, int3 gridIndecies, int wholeGridLength)
        {
            this.tlnv = topLeftNearVertex;
            this.dimensions = dimensions;
            this.GridIndecies = gridIndecies;
            this.wholeGridLength = wholeGridLength;

            /// creating a gameobject that will hold the mesh for this chunk
            var meshy = new GameObject($"Meshy{topLeftNearVertex.x}{topLeftNearVertex.y}{topLeftNearVertex.z}");
            MeshRenderer meshRenderer = meshy.AddComponent<MeshRenderer>();
            /// setting the material with Wireframe shader, you can pick a different shader or material
            meshRenderer.sharedMaterial = new Material(Shader.Find("VR/SpatialMapping/Wireframe"));
            /// adding mesh filter and collider. We assign the generated meshed to those fields to get visualisation and physics respectivelly
            /// the phisics are necessary for raycasting to work and we need this for defermation on mouse click
            this.meshFilter = meshy.AddComponent<MeshFilter>();
            this.meshCollider = meshy.AddComponent<MeshCollider>();
        }

        /// <summary>
        /// Regenerate the mesh and assigns it to the chunk object so it is vissible
        /// </summary>
        /// <param name="isoValues"> scalar values for EVERY sampled point/vertex not just chunk vertecies </param>
        public void ApplyMeshChanges(float[,,] isoValues)
        {
            Mesh m = ProcessCubes(isoValues, 0, true);
            this.meshFilter.mesh = m;
            this.meshCollider.sharedMesh = m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isoValues"> scalar values for EVERY sampled point/vertex not just chunk vertecies </param>
        /// <param name="isoValue"> the current isovalue </param>
        /// <param name="doubleSided"> it is a bit weird when you look at single sided perlin noise mesh, that's why I left the option for doublesided </param>
        /// <returns></returns>
        private Mesh ProcessCubes(float[,,] isoValues, float isoValue, bool doubleSided = false)
        {
            /// this lists will collect the verts and tris for all unit cubes in this chunk
            List<int> tris = new List<int>();
            List<Vector3> verts = new List<Vector3>();

            /// loop over the vertecies that belong to this chunk - keep in mind that isoValues are the values for all chunks, not just current one
            for (int z = this.tlnv.z; z < this.tlnv.z + this.dimensions.z; z++)
            {
                for (int y = this.tlnv.y; y < this.tlnv.y + this.dimensions.y; y++)
                {
                    for (int x = this.tlnv.x; x < this.tlnv.x + this.dimensions.x; x++)
                    {
                        /// since we go by the top left near vertex for each cube, we do not process cubes for vertecies that are the very right, bottom or far index for the Entire cube.
                        /// If they are are the very right, bottom or far index for the current chunk that is fine
                        if (x == this.wholeGridLength - 1 || y == this.wholeGridLength - 1 || z == this.wholeGridLength - 1)
                        {
                            continue;
                        }

                        /// generates data for given Cube
                        this.ProcessCube(isoValues, isoValue, new int3(x, y, z), tris, verts);
                    }
                }
            }

            Mesh mesh = new Mesh();

            if (doubleSided)
            {
                int trisCount = tris.Count;
                for (int i = 0; i < trisCount; i += 3)
                {
                    int one = tris[i];
                    int two = tris[i + 1];
                    int three = tris[i + 2];

                    tris.Add(two);
                    tris.Add(one);
                    tris.Add(three);
                }
            }

            /// applying out information to the mesh
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// This generates the vertecies(int the mesh sence, not the vertecies with isovalues) and the triangles necessary to generate the mesh for current chunk
        /// </summary>
        /// <param name="isoValues"> scalar values for EVERY sampled point/vertex not just chunk vertecies </param>
        /// <param name="isoValue"> the current isovalue </param>
        /// <param name="topLeftNear"> the top left near for the given Unit cube </param>
        /// <param name="trisFinal"> list that contains all triangles for this chunk's mesh </param>
        /// <param name="vertsFinal"> list that contains all vertecies for this chunk's mesh </param>
        private void ProcessCube(float[,,] isoValues, float isoValue, int3 topLeftNear, List<int> trisFinal, List<Vector3> vertsFinal)
        {
            /// TODO: might want to reuse those, no need to generate UnitCubes each time
            UnitCube cube = new UnitCube(topLeftNear);

            /// get the cube index that we can use make lookups in the tables with
            int cubeIndex = cube.GetCubeIndex(isoValues, isoValue); ;

            /// use the index to get all needed triangles for the given unitCube
            int[] trisRow = LookupTables.TriTable[cubeIndex];

            /// tris will contain the actual mesh triangles for current unitCube
            /// currently what we get from the lookup table are tris who have index of an edge and not 
            /// the index in the mesh vertecies array of a given index. This has to be fixed after 
            /// we get our vertecies in order a couple of lines down.
            List<int> tris = new List<int>();
            for (int i = 0; i < trisRow.Length; i++)
            {
                /// -1 means we do not have any more tris for this unitCube
                if (trisRow[i] != -1)
                {
                    tris.Add(trisRow[i]);
                }
                else
                {
                    break;
                }
            }

            /// this is supposed to be done by a lookup in the edges table, but since all edges indecies are in the tris table currently, I just grab them from there
            int[] edgeIndeciesList = tris.Distinct().OrderBy(x => x).ToArray();

            /// the mesh vertecies are generated by interpolating between the two vertecies to which the edge is connected
            /// notice that for edgeIndeciesList and verteciesList the indexies remain the same, this is important on the next line
            Vector3[] verteciesList = edgeIndeciesList.Select(x => cube.Interpolate(x, isoValues, 0, true)).ToArray();

            /// here we replace inside the tris the edge index with the position of said edge index in edgeIndeciesList
            /// since the index in edgeIndeciesList corresponds to the index in verteciesList, this means that the tris are
            /// accurately referencing the mesh vertecies indecies as they should. If you do not know how meshes work -
            /// check out https://docs.unity3d.com/Manual/Example-CreatingaBillboardPlane.html
            tris = tris.Select(x => edgeIndeciesList.ToList().IndexOf(x)).ToList();

            /// getting how many mesh vertecies are already in the list 
            int vertCount = vertsFinal.Count;

            /// now this offset indecies from cube vert indecies to chunk vert indecies
            /// we are gnerating a mesh for the entire chunk, not only this unitCube
            tris = tris.Select(x => x + vertCount).ToList();

            /// append the collected data to the rest of chunk data
            vertsFinal.AddRange(verteciesList);
            trisFinal.AddRange(tris);
        }
    }
}
