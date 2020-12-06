using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalKendaraan_015.Models
{
    public partial class Peminjaman
    {
        public Peminjaman()
        {
            Pengembalian = new HashSet<Pengembalian>();
        }

        public int IdPeminjaman { get; set; }

        [Required(ErrorMessage = "Tanggal Peminjaman Wajib diisi!")]
        public DateTime? TglPeminjaman { get; set; }

        [Required(ErrorMessage = "Kendaraan Wajib diisi!")]
        public int? IdKendaraan { get; set; }

        [Required(ErrorMessage = "Customer Wajib diisi!")]
        public int? IdCustomer { get; set; }

        [Required(ErrorMessage = "Jaminan Wajib diisi!")]
        public int? IdJaminan { get; set; }

        [Required(ErrorMessage = "Biaya Wajib diisi!")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Denda hanya boleh diisi dengan angka")]
        public int? Biaya { get; set; }

        public Customer IdCustomerNavigation { get; set; }
        public Jaminan IdJaminanNavigation { get; set; }
        public Kendaraan IdKendaraanNavigation { get; set; }
        public ICollection<Pengembalian> Pengembalian { get; set; }
    }
}
