using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.Text.RegularExpressions;
using Chirp.Web.BindableClasses;

namespace Chirp.Web.Pages
{
    //this contains all methods collectively used in UserTimeline.cshtml.cs, Public.cshtml.cs, HashTag.cshtml.cs
    //the methods are then used in the connected html pages
    public class BasePageModel : PageModel
    {   
        // Every one of these bind properties is used in the connected html pages. 
        // They provide a way to get data from the html pages
        [BindProperty]
        public NewCheep NewCheep { get; set; } = new NewCheep { Message = string.Empty };

        [BindProperty]
        public NewFollow NewFollow { get; set; } = new();

        [BindProperty]
        public NewcheepId NewcheepId { get; set; } = new();

        [BindProperty]
        public NewReaction NewReaction { get; set; } = new();

        protected readonly ICheepRepository<Cheep, Author> _cheepService;
        protected readonly IAuthorRepository<Author, Cheep, User> _authorService;
        protected readonly IUserRepository<User> _userService;
        protected readonly IReactionRepository<Reaction> _reactionService;
        protected readonly IFollowsRepository<Follows> _followsService;
        protected readonly int excessiveCheepsCount = 32;

        public List<CheepDTO> UserCheeps { get; set; } = new List<CheepDTO>();
        //DisplayedCheeps is used in public.cshtml
        public List<CheepDTO> DisplayedCheeps { get; set; } = new List<CheepDTO>();
        //Cheeps is used in timeline.cshtml and Hashtags.cshtml
        public List<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
        public BasePageModel(
            ICheepRepository<Cheep, Author> cheepService,
            IAuthorRepository<Author, Cheep, User> authorService,
            IUserRepository<User> userService,
            IReactionRepository<Reaction> reactionService,
            IFollowsRepository<Follows> followsService)
        {
            _authorService = authorService;
            _cheepService = cheepService;
            _userService = userService;
            _reactionService = reactionService;
            _followsService = followsService;
        }
        // This method is used to for creating a new cheep and adding it to the database
        // It also creates a new author if the author doesn't exist in the database
        public async Task<IActionResult> OnPost()
        {
            AsyncPadlock padlock = new();
            var userName = User?.Identity?.Name ?? "default";

            try
            {
                await padlock.Lock();
                var author = await _authorService.GetAuthorByName(userName);

                // Create new author if it doesn't exist in database already
                if (author is null) 
                {
                    var user = await _userService.GetUserByName(userName);

                    if (user is null) {
                        await _userService.CreateUser(userName);
                        user = await _userService.GetUserByName(userName)
                            ?? throw new InvalidOperationException("author could not be created.");
                    }

                    await _authorService.CreateAuthor(user);
                    author = await _authorService.GetAuthorByName(userName);
                }

                if (author is null) 
                {
                    throw new InvalidOperationException("author could not be created.");
                }
                
                var cheep = new CheepCreateDTO(NewCheep.Message, userName);
                await _cheepService.CreateCheep(cheep, author);

            }
            finally
            {
                padlock.Dispose();
            }

            return Redirect("/" + userName);
        }

        // This method is used to create a new follow and add it to the database
        // It also creates a new user if the user doesn't exist in the database
        public async Task<IActionResult> OnPostFollow()
        {
            var LoggedInUserName = User?.Identity?.Name ?? "default";
            var FollowedUserName = NewFollow.Author;

            // Check if followedUserName is null
            if (FollowedUserName == null)
            {
                throw new ArgumentNullException("Followed user does not exist.");
            }

            //Check if the user that is logged in exists if not create a new user
            try
            {
                var loggedInUser = await _userService.GetUserByName(LoggedInUserName);
                
                if (loggedInUser is null)
                {
                    throw new Exception("User does not exist");
                }    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await _userService.CreateUser(LoggedInUserName);
            }

            var followerId = await _userService.GetUserIDByName(LoggedInUserName);
            var followingId = await _userService.GetUserIDByName(FollowedUserName);

            var followDTO = new FollowDTO(followerId, followingId);

            await _followsService.FollowUser(followDTO);

            //return it as Json for the Ajax script, so only the form/button will reload
            return new JsonResult(new { success = true });
        }

