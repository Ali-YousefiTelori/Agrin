<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
@{ 
    ViewBag.Title = "Index";   
} 
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>index</title>
</head>
<body>
    <div>
        @{

      var now = DateTime.Now;

      <text>

            Current time is @now

      </text>

      <text>
            Current time is @Model.ToString(CultureInfo.CurrentUICulture)
      </text>

}
        <h2>Index</h2>
        @ViewData["UserName"]
    </div>
</body>
</html>
