using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Data
{
    public interface IImage
    {
        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetWithTags(string tag);
        GalleryImage GetById(int id);
        CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
        Task SetImage(string title, string tags, Uri uri);
        List<ImageTag> ParseTags(string tags);
    }
}
