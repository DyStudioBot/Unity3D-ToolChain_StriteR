using UnityEngine;
using System.Collections.Generic;

public static class UBoundsIncrement
{
    static Vector3 m_BoundsMin;
    static Vector3 m_BoundsMax;
    public static void Begin()
    {
        m_BoundsMin = Vector3.zero;
        m_BoundsMax = Vector3.zero;
    }
    public static void Iterate(Vector3 vertex)
    {
        m_BoundsMin = Vector3.Min(m_BoundsMin, vertex);
        m_BoundsMax = Vector3.Max(m_BoundsMax, vertex);
    }
    public static void Iterate(Bounds bounds)
    {
        m_BoundsMin = Vector3.Min(m_BoundsMin, bounds.min);
        m_BoundsMax = Vector3.Max(m_BoundsMax, bounds.max);
    }
    
    public static Bounds End() => MinMax(m_BoundsMin, m_BoundsMax);
    
    public static Bounds MinMax(Vector3 _min,Vector3 _max)=>new Bounds((_min + _max) / 2, _max - _min);
    
    public static Bounds Process(IEnumerable<Vector3> _vertices)
    {
        Begin();
        foreach (var vertex in _vertices)
            Iterate(vertex);
        return End();
    }
}
