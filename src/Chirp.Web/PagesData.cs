namespace Chirp.Web;

public class PagesData {
        //pagination
        public static int CheepsPerPage = 32;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages  { get; set; } = 1;
        public bool LastPage   { get; set; } = false;
        public bool HasAuthor   { get; set; } = true;
    }