﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EasyIM
{
    /// <summary>
    /// 编辑好友备注名称。
    /// </summary>
    public partial class EditCommentNameForm : BaseForm
    {
        private bool isNew = true;
        private string oldName;
        public EditCommentNameForm(string _oldName)
        {
            InitializeComponent();
            this.isNew = false;
            this.oldName = _oldName;
            this.skinTextBox1.SkinTxt.Text = oldName;
            this.skinTextBox1.Focus();
        }

        public EditCommentNameForm()
            : this("")
        {
        }

        private string newName;
        public string NewName
        {
            get
            {
                return this.newName;
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void skinButton2_Click(object sender, EventArgs e)
        {
            this.newName = this.skinTextBox1.SkinTxt.Text.Trim();
            
            if (this.newName == this.oldName)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                return;
            }           

            if (this.newName.Contains(":") || this.newName.Contains(";"))
            {
                MessageBox.Show("名称中不能包含特殊字符！");
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
