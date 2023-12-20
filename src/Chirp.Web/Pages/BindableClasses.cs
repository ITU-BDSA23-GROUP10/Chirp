using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.BindableClasses
{   
    // These classes are used to bind data to the razor pages
    //annotations https://www.bytehide.com/blog/data-annotations-in-csharp
    public class NewFollow 
    {
        [Display(Name = "author")]
        public string? Author {get; set;} = string.Empty;
    }
    public class NewCheep 
    {
        [Required]
        [MaxLength(160)]
        [Display(Name = "text")]
        public required string Message {get; set;} = string.Empty;
    }
    public class NewReaction
    {
        public string? Reaction {get; set;} = string.Empty;
    }
    public class NewcheepId
    {
        public int? id {get; set;}
    }
    public class NewEmail 
    {
        [Required]
        [Display(Name = "email")]
        public required string Email {get; set;}
    }
    public class DeleteCheep
    {
        [Required]
        [Display(Name = "CheepId")]
        public int CheepID {get; set;} = -1;
    }
}