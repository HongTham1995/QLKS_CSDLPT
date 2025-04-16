using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace BUS
{
    public class PhongBUS
    {
        Database db;

        public PhongBUS()
        {
            db = new Database();
        }

        // Lấy danh sách phòng ở server cụ thể (chi nhánh) với điều kiện XuLy = 0
        public DataTable getListPhong(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.PHONG WHERE XuLy = 0";
            return db.getList(query);
        }

        // Lấy danh sách phòng dạng DTO từ server cụ thể
        public List<PhongDTO> getListPhong_DTO(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.PHONG WHERE XuLy = 0";
            return db.getListPhong_DTO(query);
        }

        // Đếm số phòng từ bảng PHONG trên server cụ thể
        public int getCountPhong(string serverName)
        {
            string query = $"SELECT COUNT(MaP) FROM {serverName}.QLKS_PT.dbo.PHONG";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Thêm phòng mới vào server cụ thể
        public void ThemPhong(string serverName, string maP, string tenP, string loaiP, string giaP, string chiTietLoaiP, string tinhTrang, string hienTrang, string maCN)
        {
            string rowGuid = Guid.NewGuid().ToString();
            string query = "INSERT INTO " + $"{serverName}.QLKS_PT.dbo.PHONG " +
               "(maP, tenP, loaiP, giaP, chiTietLoaiP, tinhTrang, hienTrang, xuLy, MaCN, rowquid) " +
               "VALUES ('" + maP + "', N'" + tenP + "', " + loaiP + ", " + giaP + ", " + chiTietLoaiP +
               ", " + tinhTrang + ", " + hienTrang + ", 0, '" + maCN + "', '" + rowGuid + "')";

            db.ExecuteNonQuery(query);
        }

        // Sửa thông tin phòng trên server cụ thể
        public void SuaPhong(string serverName, string maP, string tenP, string loaiP, string giaP, string chiTietLoaiP, string hienTrang)
        {
            string query = string.Format("UPDATE {0}.QLKS_PT.dbo.PHONG SET tenP = N'{1}', loaiP = {2}, giaP  = {3}, chiTietLoaiP = {4}, hienTrang = {5} WHERE maP = '{6}'",
                                         serverName, tenP, loaiP, giaP, chiTietLoaiP, hienTrang, maP);
            db.ExecuteNonQuery(query);
        }

        // Xóa phòng (đánh dấu xuLy = 1) trên server cụ thể
        public void XoaPhong(string serverName, string maP)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.PHONG SET xuLy = 1 WHERE maP = '{maP}'";
            db.ExecuteNonQuery(query);
        }

        // Sửa tình trạng phòng trên server cụ thể
        public void SuaTinhTrang(string serverName, string maP, string tinhTrang)
        {
            string query = string.Format("UPDATE {0}.QLKS_PT.dbo.PHONG SET tinhTrang = {1} WHERE maP = '{2}'",
                                         serverName, tinhTrang, maP);
            db.ExecuteNonQuery(query);
        }
    }
}
