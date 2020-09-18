using CoreTweet;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitterStampMediaUploader
{
    public class Uploader
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public Uploader(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task Run()
        {
            var tokens = Tokens.Create(
                _appSettings.TwitterApi.ConsumerApiKey,
                _appSettings.TwitterApi.ConsumerApiSecretKey,
                _appSettings.TwitterApi.AccessToken,
                _appSettings.TwitterApi.AccessTokenSecret);

            var stampList = await GetStampListAsync(_appSettings.File.StampListFile);

            var imageFiles = GetImageFiles(_appSettings.File.ImageFileDirectory);

            foreach (var imageFile in imageFiles)
            {
                _logger.Info(imageFile.Name);

                var tweetId = await TweetImageAsync(tokens, imageFile);

                _logger.Info($"{imageFile.Name} {tweetId}");

                AppendNewStampSrc(tweetId, imageFile, stampList);
            }

            await WriteNewStampList(_appSettings.File.StampListOutFile, stampList);
        }

        private static async Task<IList<StampListItem>> GetStampListAsync(string stampListFile)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), stampListFile);

            var jsonString = await File.ReadAllTextAsync(path);

            return JsonSerializer.Deserialize<IList<StampListItem>>(jsonString);
        }

        private static IEnumerable<FileInfo> GetImageFiles(string imageFileDirectory)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), imageFileDirectory);

            return Directory.GetFiles(path)
                            .Where(x => Regex.IsMatch(x.ToLower(), @"stamp\d+\.png"))
                            .Select(x => new FileInfo(x));
        }

        private static async Task<long> TweetImageAsync(Tokens tokens, FileInfo imageFile)
        {
            var mediaUploadResult = await tokens.Media.UploadAsync(imageFile);

            var mediaIds = new long[] { mediaUploadResult.MediaId };

            var statusUpdateResult = await tokens.Statuses.UpdateAsync(string.Empty, media_ids: mediaIds);

            var tweetId = statusUpdateResult.Id;

            return tweetId;
        }

        private static void AppendNewStampSrc(long tweetId, FileInfo imageFile, IList<StampListItem> stampList)
        {
            var src = $"https://twitter.com/moune_you/status/{tweetId}/photo/1";

            var stampId = Path.GetFileNameWithoutExtension(imageFile.Name);

            var stamp = stampList.SingleOrDefault(s => s.Id == stampId);

            if (stamp == null)
            {
                var newStamp = new StampListItem
                {
                    Id = stampId,
                    Image = $"/images/stamp/{imageFile.Name}",
                    Src = new List<string> { src },
                };
                stampList.Add(newStamp);
            }
            else
            {
                stamp.Src.Insert(0, src);
            }
        }

        private static async Task WriteNewStampList(string stampListOutFile, IList<StampListItem> stampList)
        {
            var jsonString = JsonSerializer.Serialize(stampList, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = true,
            });

            var path = Path.Combine(Directory.GetCurrentDirectory(), stampListOutFile);

            await File.WriteAllTextAsync(path, jsonString, new UTF8Encoding(false));
        }
    }
}
