using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Geometry
{
    public partial struct GQuad
    {
        public Quad<float3> quad;
        // public Vector3 normal;
        // public float area;
        public GQuad(Quad<float3> _quad)
        {
            quad = _quad;
        }

        public static readonly GQuad kDefault = (GQuad)KQuad.k3SquareCentered;
    }
    
    [Serializable]
    public partial struct GQuad : IQuad<float3>,IIterate<float3>,IShape3D  , IConvex3D
    {
        public GQuad(float3 _vb, float3 _vl, float3 _vf, float3 _vr):this(new Quad<float3>(_vb,_vl,_vf,_vr)){}
        public GQuad((float3 _vb, float3 _vl, float3 _vf, float3 _vr) _tuple) : this(_tuple._vb, _tuple._vl, _tuple._vf, _tuple._vr) { }
        public static explicit operator GQuad(Quad<float3> _src) => new GQuad(_src);
        
        public float3 this[int _index] => quad[_index];
        public float3 this[EQuadCorner _corner] => quad[_corner];
        public float3 B => quad.B;
        public float3 L => quad.L;
        public float3 F => quad.F;
        public float3 R => quad.R;
        public int Length => quad.Length;

        public static GQuad operator +(GQuad _src, float3 _dst)=> new GQuad(_src.B + _dst, _src.L + _dst, _src.F + _dst,_src.R+_dst);
        public static GQuad operator -(GQuad _src, float3 _dst)=> new GQuad(_src.B - _dst, _src.L - _dst, _src.F - _dst,_src.R-_dst);
        public float3 GetPoint(float2 _uv)=>umath.bilinearLerp(B, L, F, R, _uv);
        public float3 GetSupportPoint(float3 _direction) => quad.Max(p => math.dot(p, _direction));
        public float3 Center => quad.Average();

        public IEnumerator<float3> GetEnumerator()
        {
            yield return quad.B;
            yield return quad.L;
            yield return quad.F;
            yield return quad.R;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void GetTriangles(out GTriangle _triangle1, out GTriangle _triangle2)
        {
            _triangle1 = new GTriangle(B, L, F);
            _triangle2 = new GTriangle(B, F, R);
        }

        public IEnumerable<GTriangle> GetTriangles()
        {
            yield return new GTriangle(B, L, F);
            yield return new GTriangle(B, F, R);
        }

        public IEnumerable<GLine> GetEdges()
        {
            yield return new GLine(quad.B, quad.L);
            yield return new GLine(quad.L, quad.F);
            yield return new GLine(quad.F, quad.R);
            yield return new GLine(quad.R, quad.B);
        }

        public IEnumerable<float3> GetAxes() => GetTriangles().Select(p => p.normal);

        public IEnumerable<float3> GetNormals()
        {
            var triangle1 = new GTriangle(B, L, F);
            var triangle2 = new GTriangle(B, F, R);

            var initialNormal = triangle1.normal;
            var midNormal = (triangle1.normal + triangle2.normal)/2;
            var finalNormal = triangle2.normal;
            yield return initialNormal;
            yield return midNormal;
            yield return finalNormal;
            yield return midNormal;
        }
    }

}