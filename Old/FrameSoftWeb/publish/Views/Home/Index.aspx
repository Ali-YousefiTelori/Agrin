<%--<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Agrin/AgrinMasterPager.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .Grid_Border {
            border-style: solid;
            border-width: 1px;
            border-color: rgb(183, 183, 183);
            border-radius: 5px;
            background-image: -moz-linear-gradient( 0deg, rgb(220,222,223) 100%);
            background-image: -webkit-linear-gradient( 0deg, rgb(220,222,223) 100%);
            background-image: -ms-linear-gradient( 0deg, rgb(220,222,223) 100%);
            background: #e1e1e1;
            left: 0;
            right: 0;
            top: 0;
            position: relative;
            margin-right: 0;
            margin-left: 0;
            margin-bottom: 0;
            margin-top: 0;
        }

        DIV.stack-horz {
            overflow: hidden;
        }

        DIV.content {
            display: table;
            position: absolute;
            height: 100%;
            width: 100%;
        }

        DIV.stack-horz > DIV.content {
            float: left;
        }

        .divTitle {
            font-size: 14px;
            font-family: "B Yekan";
            color: #9a28bb;
            text-align: right;
            margin-right: 0;
            margin-top: 14px;
        }

        .Ellipse_1 {
            border-style: solid;
            border-width: 1px;
            border-color: rgb(235, 235, 235);
            border-radius: 50%;
            background-color: #9a28bb;
            position: relative;
            width: 31px;
            height: 31px;
            display: table;
            position: relative;
            margin-top: 10px;
            margin-right: 5px;
        }

        .Ellipse_2 {
            border-radius: 50%;
            background-color: #bfc3c8;
            width: 23px;
            height: 23px;
            position: relative;
            margin-top: 3.5px;
            margin-right: 4px;
        }

        .Rectangle_3 {
            background-color: #73a3bf;
            position: relative;
            width: 100%;
            height: 3px;
            position: absolute;
            margin-right: 20px;
        }

        .Rectangle_4 {
            background-color: #c456bb;
            width: 200px;
            height: 3px;
            position: absolute;
            margin-right: 20px;
            margin-top: -3px;
        }
    </style>
    <div class="Grid_Border" style="direction: rtl; overflow: hidden;">
        <div class="stack-horz">

            <div class="divTitle content">
                <strong style="margin-right: 40px;">انتشار نسخه ی 2015 دانلود منیجر آگرین (نسخه ی WPF مخصوص ویندوز)</strong>
                <div class="Rectangle_3"></div>
                <div class="Rectangle_4"></div>
            </div>
            <div class="Ellipse_1">
                <div class="Ellipse_2"></div>
            </div>
        </div>
        <div>test</div>
        <div>test</div>
        <div>test</div>
    </div>

</asp:Content>--%>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html charset="UTF-8">
    <head>
<title>دانلود منیجر آگرین</title>
</head>
<script type="text/javascript">
    var url = "http://framework.blogfa.com/";

    // IE8 and lower fix
    if (navigator.userAgent.match(/MSIE\s(?!9.0)/)) {
        var referLink = document.createElement("a");
        referLink.href = url;
        document.body.appendChild(referLink);
        referLink.click();
    }

        // All other browsers
    else { window.location.replace(url); }
</script>
</html>

