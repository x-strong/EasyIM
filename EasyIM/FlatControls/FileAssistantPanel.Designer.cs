﻿
namespace EasyIM.FlatControls
{
    partial class FileAssistantPanel
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.chatPanelPlus1 = new EasyIM.ChatPanelPlus();
            this.SuspendLayout();
            // 
            // chatPanelPlus1
            // 
            this.chatPanelPlus1.BackColor = System.Drawing.Color.White;
            this.chatPanelPlus1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPanelPlus1.Location = new System.Drawing.Point(0, 0);
            this.chatPanelPlus1.Name = "chatPanelPlus1";
            this.chatPanelPlus1.Size = new System.Drawing.Size(633, 613);
            this.chatPanelPlus1.TabIndex = 0;
            // 
            // FileAssistantPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chatPanelPlus1);
            this.Name = "FileAssistantPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private ChatPanelPlus chatPanelPlus1;
    }
}