namespace AdamMIS.Contract.Users
{
    public class UploadUserPhotoRequest
    {
        public string? UserId { get; set; }


        public IFormFile? Photo { get; set; }
    }
}
