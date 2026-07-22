using ECIMS.Web.Models.Entities;
using ECIMS.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECIMS.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { RoleName = RoleNames.Administrator },
                    new Role { RoleName = RoleNames.ProjectManager },
                    new Role { RoleName = RoleNames.FunctionalConsultant },
                    new Role { RoleName = RoleNames.CustomerRepresentative }
                );
                await db.SaveChangesAsync();
            }

            if (!db.Users.Any())
            {
                var hasher = new PasswordHasher<User>();
                var adminRole = db.Roles.First(r => r.RoleName == RoleNames.Administrator);
                var pmRole = db.Roles.First(r => r.RoleName == RoleNames.ProjectManager);
                var consultantRole = db.Roles.First(r => r.RoleName == RoleNames.FunctionalConsultant);
                var customerRole = db.Roles.First(r => r.RoleName == RoleNames.CustomerRepresentative);

                var users = new List<User>
                {
                    new User { Username = "admin", FullName = "System Administrator", RoleId = adminRole.RoleId, IsActive = true },
                    new User { Username = "pm1", FullName = "Selam Tesfaye", RoleId = pmRole.RoleId, IsActive = true },
                    new User { Username = "consultant1", FullName = "Abebe Bekele", RoleId = consultantRole.RoleId, IsActive = true },
                    new User { Username = "customer1", FullName = "Hana Girma", RoleId = customerRole.RoleId, IsActive = true }
                };

                foreach (var user in users)
                {
                    user.PasswordHash = hasher.HashPassword(user, "Password123!");
                }

                db.Users.AddRange(users);
                await db.SaveChangesAsync();
            }

            if (!db.UatSections.Any())
            {
                var sectionNames = new[]
                {
                    "Login and Preparation",
                    "POS Home Screen",
                    "Main Navigator",
                    "Maintenance",
                    "Invoice Validation",
                    "POS Operations",
                    "Closing",
                    "Reports"
                };

                foreach (var name in sectionNames)
                {
                    db.UatSections.Add(new UatSection
                    {
                        SectionName = name,
                        MasterItems = new List<UatMasterItem>
                        {
                            new UatMasterItem { TestDescription = $"{name} - Sample Test Item 1" },
                            new UatMasterItem { TestDescription = $"{name} - Sample Test Item 2" }
                        }
                    });
                }

                await db.SaveChangesAsync();
            }

            if (!db.Customers.Any())
            {
                var adminUser = await db.Users.FirstAsync(u => u.Username == "admin");
                var pmUser = await db.Users.FirstAsync(u => u.Username == "pm1");
                var consultantUser = await db.Users.FirstAsync(u => u.Username == "consultant1");

                // 12 customers, one branch each, one project each (1 customer = 1 project rule)
                var customerData = new[]
                {
                    new { Name = "REAZ Engineering and Con PLC", Contact = "Yonas Alemu", Phone = "+251911000001", Email = "yonas@reaz.com", Branch = "Head Office", Address = "Bole Road, Addis Ababa" },
                    new { Name = "Dashen Bank S.C.", Contact = "Meron Fikru", Phone = "+251911000002", Email = "meron@dashenbank.com", Branch = "Bole Branch", Address = "Bole, Addis Ababa" },
                    new { Name = "Nyala Insurance S.C.", Contact = "Dawit Molla", Phone = "+251911000003", Email = "dawit@nyala.com", Branch = "Main Office", Address = "Piassa, Addis Ababa" },
                    new { Name = "Awash Bank S.C.", Contact = "Sara Kebede", Phone = "+251911000004", Email = "sara@awashbank.com", Branch = "Piassa Branch", Address = "Piassa, Addis Ababa" },
                    new { Name = "Ethio Telecom", Contact = "Biniam Tesfaye", Phone = "+251911000005", Email = "biniam@ethiotelecom.et", Branch = "Headquarters", Address = "Churchill Ave, Addis Ababa" },
                    new { Name = "Commercial Bank of Ethiopia", Contact = "Tigist Worku", Phone = "+251911000006", Email = "tigist@cbe.com.et", Branch = "Merkato Branch", Address = "Merkato, Addis Ababa" },
                    new { Name = "Zemen Bank S.C.", Contact = "Fitsum Alene", Phone = "+251911000007", Email = "fitsum@zemenbank.com", Branch = "Kazanchis Branch", Address = "Kazanchis, Addis Ababa" },
                    new { Name = "Awash Insurance S.C.", Contact = "Ruth Assefa", Phone = "+251911000008", Email = "ruth@awashinsurance.com", Branch = "Head Office", Address = "Mexico Square, Addis Ababa" },
                    new { Name = "Safaricom Ethiopia", Contact = "Kaleab Girma", Phone = "+251911000009", Email = "kaleab@safaricom.et", Branch = "Regional Office", Address = "CMC, Addis Ababa" },
                    new { Name = "Wegagen Bank S.C.", Contact = "Liya Solomon", Phone = "+251911000010", Email = "liya@wegagenbank.com", Branch = "Head Office", Address = "Ras Abebe Aregay St, Addis Ababa" },
                    new { Name = "Nib International Bank", Contact = "Elias Mulu", Phone = "+251911000011", Email = "elias@nibbank.com", Branch = "Bole Branch", Address = "Bole, Addis Ababa" },
                    new { Name = "United Bank S.C.", Contact = "Bethel Yosef", Phone = "+251911000012", Email = "bethel@unitedbank.com", Branch = "Gerji Branch", Address = "Gerji, Addis Ababa" }
                };

                var createdAt = DateTime.UtcNow.AddMonths(-3);
                var customers = new List<Customer>();
                var branches = new List<CustomerBranch>();

                foreach (var c in customerData)
                {
                    var customer = new Customer
                    {
                        CustomerName = c.Name,
                        ContactPerson = c.Contact,
                        ContactPhone = c.Phone,
                        ContactEmail = c.Email,
                        CreatedById = adminUser.UserId,
                        CreatedAt = createdAt
                    };
                    customers.Add(customer);
                }

                db.Customers.AddRange(customers);
                await db.SaveChangesAsync();

                for (int i = 0; i < customerData.Length; i++)
                {
                    branches.Add(new CustomerBranch
                    {
                        CustomerId = customers[i].CustomerId,
                        BranchName = customerData[i].Branch,
                        Address = customerData[i].Address,
                        SiteContactName = customerData[i].Contact,
                        SiteContactPhone = customerData[i].Phone,
                        CreatedById = adminUser.UserId,
                        CreatedAt = createdAt
                    });
                }

                db.CustomerBranches.AddRange(branches);
                await db.SaveChangesAsync();

                // One project per customer/branch, spread across every status bucket
                var projectData = new[]
                {
                    new { Name = "POS Rollout - REAZ Engineering", Status = ProjectStatus.AwaitingCustomerReview, Start = -40, End = 20 },
                    new { Name = "POS Rollout - Dashen Bank", Status = ProjectStatus.Declined, Start = -35, End = 10 },
                    new { Name = "POS Rollout - Nyala Insurance", Status = ProjectStatus.Active, Start = -5, End = 40 },
                    new { Name = "POS Rollout - Awash Bank", Status = ProjectStatus.Completed, Start = -60, End = -10 },
                    new { Name = "POS Rollout - Ethio Telecom", Status = ProjectStatus.AwaitingPmSignature, Start = -70, End = -15 },
                    new { Name = "POS Rollout - CBE", Status = ProjectStatus.AwaitingConsultantSignature, Start = -25, End = 15 },
                    new { Name = "POS Rollout - Zemen Bank", Status = ProjectStatus.Pending, Start = -3, End = 50 },
                    new { Name = "POS Rollout - Awash Insurance", Status = ProjectStatus.Completed, Start = -90, End = -30 },
                    new { Name = "POS Rollout - Safaricom Ethiopia", Status = ProjectStatus.AwaitingCustomerReview, Start = -15, End = 25 },
                    new { Name = "POS Rollout - Wegagen Bank", Status = ProjectStatus.Declined, Start = -18, End = 12 },
                    new { Name = "POS Rollout - Nib International Bank", Status = ProjectStatus.Completed, Start = -100, End = -45 },
                    new { Name = "POS Rollout - United Bank", Status = ProjectStatus.AwaitingPmSignature, Start = -80, End = -20 }
                };

                var projects = new List<Project>();
                for (int i = 0; i < projectData.Length; i++)
                {
                    projects.Add(new Project
                    {
                        ProjectName = projectData[i].Name,
                        BranchId = branches[i].BranchId,
                        ProjectManagerId = pmUser.UserId,
                        ConsultantId = consultantUser.UserId,
                        StartDate = DateTime.UtcNow.AddDays(projectData[i].Start),
                        EndDate = DateTime.UtcNow.AddDays(projectData[i].End),
                        Status = projectData[i].Status,
                        CreatedById = adminUser.UserId,
                        CreatedAt = DateTime.UtcNow.AddDays(projectData[i].Start)
                    });
                }

                db.Projects.AddRange(projects);
                await db.SaveChangesAsync();

                
            }

            
        }

