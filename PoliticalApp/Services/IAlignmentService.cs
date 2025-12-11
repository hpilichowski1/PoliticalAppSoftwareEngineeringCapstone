public interface IAlignmentService
{
    Task<int?> GetMyScoreAsync();
    Task<bool> SubmitScoreAsync(int score);
}
