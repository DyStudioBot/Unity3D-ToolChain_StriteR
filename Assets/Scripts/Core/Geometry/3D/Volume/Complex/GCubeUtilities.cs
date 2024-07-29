using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;

namespace Runtime.Geometry
{
    public struct CornerGeometry
    {
        public float3 position;
        public ushort corner;
        public ushort side1;
        public ushort side2;
    }

    public static class UCube
    {
        public static int CornerToIndex(this ECubeCorner _corners)
        {
            switch (_corners)
            {
                default:throw new InvalidEnumArgumentException();
                case ECubeCorner.DB: return 0;
                case ECubeCorner.DL: return 1;
                case ECubeCorner.DF: return 2;
                case ECubeCorner.DR: return 3;
                case ECubeCorner.TB: return 4;
                case ECubeCorner.TL: return 5;
                case ECubeCorner.TF: return 6;
                case ECubeCorner.TR: return 7;
            }
        } 
        
        public static ECubeCorner IndexToCorner(int _index)
        {
            switch (_index)
            {
                default:throw new InvalidEnumArgumentException();
                case 0:return ECubeCorner.DB; 
                case 1:return ECubeCorner.DL;
                case 2:return ECubeCorner.DF;
                case 3:return ECubeCorner.DR;
                case 4:return ECubeCorner.TB;
                case 5:return ECubeCorner.TL;
                case 6:return ECubeCorner.TF;
                case 7:return ECubeCorner.TR;
            }
        }
        
        public static void GetCornerGeometry(ECubeFacing _facing,out CornerGeometry b,out CornerGeometry l,out CornerGeometry f,out CornerGeometry r,out half3 n,out half4 t)
        {
            ushort cb, cl, cf, cr,sb1,sb2,sl1,sl2,sf1,sf2,sr1,sr2;
            switch (_facing)
            {
                default: throw new InvalidEnumArgumentException();
                case ECubeFacing.B:
                {
                    cb = 0; cl = 1; cf = 5; cr = 4;
                    sb1 = 1; sl1 = 1; sf1 = 9; sr1 = 9;
                    sb2 = 4;  sl2 = 5; sf2 = 5; sr2 = 4;
                    n = (half3)new float3(0f,0f,-1f);t = (half4)new float4(1f,0f,0f,1f);
                }break;
                case ECubeFacing.L:
                {
                    cb = 1; cl = 2; cf = 6; cr = 5;
                    sb1 = 2; sl1 = 2; sf1 = 10; sr1 = 10;
                    sb2 = 5;  sl2 = 6; sf2 = 6; sr2 = 5;
                    n = (half3)new float3(-1f,0f,0f);t = (half4)new float4(0f,0f,1f,1f);
                }break;
                case ECubeFacing.F:
                {
                    cb = 2; cl = 3; cf = 7; cr = 6;
                    sb1 = 3; sl1 = 3; sf1 = 11; sr1 = 11;
                    sb2 = 6;  sl2 = 7; sf2 = 7; sr2 = 6;
                    n = (half3)new float3(0f,0f,1f);t = (half4)new float4(-1f,0f,0f,1f);
                }break;
                case ECubeFacing.R:
                {
                    cb = 3; cl = 0; cf = 4; cr = 7; 
                    sb1 = 0; sl1 = 0; sf1 = 8; sr1 = 8;
                    sb2 = 7;  sl2 = 4; sf2 = 4; sr2 = 7;
                    n = (half3)new float3(1f,0f,0f);t = (half4)new float4(0f,0f,-1f,1f);
                }break;
                case ECubeFacing.T:
                {
                    cb = 4; cl = 5; cf = 6; cr = 7; 
                    sb1 = 8; sl1 = 9; sf1 = 10; sr1 = 11;
                    sb2 = 9;  sl2 = 10; sf2 = 11; sr2 = 8;
                    n = (half3)new float3(0f,1f,0f);t = (half4)new float4(1f,0f,0f,1f);
                }break;
                case ECubeFacing.D:
                {
                    cb = 0; cl = 3; cf = 2; cr = 1; 
                    sb1 = 1;  sl1 = 0; sf1 = 3; sr1 = 2;
                    sb2 = 0;  sl2 = 3; sf2 = 2; sr2 = 1;
                    n = (half3)new float3(0f,-1f,0f);t = (half4)new float4(-1f,0f,0f,1f);
                }break;
            }

            b = new CornerGeometry { position = KCube.kPositions[cb], corner = cb,side1 = sb1, side2 = sb2,};
            l = new CornerGeometry { position = KCube.kPositions[cl], corner = cl,side1 = sl1, side2 = sl2,};
            f = new CornerGeometry { position = KCube.kPositions[cf], corner = cf,side1 = sf1, side2 = sf2,};
            r = new CornerGeometry { position = KCube.kPositions[cr], corner = cr,side1 = sr1, side2 = sr2,};
        }


