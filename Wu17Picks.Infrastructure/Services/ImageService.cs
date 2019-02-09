using Microsoft.EntityFrameworkCore;
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
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace Wu17Picks.Services
{
    public class ImageService : IImage
    {
        private const string _key = "images";
        private readonly AppConfigHelper _appConfig;
        private readonly ApplicationDbContext _ctx;
        private IDistributedCache _cache;
        public ImageService(ApplicationDbContext ctx, IOptions<AppConfigHelper> appConfig, IDistributedCache cache)
        {
            _ctx = ctx;
            _appConfig = appConfig.Value;
            _cache = cache;
        }

        private List<GalleryImage> GetCache()
            => _cache.GetValue<List<GalleryImage>>(_key);
        private void SetCache(List<GalleryImage> toCache)
            => _cache.SetValue(_key, toCache);

        private void SetCategoryCache(int categoryid, List<GalleryImage> toCache)
           => _cache.SetValue($"{_key}:category:{categoryid}", toCache);
        private List<GalleryImage> GetCategoryCache(int categoryid)
            => _cache.GetValue<List<GalleryImage>>($"{_key}:category:{categoryid}");

        public IEnumerable<Category> Categories => _ctx.Categories;
        public IEnumerable<GalleryImage> GalleryImages => _ctx.GalleryImages.Include(c => c.Category);

        public IEnumerable<GalleryImage> GetAll()
        {
            var result = GetCache();

            if (result != null)
                return result;

            result = _ctx.GalleryImages
                .OrderBy(x => x.Created)
                .Include(img => img.Tags)
                .Select(x => new GalleryImage()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Created = x.Created,
                    CategoryId = x.CategoryId,
                    FileName = x.FileName

                })
                .ToList();

            SetCache(result);
            return result;
        }

        public GalleryImage GetById(int id)
        {
            return _ctx.GalleryImages.Find(id);
        }

        public IEnumerable<GalleryImage> GetWithTags(string tag)
        {
            return GetAll().Where(img => img.Tags
                .Any(t => t.Description == tag));
        }

        public CloudBlobContainer GetBlobContainer(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_appConfig.AzureStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
        public async Task SetImage(string title, string tags, int categoryid, Uri uri)
        {
            var imagesCached = GetCache();
            var categoryimagesCached = GetCategoryCache(categoryid);


            var image = new GalleryImage
            {
                Title = title,
                Tags = ParseTags(tags),
                FileName = uri.AbsolutePath,
                Created = DateTime.Now,
                CategoryId = categoryid
            };

            if (imagesCached != null)
            {
                imagesCached.Add(image);

                SetCache(imagesCached);
            }

            if (categoryimagesCached != null)
            {
                categoryimagesCached.Add(image);

                SetCategoryCache(categoryid, categoryimagesCached);
            }

             _ctx.Add(image);
            await _ctx.SaveChangesAsync();
        }

        public List<ImageTag> ParseTags(string tags)
        {
           return tags.Split(",").Select(tag=> new ImageTag {
                Description = tag
            }).ToList();

        }

        public bool URLExists(string url)
        {
            // TODO GetResponse fails and return false (Aux adress)
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }
        }
    }
}
