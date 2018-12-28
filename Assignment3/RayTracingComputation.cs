using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Assignment3
{
    public class RayTracingComputation
    {
        //global raytracingmodel
        RayTracingModel rayTracingModel;

        /// <summary>
        ///  raytracing  method getting parameters to move object        
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public string RayTracing(double x,double y,double z,Vector vrp,Vector vpn,Vector vup,Sphere sphere,Polygon polygon)
        {
            //initialize method
            rayTracingModel = new RayTracingModel();

            //raytracing bitmap image
            rayTracingModel.bitmap = new Bitmap(512, 512);

            //screen width
            rayTracingModel.Width = 512;

            //screen  height
            rayTracingModel.Height = 512;

            //focal
            rayTracingModel.Focal = 0.05;

            //Light coodinate system
            rayTracingModel.LRP = new Vector();

            rayTracingModel.LRP.X = x;
            rayTracingModel.LRP.Y =y;
            rayTracingModel.LRP.Z = z;

            
            //Light intensity
            rayTracingModel.Ip = 185;

            //screen size
            rayTracingModel.Xmin = 0.0175;
            rayTracingModel.Ymin = -0.0175;
            rayTracingModel.Xmax = -0.0175;
            rayTracingModel.Ymax = 0.0175;

            //Camera coordinate system
            rayTracingModel.VRP = new Vector();
            rayTracingModel.VRP.X =10;
            rayTracingModel.VRP.Y = 0;
            rayTracingModel.VRP.Z = 20;

            rayTracingModel.VPN = new Vector();
            rayTracingModel.VPN.X = 10;
            rayTracingModel.VPN.Y =0;
            rayTracingModel.VPN.Z =21;

            rayTracingModel.VUP = new Vector();
            rayTracingModel.VUP.X =0;
            rayTracingModel.VUP.Y = 1;
            rayTracingModel.VUP.Z =0;


            rayTracingModel.VRP = vrp;
            rayTracingModel.VPN = vpn;
            rayTracingModel.VUP = vup;

            //sphere 
            rayTracingModel.Sphere = new Sphere();
            rayTracingModel.Sphere.Center = new Vector();
            rayTracingModel.Sphere.Center.X =256;
            rayTracingModel.Sphere.Center.Y = 256;
            rayTracingModel.Sphere.Center.Z =50;
            rayTracingModel.Sphere.Radius = 50;
            rayTracingModel.Sphere.Kd =0.95;


            rayTracingModel.Sphere = sphere;
            rayTracingModel.Sphere.Kd = 0.95;


            rayTracingModel.Polygon = new Polygon();
            rayTracingModel.Polygon.V0 = new Vector();
            rayTracingModel.Polygon.V1 = new Vector();
            rayTracingModel.Polygon.V2 = new Vector();
            rayTracingModel.Polygon.V3 = new Vector();
            rayTracingModel.Polygon.Normal = new Vector();


            rayTracingModel.Polygon.V0 = polygon.V0;
            rayTracingModel.Polygon.V1 = polygon.V1;
            rayTracingModel.Polygon.V2 = polygon.V2;

            rayTracingModel.Polygon.Kd = 0.8;
            //calculate u v n
            GetUnitVector();

           // rayconstruction for each pixel

            for (int i = 0; i < rayTracingModel.Height; i++)
            {
                for (int j = 0; j < rayTracingModel.Width; j++)
                {
                    RayConstruction(i, j,rayTracingModel.Width,rayTracingModel.Height);
                }
            }

            string s = "bitmap" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + ".bmp";

            rayTracingModel.bitmap.Save(s);
            //Process p = new Process();
           // Process.Start(s);
            return s;
        }
        public double deg2rad(double deg)
        {
            return Convert.ToSingle((deg * 3.14) / 180);
        }
        public bool SphereIntersect()
        {

            //Vector oc = VectorSubstraction(rayTracingModel.v0, rayTracingModel.Sphere.Center);

            //double b = 2 * VectorDot(oc, rayTracingModel.VectorP0);
            //double c = VectorDot(oc, oc) - rayTracingModel.Sphere.Radius * rayTracingModel.Sphere.Radius;
            //double disc = (b * b) - (4 * c);
            //if (disc < 1e-4)
            //{
            //    return false;
            //}
            //disc = Math.Sqrt(disc); // 
            //double t0 = -b - disc; // t0 = tca – thc
            //double t1 = -b + disc; // t1 = tca + thc
            //rayTracingModel.t = (t0 < t1) ? t0 : t1;



            //calculating  L
            Vector L = VectorSubstraction(rayTracingModel.v0, rayTracingModel.Sphere.Center);

            //calculating tca
            double Tca = VectorDot(L, rayTracingModel.VectorP0);

            //calculating d
            double d = Math.Sqrt((VectorDot(L, L)) - (Tca * Tca));

            //if no intersection
            if (d > rayTracingModel.Sphere.Radius)
            {
                return false;
            }
            else
            {
                //calculating thc
                double Thc = Math.Sqrt((rayTracingModel.Sphere.Radius * rayTracingModel.Sphere.Radius) - (d * d));
                double t0 = Tca - Thc;
                double t1 = Tca + Thc;
                rayTracingModel.t = (t0 < t1) ? t0 : t1;
            }
            return true;
        }

        public bool PolygonIntersect()
        {
            try
            {
                //rayTracingModel.L1 = VectorSubstraction(rayTracingModel.Polygon.V1, rayTracingModel.Polygon.V0);
                //rayTracingModel.L2 = VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V0);

                //rayTracingModel.N = GetCrossProduct(rayTracingModel.L1, rayTracingModel.L2);
                //rayTracingModel.A = rayTracingModel.N.X;
                //rayTracingModel.B = rayTracingModel.N.Y;
                //rayTracingModel.C = rayTracingModel.N.Z;


                //rayTracingModel.D = -(rayTracingModel.A * rayTracingModel.Polygon.V0.X + rayTracingModel.B * rayTracingModel.Polygon.V0.Y + rayTracingModel.C * rayTracingModel.Polygon.V0.Z);
                //rayTracingModel.tpolygon= -(VectorDot(rayTracingModel.N,rayTracingModel.v0)+rayTracingModel.D)/ VectorDot(rayTracingModel.N, rayTracingModel.VectorP0);

                //double t = rayTracingModel.tpolygon;


                //rayTracingModel.L1 = VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V0);
                //rayTracingModel.L2 = VectorSubstraction(rayTracingModel.Polygon.V3, rayTracingModel.Polygon.V0);

                //rayTracingModel.N = GetCrossProduct(rayTracingModel.L1, rayTracingModel.L2);
                //rayTracingModel.A = rayTracingModel.N.X;
                //rayTracingModel.B = rayTracingModel.N.Y;
                //rayTracingModel.C = rayTracingModel.N.Z;


                //rayTracingModel.D = -(rayTracingModel.A * rayTracingModel.Polygon.V0.X + rayTracingModel.B * rayTracingModel.Polygon.V0.Y + rayTracingModel.C * rayTracingModel.Polygon.V0.Z);
                //rayTracingModel.tpolygon = -(VectorDot(rayTracingModel.N, rayTracingModel.v0) + rayTracingModel.D) / VectorDot(rayTracingModel.N, rayTracingModel.VectorP0);


                //if(rayTracingModel.tpolygon>t)
                //{
                //    rayTracingModel.tpolygon = t;
                //}


              

                Vector pvec = new Vector();
                pvec = GetCrossProduct(rayTracingModel.v0, rayTracingModel.L1);
                double det = VectorDot(rayTracingModel.L1, pvec);

                // ray and triangle are parallel if det is close to 0
                if ((det) < 1e-8) return false;

                double invDet = 1 / det;

                Vector tvec = VectorSubstraction(rayTracingModel.v0 , rayTracingModel.Polygon.V0);
                double u = VectorDot( tvec,pvec) * invDet;
                if (u < 0 || u > 1) return false;

                Vector qvec = GetCrossProduct( tvec,rayTracingModel.L1);
               double v =  VectorDot(rayTracingModel.VectorP0,qvec) * invDet;
                if (v < 0 || u + v > 1) return false;

                rayTracingModel.tpolygon = VectorDot(rayTracingModel.L2, qvec) * invDet;

                return true;


                
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void RayConstruction(int i, int j, int M, int N)
        {
            try
            {


                //o is the starting vector which is vrp
                //image plane is equal to screen size
                //calclate p = (x,y,f)
                 double x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (M - 1)) + rayTracingModel.Xmin;
                double y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (N - 1)) + rayTracingModel.Ymin;
             
                //vector origin
                rayTracingModel.VectorO = new Vector();
                rayTracingModel.VectorO.X = 0;
                rayTracingModel.VectorO.Y = 0;
                rayTracingModel.VectorO.Z = 0 ;
                rayTracingModel.VectorO.W = 1;

                //transforming origin
                rayTracingModel.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, rayTracingModel.VectorO);
                rayTracingModel.v0 = rayTracingModel.VectorO1;


                //this code can help you for generating xmin,xmax,ymin,ymax 
                //double scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                //double imageAspectRatio = rayTracingModel.Width / (double)rayTracingModel.Height;



                //Vector ray direction
                Vector vectorP = new Vector();
                vectorP.X = x;
                vectorP.Y = y;
                vectorP.Z = rayTracingModel.Focal;
                vectorP.W = 1;


                //transformaing direction ray

                rayTracingModel.VectorP = vectorP;
                rayTracingModel.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, rayTracingModel.VectorP);


                //normalizing ray
                rayTracingModel.VectorP1= (VectorSubstraction( rayTracingModel.VectorP1,rayTracingModel.VectorO1));

                rayTracingModel.VectorP0 = Normalize(rayTracingModel.VectorP1);
                bool sphere =  SphereIntersect() ;

                bool polygon = PolygonIntersection(ref rayTracingModel,i,j,sphere);

                //
                //bool polygon = PolygonIntersect();

                //if(rayTracingModel.t>rayTracingModel.tpolygon)
                // {
                //     rayTracingModel.t = rayTracingModel.tpolygon;
                //     rayTracingModel.N = rayTracingModel.Polygon.Normal;
                //     rayTracingModel.VectorN = rayTracingModel.N;
                // }
                // else
                // {
                // }
                ////check if intersect
                if ((sphere && !polygon) || (sphere && polygon && rayTracingModel.t < rayTracingModel.tpolygon))
                {

                    //calculate point of intersection
                    rayTracingModel.pi = VectorAddition(rayTracingModel.v0, Multiply(rayTracingModel.VectorP0, (double)rayTracingModel.t));

                    //calculate LRP-pi
                    rayTracingModel.VectorL = VectorSubstraction(rayTracingModel.LRP, rayTracingModel.pi);

                    //get plane normal
                    rayTracingModel.VectorN = GetNormal();

                    //calculate dt
                    double dt = VectorDot((rayTracingModel.VectorN), Normalize(rayTracingModel.VectorL));
                    //shading
                    Shading(dt, i, j);

                }
                else if ((rayTracingModel.tpolygon < rayTracingModel.t && sphere) || (!sphere && polygon))
                {
                    ShadingPolygon(rayTracingModel.polygondt, i, j);

                }


                else if (!sphere && !polygon)
                {
                    rayTracingModel.bitmap.SetPixel(i, j, Color.Black);

                }

                else
                {

                }

                //calculate mcw like assignment 1?
            }
            catch (Exception)
            {

                Console.WriteLine("Error occured in RayConstruction");
            }


        }
        public Vector Normalize(Vector vector)  {

    double mg = (double) Math.Sqrt(vector.X * vector.X + vector.Y* vector.Y+ vector.Z* vector.Z);
           //mg = 1 / mg;
            vector.X = vector.X / mg;
            vector.Y = vector.Y / mg;
            vector.Z = vector.Z / mg;

            return vector;

        }

        public Vector GetNormal() 
            {
            Vector vector = new Vector();
            vector = VectorSubstraction(  rayTracingModel.Sphere.Center, rayTracingModel.pi);
           // vector = VectorDivision(vector, rayTracingModel.Sphere.Radius);

            return vector;

        }



        public Vector VectorSubstraction(Vector v1, Vector v2)
        {
            Vector vector = new Vector();
            try
            {
                vector.X = v1.X - v2.X;
                vector.Y = v1.Y - v2.Y;
                vector.Z = v1.Z - v2.Z;
            }
            catch (Exception)
            {

                Console.WriteLine("Error in vector substraction");
            }
            return vector;
        }

        public double VectorDot(Vector v1, Vector v2)
        {
            Vector vector = new Vector();
            try
            {
                vector.X = v1.X * v2.X;
                vector.Y = v1.Y * v2.Y;
                vector.Z = v1.Z * v2.Z;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in vector dot" + ex.ToString());
            }
            return vector.X + vector.Y + vector.Z;
        }

        public Vector VectorAddition(Vector v1, Vector v2)
        {
            Vector vector = new Vector();
            try
            {
                vector.X = v1.X + v2.X;
                vector.Y = v1.Y + v2.Y;
                vector.Z = v1.Z + v2.Z;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in vector dot" + ex.ToString());
            }
            return vector;
        }
        public Vector VectorDivision(Vector v1, double v2)
        {
            Vector vector = new Vector();
            try
            {
                vector.X = v1.X /v2;
                vector.Y = v1.Y /v2;
                vector.Z = v1.Z /v2;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in vector dot" + ex.ToString());
            }
            return vector;
        }
        public void Shading(double dt,int i,int j)
        {
            Vector red = new Vector(255, 0, 0);
            Vector white = new Vector(255, 255, 255);
            Vector blue = new Vector(0, 0, 255);

            Vector black = new Vector(0, 0, 0);
            Vector col = new Vector();
            if (dt < 0)
            {
                col = black;
            }
            else
            {
                col = Multiply(Multiply(blue, dt), rayTracingModel.Sphere.Kd);


               

                double tcolor = rayTracingModel.Sphere.Kd * dt * rayTracingModel.Ip;

                col.X = (col.X > 255) ? 255 : (col.X < 0) ? 0 : col.X;
                col.Y = (col.Y > 255) ? 255 : (col.Y < 0) ? 0 : col.Y;
                col.Z = (col.Z > 255) ? 255 : (col.Z < 0) ? 0 : col.Z;
              //  rayTracingModel.bitmap.SetPixel(i, j,Color.FromArgb(Convert.ToInt32(tcolor)));

            }
            //col = red;

            rayTracingModel.bitmap.SetPixel(i, j, Color.FromArgb((int)col.X, (int)col.Y, (int)col.Z));
        }
        public void ShadingPolygon(double dt, int i, int j)
        {
            Vector red = new Vector(255, 0, 0);
            Vector white = new Vector(255, 255, 255);
            Vector blue = new Vector(0, 0, 255);

            Vector black = new Vector(0, 0, 0);
            Vector col = new Vector();
            if (dt < 0)

            {
                col = black;
            }
            else
            {
                col = Multiply(Multiply(red, dt), rayTracingModel.Polygon.Kd);
              //  col =new Vector (180, 0, 0);
                    


                double tcolor = rayTracingModel.Polygon.Kd * dt * rayTracingModel.Ip;

                col.X = (col.X > 255) ? 255 : (col.X < 0) ? 0 : col.X;
                col.Y = (col.Y > 255) ? 255 : (col.Y < 0) ? 0 : col.Y;
                col.Z = (col.Z > 255) ? 255 : (col.Z < 0) ? 0 : col.Z;
                //  rayTracingModel.bitmap.SetPixel(i, j,Color.FromArgb(Convert.ToInt32(tcolor)));

            }
            //col = red;

            rayTracingModel.bitmap.SetPixel(i, j, Color.FromArgb((int)col.X, (int)col.Y, (int)col.Z));
        }


        public Vector GetUnitVector()
        {
            Vector vector = new Vector();
            try
            {

                // calculating vector n

                Vector vectorN = new Vector();
                Vector vectorU = new Vector();


                // n = vpn/|vpn|

                //calculate |VPN|

                double modVPN = (rayTracingModel.VPN.X * rayTracingModel.VPN.X) +
                    (rayTracingModel.VPN.Y * rayTracingModel.VPN.Y) + (rayTracingModel.VPN.Z * rayTracingModel.VPN.Z);
                modVPN = (double)Math.Sqrt(modVPN);

                //calculate N
                vectorN.X = rayTracingModel.VPN.X / modVPN;
                vectorN.Y = rayTracingModel.VPN.Y / modVPN;
                vectorN.Z = rayTracingModel.VPN.Z / modVPN;

                //calculate vector u

                //u=vup*vpn/|vup*vpn|

                // vup = v2 , vpn = v1
                Vector crossProductVUPVPN = this.GetCrossProduct(rayTracingModel.VPN, rayTracingModel.VUP);

                rayTracingModel.angle = this.GetAngle(rayTracingModel.VPN, rayTracingModel.VUP);
                double vectormod = this.GetVectorModResult(rayTracingModel.VUP, rayTracingModel.VPN, true);

                vectorU.X = Convert.ToSingle(crossProductVUPVPN.X / vectormod);
                vectorU.Y = Convert.ToSingle(crossProductVUPVPN.Y / vectormod);
                vectorU.Z = Convert.ToSingle(crossProductVUPVPN.Z / vectormod);



                // calculate vector v
                //v = n*u

                Vector vectorV = new Vector();
                vectorV = this.GetCrossProduct(vectorU, vectorN);


                rayTracingModel.VectorU = vectorU;
                rayTracingModel.VectorN = vectorN;
                rayTracingModel.VectorV = vectorV;


                this.MatrixProcess();

                rayTracingModel.matrixRT = this.MatrixMultiuplication(rayTracingModel.matrixR, rayTracingModel.matrixT);
                rayTracingModel.matrixRTranspose = this.MatrixTranspose(rayTracingModel.matrixR);

                rayTracingModel.matrixTInv = this.MatrixInverse(rayTracingModel.matrixT);
                rayTracingModel.matrixMCW = this.MatrixMultiuplication(rayTracingModel.matrixTInv, rayTracingModel.matrixRTranspose);



            }
            catch (Exception ex)
            {

                return null;
            }
            return null;
        }

        public void MatrixProcess()
        {
           double[,] matrixR = new double[4, 4];
            matrixR[0, 0] = rayTracingModel.VectorU.X;
            matrixR[0, 1] = rayTracingModel.VectorU.Y;
            matrixR[0, 2] = rayTracingModel.VectorU.Z;
            matrixR[0, 3] = 0;

            matrixR[1, 0] = rayTracingModel.VectorV.X;
            matrixR[1, 1] = rayTracingModel.VectorV.Y;
            matrixR[1, 2] = rayTracingModel.VectorV.Z;
            matrixR[1, 3] = 0;

            matrixR[2, 0] = rayTracingModel.VectorN.X;
            matrixR[2, 1] = rayTracingModel.VectorN.Y;
            matrixR[2, 2] = rayTracingModel.VectorN.Z;
            matrixR[2, 3] = 0;

            matrixR[3, 0] = 0;
            matrixR[3, 1] = 0;
            matrixR[3, 2] = 0;
            matrixR[3, 3] = 1;

           double[,]  matrixT = new double[4, 4];
            matrixT[0, 0] = 1;
            matrixT[0, 1] = 0;
            matrixT[0, 2] = 0;
            matrixT[0, 3] = 0 - rayTracingModel.VRP.X;

            matrixT[1, 0] = 0;
            matrixT[1, 1] = 1;
            matrixT[1, 2] = 0;
            matrixT[1, 3] = 0 - rayTracingModel.VRP.Y;

            matrixT[2, 0] = 0;
            matrixT[2, 1] = 0;
            matrixT[2, 2] = 1;
            matrixT[2, 3] = 0 - rayTracingModel.VRP.Z;

            matrixT[3, 0] = 0;
            matrixT[3, 1] = 0;
            matrixT[3, 2] = 0;
            matrixT[3, 3] = 1;

            rayTracingModel.matrixR = matrixR;
            rayTracingModel.matrixT = matrixT;

        }

        public Vector MultiplyVector(double[,] matrix, Vector vector)
        {
            Vector v = new Vector();
            try
            {
                v.X = matrix[0, 0] * vector.X + matrix[0,1] * vector.Y + matrix[0,2] * vector.Z + matrix[0,3] * vector.W;
                v.Y = matrix[1,0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z + matrix[1, 3] * vector.W;

                v.Z = matrix[2,0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z + matrix[2, 3] * vector.W;
                v.W = matrix[3,0] * vector.X + matrix[3, 1] * vector.Y + matrix[3, 2] * vector.Z + matrix[3, 3] * vector.W;


            }
            catch (Exception ex)
            {

                throw;
            }
            return v;

        }

        public double[,] MatrixMultiuplication(double[,] matrix1, double[,] matrix2)
        {
            double[,] matrixResult = new double[4, 4];
            try
            {
                for (int i = 0; i < 4; i++)
                {

                    int column = 0;
                    while (column < 4)
                    {
                        int count = 0;
                        for (int j = 0; j < 4; j++)
                        {

                            matrixResult[i, column] += (matrix1[i, j] * matrix2[count, column]);
                            count = count + 1;

                        }


                        column = column + 1;
                    }

                }
                return matrixResult;
            }
            catch (Exception)
            {

                return null;
            }
        }


        public double[,] MatrixTranspose(double[,] matrix1)
        {
            double[,] matrixResult = new double[4, 4];
            try
            {
                int count = 0;
                while (count < 4)
                {

                    for (int i = 0; i < 4; i++)
                    {

                        matrixResult[i, count] = matrix1[count, i];

                    }
                    count = count + 1;
                }

            }
            catch (Exception)
            {


            }
            return matrixResult;
        }

        public double[,] MatrixInverse(double[,] matrix1)
        {
            try
            {

                matrix1[0, 3] = matrix1[0, 3] * -1;
                matrix1[1, 3] = matrix1[1, 3] * -1;

                matrix1[2, 3] = matrix1[2, 3] * -1;


            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }
            return matrix1;
        }


        public Vector GetCrossProduct(Vector v1, Vector v2)
        {
            Vector vectorModel = new Vector();
            try
            {

                //calcualting vector model cross product

                // result


                vectorModel.X = (v2.Y * v1.Z - v2.Z * v1.Y);
                vectorModel.Y = -1 * (v2.X * v1.Z - v2.Z * v1.X);
                vectorModel.Z = (v2.X * v1.Y - v2.Y * v1.X);





            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }

            return vectorModel;
        }




        public double GetVectorModResult(Vector v1, Vector v2, bool isAngle)
        {
            double result = 0.0;
            try
            {

                double v1mod = Math.Sqrt((v1.X * v1.X) + (v1.Y * v1.Y) + (v1.Z * v1.Z));
                double v2mod = Math.Sqrt((v2.X * v2.X) + (v2.Y * v2.Y) + (v2.Z * v2.Z));

                result = v1mod * v2mod;

                if (isAngle)
                {
                    result = result * Math.Sin(rayTracingModel.angle * Math.PI / 180);
                }

            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }
            return result;
        }

        public double GetAngle(Vector v1, Vector v2)
        {
            try
            {
                double vectormod = GetVectorModResult(v1, v2, false);
                rayTracingModel.angle = ((v1.X * v2.X) + (v1.Y * v2.Y)) / vectormod;
                rayTracingModel.angle = Math.Acos(rayTracingModel.angle);

                //convert it into degrees
                rayTracingModel.angle = rayTracingModel.angle * 180 / Math.PI;

            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }
            return rayTracingModel.angle;
        }
        public Vector Multiply(Vector t,double f)
        {

            Vector v = new Vector();
            v.X = t.X * f;
            v.Y = t.Y * f;
            v.Z = t.Z * f;

            return v;
        }

       public void Normalize1(Vector vector)
        {
            double result = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
            if (result > 0)
            {
                double invNor = 1 / Math.Sqrt(result);

                rayTracingModel.VectorP.X =Convert.ToInt32( rayTracingModel.VectorP.X * invNor);
                rayTracingModel.VectorP.Y = Convert.ToInt32(rayTracingModel.VectorP.Y * invNor);
                rayTracingModel.VectorP.Z = Convert.ToInt32(rayTracingModel.VectorP.Z * invNor);


            }

        }


        public bool PolygonIntersection(ref RayTracingModel rayTracingModel, int i, int j,bool sphere)
        {
            try
            {
                rayTracingModel.L1 = VectorSubstraction(rayTracingModel.Polygon.V1, rayTracingModel.Polygon.V0);
                rayTracingModel.L2 = VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V0);

                rayTracingModel.N = GetCrossProduct(rayTracingModel.L1, rayTracingModel.L2);
                double denom = VectorDot(rayTracingModel.N, rayTracingModel.N);

                double NdotRayDirection = VectorDot(rayTracingModel.N, rayTracingModel.VectorP0);

                if (Math.Abs(NdotRayDirection) < 1e-8) // almost 0
                    return false; // they are parallel so they don't intersect ! 






                double d = VectorDot(rayTracingModel.N, rayTracingModel.Polygon.V0);

                // compute t (equation 3)
                double t = -(VectorDot(rayTracingModel.N, rayTracingModel.v0) + d) / NdotRayDirection;
                rayTracingModel.tpolygon = t;

                
                // check if the triangle is in behind the ray
                if (t < 0) return false; // the triangle is behind

                // compute the intersection point using equation 1
                Vector P = VectorAddition(rayTracingModel.v0, Multiply(rayTracingModel.VectorP0, t));

                // Step 2: inside-outside test
                Vector C; // vector perpendicular to triangle's plane

                // edge 0
                Vector edge0 = VectorSubstraction(rayTracingModel.Polygon.V1, rayTracingModel.Polygon.V0);
                Vector vp0 = VectorSubstraction(P, rayTracingModel.Polygon.V0);
                C = GetCrossProduct(edge0, vp0);

                if (VectorDot(rayTracingModel.N, C) < 0) return false; // P is on the right side

                // edge 1
                Vector edge1 = VectorSubstraction(rayTracingModel.Polygon.V2, rayTracingModel.Polygon.V1);

                Vector vp1 = VectorSubstraction(P, rayTracingModel.Polygon.V1);
                C = GetCrossProduct(edge1, vp1);
                double u = VectorDot(rayTracingModel.N, C);

                if (VectorDot(rayTracingModel.N, C) < 0) return false; // P is on the right side


                // edge 2

                Vector edge2 = VectorSubstraction(rayTracingModel.Polygon.V0, rayTracingModel.Polygon.V2);

                Vector vp2 = VectorSubstraction(P, rayTracingModel.Polygon.V2);
                C = GetCrossProduct(edge2, vp2);
                double v = VectorDot(rayTracingModel.N, C);

                if (VectorDot(rayTracingModel.N, C) < 0) return false; // P is on the right side


                u /= denom;
                v /= denom;
                Vector red = new Vector(255, 0, 0);
                Vector white = new Vector(255, 255, 255);

                Vector black = new Vector(0, 0, 0);
                Vector col = new Vector();
                u = u * 255;
                v = v * 255;

                rayTracingModel.pi = VectorAddition(rayTracingModel.v0, Multiply(rayTracingModel.VectorP0, (double)rayTracingModel.tpolygon));

                //calculate LRP-pi
                rayTracingModel.VectorL = VectorSubstraction(rayTracingModel.LRP, rayTracingModel.pi);

                //get plane normal
                rayTracingModel.VectorN = GetNormal();

                //calculate dt
                double dt = VectorDot((rayTracingModel.VectorN), Normalize(rayTracingModel.VectorL));


                col.X = u * 255;
                col.Y = v * 255;
                col.Z = (1 - u - v) * 255;

                rayTracingModel.polygondt = dt;
                //shading
              

                //col.X = (col.X > 255) ? 255 : (col.X < 0) ? 0 : u;
                //col.Y = (col.Y > 255) ? 255 : (col.Y < 0) ? 0 : v;
                //col.Z = (col.Z > 255) ? 255 : (col.Z < 0) ? 0 : col.Z;
                ////  rayTracingModel.bitmap.SetPixel(i, j,Color.FromArgb(Convert.ToInt32(tcolor)));


                ////col = red;

                //rayTracingModel.bitmap.SetPixel(i, j, Color.FromArgb((int)col.X, (int)col.Y, (int)col.Z));


                return true; // this ray hits the triangle
            
            }
            catch (Exception)
            {


            }
            return true;
        }


    }
}
