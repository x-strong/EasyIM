using ESBasic;
using EasyIM.Controls;
using EasyIM.FlatControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkBase.Client;
using TalkBase.Client.Application;
using TalkBase.Client.Bridges;

namespace EasyIM
{
    public class FlatController : SeperateChatFormController<IMUser, IMGroup>
    {

        public FlatController(ResourceCenter<IMUser, IMGroup> center, IChatFormShower formShower) : base(center, formShower)
        {

        }

        protected override IChatForm NewFriendChatForm(string friendID)
        {
            return new FriendChatPanel(base.resourceCenter, friendID);
        }

        protected override IChatForm NewGroupChatForm(string groupID)
        {
            GroupChatPanel groupChatForm = new GroupChatPanel(this.resourceCenter, groupID);
            groupChatForm.GroupMemberClicked += new CbGeneric<string>(groupChatForm_GroupMemberClicked);
            return groupChatForm;
        }

        void groupChatForm_GroupMemberClicked(string unitID)
        {
            this.FocusOnForm(unitID, true);
        }

        protected override IChatForm NewFileAssistantForm()
        {
            FileAssistantPanel form = new FileAssistantPanel(this.resourceCenter);
            return form;
        }

        protected override Form NewNotifyForm()
        {
            NotifyForm form = new NotifyForm(this.resourceCenter);
            return form;
        }

        protected override Form NewControlForm()
        {
            ControlMainForm form = new ControlMainForm(this.resourceCenter);
            return form;
        }

        protected override Form NewAddFriendForm(string unitID)
        {
            AddFriendForm addFriendForm = new AddFriendForm(this.resourceCenter, unitID);
            return addFriendForm;
        }

        protected override Form NewGroupVideoCallForm(string videoGroupID, string requestorID, List<string> memberIDList)
        {
            IMUser user = this.resourceCenter.ClientGlobalCache.GetUser(requestorID);
            return new GroupVideoCallForm(this.resourceCenter, videoGroupID, user, memberIDList);
        }

        protected override Form NewGroupVideoChatForm(string videoGroupID)
        {
            return new GroupVideoChatForm(this.resourceCenter, videoGroupID);
        }
    }
}
