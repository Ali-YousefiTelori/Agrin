VERSION 5.00
Begin VB.Form Form1 
   BorderStyle     =   1  'Fixed Single
   Caption         =   "Agrin Download Manager"
   ClientHeight    =   1455
   ClientLeft      =   45
   ClientTop       =   390
   ClientWidth     =   5490
   BeginProperty Font 
      Name            =   "Tahoma"
      Size            =   8.25
      Charset         =   178
      Weight          =   400
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   Icon            =   "frmInstall.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1455
   ScaleWidth      =   5490
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton btnInstall 
      Caption         =   "Download .Net Framework 4.0"
      Default         =   -1  'True
      Height          =   495
      Left            =   2805
      TabIndex        =   0
      Top             =   885
      Width           =   2565
   End
   Begin VB.CommandButton btnCancel 
      Cancel          =   -1  'True
      Caption         =   "Cancel"
      Height          =   495
      Left            =   120
      TabIndex        =   1
      Top             =   840
      Width           =   1215
   End
   Begin VB.Label Label1 
      Alignment       =   1  'Right Justify
      Caption         =   $"frmInstall.frx":9A1A
      Height          =   765
      Left            =   75
      TabIndex        =   2
      Top             =   90
      Width           =   5295
      WordWrap        =   -1  'True
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit
Private Declare Function ShellExecute _
                            Lib "shell32.dll" _
                            Alias "ShellExecuteA" ( _
                            ByVal hWnd As Long, _
                            ByVal lpOperation As String, _
                            ByVal lpFile As String, _
                            ByVal lpParameters As String, _
                            ByVal lpDirectory As String, _
                            ByVal nShowCmd As Long) _
                            As Long

Private Sub btnCancel_Click()
End
End Sub

Private Sub btnInstall_Click()
Dim r As Long
   r = ShellExecute(0, "open", "http://www.microsoft.com/en-us/download/details.aspx?id=24872", 0, 0, 1)
End
End Sub


Private Sub Form_Initialize()
    Dim a As Integer
    With Form1
        .Top = (Screen.Height - .Height) / 2
        .Left = (Screen.Width - .Width) / 2
    End With
    
    Dim text As String
    Dim fso As New FileSystemObject
    text = ReadRegistry(HKEY_LOCAL_MACHINE, "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client\", "Install")
    Dim tempPath As String
    tempPath = Environ$("TEMP")
    If text = "001" Then
      Dim fileName As String
      fileName = tempPath + "\AgrinSetup.exe"
      fso.CopyFile App.Path + "\Data.Agn", fileName, True
      Dim r As Long
      r = ShellExecute(0, "open", fileName, 0, 0, 1)
      End
    End If
End Sub

