using Runtime.Geometry;
using TechToys.ThePlanet.Module;
using Unity.Mathematics;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace TechToys.ThePlanet.Baking
{
    public class ModuleByteVisualizer : MonoBehaviour
    {
        public bool m_DrawCluster;
        private static readonly Qube<float3> kUnitQube = KQuad.k3SquareCenteredUpward.ExpandToQube(kfloat3.up, .5f);
        private static readonly Qube<float3> kHalfUnitQube = kUnitQube.Resize(.5f);
        private static readonly Vector3 kHalfSize = Vector3.one * .45f;
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            int horizonztal = 0;
            int vertical = 0;
            for (int i = 1; i < 255; i++)
            {
                var translate = new float3(horizonztal * 2f, 0f, vertical * 2f);
                var qubeByte = (byte)i;

                var tuple = UModuleByte.kByteOrientation[qubeByte];


                if (tuple._orientation > 0)
                    continue;

                Gizmos.color = Color.white;
                Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Translate(translate);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                UGizmos.DrawString(qubeByte.ToString(), Vector3.zero, 0f);
                if (!m_DrawCluster)
                {
                    Qube<bool> qube = default;
                    qube.SetByteElement(qubeByte);
                    for (int j = 0; j < 8; j++)
                    {
                        Gizmos.color = (qube[j] ? Color.green : Color.red).SetA(.5f);
                        Gizmos.DrawWireSphere(kUnitQube[j], .1f);
                    }
                }
                else
                {
                    Gizmos.color = Color.green;
                    for (int j = 0; j < 8; j++)
                    {
                        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.TRS(translate + kHalfUnitQube[j], Quaternion.identity, kHalfSize);
                        Qube<bool> clusterUnit = default;
                        clusterUnit.SetByteElement(UModuleByte.kByteQubeIndexer[tuple._byte][j]);
                        for (int k = 0; k < 8; k++)
                        {
                            if (!clusterUnit[k])
                                continue;
                            Gizmos.DrawWireSphere(kUnitQube[k], .1f);
                        }
                    }
                }

                horizonztal++;
                if (horizonztal >= 16)
                {
                    horizonztal = 0;
                    vertical += 1;
                }
            }
#endif
        }
    }
}
