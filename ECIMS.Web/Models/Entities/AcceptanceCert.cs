namespace ECIMS.Web.Models.Entities
{
    public class AcceptanceCert
    {
        public int CertificateId { get; set; }

        public int AttemptId { get; set; }
        public ProjectUatAttempt Attempt { get; set; } = null!;

        public DateTime GeneratedDate { get; set; }

        public string PdfFilePath { get; set; } = string.Empty;
    }
}