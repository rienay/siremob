using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using siremob.model;
using siremob.konfigurasi;

namespace siremob.service
{
    internal class transaksirental_serve
    {
        private Koneksi koneksiDb = new Koneksi();

        // 1. MENGAMBIL DATA PELANGGAN UNTUK COMBOBOX
        public DataTable AmbilSemuaPelanggan()
        {
            string query = "SELECT id_pelanggan, nama_pelanggan FROM pelanggan";
            return koneksiDb.EksekusiQuery(query);
        }

        // 2. MENGAMBIL DATA MOBIL YANG TERSEDIA UNTUK COMBOBOX
        public DataTable AmbilMobilTersedia()
        {
            string query = "SELECT id_mobil, merk, harga_sewa FROM mobil WHERE status = 'Tersedia'";
            return koneksiDb.EksekusiQuery(query);
        }

        // 3. MENGAMBIL DATA UNTUK DATAGRIDVIEW
        public DataTable TampilkanDaftarRental()
        {
            // Catatan: Ubah 'transaksirental' di bawah jika nama tabel sewa Anda di database berbeda
            string query = @"SELECT r.id_rental, p.nama_pelanggan, m.merk AS Mobil, 
                                    r.tanggalsewa, r.tangkalkembali_rencana, 
                                    r.totalbiaya_estimasi, r.statusrental 
                             FROM transaksirental r
                             INNER JOIN pelanggan p ON r.id_pelanggan = p.id_pelanggan
                             INNER JOIN mobil m ON r.id_mobil = m.id_mobil";

            return koneksiDb.EksekusiQuery(query);
        }

        // 4. FUNGSI YANG DICARI OLEH ERROR CS1061
        public bool SimpanTransaksiSewa(string idRental, string idMobil, string idPelanggan, DateTime tglSewa, DateTime tglKembali, decimal totalBiaya)
        {
            // Format tanggal agar sesuai dengan MySQL (YYYY-MM-DD)
            string sSewa = tglSewa.ToString("yyyy-MM-dd");
            string sKembali = tglKembali.ToString("yyyy-MM-dd");

            // Query A: Insert data rental baru
            // Sesuai tabel di database Anda, urutan kolom: id_rental, id_mobil, id_pelanggan, tanggalsewa, tangkalkembali_rencana, jaminan, totalbiaya_estimasi, statusrental
            string queryInsert = $@"INSERT INTO transaksirental 
                                   (id_rental, id_mobil, id_pelanggan, tanggalsewa, tangkalkembali_rencana, jaminan, totalbiaya_estimasi, statusrental) 
                                   VALUES ('{idRental}', '{idMobil}', '{idPelanggan}', '{sSewa}', '{sKembali}', '-', {totalBiaya}, 'Berjalan')";

            // Query B: Update status mobil menjadi 'Disewa'
            string queryUpdateMobil = $"UPDATE mobil SET status = 'Disewa' WHERE id_mobil = '{idMobil}'";

            // Eksekusi kedua perintah ke kelas Koneksi Anda
            int hasilInsert = koneksiDb.EksekusiNonQuery(queryInsert);
            int hasilUpdate = koneksiDb.EksekusiNonQuery(queryUpdateMobil);

            // Mengembalikan true jika kedua query berhasil dieksekusi
            return (hasilInsert > 0 && hasilUpdate > 0);
        }
    }
}