using DAO;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;

namespace BUS
{
    public class NhanVienBUS
    {
        Database db;

        public NhanVienBUS()
        {
            db = new Database();
        }

        public DataTable getNhanVien(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.NHANVIEN WHERE XuLy = 0";
            return db.getList(query);
        }

        public int getNhanVienCount(string serverName)
        {
            string query = $"SELECT COUNT(*) FROM {serverName}.QLKS_PT.dbo.NHANVIEN";
            return db.ExecuteNonQuery_getInteger(query);
        }

        public void addNhanVien(string serverName, NhanVienDTO nv)
        {
            string ns = nv.NgaySinh.ToString("yyyy-MM-dd");
            string nvl = nv.NgayVaoLam.ToString("yyyy-MM-dd");

            string query = $@"INSERT INTO {serverName}.QLKS_PT.dbo.NHANVIEN 
            VALUES ('{nv.MaNV}', N'{nv.TenNV}', {nv.GioiTinh}, {nv.SoNgayPhep}, {nv.ChucVu}, 
                    '{ns}', '{nvl}', '{nv.Email}', {nv.Luong1Ngay}, 0, '{nv.MaCN}')";
            db.ExecuteNonQuery(query);
        }

        public DataTable findNhanVien(string serverName, string manv, string tennv, int gioitinh, int chucvu,
            string songayphep, string luong1ngay, DateTime ngaysinhtu, DateTime ngaysinhden,
            DateTime ngayvaolamtu, DateTime ngayvaolamden, string email)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.NHANVIEN WHERE ";

            if (!string.IsNullOrEmpty(manv))
                query += $"maNV LIKE '%{manv}%' AND ";
            if (!string.IsNullOrEmpty(tennv))
                query += $"tenNV LIKE N'%{tennv}%' AND ";
            if (gioitinh != -1)
                query += $"gioiTinh = {gioitinh} AND ";
            if (chucvu != -1)
                query += $"chucVu = {chucvu} AND ";
            if (!string.IsNullOrEmpty(songayphep))
            {
                if (songayphep.StartsWith("DƯỚI"))
                    query += $"soNgayPhep < {songayphep.Split(' ')[1]} AND ";
                else if (songayphep.StartsWith("TRÊN"))
                    query += $"soNgayPhep > {songayphep.Split(' ')[1]} AND ";
                else if (songayphep.StartsWith("TỪ"))
                    query += $"soNgayPhep >= {songayphep.Split(' ')[1]} AND soNgayPhep <= {songayphep.Split(' ')[4]} AND ";
            }
            if (ngaysinhtu != DateTime.MinValue)
                query += $"ngaySinh >= '{ngaysinhtu:yyyy-MM-dd}' AND ";
            if (ngaysinhden != DateTime.MinValue)
                query += $"ngaySinh <= '{ngaysinhden:yyyy-MM-dd}' AND ";
            if (ngayvaolamtu != DateTime.MinValue)
                query += $"ngayVaoLam >= '{ngayvaolamtu:yyyy-MM-dd}' AND ";
            if (ngayvaolamden != DateTime.MinValue)
                query += $"ngayVaoLam <= '{ngayvaolamden:yyyy-MM-dd}' AND ";
            if (!string.IsNullOrEmpty(luong1ngay))
            {
                if (luong1ngay.StartsWith("DƯỚI"))
                    query += $"luong1Ngay < {luong1ngay.Split(' ')[1]} AND ";
                else if (luong1ngay.StartsWith("TRÊN"))
                    query += $"luong1Ngay > {luong1ngay.Split(' ')[1]} AND ";
                else if (luong1ngay.StartsWith("TỪ"))
                    query += $"luong1Ngay >= {luong1ngay.Split(' ')[1]} AND luong1Ngay <= {luong1ngay.Split(' ')[4]} AND ";
            }
            if (!string.IsNullOrEmpty(email))
                query += $"email LIKE '%{email}%' AND ";

            query += "xuLy = 0";
            return db.getList(query);
        }

        public void deleteNhanVien(string serverName, string manv)
        {
            string query = $"UPDATE {serverName}.QLKS_PT.dbo.NHANVIEN SET xuLy = 1 WHERE maNV = '{manv}'";
            db.ExecuteNonQuery(query);
        }

