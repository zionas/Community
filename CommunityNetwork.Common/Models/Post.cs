﻿using CommunityNetwork.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityNetwork.Common.Models
{
    public class Post
    {
        public DateTime CreateTime { get; set; }
        public VisibilityPermission VisibilityPermission { get; set; }
        public string ImageSourcePath { get; set; }
        private string _content;

        public int GetLikes()
        {
            /// neo4j return count of likes
            return 0;
        }

        public List<Comment> GetComments()
        {
            /// neo4j return count of likes
            return new List<Comment>();
        }

        public void DoLike(Profile profile)
        {
            /// neo4j create connection of 'Like'
        }

        public void Comment(Profile profile)
        {
            /// neo4j create connection of 'Like'
        }

        public void EditPost(string content)
        {
            _content = content;
        }
        

        public List<Profile> GetLikers()
        {
            return new List<Profile>();
        }

        public Post(string content)
        {
            this._content = content;
        }


    }
}