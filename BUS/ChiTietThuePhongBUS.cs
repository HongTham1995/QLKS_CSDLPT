using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace BUS
{
    public class ChiTietThuePhongBUS
    {
        Database db;
        public ChiTietThuePhongBUS()
        {
            db = new Database();
        }

        // Lấy danh sách chi tiết thuê phòng theo maCTT từ server xác định
        public List<ChiTietThuePhongDTO> GetDSListCTTP(string serverName, string maCTT)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG WHERE maCTT = '{maCTT}'";
            return db.getListCTTP_DTO(query);
        }

        // Lấy danh sách tất cả chi tiết thuê phòng từ server xác định
        public List<ChiTietThuePhongDTO> GetDSListCTTP(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG";
            return db.getListCTTP_DTO(query);
        }

        // Insert chi tiết thuê phòng (có trường hợp check có giá trị null cho ngayTra, ngayCheckOut)
        public void InsertCTTP(string serverName, bool check, string maCTT, string maP, string ngayThue, string ngayTra, string loaiHinhThue, string giaThue)
        {
            string rowGuid = Guid.NewGuid().ToString();
            if (check)
            {
                // Nếu check = true: insert với giá trị ngayThue, ngayTra (với ngayTra có giá trị) và giá trị kia, null cho những cột không cần
                string query = string.Format("INSERT INTO {0}.QLKS_PT.dbo.CHITIETTHUEPHONG VALUES ('{1}','{2}','{3}','{4}',NULL,{5},{6},0,'{7}')",
                                             serverName, maCTT, maP, ngayThue, ngayTra, loaiHinhThue, giaThue,rowGuid);
                db.ExecuteNonQuery(query);
            }
            else
            {
                // Nếu check = false: insert với ngayTra = null
                string query = string.Format("INSERT INTO {0}.QLKS_PT.dbo.CHITIETTHUEPHONG VALUES ('{1}','{2}','{3}',NULL,NULL,{4},{5},0,'{6}')",
                                             serverName, maCTT, maP, ngayThue, loaiHinhThue, giaThue,rowGuid);
                db.ExecuteNonQuery(query);
            }
        }

        // Xóa chi tiết thuê phòng theo maCTT, maP, ngayThue trên server xác định
        public void DeleteCTTP(string serverName, string maCTT, string maP, string ngayThue)
        {
            string query = $"DELETE FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG WHERE maCTT = '{maCTT}' AND maP = '{maP}' AND ngayThue = '{ngayThue}'";
            db.ExecuteNonQuery(query);
        }

        // Cập nhật tình trạng của chi tiết thuê phòng trên server xác định
        public void UpdateTinhTrang(string serverName, string maCTT, string maP, string ngayThue, string tinhTrang)
        {
            string query = string.Format("UPDATE {0}.QLKS_PT.dbo.CHITIETTHUEPHONG SET tinhTrang = {1} WHERE maCTT = '{2}' AND maP = '{3}' AND ngayThue = '{4}'",
                                         serverName, tinhTrang, maCTT, maP, ngayThue);
            db.ExecuteNonQuery(query);
        }

        // Cập nhật ngày check out (và có trường hợp cập nhật thêm ngày trả, giá thuê) trên server xác định
        public void UpdateCheckOut(string serverName, bool check, string maCTT, string maP, string ngayThue, string ngayCheckOut, string giaThue)
        {
            if (check)
            {
                string query = string.Format("UPDATE {0}.QLKS_PT.dbo.CHITIETTHUEPHONG SET ngayCheckOut = '{1}' WHERE maCTT = '{2}' AND maP = '{3}' AND ngayThue = '{4}'",
                                             serverName, ngayCheckOut, maCTT, maP, ngayThue);
                db.ExecuteNonQuery(query);
            }
            else
            {
                string query = string.Format("UPDATE {0}.QLKS_PT.dbo.CHITIETTHUEPHONG SET ngayCheckOut = '{1}', ngayTra = '{2}', giaThue = {3} WHERE maCTT = '{4}' AND maP = '{5}' AND ngayThue = '{6}'",
                                             serverName, ngayCheckOut, ngayCheckOut, giaThue, maCTT, maP, ngayThue);
                db.ExecuteNonQuery(query);
            }
        }

        // Lấy thông tin phòng: lấy thông tin của khách hàng và ngày trả từ các bảng liên quan trên server xác định
        public DataTable GetInfoRoom(string serverName, string maP)
        {
            string query = $@"
                SELECT TOP 1 CHITIETTHUE.maCTT, tenKH, ngayTra 
                FROM {serverName}.QLKS_PT.dbo.KHACHHANG, {serverName}.QLKS_PT.dbo.CHITIETTHUE, {serverName}.QLKS_PT.dbo.CHITIETTHUEPHONG 
                WHERE KHACHHANG.maKH = CHITIETTHUE.maKH 
                  AND CHITIETTHUE.maCTT = CHITIETTHUEPHONG.maCTT 
                  AND CHITIETTHUE.tinhTrangXuLy = 0 
                  AND maP = '{maP}' 
                  AND CHITIETTHUEPHONG.tinhTrang = 1 
                ORDER BY ngayThue ASC";
            return db.getList(query);
        }
    }
}
