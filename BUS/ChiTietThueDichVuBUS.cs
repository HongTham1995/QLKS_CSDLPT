using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BUS
{
    public class ChiTietThueDichVuBUS
    {
        Database db;
        public ChiTietThueDichVuBUS()
        {
            db = new Database();
        }

        // Lấy danh sách chi tiết thuê dịch vụ theo maCTT từ server xác định
        public List<ChiTietThueDichVuDTO> GetListChiTietDichVu(string serverName, string maCTT)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.CHITIETTHUEDICHVU WHERE maCTT = '{maCTT}'";
            return db.getListCTTDV_DTO(query);
        }

        // Xóa chi tiết thuê dịch vụ theo maCTT, maDV, và ngaySuDung trên server xác định
        public void DeleteCTTDV(string serverName, string maCTT, string maDV, string ngaySuDung)
        {
            string query = string.Format("DELETE FROM {0}.QLKS_PT.dbo.CHITIETTHUEDICHVU WHERE maCTT = '{1}' AND maDV = '{2}' AND ngaySuDung = '{3}'",
                                         serverName, maCTT, maDV, ngaySuDung);
            db.ExecuteNonQuery(query);
        }

        // Sửa số lượng của chi tiết thuê dịch vụ trên server xác định
        public void SuaSoLuongCTTDV(string serverName, string maCTT, string maDV, string ngaySuDung, string soLuong)
        {
            string query = string.Format("UPDATE {0}.QLKS_PT.dbo.CHITIETTHUEDICHVU SET soLuong = {1} WHERE maCTT = '{2}' AND maDV = '{3}' AND ngaySuDung = '{4}'",
                                         serverName, soLuong, maCTT, maDV, ngaySuDung);
            db.ExecuteNonQuery(query);
        }

        // Thêm mới chi tiết thuê dịch vụ vào server xác định
        public void ThemCTTDV(string serverName, string maCTT, string maDV, string ngaySuDung, string soLuong, string giaDV)
        {
            string rowGuid = Guid.NewGuid().ToString();
            string query = string.Format("INSERT INTO {0}.QLKS_PT.dbo.CHITIETTHUEDICHVU VALUES ('{1}','{2}','{3}',{4},{5},'{6}')",
                                         serverName, maCTT, maDV, ngaySuDung, soLuong, giaDV,rowGuid);
            db.ExecuteNonQuery(query);
        }

        // Cộng dồn số lượng (tăng thêm soLuong) của chi tiết thuê dịch vụ trên server xác định
        public void SuaSoLuong(string serverName, string maCTT, string maDV, string ngaySuDung, string soLuong)
        {
            string query = string.Format("UPDATE {0}.QLKS_PT.dbo.CHITIETTHUEDICHVU SET soLuong = soLuong + {1} WHERE maCTT = '{2}' AND maDV = '{3}' AND ngaySuDung = '{4}'",
                                         serverName, soLuong, maCTT, maDV, ngaySuDung);
            db.ExecuteNonQuery(query);
        }
    }
}
