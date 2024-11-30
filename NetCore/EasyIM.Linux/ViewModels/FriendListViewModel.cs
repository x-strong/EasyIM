using CPF;
using EasyIM.Linux.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyIM.Linux.ViewModels
{
    internal class FriendListViewModel : CPF.CpfObject
    {
        public Collection<FriendListModel> FriendListModelList
        {
            get { return (Collection<FriendListModel>)GetValue(); }
            set { SetValue(value); }
        }
    }

    internal class FriendListModel : CpfObject
    {
        public string CatalogName
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public Collection<IMUserPlus> UserList
        {
            get { return (Collection<IMUserPlus>)GetValue(); }
            set { SetValue(value); }
        }
    }
}
