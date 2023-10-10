namespace Chirp.Razor;

public class PagesData {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages  { get; set; } = 1;
        public bool LastPage   { get; set; } = false;
        public bool HasAuthor   { get; set; } = true;
    }