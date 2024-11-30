using ESBasic;
using System.Windows.Forms;
using TalkBase;
using TalkBase.Client;

namespace EasyIM
{
    public partial class ControlMainForm : BaseForm
    {
        private ResourceCenter<IMUser, IMGroup> resourceCenter;

        public ControlMainForm(ResourceCenter<IMUser, IMGroup> center)
        {
            InitializeComponent();
            
            this.resourceCenter = center;
            this.addUserBox1.Initialize(this.resourceCenter.RapidPassiveEngine);
            this.updateUserStateBox1.Initialize(this.resourceCenter);
        }


    }
}
