using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace siremob.model
{
    internal class pengembalian_pembayaran_model
    {
        // Menyimpan properti fields pada kolom di tabel pengembalian
        public string IdPengembalian { get; set; }
        public string IdRental { get; set; }
        public DateTime TglKembaliAktual { get; set; }
        public string KondisiMobil { get; set; }
        public int KeterlambatanHari { get; set; }
        public decimal DendaKeterlambatan { get; set; }
        public decimal DendaKerusakan { get; set; }
        public decimal TotalBayar { get; set; }
        public string StatusPembayaran { get; set; }
    }
}
