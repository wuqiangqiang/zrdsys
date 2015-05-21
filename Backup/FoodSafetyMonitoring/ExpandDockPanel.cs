using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace WelfareInstitution
{
    public class ExpandDockPanel : ItemsControl
    {
        public delegate void ClickEventHandle(ExpandDockItemData data, bool bExpand);
        public event ClickEventHandle ClickEvent;
        private List<ExpandDockItemData> datas = null;

        public ExpandDockPanel()
        {
            this.AddHandler(ExpandDockItem.ClickEvent, new RoutedEventHandler(ExpandDockItem_ClickItem));
        }

        void ExpandDockItem_ClickItem(object sender, RoutedEventArgs e)
        {
            if (e is ExpandDockItem.ItemClickEventArgs)
            {
                ExpandDockItem.ItemClickEventArgs args = (ExpandDockItem.ItemClickEventArgs)e;
                if (ClickEvent != null)
                {
                    ClickEvent(args.SourceData, args.bExpand);
                }
            }

        }


        public List<ExpandDockItemData> ItemSource
        {
            get
            {
                return datas;
            }
            set
            {
                datas = value;
                List<ExpandDockItemData> items = value;
                foreach (ExpandDockItemData kv in value)
                {
                    ExpandDockItem item = new ExpandDockItem(kv, this);
                    this.Items.Add(item);
                    kv.UI = item;
                }
            }
        }

        public void SetItemContext(string itemid, string content)
        {
            List<ExpandDockItem> items = (from kv in datas where kv.Areafnbr == itemid select (ExpandDockItem)(kv.UI)).ToList<ExpandDockItem>();
            if (items.Count > 0)
            {
                foreach (UIElement kv in items[0].Children)
                {
                    if (kv is Button)
                    {
                        Button btn = (Button)kv;
                        btn.Content = content;
                        break;
                    }
                }
            }
            else
            {
                ExpandDockItem item = null;
                foreach (ExpandDockItem kv in Items)
                {
                    if ((item = kv.FindChild(itemid)) != null)
                    {
                        foreach (UIElement k in item.Children)
                        {
                            if (k is Button)
                            {
                                Button btn = (Button)k;
                                btn.Content = content;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        public string GetItemContext(string itemid)
        {
            string ret = "";
            List<ExpandDockItem> items = (from kv in datas where kv.Areafnbr == itemid select (ExpandDockItem)(kv.UI)).ToList<ExpandDockItem>();
            if (items.Count > 0)
            {
                foreach (UIElement kv in items[0].Children)
                {
                    if (kv is Button)
                    {
                        Button btn = (Button)kv;
                        ret = btn.Content.ToString();
                        break;
                    }
                }
            }
            else
            {
                ExpandDockItem item = null;
                foreach (ExpandDockItem kv in Items)
                {
                    if ((item = kv.FindChild(itemid)) != null)
                    {
                        foreach (UIElement k in item.Children)
                        {
                            if (k is Button)
                            {
                                Button btn = (Button)k;
                                ret = btn.Content.ToString();
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return ret;
        }

        public void UpdateItemContext(string itemid, string context)
        {
            List<ExpandDockItem> items = (from kv in datas where kv.Areafnbr == itemid select (ExpandDockItem)(kv.UI)).ToList<ExpandDockItem>();
            if (items.Count > 0)
            {
                foreach (UIElement kv in items[0].Children)
                {
                    if (kv is Button)
                    {
                        Button btn = (Button)kv;
                        btn.Content = context;
                        break;
                    }
                }
            }
            else
            {
                ExpandDockItem item = null;
                foreach (ExpandDockItem kv in Items)
                {
                    if ((item = kv.FindChild(itemid)) != null)
                    {
                        foreach (UIElement k in item.Children)
                        {
                            if (k is Button)
                            {
                                Button btn = (Button)k;
                                btn.Content = context;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }


        public bool Expand(string itemid)
        {
            bool bret = false;
            List<ExpandDockItem> items = (from kv in datas where kv.Areafnbr == itemid select (ExpandDockItem)(kv.UI)).ToList<ExpandDockItem>();
            if (items.Count > 0)
            {
                items[0].bExpand = true;
                //name=items[0].Name;
                bret = true;
            }
            else
            {
                ExpandDockItem item = null;
                foreach (ExpandDockItem kv in Items)
                {
                    if ((item = kv.FindChild(itemid)) != null)
                    {
                        ((ExpandDockItem)item.ItemParent).bExpand = true;
                        //name = ((ExpandDockItem)item.ItemParent).Name;
                        break;
                    }
                }
            }
            return bret;

        }



    }

    public class HeaderItem : Button
    {
        public static readonly DependencyProperty HeaderTypeProperty = DependencyProperty.Register("HeaderType", typeof(int), typeof(HeaderItem), new PropertyMetadata(1));
        public static readonly DependencyProperty IsExpandProperty = DependencyProperty.Register("IsExpand", typeof(bool), typeof(HeaderItem), new PropertyMetadata(false));

        public int HeaderType
        {
            get
            {
                return (int)base.GetValue(HeaderTypeProperty);
            }
            set
            {
                base.SetValue(HeaderTypeProperty, value);
            }
        }

        public bool IsExpand
        {
            get
            {
                return (bool)base.GetValue(IsExpandProperty);
            }
            set
            {
                base.SetValue(IsExpandProperty, value);
            }
        }
    }

    public class ExpandDockItem : DockPanel
    {
        private StackPanel stackPanel = null;
        private ExpandDockItemData self = null;
        private UIElement itemparent = null;
        private bool bexpand = false;

        public class ItemClickEventArgs : RoutedEventArgs
        {
            public ExpandDockItemData SourceData = null;
            public bool bExpand = false;
            public ItemClickEventArgs(RoutedEvent routEvent, object source)
                : base(routEvent, source)
            {

            }
        }
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("ClickItem", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExpandDockItem));

        public ExpandDockItem(ExpandDockItemData data, UIElement itemParent)
        {
            self = data;
            itemparent = itemParent;
            HeaderItem button = new HeaderItem();
            button.HeaderType = data.Type;
            button.Click += new RoutedEventHandler(button_Click);
            button.Content = data.Name;
            button.SetValue(DockPanel.DockProperty, Dock.Top);
            this.Children.Add(button);
            stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            if (data.Children.Count > 0)
            {
                foreach (ExpandDockItemData kv in data.Children)
                {
                    ExpandDockItem item = new ExpandDockItem(kv, this);
                    kv.UI = item;
                    item.SetValue(DockProperty, Dock.Top);
                    stackPanel.Children.Add(item);
                }
                stackPanel.Height = 0;
            }
            this.Children.Add(stackPanel);

        }

        public ExpandDockItem FindChild(string id)
        {
            ExpandDockItem item = null;
            List<ExpandDockItem> items = (from kv in ItemChildren where kv.Context.Areafnbr == id select kv).ToList<ExpandDockItem>();
            if (items.Count > 0)
            {
                item = items[0];
            }
            else
            {
                foreach (ExpandDockItem kv in ItemChildren)
                {
                    if ((item = kv.FindChild(id)) != null)
                    {
                        break;
                    }
                }
            }
            return item;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            this.bExpand = !this.bExpand;
            ItemClickEventArgs args = new ItemClickEventArgs(ExpandDockItem.ClickEvent, sender);
            args.SourceData = self;
            args.bExpand = this.bExpand;
            RaiseEvent(args);
        }


        public UIElement ItemParent
        {
            get
            {
                return itemparent;
            }
            set
            {
                itemparent = value;
            }
        }

        public List<ExpandDockItem> ItemChildren
        {
            get
            {
                List<ExpandDockItem> items = new List<ExpandDockItem>();
                if (stackPanel != null)
                {
                    foreach (UIElement kv in stackPanel.Children)
                    {
                        if (kv is ExpandDockItem)
                        {
                            items.Add((ExpandDockItem)kv);
                        }
                    }
                }
                return items;
            }

        }

        public List<ExpandDockItem> ItemBrother
        {
            get
            {
                List<ExpandDockItem> brother = new List<ExpandDockItem>();
                IEnumerable<ExpandDockItem> items = null;
                if (this.ItemParent is ExpandDockPanel)
                {
                    items = ((ExpandDockPanel)this.ItemParent).Items.OfType<ExpandDockItem>();
                }
                else
                {
                    items = ((ExpandDockItem)this.ItemParent).ItemChildren;
                }
                foreach (UIElement kv in items)
                {
                    ExpandDockItem item = (ExpandDockItem)kv;
                    if (item != this)
                    {
                        brother.Add(item);
                    }
                }

                return brother;
            }
        }

        public void AddItem(ExpandDockItem item)
        {
            if (stackPanel == null)
            {
                stackPanel = new StackPanel();
                this.Children.Add(stackPanel);
            }
            item.ItemParent = this;
            stackPanel.Children.Add(item);

        }


        public ExpandDockItemData Context
        {
            get
            {
                return self;
            }
        }


        public bool bExpand
        {
            get
            {
                return bexpand;
            }
            set
            {
                //如果展开，通知兄弟收起
                if (value)
                {
                    foreach (ExpandDockItem kv in this.ItemBrother)
                    {
                        kv.bExpand = false;
                    }
                    //有儿子才会展开
                    if (this.ItemChildren.Count > 0)
                    {
                        ExpandAnimation();
                    }
                }
                else
                {
                    //如果原来是展开的
                    if (bexpand)
                    {
                        CollapseAnimation();
                    }

                }
                ((HeaderItem)Children[0]).IsExpand = value;
                bexpand = value;
            }
        }

        public void CollapseAnimation()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = stackPanel.ActualHeight;
            da.To = 0;
            Duration duration = new Duration(TimeSpan.FromMilliseconds(300));
            da.Duration = duration;
            stackPanel.BeginAnimation(HeightProperty, da);
        }

        private double GetLeftHeight()
        {
            ExpandDockItem item = this;
            int count = 0;
            while (item.ItemParent is ExpandDockItem)
            {
                count += ((ExpandDockItem)item.ItemParent).ItemChildren.Count;
                if (item.ItemParent is ExpandDockItem)
                {
                    item = (ExpandDockItem)item.ItemParent;
                }
                else
                {
                    break;
                }
            }
            double height = 0;
            if (item.Parent is ExpandDockPanel)
            {
                ExpandDockPanel panel = (ExpandDockPanel)item.Parent;
                count += panel.Items.Count;
                height = count * (((Button)this.Children[0]).ActualHeight);
                height = panel.ActualHeight - height;
            }
            return height;

        }

        public void ExpandAnimation()
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = GetLeftHeight();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(300));
            da.Duration = duration;
            stackPanel.BeginAnimation(HeightProperty, da);
        }



    }


}
