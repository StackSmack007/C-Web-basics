﻿// <auto-generated />
using IRunes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IRunes.Infrastructure.Data.Migrations
{
    [DbContext(typeof(IRunesContext))]
    partial class IRunesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.Album", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CoverImgUrl");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("UserCreatorID");

                    b.HasKey("Id");

                    b.HasIndex("UserCreatorID");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.AlbumTrack", b =>
                {
                    b.Property<string>("AlbumId");

                    b.Property<string>("TrackId");

                    b.HasKey("AlbumId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("AlbumsTracks");
                });

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.Track", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LinkURL");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<decimal>("Price");

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.Album", b =>
                {
                    b.HasOne("IRunes.Infrastructure.Models.Models.User", "UserCreator")
                        .WithMany("AlbumsCreated")
                        .HasForeignKey("UserCreatorID");
                });

            modelBuilder.Entity("IRunes.Infrastructure.Models.Models.AlbumTrack", b =>
                {
                    b.HasOne("IRunes.Infrastructure.Models.Models.Album", "Album")
                        .WithMany("AlbumTracks")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IRunes.Infrastructure.Models.Models.Track", "Track")
                        .WithMany("TrackAlbums")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
