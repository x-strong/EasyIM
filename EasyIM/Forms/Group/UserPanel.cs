using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ESBasic;

namespace EasyIM.Forms.Group
{
    public partial class UserPanel : UserControl
    {
        public UserPanel()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.ResizeRedraw, true);//调整大小时重绘
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);// 双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);// 禁止擦除背景.
            this.SetStyle(ControlStyles.UserPaint, true);//自行绘制            
            this.UpdateStyles();
        }

        public event CbGeneric<UserPanel> Closed;


        private IMUser user;
        public IMUser User
        {
            get { return user; }
            set 
            { 
                user = value;              
                this.skinLabel_Name.Text = string.Format("{0}({1})", user.DisplayName, user.ID);
                this.pictureBox1.BackgroundImage = GlobalResourceManager.GetHeadImageOnline(user);
                this.pictureBox2.BackgroundImage = null;
            }
        }

        private void skinPictureBox2_Click(object sender, EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this);
            }
        }       

        private void skinPictureBox2_MouseEnter(object sender, EventArgs e)
        {
            this.pictureBox2.BackgroundImage = global::EasyIM.Properties.Resources.delete_btn_pre;
            this.BackColor = Color.WhiteSmoke;
        }

        private void skinPictureBox2_MouseLeave(object sender, EventArgs e)
        {
            this.pictureBox2.BackgroundImage = global::EasyIM.Properties.Resources.delete_btn_nor;
        }

        private void UserPanel_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.WhiteSmoke;
            this.pictureBox2.BackgroundImage = global::EasyIM.Properties.Resources.delete_btn_nor;
        }

        private void UserPanel_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
            this.pictureBox2.BackgroundImage = null;
        }

        
    }
}
