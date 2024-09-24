﻿using System.ComponentModel.DataAnnotations;

namespace OnlineLearning.Models.ViewModel
{
    public class RegisterViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string  Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(40, MinimumLength =4)]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage ="Password does not match")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(10)]
        public string PhoneNumber { get; set; }

    }
}