using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalKendaraan_015.Models
{
    public partial class Pengembalian
    {
        public int IdPengembalian { get; set; }

        [Required(ErrorMessage = "Tanggal pengembalian Wajib diisi!")]
        public DateTime? TglPengembalian { get; set; }

        [Required(ErrorMessage = "Peminjam Wajib diisi!")]
        public int? IdPeminjaman { get; set; }

        [Required(ErrorMessage = "Kondisi Wajib diisi!")]
        public int? IdKondisi { get; set; }

        [Required(ErrorMessage = "Denda Kendaraan Wajib diisi!")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Denda hanya boleh diisi dengan angka")]
        public int? Denda { get; set; }

        public KondisiKendaraan IdKondisiNavigation { get; set; }
        public Peminjaman IdPeminjamanNavigation { get; set; }
    }
}
