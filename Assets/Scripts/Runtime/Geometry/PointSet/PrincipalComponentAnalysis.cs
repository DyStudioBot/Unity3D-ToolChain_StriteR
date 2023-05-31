using Unity.Mathematics;
using UnityEngine;

namespace Geometry.PointSet
{
    
    
    public static class UPrincipleComponentAnalysis
    {
        public static void Evaluate(float3[] _points,out float3 _centre, out float3 _right, out float3 _up, out float3 _forward)
        {
            _right = kfloat3.right;
            _up = kfloat3.up;
            _forward = kfloat3.forward;

            var m = _points.Average();
            _centre = m;
            var a11 = _points.Average(p => umath.pow2(p.x - m.x));
            var a22 = _points.Average(p => umath.pow2(p.y - m.y));
            var a33 = _points.Average(p => umath.pow2(p.z - m.z));

            var a12mirror = _points.Average(p => (p.x - m.x)*(p.y-m.y));
            var a13mirror = _points.Average(p => (p.x - m.x)*(p.z-m.z));
            var a23mirror = _points.Average(p => (p.y - m.y)*(p.z-m.z));

            var C = new float3x3(a11,a12mirror,a13mirror,a12mirror,a22,a23mirror,a13mirror,a23mirror,a33);
            
            var c0 = C.c0; var c00 = c0.x; var c01 = c0.y; var c02 = c0.z;
            var c1 = C.c1; var c10 = c1.x; var c11 = c1.y; var c12 = c1.z;
            var c2 = C.c2; var c20 = c2.x; var c21 = c2.y; var c22 = c2.z;
            math.determinant(C);
            Debug.Log(C);
            
            var polynomial = new CubicPolynomial(-1,
                c00 + c11 + c22,
                -c00*c11 -c00*c22 + c12*c21 -c11*c22 +c10*c01 +c20*c02,
                -c00*c12*c21 + c00*c11*c22-c10*c01*c22 +c10*c02*c21+c20*c01*c12-c20*c02*c11);
            Debug.Log(polynomial.ToString());
        }
    }
    
    #region Notes
    internal static class Notes
    {
        public static float OutputPolynomial(float3x3 C,float λ)       //Jezz
        {         
            var c0 = C.c0; var c00 = c0.x; var c01 = c0.y; var c02 = c0.z;
            var c1 = C.c1; var c10 = c1.x; var c11 = c1.y; var c12 = c1.z;
            var c2 = C.c2; var c20 = c2.x; var c21 = c2.y; var c22 = c2.z;
            
            //Resolve determination parts
        var part1 = c00 * (c11 * c22 - c12 * c21);      //   c0.x * (c1.y * c2.z - c1.z * c2.y) 
            part1 = (c00 - λ) * ((c11-λ) * (c22-λ) - c12*c21);
            part1 = (c00 - λ) * (+c11*c22 -c11*λ - c22*λ     + λ*λ -c12*c21 );
            part1 = (c00 - λ) * (λ*λ      -c11*λ - c22*λ     -c12*c21 +c11*c22);
            part1 =        c00* (λ*λ      -c11*λ - c22*λ     -c12*c21 +c11*c22)
                           - λ* (λ*λ      -c11*λ - c22*λ     -c12*c21 +c11*c22);
            part1 =        c00*λ*λ   -c00*c11*λ - c00*c22*λ  -c00*c12*c21 + c00*c11*c22 
                            - λ*λ*λ      +c11*λ*λ + c22*λ*λ   +c12*c21*λ -c11*c22*λ;
            part1 = -λ*λ*λ 
                + c00*λ*λ +c11*λ*λ + c22*λ*λ
                -c00*c11*λ - c00*c22*λ +c12*c21*λ -c11*c22*λ
                -c00*c12*c21 + c00*c11*c22  ;
            part1 = -1*λ*λ*λ 
                + (c00 + c11 +c22)*λ*λ
                -c00*c11*λ -c00*c22*λ + c12*c21*λ -c11*c22*λ
                -c00*c12*c21 + c00*c11*c22;
            
        var part2 = -c10 * (c01 * c22 - c02 * c21);      // - c1.x * (c0.y * c2.z - c0.z * c2.y) 
            part2 = -c10 * (c01 * (c22-λ) - c02 * c21); 
            part2 = -c10 * (c01*c22  -c01*λ      -c02*c21); 
            part2 = -c10*c01*c22  +c10*c01*λ   +c10*c02*c21; 
            part2 = +c10*c01*λ    -c10*c01*c22 +c10*c02*c21; 
            
        var part3 =  + c20 * (c01 * c12 - c02 * c11);   // + c2.x * (c0.y * c1.z - c0.z * c1.y);
            part3 = c20 * (c01*c12 - c02 * (c11-λ));
            part3 = c20 * (c01*c12 - c02*c11   +c02*λ);
            part3 = +c20*c01*c12 -c20*c02*c11  +c20*c02*λ;
            part3 = +c20*c02*λ   +c20*c01*c12 -c20*c02*c11;

            var cubic = -1;
            var quadratic = c00 + c11 + c22;
            var linear = -c00*c11 -c00*c22 + c12*c21 -c11*c22 +c10*c01 +c20*c02;
            var constant = -c00*c12*c21 + c00*c11*c22-c10*c01*c22 +c10*c02*c21+c20*c01*c12-c20*c02*c11;
            
            return part1 + part2 + part3;
        }
    }
    #endregion
}