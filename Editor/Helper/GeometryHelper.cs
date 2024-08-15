using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public static class GeometryHelper
    {
        public static bool cross_check(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            if (Mathf.Max(a.x, b.x) <= Mathf.Min(c.x, d.x))
            {
                return false;
            }

            if (Mathf.Min(a.x, b.x) >= Mathf.Max(c.x, d.x))
            {
                return false;
            }

            if (Mathf.Max(a.y, b.y) <= Mathf.Min(c.y, d.y))
            {
                return false;
            }

            if (Mathf.Min(a.y, b.y) >= Mathf.Max(c.y, d.y))
            {
                return false;
            }

            double u, v, w, z;//分别记录两个向量
            u = (c.x - a.x) * (b.y - a.y) - (b.x - a.x) * (c.y - a.y);
            v = (d.x - a.x) * (b.y - a.y) - (b.x - a.x) * (d.y - a.y);
            w = (a.x - c.x) * (d.y - c.y) - (d.x - c.x) * (a.y - c.y);
            z = (b.x - c.x) * (d.y - c.y) - (d.x - c.x) * (b.y - c.y);
            return (u * v <= 0.00000001 && w * z <= 0.00000001);
        }

        //凸多边形分解
        public static int[] poly_convex_decomposition(Vector3[] vertex)
        {
            List<int> origin = new List<int>();
            List<int> indexs = new List<int>();

            List<int> squeeze = new List<int>();
            bool AllConvex = true;
            for (int i = 0; i < vertex.Length; i++)
            {
                origin.Add(i);
            }

            do
            {
                AllConvex = true;
                for (int i = 1; i < origin.Count; i++)
                {
                    int victim = i;
                    for (int t = 0; t < 2; t++)
                    {
                        victim -= 1;
                        if (victim < 0)
                        {
                            victim = origin.Count - 1;
                        }
                    }

                    int idx0 = (i - 1) < 0 ? origin.Count - 1 : (i - 1);
                    int idx1 = i;
                    int idx2 = (i + 1) % origin.Count;
                    int idx3 = (i + 2) % origin.Count;

                    Vector3 p0 = vertex[origin[idx0]];
                    Vector3 p1 = vertex[origin[idx1]];
                    Vector3 p2 = vertex[origin[idx2]];
                    Vector3 p3 = vertex[origin[idx3]];

                    Vector3 e0 = p1 - p0;
                    Vector3 e1 = p2 - p1;

                    Vector3 normal = Vector3.Cross(e1, e0);

                    float dis2_e0 = e0.sqrMagnitude;
                    float dis2_e1 = e1.sqrMagnitude;

                    float cross = e0.x * e1.z - e0.z * e1.x;

                    if (cross >= 0)
                    {
                        continue;
                    }

                    AllConvex = false;
                    //判断为凹点,取距离最短的临边 
                    if (dis2_e0 < dis2_e1)
                    {
                        if (!squeeze.Contains(origin[idx0]))
                        {
                            squeeze.Add(origin[idx0]);

                            indexs.Add(origin[victim]);
                            indexs.Add(origin[idx0]);
                            indexs.Add(origin[idx1]);

                            origin.RemoveAt(idx0);
                            break;
                        }
                    }
                    else
                    {
                        if (!squeeze.Contains(origin[idx2]))
                        {
                            squeeze.Add(origin[idx2]);

                            indexs.Add(origin[idx1]);
                            indexs.Add(origin[idx2]);
                            indexs.Add(origin[idx3]);

                            origin.RemoveAt(idx2);
                            break;
                        }
                    }
                }
            }
            while (!AllConvex);

            //deal with rest index
            if (origin.Count >= 3)
            {
                for (int i = 2; i < origin.Count; i++)
                {
                    indexs.Add(origin[0]);
                    indexs.Add(origin[i - 1]);
                    indexs.Add(origin[i]);
                }
            }

            //normal flip
            indexs.Reverse();
            return indexs.ToArray();
        }

        public static bool PointInTriangle(Vector3 point, Vector3 triP1, Vector3 triP2, Vector3 triP3)
        {
            float fsx = 0;
            float fex = 0;
            float fsy = 0;
            float fey = 0;

            float t = 0;
            float y = 0;

            float fx = point.x;
            float fy = point.z;

            int iInterSectCount = 0;

            Vector3[] array = { triP1, triP2, triP3 };
            int iPointCount = 3;

            for (int i = 0; i < iPointCount; i++)
            {
                t = 0;
                fsx = array[i].x;
                fsy = array[i].z;

                if (i == iPointCount - 1)
                {
                    fex = array[0].x;
                    fey = array[0].z;
                }
                else
                {
                    fex = array[i + 1].x;
                    fey = array[i + 1].z;
                }

                double slope = (fey - fsy) / (fex - fsx);
                bool cond1 = (fsx <= fx) && (fx < fex);
                bool cond2 = (fx < fsx) && (fx >= fex);
                bool above = (fy < slope * (fx - fsx) + fsy);

                if ((cond1 || cond2) && above)
                {
                    iInterSectCount++;
                }
            }

            if ((iInterSectCount % 2) != 0)
            {
                return true;
            }

            return false;//
        }

        public static bool IntersectRayAndHorizontalPlane(Ray ray, float horizonHeight, ref Vector3 point)
        {
            float t = 0f;
            float x, z;
            Vector3 ori = ray.origin;
            Vector3 dir = ray.direction;

            if (Mathf.Abs(dir.y) <= Mathf.Epsilon) return false;

            t = (horizonHeight - ori.y) / dir.y;
            x = ori.x + t * dir.x;
            z = ori.z + t * dir.z;

            point.x = x;
            point.y = horizonHeight;
            point.z = z;
            return true;
        }

        public static void AdjustPoint(ref Vector3 point)
        {
            float x = Mathf.Floor(point.x);
            float z = Mathf.Floor(point.z);

            point.x = x + 0.5f;
            point.z = z + 0.5f;
        }

        public static void AdjustPointFloor(ref Vector3 point)
        {
            float x = Mathf.Floor(point.x);
            float z = Mathf.Floor(point.z);

            point.x = x + 0.5f;
            point.z = z + 0.5f;
        }
    }


