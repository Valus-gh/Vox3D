using System.Collections.Generic;
using UnityEngine;

namespace Vox3D
{

    public class ChunkMesh
    {
        private Mesh _Mesh;

        private List<Vector3>   _Vertices;
        private List<int>       _Indices;
        private List<Color32>   _Colors;

        private MeshFilter      _Filter;
        private MeshRenderer    _Renderer;
        private MeshCollider    _Collider;

        public Mesh Mesh                { get => _Mesh; set => _Mesh = value; }
        public List<Vector3> Vertices   { get => _Vertices; set => _Vertices = value; }
        public List<int> Indices        { get => _Indices; set => _Indices = value; }
        public List<Color32> Colors     { get => _Colors; set => _Colors = value; }

        public MeshFilter Filter        { get => _Filter; set => _Filter = value; }
        public MeshRenderer Renderer    { get => _Renderer; set => _Renderer = value; }
        public MeshCollider Collider    { get => _Collider; set => _Collider = value; }

        public ChunkMesh()
        {
            Vertices    = new List<Vector3>();
            Indices     = new List<int>();
            Colors      = new List<Color32>();

            Filter      = new MeshFilter();
            Renderer    = new MeshRenderer();
            Collider    = new MeshCollider();
        }

        public void Dispose()
        {
            Vertices.Clear();
            Indices.Clear();
            Colors.Clear();
        }

    }

}