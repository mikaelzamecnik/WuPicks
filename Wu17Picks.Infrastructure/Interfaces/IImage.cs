﻿using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wu17Picks.Data.Entities;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Infrastructure.Interfaces
{
    public interface IImage
    {
        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetWithTags(string tag);
        IEnumerable<Category> Categories { get; }
        IEnumerable<GalleryImage> GalleryImages { get; }
        GalleryImage GetById(int id);
        CloudBlobContainer GetBlobContainer(string containerName);
        Task SetImage(string title, string tags, int categoryid, Uri uri);
        List<ImageTag> ParseTags(string tags);
        bool URLExists(string url);
    }
}
