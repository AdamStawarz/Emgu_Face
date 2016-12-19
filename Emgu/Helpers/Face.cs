using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Project_FaceRecognition
{
    internal class Face
    {
        public byte[] Image { get; set; }
        public int Id { get; set; }
        public String Label { get; set; }
        public int UserId { get; set; }


    }
}
