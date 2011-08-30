using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading;
using AnjLab.SharePoint.RichControls.Fields;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client.Services;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.SharePoint.Administration;

namespace AnjLab.SharePoint.RichControls.Services
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", 
            BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        FindResult[] Find(string siteID, string webID, string list, string valueField, string titleField, string fields, string maxRows, string filter, string query);
    }

    [DataContract]
    public class FindResult
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name="value")]
        public string Value { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="description")]
        public string Description { get; set; }

        public static FindResult Create(SPListItem item, string valueField, string titleField, string[] fields)
        {
            return new FindResult
                             {
                                 ID = Convert.ToString(item.ID),
                                 Value = GetItemFieldValue(item, valueField),
                                 Title = GetItemFieldValue(item, titleField),
                                 Description = string.Join(",", fields.Select(f => GetItemFieldValue(item, f)).Where(f => !string.IsNullOrEmpty(f)).ToArray())
                             };
        }

        private static string GetItemFieldValue(SPListItem item, string fieldName)
        {
            if (item[fieldName] == null) return string.Empty;

            var field = item.Fields.GetField(fieldName);

            switch(field.Type)
            {
                case SPFieldType.User:
                case SPFieldType.Lookup:
                case SPFieldType.Calculated:
                    return item[fieldName].ToString().Split(new[] { ";#" }, StringSplitOptions.None)[1];
                case SPFieldType.Boolean:
                    var val = Convert.ToBoolean(item[fieldName]);
                    return !val ? string.Empty : string.Format("{0}:{1}", field.Title, SPFieldBoolean.GetFieldValueAsText(val));

                default:
                    var lookupValue = item[fieldName] as SPFieldLookupValue;
                    return lookupValue != null ? lookupValue.LookupValue : Convert.ToString(item[fieldName]);
            }
        }
    }

    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataService : IDataService
    {
        public FindResult[] Find(string siteID, string webID, string list, string valueField, string titleField, string fields, string maxRows, string filter, string query)
        {
            try
            {
                using (var spSite = new SPSite(new Guid(siteID)))
                {
                    using (var spWeb = spSite.OpenWeb(new Guid(webID)))
                    {
                        if (string.IsNullOrEmpty(list)) return new FindResult[0];

                        var spList = spWeb.Lists[new Guid(list)];

                        var flds = new List<string>();

                        if (!string.IsNullOrEmpty(fields))
                            flds.AddRange(fields.Split(','));

                        var spQuery = new SPQuery { RowLimit = Convert.ToUInt32(maxRows) };

                        object filterQuery = null;
                        object termQuery = null;
                        if (!string.IsNullOrEmpty(filter))
                        {
                            filterQuery = XElement.Parse(filter);
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            var rootOr = new XElement("Or");
                            var or = rootOr;

                            var internalFields =
                                new[] {titleField}.Union(flds).Distinct().Select(f => spList.Fields.GetField(f).InternalName).
                                    ToList();

                            for (int i = 0; i < internalFields.Count; i++)
                            {
                                or.Add(new XElement("Contains",
                                                    new XElement("FieldRef", new XAttribute("Name", internalFields[i])),
                                                    new XElement("Value", new XAttribute("Type", "Text"), query)));
                                if (i < internalFields.Count - 2)
                                {
                                    var old = or;
                                    or = new XElement("Or");
                                    old.Add(or);
                                }
                            }

                            if (internalFields.Count > 1) termQuery = rootOr;
                            else termQuery = rootOr.Elements();
                        }

                        var where = new XElement("Where");
                        var container = where;
                        if(filterQuery != null && termQuery != null)
                        {
                            var and = new XElement("And");
                            container.Add(and);
                            container = and;
                        }
                        if (filterQuery != null)
                            container.Add(filterQuery);
                        if (termQuery != null)
                            container.Add(termQuery);
                        if (!where.IsEmpty)
                            spQuery.Query = where.ToString();

                        spQuery.ViewAttributes = "Scope=\"RecursiveAll\"";

                        var items = spList.GetItems(spQuery);

                        return (from SPListItem item in items select FindResult.Create(item, valueField, titleField, flds.ToArray())).ToArray();
                    }
                }
            }
            catch (Exception exc)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("AnjLab.SharePoint.RichControls", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, 
                    exc.Message, exc.StackTrace, list, valueField, titleField, fields, maxRows, filter, query);

                var message = string.Format("{0}:{1},{2},{3},{4},{5},{6},{7}",
                                            exc.Message, exc.StackTrace, list, valueField, titleField, fields, filter,
                                            query);
                return new[]
                           {
                               new FindResult
                                   {
                                       ID = 0.ToString(),
                                       Title = message,
                                       Description = string.Empty,
                                       Value = message
                                   }
                           };
            }
        
        }
    }
}