public static async Task SeedDemoProjectsAsync(ApplicationDbContext db)
{
 if (await db.Projects.AnyAsync(p => p.ProjectName == "NVS POS Rollout - Not Started")) return;

    var consultant = await db.Users.FirstAsync(u => u.Username == "consultant1");
    var pm = await db.Users.FirstAsync(u => u.Username == "pm1");
    var customerRep = await db.Users.FirstAsync(u => u.Username == "customer1");
    var admin = await db.Users.FirstAsync(u => u.Username == "admin");

var customer = await db.Customers.FirstOrDefaultAsync(c => c.CustomerName == "REAZ Engineering and Con PLC");
    if (customer is null)
    {
        customer = new Customer
        {
            CustomerName = "REAZ Engineering and Con PLC",
            ContactPerson = "Selam Girma",
            ContactPhone = "0911000000",
            ContactEmail = "selam@reaz.com",
            CreatedById = admin.UserId,
            CreatedAt = DateTime.UtcNow
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
    }
    if (customerRep.CustomerId != customer.CustomerId)
    {
        customerRep.CustomerId = customer.CustomerId;
        await db.SaveChangesAsync();
    }

    var branch = await db.CustomerBranches.FirstOrDefaultAsync(b => b.CustomerId == customer.CustomerId && b.BranchName == "Head Office");
    if (branch is null)
    {
        branch = new CustomerBranch
        {
            CustomerId = customer.CustomerId,
            BranchName = "Head Office",
            Address = "Addis Ababa",
            SiteContactName = "Selam Girma",
            SiteContactPhone = "0911000000",
            CreatedById = admin.UserId,
            CreatedAt = DateTime.UtcNow
        };
        db.CustomerBranches.Add(branch);
        await db.SaveChangesAsync();
    }

    var masterItems = await db.UatMasterItems.ToListAsync();

    async Task<Project> CreateProject(string name, ProjectStatus status)
    {
        var project = new Project
        {
            ProjectName = name,
            BranchId = branch.BranchId,
            ProjectManagerId = pm.UserId,
            ConsultantId = consultant.UserId,
            StartDate = DateTime.UtcNow.AddDays(-14),
            EndDate = DateTime.UtcNow.AddDays(14),
            Status = status,
            CreatedById = pm.UserId,
            CreatedAt = DateTime.UtcNow.AddDays(-14)
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return project;
    }

    async Task<ProjectUatAttempt> CreateAttempt(int projectId, int number, AttemptOverallStatus status, int passCount, int pendingCount, DateTime started, DateTime? submitted = null, DateTime? decided = null, int? decidedById = null)
    {
        var attempt = new ProjectUatAttempt
        {
            ProjectId = projectId,
            AttemptNumber = number,
            StartedDate = started,
            SubmittedDate = submitted,
            DecidedDate = decided,
            DecidedById = decidedById,
            OverallStatus = status,
            InitiatedById = consultant.UserId
        };
        db.ProjectUatAttempts.Add(attempt);
        await db.SaveChangesAsync();

        var items = masterItems.Take(passCount + pendingCount).ToList();
        for (int i = 0; i < items.Count; i++)
        {
            db.ProjectUatResults.Add(new ProjectUatResult
            {
                AttemptId = attempt.AttemptId,
                MasterItemId = items[i].MasterItemId,
                PassStatus = i < passCount ? PassStatus.Pass : PassStatus.Pending,
                ExecutedById = consultant.UserId,
                ExecutedDate = started
            });
        }
        await db.SaveChangesAsync();
        return attempt;
    }

    await CreateProject("NVS POS Rollout - Not Started", ProjectStatus.Pending);

    var activeProject = await CreateProject("NVS POS Rollout - In Progress", ProjectStatus.Active);
    await CreateAttempt(activeProject.ProjectId, 1, AttemptOverallStatus.InProgress, passCount: 6, pendingCount: masterItems.Count - 6, started: DateTime.UtcNow.AddDays(-2));

    var reviewProject = await CreateProject("NVS POS Rollout - In Review", ProjectStatus.AwaitingCustomerReview);
    await CreateAttempt(reviewProject.ProjectId, 1, AttemptOverallStatus.SubmittedForReview, passCount: masterItems.Count, pendingCount: 0, started: DateTime.UtcNow.AddDays(-3), submitted: DateTime.UtcNow.AddDays(-1));

    var declinedProject = await CreateProject("NVS POS Rollout - Declined", ProjectStatus.Declined);
    var declinedAttempt1 = await CreateAttempt(declinedProject.ProjectId, 1, AttemptOverallStatus.Declined, passCount: masterItems.Count, pendingCount: 0, started: DateTime.UtcNow.AddDays(-6), submitted: DateTime.UtcNow.AddDays(-4), decided: DateTime.UtcNow.AddDays(-3), decidedById: customerRep.UserId);

    var firstResult = await db.ProjectUatResults.FirstAsync(r => r.AttemptId == declinedAttempt1.AttemptId);
    db.ProjectUatResultHistories.Add(new ProjectUatResultHistory
    {
        ResultId = firstResult.ResultId,
        PreEditPassStatus = PassStatus.Pass,
        PostEditPassStatus = PassStatus.Fail,
        EditComment = "Register did not print receipt correctly",
        EditedById = customerRep.UserId,
        EditedAt = DateTime.UtcNow.AddDays(-3)
    });
    await db.SaveChangesAsync();

    await CreateAttempt(declinedProject.ProjectId, 2, AttemptOverallStatus.InProgress, passCount: masterItems.Count - 1, pendingCount: 1, started: DateTime.UtcNow.AddDays(-2));

    var signatureProject = await CreateProject("NVS POS Rollout - Signature Required", ProjectStatus.AwaitingConsultantSignature);
    var sigAttempt = await CreateAttempt(signatureProject.ProjectId, 1, AttemptOverallStatus.Accepted, passCount: masterItems.Count, pendingCount: 0, started: DateTime.UtcNow.AddDays(-5), submitted: DateTime.UtcNow.AddDays(-3), decided: DateTime.UtcNow.AddDays(-2), decidedById: customerRep.UserId);
    db.DigitalSignatures.Add(new DigitalSignature
    {
        AttemptId = sigAttempt.AttemptId,
        SignatoryRole = SignatoryRole.CustomerRepresentative,
        SignedById = customerRep.UserId,
        OriginalSignatureBlob = "demo",
        DateStamped = DateTime.UtcNow.AddDays(-2)
    });
    await db.SaveChangesAsync();

    var pmProject = await CreateProject("NVS POS Rollout - Awaiting PM Approval", ProjectStatus.AwaitingPmSignature);
    var pmAttempt = await CreateAttempt(pmProject.ProjectId, 1, AttemptOverallStatus.Accepted, passCount: masterItems.Count, pendingCount: 0, started: DateTime.UtcNow.AddDays(-6), submitted: DateTime.UtcNow.AddDays(-4), decided: DateTime.UtcNow.AddDays(-3), decidedById: customerRep.UserId);
    db.DigitalSignatures.AddRange(
        new DigitalSignature { AttemptId = pmAttempt.AttemptId, SignatoryRole = SignatoryRole.CustomerRepresentative, SignedById = customerRep.UserId, OriginalSignatureBlob = "demo", DateStamped = DateTime.UtcNow.AddDays(-3) },
        new DigitalSignature { AttemptId = pmAttempt.AttemptId, SignatoryRole = SignatoryRole.Consultant, SignedById = consultant.UserId, OriginalSignatureBlob = "demo", DateStamped = DateTime.UtcNow.AddDays(-2) }
    );
    await db.SaveChangesAsync();

    var completedProject = await CreateProject("NVS POS Rollout - Completed", ProjectStatus.Completed);
    var completedAttempt = await CreateAttempt(completedProject.ProjectId, 1, AttemptOverallStatus.Accepted, passCount: masterItems.Count, pendingCount: 0, started: DateTime.UtcNow.AddDays(-10), submitted: DateTime.UtcNow.AddDays(-8), decided: DateTime.UtcNow.AddDays(-7), decidedById: customerRep.UserId);
    db.DigitalSignatures.AddRange(
        new DigitalSignature { AttemptId = completedAttempt.AttemptId, SignatoryRole = SignatoryRole.CustomerRepresentative, SignedById = customerRep.UserId, OriginalSignatureBlob = "demo", DateStamped = DateTime.UtcNow.AddDays(-7) },
        new DigitalSignature { AttemptId = completedAttempt.AttemptId, SignatoryRole = SignatoryRole.Consultant, SignedById = consultant.UserId, OriginalSignatureBlob = "demo", DateStamped = DateTime.UtcNow.AddDays(-6) },
        new DigitalSignature { AttemptId = completedAttempt.AttemptId, SignatoryRole = SignatoryRole.ProjectManager, SignedById = pm.UserId, OriginalSignatureBlob = "demo", DateStamped = DateTime.UtcNow.AddDays(-5) }
    );
    db.AcceptanceCerts.Add(new AcceptanceCert
    {
        AttemptId = completedAttempt.AttemptId,
        GeneratedDate = DateTime.UtcNow.AddDays(-5),
        PdfFilePath = "/certs/demo.pdf"
    });
    await db.SaveChangesAsync();
}
    }
}