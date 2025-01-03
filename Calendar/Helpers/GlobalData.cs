
namespace Calendar.Helpers
{
    public class GlobalData
    {
        public string urlreplace(string? name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return name.Replace(" ", "-");
            }
            else
            {
                return "";
            }
        }
        public string DeleteImage(IWebHostEnvironment _webHostEnvironment, string folderName, string ImageName)
        {
            var ImagePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, ImageName);
            FileInfo file = new FileInfo(ImagePath);
            if (file.Exists)//check file exsit or not  
            {
                file.Delete();
                return "Failed to image";
            }
            return "Deleted";
        }
        public async Task<string> SaveImage(IFormFile ImageFile, IWebHostEnvironment _webHostEnvironment, string folderName)
        {
            string ImageName = new string(Path.GetFileNameWithoutExtension(ImageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            ImageName = ImageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(ImageFile.FileName);
            var ImagePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, ImageName);
            using (var fileStream = new FileStream(ImagePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
            return ImageName;
        }
        public async Task<string> UpdateImage(IFormFile ImageFile, IWebHostEnvironment _webHostEnvironment, string folderName, string ImageName)
        {
            var ImagePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, ImageName);
            using (var fileStream = new FileStream(ImagePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
            return ImageName;
        }
        public string GetTransactionNo()
        {
            string s = "TR" + String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
            return s;
        }
        public DateTime GetLocalTime(DateTime utctime)
        {
            var istdate = TimeZoneInfo.ConvertTimeFromUtc(utctime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            return istdate;
        }
    }
    public static class RequestStatus
    {
        public const string New = "NEW";
        public const string CollectorAssigned = "COLLECTOR ASSIGNED";
        public const string LogisiticAssigned = "PICKUP ASSIGNED";
        public const string DriverAssigned = "DRIVER ASSGINED";
        public const string Collected = "COLLECTED";
        public const string Delivered = "DELIVERED";
        public const string Recycled = "RECYCLED";
        public const string Sold = "SOLD";
    }
}
