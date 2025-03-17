//using Nawar.API.Core.Interfaces.Application.UtilityServices;

//namespace Nawar.API.Application.UtilityServices
//{
//    public class FileService : IFileService
//    {
//        private async Task<string> UploadFile(IFormFile file, string folderPath, string fileName)
//        {
//            if (file == null || file.Length == 0)
//                throw new Exception("No file uploaded or file is empty.");

//            Directory.CreateDirectory(folderPath); // Ensure folder exists
//                                                   // Get the original file extension
//            var fileExtension = Path.GetExtension(file.FileName);

//            // If the file name provided doesn't already have an extension, append the original file's extension
//            if (!fileName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
//            {
//                fileName += fileExtension;
//            }
//            var filePath = Path.Combine(folderPath, fileName);
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            return fileName;
//        }

//        public async Task<string> UploadFileAsync(IFormFile file, FilePathType pathType, string fileName)
//        {
//            var folderPath = FilePathConstants.PathMappings[pathType];
//            return await UploadFile(file, folderPath, fileName);
//        }

//        public async Task<bool> DeleteFileAsync(FilePathType pathType, string fileName)
//        {
//            var filePath = Path.Combine(FilePathConstants.PathMappings[pathType], fileName);
//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("File not found.");

//            File.Delete(filePath);
//            return await Task.FromResult(true);
//        }

//        public async Task<string> UpdateFileAsync(IFormFile file, FilePathType pathType, string fileName)
//        {
//            await DeleteFileAsync(pathType, fileName);
//            return await UploadFileAsync(file, pathType, fileName);
//        }

//        public async Task<string> GetFileLinkAsync(FilePathType pathType, string fileName)
//        {
//            var filePath = Path.Combine(FilePathConstants.PathMappings[pathType], fileName);

//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("File not found.");

//            string contentType = "application/octet-stream"; // Default to a generic type
//            switch (Path.GetExtension(filePath).ToLower())
//            {
//                case ".jpg":
//                case ".jpeg":
//                    contentType = "image/jpeg";
//                    break;
//                case ".png":
//                    contentType = "image/png";
//                    break;
//                case ".gif":
//                    contentType = "image/gif";
//                    break;
//                case ".pdf":
//                    contentType = "application/pdf";
//                    break;
//            }
//            var relativePath = filePath.Replace("/", "%5C").Replace("\\", "%5C%5C"); // Adjust for URL format
//            var baseUrl = "https://localhost:7198/api/company/view/";
//            filePath = $"{baseUrl}{relativePath}";
//            return filePath; // Adjust this to return a URL or path based on your app's structure
//        }
//    }

//}
