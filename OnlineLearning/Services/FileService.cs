using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineLearning.Services
{
    public class FileService
    {
        private static string ApiKey = "AIzaSyAh05m43IMgIKFxjJnOz1qUy7ZpblQUVVo";
        private static string Bucket = "online-88d8b.appspot.com";
        private static string AuthEmail = "quyloivn@gmail.com";
        private static string AuthPassword = "Abc123";

        public async Task<string> UploadImage(IFormFile file)
        {
            return await UploadFile(file, "Images");
        }
        public async Task<string> UploadVideo(IFormFile file)
        {
            return await UploadFile(file, "Videos");
        }
        public async Task<string> UploadCourseDocument(IFormFile file)
        {
            return await UploadFile(file, "CourseDocuments");
        }
        public async Task<string> UploadLectureDocument(IFormFile file)
        {
            return await UploadFile(file, "LectureDocuments");
        }
        public async Task<string> UploadInstructorCV(IFormFile file)
        {
            return await UploadFile(file, "InstructorCVs");
        }
        public async Task<string> UploadAssignment(IFormFile file)
        {
            return await UploadFile(file, "Assignments");
        }

        public async Task<string> UploadCertificate(IFormFile file)
        {
            return await UploadFile(file, "Certificate");
        }
        public async Task<string> UploadFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty or not provided.");
            }

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var stream = file.OpenReadStream();

            try
            {
                var task = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(folder)
                    .Child(fileName)
                    .PutAsync(stream, cancellation.Token);

                string downloadUrl = await task;
                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                throw;
            }
        }

        public async Task DeleteFile(string downloadUrl)
        {
            if (string.IsNullOrEmpty(downloadUrl))
            {
                throw new Exception("Download URL is empty or not provided.");
            }

            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var uri = new Uri(downloadUrl);

                string filePath = uri.AbsolutePath.Replace("/v0/b/online-88d8b.appspot.com/o/", "");

                filePath = Uri.UnescapeDataString(filePath);

                var storage = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    });

                await storage.Child(filePath).DeleteAsync();
                Console.WriteLine($"File {filePath} đã được xóa thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xóa file thất bại. Lỗi: {ex.Message}");
                throw;
            }
        }


    }
}