using BUS;
using DAO;
using DTO;
using Firebase.Database.Offline;
using GUI.GUI_COMPONENT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmLogin : Form
    {
        TaiKhoanBUS taiKhoan = new TaiKhoanBUS();
        NhanVienBUS nhanVien = new NhanVienBUS();
        
        public frmLogin()
        {
            InitializeComponent();
            timerOpacity.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void pnTop_MouseDown(object sender, MouseEventArgs e)
        {
            int mouseX = e.X;
            int mouseY = e.Y;
            mouseOffset = new Point(-mouseX, -mouseY);
        }

        private void pnTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                this.Location = mousePos;
            }
        }
        private const int CS_DropShadow = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DropShadow;
                return cp;
            }
        }
        private Point mouseOffset;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            
            taiKhoan = new TaiKhoanBUS();
            nhanVien = new NhanVienBUS();
            this.ActiveControl = null;
            var item = from taikhoan in taiKhoan.getTAIKHOAN_3CN()
                       join nhanvien in nhanVien.getNhanVien_3CN() on taikhoan.MaNV equals nhanvien.MaNV
                       select new
                       {
                           taiKhoan = taikhoan.TaiKhoan,
                           matKhau = taikhoan.MatKhau,
                           tinhTrang = taikhoan.TinhTrang,
                           maNV = nhanvien.MaNV,
                           tenNV = nhanvien.TenNV,
                           gioiTinh = nhanvien.GioiTinh,
                           xuly = nhanvien.XuLy
                       };
            if (txtTK.Text.Length == 0)
            {
                MessageBoxDialog message = new MessageBoxDialog();
                message.ShowDialog("Thông báo", "Thông báo", "Vui lòng nhập tài khoản", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                txtTK.Focus();
                return;
            }
            if (txtPass.Text.Length == 0)
            {
                MessageBoxDialog message = new MessageBoxDialog();
                message.ShowDialog("Thông báo", "Thông báo", "Vui lòng nhập mật khẩu", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                txtPass.Focus();
                return;
            }
            bool check = false;
            foreach (var it in item)
            {
                if (it.taiKhoan.Equals(txtTK.Text))
                {
                    check = true;
                    if (it.tinhTrang == 1)
                    {
                        MessageBoxDialog message = new MessageBoxDialog();
                        message.ShowDialog("Thông báo", "Thông báo", "Tài khoản đã bị khóa vui lòng liên hệ admin", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                        txtTK.Focus();
                        return;
                    }
                    else
                    {
                        if (it.xuly == 1)
                        {
                            MessageBoxDialog message = new MessageBoxDialog();
                            message.ShowDialog("Thông báo", "Thông báo", "Nhân viên sử dụng tài khoản này đã nghỉ việc", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                            txtTK.Focus();
                            return;
                        }
                        else
                        {
                            string pass = GetHash(txtPass.Text);
                            if (pass.Equals(it.matKhau))
                            {
                                this.Hide();
                                
                                Program.nhanVien = nhanVien.GetNV(it.maNV);
                                var maCN = Program.nhanVien.MaCN;
                                
                                switch (maCN)
                                {
                                    case "CN_1":
                                        Program.chinhanh = "https://www.google.com/maps/place/Ho%C3%A0n+Ki%E1%BA%BFm+Lake/@21.0287797,105.8497898,17z/data=!3m1!4b1!4m6!3m5!1s0x3135ab953357c995:0x1babf6bb4f9a20e!8m2!3d21.0286669!4d105.8521484!16zL20vMGdwNjV3?entry=ttu&g_ep=EgoyMDI1MDQyOS4wIKXMDSoASAFQAw%3D%3D"; 
                                        break;
                                    case "CN_2":
                                        Program.chinhanh = "https://www.google.com/maps/place/Hue+Museum+of+Royal+Antiquities/@15.9399159,107.3273326,9.29z/data=!4m6!3m5!1s0x3141a12e238f97ed:0x2785f852d78ff1c!8m2!3d16.471343!4d107.58208!16s%2Fm%2F03hnkbw?entry=ttu&g_ep=EgoyMDI1MDQyOS4wIKXMDSoASAFQAw%3D%3D";
                                        break;
                                    default:
                                        Program.chinhanh = "https://www.google.com/maps/place/Saigon+University/@10.7599224,106.6796834,17z/data=!3m1!4b1!4m6!3m5!1s0x31752f1b7c3ed289:0xa06651894598e488!8m2!3d10.7599171!4d106.6822583!16s%2Fm%2F02qvnkv?entry=ttu&g_ep=EgoyMDI1MDQyOS4wIKXMDSoASAFQAw%3D%3D";
                                        break;
                                }
                                Program.ServerName=nhanVien.GetServerNameFromMaCN(maCN);
                                QLKS_Form qlks = new QLKS_Form();
                                if (Program.nhanVien.GioiTinh == 1)
                                {
                                    qlks.pictureBox1.Image = global::GUI.Properties.Resources.icons8_avatar_48__1_;
                                } else
                                {
                                    qlks.pictureBox1.Image = global::GUI.Properties.Resources.icons8_avatar_48;
                                }
                                qlks.lbName.Text = "Xin chào, Nhân viên " + Program.nhanVien.TenNV;
                                qlks.ShowDialog();
                                this.Close();
                            }
                            else
                            {
                                MessageBoxDialog message = new MessageBoxDialog();
                                message.ShowDialog("Thông báo", "Thông báo", "Sai mật khẩu", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                                txtTK.Focus();
                                return;
                            }
                        }
                    }
                }
                if (check)
                {
                    break;
                }
            }
            if (!check)
            {
                MessageBoxDialog message = new MessageBoxDialog();
                message.ShowDialog("Thông báo", "Thông báo", "Tài khoản không tồn tại", MessageBoxDialog.ERROR, MessageBoxDialog.YES, "Đóng", "", "");
                txtTK.Focus();
                return;
            }
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void buttonRounded1_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmChangePassword frmChange = new frmChangePassword();
            frmChange.ShowDialog();
            this.Close();
        }

        private void buttonRounded2_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmForgotPassword frmChange = new frmForgotPassword();
            frmChange.ShowDialog();
            this.Close();
        }

        private void timerOpacity_Tick(object sender, EventArgs e)
        {
            if (Opacity >= 1)
            {
                timerOpacity.Stop();
            }
            else
            {
                Opacity += 0.03;
            }
            if (p.Y <= 278)
            {
                Location = p;
                p.Y += 2;
            }
        }
        Point p;
        private void frmLogin_Load(object sender, EventArgs e)
        {
            p = new Point(Location.X, 150);
        }
        public string GetHash(string matKhau)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(matKhau));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        bool check = true;
        private void buttonRounded3_Click(object sender, EventArgs e)
        {
            if (check)
            {
                txtPass.UseSystemPasswordChar = false;
                check = false;
            }
            else
            {
                txtPass.UseSystemPasswordChar = true;
                check = true;
            }
        }
        /*
        private void comboCN_SelectedValueChanged(object sender, EventArgs e)
        {
            
            if (comboCN.SelectedIndex == 0) {
                Database.server = 4;
            }else if (comboCN.SelectedIndex == 1) { Database.server = 1; }
            else if (comboCN.SelectedIndex == 2) { Database.server = 2; }
            else 
            {
                Database.server = 3;
            }
        }
        */
        
    }
}
