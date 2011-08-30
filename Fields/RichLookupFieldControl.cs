using System.Xml.Serialization;
using AnjLab.SharePoint.RichControls.ControlTemplates;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.Web.UI;

namespace AnjLab.SharePoint.RichControls.Fields
{
    [ToolboxData("<{0}:RichLookupFieldControl runat=server></{0}:RichLookupFieldControl>"),
    XmlRoot(Namespace = "AnjLab.SharePoint.RichControls.Fields")]
    public class RichLookupFieldControl : BaseFieldControl
    {
        private RichLookupFormField _richLookupFormField;

        public override object Value
        {
            get
            {
                EnsureChildControls();
                return _richLookupFormField == null ? null : _richLookupFormField.Value;
            }
            set
            {
                EnsureChildControls();
                if(_richLookupFormField != null)
                    _richLookupFormField.Value = value;
            }
        }

        public string ValueField { get; set; }
        public string TitleField { get; set; }
        public string DescriptionFields { get; set; }
        public string MinLength { get; set; }
        public string MaxRows { get; set; }
        public string MaxHeight { get; set; }
        public string Filter { get; set; }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _richLookupFormField = Page.LoadControl("~/_controltemplates/RichLookupFormField.ascx") as RichLookupFormField;
            if (_richLookupFormField != null)
            {
                var field = Field as SPFieldLookup;
                if (field != null)
                {
                    _richLookupFormField.Field = Field as SPFieldLookup;
                    _richLookupFormField.TitleField = TitleField ?? field.GetFieldAttribute("TitleField");
                    _richLookupFormField.DescriptionFields = DescriptionFields ?? field.GetFieldAttribute("DescriptionFields");
                    _richLookupFormField.MaxHeight = MaxHeight ?? field.GetFieldAttribute("MaxHeight");
                    _richLookupFormField.MaxRows = MaxRows ?? field.GetFieldAttribute("MaxRows");
                    _richLookupFormField.MinLength = MinLength ?? field.GetFieldAttribute("MinLength");
                    _richLookupFormField.ValueField = ValueField ?? field.GetFieldAttribute("ValueField");
                    _richLookupFormField.Filter = Filter ?? field.GetFieldAttribute("Filter");
                }
                this.Controls.Add(_richLookupFormField);
            }
        }

        public override void Validate()
        {
            if (ControlMode == SPControlMode.Display) return;

            base.Validate();

            if (IsValid && Value == null)
            {
                if (Field.Required)
                {
                    IsValid = false;
                    ErrorMessage = SPResource.GetString("MissingRequiredField");
                }
            }
        }
    }
}
