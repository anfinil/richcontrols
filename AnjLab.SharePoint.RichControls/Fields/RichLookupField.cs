using AnjLab.SharePoint.RichControls.ControlTemplates;
using Microsoft.SharePoint;

namespace AnjLab.SharePoint.RichControls.Fields
{
    public class RichLookupField : SPFieldLookup
    {
        public RichLookupField(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
        }

        public RichLookupField(SPFieldCollection fields, string typeName, string displayName) : base(fields, typeName, displayName)
        {
        }

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get { return new RichLookupFieldControl() { FieldName = InternalName }; }
        }

        public string Filter
        {
            get { return this.GetFieldAttribute("Filter");}
            set { this.SetFieldAttribute("Filter", value);}
        }
    }
}
