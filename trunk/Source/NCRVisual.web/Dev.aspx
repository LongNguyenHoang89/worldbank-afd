<%@ Page Language="C#" AutoEventWireup="true" %>

<!-- BEGIN: Scripts for Live ID -->
<%@ Import Namespace="System.Web.Configuration" %>
<%@ Import Namespace="Microsoft.Live" %>

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
<!-- END: Scripts for Live ID -->

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:wl="http://apis.live.net/js/2010">
<head id="Head1" runat="server">
    <title>WORLD STATE VISUALIZATION</title>
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>

    <!-- BEGIN: Scripts for Live ID -->
    <script type="text/javascript" src="http://js.live.net/4.1/loader.js"></script>
    <script type="text/javascript">

        var plugin;

        function pluginLoaded(sender, args) {
            plugin = sender.getHost();
        }

        function appLoaded(appLoadedEventArgs) {
            Microsoft.Live.Core.Namespace.using("wl:Microsoft.Live");
        }

        function signIn() {
            if (Microsoft.Live.App.get_auth().get_state() !== Microsoft.Live.AuthState.authenticated) {
                Microsoft.Live.App.signIn(signInCompleted);
            }
        }

        function signInCompleted(signInCompletedEventArgs) {
            if (signInCompletedEventArgs.get_resultCode() !== Microsoft.Live.AsyncResultCode.success) {
                // Take action.
                Sys.Debug.writeLine("error: " + signInCompletedEventArgs.get_error());

            }
            else {
                plugin.Content.MainPage.SignInCompleted(true, Microsoft.Live.App.get_auth().get_cid());
                messengerContext = wl.App.get_messengerContext();
                messengerContext.signIn(Microsoft.Live.Messenger.PresenceStatus.online, onSignedIn);
            }
        }

        function onSignedIn(evtArgs) {
            if (evtArgs != Microsoft.Live.Messenger.SignInResultCode.success) {
                plugin.Content.MainPage.SignInCompleted(true, "Failed");
            }
            else {
                messengerContext = wl.App.get_messengerContext();
                user = messengerContext.get_user();
                plugin.Content.MainPage.SignInMessengerCompleted(
                    Microsoft.Live.App.get_auth().get_cid(),
                    user.get_displayName()
                    );
            }
        }

        function signOut() {
            if (Microsoft.Live.App.get_auth().get_state() === Microsoft.Live.AuthState.authenticated) {
                Microsoft.Live.App.signOut();
                plugin.Content.MainPage.SignInCompleted(false, Microsoft.Live.App.get_auth().get_cid());
            }
        }  
    </script>
    <!-- END: Scripts for Live ID -->

    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
</head>
<body>

    <script runat="server">
        private string InitParam = string.Empty;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString != null)
            {
                InitParam = "Type =" + Request.QueryString["Param"] + ", FileName=" + Request.QueryString["FileName"];
            }
        }
    </script>

    <!-- END: Scripts for Live ID -->
    <wl:app channel-url="<%=WebConfigurationManager.AppSettings["wl_wrap_channel_url"]%>"
    callback-url="<%=WebConfigurationManager.AppSettings["wl_wrap_client_callback"]%>?<%=SessionId%>"
    client-id="<%=WebConfigurationManager.AppSettings["wl_wrap_client_id"]%>" scope="Messenger.SignIn"
    onload="{{appLoaded}}">
    </wl:app>
    <!-- END: Scripts for Live ID -->

    <form id="form1" runat="server" style="height:100%">
    <div id="silverlightControlHost">
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		  <param name="source" value="ClientBin/Dev.xap"/>
          <!-- END: Scripts for Live ID -->
          <param name="onLoad" value="pluginLoaded" />
          <!-- END: Scripts for Live ID -->
		  <param name="onError" value="onSilverlightError" />
		  <param name="background" value="white" />
		  <param name="minRuntimeVersion" value="4.0.50401.0" />
		  <param name="autoUpgrade" value="true" />
          <param name="InitParams" value = "<%=InitParam %>" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe></div>
    </form>
</body>
</html>
