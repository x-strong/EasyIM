using System;
using System.Collections.Generic;
using System.Text;
using TalkBase.Server;


namespace EasyIM.Server
{
    /// <summary>
    /// 组管理器。
    /// </summary>
    internal class ContactsManager : ESPlus.Application.Contacts.Server.IContactsManager
    {
        private ServerGlobalCache<IMUser, IMGroup> globalCache;
        public ContactsManager(ServerGlobalCache<IMUser, IMGroup> db)
        {
            this.globalCache = db;
        }

        public List<string> GetGroupMemberList(string groupID)
        {
            IMGroup group =  this.globalCache.GetGroup(groupID);
            if (group == null)
            {
                return new List<string>();
            }

            return group.MemberList;
        }       

        public List<string> GetContacts(string userID)
        {
            return this.globalCache.GetAllContactsNecessary(userID); //上下线仅通知关注的联系人。
        }

        public void OnUserOffline(string userID)
        {
            
        }
    }
}
