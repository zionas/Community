using System;
using CommunityNetwork.Common.Inerfaces;


namespace CommunityNetwork.Common.Models
{
    
        public class Node : INode
    {
        public Guid Id { get; set; }
        public string NodeName { get { return GetType().Name; } }
        public DateTime CreateTime { get; }
        public Node()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }
        public void ResetId()
        {

            if (Id == default(Guid))
                Id = Guid.NewGuid();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Id == ((Node)obj).Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

