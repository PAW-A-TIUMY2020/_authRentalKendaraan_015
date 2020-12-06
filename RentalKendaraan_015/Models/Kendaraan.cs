using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalKendaraan_015.Models
{
    public partial class Kendaraan
    {
        public Kendaraan()
        {
            Peminjaman = new HashSet<Peminjaman>();
        }

        public int IdKendaraan { get; set; }

        [Required(ErrorMessage = "Nama Kendaraan Wajib diisi!")]
        public string NamaKendaraan { get; set; }

        [MaxLength(9, ErrorMessage = "No Polisi Salah")]
        [Required(ErrorMessage = "No Polisi Wajib diisi!")]
        public string NoPolisi { get; set; }

        [Required(ErrorMessage = "No STNK Wajib diisi!")]
        public string NoStnk { get; set; }

        [Required(ErrorMessage = "Jenis Kendaraan Wajib diisi!")]
        public int? IdJenisKendaraan { get; set; }

        [Required(ErrorMessage = "Ketersediaan Wajib diisi!")]
        public string Ketersediaan { get; set; }

        public JenisKendaraan IdJenisKendaraanNavigation { get; set; }
        public ICollection<Peminjaman> Peminjaman { get; set; }
    }
}
