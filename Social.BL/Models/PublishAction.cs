
using CommunityNetwork.Common.Inerfaces;

namespace Social.BL.Models
{
    public class PublishAction
    {
        public string AuthorId { get; set; }
        public IPost Publish { get; set; }
        public string CommentedId { get; set; }
        public string Content { get; set; }
        public string Publisher { get; set; }
    }
}
