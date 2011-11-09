using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
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

            //webDropDownList.Items.AddRange(SPContext.Current.Site.AllWebs.Select(
            //    w => new ListItem(w.Title, w.ID.ToString())).ToArray());
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

            var lookupField = field as SPFieldLookup;
            if (lookupField == null) return;

            //webDropDownList.SelectedValue = Guid.Empty == lookupField.LookupWebId
            //                                    ? SPContext.Current.Web.ID.ToString()
            //                                    : lookupField.LookupWebId.ToString();
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

        //protected void WebDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        //{
            
        //}
    }
}
