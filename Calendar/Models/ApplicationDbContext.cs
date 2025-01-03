using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Calendar.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<CompanyManagement> CompanyManagements { get; set; }
        public DbSet<CommunicationManagement> CommunicationManagements { get; set; }
        public DbSet<CompanyCommunication> CompanyCommunications { get; set; }
        public DbSet<LoginRequest> LoginRequests { get; set; }
        public DbSet<LoginResponse> LoginResponses { get; set; }
        public DbSet<RegisterRequest> RegisterRequests { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<LoginRequest>().HasNoKey();
            modelBuilder.Entity<LoginResponse>().HasNoKey();
            modelBuilder.Entity<RegisterRequest>().HasNoKey();
        }
    }
}
