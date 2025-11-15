
using Fall2025_Project3_gbward.Models;

namespace Fall2025_Project3_gbward.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; } = null!;
        public List<ReviewSentiment> Reviews { get; set; } = new();
        public double AverageSentiment { get; set; }
        public string SentimentLabel { get; set; } = string.Empty;
    }
}
