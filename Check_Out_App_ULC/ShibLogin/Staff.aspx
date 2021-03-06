﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Staff.aspx.cs" Inherits="ShibLogin.ShibLogin_Staff" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Academic Alerts Login</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Panel ID="Panel_Login_reloc" runat="server">
        This page uses Shiboleth to log you in.  If you are seeing this message, something in that process has gone wrong and you will need to <asp:HyperLink ID="Login_Link" runat="server" Text="Login"></asp:HyperLink>
        </asp:Panel>
        <asp:Panel ID="Panel_Problem" runat="server">
            <h2>A problem has been detected:</h2>
            <asp:Literal ID="message" runat="server"></asp:Literal>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
