﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using Runtime.Geometry.Extension;
using Unity.Mathematics;

namespace Runtime.Geometry
{
    public struct GMesh : IVolume
    {
        public float3[] vertices;
        public int[] triangles;

        public GMesh(float3[] _vertices, int[] _triangles)
        {
            vertices = _vertices;
            triangles = _triangles;
            Center = default;
            Ctor();
        }
        
        public GMesh(IEnumerable<float3> _vertices, IEnumerable<int> _triangles):this(_vertices.ToArray(),_triangles.ToArray()) { }

        void Ctor()
        {
            Center = vertices.Average();
        }
        
        public float3 Center { get; set; }
        public float3 GetSupportPoint(float3 _direction)=> vertices.MaxElement(_p => math.dot(_direction, _p));

        public GBox GetBoundingBox() => UGeometry.GetBoundingBox(vertices);
        public GSphere GetBoundingSphere() => UGeometry.GetBoundingSphere(vertices);
    }
}