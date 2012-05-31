using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Example.Core.Validation;
using Utilities;
using Utilities.Extensions;

namespace Example.Core.Model
{
    public class Bookmark
    {
        private string _tags;

        public Bookmark()
        {
            DateClosed = DateTime.Now;
            DateStarted = DateTime.Now;
        }

        [Required]
        public virtual BookmarkType BookmarkType { get; set; }

        [Required]
        [Display(Name = "Date Closed"), DataType(DataType.Date)]
        [GreaterThanDate("DateStarted", "Date Started")]
        public virtual DateTime DateClosed { get; set; }

        [Required]
        [Display(Name = "Date Started"), DataType(DataType.Date)]
        public virtual DateTime DateStarted { get; set; }

        [Required, StringLength(255)]
        public virtual string Description { get; set; }

        public virtual int Id { get; set; }

        [Display(Name = "Is Favourite")]
        [UIHint("YesNo")]
        public virtual bool IsFavourite { get; set; }

        [Required]
        public virtual List<string> Tags
        {
            get { return !_tags.IsNullOrEmpty() ? _tags.ToList() : new List<String>(); }
        }

        [Required, StringLength(255)]
        public virtual string Title { get; set; }

        [Required, StringLength(255)]
        [RegularExpression(RegexPattern.URL, ErrorMessage = "Not a valid url")]
        public virtual string Url { get; set; }

        public virtual void SetTags(List<string> value)
        {
            _tags = string.Join(",", value);
        }
    }
}