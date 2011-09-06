<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RichLookupFormField.ascx.cs" Inherits="AnjLab.SharePoint.RichControls.ControlTemplates.RichLookupFormField" %>

<script type="text/javascript">
    if (!window.jQuery) {
        document.write('<script src="/_layouts/RichControls/jquery/js/jquery-1.5.1.min.js">\x3C/script>');
    } 
 </script>

 <script type="text/javascript" src="/_layouts/RichControls/jquery/js/jquery-ui-1.8.11.custom.min.js"></script>
 <script type="text/javascript" src="/_layouts/RichControls/rich-controls.js?1.1.17"></script>
<SharePoint:CssRegistration runat="server" Name="/_layouts/RichControls/jquery/css/cupertino/jquery-ui-1.8.7.custom.css"/>
<SharePoint:CssRegistration runat="server" Name="/_layouts/RichControls/rich-controls.css?1.0.5"/>

<asp:TextBox ID="lookupTextBox" runat="server" CssClass="ui-widget-content lookupTextBox ms-long ms-spellcheck-true" /><input runat="server" id="lookupButton" type="button" title='<%# GetLocalizedString("richLookupShowAll") %>' class="lookupButton ui-button ui-widget ui-state-default ui-button-icon-only ui-button-icon-primary ui-icon ui-icon-triangle-1-s"/><%if(!IsViewHidden) {%><a runat="server" id="viewButton" title='<%# GetLocalizedString("richLookupView") %>' class="lookupButton ui-button ui-widget ui-state-default ui-button-icon-only ui-button-icon-primary ui-icon ui-icon-search" href="<%# GetDisplayFormUrl() %>" target="_blank">&nbsp;</a><%} %><%if(!IsAddHidden) {%><a title="<%= GetLocalizedString("richLookupAddNew") %>" class="lookupButton ui-button ui-widget ui-state-default ui-button-icon-only ui-button-icon-primary ui-icon ui-icon-circle-plus" href="<%= GetNewFormUrl() %>" target="_blank">&nbsp;</a><%} %>
<div runat="server" id="selection" class="lookupSelection"></div>

<% if(!string.IsNullOrEmpty(MaxHeight)) {%>
<style>
	.ui-autocomplete {
		max-height: <%= MaxHeight %>;
		overflow-y: auto;
		overflow-x: hidden;
	}
	* html .ui-autocomplete {
		height: <%= MaxHeight %>;
	}
</style>
<%} %>

<script type="text/javascript">
        $(document).ready(function () {
            var tb = $('#<%= lookupTextBox.ClientID%>');
            tb.richlookup(
            {
                'viewButton':'#<%= viewButton.ClientID%>',
                'lookupButton':'#<%= lookupButton.ClientID%>',
                'selectionID' :'#<%= selection.ClientID%>',
                'list':'<%=ListGuid %>',
                'valueField':'<%=ValueField %>',
                'titleField':'<%=TitleField %>',
                'descFields':'<%=DescriptionFields %>',
                'maxRows':'<%=MaxRows %>',
                'minLength':'<%=MinLength %>',
                'isMultiple':<%=AllowMultipleValues.ToString().ToLower() %>,
                'filter':"<%=Filter.Replace("'","\\'").Replace("\"","\\'").Replace("\r", "").Replace("\n", "") %>",
                'siteID':'<%=SiteID %>',
                'webID':'<%=WebID %>'
            });
                            
            <% foreach (SPFieldLookupValue value in Values)
               {
                   Response.Write(string.Format("tb.addValueToRichlookup('{0}', '{1}', false);", value.LookupId, value.LookupValue));
               }
            %>
        });
</script>