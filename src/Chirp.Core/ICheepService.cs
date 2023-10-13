namespace Chirp.Core;
public interface ICheepService
{
    //page is for pagination
    public (List<CheepDTO>, int CheepsCount) GetCheeps(int page);
    public (List<CheepDTO>, int CheepsCount) GetCheepsFromAuthor(string author, int page);
    int GetLimit();
}