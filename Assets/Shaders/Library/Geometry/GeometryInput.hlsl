﻿//Ray
struct GRay
{
    float3 origin;
    float3 direction;
    float3 GetPoint(float _distance)  {  return origin + direction * _distance;  }
};
GRay GRay_Ctor(float3 _origin, float3 _direction)
{
    GRay ray;
    ray.origin = _origin;
    ray.direction = _direction;
    return ray;
}
struct GLine
{
    float3 start;
    float3 end;
    float3 direction;
    float length;
    float3 GetPoint(float _distance)  { return start + direction * _distance;  }
    GRay ToRay()
    {
        GRay ray;
        ray.origin = start;
        ray.direction = direction;
        return ray;
    }
};
GLine GLine_Ctor(float3 _origin, float3 _direction, float _length)
{
    GLine gline;
    gline.start = _origin;
    gline.direction = _direction;
    gline.length = _length;
    gline.end = _origin + _direction * _length;
    return gline;
}
GLine GLine_Ctor(float3 _origin, float3 _end)
{
    GLine gline;
    gline.start = _origin;
    gline.end = _end;
    float3 delta = _end - _origin;
    gline.length = length(delta);
    gline.direction = normalize(delta);
    return gline;
}
//Plane
struct GPlane
{
    float3 normal;
    float distance;
    float3 position;
};
GPlane GPlane_Ctor(float3 _normal, float _distance)
{
    GPlane plane;
    plane.normal = _normal;
    plane.distance = _distance;
    plane.position = plane.normal * plane.distance;
    return plane;
}
GPlane GPlane_Ctor(float3 _normal, float3 _position)
{
    GPlane plane;
    plane.normal = _normal;
    plane.position = _position;
    plane.distance = dot(_position, _normal);
    return plane;
}
//Sphere
struct GSphere
{
    float3 center;
    float radius;
};
GSphere GSphere_Ctor(float3 _center, float _radius)
{
    GSphere sphere;
    sphere.center = _center;
    sphere.radius = _radius;
    return sphere;
}
//Box
struct GBox
{
    float3 center;
    float3 size;
    float3 extent;
    float3 boxMin;
    float3 boxMax;
    float3 GetNormalizedPoint(float3 _point) { return(_point - boxMin)/size;}
};
GBox GBox_Ctor(float3 _center, float3 _size)
{
    GBox box;
    box.center=_center;
    box.size=_size;
    box.extent=_size*.5;
    box.boxMin = _center-box.extent;
    box.boxMax = _center+box.extent;
    return box;
}
GBox GBox_Ctor_Extent(float3 _center, float3 _extent)
{
    GBox box;
    box.center=_center;
    box.size= _extent * 2;
    box.extent= _extent ;
    box.boxMin = _center-box.extent;
    box.boxMax = _center+box.extent;
    return box;
}
struct GBoxRound
{
    GBox box;
    float roundness;
};
GBoxRound GRoundBox_Ctor(float3 _center,float3 _size,float _roundness)
{
    GBoxRound roundBox;
    roundBox.box=GBox_Ctor(_center,_size-_roundness*2);
    roundBox.roundness=_roundness;
    return roundBox;
}
struct GBoxFrame
{
    GBox box;
    float frameExtend;
};
GBoxFrame GFrameBox_Ctor(float3 _center,float3 _size,float _frameExtend)
{
    GBoxFrame frameBox;
    frameBox.box=GBox_Ctor(_center,_size);
    frameBox.frameExtend=_frameExtend;
    return frameBox;
}
//Torus
struct GTorus
{
    float3 center;
    float majorRadius;
    float minorRadius;
};
GTorus GTorus_Ctor(float3 _center,float _majorRadius,float _minorRadius)
{
    GTorus torus;
    torus.center=_center;
    torus.majorRadius=_majorRadius;
    torus.minorRadius=_minorRadius;
    return torus;
}
struct GTorusLink
{
    GTorus torus;
    float extend;
};
GTorusLink GTorusLink_Ctor(float3 _center,float _majorRadius,float _minorRadius,float _extend)
{
    GTorusLink torusLink;
    torusLink.torus=GTorus_Ctor(_center,_majorRadius,_minorRadius);
    torusLink.extend=_extend;
    return torusLink;
}
struct GTorusCapped
{
    GTorus torus;
    float2 capRadianSinCos;
};
GTorusCapped GTorusCapped_Ctor(float3 _center,float _majorRadius,float _minorRadius,float2 _capRadianSinCos)
{
    GTorusCapped torusCapped;
    torusCapped.torus=GTorus_Ctor(_center,_majorRadius,_minorRadius);
    torusCapped.capRadianSinCos=_capRadianSinCos;
    return torusCapped;
}

