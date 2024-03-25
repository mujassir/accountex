using System.Data.Entity.ModelConfiguration;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Code)
                .HasMaxLength(50);

            Property(t => t.Name)
                .HasMaxLength(50);

            Property(t => t.FatherName)
                .HasMaxLength(50);

            Property(t => t.CNIC)
                .HasMaxLength(20);

            Property(t => t.ContactNumber)
                .HasMaxLength(50);

            Property(t => t.Email)
                .HasMaxLength(250);

            Property(t => t.BloodGroup)
                .HasMaxLength(50);

            Property(t => t.BankName)
                .HasMaxLength(250);

            Property(t => t.AccountNumber)
                .HasMaxLength(50);

            Property(t => t.AccountTitle)
                .HasMaxLength(250);

            Property(t => t.Address)
                .HasMaxLength(1000);

            Property(t => t.PermanentAddress)
                .HasMaxLength(1000);

            Property(t => t.Qualification)
                .HasMaxLength(250);

            Property(t => t.Designation)
                .HasMaxLength(50);

            Property(t => t.PictureUrl)
                .HasMaxLength(250);

            Property(t => t.Reference)
                .HasMaxLength(1000);

            Property(t => t.PassportNumber)
                .HasMaxLength(50);

            Property(t => t.NTN)
                .HasMaxLength(50);

            Property(t => t.DeploymentStatus)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Employees");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.FatherName).HasColumnName("FatherName");
            Property(t => t.CNIC).HasColumnName("CNIC");
            Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            Property(t => t.ContactNumber).HasColumnName("ContactNumber");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            Property(t => t.BankName).HasColumnName("BankName");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.AccountTitle).HasColumnName("AccountTitle");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.PermanentAddress).HasColumnName("PermanentAddress");
            Property(t => t.Qualification).HasColumnName("Qualification");
            Property(t => t.Designation).HasColumnName("Designation");
            Property(t => t.PictureUrl).HasColumnName("PictureUrl");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            Property(t => t.NTN).HasColumnName("NTN");
            Property(t => t.DeploymentStatus).HasColumnName("DeploymentStatus");
            Property(t => t.DOB).HasColumnName("DOB");
            Property(t => t.DOJ).HasColumnName("DOJ");
            Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
