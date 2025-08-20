using APIManyToMany.Models;
using Microsoft.EntityFrameworkCore;

namespace APIManyToMany.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options) { }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Doctor–Patient many-to-many with explicit join table
            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Patients)
                .WithMany(p => p.Doctors)
                .UsingEntity<Dictionary<string, object>>(
                    "DoctorPatients",
                    j => j.HasOne<Patient>().WithMany().HasForeignKey("PatientId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<Doctor>().WithMany().HasForeignKey("DoctorId").OnDelete(DeleteBehavior.NoAction),
                    j =>
                    {
                        j.Property<int>("Id").ValueGeneratedOnAdd();
                        j.HasKey("Id");
                        j.Property<string>("DoctorId").HasColumnType("nvarchar(450)");
                        j.Property<string>("PatientId").HasColumnType("nvarchar(450)");
                        j.ToTable("DoctorPatients");

                        // Seed data for DoctorPatients
                        j.HasData(
                            new { Id = 1, DoctorId = "DOC001", PatientId = "PAT001" },
                            new { Id = 2, DoctorId = "DOC001", PatientId = "PAT002" },
                            new { Id = 3, DoctorId = "DOC002", PatientId = "PAT003" }
                        );
                    });

            // Ensure RoleId is required in User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();

            // Hospital Seed
            modelBuilder.Entity<Hospital>().HasData(
                new Hospital { HospitalId = "HOS001", Name = "City Hospital" },
                new Hospital { HospitalId = "HOS002", Name = "Green Valley Hospital" }
            );

            // Doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { DoctorId = "DOC001", Name = "Dr. Smith", HospitalId = "HOS001", Specialization = "Cardiology" },
                new Doctor { DoctorId = "DOC002", Name = "Dr. John", HospitalId = "HOS001", Specialization = "Neurology" },
                new Doctor { DoctorId = "DOC003", Name = "Dr. Emma", HospitalId = "HOS002", Specialization = "Pediatrics" }
            );

            // Patients
            modelBuilder.Entity<Patient>().HasData(
                new Patient { PatientId = "PAT001", Name = "Alice", HospitalId = "HOS001" },
                new Patient { PatientId = "PAT002", Name = "Bob", HospitalId = "HOS001" },
                new Patient { PatientId = "PAT003", Name = "Charlie", HospitalId = "HOS002" }
            );

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = "R001", RoleName = "Admin" },
                new Role { RoleId = "R002", RoleName = "Doctor" },
                new Role { RoleId = "R003", RoleName = "Patient" }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = "U001", UserName = "SuperAdmin", Email = "admin@hospital.com", PasswordHash = "hashed123", RoleId = "R001" },
                new User { UserId = "U002", UserName = "DrJohn", Email = "doctor@hospital.com", PasswordHash = "hashed456", RoleId = "R002" },
                new User { UserId = "U003", UserName = "PatientMary", Email = "patient@hospital.com", PasswordHash = "hashed789", RoleId = "R003" }
            );
        }
    }
}