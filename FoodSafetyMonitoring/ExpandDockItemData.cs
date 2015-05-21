using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WelfareInstitution
{
    public class ExpandDockItemData
    {
        private string name="";
        private int type = 1;
        private string areafnbr = "";
        private List<ExpandDockItemData> child = new List<ExpandDockItemData>();
        private ExpandDockItemData parent = null;
        public ExpandDockItemData(string name, int areatype)
        {
            this.name = name;
            this.type = areatype;
        }
        private UIElement ui;
        public UIElement UI
        {
            get
            {
                return ui;
            }
            set
            {
                ui = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Areafnbr
        {
            get
            {
                return areafnbr;
            }
            set 
            {
                areafnbr = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public void AddChild(ExpandDockItemData data)
        {
            child.Add(data);
            data.Parent = this;
        }

        public List<ExpandDockItemData> Brother
        {
            get
            {
                List<ExpandDockItemData> brother = new List<ExpandDockItemData>();
                if (this.Parent != null)
                {
                    foreach (ExpandDockItemData kv in this.Parent.Children)
                    {
                        if (kv != this)
                        {
                            brother.Add(kv);
                        }
                    }
                }
                return brother;
            }
        }

        public List<ExpandDockItemData> Children
        {
            get
            {
                return child;
            }
        }

        public ExpandDockItemData Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public string tostring()
        {
            ExpandDockItemData data = this;
            List<string> nameList = new List<string>();
            nameList.Add(data.Name);
            while (data.Parent != null)
            {
                data = data.Parent;
                nameList.Add(data.Name);
            }
            string str = "";
            for (int index = nameList.Count - 1; index >= 0; index--)
            {
                str += nameList[index] + ",";
            }
            str = str.Substring(0, str.Length - 1);
            return str;
        }

     
    }
}
