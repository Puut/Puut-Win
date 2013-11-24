using System;
using System.Drawing;
using System.IO;

namespace Puut.Capture
{
    public class Upload
    {
        private Image image = null;

        public Upload(Image image)
        {
            this.image = image;
        }

        public void DoUpload()
        {
            String tempFilename = Path.GetTempFileName();
            Console.WriteLine("Upload temp filename: " + tempFilename);

            this.image.Save(tempFilename);
        }
    }
}
