using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application
{
    public class UploadFile
    {
        public string Up(HttpPostedFileBase file)
        {
            const string imageExtensions = "jpg,jpeg,png,gif";
            var fileName = Path.GetFileName(file.FileName);
            if (fileName != null)
            {
                // Lower case
                fileName = fileName.ToLower();

                // Get the file extension
                var fileExtension = Path.GetExtension(fileName);

            }

                return "";
        }


    }
}