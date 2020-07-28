using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication5.Models
{
    public class ManagePostVM
    {
        public int id { get; set; }
        public string categoryId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string createOn { get; set; }
        public string userId { get; set; }
        public string tag { get; set; }
        public string image { get; set; }
        public Nullable<int> postView { get; set; }
        public Nullable<int> postLike { get; set; }
        public string RawContent { get; set; }
        public Nullable<int> isActive { get; set; }
        public string categoryName { get; set; }
    }
}