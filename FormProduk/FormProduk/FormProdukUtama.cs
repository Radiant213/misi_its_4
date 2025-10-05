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
    public partial class FormProdukUtama : Form
    {
        public FormProdukUtama()
        {
            InitializeComponent();
        }
        private int? GetSelectedProductId()
        {
            if (dgvProduk.SelectedRows.Count > 0)
            {
                return Convert.ToInt32(dgvProduk.SelectedRows[0].Cells["Id"].Value);
            }
            return null;
        }
        private void LoadDataProduk()
        {
            dgvProduk.Rows.Clear();
            dgvProduk.Columns.Clear();
            using (SqlConnection conn = Koneksi.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT p.Id, p.NamaProduk, p.Harga, p.Stok, k.NamaKategori, p.Deskripsi FROM Produk p LEFT JOIN Kategori k ON p.KategoriId = k.Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Tambahkan kolom ke grid
                    dgvProduk.Columns.Add("Id", "ID");
                    dgvProduk.Columns.Add("NamaProduk", "Nama Produk");
                    dgvProduk.Columns.Add("Harga", "Harga");
                    dgvProduk.Columns.Add("Stok", "Stok");
                    dgvProduk.Columns.Add("Kategori", "Kategori");
                    dgvProduk.Columns.Add("Deskripsi", "Deskripsi");
                    while (reader.Read())
                    {
                        string hargaRupiah = "Rp." + string.Format("{0:N0}", reader["Harga"]);
                        dgvProduk.Rows.Add(
                            reader["Id"],
                            reader["NamaProduk"],
                            hargaRupiah,
                            reader["Stok"],
                            reader["NamaKategori"],
                            reader["Deskripsi"]
                        );
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menampilkan data: " + ex.Message);
                }
            }
        }
        private void FormProdukUtama_Load(object sender, EventArgs e)
        {
            LoadDataProduk();
            btnEdit.Enabled = false;
            btnHapus.Enabled = false;
            dgvProduk.SelectionChanged += dgvProduk_SelectionChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            FormProdukDetail frm = new FormProdukDetail();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadDataProduk(); // refresh data setelah tambah
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedProductId();
            if (id == null)
            {
                MessageBox.Show("Pilih produk yang ingin dihapus.");
                return;
            }
            DialogResult result = MessageBox.Show("Yakin ingin menghapus produk ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = Koneksi.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM Produk WHERE Id = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Produk berhasil dihapus!");
                        LoadDataProduk();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus produk: " + ex.Message);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedProductId();
            if (id == null)
            {
                MessageBox.Show("Pilih produk yang ingin diedit.");
                return;
            }
            FormProdukDetail form = new FormProdukDetail();
            form.ProdukId = id;
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadDataProduk();
            }
        }

        private void dgvProduk_SelectionChanged(object sender, EventArgs e)
        {
            bool rowSelected = dgvProduk.SelectedRows.Count > 0;
            btnEdit.Enabled = rowSelected;
            btnHapus.Enabled = rowSelected;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedProductId();
            LoadDataProduk();
            if (id.HasValue)
            {
                foreach (DataGridViewRow row in dgvProduk.Rows)
                {
                    if (Convert.ToInt32(row.Cells["Id"].Value) == id.Value)
                    {
                        row.Selected = true;
                        dgvProduk.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
        }
    }
}
