﻿using System;
using Unity.Mathematics;
using UnityEngine;
using static kmath;
using static Unity.Mathematics.math;
public enum EAxis
{
    X = 0,
    Y = 1,
    Z = 2,
}

public static partial class umath
{
    public static bool IsPrime(ushort _value)
    {
        for (ushort i = 2; i < _value; i++)
        {
            if (_value % i == 0)
                return false;
        }
        return true;
    }

    public static int Factorial(int n)
    {
        var result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
    
    public static ushort[] ComputePrimes(int _count)
    {
        ushort[] primes = new ushort[_count];
        ushort currentNum = 1;
        ushort curIndex = 0;
        while (curIndex < _count)
        {
            currentNum++;
            if (!IsPrime(currentNum))
                continue;
            primes[curIndex++] = currentNum;
        }
        return primes;
    }
    

    public static int pow(int _src, int _pow)
    {
        if (_pow == 0) return 1;
        if (_pow == 1) return _src;
        int dst = _src;
        for (int i = 0; i < _pow - 1; i++)
            dst *= _src;
        return dst;
    }

    public static uint pow(uint _src, uint _pow)
    {
        if (_pow == 0) return 1;
        if (_pow == 1) return _src;
        var dst = _src;
        for (int i = 0; i < _pow - 1; i++)
            dst *= _src;
        return dst;
    }

    public static int sqr(int _src) => _src * _src;
    public static float sqr(float _src) => _src * _src; public static float2 sqr(float2 _src) => _src * _src; public static float3 sqr(float3 _src) => _src * _src; public static float4 sqr(float4 _src) => _src * _src;
    public static float pow2(float _src) => _src * _src; public static float2 pow2(float2 _src) => _src * _src; public static float3 pow2(float3 _src) => _src * _src; public static float4 pow2(float4 _src) => _src * _src;
    
    public static float pow3(float _src) => _src * _src* _src; public static float2 pow3(float2 _src) => _src * _src* _src; public static float3 pow3(float3 _src) => _src * _src* _src; public static float4 pow3(float4 _src) => _src * _src* _src;
    
    
    public static float pow4(float _src) => _src * _src* _src* _src;
    public static float mod(float _src, float _dst) => _src - _dst * Mathf.Floor(_src/_dst);

    public static float lerp(float _a, float _b, float _value) => Mathf.Lerp(_a, _b, _value);
    
    public static float invLerp(float _a, float _b, float _value)=> (_value - _a) / (_b - _a);

    public static float3 trilerp(float3 _a, float3 _b, float3 _c, float _value)
    {
        if (_value < .5f)
            return math.lerp(_a, _b, _value * 2);
        return math.lerp(_b, _c, _value * 2 - 1f);
    }
    
    public static float angle(float3 a, float3 b)
    {
        var sqMagA = a.sqrmagnitude();
        var sqMagB = b.sqrmagnitude();
        if (sqMagB == 0 || sqMagA == 0)
            return 0;
            
        var dot = math.dot(a, b);
        if (abs(1 - sqMagA) < EPSILON && abs(1 - sqMagB) < EPSILON) {
            return acos(dot);
        }
 
        float length = sqrt(sqMagA) * sqrt(sqMagB);
        return acos(dot / length);
    }
    public static float3 slerp(float3 from, float3 to, float t,float3 up)
    {
        float theta = angle(from, to);
        float sin_theta = sin(theta);
        var dotValue = dot(from.normalize(), to.normalize());
        Debug.Log(dotValue);
        if (dotValue > .999f)
            return to;
        if(dotValue < -.999f)
            return trilerp(from, up,to, t);

        float a = sin((1 - t) * theta) / sin_theta;
        float b = sin(t * theta) / sin_theta;
        return from * a + to * b;
    }

    public static float3 nlerp(float3 _from, float3 _to, float t)
    {
        return normalize(math.lerp(_from,_to,t));
    }
    
    public static EAxis maxAxis(this float2 _value)
    {
        if (_value.x > _value.y)
            return EAxis.X;
        return EAxis.Y;
    }
    public static EAxis maxAxis(this float3 _value)
    {
        if (_value.x > _value.y && _value.x > _value.z)
            return EAxis.X;
        return _value.y > _value.z ? EAxis.Y : EAxis.Z;
    }
    
    public static float2 bilinearLerp(float2 tl, float2 tr, float2 br, float2 bl,float2 p)=> tl + (tr - tl) * p.x + (bl - tl) * p.y + (tl - tr + br - bl) * (p.x * p.y);
    public static float3 bilinearLerp(float3 tl, float3 tr, float3 br, float3 bl,float2 p)=> tl + (tr - tl) * p.x + (bl - tl) * p.y + (tl - tr + br - bl) * (p.x * p.y);

    public static float3 bilinearLerp(float3 tl, float3 tr, float3 br, float3 bl,float u,float v)=> tl + (tr - tl) * u + (bl - tl) * v + (tl - tr + br - bl) * (u * v);
    public static float2 bilinearLerp(float2 tl, float2 tr, float2 br, float2 bl,float u,float v)=> tl + (tr - tl) * u + (bl - tl) * v + (tl - tr + br - bl) * (u * v);
    public static float bilinearLerp(float tl, float tr, float br, float bl,float u,float v)=> tl + (tr - tl) * u + (bl - tl) * v + (tl - tr + br - bl) * (u * v);
    public static float2 invBilinearLerp(float2 tl, float2 tr, float2 br, float2 bl, float2 p)
    {
        var e = tr - tl;
        var f = bl - tl;
        var g = tl - tr + br - bl;
        var h = p - tl;
        var k2 = cross(g,f);
        var k1 = cross(e, f);
        var k0 = cross(h, e);
        if (Mathf.Abs(k2) > float.Epsilon)
        {
            float w = k1 * k1 - 4f * k0 * k2;
            if (w < 0f)
                return -Vector2.one;
            w = Mathf.Sqrt(w);
            float ik2 = .5f / k2;
            float v = (-k1 - w) * ik2;
            float u = (h.x - f.x * v) / (e.x + g.x * v);
            if (!RangeFloat.k01.Contains(u) || !RangeFloat.k01.Contains(v))
            {
                v = (-k1 + w) * ik2;
                u = (h.x - f.x * v) / (e.x + g.x * v);
            }
            return new Vector2(u,v);
        }
        else
        {
            float u=(h.x*k1+f.x*k0)/(e.x*k1-g.x*k0);
            float v = -k0 / k1;
            return new Vector2(u,v);
        }
    }

    public static float smoothLerp(float from,float to,float t)
    {
        t = -2.0f * t * t * t + 3.0f * t * t;
        return to * t + from * (1.0f - t);
    }
    
    public static Matrix4x4 add(this Matrix4x4 _src, Matrix4x4 _dst)
    {
        Matrix4x4 dst = Matrix4x4.identity;
        for(int i=0;i<4;i++)
            dst.SetRow(i,_src.GetRow(i)+_dst.GetRow(i));
        return dst;
    }
    
    public static int lerp(int _src, int _dst, float _interpolate)=> (int)math.lerp(_src, _dst, _interpolate);
    public static bool lerp(bool _src, bool _dst, float _interpolate)
    {
        if (Math.Abs(_interpolate - 1) < float.Epsilon)
            return _dst;
        if (_interpolate == 0)
            return _src;
        return _src || _dst;
    }

    
    public static float cosH(float _src) => (Mathf.Exp(_src) + Mathf.Exp(_src)) / 2;
    public static float copySign(float _a, float _b)
    {
        var signA = Mathf.Sign(_a);
        var signB = Mathf.Sign(_b);
        return Math.Abs(signA - signB) < float.Epsilon ? _a : _a * signB;
    }
    
    //Fast
    public static float negExp_Fast(float _x)
    {
        return 1.0f / (1.0f + _x + 0.48f * _x * _x + 0.235f * _x * _x * _x);
    }

    public static float atan_Fast(float _x)
    {
        float z = Mathf.Abs(_x);
        float w = z > 1f ? 1f / z : z;
        float y = (kPI / 4.0f) * w - w * (w - 1) * (0.2447f + 0.0663f * w);
        return copySign(z > 1 ? kPIDiv2 - y : y,_x);
    }

    static float sin_kinda(float _x)
    {
        float x2 = sqr(_x);
        float x3 = x2*_x;
        float x5 = x3 * x2;
        _x = _x - x3 / 6.0f + x5 / 120f;
        return _x;
    }

    public static float sin_basic_approximation(float _x)
    {
        int k = (int)math.floor(_x / kPIDiv2);
        float y = _x - k * kPIDiv2;
        switch (( k % 4+4) % 4)
        {
            default: throw new ArgumentNullException();
            case 0: return sin_kinda(y);
            case 1: return sin_kinda(kPIDiv2 - y);
            case 2: return -sin_kinda(y);
            case 3: return -sin_kinda(kPIDiv2 - y);
        }
    }
    
    private static readonly float2[] kAlphaSinCos = GenerateAlphaCosSin();
    static float2[] GenerateAlphaCosSin()
    {
        var alphaSinCos = new float2[256];
        for (int i = 0; i < 256; i++)
        {
            var angle = i * kPiDiv128;
            alphaSinCos[i] = new float2( math.cos(angle),math.sin(angle));
        }
        return alphaSinCos;
    }
    
    public static void sincos(float _f, out float _s, out float _c)
    {
        var a =math.abs(_f) * k128InvPi;
        var i = (int)math.floor(a);
        var b = (a - i) * kPiDiv128;
        var alphaCosSin = kAlphaSinCos[i&255];
        var b2 = b * b;
        var sine_beta = b - b * b2 * (0.1666666667F - b2 * 0.00833333333F);
        var cosine_beta = 1.0f - b2 * (0.5f - b2 * 0.04166666667F);

        var sine = alphaCosSin.y * cosine_beta + alphaCosSin.x * sine_beta;
        var cosine = alphaCosSin.x * cosine_beta - alphaCosSin.y * sine_beta;

        _s = _f < 0f ? -sine : sine;
        _c = cosine;
    }

    public static float2 tripleProduct(float2 _a, float2 _b, float2 _c) => _b *math.dot(_a, _c)  - _a * math.dot(_c, _b);
    public static float3 tripleProduct(float3 _a, float3 _b, float3 _c) => _b *math.dot(_a, _c)  - _a * math.dot(_c, _b);
    public static float repeat(float _t,float _length) => math.clamp(_t - math.floor(_t / _length) * _length, 0.0f, _length);
    public static float2 repeat(float2 _t,float2 _length) => math.clamp(_t - math.floor(_t / _length) * _length, 0.0f, _length);
    
    public static float deltaAngle(float _x,float _xd)
    {
        float num = repeat(_xd - _x, 360f);
        if (num > 180.0)
            num -= 360f;
        return num;
    }
    public static float2 deltaAngle(float2 _x, float2 _xd)
    {
        return new float2(
            deltaAngle(_x.x, _xd.x),
            deltaAngle(_x.y, _xd.y)
        );
    }

    public static float4 deltaAngle(float4 _x, float4 _xd)
    {
        return new float4(
            deltaAngle(_x.x, _xd.x),
            deltaAngle(_x.y, _xd.y),
            deltaAngle(_x.z, _xd.z),
            deltaAngle(_x.w, _xd.w)
        );
    }

}
