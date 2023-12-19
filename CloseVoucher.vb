
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Windows.Forms
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Softone

Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Globalization

Public Class CloseVoucher
    Private ReadOnly XSupport As XSupport
    Private ReadOnly XModule As XModule
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim notif = New NotifyIcon With {
                .Icon = My.Resources.Resources.day1_logo,
                .Visible = True
            }
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim ClosingDate = Date.Today
        Dim str As String = "SELECT APIKEY, COMPANYID, COMPANYPASS, USERNAME, PASSWORD, URL, FOLDERPATH FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=1 AND COMPANY=" + company.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)

        Dim credentials As XRow = ds.Current
        Dim companyID = credentials("COMPANYID")
        Dim companyPass = credentials("COMPANYPASS")
        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim url = New Uri(credentials("URL"))
        Dim apikey = credentials("APIKEY")
        Dim folderpath = credentials("FOLDERPATH")


        If ds.Count > 0 Then
            notif.ShowBalloonTip(1000, "ACS", "Έναρξη επικοινωνίας.", ToolTipIcon.Info)
            Dim jsonString As String = "{ " + Chr(34) + "ACSAlias" + Chr(34) + ": " + Chr(34) + "ACS_Issue_Pickup_List" + Chr(34) + ", " + Chr(34) + "ACSInputParameters" + Chr(34) + ": { " + Chr(34) + "Company_ID" + Chr(34) + ": " + Chr(34) + companyID + Chr(34) + ", " + Chr(34) + "Company_Password" + Chr(34) + " : " + Chr(34) + companyPass + Chr(34) + ", " + Chr(34) + "User_ID" + Chr(34) + ": " + Chr(34) + username + Chr(34) + ", " + Chr(34) + "User_Password" + Chr(34) + ": " + Chr(34) + password + Chr(34) + ", " + Chr(34) + "Language" + Chr(34) + ": " + Chr(34) + "GR" + Chr(34) + "," + Chr(34) + "Pickup_Date" + Chr(34) + ": " + Chr(34) + ClosingDate.ToString("yyyy-MM-dd") + Chr(34) + " }} "
            Dim data = Encoding.UTF8.GetBytes(jsonString)
            Dim result_post = SendRequest(url, data, "application/json", "POST", apikey)
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(result_post)


            Dim success = jsonResulttodict.Item("ACSExecution_HasError")
            If Not success Then

                Dim ACSOutputResponce = jsonResulttodict.Item("ACSOutputResponce").ToString
                Dim ACSOutputResponcetodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(ACSOutputResponce)

                Dim ACSValueOutput = ACSOutputResponcetodict.Item("ACSValueOutput").first().ToString
                Dim AcsValueOutputtodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(ACSValueOutput)

                Dim Error_Message = AcsValueOutputtodict.Item("Error_Message")
                Dim PickupList_No = AcsValueOutputtodict.Item("PickupList_No")
                If Error_Message Is Nothing Or Error_Message = "" Then

                    If PickupList_No Is Nothing Then
                        XSupport.Warning("Δεν υπάρχουν ανοιχτές αποστολές")
                    Else
                        Dim strFileURL = New Uri("https://acs-eud2.acscourier.net/Eshops/getlist.aspx?MainID=" + companyID + "&MainPass=" + companyPass + "&UserID=" + username + "&UserPass=" + password + "&MassNumber=" + PickupList_No.ToString + "&DateParal=" + ClosingDate.ToString("yyyy-MM-dd"))
                        Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
                        Dim strPDFLocation = strFileLocation + "\" + PickupList_No.ToString + ".pdf"
                        Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                        Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                        If Not folderexists Then
                            Directory.CreateDirectory(strFileLocation)
                        End If
                        If pdfgexists Then
                            File.Delete(strPDFLocation)
                        End If
                        Dim WC As New WebClient
                        WC.DownloadFile(strFileURL, strPDFLocation)

                        XSupport.Warning("Επιτυχής οριστικοποίηση λίστας" + vbCrLf + "Ιd : " + PickupList_No.ToString)
                    End If

                Else
                    XSupport.Warning(Error_Message.ToString)
                End If

            End If

        Else
            XSupport.Warning("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If

        notif.Visible = False

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim notif = New NotifyIcon With {
                .Icon = My.Resources.Resources.day1_logo,
                .Visible = True
            }
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim ClosingDate = Date.Today
        Dim str As String = "SELECT * FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=2 AND COMPANY=" + XSupport.ConnectionInfo.CompanyId.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)

        Dim credentials As XRow = ds.Current
        Dim folderpath = credentials("FOLDERPATH")


        If ds.Count > 0 Then
            notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Κλείσιμο Ημέρας", ToolTipIcon.Info)

            Dim services As Object
            Dim AuthResult As Object


            If credentials("TESTENVIROMENT") = 1 Then
                services = New Web.taxydromiki.test.JobServicesV2
                AuthResult = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
            Else
                services = New Web.taxydromiki.JobServicesV2
                AuthResult = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
            End If

            If AuthResult.Result = 0 Then
                Dim delResults = services.ClosePendingJobs(AuthResult.Key)
                If delResults = 0 Then
                    Dim xlApp As New Excel.Application()
                    If xlApp Is Nothing Then
                        XSupport.Warning("Excel is not properly installed!!")
                        Return
                    End If

                    str = "SELECT DISTINCT F.CCCD1VOUCHERNO, F.FINCODE, F.CCCD1SHIPNAME, F.CCCD1SHIPADDRESS, F.CCCD1SHIPZIP, F.CCCD1SHIPCITY, F.CCCD1SHIPCELLPHONE, F.CCCD1VOUCHERVALUE, (SELECT P.NAME FROM PAYMENT P WHERE P.COMPANY=F.COMPANY AND P.SODTYPE=13 AND P.PAYMENT=F.PAYMENT) AS PAYMENT,  F.CCCD1VOUCHERQUANTITY FROM FINDOC F WHERE F.CCCD1COURIERCOMPANY=2 AND F.COMPANY=" + company.ToString + " AND CCCD1VOUCHERNO!='' AND F.CCCD1VOUCHERDATE='" + ClosingDate.ToString("yyyyMMdd") + "'"
                    ds = XSupport.GetSQLDataSet(str)
                    If ds.Count > 0 Then

                        Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
                        Dim strExcelLocation = strFileLocation + "\" + Date.Now.ToString("dd-MM") + ".xlsx"
                        Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                        Dim pdfgexists As Boolean = File.Exists(strExcelLocation)
                        If Not folderexists Then
                            Directory.CreateDirectory(strFileLocation)
                        End If
                        If pdfgexists Then
                            File.Delete(strExcelLocation)
                        End If

                        Dim oExcel As Object
                        oExcel = CreateObject("Excel.Application")
                        Dim oBook As Excel.Workbook
                        Dim oSheet As Excel.Worksheet
                        oBook = oExcel.Workbooks.add
                        oSheet = oExcel.Worksheets(1)

                        oSheet.Name = "Λιστα Παραλαβής"
                        oSheet.Range("A1").Value = "Αριθμός Αποστολής"
                        oSheet.Range("B1").Value = "Παραστατικό"
                        oSheet.Range("C1").Value = "Όνομα Παραλήπτη"
                        oSheet.Range("D1").Value = "Διεύθυνση Αποστολής"
                        oSheet.Range("E1").Value = "Πόλη"
                        oSheet.Range("F1").Value = "Τ.Κ."
                        oSheet.Range("G1").Value = "Τηλέφωνο"
                        oSheet.Range("H1").Value = "Τεμάχια"
                        oSheet.Range("I1").Value = "Πληρωμή"
                        oSheet.Range("J1").Value = "Ποσό Αντ/λής"
                        oSheet.Range("A1:J1").Font.Bold = True

                        For i As Integer = 1 To ds.Count
                            oSheet.Cells(i + 1, 1) = ds(i - 1, "CCCD1VOUCHERNO")
                            oSheet.Cells(i + 1, 2) = ds(i - 1, "FINCODE")
                            oSheet.Cells(i + 1, 3) = ds(i - 1, "CCCD1SHIPNAME")
                            oSheet.Cells(i + 1, 4) = ds(i - 1, "CCCD1SHIPADDRESS")
                            oSheet.Cells(i + 1, 5) = ds(i - 1, "CCCD1SHIPCITY")
                            oSheet.Cells(i + 1, 6) = ds(i - 1, "CCCD1SHIPZIP")
                            oSheet.Cells(i + 1, 7) = ds(i - 1, "CCCD1SHIPCELLPHONE")
                            oSheet.Cells(i + 1, 8) = ds(i - 1, "CCCD1VOUCHERQUANTITY")
                            oSheet.Cells(i + 1, 9) = ds(i - 1, "PAYMENT")
                            oSheet.Cells(i + 1, 10) = ds(i - 1, "CCCD1VOUCHERVALUE")
                        Next

                        oSheet.Range("A1:X1").EntireColumn.AutoFit()

                        oBook.SaveAs(strExcelLocation)
                        oBook.Close()
                        oBook = Nothing
                        oExcel.Quit()
                        oExcel = Nothing

                    End If
                    XSupport.Warning("Επιτυχής οριστικοποίηση λίστας")
                Else
                    XSupport.Exception(geniki.ErrorCode(delResults))
                End If
            Else
                XSupport.Exception(geniki.ErrorCode(authresult.Result))
            End If

        Else
            notif.Visible = False
            XSupport.Exception("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If
        notif.Visible = False
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim notif = New NotifyIcon With {
                .Icon = My.Resources.Resources.day1_logo,
                .Visible = True
            }
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim ClosingDate = Date.Today
        Dim str As String = "SELECT COURIERCENTERUSERALIAS, COURIERCENTERCREDENTIALVALUE, COURIERCENTERAPIKEY, URL, FOLDERPATH FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=3 AND COMPANY=" + company.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)

        Dim credentials As XRow = ds.Current
        Dim vCCUserAlias = credentials("COURIERCENTERUSERALIAS")
        Dim vCCCredentialValue = credentials("COURIERCENTERCREDENTIALVALUE")
        Dim vCCApiKey = credentials("COURIERCENTERAPIKEY")
        Dim url = New Uri(credentials("URL") + "Manifest")
        Dim folderpath = credentials("FOLDERPATH")


        If ds.Count > 0 Then
            notif.ShowBalloonTip(1000, "Courier Center", "Έναρξη επικοινωνίας.", ToolTipIcon.Info)

            Dim params As String = "{" +
                                    Chr(34) + "Context" + Chr(34) + " : {" +
                                        Chr(34) + "UserAlias" + Chr(34) + ":" + Chr(34) + vCCUserAlias.ToString + Chr(34) + "," +
                                        Chr(34) + "CredentialValue" + Chr(34) + ":" + Chr(34) + vCCCredentialValue.ToString + Chr(34) + "," +
                                        Chr(34) + "ApiKey" + Chr(34) + ":" + Chr(34) + vCCApiKey.ToString + Chr(34) + "," +
                                    "}," +
                                    Chr(34) + "Date" + Chr(34) + ":" + Chr(34) + ClosingDate.ToString("yyyy-MM-dd") + Chr(34) + "}"

            Dim data = Encoding.UTF8.GetBytes(params)
            Dim result_post = SendCCRequest(url, data, "application/json", "POST")
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(result_post)

            Dim success = jsonResulttodict.Item("Result")
            If success = "Success" Then
                Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
                Dim strPDFLocation = strFileLocation + "\" + Date.Now.ToString("dd-MM-yyyy hh mm ss") + ".pdf"
                Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                If Not folderexists Then
                    Directory.CreateDirectory(strFileLocation)
                End If


                If Not pdfgexists Then
                    File.WriteAllBytes(strPDFLocation, Convert.FromBase64String(jsonResulttodict("Manifest")))
                End If

                XSupport.Warning("Επιτυχής οριστικοποίηση λίστας")

            Else
                Dim Errors = jsonResulttodict.Item("Errors")
                Dim ErrorMessage = ""
                For Each item As JObject In Errors
                    ErrorMessage = If(ErrorMessage = "", item("Message").ToString, vbCrLf + item("Message").ToString)
                Next
                XSupport.Warning(ErrorMessage.ToString)
            End If

        Else
            XSupport.Warning("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If

        notif.Visible = False
    End Sub

    Private Function SendRequest(uri As Uri, jsonDataBytes As Byte(), contentType As String, method As String, apikey As String) As String
        Dim response As String
        Dim request As WebRequest

        request = WebRequest.Create(uri)
        request.ContentLength = jsonDataBytes.Length
        request.ContentType = contentType
        request.Headers.Add("AcsApiKey", apikey)
        request.Method = method

        Using requestStream = request.GetRequestStream

            requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
            requestStream.Close()

            Using responseStream = request.GetResponse.GetResponseStream

                Using reader As New StreamReader(responseStream)

                    response = reader.ReadToEnd()

                End Using

            End Using

        End Using

        Return response

    End Function

    Private Function SendCCRequest(uri As Uri, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim response As String
        Dim request As WebRequest

        request = WebRequest.Create(uri)
        request.ContentLength = jsonDataBytes.Length
        request.ContentType = contentType
        request.Method = method

        Using requestStream = request.GetRequestStream

            requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
            requestStream.Close()

            Using responseStream = request.GetResponse.GetResponseStream

                Using reader As New StreamReader(responseStream)

                    response = reader.ReadToEnd()

                End Using

            End Using

        End Using

        Return response

    End Function

    Private Sub CloseVoucher_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim ACSstr As String = "SELECT COURIERCOMPANY FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=1 AND COMPANY=" + company.ToString
        Dim Genikistr As String = "SELECT COURIERCOMPANY FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=2 AND COMPANY=" + company.ToString
        Dim CCstr As String = "SELECT COURIERCOMPANY FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=3 AND COMPANY=" + company.ToString
        Dim ACSds As XTable = XSupport.GetSQLDataSet(ACSstr)
        Dim Genikids As XTable = XSupport.GetSQLDataSet(Genikistr)
        Dim CCds As XTable = XSupport.GetSQLDataSet(CCstr)
        If Genikids.Count < 1 Then
            TabControl1.TabPages.Remove(TabGeniki)
        End If
        If ACSds.Count < 1 Then
            TabControl1.TabPages.Remove(TabACS)
        End If
        If CCds.Count < 1 Then
            TabControl1.TabPages.Remove(TabCC)
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim str As String = "SELECT FOLDERPATH FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=1 AND COMPANY=" + company.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)
        Dim credentials As XRow = ds.Current
        Dim folderpath = credentials("FOLDERPATH")
        If ds.Count > 0 Then
            Dim ClosingDate = Date.Today
            Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
            If Directory.Exists(strFileLocation) Then
                Process.Start(strFileLocation)
            Else
                XSupport.Warning("Δεν βρέθηκαν σημερινές οριστικοποιημένες λίστες")
            End If

        Else
                XSupport.Warning("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim str As String = "SELECT FOLDERPATH FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=2 AND COMPANY=" + company.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)
        Dim credentials As XRow = ds.Current
        Dim folderpath = credentials("FOLDERPATH")
        If ds.Count > 0 Then
            Dim ClosingDate = Date.Today
            Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
            If Directory.Exists(strFileLocation) Then
                Process.Start(strFileLocation)
            Else
                XSupport.Warning("Δεν βρέθηκαν σημερινές οριστικοποιημένες λίστες")
            End If

        Else
            XSupport.Warning("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Dim company = XSupport.ConnectionInfo.CompanyId
        Dim str As String = "SELECT FOLDERPATH FROM CCCD1COURIERCONFIG WHERE COURIERCOMPANY=3 AND COMPANY=" + company.ToString
        Dim ds As XTable = XSupport.GetSQLDataSet(str)
        Dim credentials As XRow = ds.Current
        Dim folderpath = credentials("FOLDERPATH")
        If ds.Count > 0 Then
            Dim ClosingDate = Date.Today
            Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
            If Directory.Exists(strFileLocation) Then
                Process.Start(strFileLocation)
            Else
                XSupport.Warning("Δεν βρέθηκαν σημερινές οριστικοποιημένες λίστες")
            End If

        Else
            XSupport.Warning("Σφάλμα Παραμετροποίησης." + vbCrLf + "Επικοινωνήστε με την Dayone!")
        End If
    End Sub

End Class
