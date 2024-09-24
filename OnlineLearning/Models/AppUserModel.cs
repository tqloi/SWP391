﻿using Microsoft.AspNetCore.Identity;

namespace OnlineLearning.Models
{
    public class AppUserModel: IdentityUser
    {
        public string ProfileImagePath { get; set; }
    }
}