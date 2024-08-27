﻿using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Extensions
{
    public enum EMeshInspectMode
    {
        None,
        Vertices,
        Normals,
        Tangents,
        BiTangents,
        Colors,
    }
    
    [CustomEditor(typeof(MeshFilter)), CanEditMultipleObjects]
    public class MeshFilterInspector : Editor
    {
        private EMeshInspectMode m_Mode= EMeshInspectMode.None;
        MeshFilter m_Target;
        private Mesh m_SharedMesh;
        private Vector3[] m_vertices;
        private Vector3[] m_Normals;
        private Vector4[] m_Tangents;
        private Color[] m_Colors;
        
        void OnEnable()
        {
            m_Target = target as MeshFilter;
            
            m_SharedMesh= m_Target.sharedMesh;
        }

        private void OnDisable()
        {
            m_vertices = null;
            m_Normals = null;
            m_Tangents = null;
            m_Colors = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_SharedMesh==null||!m_SharedMesh.isReadable)
                return;

            EditorGUI.BeginChangeCheck();
            m_Mode = (EMeshInspectMode)EditorGUILayout.EnumPopup(m_Mode);
            if (EditorGUI.EndChangeCheck())
            {
                m_vertices = m_SharedMesh.vertices;
                m_Normals = m_SharedMesh.normals;
                m_Tangents = m_SharedMesh.tangents; //UModeling.RegenerateTangents(m_SharedMesh.GetPolygons(out var indices),m_Normals,m_vertices,m_SharedMesh.uv).Select(p=>(Vector4)p).ToArray();
                m_Colors = m_SharedMesh.colors;
            }
        }
        private void OnSceneGUI()
        {
            if (!m_Target||!m_SharedMesh)
                return;
            Handles.matrix = m_Target.transform.localToWorldMatrix;

            if (m_Mode == EMeshInspectMode.Vertices)
            {
                Handles.color = Color.white.SetA(.3f);
                foreach (var vertex in m_vertices)
                    UHandles.DrawWireSphere(vertex,Quaternion.identity, .05f);
            }
            else if (m_Mode == EMeshInspectMode.Normals& m_SharedMesh.HasVertexAttribute(VertexAttribute.Normal))
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Handles.color = Color.green.SetA(.5f);
                    Handles.DrawLine(m_vertices[i], m_vertices[i] + m_Normals[i] * .1f);
                }
            }
            else if (m_Mode == EMeshInspectMode.Tangents& m_SharedMesh.HasVertexAttribute(VertexAttribute.Tangent))
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Handles.color = Color.cyan.SetA(.5f);
                    Handles.DrawLine(m_vertices[i], m_vertices[i] + (Vector3)m_Tangents[i] * .1f);
                }
            }
            else if (m_Mode == EMeshInspectMode.BiTangents && m_SharedMesh.HasVertexAttribute(VertexAttribute.Normal) && m_SharedMesh.HasVertexAttribute(VertexAttribute.Tangent))
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Handles.color = Color.yellow.SetA(.5f);
                    Handles.DrawLine(m_vertices[i], m_vertices[i] + Vector3.Cross(m_Tangents[i],m_Normals[i]).normalized * .1f);
                }
            }
            else if (m_Mode == EMeshInspectMode.Colors && m_SharedMesh.HasVertexAttribute(VertexAttribute.Color))
            {
                for (int i = 0; i < m_vertices.Length; i++)
                {
                    Handles.color = m_Colors[i];
                    UHandles.DrawWireSphere(m_vertices[i],Quaternion.identity, .05f);
                }
            }
        }
    }
}