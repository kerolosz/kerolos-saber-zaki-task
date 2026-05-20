using Products.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Repositories
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        // webRootPath = wwwroot folder path injected from API layer
        public FileStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
        {
            // Sanitize filename and make unique
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(_webRootPath, folder);
            Directory.CreateDirectory(folderPath);  // Ensure folder exists

            var fullPath = Path.Combine(folderPath, uniqueFileName);

            using var output = File.Create(fullPath);
            await fileStream.CopyToAsync(output);

            // Return relative path to store in DB
            //return Path.Combine(folder, uniqueFileName);
            return uniqueFileName;
        }

        public void DeleteFile(string relativePath)
        {
            var fullPath = Path.Combine(_webRootPath, relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
