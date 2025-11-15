using Fall2025_Project3_gbward.Models;

namespace Fall2025_Project3_gbward.ViewModels
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; } = null!;
        public List<TweetSentiment> Tweets { get; set; } = new();
        public double AverageSentiment { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
    }
}