        public static bool IsTopFloor(this ECubeCorner _corner)=> _corner.CornerToIndex() >= 4;
        
        public static ECubeCorner FlipFloor(this ECubeCorner _corner)
        {
            var _index = _corner.CornerToIndex();
            _index += _corner.IsTopFloor() ? -4 : 4;
            return IndexToCorner(_index);
        }

        public static ECubeCorner NextCornerFlooredCW(this ECubeCorner _corner,int _step)
        {
            var _index = _corner.CornerToIndex();
            var baseIndex = _corner.IsTopFloor() ? 4:0;
            _index -= baseIndex;
            _index += _step;
            _index %= 4;
            return IndexToCorner(baseIndex+_index);
        }

        public static ECubeCorner DiagonalCorner(this ECubeCorner _corner)
        {
            switch (_corner)
            {
                default: throw new Exception("Invalid Corner:"+_corner);
                case ECubeCorner.DB: return ECubeCorner.TF;
                case ECubeCorner.DL: return ECubeCorner.TR;
                case ECubeCorner.DF: return ECubeCorner.TB;
                case ECubeCorner.DR: return ECubeCorner.TL;
                case ECubeCorner.TB: return ECubeCorner.DF;
                case ECubeCorner.TL: return ECubeCorner.DR;
                case ECubeCorner.TF: return ECubeCorner.DB;
                case ECubeCorner.TR: return ECubeCorner.DL;
            }
        }

        public static ECubeCorner HorizontalDiagonalCorner(this ECubeCorner _corner)
        {
            switch (_corner)
            {
                default: throw new Exception("Invalid Corner:"+_corner);
                case ECubeCorner.DB: return ECubeCorner.DF;
                case ECubeCorner.DL: return ECubeCorner.DR;
                case ECubeCorner.DF: return ECubeCorner.DB;
                case ECubeCorner.DR: return ECubeCorner.DL;
                case ECubeCorner.TB: return ECubeCorner.TF;
                case ECubeCorner.TL: return ECubeCorner.TR;
                case ECubeCorner.TF: return ECubeCorner.TB;
                case ECubeCorner.TR: return ECubeCorner.TL;
            }
        }
        public static IEnumerable<(ECubeCorner _qube, ECubeCorner _adjactileCorner1, ECubeCorner _adjactileCorner2)> NearbyValidCornerQube(this ECubeCorner _srcCorner)
        {
            var flip = _srcCorner.FlipFloor();
            var qube0 =  _srcCorner.HorizontalDiagonalCorner();
            yield return (qube0,_srcCorner,_srcCorner.FlipFloor());

            var qube1 = flip.NextCornerFlooredCW(1);
            yield return (qube1,_srcCorner,_srcCorner.NextCornerFlooredCW(3));
            
            var qube2 = flip.NextCornerFlooredCW(3);
            yield return (qube2,_srcCorner,_srcCorner.NextCornerFlooredCW(1));
        }
        
        public static Qube<T> Resize_Dynamic<T>(this Qube<T> _qube, float _shrinkScale) where  T: struct
        {
            dynamic db = _qube.vDB;
            dynamic dl = _qube.vDL;
            dynamic df = _qube.vDF;
            dynamic dr = _qube.vDR;
            dynamic tb = _qube.vTB;
            dynamic tl = _qube.vTL;
            dynamic tf = _qube.vTF;
            dynamic tr = _qube.vTR;
            _qube.vDB = db * _shrinkScale;
            _qube.vDL = dl * _shrinkScale;
            _qube.vDF = df * _shrinkScale;
            _qube.vDR = dr * _shrinkScale;
            _qube.vTB = tb * _shrinkScale;
            _qube.vTL = tl * _shrinkScale;
            _qube.vTF = tf * _shrinkScale;
            _qube.vTR = tr * _shrinkScale;
            return _qube;
        }