//GCylinder
struct GCylinder
{
    float3 center;
    float radius;
};
GCylinder GCylinder_Ctor(float3 _center,float _radius)
{
    GCylinder cylinder;
    cylinder.center=_center;
    cylinder.radius=_radius;
    return cylinder;
}
struct GCylinderCapped
{
    GCylinder cylinder;
    float height;
};
GCylinderCapped GCylinderCapped_Ctor(float3 _center,float _radius,float _height)
{
    GCylinderCapped cylinderCapped;
    cylinderCapped.cylinder=GCylinder_Ctor(_center,_radius);
    cylinderCapped.height=_height;
    return cylinderCapped;
}
struct GCylinderRound
{
    GCylinder cylinder;
    float height;
    float roundRadius;
};
GCylinderRound GCylinderRound_Ctor(float3 _center,float _radius,float _height,float _roundRadius)
{
    GCylinderRound cylinderRound;
    cylinderRound.cylinder=GCylinder_Ctor(_center,_radius);
    cylinderRound.height=_height;
    cylinderRound.roundRadius=_roundRadius;
    return cylinderRound;
}
struct GCapsule
{
    float3 direction;
    float height;
    float radius;
    float3 top;
    float3 bottom;
};
GCapsule GCapsule_Ctor(float3 _center,float _radius,float3 _direction,float _height)
{
    GCapsule capsule;
    capsule.direction=_direction;
    capsule.height=_height;
    capsule.radius = _radius;
    float3 offset=_direction*capsule.height*.5;
    capsule.top=_center+offset;
    capsule.bottom=_center-offset;
    return capsule;
}

struct GCone
{
    float3 origin;
    float3 normal;
    float sqrCosA;
    float tanA;
};
GCone GCone_Ctor(float3 _origin, float3 _normal, float _angle)
{
    GCone cone;
    cone.origin = _origin;
    cone.normal = _normal;
    float radianA = _angle / 360. * PI;
    float cosA = cos(radianA);
    cone.sqrCosA = cosA * cosA;
    cone.tanA = tan(radianA);
    return cone;
}

struct GHeightCone
{
    float3 origin;
    float3 normal;
    float sqrCosA;
    float tanA;
    float height;
    float3 bottom;
    float bottomRadius;
    GPlane bottomPlane;
    float GetRadius(float _height)
    {
        return _height * tanA;
    }
};
GHeightCone GHeightCone_Ctor(float3 _origin, float3 _normal, float _angle, float _height)
{
    GHeightCone cone;
    cone.origin = _origin;
    cone.normal = _normal;
    cone.height = _height;
    float radianA = _angle / 360. * PI;
    float cosA = cos(radianA);
    cone.sqrCosA = cosA * cosA;
    cone.tanA = tan(radianA);
    cone.bottom = _origin + _normal * _height;
    cone.bottomRadius = _height *cone. tanA;
    cone.bottomPlane = GPlane_Ctor(_normal, cone.bottom);
    return cone;
}

struct GAxis
{
    float3 origin;
    float3 right;
    float3 forward;
    float3 up;
    float distance;
    
    float2 GetUV(float3 _point)
    {
        float3 v0 = right;
        float3 v1 = forward;
        float3 v2 = _point - origin;
        float dot00 = dot(v0, v0);
        float dot01 = dot(v0, v1);
        float dot02 = dot(v0, v2);
        float dot11 = dot(v1, v1);
        float dot12 = dot(v1, v2);

        float denominator = (dot00 * dot11 - dot01 * dot01);
    
        float u = (dot11 * dot02 - dot01 * dot12) / denominator;
        float v = (dot00 * dot12 - dot01 * dot02) / denominator;

        return float2(u,v);
    }
};

GAxis GAxis_Ctor(float3 _origin, float3 _right, float3 _forward)
{
    GAxis axis;
    axis.origin = _origin;
    axis.right = _right;
    axis.forward = _forward;
    axis.up = cross(axis.right, axis.forward);
    axis.distance = dot(_origin, axis.up);
    return axis;
}

GAxis GAxis_Ctor(float3 _origin, float3 _right, float3 _up,float3 _forward)
{
    GAxis axis;
    axis.origin = _origin;
    axis.right = _right;
    axis.up = _up;
    axis.forward = _forward;
    axis.distance = dot(_origin, axis.forward);
    return axis;
}