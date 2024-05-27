using Runtime.Geometry;
using Runtime.Geometry.Extension;
using Unity.Mathematics;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace Examples.Algorithm.Geometry
{
    public class GeometryIntersectPoint : MonoBehaviour
    {
        [Header("Ray & Point")]
        public GRay m_Ray;
        public Vector3 m_Point;
        [Header("Ray & Line")]
        public GRay m_Ray1;
        public GLine m_Line1;
        [Header("Ray & Ray")]
        public GRay m_Ray20;
        public GRay m_Ray21;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(m_Point, .1f);
            float rayPointProjection= m_Ray.Projection(m_Point);
            m_Ray.ToLine(rayPointProjection).DrawGizmos();
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_Ray.GetPoint(rayPointProjection),.1f);

            Gizmos.color = Color.white;
            var lineRayProjections = m_Ray1.Projection(m_Line1);
            m_Line1.DrawGizmos();
            m_Ray1.ToLine(lineRayProjections.y).DrawGizmos();
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_Line1.GetPoint(lineRayProjections.x), .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_Ray1.GetPoint(lineRayProjections.y), .1f);

            Gizmos.color = Color.white;
            var rayrayProjections = m_Ray20.Projection(m_Ray21);
            m_Ray20.ToLine(rayrayProjections.x).DrawGizmos();
            m_Ray21.ToLine(rayrayProjections.y).DrawGizmos();
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_Ray20.GetPoint(rayrayProjections.x), .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_Ray21.GetPoint(rayrayProjections.y), .1f);
        }
#endif
    }
}