<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<%--<head runat="server">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
        <title>Frame Soft</title>
    </head>
    <style type="text/css">
        .auto-style1 {
            width: 203px;
        }
    </style>
</head>--%>
<body>
    <%--    <style>
        .designer {
            display:;
            background: #fff;
            text-align: center;
            font-size: 9pt;
            padding: 3px 0;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            margin: 0 0 6 0;
        }

            .designer a {
                text-decoration: none;
                color: #444444;
                font-weight: bold;
            }

        .ali {
            text-align: center;
            font-size: 7pt;
        }

            .ali a {
                text-decoration: none;
                color: #333333;
            }

        .nama {
            font-family: tahoma;
            font-size: 8pt;
            color: #333333;
        }

        .ss {
            height: 18px;
            overflow: hidden;
        }

        b {
            color: #;
        }

        .gg {
            height: 33px;
            width: 100%;
        }
    </style>
    <form action="/AmarGiri/GetAmar" style="direction: rtl">
        <center>
<table cellspacing="0" cellpadding="0">
  <tr> 
        <td dir="rtl" class="auto-style1">
            <div class="ss">
                <img src="3.gif" align="right">&nbsp;&nbsp;آی پی آنلاين: <b><%= ViewData["OnlineCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="3.gif" align="right">&nbsp;&nbsp;نرم افزار آنلاين: <b><%= ViewData["OnlineApplicationCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="2.gif" align="right">&nbsp; بازديد امروز: <b><%= ViewData["TodayCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="2.gif" align="right">&nbsp; بازديد نرم افزار امروز: <b><%= ViewData["TodayApplicationCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="2.gif" align="right">&nbsp; بازديد آی پی امروز: <b><%= ViewData["TodayIPCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="1.gif" align="right">&nbsp; بازديد دیروز: <b><%= ViewData["YesterdayCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="1.gif" align="right">&nbsp; بازديد هفتگي: <b><%= ViewData["LastWeekCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="1.gif" align="right">&nbsp; بازديد ماهانه: <b><%= ViewData["LastMonthCount"] %></b> نفر
            </div>
            <div class="ss">
                <img src="4.gif" align="right">&nbsp; بازديد كل: <b><%= ViewData["TotalCount"] %></b> نفر
            </div> 
            <div class="ss">
                <img src="4.gif" align="right">&nbsp; آخرین آی پی: <b><%= ViewData["LastIPAddress"] %></b> نفر
            </div>
        </td>
      </tr>
</table>
        </center>
    </form>--%>
    <style>
        h1 {
            font-family: Tahoma;
            font-weight: normal;
            padding: 0px;
            margin: 0px;
        }

            h1.TitleH {
                font-size: 10pt;
            }

            h1.TextH {
                font-size: 9pt;
                text-align: right;
                margin: 5px;
            }

        div.centered div {
            border-style: solid;
            border-width: 1px;
            width: 200px;
            height: 150px;
            direction: rtl;
        }

        div.OnlineItems {
            border-color: #2a913a;
            background-color: #138e26;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        div.TodayItems {
            border-color: #d75a3c;
            background-color: #db562e;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        div.YesterdayItems {
            border-color: #3c80ee;
            background-color: #335da0;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        div.WeekdayItems {
            border-color: #981aa0;
            background-color: #a500ac;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        div.MonthItems {
            border-color: #1aaab9;
            background-color: #00879d;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        div.TotalItems {
            border-color: #6a6a6a;
            background-color: #6b6b6b;
            color: #ffffff;
            margin: 10px;
            height: 120px;
        }

        html, body {
            height: 100%;
            direction: rtl;
        }
    </style>

    <div style="background-color: #000000; width: 100%; height: 100%">
        <form action="/AmarGiri/GetAmar" style="direction: rtl" style="background-color: #000000; width: 100%; height: 100%">
            <table cellpadding="0" cellspacing="0" border="0" style="margin: auto; vertical-align: central">
                <tr>
                    <th>
                        <div class="OnlineItems">
                            <h1 class="TitleH">افراد آنلاین:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">IP آنلاین:</h1>
                                        <h1 class="TextH">نرم افزار آنلاین:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["OnlineCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["OnlineApplicationCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                    <th>
                        <div class="TodayItems">
                            <h1 class="TitleH">امروز:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">بازدید امروز:</h1>
                                        <h1 class="TextH">بازدید نرم افزار امروز:</h1>
                                        <h1 class="TextH">بازدید IP امروز:</h1>
                                        <h1 class="TextH">نصب شده امروز:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["TodayCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["TodayApplicationCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["TodayIPCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["TodayInstallCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                    <th>
                        <div class="YesterdayItems">
                            <h1 class="TitleH">دیروز:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">بازدید دیروز:</h1>
                                        <h1 class="TextH">بازدید نرم افزار دیروز:</h1>
                                        <h1 class="TextH">بازدید IP دیروز:</h1>
                                        <h1 class="TextH">نصب شده دیروز:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["YesterdayCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["YesterdayApplicationCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["YesterdayIPCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["YesterdayInstallCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                </tr>
                <tr>
                    <th>
                        <div class="WeekdayItems">
                            <h1 class="TitleH">هفتگی:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">بازدید هفته پیش:</h1>
                                        <h1 class="TextH">بازدید نرم افزار هفته پیش:</h1>
                                        <h1 class="TextH">بازدید IP هفته پیش:</h1>
                                        <h1 class="TextH">نصب شده هفته پیش:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["LastWeekCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastWeekApplicationCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastWeekIPCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastWeekInstallCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                    <th>
                        <div class="MonthItems">
                            <h1 class="TitleH">ماهانه:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">بازدید ماه پیش:</h1>
                                        <h1 class="TextH">بازدید نرم افزار ماه پیش:</h1>
                                        <h1 class="TextH">بازدید IP ماه پیش:</h1>
                                        <h1 class="TextH">نصب شده ماه پیش:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["LastMonthCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastMonthApplicationCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastMonthIPCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["LastMonthInstallCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                    <th>
                        <div class="TotalItems">
                            <h1 class="TitleH">کل:</h1>
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <th>
                                        <h1 class="TextH">بازدید کل:</h1>
                                        <h1 class="TextH">بازدید و نصب نرم افزار کل:</h1>
                                        <h1 class="TextH">بازدید IP کل:</h1>
                                    </th>
                                    <th>
                                        <h1 class="TextH"><b><%= ViewData["TotalCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["TotalApplicationAndInstallCount"] %></b> نفر</h1>
                                        <h1 class="TextH"><b><%= ViewData["TotalIPCount"] %></b> نفر</h1>
                                    </th>
                                </tr>
                            </table>
                        </div>
                    </th>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
