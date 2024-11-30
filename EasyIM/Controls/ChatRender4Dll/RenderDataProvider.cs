﻿using ESFramework.Extensions.ChatRendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using TalkBase;

namespace EasyIM.Controls.ChatRender4Dll
{
    internal class RenderDataProvider : IRenderDataProvider
    {
        private static string empty = string.Empty;

        public Dictionary<int, object> GetEmotions()
        {
            return RenderResource.GetSingleton.Emotions;
        }

        public string GetFilePathToSave(string fileName)
        {
            return empty;
        }

        public object GetImageOfAudioCall()
        {
            return null;
        }

        public object GetImageOfFileType(string fileExtendName)
        {
            return null;
        }

        public object GetImageOfSendFailed()
        {
            return null;
        }

        public object GetImageOfVideoCall()
        {
            return null;
        }

        public string GetSpeakerDisplayName(string speakerID)
        {
            if (speakerID.StartsWith("#"))
            {
                return Program.ResourceCenter.ClientGlobalCache.CurrentUser.DisplayName;
            }
            string displayName = speakerID;
            IUnit unit = Program.ResourceCenter.ClientGlobalCache.GetUnit(speakerID);
            if (unit != null)
            {
                displayName = unit.DisplayName;
            }
            return displayName;
        }

        public object GetUserHeadImage(string speakerID)
        {
            if (speakerID.StartsWith("#"))
            {
                speakerID = Program.ResourceCenter.CurrentUserID;
            }
            IMUser user = Program.ResourceCenter.ClientGlobalCache.GetUser(speakerID);
            if (user == null) return null;
            Image image = GlobalResourceManager.GetHeadImage(user);
            return image;
        }

        public string GetUserName(string speakerID)
        {
            if (speakerID.StartsWith("#"))
            {
                return Program.ResourceCenter.ClientGlobalCache.CurrentUser.DisplayName;
            }
            string displayName = speakerID;
            IUnit unit = Program.ResourceCenter.ClientGlobalCache.GetUnit(speakerID);
            if (unit != null)
            {
                displayName = unit.DisplayName; 
            }
            return displayName;
        }
    }
}