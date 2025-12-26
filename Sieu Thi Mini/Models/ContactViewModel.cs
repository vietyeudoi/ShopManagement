using System;
using System.ComponentModel.DataAnnotations;

namespace Sieu_Thi_Mini.Models
{
    public class ContactViewModel
    {
        public int ContactId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string? ThongBao { get; set; }
    }
}
