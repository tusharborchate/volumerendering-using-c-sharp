using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment3
{
    public class VolumeRenderingLogic
    {
        RayTracingModel rayTracingModel = new RayTracingModel();

        public VolumeRenderingLogic()
        {
            rayTracingModel.VRP = new Vector();
            rayTracingModel.VRP.X = 128f;
            rayTracingModel.VRP.Y = 64f;
            rayTracingModel.VRP.Z = 250f;


            rayTracingModel.VPN = new Vector();
            rayTracingModel.VPN.X = -64f;
            rayTracingModel.VPN.Y = 0f;
            rayTracingModel.VPN.Z = -186f;

            rayTracingModel.VUP = new Vector();
            rayTracingModel.VUP.X = 0;
            rayTracingModel.VUP.Y = 1f;
            rayTracingModel.VUP.Z = 0f;
            loadCTData();
        }
        public VolumeRenderingLogic(float rotation, float elevation)
        {
            rayTracingModel.VRP = new Vector();
            rayTracingModel.VRP.X = 128f;
            rayTracingModel.VRP.Y = 64f;
            rayTracingModel.VRP.Z = 250f;


            rayTracingModel.VPN = new Vector();
            rayTracingModel.VPN.X = -64f;
            rayTracingModel.VPN.Y = 0f;
            rayTracingModel.VPN.Z = -186f;

            rayTracingModel.VUP = new Vector();
            rayTracingModel.VUP.X = 0f;
            rayTracingModel.VUP.Y = -1f;
            rayTracingModel.VUP.Z = 0f;
            loadCTData();

            rayTracingModel.rotation = rotation;
            rayTracingModel.elevation = elevation;
        }
        float[,,] shadingData = new float[128, 128, 128];
        float[,,] ctData = new float[128, 128, 128];
        DateTime dt2 = new DateTime();
        DateTime dt1 = new DateTime();
        int[,] imageBuffer = new int[200, 200];

        int[,] imageBuffer1 = new int[128, 128];
        int[,] imageBuffer2 = new int[128, 128];
        int[,] imageBuffer3 = new int[128, 128];
        int[,] imageBuffer4 = new int[128, 128];
        int[,] imageBuffer5 = new int[128, 128];

        private static float ZOOM_IN = -10;       // Constant which define amount of zoom in
        private static float ZOOM_OUT = 10;       // Constant which define amount of zoom out
        private Vector objPosition = new Vector(64f, 64f, 64f);      // A point around which camera is rotated


        public class RenderColor
        {
            public float opactity { get; set; }
            public float shading { get; set; }
            public float T { get; set; }

        }

        public string VolumeRendering(int choice, float elevation, float rotation, float motion, float zoomin, float zoomout)
        {
            try
            {

                rayTracingModel.MOTION_VALUE = motion;
                rayTracingModel.ZOOM_IN = zoomin;
                rayTracingModel.ZOOM_OUT = zoomout;
                //    rayTracingModel.elevation = elevation;



                Console.WriteLine("please wait generating image.. it will take between 1-3 minutes");
                //store data
                float[,,] ctData = new float[128, 128, 128];

                //set camera

                rayTracingModel.LightVector = new Vector();
                rayTracingModel.LightVector.X = 0.577f;
                rayTracingModel.LightVector.Y = -0.577f;
                rayTracingModel.LightVector.Z = -0.577f;

                rayTracingModel.Width = 200;

                //screen  height
                rayTracingModel.Height = 200;

                //focal
                rayTracingModel.Focal = 0.05f;

                //Light coodinate system
                rayTracingModel.LRP = new Vector();




                //Light intensity
                rayTracingModel.Ip = 255;

                //screen size
                rayTracingModel.Xmin = -0.0175f;
                rayTracingModel.Ymin = -0.0175f;
                rayTracingModel.Xmax = 0.0175f;
                rayTracingModel.Ymax = 0.0175f;

                GetUnitVector();


                if (choice != 0 && choice != 5 && choice != 6)
                {
                    orbitCamera(choice);
                }
                if (choice == 5 || choice == 6)
                {
                    float zoomValue = choice == 5 ? rayTracingModel.ZOOM_IN : rayTracingModel.ZOOM_OUT;
                    // find unit vector directed towards object
                    Vector unitVector = Normalize(rayTracingModel.VPN);
                    // update zoom value
                    Vector zoomVector = Multiply(unitVector, (zoomValue));
                    // re calculate Reference Point using the zoom vector
                    rayTracingModel.VRP = VectorAddition(rayTracingModel.VRP, zoomVector);

                }


                updateCamera();
                GetUnitVector();
                computeShadingData();

                // renderFrame();
                // firstly shading value for each voxel is calculated
                MethodInfo m = this.GetType().GetMethod("renderFrame");
                Parallel.Invoke(() => m.Invoke(this, new object[] { 0, 25, 0, 25 })
                , () => m.Invoke(this, new object[] { 0, 25, 25, 50 }),
                () => m.Invoke(this, new object[] { 0, 25, 50, 75 }),
                () => m.Invoke(this, new object[] { 0, 25, 75, 100 }),
                () => m.Invoke(this, new object[] { 0, 25, 100, 125 }),
                  () => m.Invoke(this, new object[] { 0, 25, 125, 150 }),
                    () => m.Invoke(this, new object[] { 0, 25, 150, 175 }),
                      () => m.Invoke(this, new object[] { 0, 25, 175, 200 }),
                    //      () => m.Invoke(this, new object[] { 0, 25, 200, 225 }),
                    //        () => m.Invoke(this, new object[] { 0, 25, 225, 128 }),

                    () => m.Invoke(this, new object[] { 25, 50, 0, 25 })
                , () => m.Invoke(this, new object[] { 25, 50, 25, 50 }),
                () => m.Invoke(this, new object[] { 25, 50, 50, 75 }),
                () => m.Invoke(this, new object[] { 25, 50, 75, 100 }),
                () => m.Invoke(this, new object[] { 25, 50, 100, 125 }),
                 () => m.Invoke(this, new object[] { 25, 50, 125, 200 }),
                  () => m.Invoke(this, new object[] { 25, 50, 150, 175 }),
                    () => m.Invoke(this, new object[] { 25, 50, 175, 200 }),
                //      () => m.Invoke(this, new object[] { 25,50, 200, 225 }),
                //        () => m.Invoke(this, new object[] { 25,50, 225, 128 }),

                () => m.Invoke(this, new object[] { 50, 75, 0, 25 })
                , () => m.Invoke(this, new object[] { 50, 75, 25, 50 }),
                () => m.Invoke(this, new object[] { 50, 75, 50, 75 }),
                () => m.Invoke(this, new object[] { 50, 75, 75, 100 }),
                () => m.Invoke(this, new object[] { 50, 75, 100, 128 }),
                  () => m.Invoke(this, new object[] { 50, 75, 125, 150 }),
                    () => m.Invoke(this, new object[] { 50, 75, 150, 175 }),
                      () => m.Invoke(this, new object[] { 50, 75, 175, 200 }),
                // () => m.Invoke(this, new object[] { 50, 75, 200, 225 }),
                //        () => m.Invoke(this, new object[] { 50 , 75, 225, 128 })
                //,
                () => m.Invoke(this, new object[] { 75, 100, 0, 25 })
                , () => m.Invoke(this, new object[] { 75, 100, 25, 50 }),
                () => m.Invoke(this, new object[] { 75, 100, 50, 75 }),
                () => m.Invoke(this, new object[] { 75, 100, 75, 100 }),
                () => m.Invoke(this, new object[] { 75, 100, 100, 125 }),
                  () => m.Invoke(this, new object[] { 75, 100, 125, 150 }),
                    () => m.Invoke(this, new object[] { 75, 100, 150, 175 }),
                      () => m.Invoke(this, new object[] { 75, 100, 175, 200 }),
                           //   () => m.Invoke(this, new object[] { 75, 100, 200, 225 }),
                           //        () => m.Invoke(this, new object[] { 75,100, 225, 128 })

                           //        ,

                           () => m.Invoke(this, new object[] { 100, 150, 0, 25 })
                , () => m.Invoke(this, new object[] { 100, 150, 25, 50 }),
                () => m.Invoke(this, new object[] { 100, 150, 50, 75 }),
                () => m.Invoke(this, new object[] { 100, 150, 75, 100 }),
                () => m.Invoke(this, new object[] { 100, 150, 100, 125 }),
                  () => m.Invoke(this, new object[] { 100, 150, 125, 150 }),
                    () => m.Invoke(this, new object[] { 100, 150, 150, 175 }),
                      () => m.Invoke(this, new object[] { 100, 150, 175, 200 }),
                            //    () => m.Invoke(this, new object[] { 100, 150, 200, 225 }),
                            //        () => m.Invoke(this, new object[] { 100,150, 225, 128 }),

                            () => m.Invoke(this, new object[] { 150, 200, 0, 25 })
                  , () => m.Invoke(this, new object[] { 150, 200, 25, 50 }),
                  () => m.Invoke(this, new object[] { 150, 200, 50, 75 }),
                  () => m.Invoke(this, new object[] { 150, 200, 75, 100 }),
                  () => m.Invoke(this, new object[] { 150, 200, 100, 125 }),
                    () => m.Invoke(this, new object[] { 150, 200, 125, 150 }),
                      () => m.Invoke(this, new object[] { 150, 200, 150, 175 }),
                        () => m.Invoke(this, new object[] { 150, 200, 175, 200 })
                  //      () => m.Invoke(this, new object[] { 150, 200, 200, 225 }),
                  //  //        () => m.Invoke(this, new object[] { 150,200, 225, 128 }),

                  //          () => m.Invoke(this, new object[] { 200,225, 0, 25 })
                  //, () => m.Invoke(this, new object[] { 200,225, 25, 50 }),
                  //() => m.Invoke(this, new object[] { 200,225, 50,75 }),
                  //() => m.Invoke(this, new object[] { 200,225, 75,100 }),
                  //() => m.Invoke(this, new object[] { 200,225, 100, 125 }),
                  //  //() => m.Invoke(this, new object[] { 200,225, 125, 150 }),
                  //  //  () => m.Invoke(this, new object[] { 200,225, 150, 175 }),
                  //  //    () => m.Invoke(this, new object[] { 200,225, 175, 200 }),
                  //  //      () => m.Invoke(this, new object[] { 200,225, 200, 225 }),
                  //  //        () => m.Invoke(this, new object[] { 200,225, 225, 128 })


                  //          () => m.Invoke(this, new object[] { 225,128, 0, 25 })
                  //, () => m.Invoke(this, new object[] { 225,128, 25, 50 }),
                  //() => m.Invoke(this, new object[] { 225,128, 50,75 }),
                  //() => m.Invoke(this, new object[] { 225,128, 75,100 }),
                  //() => m.Invoke(this, new object[] { 225,128, 100, 125 })
                  //  //() => m.Invoke(this, new object[] { 225,128, 125, 150 }),
                  //  //  () => m.Invoke(this, new object[] { 225,128, 150, 175 }),
                  //  //    () => m.Invoke(this, new object[] { 225,128, 175, 200 }),
                  //  //      () => m.Invoke(this, new object[] { 225,128, 200, 225 }),
                  //  //        () => m.Invoke(this, new object[] { 225,128, 225, 128 })
                  //  //
                  );

                //List<string> str = new List<string>();
                //List<string> str1 = new List<string>();

                //for (int i = 0; i < 128; i+=2)
                //{

                //    for (int j = 0; j < 250; j+=2)
                //    {
                //        str.Add(i + "." + (i + 2));

                //        str1.Add(j + "." + (j + 2));

                //    }



                //}

                //renderFrame(0, 128, 0, 128);
                ////Parallel.For(0, str.Count(), i => {
                //    renderFrame(Convert.ToInt32(str[i].Split('.')[0]), Convert.ToInt32(str[i].Split('.')[1]),

                //        Convert.ToInt32(str1[i].Split('.')[0]), Convert.ToInt32(str1[i].Split('.')[1]));

                //});

                // Parallel.ForEach(str, (temp) => renderFrame(Convert.ToInt32(temp.Split('.')[0]), Convert.ToInt32(temp.Split('.')[1])));
                //str = new List<string>();
                //for (int i = 0; i < 500; i += 10)
                //{


                //    str.Add(i + "." + (i + 10));
                //}


                //Parallel.ForEach(str, (temp) => renderFrame(Convert.ToInt32(temp.Split('.')[0]), Convert.ToInt32(temp.Split('.')[1])));





                // Parallel.Invoke(() => m.Invoke(this, new object[] { 0, 20 }), () => m.Invoke(this, new object[] { 20, 40 })
                //, () => m.Invoke(this, new object[] { 40, 60 })
                //               , () => m.Invoke(this, new object[] { 60, 80 })
                //                , () => m.Invoke(this, new object[] { 80, 100 })

                // );


                saveImageInPng(imageBuffer);
                return rayTracingModel.imgpath;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Vector VectorAddition(Vector vector1, Vector vector2)
        {
            Vector point = new Vector();
            point.X = vector1.X + vector2.X;
            point.Y = vector1.Y + vector2.Y;
            point.Z = vector1.Z + vector2.Z;

            return point;

        }



        public void updateCamera()
        {
            Vector objPosition = new Vector(64f, 64f, 64f);
            Vector objPosition1 = new Vector(63.9f, 64f, 64f);

            // VPN is calculating by subtracting Obj Position and VRP
            rayTracingModel.VPN1 = VectorSubstraction(rayTracingModel.VRP, objPosition);


            Vector anotherVector = VectorSubstraction(objPosition1, rayTracingModel.VRP);

            // An arbitrary Vector is calculated to find the VUP
            // cross product between VPN and the arbitrary Vector helps to compute a Up Vector
            rayTracingModel.VUP1 = GetCrossProduct(anotherVector, rayTracingModel.VPN1);
            rayTracingModel.VUP1 = Normalize(rayTracingModel.VUP1);

            /*if((prevVUP.getY()>0 && vectorVup.getY()<0) ||(prevVUP.getY()<0 && vectorVup.getY()>0)){
                this.vectorVup= vectorVpn.crossProduct(anotherVector).getUnitVector();
            }*/
            if ((Math.Abs(rayTracingModel.VUP.Y - rayTracingModel.VUP1.Y) > .01) && (Math.Abs(rayTracingModel.VUP.Y) == Math.Abs(rayTracingModel.VUP1.Y)))
            {
                rayTracingModel.VUP1 = GetCrossProduct(rayTracingModel.VPN1, anotherVector);
                rayTracingModel.VUP1 = Normalize(rayTracingModel.VUP1);

            }
            /*float x=0;
            float y=1;
            float z=  -(x*vectorVpn.getX() + y*vectorVpn.getY())/ vectorVpn.getZ() ;
            vectorVup.setX(x);
            vectorVup.setY(y);
            vectorVup.setZ(z);*/
            rayTracingModel.VUP1 = Normalize(rayTracingModel.VUP1);
            //      System.out.println(String.format("x-%s,y- %s,z- %s",vectorVup.getX(),vectorVup.getY(),vectorVup.getZ()));

            rayTracingModel.VPN = rayTracingModel.VPN1;
            rayTracingModel.VUP = rayTracingModel.VUP1;

        }

        //read binary file

        public float[,,] loadCTData()
        {

            try
            {
                using (FileStream streamReader = new FileStream("smallHead.den", FileMode.Open))
                {
                    byte[] data = new byte[128 * 128];
                    int byteRead;
                    byte[] bytes;
                    if (File.Exists("output.raw"))
                    {
                        try
                        {
                            File.Delete("output.raw");
                        }
                        catch (Exception ex)
                        {
                            //Do something
                        }
                    }
                    FileStream buffer = new FileStream("output.raw", FileMode.CreateNew);

                    try
                    {
                        int layer = 0;
                        while ((byteRead = streamReader.Read(data, 0, data.Length)) != -1)
                        {
                            buffer.Write(data, 0, byteRead);

                            for (int i = 0; i < 128; i++)
                            {
                                for (int j = 0; j < 128; j++)
                                {
                                    int i1 = data[i * 128 + j] & 0xff;
                                    // Ignores data if opacity is less than given Opacity
                                    if (i1 < 40)
                                    {
                                        ctData[layer, i, j] = 0;
                                    }
                                    else
                                    {
                                        float i2 = i1;       // opacity is divided by 255 get floating point value
                                        ctData[layer, i, j] = i2;
                                    }
                                }
                            }
                            layer++;
                            if (layer > 127)
                            {
                                break;
                            }
                        }
                        buffer.Flush();

                        return ctData;
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }



            return null;
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

                float modVPN = (rayTracingModel.VPN.X * rayTracingModel.VPN.X) +
                    (rayTracingModel.VPN.Y * rayTracingModel.VPN.Y) + (rayTracingModel.VPN.Z * rayTracingModel.VPN.Z);
                modVPN = (float)Math.Sqrt(modVPN);

                //calculate N
                vectorN.X = rayTracingModel.VPN.X / modVPN;
                vectorN.Y = rayTracingModel.VPN.Y / modVPN;
                vectorN.Z = rayTracingModel.VPN.Z / modVPN;

                //calculate vector u

                //u=vup*vpn/|vup*vpn|

                // vup = v2 , vpn = v1
                Vector crossProductVUPVPN = this.GetCrossProduct(rayTracingModel.VPN, rayTracingModel.VUP);

                rayTracingModel.angle = this.GetAngle(rayTracingModel.VPN, rayTracingModel.VUP);
                float vectormod = this.GetVectorModResult(rayTracingModel.VUP, rayTracingModel.VPN, true);

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
                float[,] m = rayTracingModel.mtrixT;
                rayTracingModel.matrixTInv = this.MatrixInverse(ref m);
                rayTracingModel.matrixMWC = this.MatrixMultiuplication(rayTracingModel.matrixR, rayTracingModel.matrixT);
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
            float[,] matrixR = new float[4, 4];
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

            float[,] matrixT = new float[4, 4];
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

            rayTracingModel.mtrixT = matrixT;

            rayTracingModel.matrixR = matrixR;
            rayTracingModel.matrixT = matrixT.Clone() as float[,];

        }

        public Vector MultiplyVector(float[,] matrix, Vector vector)
        {
            Vector v = new Vector();
            try
            {
                v.X = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z + matrix[0, 3] * vector.W;
                v.Y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z + matrix[1, 3] * vector.W;

                v.Z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z + matrix[2, 3] * vector.W;
                v.W = matrix[3, 0] * vector.X + matrix[3, 1] * vector.Y + matrix[3, 2] * vector.Z + matrix[3, 3] * vector.W;


            }
            catch (Exception ex)
            {

                throw;
            }
            return v;

        }

        public float[,] MatrixMultiuplication(float[,] matrix1, float[,] matrix2)
        {
            float[,] matrixResult = new float[4, 4];
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


        public float[,] MatrixTranspose(float[,] matrix1)
        {
            float[,] matrixResult = new float[4, 4];
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

        public float[,] MatrixInverse(ref float[,] matrix1)
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




        public float GetVectorModResult(Vector v1, Vector v2, bool isAngle)
        {
            float result = 0.0f;
            try
            {

                float v1mod = (float)Math.Sqrt((v1.X * v1.X) + (v1.Y * v1.Y) + (v1.Z * v1.Z));
                float v2mod = (float)Math.Sqrt((v2.X * v2.X) + (v2.Y * v2.Y) + (v2.Z * v2.Z));

                result = v1mod * v2mod;

                if (isAngle)
                {
                    result = result * (float)Math.Sin(rayTracingModel.angle * Math.PI / 180);
                }

            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }
            return result;
        }

        public float GetAngle(Vector v1, Vector v2)
        {
            try
            {
                float vectormod = GetVectorModResult(v1, v2, false);
                rayTracingModel.angle = ((v1.X * v2.X) + (v1.Y * v2.Y)) / vectormod;
                rayTracingModel.angle = (float)Math.Acos(rayTracingModel.angle);

                //convert it into degrees
                rayTracingModel.angle = (float)rayTracingModel.angle * 180 / (float)Math.PI;

            }
            catch (Exception)
            {

                Console.WriteLine("\n" + "Please change your input values..");
            }
            return rayTracingModel.angle;
        }
        public Vector Multiply(Vector t, float f)
        {

            Vector v = new Vector();
            v.X = t.X * f;
            v.Y = t.Y * f;
            v.Z = t.Z * f;

            return v;
        }

        public float toRad(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public void Normalize1(Vector vector)
        {
            float result = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
            if (result > 0)
            {
                float invNor = 1 / (float)Math.Sqrt(result);

                rayTracingModel.VectorP.X = Convert.ToInt32(rayTracingModel.VectorP.X * invNor);
                rayTracingModel.VectorP.Y = Convert.ToInt32(rayTracingModel.VectorP.Y * invNor);
                rayTracingModel.VectorP.Z = Convert.ToInt32(rayTracingModel.VectorP.Z * invNor);


            }

        }
        private static IEnumerable<float> Iterate(float fromInclusive, float toExclusive, float step)
        {
            for (float d = fromInclusive; d < toExclusive; d += step)
                yield return d;
        }

        public void renderlist(float count, float t, int k, RayTracingModel ray1, ref List<RenderColor> list, int i)
        {

            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            RenderColor renderColor = new RenderColor();

            k = k + 1;
            //  System.out.print(".");
            // Get a point in the cube, this point will be surrounded by other values
            Vector point = getPointWithT(t, ray1);
            /* Obtain the shading value C and opacity value A
            from the shading volume and CT volume, respectively,
            by using tri-linear interpolation. */


            float opacity = 0;
            try
            {
                // calculate opacity using tri-linear interpolation
                opacity = get3dInterpolatedValue(ctData, point);
                opacity = opacity / 255.0f;
                renderColor.opactity = opacity;

            }
            catch (Exception e)


            {
                //  throw e;
            }

            float shading = 0;
            try
            {
                // calculate shading using tri-linear interpolation
                shading = get3dInterpolatedValue(shadingData, point);
                renderColor.shading = shading;

            }
            catch (Exception e)
            {
                //continue;
                //
            }
            try
            {
                list[i] = renderColor;
            }
            catch (Exception ex)
            {


            }
        }
        public void renderFrame(int x1, int x2, int j1, int j2)
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = x1; i < x2; i++)
            {
                for (int j = j1; j < j2; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }



        public void orbitCamera(int choice)
        {
            switch (choice)
            {
                case 1:
                    rayTracingModel.rotation += rayTracingModel.MOTION_VALUE;

                    break;
                case 2:
                    rayTracingModel.rotation -= rayTracingModel.MOTION_VALUE;

                    break;
                case 3:
                    //      rotation+= MOTION_VALUE;
                    rayTracingModel.elevation += rayTracingModel.MOTION_VALUE;
                    break;
                case 4:
                    rayTracingModel.elevation -= rayTracingModel.MOTION_VALUE;
                    break;
                default:

                    break;
            }
            // Calculate new position of camera using equation
            // x= r*Sin(rotation)*sin(elevation)
            // y= r*cos(elevation)
            // x= r*Sin(elevation)*cos(rotation)
            // these lengths added to objPosition
            float z = objPosition.Z + getMagnitude(rayTracingModel.VPN) *
                    (float)Math.Sin(toRad(rayTracingModel.elevation)) * (float)Math.Cos(toRad(rayTracingModel.rotation));
            float x = objPosition.X + getMagnitude(rayTracingModel.VPN) *
                   (float)Math.Sin(toRad(rayTracingModel.rotation)) * (float)Math.Sin(toRad(rayTracingModel.elevation));
            float y = objPosition.Y + getMagnitude(rayTracingModel.VPN) * (float)Math.Cos(toRad(rayTracingModel.elevation));

            rayTracingModel.VRP.X = x;
            rayTracingModel.VRP.Y = y;
            rayTracingModel.VRP.Z = z;


            //        System.out.println(String.format("x-%s,y- %s,z- %s, rotation %s, elev %s sin-%s",
            //                x,y,z,rotation,elevation,Math.sin(toRad(rotation)) ));
            // refresh camera to update VPN and VUP
            //  updateCamera();

        }

        public void renderFrame11()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 0; i < 50; i++)
            {
                for (int j = 50; j < 100; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame12()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 150; j < 200; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame13()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 200; j < 220; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame14()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 220; j < 128; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }


        public void renderFrame2()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 100; i < 200; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame21()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 100; i < 200; i++)
            {
                for (int j = 100; j < 200; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame22()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 100; i < 200; i++)
            {
                for (int j = 200; j < 128; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        //public void renderFrame23()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 100; i < 200; i++)
        //    {
        //        for (int j = 220; j < 128; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame24()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 100; i < 200; i++)
        //    {
        //        for (int j = 400; j < 128; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}


        public void renderFrame3()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 200; i < 128; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame31()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 200; i < 128; i++)
            {
                for (int j = 100; j < 200; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        public void renderFrame32()
        {
            int counttime = 0;
            Console.WriteLine("renderframe");
            RayTracingModel ray1 = new RayTracingModel();

            // rows and columns are initialized for the camera
            int rows = 128;
            int columns = 128;
            int count = 0;
            dt2 = DateTime.Now;
            for (int i = 200; i < 128; i++)
            {
                for (int j = 200; j < 128; j++)
                {
                    count = count + 1;

                    // Ray is calculated for each point on 3.5 mm film
                    float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
                    float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

                    //vector origin
                    ray1.VectorO = new Vector();
                    ray1.VectorO.X = 0;
                    ray1.VectorO.Y = 0;
                    ray1.VectorO.Z = 0;
                    ray1.VectorO.W = 1;

                    //transforming origin
                    ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
                    ray1.v0 = ray1.VectorO1;


                    //this code can help you for generating xmin,xmax,ymin,ymax 
                    //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                    //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



                    //Vector ray direction
                    Vector vectorP = new Vector();
                    vectorP.X = x;
                    vectorP.Y = y;
                    vectorP.Z = rayTracingModel.Focal;
                    vectorP.W = 1;


                    //transformaing direction ray

                    ray1.VectorP = vectorP;
                    ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


                    //normalizing ray
                    ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

                    ray1.VectorP0 = Normalize(ray1.VectorP1);



                    List<float> tValues = rayboxintersection(ray1);
                    if (tValues == null)
                    {      // it means no intersection
                        continue;
                    }
                    // we arrange t0 and t1 such that t0<t1
                    float t0 = Math.Min(tValues[0], tValues[1]);
                    float t1 = Math.Max(tValues[0], tValues[1]);

                    float dt = 1.0f; // the interval for sampling along the ray
                    float C = 0.0f;   // for accumulating the shading value
                    float T = 1.0f;   // for accumulating the transparency

                    // we start traversing in the cube and find integrating the values of color and shading
                    // over different values of t for a particular ray
                    // this shading value is stored in a pixel in camera
                    int k = 0;
                    for (float t = t0; t <= t1; t += dt)
                    {
                        k = k + 1;
                        //  System.out.print(".");
                        // Get a point in the cube, this point will be surrounded by other values
                        Vector point = getPointWithT(t, ray1);
                        /* Obtain the shading value C and opacity value A
                        from the shading volume and CT volume, respectively,
                        by using tri-linear interpolation. */

                        float opacity = 0;
                        try
                        {
                            // calculate opacity using tri-linear interpolation
                            opacity = get3dInterpolatedValue(ctData, point);
                            opacity = opacity / 255.0f;

                        }
                        catch (Exception e)


                        {
                            //  throw e;
                        }

                        float shading = 0;
                        try
                        {
                            // calculate shading using tri-linear interpolation
                            shading = get3dInterpolatedValue(shadingData, point);

                        }
                        catch (Exception e)
                        {
                            //continue;
                            //
                        }

                        /* if(opacity<0||shading<0){
                             System.err.println("Opacity or shading is negative");
                         }*/
                        /* Accumulate the shading values in the front-to-back order.
                        Note: You will accumulate the transparency. This value
                        can be used in the for-loop for early termination.
                        */

                        if (T < 0.01f)
                        {
                            // if value of T goes beyond certain value we can terminate loop
                            break;
                        }
                        C += opacity * shading * T;      // value of color and shading is accumulated
                                                         // For the next loop transparency will be updated as transparency decreases
                                                         // each time we keep on penetrating the cube
                        T = T * (1 - opacity);

                    }


                    //  System.out.println( C);
                    /*if(C<0){
                        System.err.println("C is negative "+ C);
                    }*/
                    // value is saved in image pixel
                    imageBuffer[i, j] = (int)C;

                }


            }

            dt1 = DateTime.Now;
            int time = (dt1 - dt2).Seconds;
            Console.WriteLine(time);

            //    System.out.println("Z error is "+zerr);


        }
        //public void renderFrame33()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 200; i < 300; i++)
        //    {
        //        for (int j = 300; j < 400; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame34()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 200; i < 300; i++)
        //    {
        //        for (int j = 400; j < 128; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}



        //public void renderFrame4()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 300; i < 400; i++)
        //    {
        //        for (int j = 0; j < 100; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame41()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 300; i < 400; i++)
        //    {
        //        for (int j = 100; j < 200; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame42()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 300; i < 400; i++)
        //    {
        //        for (int j = 200; j < 300; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame43()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 300; i < 400; i++)
        //    {
        //        for (int j = 300; j < 400; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame44()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 300; i < 400; i++)
        //    {
        //        for (int j = 400; j < 128; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}

        //public void renderFrame5()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 400; i < 128; i++)
        //    {
        //        for (int j = 0; j < 100; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame51()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 400; i < 128; i++)
        //    {
        //        for (int j = 100; j < 200; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame52()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 400; i < 128; i++)
        //    {
        //        for (int j = 200; j < 300; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame53()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 400; i < 128; i++)
        //    {
        //        for (int j = 300; j < 400; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = (dt1 - dt2).Seconds;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame54()
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = 400; i < 128; i++)
        //    {
        //        for (int j = 400; j < 128; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = dt1.Minute-dt2.Minute;
        //    time = time * 60;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}
        //public void renderFrame55(int i1,int i2,int j1,int j2)
        //{
        //    int counttime = 0;
        //    Console.WriteLine("renderframe");
        //    RayTracingModel ray1 = new RayTracingModel();

        //    // rows and columns are initialized for the camera
        //    int rows = 128;
        //    int columns = 128;
        //    int count = 0;
        //    dt2 = DateTime.Now;
        //    for (int i = i1; i < i2; i++)
        //    {
        //        for (int j = j1; j < j2; j++)
        //        {
        //            count = count + 1;

        //            // Ray is calculated for each point on 3.5 mm film
        //            float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (128 - 1)) + rayTracingModel.Xmin;
        //            float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (128 - 1)) + rayTracingModel.Ymin;

        //            //vector origin
        //            ray1.VectorO = new Vector();
        //            ray1.VectorO.X = 0;
        //            ray1.VectorO.Y = 0;
        //            ray1.VectorO.Z = 0;
        //            ray1.VectorO.W = 1;

        //            //transforming origin
        //            ray1.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorO);
        //            ray1.v0 = ray1.VectorO1;


        //            //this code can help you for generating xmin,xmax,ymin,ymax 
        //            //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
        //            //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



        //            //Vector ray direction
        //            Vector vectorP = new Vector();
        //            vectorP.X = x;
        //            vectorP.Y = y;
        //            vectorP.Z = rayTracingModel.Focal;
        //            vectorP.W = 1;


        //            //transformaing direction ray

        //            ray1.VectorP = vectorP;
        //            ray1.VectorP1 = MultiplyVector(rayTracingModel.matrixMCW, ray1.VectorP);


        //            //normalizing ray
        //            ray1.VectorP1 = (VectorSubstraction(ray1.VectorP1, ray1.VectorO1));

        //            ray1.VectorP0 = Normalize(ray1.VectorP1);



        //            List<float> tValues = rayboxintersection(ray1);
        //            if (tValues == null)
        //            {      // it means no intersection
        //                continue;
        //            }
        //            // we arrange t0 and t1 such that t0<t1
        //            float t0 = Math.Min(tValues[0], tValues[1]);
        //            float t1 = Math.Max(tValues[0], tValues[1]);

        //            float dt = 1.0f; // the interval for sampling along the ray
        //            float C = 0.0f;   // for accumulating the shading value
        //            float T = 1.0f;   // for accumulating the transparency

        //            // we start traversing in the cube and find integrating the values of color and shading
        //            // over different values of t for a particular ray
        //            // this shading value is stored in a pixel in camera
        //            int k = 0;
        //            for (float t = t0; t <= t1; t += dt)
        //            {
        //                k = k + 1;
        //                //  System.out.print(".");
        //                // Get a point in the cube, this point will be surrounded by other values
        //                Vector point = getPointWithT(t, ray1);
        //                /* Obtain the shading value C and opacity value A
        //                from the shading volume and CT volume, respectively,
        //                by using tri-linear interpolation. */

        //                float opacity = 0;
        //                try
        //                {
        //                    // calculate opacity using tri-linear interpolation
        //                    opacity = get3dInterpolatedValue(ctData, point);
        //                    opacity = opacity / 255.0f;

        //                }
        //                catch (Exception e)


        //                {
        //                    //  throw e;
        //                }

        //                float shading = 0;
        //                try
        //                {
        //                    // calculate shading using tri-linear interpolation
        //                    shading = get3dInterpolatedValue(shadingData, point);

        //                }
        //                catch (Exception e)
        //                {
        //                    //continue;
        //                    //
        //                }

        //                /* if(opacity<0||shading<0){
        //                     System.err.println("Opacity or shading is negative");
        //                 }*/
        //                /* Accumulate the shading values in the front-to-back order.
        //                Note: You will accumulate the transparency. This value
        //                can be used in the for-loop for early termination.
        //                */

        //                if (T < 0.01f)
        //                {
        //                    // if value of T goes beyond certain value we can terminate loop
        //                    break;
        //                }
        //                C += opacity * shading * T;      // value of color and shading is accumulated
        //                                                 // For the next loop transparency will be updated as transparency decreases
        //                                                 // each time we keep on penetrating the cube
        //                T = T * (1 - opacity);

        //            }


        //            //  System.out.println( C);
        //            /*if(C<0){
        //                System.err.println("C is negative "+ C);
        //            }*/
        //            // value is saved in image pixel
        //            imageBuffer[i, j] = (int)C;

        //        }


        //    }

        //    dt1 = DateTime.Now;
        //    int time = dt1.Minute - dt2.Minute;
        //    time = time * 60;
        //    Console.WriteLine(time);

        //    //    System.out.println("Z error is "+zerr);


        //}


        private void saveImageInPng(int[,] arr)
        {

            int xLength = 200;
            int yLength = 200;

            string s = "bitmap" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + ".bmp";

            rayTracingModel.imgpath = s;
            rayTracingModel.bitmap = new Bitmap(200, 200);
            int count = 0;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    count = count + 1;

                    int pixel = arr[y, x];
                    pixel = pixel % 128;


                    // a 32 bit integer is created with the pixel value
                    int rgb = unchecked((int)0xff000000) | (int)pixel << 16 | (int)pixel << 8 | (int)pixel;

                    rayTracingModel.bitmap.SetPixel(x, y, Color.FromArgb(rgb));
                }
            }
            rayTracingModel.bitmap.Save(s);



        }

        private void computeShadingData()
        {
            Vector normal = new Vector();
            for (int i = 1; i < 128 - 1; i++)
            {
                for (int j = 1; j < 128 - 1; j++)
                {
                    for (int k = 1; k < 128 - 1; k++)
                    {

                        // x,y,z component for gradient vector is calculated
                        float x = .5f * (ctData[i, j, k + 1] - ctData[i, j, k - 1]);
                        float y = .5f * (ctData[i, j + 1, k] - ctData[i, j - 1, k]);
                        float z = .5f * (ctData[i + 1, j, k] - ctData[i - 1, j, k]);
                        // normal vector is initialized
                        normal.X = x;
                        normal.Y = y;
                        normal.Z = z;

                        float mg = (float)Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
                        if (mg < 10)
                        {
                            shadingData[i, j, k] = 0;
                        }
                        else
                        {
                            // Unit vector is calculated
                            normal = Normalize(normal);                        // shading is calculated
                            normal = Normalize(normal);
                            float v = 200 * 0.75f * (float)VectorDot(normal, rayTracingModel.LightVector);
                            // if shading value is negative than we have to ignore those values as
                            // in that case normal will be in opposite direction to light
                            int value = (int)Math.Max(0, v);
                            if (value.ToString().Contains("E"))
                            {

                            }
                            shadingData[i, j, k] = v < 0 ? v * -1 : v;
                        }
                    }
                }
            }
        }

        public float VectorDot(Vector v1, Vector v2)
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


        public float getMagnitude(Vector vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /**
         * Calculate the unit vector of the given vector
         * @return Unit Vector for the given Vector
         */
        public Vector Normalize(Vector vector)
        {

            float mg = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            //mg = 1 / mg;
            vector.X = vector.X / mg;
            vector.Y = vector.Y / mg;
            vector.Z = vector.Z / mg;

            return vector;

        }
        public void RayConstruction(int i, int j, int M, int N)
        {
            try
            {


                //o is the starting vector which is vrp
                //image plane is equal to screen size
                //calclate p = (x,y,f)
                float x = ((rayTracingModel.Xmax - rayTracingModel.Xmin) * j / (M - 1)) + rayTracingModel.Xmin;
                float y = ((rayTracingModel.Ymax - rayTracingModel.Ymin) * i / (N - 1)) + rayTracingModel.Ymin;

                //vector origin
                rayTracingModel.VectorO = new Vector();
                rayTracingModel.VectorO.X = 0;
                rayTracingModel.VectorO.Y = 0;
                rayTracingModel.VectorO.Z = 0;
                rayTracingModel.VectorO.W = 1;

                //transforming origin
                rayTracingModel.VectorO1 = MultiplyVector(rayTracingModel.matrixMCW, rayTracingModel.VectorO);
                rayTracingModel.v0 = rayTracingModel.VectorO1;


                //this code can help you for generating xmin,xmax,ymin,ymax 
                //float scale = Convert.ToSingle(Math.Tan(deg2rad(Convert.ToSingle(rayTracingModel.fov * 0.5))));
                //float imageAspectRatio = rayTracingModel.Width / (float)rayTracingModel.Height;



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
                rayTracingModel.VectorP1 = (VectorSubstraction(rayTracingModel.VectorP1, rayTracingModel.VectorO1));

                rayTracingModel.VectorP0 = Normalize(rayTracingModel.VectorP1);


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


                //calculate mcw like assignment 1?
            }
            catch (Exception)
            {

                Console.WriteLine("Error occured in RayConstruction");
            }


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

        private List<float> rayboxintersection(RayTracingModel ray)
        {

            List<float> tList = new List<float>();
            // x minimum bound check - 0
            float tXMin = getTPointForPlane("x", 0, ray);
            if (tXMin != float.NegativeInfinity)
            {
                tList.Add(tXMin);
            }
            // x maximum bound check 127
            float tXMax = getTPointForPlane("x", 128 - 1, ray);
            if (tXMax != float.NegativeInfinity)
            {
                tList.Add(tXMax);
            }
            // y minimum bound check -0
            float tYMin = getTPointForPlane("y", 0, ray);
            if (tYMin != float.NegativeInfinity)
            {
                tList.Add(tYMin);
            }

            // y maximum bound check-127

            float tYMax = getTPointForPlane("y", 128 - 1, ray);
            if (tYMax != float.NegativeInfinity)
            {
                tList.Add(tYMax);
            }

            // z minimum bound check-0
            float tZMin = getTPointForPlane("z", 0, ray);
            if (tZMin != float.NegativeInfinity)
            {
                tList.Add(tZMin);
            }

            // z maximum bound check- 127
            float tZMax = getTPointForPlane("z", 128 - 1, ray);
            if (tZMax != float.NegativeInfinity)
            {
                tList.Add(tZMax);
            }


            if (tList.Count() == 2)
            {
                return tList;
            }

            return null;
        }


        public float getTPointForPlane(String plane, float value, RayTracingModel ray)
        {
            // we use ray equationg for finding value of t
            // this t gives us the value for which ray intersects plane of cube
            float t = getTvalueFor(plane, value, ray);
            if (t.ToString().Contains("E"))
            {
                t = 0;
            }
            float x = 0, y = 0, z = 0;
            // we check whether this t value is for a point which lies within range of our cube
            switch (plane)
            {
                case "x":
                    x = value;
                    y = ray.v0.Y + t * ray.VectorP0.Y;
                    z = ray.v0.Z + t * ray.VectorP0.Z;
                    // checks if the point lies in x face
                    if ((y > 0 && y < 128 - 1) && (z > 0 && z < 128 - 1))
                    {
                        return t;
                    }
                    break;
                case "y":
                    y = value;
                    x = ray.v0.X + t * ray.VectorP0.X;
                    z = ray.v0.Z + t * ray.VectorP0.Z;
                    // checks if the point lies in y face
                    if ((x > 0 && x < 128 - 1) && (z > 0 && z < 128 - 1))
                    {
                        return t;
                    }
                    break;
                case "z":
                    z = value;
                    x = ray.v0.X + t * ray.VectorP0.X;
                    y = ray.v0.Y + t * ray.VectorP0.Y;
                    // checks if the point lies in z face
                    if ((x > 0 && x < 128 - 1) && (y > 0 && y < 128 - 1))
                    {
                        return t;

                    }
                    break;

            }

            return float.NegativeInfinity;
        }

        public float getTvalueFor(String plane, float value, RayTracingModel ray)
        {



            float point = 0;
            float rayVectorComponent = 0;
            // check plane and assign point and rayVectorComponent values corresponding to given plane
            switch (plane)
            {
                case "x":
                    point = ray.v0.X;
                    rayVectorComponent = ray.VectorP0.X;
                    break;
                case "y":
                    point = ray.v0.Y;
                    rayVectorComponent = ray.VectorP0.Y;
                    break;
                case "z":
                    point = ray.v0.Z;
                    rayVectorComponent = ray.VectorP0.Z;
                    break;

            }
            float r = (float)rayVectorComponent;
            // use ray equation to value of t
            float t = (value - point) / r;
            float tt = (float)(value - point) / r;
            if (t.ToString().Contains("E"))
            {
                t = 0;
            }
            return (float)t;
        }

        public Vector getPointWithT(float t0, RayTracingModel ray)
        {
            float x = ray.v0.X + t0 * ray.VectorP0.X;
            float y = ray.v0.Y + t0 * ray.VectorP0.Y;
            float z = ray.v0.Z + (t0 * ray.VectorP0.Z);
            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }

            if (z < 0)
            {
                z = 0;
            }
            if (x.ToString().Contains("E"))
            {
                x = 0;
            }
            if (y.ToString().Contains("E"))
            {
                y = 0;
            }
            if (z.ToString().Contains("E"))
            {
                z = 0;
            }


            return new Vector(x, y, z);
        }
        private float get3dInterpolatedValue(float[,,] data, Vector point)
        {
            if (point.X.ToString().Contains("E"))
            {
                point.X = 0;
            }
            if (point.Y.ToString().Contains("E"))
            {
                point.Y = 0;
            }
            if (point.Z.ToString().Contains("E"))
            {
                point.Z = 0;
            }

            int xIndex = (int)Math.Floor(point.X);
            int yIndex = (int)Math.Floor(point.Y);
            int zIndex = (int)Math.Floor(point.Z);

            if (xIndex > 126)
            {
                xIndex = 126;
            }
            if (yIndex > 126)
            {
                yIndex = 126;
            }
            if (zIndex > 126)
            {
                zIndex = 126;
            }
            float f1 = data[zIndex, yIndex, xIndex];
            float f2 = 0;
            try
            {
                f2 = data[zIndex, yIndex, xIndex + 1];
            }
            catch (Exception e)
            {
                /*String x = "%s , %s , %s";
                x=String.format(x,point.getX(),yIndex,zIndex);
                System.out.println(x);*/
            }
            float s1 = oneDInterpolation(f1, f2, point.X);

            f1 = data[zIndex, yIndex + 1, xIndex];
            f2 = data[zIndex, yIndex + 1, xIndex + 1];
            float s2 = oneDInterpolation(f1, f2, point.X);

            f1 = data[zIndex + 1, yIndex, xIndex];
            f2 = data[zIndex + 1, yIndex, xIndex + 1];
            float s3 = oneDInterpolation(f1, f2, point.X);

            f1 = data[zIndex + 1, yIndex + 1, xIndex];
            f2 = data[zIndex + 1, yIndex + 1, xIndex + 1];
            float s4 = oneDInterpolation(f1, f2, point.X);


            float s5 = oneDInterpolation(s1, s3, point.Z);
            float s6 = oneDInterpolation(s2, s4, point.Z);

            float v = oneDInterpolation(s5, s6, point.Y);

            if (v.ToString().Contains("E"))
            {
                v = 0;
            }
            return v;
        }

        private float oneDInterpolation(float f1, float f2, float x)
        {
            // find the floor of the x
            // for example if point is 4.5
            // we will get first point x0= 4
            double x0 = Math.Floor(x);
            // similarly second point will x1=5
            double x1 = x0 + 1f; //Math.ceil(x);
                                 // This formula helps in interpolating value between two points
            float result = (float)(f1 * (x1 - x) + f2 * (x - x0));
            if (result.ToString().Contains("E"))
            {
                result = 0;
            }
            return result;
        }


    }
}
