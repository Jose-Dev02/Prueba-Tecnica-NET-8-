﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Dtos
{
    public class Host_DTO
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
