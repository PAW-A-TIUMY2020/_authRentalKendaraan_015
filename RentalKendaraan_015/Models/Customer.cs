using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalKendaraan_015.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Peminjaman = new HashSet<Peminjaman>();
        }

        
        public int IdCustomer { get; set; }

        [Required(ErrorMessage = "Nama Customer Wajib diisi!")]
        public string NamaCustomer { get; set; }

        [Required(ErrorMessage = "NIK Wajib diisi!")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "NIK hanya boleh diisi dengan angka")]
        public string Nik { get; set; }

        [Required(ErrorMessage = "Alamat Customer Wajib diisi!")]
        public string Alamat { get; set; }

        [MinLength(11, ErrorMessage = "No HP tidak boleh kurang dari 10 angka")]
        [MaxLength(13, ErrorMessage = "No HP tidak boleh lebih dari 13 angka")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Nomor HP hanya boleh diisi dengan angka")]
        [Required(ErrorMessage = "Nomor HP Customer Wajib diisi!")]
        public string NoHp { get; set; }


        public int? IdGender { get; set; }

        public Gender IdGenderNavigation { get; set; }
        public ICollection<Peminjaman> Peminjaman { get; set; }
    }
}
