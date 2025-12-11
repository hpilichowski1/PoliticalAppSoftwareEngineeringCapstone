namespace PoliticalAppAPI.DTOs.Profile
{
    public class UserVoteDto
    {
        public int BillId { get; set; }
        public string BillTitle { get; set; } = string.Empty;
        public VoteType Vote { get; set; }
    }

    public class ProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }

        public string? State { get; set; }
        public string? Region { get; set; }

        public List<UserVoteDto> Votes { get; set; } = new();
        public int? AlignmentScore { get; set; }
    }

    public class UpdateLocationRequest
    {
        public string State { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}