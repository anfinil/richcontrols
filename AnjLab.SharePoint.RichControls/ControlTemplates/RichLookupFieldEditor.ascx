<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RichLookupFieldEditor.ascx.cs" Inherits="AnjLab.SharePoint.RichControls.ControlTemplates.RichLookupFieldEditorControl" %>
<%@ Register TagPrefix="wss" TagName="LookupFieldEditor" src="~/_controltemplates/LookupFieldEditor.ascx" %>


<wss:LookupFieldEditor ID="lookupFieldEditor" runat="server" />
<br />

<div>
    Title field: <asp:TextBox ID="titleFieldTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Value field (Title field by default): <asp:TextBox ID="valueFieldTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Description fields (comma-separated): <asp:TextBox ID="descriptionFieldsTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Autocomplete min length (2, by default): <asp:TextBox ID="minLengthTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Autocomplete max rows (10, by default): <asp:TextBox ID="maxRowsTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Autocomplete max height (unlimited by default): <asp:TextBox ID="maxHeightTextBox" runat="server" Width="100%" CssClass="ms-input" />
</div>
<div>
    Filter CAML: <asp:TextBox ID="filterTextBox" runat="server" Width="100%" TextMode="MultiLine" Rows="3" CssClass="ms-input" />
</div>
<br />
