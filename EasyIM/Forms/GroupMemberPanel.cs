using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TalkBase;
using TalkBase.Client;

namespace EasyIM
{
    public partial class GroupMemberPanel : UserControl
    {
        private IUnit unit ;
        public event Action<IUnit> OnClicked;

        public GroupMemberPanel(IUnit unit)
        {
            InitializeComponent();
            this.unit = unit;
            if (unit.UnitType == UnitType.User)
            {
                this.skinPictureBox1.Image = GlobalResourceManager.GetHeadImageOnline((IMUser)unit);
            }
            else if (unit.UnitType == UnitType.Group)
            {
                this.skinPictureBox1.Image = EasyIM.Properties.Resources.Group2;
            }
            
            this.skinLabel_displayName.Text = unit.DisplayName;
        }

        private void GroupMemberPanel_Click(object sender, EventArgs e)
        {
            if (this.OnClicked != null)
            {
                this.OnClicked(this.unit);
            }
        }
    }
}
