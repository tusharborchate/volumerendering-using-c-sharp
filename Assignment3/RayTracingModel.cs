using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class RayTracingModel
    {
        internal float angle;
        internal float[,] matrixR;
        internal float[,] matrixT;
        internal float[,] matrixRT;
        internal float[,] matrixTInv;
        internal float[,] matrixMCW;
        internal float[,] matrixMWC;

        internal float[,] matrixRTranspose;
        internal float t;
        internal Vector v0;
        internal float fov=90;
        internal float polygondt;
        internal float[,] mtrixT;
        internal string imgpath;


        // Constant which define amount of rotation

        public float MOTION_VALUE = 100f;

        public float rotation = 2f;      // initial rotation
        public float elevation = 90f;    // initial elevation
        public float zoomValue = 1;

        public   float ZOOM_IN = -20;       // Constant which define amount of zoom in
        public   float ZOOM_OUT = 20;       // Constant which define amount of zoom out

        public float tpolygon { get; set; }
        public Vector pi { get; set; }
        public Sphere Sphere { get; set; }
        public Polygon Polygon { get; set; }
        public Vector VRP { get; set; }
        public Vector VPN { get; set; }
        public Vector VUP { get; set; }

        public float LightIntensity { get; set; }
        public char[,] Image { get; set; }
        public float Focal { get; set; }
        public float Xmin { get; set; }
        public float Xmax { get; set; }
        public float Ymin { get; set; }
        public float Ymax { get; set; }
        public Vector VectorP { get; set; }
        public Vector VectorL { get; set; }
        public Vector VectorO { get; set; }

        public Vector LRP { get; set; }
        public Vector LPN { get; set; }
        public Vector LUP { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public Bitmap bitmap { get; set; }
        public int Color { get; internal set; }
        public float Ip { get; internal set; }
        public Vector VectorU { get; internal set; }
        public Vector VectorN { get; internal set; }
        public Vector VectorV { get; internal set; }
        public Vector VectorO1 { get; internal set; }
        public Vector VectorP1 { get; internal set; }
        public Vector VectorP0 { get; internal set; }
        public Vector L1 { get; internal set; }
        public Vector L2 { get; internal set; }
        public Vector N { get; internal set; }
        public float A { get; internal set; }
        public float B { get; internal set; }
        public float C { get; internal set; }
        public float D { get; internal set; }
        public Vector LightVector { get; internal set; }
        public Vector VPN1 { get; internal set; }
        public Vector VUP1 { get; internal set; }
    }


    public class Sphere
    {
        public Vector Center { get; set; }
        public float Radius { get; set; }
        public float Kd { get; set; }

    }

    
    public class Polygon
    {
        public Vector V0 { get; set; }
        public Vector V1 { get; set; }
        public Vector V2 { get; set; }
        public Vector V3 { get; set; }
        public Vector Normal { get; set; }
        public float Kd { get; set; }

    }

    public class Vector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector()
        {

        }
        public Vector(float a,float b,float c)
        {
            this.X = a;
            this.Y = b;
            this.Z = c;
        }
    }

}

