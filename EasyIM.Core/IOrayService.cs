using System;
using System.Collections.Generic;
using System.Text;
using ESBasic;
using EasyIM.Core;
using TalkBase;

namespace EasyIM
{
    /// <summary>
    /// 服务端用于提供Remoting服务的接口。功能有：
    /// （1）注册用户、查找用户。
    /// （2）获取最新版本号。
    /// （3）获取组织结构信息。
    /// （4）获取聊天记录。
    /// </summary>
    public interface IIMService : IChatRecordGetter
    {
        int GetVersion();
        RegisterResult Register(IMUser user,bool checkLuckNumber=false);
        List<IMUser> SearchUser(string idOrName);       
         
      
    }


}
