<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PHPBB_DB_Info.aspx.cs" Inherits="NCRVisual.Web.PHP_BB_Info" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1
        {
            height: 367px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Label ID="LabelStatus" runat="server" Text="" ForeColor="Blue"></asp:Label>
        <div>
            <asp:Label ID="lblBanner" runat="server" 
                Text="Please enter the following data of your PHPBB database"></asp:Label><br />
        </div>
        <asp:Label ID="Label1" runat="server" Text="DB URL:"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server">localhost</asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="DB name:"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server">phpbb</asp:TextBox>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Username:"></asp:Label>
        <asp:TextBox ID="TextBox3" runat="server">root</asp:TextBox>
        <br />
        <asp:Label ID="Label4" runat="server" Text="Password"></asp:Label>
        <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
        <br />
        <br />

        <asp:Button ID="Button1" runat="server" Text="Submit" 
            onclick="Button1_Click" />
        <br />
        
        <br />
        <asp:HyperLink ID="HyperLink1" runat="server">Return to Visualize</asp:HyperLink>        

        <br />
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    </form>
</body>
</html>