        public string getCN(string serverName, string maNV)
        {
            string query = $@"SELECT TenCN FROM {serverName}.QLKS_PT.dbo.CHINHANH CN, 
                            {serverName}.QLKS_PT.dbo.NHANVIEN NV 
                            WHERE NV.MaNV = '{maNV}' AND CN.MaCN = NV.MaCN";
            return db.getCN(query);
        }

        public void chuyencongtac(NhanVienDTO nv, string chinhanhMoi)
        {
            string serverLinked, maCN;

            switch (chinhanhMoi)
            {
                case "Hà Nội":
                    maCN = "CN_1";
                    serverLinked = "[Server_HANOI]";
                    break;
                case "Huế":
                    maCN = "CN_2";
                    serverLinked = "[Server_HUE]";
                    break;
                default:
                    maCN = "CN_3";
                    serverLinked = "[Server_SAIGON]";
                    break;
            }

            nv.MaCN = maCN;

            string query = $@"
            INSERT INTO {serverLinked}.QLKS_PT.dbo.NHANVIEN 
            (maNV, tenNV, gioiTinh, soNgayPhep, chucVu, ngaySinh, ngayVaoLam, email, luong1Ngay,xuLy, MaCN) 
            VALUES 
            ('{nv.MaNV}', N'{nv.TenNV}', {nv.GioiTinh}, {nv.SoNgayPhep}, {nv.ChucVu}, 
            '{nv.NgaySinh:yyyy-MM-dd}', '{nv.NgayVaoLam:yyyy-MM-dd}', '{nv.Email}', {nv.Luong1Ngay}, 0, '{maCN}')";

            db.ExecuteNonQuery(query);
        }

        public void updateNhanVien(string serverName, NhanVienDTO nv)
        {
            string ns = nv.NgaySinh.ToString("yyyy-MM-dd");
            string nvl = nv.NgayVaoLam.ToString("yyyy-MM-dd");

            string query = $@"UPDATE {serverName}.QLKS_PT.dbo.NHANVIEN SET 
                                tenNV = N'{nv.TenNV}', 
                                gioiTinh = {nv.GioiTinh}, 
                                soNgayPhep = {nv.SoNgayPhep}, 
                                chucVu = {nv.ChucVu}, 
                                ngaySinh = '{ns}', 
                                ngayVaoLam = '{nvl}', 
                                email = '{nv.Email}', 
                                luong1Ngay = {nv.Luong1Ngay}
                              WHERE maNV = '{nv.MaNV}'";

            db.ExecuteNonQuery(query);
        }

        public List<NhanVienDTO> getNhanVien_3CN()
        {
            string query = @"
                SELECT * FROM Server_HANOI.QLKS_PT.dbo.NHANVIEN WHERE XuLy = 0
                UNION ALL 
                SELECT * FROM Server_HUE.QLKS_PT.dbo.NHANVIEN WHERE XuLy = 0
                UNION ALL 
                SELECT * FROM Server_SAIGON.QLKS_PT.dbo.NHANVIEN WHERE XuLy = 0";
            return db.getListNV_DTO(query);
        }
        #region new
        public List<NhanVienDTO> getDSNhanVien(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.NHANVIEN WHERE XuLy = 0";
            return db.getListNV_DTO(query);
        }

        public List<NhanVienDTO> getAllDSNhanVien(string serverName)
        {
            string query = $"SELECT * FROM {serverName}.QLKS_PT.dbo.NHANVIEN";
            return db.getListNV_DTO(query);
        }

        public NhanVienDTO GetNV( string maNV)
        {
            foreach (var nv in getNhanVien_3CN())
            {
                if (nv.MaNV == maNV)
                    return nv;
            }
            return null;
        }
        #endregion

        public string GetServerNameFromMaCN(string maCN)
        {
            switch (maCN)
            {
                case "CN_1":
                    return "Server_HANOI";
                case "CN_2":
                    return "Server_HUE";
                case "CN_3":
                    return "Server_SAIGON";
                default:
                    throw new ArgumentException("Mã chi nhánh không hợp lệ");
            }
        }

    }
}
