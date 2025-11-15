namespace Fall2025_Project3_gbward.ViewModels
{
    public class TweetSentiment
    {
        public string Tweet { get; set; } = string.Empty;
        public double CompoundScore { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
    }
}
