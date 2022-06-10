using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Task.Extentions
{
    public static class Extention
    {

        public static bool CheckTypeImage(this IFormFile file,string type)
        {
            if (!file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }
        public static bool CheckImageLength(this IFormFile file, int mb)
        {
            if (file.Length/1024/1024>=mb)
            {
                return true;
            }
            return false;
        }
        public static async Task<string> SaveFile(this IFormFile file,string imgPath)
        {
            string imgUrl = Guid.NewGuid().ToString() + file.FileName;
            string path = Path.Combine(imgPath, imgUrl);
            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
               await file.CopyToAsync(fs);
            }
            return imgUrl;
        }
        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
