using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
   public class PolygonTracing
    {
        RayTracingComputation rc = new RayTracingComputation();

        public bool PolygonIntersection(ref RayTracingModel rayTracingModel,int i,int j)
        {
            try
            {
                rayTracingModel.L1 = rc.VectorSubstraction (rayTracingModel.Polygon.V1 , rayTracingModel.Polygon.V0);
                rayTracingModel.L2 = rc.VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V0);

                rayTracingModel.N = rc.GetCrossProduct(rayTracingModel.L1, rayTracingModel.L2);
                double denom = rc.VectorDot(rayTracingModel.N, rayTracingModel.N);

                double NdotRayDirection = rc.VectorDot(rayTracingModel.N, rayTracingModel.VectorP0);

                if (Math.Abs(NdotRayDirection) < 1e-8) // almost 0
                    return false; // they are parallel so they don't intersect ! 






                double d =   rc.VectorDot(rayTracingModel.N,rayTracingModel.Polygon.V0);

                // compute t (equation 3)
                double t = (rc.VectorDot(rayTracingModel.N, rayTracingModel.v0) + d)  / NdotRayDirection;

                // check if the triangle is in behind the ray
                if (t < 0) return false; // the triangle is behind

                // compute the intersection point using equation 1
                Vector P = rc.VectorAddition(rayTracingModel.v0, rc.Multiply(rayTracingModel.VectorP0,t));

                // Step 2: inside-outside test
                Vector C; // vector perpendicular to triangle's plane

                // edge 0
                Vector edge0 = rc.VectorSubstraction(rayTracingModel.Polygon.V1,rayTracingModel.Polygon.V0);
                Vector vp0 = rc.VectorSubstraction(P,rayTracingModel.Polygon.V0);
                C = rc.GetCrossProduct(edge0, vp0);

                if (rc.VectorDot(rayTracingModel.N,C) < 0) return false; // P is on the right side

                // edge 1
                Vector edge1 = rc.VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V1);

                Vector vp1 = rc.VectorSubstraction(P, rayTracingModel.Polygon.V1);
                C = rc.GetCrossProduct(edge1, vp1);
                double u = rc.VectorDot(rayTracingModel.N, C);

                if (rc.VectorDot(rayTracingModel.N, C) < 0) return false; // P is on the right side


                // edge 2

                Vector edge2 = rc.VectorSubstraction(rayTracingModel.Polygon.V0, rayTracingModel.Polygon.V2);

                Vector vp2 = rc.VectorSubstraction(P, rayTracingModel.Polygon.V2);
                C = rc.GetCrossProduct(edge2, vp2);
                double v = rc.VectorDot(rayTracingModel.N, C);

                if (rc.VectorDot(rayTracingModel.N, C) < 0) return false; // P is on the right side


               u =u/ denom;
                v =v/ denom;
                Vector red = new Vector(255, 0, 0);
                Vector white = new Vector(255, 255, 255);

                Vector black = new Vector(0, 0, 0);
                Vector col = new Vector();
                u = u * 255;
                v = v * 255;
                col.Z = (1 - u - v )* 255;


                col.X = (col.X > 255) ? 255 : (col.X < 0) ? 0 : u;
                col.Y = (col.Y > 255) ? 255 : (col.Y < 0) ? 0 : v;
                col.Z = (col.Z > 255) ? 255 : (col.Z < 0) ? 0 : col.Z;
                //  rayTracingModel.bitmap.SetPixel(i, j,Color.FromArgb(Convert.ToInt32(tcolor)));

            
            //col = red;

            rayTracingModel.bitmap.SetPixel(i, j, Color.FromArgb((int)col.X, (int)col.Y, (int)col.Z));

            return true; // this ray hits the triangle

            }
            catch (Exception)
            {

                
            }
            return true;
        }

        public void Shading(double dt, int i, int j)
        {
                }

    }
}
