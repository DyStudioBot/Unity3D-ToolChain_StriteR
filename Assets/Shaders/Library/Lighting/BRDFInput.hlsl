﻿#define DIELETRIC_SPEC half4(0.04,0.04,0.04,1.0-0.04)
struct BRDFSurface
{
    half3 diffuse;
    half3 specular;
    
    half metallic;
    half smoothness;
    half ao;
    
    half3 normal;
    half3 tangent;
    half3 viewDir;
    half3 reflectDir;
    half NDV;
    
    half grazingTerm;
    half fresnelTerm;
    half perceptualRoughness;
    half roughness;
    half roughness2;
};

BRDFSurface BRDFSurface_Ctor(half3 albedo, half smoothness, half metallic,half ao, half3 normal, half3 tangent, half3 viewDir)
{
    BRDFSurface surface;
    
    half oneMinusReflectivity = DIELETRIC_SPEC.a - metallic * DIELETRIC_SPEC.a;
    half reflectivity = 1.0 - oneMinusReflectivity;
				
    surface.diffuse = albedo * oneMinusReflectivity;
    surface.specular = lerp(DIELETRIC_SPEC.rgb, albedo, metallic);
    surface.metallic = metallic;
    surface.smoothness = smoothness;
    surface.ao = ao;
    
    surface.normal = normal;
    surface.tangent = tangent;
    surface.viewDir = viewDir;
    surface.reflectDir = normalize(reflect(-surface.viewDir, surface.normal));
    surface.NDV = max(0., dot(surface.normal,surface.viewDir));
    
    surface.grazingTerm = saturate(smoothness + reflectivity);
    surface.fresnelTerm=F_Schlick(surface.NDV);
    surface.perceptualRoughness = 1. - surface.smoothness;
    surface.roughness = max(HALF_MIN_SQRT, surface.perceptualRoughness * surface.perceptualRoughness);
    surface.roughness2 = max(HALF_MIN, surface.roughness * surface.roughness);
    return surface;
}


struct BRDFLight
{
    half3 color;
    half3 radiance;
    half3 lightDir;
    
    half normalDistribution;
    half invNormalizationTerm;
};

BRDFLight BRDFLight_Ctor(BRDFSurface surface, half3 lightDir, half3 lightCol, half3 lightAtten, half anisotropic)
{
    half3 viewDir = surface.viewDir;
    half3 normal = surface.normal;
    half3 tangent = surface.tangent;
    half3 halfDir = normalize(viewDir + lightDir);
    half glossiness = surface.smoothness;
    half roughness = surface.roughness;
    half sqrRoughness = surface.roughness2;
        
    half NDV = surface.NDV;
    half NDL = max(0., dot(normal, lightDir));
    half NDH = max(0., dot(normal, halfDir));
    half VDH = max(0., dot(viewDir, halfDir));
    half LDH = max(0., dot(lightDir, halfDir));
    half LDV = max(0., dot(lightDir, viewDir));
    
    BRDFLight light;
    light.lightDir = lightDir;
    light.color = lightCol;
    light.radiance = lightCol * lightAtten * NDL;
    light.normalDistribution =
#if _NDF_BLINNPHONG
                NDF_BlinnPhong(NDH, glossiness,max(1, glossiness *40));
#elif _NDF_COOKTORRANCE
                NDF_CookTorrance(NDH,LDH,roughness,sqrRoughness);
#elif _NDF_BECKMANN
                NDF_Beckmann(NDH,sqrRoughness);
#elif _NDF_GAUSSIAN
                NDF_Gaussian(NDH,sqrRoughness);
#elif _NDF_GGX
                NDF_GGX(NDH,roughness,sqrRoughness);
#elif _NDF_TROWBRIDGEREITZ
                NDF_TrowbridgeReitz(NDH,roughness,sqrRoughness);
#elif _NDF_ANISOTROPIC_TROWBRIDGEREITZ
                NDFA_TrowbridgeReitz(NDH, dot(halfDir, tangent), dot(halfDir, cross(normal,tangent)), anisotropic, glossiness);
#elif _NDF_ANISOTROPIC_WARD
                NDFA_Ward(NDL, NDV, NDH, dot(halfDir, tangent), dot(halfDir,  cross(normal,tangent)), anisotropic, glossiness);
#else
				0;
#endif
    
    light.invNormalizationTerm =
#if _VF_GGX
    InvVF_GGX(LDH,roughness);
#elif _VF_BLINNPHONG
    InvVF_BlinnPhong(LDH);
#else
    0;
#endif
    
    return light;
}
BRDFLight BRDFLight_Ctor(BRDFSurface surface, Light light,half anisotropic)
{
    return BRDFLight_Ctor(surface,light.direction,light.color,light.shadowAttenuation*light.distanceAttenuation,anisotropic);
}