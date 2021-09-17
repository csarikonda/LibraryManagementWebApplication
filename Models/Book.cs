using System;
using System.Collections.Generic;

#nullable disable

namespace LibraryManagementWebApplication.Models
{
    public partial class Book
    {
        public long BookId { get; set; }
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public string PublishedDate { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Authors { get; set; }
        public string Categories { get; set; }
    }
}
