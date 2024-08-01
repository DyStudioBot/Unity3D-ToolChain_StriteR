using UnityEngine;
namespace TechToys.ThePlanet.Module.BOIDS.Bird
{

    public class FBirdConfig:ScriptableObject
    {
        public FBoidsMeshAnimationConfig animConfig;
        
        [Header("Flying")]
        public BoidsStartleConfig startleConfig;
        public BoidsFlyingConfig flyingConfig;
        public BoidsPreLandingConfig hoveringConfig;
        public BoidsLandingConfig landConfig;
        [Header("_Additional")]
        public BoidsFlockingConfig flockingConfig;
        public BoidsFollowingConfig m_FollowingConfig;
        public BoidsEvadeConfig evadeConfig;

        [Header("Perching")]
        public BoidsPerchConfig perchConfig;
        
        #if UNITY_EDITOR
        public void DrawGizmos()
        {
            flyingConfig.DrawGizmos();
        }
        #endif
    }
}