        public static (Quad<T> _downQuad, Quad<T> _topQuad) SplitTopDownQuads<T>(this Qube<T> _qube)where T:struct
        {
            Quad<T> downQuad = new Quad<T>( _qube.vDB,_qube.vDL,_qube.vDF,_qube.vDR);;
            Quad<T> topQuad = new Quad<T>(_qube.vTB,_qube.vTL,_qube.vTF,_qube.vTR);
            return (downQuad, topQuad);
        }
        public static Qube<T> RotateYawCW<T>(this Qube<T> _qube,int _90DegMult) where T:struct
        {
            var quads = _qube.SplitTopDownQuads<T>();
            var top = quads._topQuad.RotateYawCW(_90DegMult);
            var down = quads._downQuad.RotateYawCW(_90DegMult);
            return new Qube<T>(down,top);
        }

        public static Qube<T> MirrorLR<T>(this Qube<T> _qube) where T:struct
        {
            var quads = _qube.SplitTopDownQuads<T>();
            return new Qube<T>(quads._downQuad.MirrorLR(),quads._topQuad.MirrorLR());
        }
        
        public static Qube<byte> SplitByteQubes(this Qube<bool> _qube,bool _fillHorizontalDiagonal)
        {
            Qube<bool>[] splitQubes = new Qube<bool>[8];
            for (int i = 0; i < 8; i++)
            {
                splitQubes[i] = default;
                splitQubes[i].SetByteElement(_qube[i]?byte.MaxValue:byte.MinValue);
            }

            foreach (var corner in UEnum.GetEnums<ECubeCorner>())
            {
                if(_qube[corner])
                    continue;

                var diagonal = corner.DiagonalCorner();
                
                if (_qube[diagonal])
                    splitQubes[diagonal.CornerToIndex()][corner]=false;

                foreach (var tuple in corner.NearbyValidQubeFacing())
                {
                    var qube = tuple._cornerQube;
                    var facing = tuple._facingDir;
                    if(!_qube[qube])
                        continue;
                    var qubeIndex = qube.CornerToIndex();
                    foreach (var facingCorner in facing.Opposite().FacingCorners())
                        splitQubes[qubeIndex][facingCorner] = false;
                }
                
                foreach (var tuple in corner.NearbyValidCornerQube())
                {
                    var qube = tuple._qube;
                    if(!_qube[qube])
                        continue;
                    splitQubes[qube.CornerToIndex()][tuple._adjactileCorner1]=false;
                    splitQubes[qube.CornerToIndex()][tuple._adjactileCorner2]=false;
                }
            }

            if (_fillHorizontalDiagonal)
            {
                int bottomValidCount = 0;
                int topValidCount = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (!_qube[i])
                        continue;
                    if (i < 4)
                        bottomValidCount += 1;
                    else
                        topValidCount += 1;
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i < 4)
                    {
                        if(bottomValidCount%2!=0)
                            continue;
                    }
                    else
                    {
                        if (topValidCount % 2 != 0)
                            continue;
                    }
                    
                    if(!_qube[i])
                        continue;
                    
                    var horizontalDiagonal = IndexToCorner(i).HorizontalDiagonalCorner();
                    if (!_qube[horizontalDiagonal])
                        continue;

                    splitQubes[i][horizontalDiagonal] = true;
                    splitQubes[horizontalDiagonal.CornerToIndex()][i] = true;
                }
            }

