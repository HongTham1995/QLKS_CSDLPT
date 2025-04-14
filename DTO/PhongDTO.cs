using System;

namespace DTO
{
    public class PhongDTO
    {
        private string _maP;
        private string _tenP;
        private int _loaiP; // 0 là VIP, 1 là Thường
        private int _giaP;
        private int _chiTietLoaiP; // 0 là Phòng đơn, 1 là Phòng đôi, 2 là Phòng gia đình
        private int _tinhTrang; // 0 là Phòng trống, 1 là Phòng có khách, 2 là Phòng chưa được dọn, 3 là Phòng đang sửa chữa
        private int _hienTrang; // 0 là mới, 1 là cũ
        private int _xuLy; // 0 là chưa xóa, 1 là đã xóa
        private string _maCN; // Mã chi nhánh

        public PhongDTO()
        {
        }

        public PhongDTO(string maP, string tenP, int loaiP, int giaP, int chiTietLoaiP, int tinhTrang, int hienTrang, int xuLy, string maCN)
        {
            _maP = maP;
            _tenP = tenP;
            _loaiP = loaiP;
            _giaP = giaP;
            _chiTietLoaiP = chiTietLoaiP;
            _tinhTrang = tinhTrang;
            _hienTrang = hienTrang;
            _xuLy = xuLy;
            _maCN = maCN;
        }

        public string MaP { get => _maP; set => _maP = value; }
        public string TenP { get => _tenP; set => _tenP = value; }
        public int LoaiP { get => _loaiP; set => _loaiP = value; }
        public int GiaP { get => _giaP; set => _giaP = value; }
        public int ChiTietLoaiP { get => _chiTietLoaiP; set => _chiTietLoaiP = value; }
        public int TinhTrang { get => _tinhTrang; set => _tinhTrang = value; }
        public int HienTrang { get => _hienTrang; set => _hienTrang = value; }
        public int XuLy { get => _xuLy; set => _xuLy = value; }
        public string MaCN { get => _maCN; set => _maCN = value; } // Thêm property cho mã chi nhánh
    }
}
