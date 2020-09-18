namespace TwitterStampMediaUploader
{
    public class AppSettings
    {
        public FileSettings File { get; set; }
        public TwitterApiSettings TwitterApi { get; set; }
    }

    public class FileSettings
    {
        public string ImageFileDirectory { get; set; }
        public string StampListFile { get; set; }
        public string StampListOutFile { get; set; }
    }

    public class TwitterApiSettings
    {
        public string ConsumerApiKey { get; set; }
        public string ConsumerApiSecretKey { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
