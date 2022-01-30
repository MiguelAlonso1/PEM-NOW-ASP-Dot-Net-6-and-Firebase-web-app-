using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using TestingFirebase.HelperClasses;

namespace TestingFirebase.Models;//Miguel upgraded this to scoped namespaces -a C# 10 new feature
public class Subcategory
    {
        [Key]
        public string Key { get; set; }//primary key. A sequence is created and incremented by default

        [Required(ErrorMessage = "Title can't be empty!")]
        public string Title { get; set; }

        //[Required(ErrorMessage = "Color can't be empty!")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Evaluation can't be empty!")]
        public string Evaluation { get; set; }

        [Required(ErrorMessage = "Medications can't be empty!")]
        public string Medications { get; set; }

        [Required(ErrorMessage = "Management can't be empty!")]
        public string Management { get; set; }

        [Required(ErrorMessage = "Symptoms can't be empty!")]
        public string Signs { get; set; }

        [Required(ErrorMessage = "References can't be empty!")]
        public string References { get; set; }

        public Object Image { get; set; }//this holds a JSON object of type array
        //for linking to Main Kategory
        [Display(Name = "Main Category ID")]
        [Required(ErrorMessage = "Main Category can't be empty!")]
        [StringRange(AllowableValues = new[] { "c1", "c2", "c3", "c4", "c5" }, ErrorMessage = "Please select a Main Category.")]
        public string SubId { get; set; }

        //this holds the image uploded as a file
        [DataType(DataType.Upload)]
        [Display(Name = "Upload Subcategory Image")]
        [Required(ErrorMessage = "An Image Needs to be Uploaded")]
        public IFormFile Pictures { get; set; }
    }