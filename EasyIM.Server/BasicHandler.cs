using ESBasic.Security;
using ESFramework;
using ESPlus.Application.Basic.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using TalkBase;
using TalkBase.Server;

namespace EasyIM.Server
{
    /// <summary>
    /// ������������������֤��½���û���
    /// </summary>
    internal class BasicHandler : IBasicHandler
    {
        private IDBPersister<IMUser, IMGroup> dbPersister;
        private ResourceCenter<IMUser, IMGroup> resourceCenter;
        private IMService service;
        private List<string> adminUserList;
        public BasicHandler(IDBPersister<IMUser, IMGroup> persister , ResourceCenter<IMUser, IMGroup> center)
        {
            this.dbPersister = persister;
            this.resourceCenter = center;
            this.service = new IMService(center);
            this.adminUserList = new List<string>(ConfigurationManager.AppSettings["AdminUserIDList"].Split(','));
        }

        /// <summary>
        /// �˴���֤�û����˺ź����롣����true��ʾͨ����֤��
        /// </summary>  
        public bool VerifyUser(ClientType clientType, string systemToken, string userID, string password, out string failureCause)
        {
            if (string.IsNullOrEmpty(password))
            {
                failureCause = "�������";
                return false;
            }
            if (password.StartsWith(FunctionOptions.RegistActionToken)) //��ʾ��ע��
            {
                //password����ʾ���� #Reg:name;pwd;phone;smsCode;#12(orgID);3(headIndex);���ͣ�(signature)
                failureCause = this.Register(password ,false);
                return false;
            }

            if (password.StartsWith(FunctionOptions.RegistActionToken2)) //��ʾ��ע��
            {
                //password����ʾ���� #Reg:name;pwd;phone;smsCode;#12(orgID);3(headIndex);���ͣ�(signature);10000(userID)
                failureCause = this.Register(password ,true);
                return false;
            }

            if (password.StartsWith(FunctionOptions.ResetPasswordActionToken))
            {
                //password����ʾ���� #ResetPassword:13678945632;123456(newPwdMD5);058941(smsCode)

                failureCause = this.ResetPassword(password);
                return false;
            }
            failureCause = "";
            IMUser user = this.dbPersister.GetUser(userID);
            // string pwd = this.dbPersister.GetUserPassword(userID);
            if (user == null)
            {
                failureCause = "�û������ڣ�";
                return false;
            }
            if (user.UserState == UserState.StopUsing)
            {
                failureCause = "�û���ͣ�ã�";
                return false;
            }
            if (user.UserState == UserState.Frozen)
            {
                failureCause = "�û��Ѷ��ᣡ";
                return false;
            }
            if (user.PasswordMD5!= password)
            {
                failureCause = "�������";
                return false;
            }
            failureCause = this.adminUserList.Contains(user.ID).ToString();
            return true;
        }



        private string Register(string content ,bool containsUserID)
        {
            try
            {
                //content����ʾ���� #Reg:name;pwd;phone;smsCode;#12(orgID);3(headIndex);���ͣ�(signature)
                int startIndex = containsUserID ? FunctionOptions.RegistActionToken2.Length : FunctionOptions.RegistActionToken.Length;
                string[] parts = content.Substring(startIndex).Split(';');
                string name = parts[0];
                string pwd = parts[1];
                string phone = parts[2];
                string smsCode = parts[3];
                string orgID = parts[4];
                int headIndex = int.Parse(parts[5]);
                string signature = parts[6];

                string userID = null;
                if (containsUserID)
                {
                    userID = parts[7];
                }
                else
                {
                    do
                    {
                        userID = GlobalFunction.CreateRandomStr(true, 8);
                    } while (GlobalFunction.IsLuckNumber_UserID(userID) || userID.StartsWith("0"));
                }
                IMUser user = new IMUser(userID, SecurityHelper.MD5String2(pwd), name, "", orgID, signature, headIndex, "");
                user.Phone = phone??string.Empty;
                RegisterResult res = this.service.Register(user, false);
                if (res == RegisterResult.Succeed)
                {
                    return res.ToString() + userID;//ע��ɹ��� userid���ظ��ͻ���
                }
                return res.ToString(); //Succeed ,Existed ,Error,IDIsLuckNumber,SmsCodeError,SmsCodeExpired
            }
            catch (Exception ee)
            {
                return "Error: ����ע���ַ�������" + ee.Message;
            }
        }




        private string ResetPassword(string content)
        {
            try
            {
                string[] parts = content.Substring(FunctionOptions.ResetPasswordActionToken.Length).Split(';');
                string phone = parts[0];
                string newPwd = parts[1];
                string smsCode = parts[2];
                int resetPasswordResult = (int)this.ResetPassword(phone, newPwd, smsCode);
                return  resetPasswordResult.ToString();
            }
            catch (Exception ee)
            {
                return "Error: ����ע���ַ�������" + ee.Message;
            }
        }

        private ResetPasswordResult ResetPassword(string phone, string newPassword, string smsCode)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                {
                    return ResetPasswordResult.Error;
                }

                IMUser user = this.resourceCenter.ServerGlobalCache.GetUser4Phone(phone);
                if (user == null)
                {
                    return ResetPasswordResult.UserNotExist;
                }
                this.resourceCenter.ServerGlobalCache.ChangePassword(user.UserID, newPassword);
                return ResetPasswordResult.Succeed;
            }
            catch (Exception)
            {
                return ResetPasswordResult.Error;
            }

        }

        public string HandleQueryBeforeLogin(AgileIPE clientAddr, int queryType, string query)
        {
            return string.Empty;
        }
    }
   
}
