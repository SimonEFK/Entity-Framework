using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MusicHub.Data.Models
{
    public class SongConfiguration:IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Writer)
                .WithMany(x => x.Songs)
                .HasForeignKey(x => x.WriterId);

            builder.HasOne(x => x.Album)
                .WithMany(x => x.Songs)
                .HasForeignKey(x => x.AlbumId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(20);
            
            builder
                .Property(x => x.Genre)
                .IsRequired();
        }
    }
}