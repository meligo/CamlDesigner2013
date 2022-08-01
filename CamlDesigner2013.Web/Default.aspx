<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CamlDesigner2013.Web.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
<p>
        We are going to connect using CSOM, therefore we need an ASPX page.
        </p>
        <p>
        The buttons on top of the current CAML Designer can remain the same. 
        When clicking on the Connection button, it will open a connection panel. Users will only be able to connect via CSOM but you have to ask for their credentials.
        </p>
        <p>
            The List treeview (or listbox) remains left from the tab control. All clauses (ViewFields/OrderBy/Where/QueryOptions) will have there own tab.
        </p>
    
    </div>
    </form>
</body>
</html>
