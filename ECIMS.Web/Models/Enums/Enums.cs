namespace ECIMS.Web.Models.Enums
{
    public enum ProjectStatus
    {
        Pending = 0,                    
        Active = 1,                    
        AwaitingCustomerReview = 2,     
        Declined = 3,                   
        AwaitingConsultantSignature = 4,
        AwaitingPmSignature = 5,      
        Completed = 6               
    }

    public enum AttemptOverallStatus
    {
        InProgress = 1,        
        SubmittedForReview = 2,
        Declined = 3,         
        Accepted = 4          
    }

   public enum PassStatus
{
    Pending = 0,
    Pass = 1,
    Fail = 2,
    NA = 3
}

    
    public enum SignatoryRole
    {
        CustomerRepresentative = 1,
        Consultant = 2,
        ProjectManager = 3
    }

    public static class RoleNames
    {
        public const string Administrator = "Administrator";
        public const string ProjectManager = "Project Manager";
        public const string FunctionalConsultant = "Functional Consultant";
        public const string CustomerRepresentative = "Customer Representative";
    }
}