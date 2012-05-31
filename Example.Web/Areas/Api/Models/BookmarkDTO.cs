using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Api.Models
{
    public class BookmarkDTO
    {
        public virtual int Id { get; set; }

        public virtual string Title { get; set; }

        public virtual string Url { get; set; }

        public virtual string Description { get; set; }

        public virtual string Tags { get; set; }

        [Display(Name = "Type")]
        public virtual int BookmarkTypeId { get; set; }

        [Display(Name = "Type")]
        public virtual string BookmarkTypeTypeName { get; set; }

        [Display(Name = "Date Started")]
        public virtual DateTime DateStarted { get; set; }

        [Display(Name = "Date Closed")]
        public virtual DateTime DateClosed { get; set; }
    }
}