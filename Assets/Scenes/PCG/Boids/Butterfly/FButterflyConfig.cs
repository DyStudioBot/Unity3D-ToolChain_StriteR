using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG.Module.BOIDS.Butterfly
{
    public enum EButterFlyBehaviour
    {
        Invalid=-1,
        
        Startle,
        Floating,
        
        Landing,
        Stopped,
    }

    
    [CreateAssetMenu(fileName = "Butterfly_Config",menuName = "Config/Boids/Butterfly")]
    public class FButterflyConfig:ScriptableObject
    {
        public FBoidsMeshAnimationConfig animConfig;
        
        [Header("Base")]
        // public BoidsStartleConfig startleConfig;
        public BoidsFloatingConfig floatingConfig;
        // public BoidsHoverLandingConfig landConfig;
        // public BoidsIdleConfig idleConfig;
        // [Header("Additional")]
        // public BoidsFlockingConfig flockingConfig;
    }
}