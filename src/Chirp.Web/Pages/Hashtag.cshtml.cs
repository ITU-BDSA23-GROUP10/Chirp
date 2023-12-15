using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.Text.RegularExpressions;

namespace Chirp.Web.Pages;

public class HashtagModel : BasePageModel
{
    public HashtagModel(
        ICheepRepository<Cheep, Author> cheepService,
        IAuthorRepository<Author, Cheep, User> authorService,
        IUserRepository<User> userService,
        IReactionRepository<Reaction> reactionService,
        IFollowsRepository<Follows> followsService)
        : base(cheepService, authorService, userService, reactionService, followsService)
        {
        }
    public string TagName { get; set; }
    public async Task<ActionResult> OnGetAsync(string tagName)
    {
        try
        {
            TagName = tagName;

            Cheeps = await _cheepService.GetCheepsByHashtag(tagName);

            return Page();
        }
        catch (NullReferenceException)
        {
            return Redirect("/");
        }
    }
}