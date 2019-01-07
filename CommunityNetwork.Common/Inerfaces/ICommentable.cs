using CommunityNetwork.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Inerfaces
{
    interface ICommentable
    {
        List<Comment> Comments();
        List<Profile> Commenters();
    }
}
