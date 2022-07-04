using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MusicHub.Data.Models;

namespace MusicHub
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);


            var result = ExportSongsAboveDuration(context, 260);
            Console.WriteLine(result);

            //Test your solutions here
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(x => x.ProducerId == producerId)
                .Select(x => new
                {
                    x.Name,
                    x.ReleaseDate,
                    ProducerName = x.Producer.Name,
                    AllSongs = x.Songs
                        .Select(y => new
                        {
                            SongName = y.Name,
                            SongPrice = y.Price,
                            SongWriter = y.Writer.Name,

                        })
                        .OrderByDescending(y => y.SongName)
                        .ThenBy(y => y.SongWriter).ToList()


                })

                .ToList();

            ;

            var sb = new StringBuilder();

            foreach (var album in albums.OrderByDescending(x => x.AllSongs.Sum(x => x.SongPrice)))
            {
                var date = album.ReleaseDate.ToString("MM/dd/yyyy");
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {date}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");
                int count = 1;
                foreach (var song in album.AllSongs)
                {
                    sb.AppendLine($"---#{count++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:F2}");
                    sb.AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AllSongs.Sum(x => x.SongPrice):f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            var songs = context.Songs                
                .Select(x => new
                {
                    SongName = x.Name,
                    Writer = x.Writer.Name,
                    AlbumProducer = x.Album.Producer.Name,
                    PerformerFullName = x.SongPerformers.Select(y => y.Performer.FirstName + " " + y.Performer.LastName).FirstOrDefault(),
                    Duration = x.Duration

                })
                .OrderBy(song=>song.SongName)
                .ThenBy(song=>song.Writer)
                .ThenBy(song=>song.PerformerFullName)
                .ToList()
                .Where(x=>x.Duration.TotalSeconds>duration);

            var sb = new StringBuilder();
            int counter = 0;
            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{++counter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerformerFullName}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}
