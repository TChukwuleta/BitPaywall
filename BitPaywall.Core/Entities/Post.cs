﻿using BitPaywall.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPaywall.Core.Entities
{
    public class Post : AuditableEntity
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Story { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public PostStatusType PostType { get; set; }
        public string PostStatusTypeDesc { get { return PostType.ToString(); } }
        public PostCategory PostCategory { get; set; }
        public string PostCategoryDesc { get { return PostCategory.ToString(); } }
        public int Views { get; set; }
        public string UserId { get; set; }
    }
}
