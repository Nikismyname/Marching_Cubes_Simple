namespace Assets.Scripts.MarchingCubesMine
{
    using Assets.Common;
    using System;
    using Unity.Mathematics;
    using UnityEngine;

    public class UnitCube
    {
        /// those 8 vertecies are the corners of the current unitCubel which is which is described in <see cref="UnitCube">
        public int3 Vertex0 { get; set; }

        public int3 Vertex1 { get; set; }

        public int3 Vertex2 { get; set; }

        public int3 Vertex3 { get; set; }

        public int3 Vertex4 { get; set; }

        public int3 Vertex5 { get; set; }

        public int3 Vertex6 { get; set; }

        public int3 Vertex7 { get; set; }

        /// <summary>
        /// The order is dictated by the paper I am following: http://paulbourke.net/geometry/polygonise/
        /// </summary>
        /// <param name="topLeftNearCorer"></param>
        public UnitCube(int3 topLeftNearCorer)
        {
            int3 v = topLeftNearCorer;
            this.Vertex0 = new int3(v.x, v.y + 1, v.z + 1);
            this.Vertex1 = new int3(v.x + 1, v.y + 1, v.z + 1);
            this.Vertex2 = new int3(v.x + 1, v.y + 1, v.z);
            this.Vertex3 = new int3(v.x, v.y + 1, v.z);
            this.Vertex4 = new int3(v.x, v.y, v.z + 1);
            this.Vertex5 = new int3(v.x + 1, v.y, v.z + 1);
            this.Vertex6 = new int3(v.x + 1, v.y, v.z);
            this.Vertex7 = v;
        }

        /// those are the edges which hold as a value the two vertecies they connect: order is dictated by same paper
        /// static since they are the same for every unitCube
        static int2 Edge0 = new int2(0, 1);
        static int2 Edge1 = new int2(1, 2);
        static int2 Edge2 = new int2(2, 3);
        static int2 Edge3 = new int2(3, 0);
        static int2 Edge4 = new int2(4, 5);
        static int2 Edge5 = new int2(5, 6);
        static int2 Edge6 = new int2(6, 7);
        static int2 Edge7 = new int2(7, 4);
        static int2 Edge8 = new int2(4, 0);
        static int2 Edge9 = new int2(5, 1);
        static int2 Edge10 = new int2(6, 2);
        static int2 Edge11 = new int2(7, 3);

        public int2 GetEdgeAtIndex(int index)
        {
            switch (index)
            {
                case 0: return Edge0;
                case 1: return Edge1;
                case 2: return Edge2;
                case 3: return Edge3;
                case 4: return Edge4;
                case 5: return Edge5;
                case 6: return Edge6;
                case 7: return Edge7;
                case 8: return Edge8;
                case 9: return Edge9;
                case 10: return Edge10;
                case 11: return Edge11;
                default: throw new Exception();
            }
        }

        public int3 GetVertAtIndex(int index)
        {
            switch (index)
            {
                case 0: return this.Vertex0;
                case 1: return this.Vertex1;
                case 2: return this.Vertex2;
                case 3: return this.Vertex3;
                case 4: return this.Vertex4;
                case 5: return this.Vertex5;
                case 6: return this.Vertex6;
                case 7: return this.Vertex7;
                default: throw new Exception();
            }
        }

        /// <summary>
        /// Same paper: http://paulbourke.net/geometry/polygonise/ describes how to get cubeIndex
        /// this index can then be used for lookup in lookup tables
        /// </summary>
        /// <param name="grid">the ENTIRE vertecies list</param>
        /// <param name="isolevel"> the current isolevel </param>
        /// <returns></returns>
        public int GetCubeIndex(float[,,] grid, float isolevel)
        {
            int cubeindex = 0;
            if (grid[Vertex0.x, Vertex0.y, Vertex0.z] < isolevel) cubeindex |= 1;
            if (grid[Vertex1.x, Vertex1.y, Vertex1.z] < isolevel) cubeindex |= 2;
            if (grid[Vertex2.x, Vertex2.y, Vertex2.z] < isolevel) cubeindex |= 4;
            if (grid[Vertex3.x, Vertex3.y, Vertex3.z] < isolevel) cubeindex |= 8;
            if (grid[Vertex4.x, Vertex4.y, Vertex4.z] < isolevel) cubeindex |= 16;
            if (grid[Vertex5.x, Vertex5.y, Vertex5.z] < isolevel) cubeindex |= 32;
            if (grid[Vertex6.x, Vertex6.y, Vertex6.z] < isolevel) cubeindex |= 64;
            if (grid[Vertex7.x, Vertex7.y, Vertex7.z] < isolevel) cubeindex |= 128;

            return cubeindex;
        }

        /// <summary>
        /// Here we have an edge connecting two vertecies - this method returns a 3d point somewhere on the edge.
        /// Naiv interpretation might be to just return the halfway point, but this results in much blockier looking surfaces.
        /// To calcualte exact position we have to find the proprtion of the isovalue of vertex one to the isovalue to the entire 
        /// difference between isovalues of both vertecies and lerp that proportion from vert one to vert two. 
        /// (the code is probably clearer on that an explanation)
        /// </summary>
        /// <param name="edgeIndex"></param>
        /// <param name="verts"></param>
        /// <param name="isovalue"></param>
        /// <param name="proper"></param>
        /// <returns></returns>
        public Vector3 Interpolate(int edgeIndex, float[,,] verts, float isovalue, bool proper = true)
        {
            int2 edge = this.GetEdgeAtIndex(edgeIndex);
            int3 vert1 = this.GetVertAtIndex(edge.x);
            int3 vert2 = this.GetVertAtIndex(edge.y);

            float lerpAmount = 0.5f;

            if (proper)
            {
                float scalar1 = verts[vert1.x, vert1.y, vert1.z];
                float scalar2 = verts[vert2.x, vert2.y, vert2.z];
                lerpAmount = Math.Abs(scalar1 - isovalue) / math.abs(scalar1 - scalar2);
            }

            return Vector3.Lerp(vert1.ToVec3(), vert2.ToVec3(), lerpAmount);
        }
    }
}
