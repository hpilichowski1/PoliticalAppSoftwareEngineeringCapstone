using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PoliticalApp.Models
{
    public class Bill : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notify([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public int Id { get; set; }
        public int Congress { get; set; }
        public string BillType { get; set; } = string.Empty;
        public int BillNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PolicyArea { get; set; }
        public string? SponsorName { get; set; }
        public DateTime? LatestActionDate { get; set; }
        public string? LatestActionText { get; set; }
        public string? SummaryText { get; set; }

        private int _upVotes;
        public int UpVotes
        {
            get => _upVotes;
            set { _upVotes = value; Notify(); }
        }

        private int _downVotes;
        public int DownVotes
        {
            get => _downVotes;
            set { _downVotes = value; Notify(); }
        }

        private VoteType? _userVote;
        public VoteType? UserVote
        {
            get => _userVote;
            set { _userVote = value; Notify(); }
        }
    }

    public enum VoteType
    {
        None = 0,
        Up = 1,
        Down = -1
    }
}
