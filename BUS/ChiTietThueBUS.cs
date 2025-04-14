using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class ChiTietThueBUS
    {
        Database db;
        public ChiTietThueBUS()
        {
            db = new Database();
        }

        // Lấy danh sách ChiTietThue với điều kiện xuLy = 0 trên server xác định
        public List<ChiTietThueDTO> getDSChiTietThue(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.CHITIETTHUE WHERE xuLy = 0";
            return db.getListCTT_DTO(query);
        }

        // Lấy số lượng (count) của ChiTietThue trong ngày dateNow trên server xác định
        public int GetCountAll(string serverName, string dateNow)
        {
            string query = $"SELECT COUNT(MaCTT) + 1 FROM {serverName}.QLKS_PT.dbo.CHITIETTHUE WHERE CAST(ngayLapPhieu AS date) = '{dateNow}'";
            return db.ExecuteNonQuery_getInteger(query);
        }

        // Insert mới một ChiTietThue vào server xác định
        public void InsertCTT(string serverName, string maCTT, string maKH, string maNV, string ngayLapPhieu, string tienDatCoc)
        {
            string query = string.Format("INSERT INTO {0}.QLKS_PT.dbo.CHITIETTHUE VALUES('{1}','{2}','{3}','{4}',{5},0,0)",
                                         serverName, maCTT, maKH, maNV, ngayLapPhieu, tienDatCoc);
            db.ExecuteNonQuery(query);
        }

        // Lấy thông tin ChiTietThue theo maCTT từ server xác định
        public ChiTietThueDTO getChiTietThue(string serverName, string mactt)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.CHITIETTHUE WHERE maCTT = '{mactt}'";
            return db.getCTT_DTO(query);
        }

        // Xóa (đánh dấu xuLy = 1) ChiTietThue theo maCTT trên server xác định
        public void DeleteCTT(string serverName, string maCTT)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.CHITIETTHUE SET xuLy = 1 WHERE maCTT = '{maCTT}'";
            db.ExecuteNonQuery(query);
        }

        // Cập nhật tình trạng xử lý (tinhTrangXuLy = 1) cho ChiTietThue theo maCTT trên server xác định
        public void SuaTinhTrangXuLy(string serverName, string maCTT)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.CHITIETTHUE SET tinhTrangXuLy = 1 WHERE maCTT = '{maCTT}'";
            db.ExecuteNonQuery(query);
        }
    }
}
