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

namespace EasyIM.Linux.Controls
{
    [CPF.Design.DesignerLoadStyle("res://EasyIM.Linux.Controls/Stylesheet1.css")]//用于设计的时候加载样式
    internal class UserInGroupBox : Control
    {
        private TreeView treeView;
        private ContextMenu contextMenu_User;
        private Window parentWindow;
        private IMGroup currentGroup;
        private string catalogName = "群成员";
        private object locker = new object();
        private Collection<FriendListModel> friendListModelList = new Collection<FriendListModel>();
        private FriendListModel friendListModel;

        public UserInGroupBox()
        {
            this.friendListModel = new FriendListModel() { CatalogName = this.catalogName, UserList = new Collection<IMUserPlus>() };
        }


        private bool initialized = false;
        protected override void InitializeComponent()
        {//模板定义
            if (this.initialized) { return; }
            this.initialized = true;
            DataContext = null;
            Size = SizeField.Fill;
            Children.Add(new Panel
            {
                Size = SizeField.Fill,
                Children =
                {
                    new TreeView
                    {
                        Name=nameof(this.treeView),
                        PresenterFor=this,
                        Size=SizeField.Fill,
                        DisplayMemberPath=nameof(FriendListModel.CatalogName) ,
                        ItemsMemberPath=nameof(FriendListModel.UserList),

                        ItemTemplate=new CatalogItem
                        {
                            ItemTemplate= new UserInGroupItem{
                                    
                                    Commands ={
                                    {
                                        nameof(MouseDown),(s,e)=>{
                                            CPF.Input.MouseButtonEventArgs args = e as CPF.Input.MouseButtonEventArgs;
                                            if (args.MouseButton == CPF.Input.MouseButton.Right)
                                            {
                                                this.contextMenu_User.PlacementTarget = (UIElement)s;
                                                this.contextMenu_User.IsOpen = true;
                                                args.Handled = true;
                                            }
                                    } },
                                    {nameof(DoubleClick),(s,e)=>this.Friend_DoubleClick((UIElement)s) }
                                }
                            },
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

                this.contextMenu_User = new ContextMenu()
                {
                    //Items = new UIElement[] {
                    //             new MenuItem
                    //             {
                    //                Header = "发送消息",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => SendMessage_Click(s)
                    //                    }
                    //                }
                    //             },
                    //             new MenuItem
                    //             {
                    //                Header = "消息记录",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => MessageRecord_Click(s)
                    //                    }
                    //                }
                    //             },
                    //             new MenuItem
                    //             {
                    //                Header = "查看资料",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => UserInfo_Click(s)
                    //                    }
                    //                }
                    //             },
                    //             new MenuItem
                    //             {
                    //                Header = "修改备注名称",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => UpdateFirendRemark_Click(s)
                    //                    }
                    //                }
                    //             },
                    //             new MenuItem
                    //             {
                    //                Header = "移动联系人至",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => MoveToCatalog_Click(s)
                    //                    }
                    //                }
                    //             },
                    //             new MenuItem
                    //             {
                    //                Header = "删除好友",
                    //                Commands = {
                    //                    {
                    //                        nameof(MouseDown),
                    //                        (s, e) => DeleteFirend_Click(s)
                    //                    }
                    //                }
                    //             },
                    //            }

                };


            }
        }

        public void Initialize(IMGroup group, Window parentWindow)
        {
            this.parentWindow = parentWindow;
            this.currentGroup = group;
            this.AddUser(new IMUserPlus(Program.ResourceCenter.ClientGlobalCache.CurrentUser));
            foreach (string friendID in this.currentGroup.MemberList)
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

        private void BindSource()
        {
            this.friendListModelList.Add(this.friendListModel);
            FriendListViewModel viewModel = new FriendListViewModel()
            {
                FriendListModelList = this.friendListModelList
            };
            DataContext = viewModel;
            this.treeView.ExpandFirstNode();
        }

        //private void AddCatalogName(string _catalogName)
        //{
        //    bool existCatalogName = this.friendListModelList.Exists(x => x.CatalogName == _catalogName);
        //    if (existCatalogName) { return; }
        //    this.friendListModelList.Add(new FriendListModel() { CatalogName = _catalogName, UserList = new Collection<IMUserPlus>() });
        //}

        public void AddUser(IMUserPlus user)
        {
            lock (this.locker)
            {
                Collection<IMUserPlus> list = this.friendListModel.UserList;
                //若集合中未存在该用户则添加
                if (list.Find(x => x.ID == user.ID) == null)
                {
                    list.Add(user);
                }
            }

        }

        public void DeleteUser(string userID)
        {
            lock (this.locker)
            {
                for (int i = 0; i < this.friendListModel.UserList.Count; i++)
                {
                    if (this.friendListModel.UserList[i].ID == userID)
                    {
                        this.friendListModel.UserList.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public void UpdateUser(IMUser user)
        {
            lock (this.locker)
            {
                for (int i = 0; i < this.friendListModel.UserList.Count; i++)
                {
                    if (this.friendListModel.UserList[i].ID == user.ID)
                    {
                        this.friendListModel.UserList[i] = new IMUserPlus(user);
                        return;
                    }
                }
            }
        }


        private void Friend_DoubleClick(UIElement s)
        {
            IMUserPlus userPlus = ((UIElement)s).DataContext as IMUserPlus;
            CommonHelper.MoveToChat(userPlus);
        }


        #region 右键菜单
        //点击添加好友
        private void AddFriend_Click(UIElement s)
        {
            IMUserPlus userPlus = ((UIElement)s).DataContext as IMUserPlus;
            if (userPlus == null) { return; }
            CommonBusinessMethod.AddFriend(this.parentWindow, Program.ResourceCenter, userPlus.ID);
        }
        #endregion
    }
}
