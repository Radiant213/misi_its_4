using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormProduk
{
    public partial class FormProdukDetail : Form
    {
        public FormProdukDetail()
        {
            InitializeComponent();
        }
        public int? ProdukId { get; set; } = null;
        private void LoadDataProduk()
        {
            if (ProdukId == null) return;
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT NamaProduk, Harga, Stok, KategoriId, Deskripsi FROM Produk WHERE Id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", ProdukId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNamaProduk.Text = reader["NamaProduk"].ToString();
                        txtHarga.Text = reader["Harga"].ToString();
                        txtStok.Text = reader["Stok"].ToString();
                        cmbKategori.SelectedValue = Convert.ToInt32(reader["KategoriId"]);
                        txtDeskripsi.Text = reader["Deskripsi"].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat data produk: " + ex.Message);
                }
            }
        }
        private void FormProdukDetail_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Id, NamaKategori FROM Kategori";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    Dictionary<int, string> kategoriDict = new Dictionary<int,
                    string>();
                    while (reader.Read())
                    {
                        kategoriDict.Add((int)reader["Id"],
                        reader["NamaKategori"].ToString());
                    }
                    if (kategoriDict.Count == 0)
                    {
                           MessageBox.Show("Tidak ada kategori ditemukan di database.");
                    }
                    cmbKategori.DataSource = new BindingSource(kategoriDict, null);
                    cmbKategori.DisplayMember = "Value";
                    cmbKategori.ValueMember = "Key";
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memuat kategori: " + ex.Message);
                }
                if (ProdukId.HasValue)
                {
                    LoadDataProduk();
                    this.Text = "Edit Produk";
                }
                else
                {
                    this.Text = "Tambah Produk";
                }
            }
        }
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi txtHarga
            if (string.IsNullOrWhiteSpace(txtHarga.Text) || !decimal.TryParse(txtHarga.Text, out _))
            {
                MessageBox.Show("Harga harus diisi dengan angka!");
                txtHarga.Focus();
                return;
            }

            // Validasi txtStok
            if (string.IsNullOrWhiteSpace(txtStok.Text) || !int.TryParse(txtStok.Text, out _))
            {
                MessageBox.Show("Stok harus diisi dengan angka!");
                txtStok.Focus();
                return;
            }

            // Validasi cmbKategori dan txtNamaProduk serta txtDeskripsi
            if (cmbKategori.SelectedItem == null)
            {
                MessageBox.Show("Kategori harus dipilih!");
                cmbKategori.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamaProduk.Text))
            {
                MessageBox.Show("Nama Produk harus diisi!");
                txtNamaProduk.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDeskripsi.Text))
            {
                MessageBox.Show("Deskripsi harus diisi!");
                txtDeskripsi.Focus();
                return;
            }

            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query;
                    if (ProdukId.HasValue)
                    {
                        // UPDATE
                        query = @"UPDATE Produk SET NamaProduk = @nama, Harga = @harga, Stok = @stok, Deskripsi = @deskripsi, KategoriId = @kategori WHERE Id = @id";
                    }
                    else
                    {
                        // INSERT (modul sebelumnya)
                        query = @"INSERT INTO Produk (NamaProduk, Harga, Stok, KategoriId, Deskripsi) VALUES (@nama, @harga, @stok, @kategori, @deskripsi)";
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (ProdukId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", ProdukId.Value);
                    }
                    cmd.Parameters.AddWithValue("@nama", txtNamaProduk.Text);
                    cmd.Parameters.AddWithValue("@harga", Convert.ToDecimal(txtHarga.Text));
                    cmd.Parameters.AddWithValue("@stok", Convert.ToInt32(txtStok.Text));
                    cmd.Parameters.AddWithValue("@kategori", ((KeyValuePair<int,string>)cmbKategori.SelectedItem).Key);
                    cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);
                    cmd.ExecuteNonQuery();
                    if (ProdukId.HasValue)
                    {
                        MessageBox.Show("Produk berhasil di edit!");
                    }
                    else
                    {
                        MessageBox.Show("Produk berhasil ditambahkan!");
                    }
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menambahkan produk: " + ex.Message);
                }
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
