﻿using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos
{
    public class EditarAdminDto
    {
        [Required]
        [EmailAddress]
        public string  Email { get; set; }
    }
}
