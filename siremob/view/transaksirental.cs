using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using siremob.service;

namespace siremob.view
{
    // Nama class diubah menjadi transaksirental_view agar tidak bentrok dengan Model
    public partial class transaksirental_view : Form
    {
        private transaksirental_serve sewaService = new transaksirental_serve();
        private decimal hargaMobilTerpilih = 0;

        // Nama Constructor harus sama dengan nama Class baru
        public transaksirental_view()
        {
            InitializeComponent();
        }

        private void rental_Load(object sender, EventArgs e)
        {
            RefreshHalaman();
        }

        private void RefreshHalaman()
        {
            try
            {
                dgv_transaksi.DataSource = sewaService.TampilkanDaftarRental();

                DataTable dtPelanggan = sewaService.AmbilSemuaPelanggan();
                cmb_pelanggan.DataSource = dtPelanggan;
                cmb_pelanggan.DisplayMember = "nama_pelanggan";
                cmb_pelanggan.ValueMember = "id_pelanggan";
                cmb_pelanggan.SelectedIndex = -1;

                DataTable dtMobil = sewaService.AmbilMobilTersedia();
                cmb_mobil.DataSource = dtMobil;
                cmb_mobil.DisplayMember = "merk";
                cmb_mobil.ValueMember = "id_mobil";
                cmb_mobil.SelectedIndex = -1;

                txt_lama_sewa.Clear();
                txt_total.Clear();
                hargaMobilTerpilih = 0;

                dtp_tgl_sewa.Value = DateTime.Now;
                dtp_tgl_kembali.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyegarkan halaman: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HitungEstimasiBiaya()
        {
            if (cmb_mobil.SelectedIndex == -1) return;

            DateTime tglSewa = dtp_tgl_sewa.Value.Date;
            DateTime tglKembali = dtp_tgl_kembali.Value.Date;

            TimeSpan selisih = tglKembali - tglSewa;
            int lamaSewa = selisih.Days;

            if (lamaSewa < 0) lamaSewa = 0;

            txt_lama_sewa.Text = lamaSewa.ToString();

            decimal totalBiaya = lamaSewa * hargaMobilTerpilih;
            txt_total.Text = totalBiaya.ToString();
        }

        private void dtp_tgl_sewa_ValueChanged(object sender, EventArgs e) => HitungEstimasiBiaya();
        private void dtp_tgl_kembali_ValueChanged(object sender, EventArgs e) => HitungEstimasiBiaya();

        private void cmb_mobil_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_mobil.SelectedIndex != -1 && cmb_mobil.SelectedItem is DataRowView)
            {
                DataRowView row = (DataRowView)cmb_mobil.SelectedItem;
                hargaMobilTerpilih = Convert.ToDecimal(row["harga_sewa"]);
                HitungEstimasiBiaya();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (cmb_pelanggan.SelectedIndex == -1 || cmb_mobil.SelectedIndex == -1 || string.IsNullOrEmpty(txt_total.Text))
            {
                MessageBox.Show("Harap lengkapi data Pelanggan, Mobil, dan Tanggal terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tglSewa = dtp_tgl_sewa.Value;
            DateTime tglKembali = dtp_tgl_kembali.Value;

            if (tglKembali.Date < tglSewa.Date)
            {
                MessageBox.Show("Tanggal kembali tidak boleh lebih awal dari tanggal sewa!", "Validasi Salah", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idRental = "R01";
            string idMobil = cmb_mobil.SelectedValue.ToString();
            string idPelanggan = cmb_pelanggan.SelectedValue.ToString();
            decimal totalBiaya = Convert.ToDecimal(txt_total.Text);

            bool apakahSukses = sewaService.SimpanTransaksiSewa(idRental, idMobil, idPelanggan, tglSewa, tglKembali, totalBiaya);

            if (apakahSukses)
            {
                MessageBox.Show("Transaksi Rental berhasil disimpan! Status mobil otomatis berubah menjadi 'Disewa'.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshHalaman();
            }
            else
            {
                MessageBox.Show("Gagal menyimpan transaksi! Periksa kembali koneksi atau data database Anda.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            RefreshHalaman();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}