using ApiVideo.Model;

namespace OnlineLearning.Models.ViewModel
{
    public class SeeAllLiveViewModel
    {
        public List<LivestreamRecordModel> LivestreamRecords { get; set; }
        public List<LiveStream> liveStreams { get; set; }
    }
}
