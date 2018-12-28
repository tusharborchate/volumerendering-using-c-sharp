using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment3
{
    public class RayTracing
    {
        VolumeRenderingLogic volumeRenderingLogic;

        public  RayTracing()
        {
          }
        static void Main(string[] args)
        {
           
           Application.Run(new Form2());
//Console.ReadLine();




        }


        public string Start(int choice, float elevation, float rotation, float motion, float zoomin, float zoomout)
        {
            volumeRenderingLogic = new VolumeRenderingLogic(rotation,elevation);
            return volumeRenderingLogic.VolumeRendering(choice, elevation, rotation, motion, zoomin, zoomout);

        }
        public string GetImagePath(int choice, float elevation, float rotation, float motion, float zoomin, float zoomout)
        {


            return volumeRenderingLogic.VolumeRendering(choice, elevation,rotation, motion, zoomin, zoomout);


        }


    }
}
