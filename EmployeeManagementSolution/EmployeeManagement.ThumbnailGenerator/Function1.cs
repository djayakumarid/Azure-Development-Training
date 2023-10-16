using EmployeeManagement.SharedLibrary;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Data.SqlClient;
using System.IO;

namespace EmployeeManagement.ThumbnailGenerator
{
    public class Function1
    {
        private IConfiguration Configuration;
        public Function1(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [FunctionName("GenerateThumnail")]
        public void Run(
                    [QueueTrigger("thumbnail-queue")] BlobInformation blobInfo,
                    [Blob("empimages/{BlobName}", FileAccess.Read)] Stream image,
                    [Blob("empimages/{BlobNameWithoutExtension}_thumbnail.jpg", FileAccess.Write)] Stream imageSmall)
        {
            //Creating a Thumbnail and Saving to imageSmall (output stream)
            using (Image<Rgba32> input = Image.Load<Rgba32>(image))
            {
                input.Mutate(x => x.Resize(50, 50));
                input.Save(imageSmall, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
            }

            //Update the SQL Database
            SqlConnection conn = new SqlConnection(Configuration.GetConnectionString("EmployeeDataContext"));
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            string thumbNailUrl = $"https://jd1storageaccount.blob.core.windows.net/empimages/{blobInfo.BlobNameWithoutExtension}_thumbnail.jpg";
            cmd.CommandText = $"update employee set ThumbnailURL='{thumbNailUrl}' where Id={blobInfo.EmpId}";
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
