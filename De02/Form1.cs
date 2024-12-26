using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using DAL.Model1;

namespace De02
{
    public partial class Form1 : Form
    {

        private Model1 context;

        public Form1()
        {
            InitializeComponent();
            context = new Model1(); 
        }


        private void LoadData()
        {
            lvSanpham.Items.Clear();
            var products = context.Sanpham.Include(s => s.LoaiSP).ToList();

            foreach (var product in products)
            {
                var item = new ListViewItem(product.MaSP);
                item.SubItems.Add(product.TenSP);
                item.SubItems.Add(product.NgayNhap.HasValue ? product.NgayNhap.Value.ToString("dd/MM/yyyy") : "Chưa có ngày nhập");
                item.SubItems.Add(product.LoaiSP.TenLoai);
                lvSanpham.Items.Add(item);
            }
        }
        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTim.Text.ToLower();
            var filteredProducts = context.Sanpham
                .Include(s => s.LoaiSP)
                .Where(p => p.TenSP.ToLower().Contains(searchTerm) || p.MaSP.ToLower().Contains(searchTerm))
                .ToList();

            lvSanpham.Items.Clear();

            foreach (var product in filteredProducts)
            {
                var item = new ListViewItem(product.MaSP);
                item.SubItems.Add(product.TenSP);
                item.SubItems.Add(product.NgayNhap.HasValue ? product.NgayNhap.Value.ToString("dd/MM/yyyy") : "Chưa có ngày nhập");
                item.SubItems.Add(product.LoaiSP.TenLoai);
                lvSanpham.Items.Add(item);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem các trường dữ liệu có hợp lệ hay không (ví dụ: không để trống mã sản phẩm và tên sản phẩm)
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) || string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm.");
                return;
            }

            // Kiểm tra nếu mã sản phẩm đã tồn tại trong cơ sở dữ liệu
            var existingProduct = context.Sanpham.FirstOrDefault(p => p.MaSP == txtMaSP.Text);
            if (existingProduct != null)
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại. Vui lòng nhập mã sản phẩm khác.");
                return;
            }

            // Tạo một đối tượng sản phẩm mới
            var newProduct = new Sanpham
            {
                MaSP = txtMaSP.Text,
                TenSP = txtTenSP.Text,
                NgayNhap = dtNgaynhap.Value,  // Ngày nhập từ DateTimePicker
                MaLoai = cboLoaiSP.SelectedValue.ToString()  // Mã loại sản phẩm từ ComboBox
            };

            // Thêm sản phẩm mới vào cơ sở dữ liệu
            context.Sanpham.Add(newProduct);

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();
                MessageBox.Show("Thêm sản phẩm thành công!");

                // Sau khi thêm, reset form nhập liệu
                ClearInputFields();
                LoadData();  // Tải lại dữ liệu vào ListView
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra trong quá trình lưu, hiển thị thông báo lỗi
                MessageBox.Show("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (lvSanpham.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa.");
                return;
            }

            var selectedItem = lvSanpham.SelectedItems[0];
            var productId = selectedItem.Text;
            var productToDelete = context.Sanpham.FirstOrDefault(p => p.MaSP == productId);

            if (productToDelete != null)
            {
                context.Sanpham.Remove(productToDelete);
                context.SaveChanges();
                MessageBox.Show("Xóa sản phẩm thành công!");
                LoadData();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (lvSanpham.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.");
                return;
            }

            // Lấy mã sản phẩm từ mục đã chọn trong ListView
            var selectedProductCode = lvSanpham.SelectedItems[0].SubItems[0].Text;

            // Tìm sản phẩm trong cơ sở dữ liệu theo mã sản phẩm
            var productToUpdate = context.Sanpham.FirstOrDefault(p => p.MaSP == selectedProductCode);

            if (productToUpdate == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm để sửa.");
                return;
            }

            // Cập nhật thông tin sản phẩm
            productToUpdate.TenSP = txtTenSP.Text;

            // Kiểm tra nếu ngày nhập hợp lệ, nếu không sẽ không thay đổi
            if (dtNgaynhap.Value != null)
            {
                productToUpdate.NgayNhap = dtNgaynhap.Value;
            }

            // Kiểm tra xem ComboBox có giá trị hợp lệ trước khi gán
            if (cboLoaiSP.SelectedValue != null)
            {
                productToUpdate.MaLoai = cboLoaiSP.SelectedValue.ToString();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm.");
                return;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                context.SaveChanges();
                MessageBox.Show("Sửa sản phẩm thành công!");

                // Sau khi sửa, tải lại dữ liệu vào ListView
                LoadData();
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra trong quá trình lưu, hiển thị thông báo lỗi
                MessageBox.Show("Lỗi khi sửa sản phẩm: " + ex.Message);
            }

        }
        private void ClearInputFields()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra tính hợp lệ của các trường nhập liệu
            if (string.IsNullOrEmpty(txtTenSP.Text))
            {
                MessageBox.Show("Tên sản phẩm không được để trống.");
                return;
            }

            if (cboLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm.");
                return;
            }

            // Kiểm tra ngày nhập
            if (dtNgaynhap.Value == null || dtNgaynhap.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày nhập không hợp lệ.");
                return;
            }

            // Kiểm tra nếu đang tạo mới hay sửa một sản phẩm
            Sanpham productToSave;

            // Nếu mã sản phẩm không trống, có thể là sửa sản phẩm đã tồn tại
            if (!string.IsNullOrEmpty(txtMaSP.Text))
            {
                // Lấy sản phẩm từ cơ sở dữ liệu
                productToSave = context.Sanpham.FirstOrDefault(p => p.MaSP == txtMaSP.Text);

                if (productToSave == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để sửa.");
                    return;
                }
            }
            else
            {
                // Nếu không có mã sản phẩm (đang tạo mới), tạo một sản phẩm mới
                productToSave = new Sanpham();
                context.Sanpham.Add(productToSave);
            }

            // Cập nhật thông tin sản phẩm từ các điều khiển giao diện
            productToSave.MaSP = txtMaSP.Text;
            productToSave.TenSP = txtTenSP.Text;
            productToSave.NgayNhap = dtNgaynhap.Value;
            productToSave.MaLoai = cboLoaiSP.SelectedValue.ToString();

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();

                // Thông báo thành công
                MessageBox.Show("Lưu sản phẩm thành công!");

                // Cập nhật lại giao diện, ví dụ: tải lại danh sách sản phẩm trong ListView
                LoadData();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có vấn đề trong quá trình lưu
                MessageBox.Show("Lỗi khi lưu sản phẩm: " + ex.Message);
            }
        }

        private void btnKoLuu_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            cboLoaiSP.DataSource = context.LoaiSP.ToList();
            cboLoaiSP.DisplayMember = "TenLoai";
            cboLoaiSP.ValueMember = "MaLoai";
        }
    }
}
