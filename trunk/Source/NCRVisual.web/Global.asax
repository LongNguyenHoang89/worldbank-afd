<%@ Application Language="C#" %>
<script runat="server">
  void Session_Start(object sender, EventArgs e) {
    Session.Add("wl_Session_started", true);
  }       
</script>