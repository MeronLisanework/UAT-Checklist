namespace ECIMS.Web.Models.Entities
{
    public class CompanyStampAsset
    {
        public int AssetId { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public int UploadedById { get; set; }
        public User UploadedBy { get; set; } = null!;

        public DateTime UploadedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}