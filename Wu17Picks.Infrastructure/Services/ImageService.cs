﻿using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu17Picks.Data;
using Wu17Picks.Data.Models;
using Wu17Picks.Data.Entities;
using Wu17Picks.Infrastructure.Interfaces;
using Wu17Picks.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace Wu17Picks.Services
{
    public class ImageService : IImage
    {
        private readonly AppSettingsHelper _appSettings;
        private readonly ApplicationDbContext _ctx;
        public ImageService(ApplicationDbContext ctx, IOptions<AppSettingsHelper> settings)
        {
            _ctx = ctx;
            _appSettings = settings.Value;
        }

        public IEnumerable<Category> Categories => _ctx.Categories;
        public IEnumerable<GalleryImage> GalleryImages => _ctx.GalleryImages.Include(c => c.Category);
        public IEnumerable<GalleryImage> GetAll()
        {
            return _ctx.GalleryImages
                .Include(img => img.Tags);
        }

        public GalleryImage GetById(int id)
        {
            return GetAll().Where(img => img.Id == id).First();
        }

        public IEnumerable<GalleryImage> GetWithTags(string tag)
        {
            return GetAll()
                .Where(img => img.Tags
                .Any(t => t.Description == tag));
        }

        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
        public async Task SetImage(string title, string tags, int categoryid, Uri uri)
        {
            var image = new GalleryImage
            {
                Title = title,
                Tags = ParseTags(tags),
                Url = uri.AbsoluteUri,
                Created = DateTime.Now,
                CategoryId = categoryid
            };

            _ctx.Add(image);
            await _ctx.SaveChangesAsync();
        }

        public List<ImageTag> ParseTags(string tags)
        {
           return tags.Split(",").Select(tag=> new ImageTag {
                Description = tag
            }).ToList();

        }
    }
}
