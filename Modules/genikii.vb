Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Web
Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports Softone

Module geniki
    Public ErrorCode = New Dictionary(Of Integer, String) From {
       {0, "Ok"},
       {1, "Authentication failed"},
       {2, "Not implemented"},
       {3, "No data "},
       {4, "Invalid operation"},
       {5, "Max voucher No. reached"},
       {6, "Max subvoucher No. reached"},
       {700, "Validation failed"},
       {701, "Validation failed"},
       {703, "Validation failed"},
       {704, "Validation failed"},
       {705, "Validation failed"},
       {706, "Validation failed"},
       {8, "SQL error"},
       {9, "Doesn't exist "},
       {10, "Not authorized "},
       {11, "Invalid key"},
       {12, "Run-time error"},
       {13, "Job canceled "},
       {14, "Server busy "},
       {15, "Request limit reached"}
    }

    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
        Dim Cod_Ammount
        Dim GenServices
        Dim GenIServices
        If vItsCod Then
            salData("CCCD1VOUCHERVALUE") = If(salData("CCCD1VOUCHERVALUE") > 0.0, salData("CCCD1VOUCHERVALUE"), salData("SUMAMNT"))
            Cod_Ammount = salData("CCCD1VOUCHERVALUE")
            GenServices = salData("CCCD1GENIKISERVICES")
            GenIServices = If(IsDBNull(GenServices), "αμ", If(GenServices.ToString.Split(",").Contains("αμ") Or GenServices.ToString.Split(",").Contains("αν"), GenServices, GenServices + ",αμ"))
        Else
            Cod_Ammount = 0.0
            GenServices = salData("CCCD1GENIKISERVICES")
            GenIServices = If(IsDBNull(GenServices), "", GenServices)
        End If

        Dim InsAmmount = salData("CCCD1INSAMMOUNT")
        If GenIServices.ToString.Split(",").Contains("ασ") Then
            If InsAmmount > 0.0 Then
                InsAmmount = If(InsAmmount > 3000.0, 3000.0, InsAmmount)
            Else
                XSupport.Exception("Πρέπει να εισάγετε ποσό ασφάλισης μεγαλύτερο απο 0")
            End If
        Else
            InsAmmount = 0.0
        End If

        Dim Name = If(IsDBNull(salData("CCCD1SHIPNAME")), "", salData("CCCD1SHIPNAME"))
        Dim Telephone = If(IsDBNull(salData("CCCD1SHIPCELLPHONE")), "", salData("CCCD1SHIPCELLPHONE"))
        Dim Address = If(IsDBNull(salData("CCCD1SHIPADDRESS")), "", salData("CCCD1SHIPADDRESS"))
        Dim Zip = If(IsDBNull(salData("CCCD1SHIPZIP")), "", salData("CCCD1SHIPZIP"))
        Dim City = If(IsDBNull(salData("CCCD1SHIPCITY")), "", salData("CCCD1SHIPCITY"))
        Dim Comments = If(IsDBNull(salData("CCCD1VOUCHERCOMMENTS")), "", salData("CCCD1VOUCHERCOMMENTS"))
        Dim pickupdate = If(IsDBNull(salData("CCCD1VOUCHERDATE")), Date.Today, salData("CCCD1VOUCHERDATE"))

        Dim Pieces = If(salData("CCCD1VOUCHERQUANTITY").ToString = "" Or salData("CCCD1VOUCHERQUANTITY") < 1, 1, salData("CCCD1VOUCHERQUANTITY"))
        Dim WeightType = credentials("WEIGHTTYPE")
        Dim Weight = 0.0
        If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
            Dim IteTable As XTable = XModule.GetTable("ITELINES")
            If IteTable.Count > 0 Then
                For i = 0 To IteTable.Count - 1
                    Dim IteWeight = If(IteTable(i, "WEIGHT").ToString = "", 0.0, IteTable(i, "WEIGHT"))
                    Weight = +IteWeight
                Next

                Weight = If(Weight < 0.5, 0.5, Weight)

            Else
                XSupport.Warning("Το παραστατικό δεν έχει είδη")
            End If
        Else
            Weight = If(salData("CCCD1VOUVHERWEIGHT").ToString = "" Or salData("CCCD1VOUVHERWEIGHT") < 0.5, 0.5, salData("CCCD1VOUVHERWEIGHT"))
        End If


        Dim services As Object
        Dim Authresults As Object
        Dim voucher As Object
        Dim result

        If credentials("TESTENVIROMENT") = 1 Then
            services = New Web.taxydromiki.test.JobServicesV2
            Authresults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
        Else
            services = New Web.taxydromiki.JobServicesV2
            Authresults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
        End If

        If (Authresults.Result = 0) Then

            If credentials("TESTENVIROMENT") = 1 Then
                voucher = New Web.taxydromiki.test.Record
            Else
                voucher = New Web.taxydromiki.Record
            End If

            voucher.Name = Name
            voucher.Address = Address
            voucher.City = City
            voucher.Telephone = Telephone
            voucher.Zip = Zip
            voucher.Comments = Comments
            voucher.Weight = Weight
            voucher.Pieces = Pieces
            voucher.Services = GenIServices
            voucher.CodAmount = Cod_Ammount
            voucher.InsAmount = InsAmmount
            voucher.ReceivedDate = pickupdate

            If credentials("TESTENVIROMENT") = 1 Then
                result = CType(services.CreateJob(Authresults.Key, voucher, Web.taxydromiki.test.JobType.Voucher), Web.taxydromiki.test.CreateJobResult)
            Else
                result = CType(services.CreateJob(Authresults.Key, voucher, Web.taxydromiki.JobType.Voucher), Web.taxydromiki.CreateJobResult)
            End If

            If result.Result = 0 Then

                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then 'Αν έχει ήδη άλλα Subvouchers
                    While SubvTable.Count > 0
                        SubvTable.Current.Delete() 'Τα διαγραφω γραμμη γραμμη
                    End While

                End If

                If Pieces > 1 Then
                    Dim SubRow As XRow = SubvTable.Current
                    For Each item In result.SubVouchers
                        SubRow.Insert()
                        SubRow("VOUCHER") = item.VoucherNo
                        SubRow.Post()
                    Next
                End If

                salData("CCCD1VOUCHERNO") = result.Voucher
                salData("CCCD1GENIKIJOBID") = result.JobId.ToString
                salData("CCCD1GENIKISERVICES") = GenIServices
                salData("CCCD1VOUCHERVALUE") = Cod_Ammount
                salData("CCCD1VOUVHERWEIGHT") = Weight
                salData("CCCD1VOUCHERQUANTITY") = Pieces
                salData("CCCD1VOUCHERDATE") = pickupdate
                salData("CCCD1VOUCHEREXECUTION") = 1
                salData("CCCD1VOUCHERDELETED") = 0
                salData("CCCD1VOUCHERPRINTED") = 0
            Else
                XSupport.Exception(ErrorCode(result.Result) + vbCrLf + "Error Γενική: " + result.Result.ToString)
            End If
        Else
            XSupport.Exception(ErrorCode(Authresults.Result) + vbCrLf + "Error Γενική: " + Authresults.Result.ToString)
        End If
        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vUsername = credentials("USERNAME")
        Dim vPassword = credentials("PASSWORD")
        Dim vPrintType = If(credentials("VOUCHERPRINTTYPE") = 1, "Flyer", "Sticker")
        Dim vApikey = credentials("APIKEY")
        Dim folderpath = credentials("FOLDERPATH")
        Dim ClosingDate As Date = salData("CCCD1VOUCHERDATE")

        Dim services As Object
        Dim AuthResults As Object


        If credentials("TESTENVIROMENT") = 1 Then
            services = New Web.taxydromiki.test.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
        Else
            services = New Web.taxydromiki.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
        End If


        If (Authresults.Result = 0) Then
            Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
            Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1VOUCHERNO").ToString + ".pdf"
            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
            If Not folderexists Then
                Directory.CreateDirectory(strFileLocation)
            End If
            If Not pdfgexists Then
                Dim voucherNumbers = "&voucherNumbers=" + salData("CCCD1VOUCHERNO").ToString
                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then 'Αν έχει Subvouchers
                    For k As Integer = 0 To SubvTable.Count - 1
                        voucherNumbers = voucherNumbers + "&voucherNumbers=" + SubvTable(k, "VOUCHER")
                    Next
                End If
                Dim encoded As String
                If credentials("TESTENVIROMENT") = 1 Then
                    encoded = "https://testvoucher.taxydromiki.gr/JobServicesV2.asmx/GetVouchersPdf?authKey=" + HttpUtility.UrlEncode(AuthResults.Key) + voucherNumbers + "&Format=" + vPrintType + "&extraInfoFormat=None"
                Else
                    encoded = "https://voucher.taxydromiki.gr/JobServicesV2.asmx/GetVouchersPdf?authKey=" + HttpUtility.UrlEncode(AuthResults.Key) + voucherNumbers + "&Format=" + vPrintType + "&extraInfoFormat=None"
                End If
                Dim vUrl = New Uri(encoded)
                Dim WC As New WebClient
                ServicePointManager.Expect100Continue = True
                ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)  'its same like SecurityProtocolType.Tls12
                ServicePointManager.DefaultConnectionLimit = 9999
                WC.DownloadFile(vUrl, strPDFLocation)
            End If


            If credentials("INSTANTPRINT") = 1 Then

                If credentials("CUSTOMTEMPLATE") = 0 Then
                    Dim settings As New PrinterSettings
                    Dim psi As New ProcessStartInfo

                    settings.PrinterName = Chr(34) + credentials("PRINTER") + Chr(34)

                    psi.Verb = "printTo"
                    psi.Arguments = settings.PrinterName.ToString()
                    psi.UseShellExecute = True
                    psi.WindowStyle = ProcessWindowStyle.Hidden
                    psi.FileName = strPDFLocation

                    Process.Start(psi)
                Else
                    Dim form = credentials("TEMPLATE")
                    XModule.PrintForm(form, credentials("PRINTER"), "")
                End If
            End If
            salData("CCCD1VOUCHERPRINTED") = 1
            XSupport.Warning("Ολοκλήρωση εκτύπωσης")
        Else
            XSupport.Exception(geniki.ErrorCode(AuthResults.Result) + vbCrLf + "Error Γενική: " + AuthResults.Result.ToString)
        End If
        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim services As Object
        Dim AuthResults As Object
        If credentials("TESTENVIROMENT") = 1 Then
            services = New Web.taxydromiki.test.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
        Else
            services = New Web.taxydromiki.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
        End If
        If AuthResults.Result = 0 Then
            Dim vJobId = saldata("CCCD1GENIKIJOBID")
            Dim delResults = services.CancelJob(AuthResults.Key, Convert.ToInt64(vJobId), True)
            If delResults = 0 Then
                saldata("CCCD1VOUCHERDELETED") = 1
                saldata("CCCD1VOUCHEREXECUTION") = 0
                saldata("CCCD1VOUCHERPRINTED") = 0
                saldata("CCCD1VOUCHERNO") = ""
                saldata("CCCD1VOUCHERVALUE") = 0.0
                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then
                    While SubvTable.Count > 0
                        SubvTable.Current.Delete()
                    End While
                End If
                XSupport.Warning("Ολοκλήρωση ακύρωσης")
            Else
                XSupport.Exception(ErrorCode(delResults) + vbCrLf + "Error:" + delResults.ToString)
            End If
        Else
            XSupport.Exception(ErrorCode(AuthResults.Result) + vbCrLf + "Error Γενική: " + AuthResults.Result.ToString)
        End If
        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim apikey = credentials("APIKEY")

        Dim services As Object
        Dim AuthResults As Object
        Dim TrackAndTrace As Object
        If credentials("TESTENVIROMENT") = 1 Then
            services = New Web.taxydromiki.test.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
        Else
            services = New Web.taxydromiki.JobServicesV2
            AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
        End If

        If AuthResults.Result = 0 Then
            If credentials("TESTENVIROMENT") = 1 Then
                TrackAndTrace = CType(services.TrackAndTrace(AuthResults.Key, saldata("CCCD1VOUCHERNO"), "el"), Web.taxydromiki.test.TrackAndTraceResult)
            Else
                TrackAndTrace = CType(services.TrackAndTrace(AuthResults.Key, saldata("CCCD1VOUCHERNO"), "el"), Web.taxydromiki.TrackAndTraceResult)
            End If
            Dim Trform = New TrackingForm()
            If TrackAndTrace.Result = 0 Then
                Dim counter As Integer = 0
                For Each item In TrackAndTrace.Checkpoints
                    counter += 1
                    If counter = TrackAndTrace.Checkpoints.Length Then
                        Dim x As String() = {item.StatusDate, item.Status, item.Shop, TrackAndTrace.Status}
                        Trform.DataGridView1.Rows.Add(x)
                    Else
                        Dim x As String() = {item.StatusDate, item.Status, item.Shop, ""}
                        Trform.DataGridView1.Rows.Add(x)
                    End If
                Next
                Trform.Show()
            Else
                XSupport.Exception(ErrorCode(TrackAndTrace.Result) + vbCrLf + "Error Γενική: " + TrackAndTrace.Result.ToString)
            End If
        Else
            XSupport.Exception(ErrorCode(AuthResults.Result) + vbCrLf + "Error Γενική: " + AuthResults.Result.ToString)
        End If
        Return 0
    End Function

    Public Function GetMassVoucher11700011(ds As XTable, i As Integer, Success_list As List(Of String), Error_list_Messages As List(Of String), Error_list_Fincode As List(Of String), Error_list_Findoc As List(Of String), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=2 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Error_list_Messages.Add("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
        Else

            Dim credentials As XRow = credentialsTable.Current

            Dim Cod_Ammount
            Dim GenServices
            Dim GenIServices

            Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(ds.Item(i, "PAYMENT").ToString)
            If vItsCod Then
                Cod_Ammount = If(ds.Item(i, "CCCD1VOUCHERVALUE") > 0.0, ds.Item(i, "CCCD1VOUCHERVALUE"), ds.Item(i, "SUMAMNT"))
                GenServices = ds(i, "CCCD1GENIKISERVICES")
                GenIServices = If(IsDBNull(GenServices), "αμ", If(GenServices.ToString.Split(",").Contains("αμ") Or GenServices.ToString.Split(",").Contains("αν"), GenServices, GenServices + ",αμ"))
            Else
                Cod_Ammount = 0.0
                GenServices = ds(i, "CCCD1GENIKISERVICES")
                GenIServices = If(IsDBNull(GenServices), "", GenServices)
            End If

            Dim InsAmmount = ds(i, "CCCD1INSAMMOUNT")
            If GenIServices.ToString.Split(",").Contains("ασ") Then
                If InsAmmount > 0.0 Then
                    InsAmmount = If(InsAmmount > 3000.0, 3000.0, InsAmmount)
                Else
                    Error_list_Messages.Add("Ποσό ασφάλειας μεγαλυτερο απο 0.00")
                    Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                    Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                    Return 0
                End If
            Else
                InsAmmount = 0.0
            End If

            Dim Name = If(IsDBNull(ds(i, "CCCD1SHIPNAME")), "", ds(i, "CCCD1SHIPNAME"))
            Dim Telephone = If(IsDBNull(ds(i, "CCCD1SHIPCELLPHONE")), "", ds(i, "CCCD1SHIPCELLPHONE"))
            Dim Address = If(IsDBNull(ds(i, "CCCD1SHIPADDRESS")), "", ds(i, "CCCD1SHIPADDRESS"))
            Dim Zip = If(IsDBNull(ds(i, "CCCD1SHIPZIP")), "", ds(i, "CCCD1SHIPZIP"))
            Dim City = If(IsDBNull(ds(i, "CCCD1SHIPCITY")), "", ds(i, "CCCD1SHIPCITY"))
            Dim Comments = If(IsDBNull(ds(i, "CCCD1VOUCHERCOMMENTS")), "", ds(i, "CCCD1VOUCHERCOMMENTS"))
            Dim pickupdate = If(IsDBNull(ds(i, "CCCD1VOUCHERDATE")), Date.Today, ds(i, "CCCD1VOUCHERDATE"))

            Dim Pieces = If(ds(i, "CCCD1VOUCHERQUANTITY").ToString = "" Or ds(i, "CCCD1VOUCHERQUANTITY") < 1, 1, ds(i, "CCCD1VOUCHERQUANTITY"))
            Dim WeightType = credentials("WEIGHTTYPE")
            Dim Weight = 0.0
            If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
                Dim IteTable As XTable = XSupport.GetSQLDataSet("SELECT SUM(WEIGHT,0) SUM_WEIGHT FROM MTRLINES WHERE FINDOC=" + ds(i, "FINDOC").ToString())
                Weight = If(IteTable.Current("SUM_WEIGHT") < 0.5, 0.5, IteTable.Current("SUM_WEIGHT"))
            Else
                Weight = If(ds(i, "CCCD1VOUVHERWEIGHT").ToString = "" Or ds(i, "CCCD1VOUVHERWEIGHT") < 0.5, 0.5, ds(i, "CCCD1VOUVHERWEIGHT"))
            End If

            Dim services As Object
            Dim AuthResults As Object
            Dim voucher As Object
            Dim result As Object

            If credentials("TESTENVIROMENT") = 1 Then
                services = New Web.taxydromiki.test.JobServicesV2
                AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
            Else
                services = New Web.taxydromiki.JobServicesV2
                AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
            End If

            If (AuthResults.Result = 0) Then
                If credentials("TESTENVIROMENT") = 1 Then
                    voucher = New Web.taxydromiki.test.Record
                Else
                    voucher = New Web.taxydromiki.Record
                End If
                voucher.Name = Name
                voucher.Address = Address
                voucher.City = City
                voucher.Telephone = Telephone
                voucher.Zip = Zip
                voucher.Comments = Comments
                voucher.Weight = Weight
                voucher.Pieces = Pieces
                voucher.Services = GenIServices
                voucher.CodAmount = Cod_Ammount
                voucher.InsAmount = InsAmmount
                voucher.ReceivedDate = pickupdate

                If credentials("TESTENVIROMENT") = 1 Then
                    result = CType(services.CreateJob(AuthResults.Key, voucher, Web.taxydromiki.test.JobType.Voucher), Web.taxydromiki.test.CreateJobResult)
                Else
                    result = CType(services.CreateJob(AuthResults.Key, voucher, Web.taxydromiki.JobType.Voucher), Web.taxydromiki.CreateJobResult)
                End If

                If result.Result = 0 Then

                    Dim DelSubs As String = ("DELETE FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                    XSupport.ExecuteSQL(DelSubs)

                    If Pieces > 1 Then
                        For Each item In result.SubVouchers
                            Dim vInsSub As String = "INSERT INTO CCCD1SUBVOUCHERS (VOUCHER, FINDOC) " +
                                                "VALUES ('" + item.VoucherNo + "'," + ds(i, "FINDOC").ToString + ")"
                            XSupport.ExecuteSQL(vInsSub)
                        Next
                    End If

                    Dim updatestr As String = "UPDATE FINDOC " +
                                            "SET CCCD1VOUCHERNO='" + result.Voucher.ToString + "', " +
                                            "CCCD1GENIKIJOBID='" + result.JobId.ToString + "', " +
                                            "CCCD1GENIKISERVICES='" + GenIServices.ToString + "', " +
                                            "CCCD1VOUCHERVALUE=" + Cod_Ammount.ToString.Replace(",", ".") + ", " +
                                            "CCCD1INSAMMOUNT=" + InsAmmount.ToString.Replace(",", ".") + ", " +
                                            "CCCD1VOUVHERWEIGHT=" + Weight.ToString.Replace(",", ".") + ", " +
                                            "CCCD1VOUCHERQUANTITY=" + Pieces.ToString + ", " +
                                            "CCCD1VOUCHERDATE='" + String.Format("{0:yyyMMdd}", pickupdate) + "', " +
                                            "CCCD1VOUCHEREXECUTION= 1, " +
                                            "CCCD1VOUCHERDELETED= 0, " +
                                            "CCCD1VOUCHERPRINTED= 0 " +
                                            "WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString

                    XSupport.ExecuteSQL(updatestr)
                    Success_list.Add(ds.Item(i, "FINCODE").ToString + " : " + result.Voucher.ToString)
                Else
                    Error_list_Messages.Add(geniki.ErrorCode(result.Result))
                    Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                    Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                End If
            Else
                Error_list_Messages.Add(geniki.ErrorCode(AuthResults.Result))
                Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
            End If
        End If
        Return 0
    End Function

    Public Function PrintMassVoucher11700012(voucherList As List(Of List(Of String)), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=2 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", XSupport.ConnectionInfo.CompanyId, voucherList(0)(2))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + voucherList(0)(2))
        Else
            Dim credentials = credentialsTable.Current

            Dim vUsername = credentials("USERNAME")
            Dim vPassword = credentials("PASSWORD")
            Dim vPrintType = If(credentials("VOUCHERPRINTTYPE") = 1, "Flyer", "Sticker")
            Dim vApikey = credentials("APIKEY")
            Dim folderpath = credentials("FOLDERPATH")
            Dim ClosingDate As Date = Date.Now

            Dim extraInfo = If(credentials("GENIKEXTRAINFOFORMAT") = 0, "None", "ThreeW")


            Dim services As Object
            Dim AuthResults As Object


            If credentials("TESTENVIROMENT") = 1 Then
                services = New Web.taxydromiki.test.JobServicesV2
                AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
            Else
                services = New Web.taxydromiki.JobServicesV2
                AuthResults = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
            End If


            If (AuthResults.Result = 0) Then
                Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
                Dim strPDFLocation = strFileLocation + "\AllDay.pdf"
                Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                If Not folderexists Then
                    Directory.CreateDirectory(strFileLocation)
                End If
                If pdfgexists Then
                    File.Delete(strPDFLocation)
                End If
                Dim findocIn As String = ""
                Dim voucherNumbers As String = ""
                Dim Counter As Integer = 0
                For Each voucher In voucherList
                    Counter += 1
                    If Counter = voucherList.Count Then
                        findocIn += voucher(1)
                    Else
                        findocIn += voucher(1) + ","
                    End If
                    voucherNumbers += "&voucherNumbers=" + voucher(0)
                    Dim lines = XSupport.GetSQLDataSet("SELECT VOUCHER FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + voucher(1) + " ORDER BY CCCD1SUBVOUCHERS")
                    If lines.Count > 0 Then
                        For k As Integer = 0 To lines.Count - 1
                            voucherNumbers += "&voucherNumbers=" + lines(k, "VOUCHER")
                        Next
                    End If
                Next
                Dim encoded As String
                If credentials("TESTENVIROMENT") = 1 Then
                    encoded = "https://testvoucher.taxydromiki.gr/JobServicesV2.asmx/GetVouchersPdf?authKey=" + HttpUtility.UrlEncode(AuthResults.Key) + voucherNumbers + "&Format=" + vPrintType + "&extraInfoFormat=" + extraInfo
                Else
                    encoded = "https://voucher.taxydromiki.gr/JobServicesV2.asmx/GetVouchersPdf?authKey=" + HttpUtility.UrlEncode(AuthResults.Key) + voucherNumbers + "&Format=" + vPrintType + "&extraInfoFormat=" + extraInfo
                End If

                Dim vUrl = New Uri(encoded)
                Dim WC As New WebClient
                ServicePointManager.Expect100Continue = True
                ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)  'its same like SecurityProtocolType.Tls12
                ServicePointManager.DefaultConnectionLimit = 9999
                WC.DownloadFile(vUrl, strPDFLocation)


                If credentials("INSTANTPRINT") = 1 Then

                    Dim settings As New PrinterSettings
                    Dim psi As New ProcessStartInfo

                    settings.PrinterName = Chr(34) + credentials("PRINTER") + Chr(34)

                    psi.Verb = "printTo"
                    psi.Arguments = settings.PrinterName.ToString()
                    psi.UseShellExecute = True
                    psi.WindowStyle = ProcessWindowStyle.Hidden
                    psi.FileName = strPDFLocation

                    Process.Start(psi)

                End If
                Dim updatestr As String = "UPDATE FINDOC " +
                                      "SET CCCD1VOUCHERPRINTED=1" +
                                      "WHERE FINDOC IN (" + findocIn + ")"

                XSupport.ExecuteSQL(updatestr)
                XSupport.Warning("Ολοκλήρωση εκτύπωσης")
            End If
        End If
        Return 0
    End Function

    Public Function FinalizeVoucher(credentials As XRow, data As XRow, XSupport As XSupport)
        Dim ClosingDate As Date = data("CLOSINGDATE")
        Dim folderpath = credentials("FOLDERPATH")
        Dim services As Object
        Dim AuthResult As Object


        If credentials("TESTENVIROMENT") = 1 Then
            services = New Web.taxydromiki.test.JobServicesV2
            AuthResult = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.test.AuthenticateResult)
        Else
            services = New Web.taxydromiki.JobServicesV2
            AuthResult = CType(services.Authenticate(credentials("USERNAME"), credentials("PASSWORD"), credentials("APIKEY")), Web.taxydromiki.AuthenticateResult)
        End If


        Dim delResults = services.ClosePendingJobs(AuthResult.Key)
        If AuthResult.Result = 0 Then
            If delResults = 0 Then
                Dim ans = XSupport.AskYesNoCancel("Επιτυχής οριστικοποίησης", "Θέλετε να προχωρήσετε σε δημιουργία αρχείου excel;")
                If ans = 6 Then

                    Dim xlApp As New Excel.Application()
                    If xlApp Is Nothing Then
                        Throw New Exception("Excel is not properly installed!!")
                    End If

                    Dim str = "SELECT DISTINCT " +
                          "F.CCCD1VOUCHERNO, " +
                          "F.FINCODE, " +
                          "F.CCCD1SHIPNAME, " +
                          "F.CCCD1SHIPADDRESS, " +
                          "F.CCCD1SHIPZIP, " +
                          "F.CCCD1SHIPCITY, " +
                          "F.CCCD1SHIPCELLPHONE, " +
                          "F.CCCD1VOUCHERVALUE, " +
                          "(SELECT P.NAME FROM PAYMENT P WHERE P.COMPANY=F.COMPANY AND P.SODTYPE=13 AND P.PAYMENT=F.PAYMENT) AS PAYMENT,  " +
                          "F.CCCD1VOUCHERQUANTITY " +
                          "FROM FINDOC F " +
                          "WHERE F.CCCD1COURIERCOMPANY=2 " +
                          "AND F.COMPANY=" + XSupport.ConnectionInfo.CompanyId.ToString + " " +
                          "AND CCCD1VOUCHERNO!='' " +
                          "AND F.CCCD1VOUCHERDELETED=0 " +
                          "AND F.CCCD1VOUCHERDATE='" + ClosingDate.ToString("yyyyMMdd") + "'" +
                          "AND F.SERIES IN (" + credentials("SERIES") + ")"
                    Dim ds = XSupport.GetSQLDataSet(str)
                    If ds.Count > 0 Then

                        Dim strFileLocation = folderpath + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
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
                        data("EXECUTIONOK") = 1
                        data("FILELOCATION") = strFileLocation
                        XSupport.Warning("Επιτυχής δημιουργία αρχείου")
                    Else
                        XSupport.Warning("Δεν βρέθηκαν παραστατικά για δημιουργία αρχείου excel.")
                    End If
                End If
            Else
                XSupport.Exception(geniki.ErrorCode(delResults))
            End If
        Else
            XSupport.Exception(geniki.ErrorCode(AuthResult.Result))
        End If

        Return 0
    End Function
End Module