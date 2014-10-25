namespace MapFarce.EditProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    public partial class CharEditorControl : UserControl
    {
        class CharDescription
        {
            public string Name;
            public char Value;

            public CharDescription(string name,char value)
            {
                this.Name = name;
                this.Value = value;
            }
            public override string ToString()
            {
                return this.Name;
            }
            public static CharDescription Custom = new CharDescription("(Custom)", char.MinValue);
            public static CharDescription[] All = new[] {
                new CharDescription("Tab", '\t'),
                new CharDescription("Comma", ','),
                new CharDescription("Dot", ','),
                new CharDescription("Semicolon", ';'),
                new CharDescription("Colon", ':'),
                new CharDescription("Double Quote", '\"'),
                new CharDescription("Single Quote", '\''),
                new CharDescription("Space", '\t'),
                Custom
            };
        }

        public CharEditorControl()
        {
            InitializeComponent();
            this.comboCharType.Items.AddRange(CharDescription.All);
        }

        public char Value {
            get {
                var description = (CharDescription)this.comboCharType.SelectedItem;
                return description == CharDescription.Custom ? this.textBoxCustomChar.Text.FirstOrDefault() : description.Value;
            }
            set {
                var description = CharDescription.All.FirstOrDefault(cd => cd.Value == value) ?? CharDescription.Custom;
                this.comboCharType.SelectedItem = description;
                this.textBoxCustomChar.Text = value.ToString();
                this.textBoxCustomChar.Enabled = description == CharDescription.Custom;
            }
        }

        private void comboCharType_SelectedValueChanged(object sender, EventArgs e)
        {
            var description = (CharDescription)this.comboCharType.SelectedItem;
            this.textBoxCustomChar.Text = description.Value.ToString();
            this.textBoxCustomChar.Enabled = description == CharDescription.Custom;
        }
    }
}
