using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace DishAndMovie.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string subFolder = "movies")
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Ensure the uploads directory exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique filename
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return the relative path to store in the database
            return $"/uploads/{subFolder}/{uniqueFileName}";
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"Deleted image: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex}");
            }
        }
    }
}