using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using EasyIM;
using EasyIM.Linux.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyIM.Linux.ViewModels
{

    internal class GroupListViewModel : CPF.CpfObject
    {
        public Collection<GroupListModel> GroupListModelList
        {
            get { return (Collection<GroupListModel>)GetValue(); }
            set { SetValue(value); }
        }
    }

    internal class GroupListModel : CPF.CpfObject
    {
        public string CatalogName
        {
            get { return (string)GetValue(); }
            set { SetValue(value); }
        }
        public Collection<IMGroup> GroupList
        {
            get { return (Collection<IMGroup>)GetValue(); }
            set { SetValue(value); }
        }
    }

    internal class GroupDetailUserListModel : CPF.CpfObject
    {
        public Collection<IMUserPlus> UserList
        {
            get { return (Collection<IMUserPlus>)GetValue(); }
            set { SetValue(value); }
        }
    }
}
