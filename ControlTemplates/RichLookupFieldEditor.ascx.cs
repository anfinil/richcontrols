using System;
using System.Web.UI;
using AnjLab.SharePoint.RichControls.Fields;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AnjLab.SharePoint.RichControls.ControlTemplates
{
    public partial class RichLookupFieldEditorControl : UserControl, IFieldEditor
    {
        private IFieldEditor _lookupFieldEditor;

        protected void Page_Load(object sender, EventArgs e)
        {
            EnsureChildControls();
        }

        public void InitializeWithField(SPField field)
        {
            EnsureChildControls();

            _lookupFieldEditor.InitializeWithField(field);

            if (field == null || Page.IsPostBack) return;

            filterTextBox.Text = field.GetFieldAttribute("Filter");
            maxHeightTextBox.Text = field.GetFieldAttribute("MaxHeight");
            maxRowsTextBox.Text = field.GetFieldAttribute("MaxRows");
            minLengthTextBox.Text = field.GetFieldAttribute("MinLength");
            titleFieldTextBox.Text = field.GetFieldAttribute("TitleField");
            valueFieldTextBox.Text = field.GetFieldAttribute("ValueField");
            descriptionFieldsTextBox.Text = field.GetFieldAttribute("DescriptionFields");
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _lookupFieldEditor = lookupFieldEditor as IFieldEditor;
        }

        public void OnSaveChange(SPField field, bool isNewField)
        {
            EnsureChildControls();

            _lookupFieldEditor.OnSaveChange(field, isNewField);

            field.SetFieldAttribute("Filter", filterTextBox.Text);
            field.SetFieldAttribute("MaxHeight", maxHeightTextBox.Text);
            field.SetFieldAttribute("MaxRows", maxRowsTextBox.Text);
            field.SetFieldAttribute("MinLength", minLengthTextBox.Text);
            field.SetFieldAttribute("TitleField", titleFieldTextBox.Text);
            field.SetFieldAttribute("ValueField", valueFieldTextBox.Text);
            field.SetFieldAttribute("DescriptionFields", descriptionFieldsTextBox.Text);
        }

        public bool DisplayAsNewSection
        {
            get
            {
                EnsureChildControls();

                return _lookupFieldEditor.DisplayAsNewSection;
            }
        }
    }
}
