using ECIMS.Web.Models.Enums;

namespace ECIMS.Web.Models.Entities
{
    public class DigitalSignature
    {
        public int SignatureId { get; set; }

        public int AttemptId { get; set; }
        public ProjectUatAttempt Attempt { get; set; } = null!;

        public SignatoryRole SignatoryRole { get; set; }

        public int SignedById { get; set; }
        public User SignedBy { get; set; } = null!;

        public string OriginalSignatureBlob { get; set; } = string.Empty;

        public DateTime DateStamped { get; set; }
    }
}