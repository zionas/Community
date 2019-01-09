using System;
using CommunityNetwork.Common.Inerfaces;


namespace CommunityNetwork.Common.Models
{
    
        public class MNode : INode
    {
        public string Id { get; set; }
        public string NodeName { get { return GetType().Name; } }
        public DateTime CreateTime { get; }
        public MNode()
        {
            Id = Guid.NewGuid().ToString();
            CreateTime = DateTime.Now;
        }
        public void ResetId()
        {

            if (Id == default(string))
                Id = Guid.NewGuid().ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Id == ((MNode)obj).Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

