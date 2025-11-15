using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_gbward.Data;
using Fall2025_Project3_gbward.Models;
using Fall2025_Project3_gbward.Services;
using Fall2025_Project3_gbward.ViewModels;
using VaderSharp2;

namespace Fall2025_Project3_gbward.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureOpenAIService _aiService;

        public ActorsController(ApplicationDbContext context, AzureOpenAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(am => am.Movie)
                .ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            // Generate AI tweets
            var tweetTexts = await _aiService.GenerateActorTweetsAsync(actor.Name, 20);

            // Analyze sentiment for each tweet
            var analyzer = new SentimentIntensityAnalyzer();
            var tweets = new List<TweetSentiment>();

            foreach (var tweetText in tweetTexts)
            {
                var results = analyzer.PolarityScores(tweetText);
                var label = GetSentimentLabel(results.Compound);

                tweets.Add(new TweetSentiment
                {
                    Tweet = tweetText,
                    CompoundScore = results.Compound,
                    SentimentLabel = label
                });
            }

            var viewModel = new ActorDetailsViewModel
            {
                Actor = actor,
                Tweets = tweets,
                AverageSentiment = tweets.Any() ? tweets.Average(t => t.CompoundScore) : 0,
                SentimentLabel = GetSentimentLabel(tweets.Any() ? tweets.Average(t => t.CompoundScore) : 0)
            };

            return View(viewModel);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,ImdbLink")] Actor actor, IFormFile? photo)
        {
            if (photo != null && photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await photo.CopyToAsync(ms);
                    actor.Photo = ms.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbLink")] Actor actor, IFormFile? photo)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            var existingActor = await _context.Actors.FindAsync(id);
            if (existingActor == null)
            {
                return NotFound();
            }

            existingActor.Name = actor.Name;
            existingActor.Gender = actor.Gender;
            existingActor.Age = actor.Age;
            existingActor.ImdbLink = actor.ImdbLink;

            if (photo != null && photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await photo.CopyToAsync(ms);
                    existingActor.Photo = ms.ToArray();
                }
            }

            ModelState.Remove("Photo");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingActor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                var relationships = _context.MovieActors.Where(m => m.ActorId == id);
                _context.MovieActors.RemoveRange(relationships);
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }

        private string GetSentimentLabel(double compound)
        {
            if (compound >= 0.05)
                return "Positive";
            else if (compound <= -0.05)
                return "Negative";
            else
                return "Neutral";
        }
    }
}
