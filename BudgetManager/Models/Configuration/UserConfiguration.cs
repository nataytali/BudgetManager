using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace BudgetManager.Models.Configuration
{
    public class UserConfiguration: IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.HasData(
                new UserModel
                {
                    Id = "e310a6cb-6677-4aa6-93c7-2763956f7a97",
                    Email = "mark@example.com",
                    FirstName = "Mark",
                    LastName = "Smith"
                },
                new UserModel
                {
                    Id = "398d10fe-4b8d-4606-8e9c-bd2c78d4e001",
                    Email = "anna@example.com",
                    FirstName = "Anna",
                    LastName = "Simmons"
                }
            ); 
        }
    }
}
