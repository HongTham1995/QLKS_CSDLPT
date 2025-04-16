using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class TaiKhoanBUS
    {
        private Database db;

        public TaiKhoanBUS()
        {
            db = new Database();
        }

        public List<TaiKhoanDTO> GetListTaiKhoan(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.TAIKHOAN";
            return db.GetListTK_DTO(query);
        }

        public List<TaiKhoanDTO> getTAIKHOAN_3CN()
        {
            string query = @"
                SELECT * FROM Server_HANOI.QLKS_PT.dbo.TAIKHOAN WHERE XuLy = 0
                UNION ALL 
                SELECT * FROM Server_HUE.QLKS_PT.dbo.TAIKHOAN WHERE XuLy = 0
                UNION ALL 
                SELECT * FROM Server_SAIGON.QLKS_PT.dbo.TAIKHOAN WHERE XuLy = 0";
            return db.GetListTK_DTO(query);
        }

        public TaiKhoanDTO GetTK(string serverName, string taiKhoan)
        {
            foreach (TaiKhoanDTO x in GetListTaiKhoan(serverName))
            {
                if (x.TaiKhoan.Equals(taiKhoan))
                    return x;
            }
            return new TaiKhoanDTO();
        }

        public TaiKhoanDTO GetTKNV(string serverName, string maNV)
        {
            foreach (TaiKhoanDTO x in GetListTaiKhoan(serverName))
            {
                if (x.MaNV.Equals(maNV))
                    return x;
            }
            return new TaiKhoanDTO();
        }

        public void ThemTaiKhoan(string serverName, string taiKhoan, string maNV, string maPQ, string matKhau, string tinhTrang)
        {
            string rowGuid = Guid.NewGuid().ToString();
            string query = string.Format(
                "INSERT INTO {0}.QLKS_PT.dbo.TAIKHOAN VALUES ('{1}', '{2}', '{3}', '{4}', {5}, 0,'{6}')",
                serverName, taiKhoan, maNV, matKhau, tinhTrang, maPQ,rowGuid);
            db.ExecuteNonQuery(query);
        }

        public void XoaTaiKhoan(string serverName, string taiKhoan)
        {
            string query = $"DELETE FROM {serverName}.QLKS_PT.dbo.TAIKHOAN WHERE taiKhoan = '{taiKhoan}'";
            db.ExecuteNonQuery(query);
        }

        public void SuaPhanQuyen(string serverName, string taiKhoan, string phanQuyen)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.TAIKHOAN SET maPQ = '{phanQuyen}' WHERE taiKhoan = '{taiKhoan}'";
            db.ExecuteNonQuery(query);
        }

        public void KhoaTaiKhoan(string serverName, string taiKhoan)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.TAIKHOAN SET tinhTrang = 1 WHERE taiKhoan = '{taiKhoan}'";
            db.ExecuteNonQuery(query);
        }

        public void MoKhoaTaiKhoan(string serverName, string taiKhoan)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.TAIKHOAN SET tinhTrang = 0 WHERE taiKhoan = '{taiKhoan}'";
            db.ExecuteNonQuery(query);
        }

        public void SuaMatKhau(string serverName, string taiKhoan, string matKhau)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.TAIKHOAN SET matKhau = '{matKhau}' WHERE taiKhoan = '{taiKhoan}'";
            db.ExecuteNonQuery(query);
        }
    }
}
