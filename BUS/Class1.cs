using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model1;

namespace BUS
{
    public class Class1
    {
        private Model1 _context;

        // Dịch phương thức khởi tạo
        public ProductRepository(Model1 context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả sản phẩm
        public List<Sanpham> LayTatCaSanPham()
        {
            return _context.Sanpham.Include(s => s.LoaiSP).ToList();
        }

        // Lấy sản phẩm theo mã sản phẩm
        public Sanpham LaySanPhamTheoMa(string maSP)
        {
            return _context.Sanpham.FirstOrDefault(p => p.MaSP == maSP);
        }

        // Tạo sản phẩm mới
        public void TaoSanPham(Sanpham product)
        {
            _context.Sanpham.Add(product);
            _context.SaveChanges();
        }

        // Cập nhật sản phẩm
        public void CapNhatSanPham(Sanpham product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        
