using System;
using System.Collections.Generic;
using System.Text;
using ESPlus.Rapid;
using ESBasic;
using TalkBase.Server;
using TalkBase;

namespace EasyIM.Server
{
    /// <summary>
    ///  服务端用于提供Remoting服务的接口。功能有：
    /// （1）注册用户、查找用户。
    /// （2）获取最新版本号。
    /// （3）获取聊天记录。
    /// </summary>
    internal class IMService :MarshalByRefObject, IIMService
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private ResourceCenter<IMUser, IMGroup> resourceCenter;        
        public IMService(ResourceCenter<IMUser, IMGroup> center)
        {
            this.resourceCenter = center;           
        }        

        public List<IMUser> SearchUser(string idOrName)
        {
            return this.resourceCenter.ServerGlobalCache.SearchUser(idOrName);
        }

        public RegisterResult Register(IMUser user, bool checkLuckNumber = true)
        {
            try
            {
                if (checkLuckNumber && GlobalFunction.IsLuckNumber_UserID(user.ID))
                {
                    return RegisterResult.IdIsLuckNumber;
                }
                if (this.resourceCenter.ServerGlobalCache.IsUserExist(user.UserID))
                {
                    return RegisterResult.Existed;
                }
                if (!string.IsNullOrEmpty(user.Phone))
                {
                    IMUser theUser = this.resourceCenter.ServerGlobalCache.GetUser4Phone(user.Phone);
                    if (theUser != null)
                    {
                        return RegisterResult.PhoneExisted;
                    }
                }
                this.resourceCenter.ServerGlobalCache.InsertUser(user);
                if (GlobalOptions.IsAddDefaultFirend4Register && this.resourceCenter.ServerGlobalCache.IsUserExist(GlobalOptions.DefaultID))
                {
                    // this.resourceCenter.ServerGlobalCache.AddFriend(GlobalOptions.DefaultID, user.ID, FunctionOptions.DefaultFriendCatalog);
                    this.resourceCenter.GlobalService.RequestAddFirend(user.ID, GlobalOptions.DefaultID, "", FunctionOptions.DefaultFriendCatalog);
                }
                return RegisterResult.Succeed;
            }
            catch (Exception ee)
            {
                this.resourceCenter.Logger.Log(ee, "IMService.Register - " + user.ToString(), ESBasic.Loggers.ErrorLevel.Standard);
                return RegisterResult.Error;
            }
        }

        public ChatRecordPage GetChatRecordPage(ChatRecordTimeScope timeScope, string senderID, string accepterID, int pageSize, int pageIndex)
        {
            return this.resourceCenter.ServerGlobalCache.GetChatRecordPage(timeScope, senderID, accepterID, pageSize, pageIndex);
        }

        public ChatRecordPage GetGroupChatRecordPage(ChatRecordTimeScope timeScope, string groupID, int pageSize, int pageIndex)
        {
            return this.resourceCenter.ServerGlobalCache.GetGroupChatRecordPage(timeScope, groupID, pageSize, pageIndex);
        }

        public int GetVersion()
        {
            return GlobalConsts.TalkVersion;
        }

        public ChatMessageRecord GetChatMessageRecord(int id)
        {
            return this.resourceCenter.ServerGlobalCache.GetChatMessageRecord(id);
        }
    }
}
