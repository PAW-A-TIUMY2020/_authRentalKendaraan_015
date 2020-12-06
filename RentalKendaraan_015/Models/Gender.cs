using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalKendaraan_015.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Customer = new HashSet<Customer>();
        }

        public int IdGender { get; set; }

        [Required(ErrorMessage = "Nama Gender Wajib diisi!")]
        [RegularExpression("[L/P]", ErrorMessage = "Gender hanya boleh diisi dengan L/P")]
        public string NamaGender { get; set; }

        public ICollection<Customer> Customer { get; set; }
    }
}
