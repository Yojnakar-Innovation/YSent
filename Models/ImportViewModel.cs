using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace YSent.Models
{
    public class ImportViewModel
    {


        [Required]
        [Display(Name = "CSV File")]
        public IFormFile CsvFile { get; set; }

        [Required]
        [HiddenInput]
        public int ListId { get; set; } = 8;

        [Required]
        [HiddenInput]
        public int App { get; set; } = 2;

    }
}
