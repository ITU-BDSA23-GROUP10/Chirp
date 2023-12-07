using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure.Models;
using System.Text.RegularExpressions;

namespace Chirp.Web.Pages;

public class HashtagModel : PageModel
{
    public string TagName { get; set; }
    readonly ICheepRepository<Cheep, Author> _cheepService;

    public List<CheepDTO> cheeps { get; set; } = new List<CheepDTO>();

    public HashtagModel(ICheepRepository<Cheep, Author> cheepService)
    {
        _cheepService = cheepService;
    }    

    public async Task<ActionResult> OnGetAsync(string tagName)
    {
        try
        {
            TagName = tagName;

            cheeps = await _cheepService.GetCheepsByHashtag(tagName);

            return Page();
        }
        catch (NullReferenceException)
        {
            return Redirect("/");
        }
    }
    public string? GetYouTubeEmbed(string message, out string Message)
    {
        string pattern = @"(.*?)(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/(watch\?v=)?([^?&\n]+)(?:[^\n ]*)(.*)";
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
    //hashtags
    //inspired from hashtag code from worklizard.com
    public List<string>? GetHashTags(string message, out string Message)
    {
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