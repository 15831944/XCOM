using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public class StringListComboBox : ComboBox
    {
        [ListBindable(false)]
        public class ReadOnlyObjectCollection : ObjectCollection
        {
            public ReadOnlyObjectCollection(ComboBox owner, ObjectCollection items) : base(owner)
            {
                foreach (object item in items)
                {
                    base.Add(item);
                }
            }

            public override object this[int index] { get => base[index]; set => throw new InvalidOperationException(); }
            public new bool IsReadOnly { get => false; }
            public new int Add(object item) { return -1; }
            public new void AddRange(object[] items) { }
            public new void Clear() { }
            public new void Insert(int index, object item) { }
            public new void Remove(object value) { }
            public new void RemoveAt(int index) { }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ComboBoxStyle DropDownStyle
        {
            get => ComboBoxStyle.DropDownList;
            set => base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ObjectCollection Items
        {
            get => new ReadOnlyObjectCollection(this, base.Items);
        }

        public new object SelectedItem
        {
            get => base.SelectedItem;
            set => this.SetSelectedItemFromText((string)value);
        }

        public override string Text
        {
            get => (string)base.SelectedItem;
            set => this.SetSelectedItemFromText(value);
        }

        public StringListComboBox()
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void SetItems(IEnumerable<string> names)
        {
            base.Items.Clear();
            int i = 0;
            foreach (string styleName in names)
            {
                base.Items.Add(styleName);
                if (string.Compare(styleName, Text, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    base.SelectedIndex = i;
                }
                i++;
            }
        }

        protected void SetSelectedItemFromText(string value)
        {
            for (int i = 0; i < base.Items.Count; i++)
            {
                if (string.Compare((string)base.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    base.SelectedIndex = i;
                    return;
                }
            }

            if (base.SelectedIndex == -1 && base.Items.Count > 0) base.SelectedIndex = 0;
        }
    }
}
