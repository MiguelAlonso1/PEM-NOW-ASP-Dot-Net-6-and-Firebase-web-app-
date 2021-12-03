using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;//for DisplayName

namespace TestingFirebase.Models
{
    public class MainCategory
    {
        // [System.ComponentModel.DataAnnotations.Key]
        [Key]
        public string Key { get; set; }//primary key for Firebase object

        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Title can't be empty, boi!")]
        //[StringLength(100)]
        public string Title { get; set; }

        [DisplayName("Category Description")]
        [Required(ErrorMessage = "Kategory description can't be empty, boi!")]
        //[StringLength(100)]
        public string CategoryDescription { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Image Link")]//this makes this field Image Link instead of ImageUrl when used in html
        [Required(ErrorMessage = "An Image Url is required, boi!")]
        public string ImageUrl { get; set; }
    }
}