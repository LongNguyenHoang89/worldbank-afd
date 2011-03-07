<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="NCRVisual.web.SignIn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="Microsoft.Live" %>
<%@ Import Namespace="Microsoft.Live.AuthHandler" %>

<script runat="server">
    public string SessionId
    {
        get
        {
            SessionIdProvider oauth = new SessionIdProvider();
            return "wl_session_id=" + oauth.GetSessionId(HttpContext.Current);
        }     
    }
</script>