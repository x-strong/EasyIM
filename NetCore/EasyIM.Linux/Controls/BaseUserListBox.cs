using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using EasyIM;
using EasyIM.Linux;
using EasyIM.Linux.Controls;
using EasyIM.Linux.Models;
using EasyIM.Linux.Controls.Templates;
using EasyIM.Linux.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using TalkBase;

namespace EasyIM.Linux.Controls
{
    internal class BaseUserListBox : Control
    {
        private TreeView treeView;
        private FriendListViewModel viewModel = new FriendListViewModel() { FriendListModelList = new Collection<FriendListModel>() };
        private object locker = new object();
        /// <summary>
        /// 成员双击事件
        /// </summary>
        public event Action<IMUserPlus> User_DoubleTapped;
        protected override void InitializeComponent()
        {//模板定义
            Children.Add(new Panel
            {
                Background = "#fff",
                Size = SizeField.Fill,
                Children =
                {
                    new TreeView
                    {
                        Size=SizeField.Fill,
                        Name = nameof(this.treeView),
                        PresenterFor = this,
                        DisplayMemberPath = nameof(FriendListModel.CatalogName),
                        ItemsMemberPath = nameof(FriendListModel.UserList),
                        ItemTemplate=new CatalogItem
                        {
                            ItemTemplate=new BaseUserTreeViewItem()
                            {
                                Commands={ { nameof(DoubleClick),(s,e)=> { this.TreeView_DoubleTapped(s); } } }
                            }
                        },
                        Bindings =
                        {
                            {
                                nameof(TreeView.Items),
                                nameof(FriendListViewModel.FriendListModelList)
                            }
                        }
                    }
                }
            });

            if (!this.DesignMode)
            {
                this.treeView = this.FindPresenterByName<TreeView>(nameof(this.treeView));
            }
        }

        private void TreeView_DoubleTapped(CpfObject s)
        {
            if (this.User_DoubleTapped != null)
            {
                IMUserPlus userPlus = s.DataContext as IMUserPlus;
                if (userPlus != null)
                {
                    this.User_DoubleTapped(userPlus);
                }
            }
        }


        public void Initialize(string _catalogName, List<IMUser> users)
        {
            this.viewModel.FriendListModelList.Clear();
            this.AddCatalogName(_catalogName);
            foreach (IMUser user in users)
            {
                this.AddUser(new IMUserPlus(user), _catalogName);
            }
            this.BindSource();
        }

        public void Initialize4Friend()
        {
            this.AddCatalogName(FunctionOptions.DefaultFriendCatalog);
            List<string> catalogList = new List<string>(Program.ResourceCenter.ClientGlobalCache.CurrentUser.GetFriendCatalogList());
            catalogList.Remove(FunctionOptions.BlackListCatalogName);
            foreach (string catalog in catalogList)
            {
                this.AddCatalogName(catalog);
            }
            //if (FunctionOptions.BlackList)
            //{
            //    this.AddCatalogName(FunctionOptions.BlackListCatalogName);
            //}
            this.AddUser(new IMUserPlus(Program.ResourceCenter.ClientGlobalCache.CurrentUser));

            foreach (string friendID in Program.ResourceCenter.ClientGlobalCache.CurrentUser.GetAllFriendList())
            {
                if (friendID == Program.ResourceCenter.ClientGlobalCache.CurrentUser.UserID)
                {
                    continue;
                }
                IMUser friend = Program.ResourceCenter.ClientGlobalCache.GetUser(friendID);
                if (friend != null)
                {
                    this.AddUser(new IMUserPlus(friend));
                }
            }
            this.BindSource();
        }


        private void AddCatalogName(string catalogName)
        {
            if (this.viewModel.FriendListModelList.Exists(x=>x.CatalogName==catalogName))
            {
                return;
            }
            lock (this.locker)
            {
                this.viewModel.FriendListModelList.Add(new FriendListModel() { CatalogName = catalogName, UserList = new Collection<IMUserPlus>() });
            }
        }

        private FriendListModel GetFriendListModel(string catalogName)
        {
            return this.viewModel.FriendListModelList.Find(x => x.CatalogName == catalogName);
        }

        public void AddUser(IMUserPlus user, string catalogName = null)
        {
            lock (this.locker)
            {
                if (catalogName == null)
                {
                    catalogName = Program.ResourceCenter.ClientGlobalCache.CurrentUser.GetFriendCatalog(user.ID) ?? FunctionOptions.DefaultFriendCatalog;
                }
                FriendListModel model = this.GetFriendListModel(catalogName);
                if (model == null) { return; }
                //若集合中未存在该用户则添加
                if (model.UserList.Find(x => x.ID == user.ID) == null)
                {
                    model.UserList.Add(user);
                }
            }
        }

        public void DeleteUser(string userID)
        {
            lock (this.locker)
            {
                Collection<FriendListModel> friendListModels = this.viewModel.FriendListModelList;
                foreach (FriendListModel model in friendListModels)
                {
                    foreach (IMUserPlus userPlus in model.UserList)
                    {
                        if (userPlus.ID == userID)
                        {
                            model.UserList.Remove(userPlus);
                            return;
                        }
                    }
                }
            }
        }

        public void UpdateUser(IMUser user)
        {
            lock (this.locker)
            {
                foreach (FriendListModel model in this.viewModel.FriendListModelList)
                {
                    for (int i = 0; i < model.UserList.Count; i++)
                    {
                        if (model.UserList[i].ID == user.ID)
                        {
                            model.UserList[i] = new IMUserPlus(user);
                            return;
                        }
                    }
                }
            }
        }

        private void BindSource()
        {
            DataContext = this.viewModel;
            this.treeView.ExpandFirstNode();
        }

    }
}
