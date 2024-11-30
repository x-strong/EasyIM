using CPF;
using CPF.Animation;
using CPF.Controls;
using CPF.Drawing;
using CPF.Shapes;
using CPF.Styling;
using EasyIM.Linux.Models;
using EasyIM.Linux.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyIM.Linux.Controls.Templates
{
    internal class BaseUserTreeViewItem : TreeViewItem
    {
        protected override void InitializeComponent()
        {
            //模板定义
            if (DesignMode)
            {
                Width = 300;
                Background = "#fff";
            }
            else
            {
                Width = "100%";
            }
            Height = 32;

            Children.Add(new WrapPanel
            {
                Size = SizeField.Fill,
                Children =
                {
                    new Picture
                    {
                        MarginLeft = 3f,
                        Height = 28,
                        Width = 28,
                        Stretch = Stretch.Fill,
                        Bindings =
                        {
                            {
                                nameof(Picture.Source),
                                nameof(IMUserPlus.HeadImage)
                            }
                        }
                    },
                    new TextBlock
                    {
                        FontSize = 14,
                        MarginLeft = 4,
                        MarginTop = 7,
                        Text = "sdfsdsdfsdffsdf",
                        Bindings =
                        {
                            {
                                nameof(TextBlock.Text),
                                nameof(IMUserPlus.DisplayName)
                            }
                        }
                    },        
                    new TextBlock
                    {
                        FontSize = 14,
                        Foreground = "#7E7E7E",
                        MarginLeft = 10,
                        MarginTop = 7,
                        Text = "Text 2311",
                        Bindings =
                        {
                            {
                                nameof(TextBlock.Text),
                                nameof(IMUserPlus.Signature)
                            }
                        }
                    }               
                }
            });



            Triggers.Add(nameof(IsMouseOver), Relation.Me, null, (nameof(Background), "#aaaaaa55"));
            Triggers.Add(nameof(IsSelected), Relation.Me, null, (nameof(Background), "#aaaaaa55"));
        }
    }
}