            Qube<byte> byteQube = new Qube<byte>(splitQubes[0].ToByte(),splitQubes[1].ToByte(),splitQubes[2].ToByte(),splitQubes[3].ToByte(),
                splitQubes[4].ToByte(),splitQubes[5].ToByte(),splitQubes[6].ToByte(),splitQubes[7].ToByte());
            return byteQube;
        }
        

        public static void SetByteElement(ref this Qube<bool> _qube, byte _byte)
        {
            for (int i = 0; i < 8; i++)
                _qube[i] = UByte.PosValid(_byte,i);
        }

        public static Qube<bool> ToQube(this byte _byte)
        {
            Qube<bool> qube = default;
            qube.SetByteElement(_byte);
            return qube;
        }
        public static byte ToByte(this Qube<bool> _qube)
        {
            return UByte.ToByte(_qube[0],_qube[1],_qube[2],_qube[3],
                _qube[4],_qube[5],_qube[6],_qube[7]);
        }
        
        public static Qube<bool> And(this Qube<bool> _srcQube,Qube<bool> _dstQube)=> Qube<bool>.Convert(_srcQube,(index,value)=>value&&_dstQube[index]);

        public static (ECubeCorner v0, ECubeCorner v1, ECubeCorner v2, ECubeCorner v3) GetRelativeVertsCW(this ECubeFacing _facing)
        {
            switch (_facing)
            {
                default: throw new Exception("Invalid Face:"+_facing);
                case ECubeFacing.B:return (ECubeCorner.DB,ECubeCorner.DL,ECubeCorner.TL,ECubeCorner.TB);
                case ECubeFacing.L:return (ECubeCorner.DL,ECubeCorner.DF,ECubeCorner.TF,ECubeCorner.TL);
                case ECubeFacing.F:return (ECubeCorner.DF,ECubeCorner.DR,ECubeCorner.TR,ECubeCorner.TF);
                case ECubeFacing.R:return (ECubeCorner.DR,ECubeCorner.DB,ECubeCorner.TB,ECubeCorner.TR);
                case ECubeFacing.T:return (ECubeCorner.TB,ECubeCorner.TL,ECubeCorner.TF,ECubeCorner.TR);
                case ECubeFacing.D:return (ECubeCorner.DB,ECubeCorner.DR,ECubeCorner.DF,ECubeCorner.DL);
            }
        }
        
        public static IEnumerable<(ECubeCorner _cornerQube, ECubeFacing _facingDir)> NearbyValidQubeFacing(this ECubeCorner _srcCorner)
        {
            switch (_srcCorner)
            {
                default: throw new IndexOutOfRangeException();
                
                case ECubeCorner.DB:
                    yield return (ECubeCorner.TB, ECubeFacing.T);
                    yield return (ECubeCorner.DL, ECubeFacing.L);
                    yield return (ECubeCorner.DR, ECubeFacing.F);
                    break;
                case ECubeCorner.DL:
                    yield return (ECubeCorner.TL, ECubeFacing.T);
                    yield return (ECubeCorner.DF, ECubeFacing.F);
                    yield return (ECubeCorner.DB, ECubeFacing.R);
                    break;
                case ECubeCorner.DF: 
                    yield return (ECubeCorner.TF, ECubeFacing.T);
                    yield return (ECubeCorner.DR, ECubeFacing.R);
                    yield return (ECubeCorner.DL, ECubeFacing.B);
                    break;
                case ECubeCorner.DR:
                    yield return (ECubeCorner.TR, ECubeFacing.T);
                    yield return (ECubeCorner.DB, ECubeFacing.B);
                    yield return (ECubeCorner.DF, ECubeFacing.L);
                    break;
                case ECubeCorner.TB: 
                    yield return (ECubeCorner.DB, ECubeFacing.D);
                    yield return (ECubeCorner.TL, ECubeFacing.L);
                    yield return (ECubeCorner.TR, ECubeFacing.F);
                    break;
                case ECubeCorner.TL: 
                    yield return (ECubeCorner.DL, ECubeFacing.D);
                    yield return (ECubeCorner.TF, ECubeFacing.F);
                    yield return (ECubeCorner.TB, ECubeFacing.R);
                    break;
                case ECubeCorner.TF: 
                    yield return (ECubeCorner.DF, ECubeFacing.D);
                    yield return (ECubeCorner.TR, ECubeFacing.R);
                    yield return (ECubeCorner.TL, ECubeFacing.B);
                    break;
                case ECubeCorner.TR: 
                    yield return (ECubeCorner.DR, ECubeFacing.D);
                    yield return (ECubeCorner.TB, ECubeFacing.B);
                    yield return (ECubeCorner.TF, ECubeFacing.L);
                    break;
            }
        }

        static readonly Dictionary<ECubeFacing, ECubeCorner[]> kFacingCorners = new Dictionary<ECubeFacing, ECubeCorner[]>()
            {
                { ECubeFacing.D, new[] { ECubeCorner.DB, ECubeCorner.DL, ECubeCorner.DF, ECubeCorner.DR } },
                { ECubeFacing.T, new[] { ECubeCorner.TB, ECubeCorner.TL, ECubeCorner.TF, ECubeCorner.TR } },
                { ECubeFacing.B,new []{ ECubeCorner.DB, ECubeCorner.TB, ECubeCorner.DL, ECubeCorner.TL}},
                { ECubeFacing.L,new []{ ECubeCorner.DL, ECubeCorner.TL, ECubeCorner.DF,ECubeCorner.TF }},
                { ECubeFacing.F,new []{ ECubeCorner.DF, ECubeCorner.TF,ECubeCorner.DR,ECubeCorner.TR }},
                { ECubeFacing.R,new []{ ECubeCorner.DR, ECubeCorner.TR, ECubeCorner.DB, ECubeCorner.TB}}
            };

        public static Quad<T> GetSideFacing<T>(this CubeSides<T> _sides) => new Quad<T>(_sides.fBL,_sides.fLF,_sides.fFR,_sides.fRB);
        public static ECubeCorner[] FacingCorners(this ECubeFacing _facing) => kFacingCorners[_facing];

        public static (T v0, T v1, T v2, T v3) GetFacingCornersCW<T>(this Qube<T> _qube, ECubeFacing _facing) where T : struct
        {
            var corners = _facing.GetRelativeVertsCW();
            return (_qube[corners.v0],_qube[corners.v1],_qube[corners.v2],_qube[corners.v3] );
        }
        
        public static int FacingToIndex(this ECubeFacing _facing)
        {
            switch (_facing)
            {
                default: throw new InvalidEnumArgumentException();
                case ECubeFacing.B: return 0;
                case ECubeFacing.L: return 1;
                case ECubeFacing.F: return 2;
                case ECubeFacing.R: return 3;
                case ECubeFacing.T: return 4;
                case ECubeFacing.D: return 5;
            }
        }      
        public static ECubeFacing Opposite(this ECubeFacing _facing)
        {
            switch (_facing)
            {
                default: throw new InvalidEnumArgumentException();
                case ECubeFacing.B: return ECubeFacing.F;
                case ECubeFacing.L: return ECubeFacing.R;
                case ECubeFacing.F: return ECubeFacing.B;
                case ECubeFacing.R: return ECubeFacing.L;
                case ECubeFacing.T: return ECubeFacing.D;
                case ECubeFacing.D: return ECubeFacing.T;
            }
        } 
        
        public static ECubeFacing IndexToFacing(int _index)
        {
            switch (_index)
            {
                default:throw new IndexOutOfRangeException();
                case 0:return ECubeFacing.B;
                case 1:return ECubeFacing.L;
                case 2:return ECubeFacing.F;
                case 3:return ECubeFacing.R;
                case 4:return ECubeFacing.T;
                case 5:return ECubeFacing.D;
            }
        }

        public static Int3 GetCubeOffset(ECubeFacing _facing)
        {
            switch (_facing)
            {
                default: throw new InvalidEnumArgumentException();
                case ECubeFacing.B: return Int3.kBack;
                case ECubeFacing.L: return Int3.kLeft;
                case ECubeFacing.F: return Int3.kForward;
                case ECubeFacing.R: return Int3.kRight;
                case ECubeFacing.T: return Int3.kUp;
                case ECubeFacing.D: return Int3.kDown;
            }
        }

        public static ECubeFacing GetCubeFacing(float3 _direction)
        {
            var xAbs = math.abs(_direction.x);
            var yAbs = math.abs(_direction.y);
            var zAbs = math.abs(_direction.z);

            if (xAbs >= yAbs && xAbs >= zAbs)
                return _direction.x > 0 ? ECubeFacing.R : ECubeFacing.L;
            if (yAbs >= xAbs && yAbs >= zAbs)
                return _direction.y > 0 ? ECubeFacing.T : ECubeFacing.B;
            return _direction.z > 0 ? ECubeFacing.F : ECubeFacing.D;
        }
    }
    }