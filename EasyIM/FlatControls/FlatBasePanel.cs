using EasyIM.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyIM.FlatControls
{
    public partial class FlatBasePanel : UserControl, IFlatControl
    {
        public FlatBasePanel()
        {
            InitializeComponent();
        }
        

        public void Close()
        {
            this.Dispose();
        }

        public virtual void ClickMore()
        {
            
        }

        public virtual string ControlTitle => string.Empty;

    }
}
