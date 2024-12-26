using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model1;

namespace DAL
{
    public class Class1
    {
        private readonly ProductRepository _productRepository;

        // Dịch phương thức khởi tạo
        public ProductService(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Lấy danh sách sản phẩm
        public List<Sanpham> LayDanhSachSanPham()
        {
            return _productRepository.GetAllProducts();
        }

        // Tạo sản phẩm mới
        public void TaoSanPham(Sanpham product)
        {
            // Thêm logic kiểm tra dữ liệu đầu vào ở đây (ví dụ: kiểm tra mã sản phẩm đã tồn tại chưa)
            _productRepository.CreateProduct(product);
        }

        // Cập nhật sản phẩm
        public void CapNhatSanPham(Sanpham product)
        {
            // Thêm logic kiểm tra dữ liệu đầu vào ở đây
            _productRepository.UpdateProduct(product);
        }

        // Xóa sản phẩm
        public void XoaSanPham(string maSP)
        {
            var product = _productRepository.GetProductById(maSP);
            if (product != null)
            {
                _productRepository.DeleteProduct(product);
            }
        }
    }
}
