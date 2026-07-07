using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.Store.EntityFrameworkCore;

public class KeyMaterialMapping : IEntityTypeConfiguration<KeyMaterial>
{
    public void Configure(EntityTypeBuilder<KeyMaterial> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Parameters)
            .HasMaxLength(8000)
            .IsRequired();
    }
}