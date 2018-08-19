using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class ContactViewModels
    {
            [Display(Name = "Name:")]
            [Required(ErrorMessage = "Full name is required")]
            public string FullName { get; set; }


            [Display(Name = "Email Address:")]
            [Required(ErrorMessage = "Email is required")]
            public string Email { get; set; }

            [Display(Name = "Phone Number:")]
            [StringLength(60, MinimumLength = 10)]
            public string PhoneNumber { get; set; }

            [Display(Name = "Purpose:")]
            public Purpose Purpose { get; set; }


            [Display(Name = "Message:")]
            [Required(ErrorMessage = "Please state your message.")]
            public string Message { get; set; }

        }
            public enum Purpose
            {
                 Comment,
                 Question,
                 Employment,
                 Message,
                 Other
            }
    
    }

