<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>LoginHub API Barebones C#</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Login Hub API Example Page</h1>

        <section id="usernameSection" runat="server">

            <h3>Please enter your username</h3>

            <asp:TextBox ID="Username" runat="server"></asp:TextBox>

            <asp:Button ID="SubmitUsername" runat="server" Text="Submit" />

        </section>

        <section id="passwordSection" runat="server" visible="false">

            <h3>Please enter your password</h3>

            <asp:TextBox ID="Password" TextMode="Password" runat="server"></asp:TextBox>

            <asp:Button ID="SubmitPassword" runat="server" Text="Submit" />
        </section>

    </div>
    </form>
</body>
</html>