        // This method is used to unfollow a user and remove it from the database
        public async Task<IActionResult> OnPostUnfollow()
        {
            var userName = User?.Identity?.Name ?? "default";
            
            // Check if followedUserName is null
            if (string.IsNullOrEmpty(NewFollow.Author))
            {
                throw new ArgumentException("NewFollow.Author cannot be null or empty");
            }
            else
            {
                var followerId = await _userService.GetUserIDByName(userName);
                var followingId = await _userService.GetUserIDByName(NewFollow.Author);

                var unfollowDTO = new FollowDTO(followerId, followingId);

                await _followsService.UnfollowUser(unfollowDTO);
                
                //return it as Json for the Ajax script, so only the form/button will reload
                return new JsonResult(new { success = true });
            }
        }
        // This method is used to check if a user is following another user
        public async Task<bool> CheckIfFollowed(int userId, int authorId)
        {
            return await _followsService.IsFollowing(userId, authorId);
        }
        // This method is used to find a user id by their name
        public async Task<int> FindUserIDByName(string userName)
        {
            return await _userService.GetUserIDByName(userName);
        }

        // This method is used to get a youtube embed link from a youtube link
        public string? GetYouTubeEmbed(string message, out string Message)
        {
            string pattern =  @"(.*?)(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/(watch\?v=)?([^?&\n]{11})(?:[^\n ]*)(.*)";
            Match match = Regex.Match(message, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                var videoId = match.Groups[6].Value.Substring(0, 11);
                Message = match.Groups[1].Value.Trim() + " " + match.Groups[7].Value.Trim();
                return $"https://www.youtube.com/embed/{videoId}";
            }
            else
            {
                Message = message;
                return null;
            }
        }
        // This method finds the upvote count for a cheep using its id
        public async Task<int> FindUpvoteCountByCheepID(int id)
        {
            return await _reactionService.GetCheepsUpvoteCountsFromCheepID(id);
        }
        // This method finds the downvote count for a cheep using its id
        public async Task<int> FindDownvoteCountByCheepID(int id)
        {
            return await _reactionService.GetCheepsDownvoteCountsFromCheepID(id);
        }
        // This method creates a new reaction and adds it to a cheep in the database
        public async Task<IActionResult> OnPostReaction()
        {
            // The id for the user who is reacting
            var userId = await _userService.GetUserIDByName(User!.Identity!.Name!);
            int cheepId = NewcheepId.id  ?? default(int);
            string react = NewReaction.Reaction!;

            // Checks if the user exists
            try
            {
                if(userId == -1 && User.Identity.Name != null) {
                    await _userService.CreateUser(User.Identity.Name);
                    userId = await _userService.GetUserIDByName(User.Identity.Name); 
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                throw new Exception("There was a problem whilst creating the user");
            }

            var newreact = new ReactionDTO
            (
                cheepId,
                userId, 
                react
            );

            await _reactionService.ReactToCheep(newreact);

            // Retrieve new counts for frontend ajax buttons
            int upvoteCount = await FindUpvoteCountByCheepID(cheepId);
            int downvoteCount = await FindDownvoteCountByCheepID(cheepId);

            // Return counts with the response
            return new JsonResult(
                new
                {
                    success = true,
                    upVoteCount = upvoteCount,
                    downVoteCount = downvoteCount
                });
        }

        // This method is used to get the Hashtags tied to a cheep
        // Inspired from hashtag code from worklizard.com - By Jonas Skjødt
        public List<string>? GetHashTags(string message, out string Message)
        {   
            // This regex is used to find the hastags in a cheep
            var regex = new Regex(@"(?<=#)\w+"); 
            var matches = regex.Matches(message);
            var hashTags = new List<string>();

            foreach (Match match in matches)
            {
                var formattedHashtag = $"/hashtag/{match.Value}";
                hashTags.Add(formattedHashtag);
                message = message.Replace("#" + match.Value, "");
            }

            Message = message;
            return hashTags.Count > 0 ? hashTags : null;
        }
    }
}