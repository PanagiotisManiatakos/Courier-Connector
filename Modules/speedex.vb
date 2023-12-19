Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Runtime.Remoting.Messaging
Imports System.Web
Imports Newtonsoft.Json
Imports Softone

Module speedex

    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim speedexAccessPoint As Object
        If credentials("TESTENVIROMENT") = 1 Then
            speedexAccessPoint = New Web.speedex.test.AccessPoint()
        Else
            speedexAccessPoint = New Web.speedex.AccessPoint()
        End If

        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim returnCode As Integer
        Dim returnMessage As String = ""

        Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)

        If (returnCode = 1) Then
            Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
            Dim Cod_Ammount

            If vItsCod Then
                salData("CCCD1VOUCHERVALUE") = If(salData("CCCD1VOUCHERVALUE") > 0.0, salData("CCCD1VOUCHERVALUE"), salData("SUMAMNT"))
                Cod_Ammount = salData("CCCD1VOUCHERVALUE")
            Else
                Cod_Ammount = 0.0
            End If

            Dim InsAmmount = salData("CCCD1INSAMMOUNT")
            If InsAmmount > 0.0 Then
                InsAmmount = Convert.ToInt32(InsAmmount)
            Else
                InsAmmount = 0
            End If

            Dim Name = If(IsDBNull(salData("CCCD1SHIPNAME")), "", salData("CCCD1SHIPNAME"))
            Dim Telephone = If(IsDBNull(salData("CCCD1SHIPCELLPHONE")), "", salData("CCCD1SHIPCELLPHONE"))
            Dim Address = If(IsDBNull(salData("CCCD1SHIPADDRESS")), "", salData("CCCD1SHIPADDRESS"))
            Dim Zip = If(IsDBNull(salData("CCCD1SHIPZIP")), "", salData("CCCD1SHIPZIP"))
            Dim City = If(IsDBNull(salData("CCCD1SHIPCITY")), "", salData("CCCD1SHIPCITY"))
            Dim Comments = If(IsDBNull(salData("CCCD1VOUCHERCOMMENTS")), "", salData("CCCD1VOUCHERCOMMENTS"))
            Dim payCodeFlag = If(IsDBNull(salData("CCCD1SPEEDEXCHARGE")), 1, salData("CCCD1SPEEDEXCHARGE"))
            Dim isCheque = If(IsDBNull(salData("CCCD1SPEEDEXCHEQUE")), "M", If(salData("CCCD1SPEEDEXCHEQUE") = 1, "E", "M"))
            Dim isSaturday = If(IsDBNull(salData("CCCD1SPEEDEXALLOWSATURDAY")), 0, salData("CCCD1SPEEDEXALLOWSATURDAY"))
            Dim chunkSize As Integer = 40
            Dim chunks As New List(Of String)()
            For i As Integer = 0 To Comments.Length - 1 Step chunkSize
                Dim chunk As String = Comments.Substring(i, Math.Min(chunkSize, Comments.Length - i))
                chunks.Add(chunk)
            Next
            Dim par1 = ""
            Dim par2 = ""
            Dim par3 = ""
            Dim counter = 1
            For Each ch In chunks
                If counter = 1 Then
                    par1 = ch
                ElseIf counter = 2 Then
                    par2 = ch
                ElseIf counter = 3 Then
                    par3 = ch
                End If
                counter = counter + 1
            Next

            Dim Items = If(salData("CCCD1VOUCHERQUANTITY").ToString = "" Or salData("CCCD1VOUCHERQUANTITY") < 1, 1, salData("CCCD1VOUCHERQUANTITY"))
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

            Dim voucher As Object
            If credentials("TESTENVIROMENT") = 1 Then
                voucher = New Web.speedex.test.BOL
            Else
                voucher = New Web.speedex.BOL
            End If

            voucher._cust_Flag = 0
            voucher.Items = Convert.ToInt32(Items)
            voucher.PayCode_Flag = payCodeFlag
            voucher.Pod_Amount_Description = isCheque
            voucher.Saturday_Delivery = isSaturday
            voucher.Pod_Amount_Cash = Cod_Ammount
            voucher.RCV_Addr1 = Address
            voucher.RCV_Country = "GR"
            voucher.RCV_Name = Name
            voucher.RCV_Tel1 = Telephone
            voucher.RCV_Zip_Code = Zip
            voucher.Security_Value = InsAmmount
            voucher.Snd_agreement_id = credentials("SPEEDEXAGRID")
            voucher.SND_Customer_Id = credentials("SPEEDEXCUSID")
            voucher.Voucher_Weight = Weight
            voucher.Paratiriseis_2853_1 = par1
            voucher.Paratiriseis_2853_2 = par2
            voucher.Paratiriseis_2853_3 = par3


            Dim prodList() As Object
            If credentials("TESTENVIROMENT") = 1 Then
                prodList = CType(Array.CreateInstance(GetType(Web.speedex.test.BOL), 1), Web.speedex.test.BOL())
            Else
                prodList = CType(Array.CreateInstance(GetType(Web.speedex.BOL), 1), Web.speedex.BOL())
            End If

            Dim tableFlag As Integer
            Dim statusList() As String
            prodList(0) = voucher
            Dim CreateVoucher = speedexAccessPoint.CreateBOL(SessionID, prodList, tableFlag, statusList, returnCode, returnMessage)

            If returnCode = 1 Then

                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then
                    While SubvTable.Count > 0
                        SubvTable.Current.Delete()
                    End While

                End If

                Dim SubRow As XRow = SubvTable.Current
                counter = 1
                For Each item In CreateVoucher
                    If counter = 1 Then
                        salData("CCCD1VOUCHERNO") = item.voucher_code
                        salData("CCCD1SPEEDEXCHARGE") = If(payCodeFlag = 1, 1, If(payCodeFlag = 2, 2, 3))
                        salData("CCCD1VOUCHERVALUE") = Cod_Ammount
                        salData("CCCD1VOUVHERWEIGHT") = Weight
                        salData("CCCD1VOUCHERQUANTITY") = Items
                        salData("CCCD1VOUCHEREXECUTION") = 1
                        salData("CCCD1VOUCHERDELETED") = 0
                        salData("CCCD1VOUCHERPRINTED") = 0
                        salData("CCCD1VOUCHERDATE") = Date.Today
                    Else
                        SubRow.Insert()
                        SubRow("VOUCHER") = item.voucher_code
                        SubRow.Post()
                    End If
                    counter = counter + 1
                Next
            Else
                XSupport.Exception("Error Speedex: " + vbCrLf + statusList(0))
            End If

        Else
            XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
        End If

        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vUsername = credentials("USERNAME")
        Dim vPassword = credentials("PASSWORD")
        Dim vPrintType = credentials("VOUCHERPRINTTYPE")
        Dim folderpath = credentials("FOLDERPATH")
        Dim ClosingDate As Date = salData("CCCD1VOUCHERDATE")

        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim returnCode As Integer
        Dim returnMessage As String = ""

        Dim speedexAccessPoint As Object
        If credentials("TESTENVIROMENT") = 1 Then
            speedexAccessPoint = New Web.speedex.test.AccessPoint()
        Else
            speedexAccessPoint = New Web.speedex.AccessPoint()
        End If

        Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)
        If (returnCode = 1) Then
            Dim voucherIds() As String
            voucherIds = CType(Array.CreateInstance(GetType(String), 1), String())
            voucherIds(0) = salData("CCCD1VOUCHERNO").ToString
            Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
            If SubvTable.Count > 0 Then 'Αν έχει Subvouchers
                For k As Integer = 0 To SubvTable.Count - 1
                    voucherIds = voucherIds.Concat({SubvTable(k, "VOUCHER")}).ToArray()
                Next
            End If
            Dim pdf = speedexAccessPoint.GetBOLPdf(SessionID, voucherIds, False, vPrintType, returnCode, returnMessage)
            If (returnCode = 1) Then
                Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
                Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1VOUCHERNO").ToString + ".pdf"
                Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                If Not folderexists Then
                    Directory.CreateDirectory(strFileLocation)
                End If
                If Not pdfgexists Then
                    File.WriteAllBytes(strPDFLocation, pdf(0).pdf)
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
                XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
            End If
        Else
            XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)

        End If

        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim returnCode As Integer
        Dim returnMessage As String = ""

        Dim speedexAccessPoint As Object
        If credentials("TESTENVIROMENT") = 1 Then
            speedexAccessPoint = New Web.speedex.test.AccessPoint()
        Else
            speedexAccessPoint = New Web.speedex.AccessPoint()
        End If

        Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)
        If returnCode = 1 Then
            Dim vJobId = saldata("CCCD1GENIKIJOBID")
            returnCode = speedexAccessPoint.CancelBOL(SessionID, saldata("CCCD1VOUCHERNO"), returnMessage)
            If returnCode = 1 Then
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
                XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
            End If
        Else
            XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
        End If
        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim username = credentials("USERNAME")
        Dim password = credentials("PASSWORD")
        Dim returnCode As Integer
        Dim returnMessage As String = ""

        Dim speedexAccessPoint As Object
        If credentials("TESTENVIROMENT") = 1 Then
            speedexAccessPoint = New Web.speedex.test.AccessPoint()
        Else
            speedexAccessPoint = New Web.speedex.AccessPoint()
        End If

        Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)

        If returnCode = 1 Then
            Dim trace = speedexAccessPoint.GetTraceByVoucher(SessionID, saldata("CCCD1VOUCHERNO"), returnCode, returnMessage)
            Dim Trform = New TrackingForm()
            If returnCode = 1 Then
                For Each item In trace
                    Dim x As String() = {item.CheckpointDate, item.StatusDesc, item.Branch, item.SpeedexComments1}
                    Trform.DataGridView1.Rows.Add(x)
                Next
                Trform.Show()
            Else
                XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
            End If
        Else
            XSupport.Exception("Error Speedex: " + vbCrLf + returnMessage)
        End If
        Return 0
    End Function

    Public Function GetMassVoucher11700011(ds As XTable, i As Integer, Success_list As List(Of String), Error_list_Messages As List(Of String), Error_list_Fincode As List(Of String), Error_list_Findoc As List(Of String), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=3 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Error_list_Messages.Add("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
        Else

            Dim credentials As XRow = credentialsTable.Current

            Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(ds.Item(i, "PAYMENT").ToString)
            Dim Cod_Ammount

            If vItsCod Then
                Cod_Ammount = If(ds.Item(i, "CCCD1VOUCHERVALUE") > 0.0, ds.Item(i, "CCCD1VOUCHERVALUE"), ds.Item(i, "SUMAMNT"))
            Else
                Cod_Ammount = 0.0
            End If

            Dim InsAmmount = ds.Item(i, "CCCD1INSAMMOUNT")
            If InsAmmount > 0.0 Then
                InsAmmount = Convert.ToInt32(InsAmmount)
            Else
                InsAmmount = 0
            End If

            Dim Name = If(IsDBNull(ds.Item(i, "CCCD1SHIPNAME")), "", ds.Item(i, "CCCD1SHIPNAME"))
            Dim Telephone = If(IsDBNull(ds.Item(i, "CCCD1SHIPCELLPHONE")), "", ds.Item(i, "CCCD1SHIPCELLPHONE"))
            Dim Address = If(IsDBNull(ds.Item(i, "CCCD1SHIPADDRESS")), "", ds.Item(i, "CCCD1SHIPADDRESS"))
            Dim Zip = If(IsDBNull(ds.Item(i, "CCCD1SHIPZIP")), "", ds.Item(i, "CCCD1SHIPZIP"))
            Dim City = If(IsDBNull(ds.Item(i, "CCCD1SHIPCITY")), "", ds.Item(i, "CCCD1SHIPCITY"))
            Dim Comments = If(IsDBNull(ds.Item(i, "CCCD1VOUCHERCOMMENTS")), "", ds.Item(i, "CCCD1VOUCHERCOMMENTS"))
            Dim payCodeFlag = If(IsDBNull(ds.Item(i, "CCCD1SPEEDEXCHARGE")), 1, ds.Item(i, "CCCD1SPEEDEXCHARGE"))
            Dim isCheque = If(IsDBNull(ds.Item(i, "CCCD1SPEEDEXCHEQUE")), "M", If(ds.Item(i, "CCCD1SPEEDEXCHEQUE") = 1, "E", "M"))
            Dim isSaturday = If(IsDBNull(ds.Item(i, "CCCD1SPEEDEXALLOWSATURDAY")), 0, ds.Item(i, "CCCD1SPEEDEXALLOWSATURDAY"))
            Dim chunkSize As Integer = 40
            Dim chunks As New List(Of String)()
            For pp As Integer = 0 To Comments.Length - 1 Step chunkSize
                Dim chunk As String = Comments.Substring(pp, Math.Min(chunkSize, Comments.Length - pp))
                chunks.Add(chunk)
            Next
            Dim par1 = ""
            Dim par2 = ""
            Dim par3 = ""
            Dim counter = 1
            For Each ch In chunks
                If counter = 1 Then
                    par1 = ch
                ElseIf counter = 2 Then
                    par2 = ch
                ElseIf counter = 3 Then
                    par3 = ch
                End If
                counter += 1
            Next

            Dim Items = If(ds.Item(i, "CCCD1VOUCHERQUANTITY").ToString = "" Or ds.Item(i, "CCCD1VOUCHERQUANTITY") < 1, 1, ds.Item(i, "CCCD1VOUCHERQUANTITY"))
            Dim WeightType = credentials("WEIGHTTYPE")
            Dim Weight = 0.0
            If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
                Dim IteTable As XTable = XSupport.GetSQLDataSet("SELECT SUM(WEIGHT,0) SUM_WEIGHT FROM MTRLINES WHERE FINDOC=" + ds(i, "FINDOC").ToString())
                Weight = If(IteTable.Current("SUM_WEIGHT") < 0.5, 0.5, IteTable.Current("SUM_WEIGHT"))
            Else
                Weight = If(ds(i, "CCCD1VOUVHERWEIGHT").ToString = "" Or ds(i, "CCCD1VOUVHERWEIGHT") < 0.5, 0.5, ds(i, "CCCD1VOUVHERWEIGHT"))
            End If

            Dim speedexAccessPoint As Object
            If credentials("TESTENVIROMENT") = 1 Then
                speedexAccessPoint = New Web.speedex.test.AccessPoint()
            Else
                speedexAccessPoint = New Web.speedex.AccessPoint()
            End If

            Dim username = credentials("USERNAME")
            Dim password = credentials("PASSWORD")
            Dim returnCode As Integer
            Dim returnMessage As String = ""

            Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)

            If (returnCode = 1) Then
                Dim voucher As Object
                If credentials("TESTENVIROMENT") = 1 Then
                    voucher = New Web.speedex.test.BOL
                Else
                    voucher = New Web.speedex.BOL
                End If
                voucher._cust_Flag = 0
                voucher.Items = Convert.ToInt32(Items)
                voucher.PayCode_Flag = payCodeFlag
                voucher.Pod_Amount_Description = isCheque
                voucher.Saturday_Delivery = isSaturday
                voucher.Pod_Amount_Cash = Cod_Ammount
                voucher.RCV_Addr1 = Address
                voucher.RCV_Country = "GR"
                voucher.RCV_Name = Name
                voucher.RCV_Tel1 = Telephone
                voucher.RCV_Zip_Code = Zip
                voucher.Security_Value = InsAmmount
                voucher.Snd_agreement_id = credentials("SPEEDEXAGRID")
                voucher.SND_Customer_Id = credentials("SPEEDEXCUSID")
                voucher.Voucher_Weight = Weight
                voucher.Paratiriseis_2853_1 = par1
                voucher.Paratiriseis_2853_2 = par2
                voucher.Paratiriseis_2853_3 = par3

                Dim prodList() As Object
                If credentials("TESTENVIROMENT") = 1 Then
                    prodList = CType(Array.CreateInstance(GetType(Web.speedex.test.BOL), 1), Web.speedex.test.BOL())
                Else
                    prodList = CType(Array.CreateInstance(GetType(Web.speedex.BOL), 1), Web.speedex.BOL())
                End If
                Dim tableFlag As Integer
                Dim statusList() As String
                prodList(0) = voucher
                Dim CreateVoucher = speedexAccessPoint.CreateBOL(SessionID, prodList, tableFlag, statusList, returnCode, returnMessage)


                If returnCode = 1 Then

                    Dim DelSubs As String = ("DELETE FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                    XSupport.ExecuteSQL(DelSubs)

                    counter = 1
                    For Each item In CreateVoucher
                        If counter = 1 Then
                            Dim updatestr As String = "UPDATE FINDOC " +
                                                      "SET CCCD1VOUCHERNO='" + item.voucher_code.ToString() + "', " +
                                                      "CCCD1VOUCHERVALUE=" + Cod_Ammount.ToString.Replace(",", ".") + ", " +
                                                      "CCCD1INSAMMOUNT=" + InsAmmount.ToString.Replace(",", ".") + ", " +
                                                      "CCCD1VOUVHERWEIGHT=" + Weight.ToString.Replace(",", ".") + ", " +
                                                      "CCCD1VOUCHERQUANTITY=" + Items.ToString + ", " +
                                                      "CCCD1VOUCHERDATE='" + String.Format("{0:yyyMMdd}", Date.Today) + "', " +
                                                      "CCCD1SPEEDEXCHARGE=" + payCodeFlag.ToString() + ", " +
                                                      "CCCD1VOUCHEREXECUTION= 1, " +
                                                      "CCCD1VOUCHERDELETED= 0, " +
                                                      "CCCD1VOUCHERPRINTED= 0 " +
                                                      "WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString

                            XSupport.ExecuteSQL(updatestr)
                        Else

                            Dim vInsSub As String = "INSERT INTO CCCD1SUBVOUCHERS (VOUCHER, FINDOC) " +
                                                    "VALUES ('" + item.voucher_code.ToString() + "'," + ds(i, "FINDOC").ToString + ")"
                            XSupport.ExecuteSQL(vInsSub)
                        End If
                        counter += 1
                    Next

                    Success_list.Add(ds.Item(i, "FINCODE").ToString + " : " + CreateVoucher(0).voucher_code.ToString())
                Else
                    Error_list_Messages.Add(returnMessage)
                    Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                    Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                End If
            Else
                Error_list_Messages.Add(returnMessage)
                Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
            End If


        End If
        Return 0
    End Function


    Public Function PrintMassVoucher11700012(ds As XTable, i As Integer, XModule As XModule, XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=3 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
        Else
            Dim credentials = credentialsTable.Current

            Dim username = credentials("USERNAME")
            Dim password = credentials("PASSWORD")
            Dim vPrintType = credentials("VOUCHERPRINTTYPE")
            Dim folderpath = credentials("FOLDERPATH")
            Dim ClosingDate As Date = If(IsDBNull(ds(i, "CCCD1VOUCHERDATE")), Date.Today, ds(i, "CCCD1VOUCHERDATE"))


            Dim returnCode As Integer
            Dim returnMessage As String = ""

            Dim speedexAccessPoint As Object
            If credentials("TESTENVIROMENT") = 1 Then
                speedexAccessPoint = New Web.speedex.test.AccessPoint()
            Else
                speedexAccessPoint = New Web.speedex.AccessPoint()
            End If

            Dim SessionID = speedexAccessPoint.CreateSession(username, password, returnCode, returnMessage)
            If (returnCode = 1) Then
                Dim voucherIds() As String
                voucherIds = CType(Array.CreateInstance(GetType(String), 1), String())
                voucherIds(0) = ds(i, "CCCD1VOUCHERNO").ToString
                queryStr = String.Format("SELECT VOUCHER FROM CCCD1SUBVOUCHERS WHERE FINDOC={0}", ds(i, "FINDOC"))
                Dim SubvTable = XSupport.GetSQLDataSet(queryStr)
                If SubvTable.Count > 0 Then 'Αν έχει Subvouchers
                    For k As Integer = 0 To SubvTable.Count - 1
                        voucherIds = voucherIds.Concat({SubvTable(k, "VOUCHER")}).ToArray()
                    Next
                End If
                Dim pdf = speedexAccessPoint.GetBOLPdf(SessionID, voucherIds, False, vPrintType, returnCode, returnMessage)
                If (returnCode = 1) Then
                    Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
                    Dim strPDFLocation = strFileLocation + "\" + ds(i, "CCCD1VOUCHERNO").ToString + ".pdf"
                    Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                    Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                    If Not folderexists Then
                        Directory.CreateDirectory(strFileLocation)
                    End If
                    If Not pdfgexists Then
                        File.WriteAllBytes(strPDFLocation, pdf(0).pdf)
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
                    XSupport.ExecuteSQL("UPDATE FINDOC SET CCCD1VOUCHERPRINTED=1 WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                Else
                    Throw New Exception(returnMessage)
                End If
            Else
                Throw New Exception(returnMessage)
            End If
        End If
        Return 0
    End Function


End Module
