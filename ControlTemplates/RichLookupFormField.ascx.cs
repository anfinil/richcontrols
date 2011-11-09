using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;

namespace AnjLab.SharePoint.RichControls.ControlTemplates
{
    public partial class RichLookupFormField : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(string.IsNullOrEmpty(MinLength))
                MinLength = 2.ToString();

            if(string.IsNullOrEmpty(MaxRows))
                MaxRows = 10.ToString();

            if (string.IsNullOrEmpty(ValueField))
                ValueField = TitleField;

            if (Filter == null) Filter = string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeValues();

            if(!IsPostBack)
                DataBind();
        }

        public SPFieldLookup Field { get; set; }

        public string ListUrl { get; set; }

        private bool allowMultipleValues;
        public bool AllowMultipleValues
        {
            get
            {
                if (Field != null) return Field.AllowMultipleValues;
                return allowMultipleValues;
            }
            set { allowMultipleValues = value; }
        }

        public string SiteID
        {
            get
            {
                var list = GetList();

                return list != null ? list.ParentWeb.Site.ID.ToString() : SPContext.Current.Web.Site.ID.ToString();
            }
        }

        public string WebID
        {
            get
            {
                var list = GetList();

                return list != null ? list.ParentWeb.ID.ToString() : SPContext.Current.Web.ID.ToString();
            }
        }

        public bool IsAddHidden { get; set; }

        public bool IsViewHidden { get; set; }

        public string ListGuid
        {
            get
            {
                if (Field != null) return Field.LookupList;

                var list = GetList();

                return list != null ? list.ID.ToString() : string.Empty;
            }
        }

        private bool _valuesInitialized;
        public readonly SPFieldLookupValueCollection Values = new SPFieldLookupValueCollection();

        private void InitializeValues()
        {
            if (!_valuesInitialized)
            {
                _valuesInitialized = true;

                var vals = Request.Params["#" + selection.ClientID];
                if (string.IsNullOrEmpty(vals)) return;

                var regex = new Regex(@"\[([^;]+);([^]]+)\]", RegexOptions.Compiled);
                foreach (Match match in regex.Matches(vals))
                {
                    Values.Add(new SPFieldLookupValue(Convert.ToInt32(match.Groups[1].Value),
                        HttpUtility.UrlDecode(match.Groups[2].Value)));
                }
            }
        }

        public object Value
        {
            get
            {
                InitializeValues();

                if (AllowMultipleValues)
                    return !Values.Any() ? null : Values;

                return Values.FirstOrDefault();
            }
            set
            {
                _valuesInitialized = true;

                if (value == null)
                {
                    Values.Clear();
                    lookupTextBox.Text = null;
                    return;
                }

                var collection = value as SPFieldLookupValueCollection;
                if(collection != null)
                {
                    Values.AddRange(collection);
                } 
                else if(value is SPFieldLookupValue)
                {
                    Values.Add((SPFieldLookupValue)value);
                }

                if (Values.Count > 0)
                    lookupTextBox.Text = Values.First().LookupValue;
            }
        }

        public string MinLength { get; set; }

        public string MaxRows { get; set; }

        public string MaxHeight { get; set; }

        public string ValueField { get; set; }

        public string TitleField { get; set; }

        public string DescriptionFields { get; set; }

        public string Filter { get; set; }

        public string GetLocalizedString(string key)
        {
            return SPUtility.GetLocalizedString("$Resources:AnjLab.SharePoint.RichControls," + key, "AnjLab.SharePoint.RichControls", SPContext.Current.Web.Language);
        }

        public string GetNewFormUrl()
        {
            try
            {
                var list = GetList();
                return list == null ? string.Empty : list.DefaultNewFormUrl;
            }
            catch (Exception exc)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("AnjLab.SharePoint.RichControls", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    exc.Message, exc.StackTrace);
                return string.Empty;
            }
        }

        public string GetDisplayFormUrl()
        {
            try
            {
                var list = GetList();
                if (list == null) return string.Empty;

                var url = list.DefaultDisplayFormUrl + "?ID=";
                if(Values.Count > 0)
                {
                    url += Values.First().LookupId;
                }
                return url;
            }
            catch (Exception exc)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("AnjLab.SharePoint.RichControls", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    exc.Message, exc.StackTrace);
                return string.Empty;
            }
        }

        private SPList theList;
        private bool listAccessDenied;

        private SPList GetList()
        {
            if (theList != null || listAccessDenied) return theList;

            if (Field == null && string.IsNullOrEmpty(ListUrl))
                throw new ArgumentException("Field or ListUrl is not specified.");

            using(var web = SPContext.Current.Site.OpenWeb(Field == null ? SPContext.Current.Web.ID : Field.LookupWebId))
            {
                web.Lists.ListsForCurrentUser = true;

                foreach (var list in  web.Lists.Cast<SPList>())
                {
                    if (Field != null)
                    {
                        if (list.ID == new Guid(Field.LookupList))
                            return theList = list;
                    }

                    if (!string.IsNullOrEmpty(ListUrl))
                    {
                        if (list.DefaultViewUrl.StartsWith(GetListUrl(web, ListUrl)))
                            return theList = list;
                    }
                }
            }

            listAccessDenied = true;
            return null;
        }

        public static string GetListUrl(SPWeb web, string listUrl)
        {
            return GetListUrl(web.ServerRelativeUrl, listUrl);
        }

        public static string GetListUrl(string webRelativeUrl, string listUrl)
        {
            if (webRelativeUrl[webRelativeUrl.Length - 1] != '/') return (webRelativeUrl + '/' + listUrl);
            else return (webRelativeUrl + listUrl); // Root web case
        }
    }
}
