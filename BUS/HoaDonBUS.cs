using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace BUS
{
    public class HoaDonBUS
    {
        Database db;
        public HoaDonBUS()
        {
            db = new Database();
        }

        // Lấy tất cả hóa đơn (không có điều kiện nào khác)
        private DataTable GetAllHoaDon(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.HOADON";
            return db.getList(query);
        }

        // Thêm hóa đơn mới vào server xác định
        public void ThemHoaDon(string serverName, string maHD, string maCTT, string giamGia, string phuThu, string ngayThanhToan, string pttt)
        {
            string rowGuid = Guid.NewGuid().ToString();
            string query = string.Format("INSERT INTO {0}.QLKS_PT.dbo.HOADON VALUES('{1}','{2}',{3},{4},'{5}',{6},0,'{7}')",
                                          serverName, maHD, maCTT, giamGia, phuThu, ngayThanhToan, pttt,rowGuid);
            db.ExecuteNonQuery(query);
        }

        // Lấy số lượng hóa đơn trong ngày (sử dụng hàm COUNT + 1) từ server xác định
        public int SoLuongHD(string serverName, string dateNow)
        {
            string query = $"SELECT COUNT(MaHD) + 1 FROM {serverName}.QLKS_PT.dbo.HOADON WHERE CAST(ngayThanhToan AS date) = '{dateNow}'";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Lấy danh sách hóa đơn (kết hợp join và tính toán) từ server xác định
        public DataTable GetListHD(string serverName)
        {
            string query = $@"
                SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                      CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                      CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                      giamGia, phuThu, 
                      SUM(
                          CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                          CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                          (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * phuThu / 100 -
                          (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * giamGia / 100
                      ) AS TongTien,
                      ngayThanhToan, phuongThucThanhToan 
                FROM {serverName}.QLKS_PT.dbo.HOADON 
                INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT
                INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                LEFT JOIN 
                  (SELECT maHD, SUM(giaThue) AS tienPhong 
                   FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                   WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                   GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                LEFT JOIN 
                  (SELECT maHD, SUM(giaDV * SoLuong) AS tienDichVu 
                   FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                   WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                   GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan";
            return db.getList(query);
        }

        public int TongTienPhong(string serverName)
        {
            string query = $"SELECT SUM(giaThue) FROM {serverName}.QLKS_PT.dbo.HOADON, {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG " +
                           $"WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongTienDV(string serverName)
        {
            string query = $"SELECT SUM(SoLuong * giaDV) FROM {serverName}.QLKS_PT.dbo.HOADON, {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU " +
                           $"WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Tính tổng doanh thu từ hóa đơn trên server xác định
        public int TongDoanhThu(string serverName)
        {
            string query = $@"
                SELECT SUM(TongTien) AS TongTien FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong,
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu,
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * phuThu / 100 -
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * giamGia / 100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV * SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan 
                ) AS HoaDon";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongPhuThu(string serverName)
        {
            string query = $@"
                SELECT SUM((TongTienPhong + TongTienDV) * phuThu / 100) AS TongTien FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong,
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu,
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * phuThu / 100 -
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * giamGia / 100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV * SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan 
                ) AS HoaDon";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongGiamGia(string serverName)
        {
            string query = $@"
                SELECT SUM((TongTienPhong + TongTienDV) * giamGia / 100) AS TongTien FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong,
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu,
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * phuThu / 100 -
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END) * giamGia / 100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV * SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan 
                ) AS HoaDon";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Các phương thức lấy thông tin chi tiết hóa đơn theo maHD
        public DataTable getHoaDon(string serverName, string maHD)
        {
            string query = $"SELECT maHD, HOADON.maCTT, tenNV, ngayThanhToan " +
                           $"FROM {serverName}.QLKS_PT.dbo.HOADON " +
                           $"INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT " +
                           $"INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV " +
                           $"WHERE maHD = '{maHD}'";
            return db.getList(query);
        }

        public DataTable getThuePhong(string serverName, string maHD)
        {
            string query = $@"
                SELECT tenP, loaiHinhThue, ngayThue, ngayCheckOut, giaThue 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG, {serverName}.QLKS_PT.dbo.HOADON 
                WHERE CHITIETTHUEPHONG.maCTT = HOADON.maCTT 
                  AND CHITIETTHUEPHONG.maP = Phong.maP 
                  AND maHD = '{maHD}'";
            return db.getList(query);
        }

        public DataTable getDichVu(string serverName, string maHD)
        {
            string query = $@"
                SELECT tenDV, loaiDV, ngaySuDung, SoLuong, ChiTietThueDichVu.giaDV, (SoLuong*ChiTietThueDichVu.giaDV) AS Tong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.DICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                WHERE CHITIETTHUEDICHVU.maCTT = HOADON.maCTT 
                  AND CHITIETTHUEDICHVU.maDV = DICHVU.maDV 
                  AND maHD = '{maHD}'";
            return db.getList(query);
        }

        // Lấy tổng tiền của hóa đơn theo maHD
        public int TongTien(string serverName, string maHD)
        {
            string query = $@"
                SELECT TongTien FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu, 
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*phuThu/100 -
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*giamGia/100
                           ) AS TongTien, ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT 
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV*SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan 
                ) AS TB 
                WHERE maHD = '{maHD}'";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Các phương thức tính tổng theo ngày, tháng được sửa đổi theo cách tương tự:
        public int TongTienPhongTrongMotNgay(string serverName, string day, string month, string year)
        {
            string query = $@"
                SELECT CASE WHEN SUM(TongTienPhong) IS NOT NULL THEN SUM(TongTienPhong) ELSE 0 END AS TP 
                FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu, 
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*phuThu/100 - 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*giamGia/100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT 
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV*SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan
                ) AS TBHoaDon
                WHERE YEAR(ngayThanhToan) = {year} AND MONTH(ngayThanhToan) = {month} AND DAY(ngayThanhToan) = {day}";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongTienDichVuTrongMotNgay(string serverName, string day, string month, string year)
        {
            string query = $@"
                SELECT CASE WHEN SUM(TongTienDV) IS NOT NULL THEN SUM(TongTienDV) ELSE 0 END AS TP 
                FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu, 
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*phuThu/100 - 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*giamGia/100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT 
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV*SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan
                ) AS TBHoaDon
                WHERE YEAR(ngayThanhToan) = {year} AND MONTH(ngayThanhToan) = {month} AND DAY(ngayThanhToan) = {day}";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongTienPhongTrongMotThang(string serverName, string month, string year)
        {
            string query = $@"
                SELECT CASE WHEN SUM(TongTienPhong) IS NOT NULL THEN SUM(TongTienPhong) ELSE 0 END AS TP 
                FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu, 
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*phuThu/100 - 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*giamGia/100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT 
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV*SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan
                ) AS TBHoaDon
                WHERE YEAR(ngayThanhToan) = {year} AND MONTH(ngayThanhToan) = {month}";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongTienDichVuTrongMotThang(string serverName, string month, string year)
        {
            string query = $@"
                SELECT CASE WHEN SUM(TongTienDV) IS NOT NULL THEN SUM(TongTienDV) ELSE 0 END AS TP 
                FROM 
                (
                    SELECT HOADON.maHD, HOADON.maCTT, tenNV, 
                           CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END AS TongTienPhong, 
                           CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END AS TongTienDV, 
                           giamGia, phuThu, 
                           SUM(
                                CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END + 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*phuThu/100 - 
                                (CASE WHEN tienPhong IS NOT NULL THEN tienPhong ELSE 0 END + 
                                 CASE WHEN tienDichVu IS NOT NULL THEN tienDichVu ELSE 0 END)*giamGia/100
                           ) AS TongTien,
                           ngayThanhToan, phuongThucThanhToan 
                    FROM {serverName}.QLKS_PT.dbo.HOADON 
                    INNER JOIN {serverName}.QLKS_PT.dbo.CHITIETTHUE ON HOADON.maCTT = CHITIETTHUE.maCTT 
                    INNER JOIN {serverName}.QLKS_PT.dbo.NHANVIEN ON CHITIETTHUE.maNV = NHANVIEN.maNV 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaThue) AS tienPhong 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEPHONG.maCTT 
                       GROUP BY HOADON.maHD) AS TIENPHONG ON HOADON.maHD = TIENPHONG.maHD 
                    LEFT JOIN 
                      (SELECT maHD, SUM(giaDV*SoLuong) AS tienDichVu 
                       FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU, {serverName}.QLKS_PT.dbo.HOADON 
                       WHERE HOADON.maCTT = CHITIETTHUEDICHVU.maCTT 
                       GROUP BY HOADON.maHD) AS TIENDICHVU ON HOADON.maHD = TIENDICHVU.maHD 
                    GROUP BY HOADON.maHD, HOADON.maCTT, tenNV, TIENPHONG.tienPhong, TIENDICHVU.tienDichVu, giamGia, phuThu, ngayThanhToan, phuongThucThanhToan
                ) AS TBHoaDon
                WHERE YEAR(ngayThanhToan) = {year} AND MONTH(ngayThanhToan) = {month}";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhong(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(CHITIETTHUEPHONG.ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(CHITIETTHUEPHONG.ngayThue AS date) <= '{toNgay}'";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhongVip(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(ngayThue AS date) <= '{toNgay}' 
                  AND loaiP = 0";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhongThuong(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(ngayThue AS date) <= '{toNgay}' 
                  AND loaiP = 1";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhongDon(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(ngayThue AS date) <= '{toNgay}' 
                  AND chiTietLoaiP = 0";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhongDoi(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(ngayThue AS date) <= '{toNgay}' 
                  AND chiTietLoaiP = 1";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public int TongLoaiPhongGiaDinh(string serverName, string fromNgay, string toNgay)
        {
            string query = $@"SELECT COUNT(maCTT) AS TongPhong 
                FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG, {serverName}.QLKS_PT.dbo.PHONG 
                WHERE PHONG.maP = CHITIETTHUEPHONG.maP 
                  AND CAST(ngayThue AS date) >= '{fromNgay}' 
                  AND CAST(ngayThue AS date) <= '{toNgay}' 
                  AND chiTietLoaiP = 2";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Các phương thức trên đây đã được sửa đổi theo dạng linked server.
    }
}
