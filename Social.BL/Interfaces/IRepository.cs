﻿using CommunityNetwork.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.BL.Interfaces
{
    public interface IRepository
    {
        TNode Get<TNode>(Guid id) where TNode : Node;
        TNode Add<TNode>(TNode node) where TNode : Node;
    }
}