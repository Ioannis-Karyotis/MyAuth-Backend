using Microsoft.EntityFrameworkCore;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Data
{
    public static class ModelBuilderCreator
    {
        public static ModelBuilder BuildModels(this ModelBuilder builder, string schema)
        {
            builder.HasDefaultSchema(schema);

            builder.Entity<MyAuthUser>().HasKey(s => s.Id);
            builder.Entity<MyAuthUser>().Property(p => p.FaceDescriptor).HasColumnName("FaceDescriptor")
                    .HasColumnType("jsonb")
                    .HasConversion(
                        c => JsonConvert.SerializeObject(c, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }),
                        c => JsonConvert.DeserializeObject<TxtFile>(c));

            builder.Entity<ExternalApp>().HasKey(s => s.Id);
            builder.Entity<ExternalApp>().HasIndex(s => s.ClientId).IsUnique();
            builder.Entity<ExternalApp>().HasIndex(s => s.ClientSecret).IsUnique();
            builder.Entity<ExternalApp>().HasIndex(s => s.CallbackUrl).IsUnique();

            builder.Entity<ExternalAppAuthUser>().HasKey(s => s.Id);


            return builder;
        }
    }
}
