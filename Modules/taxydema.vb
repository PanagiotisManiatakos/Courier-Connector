Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports Softone

Module taxydema
    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
        Dim a_cod_poso
        Dim a_cod_flag
        Dim a_cod_date
        If vItsCod Then
            salData("CCCD1VOUCHERVALUE") = If(salData("CCCD1VOUCHERVALUE") > 0.0, salData("CCCD1VOUCHERVALUE"), salData("SUMAMNT"))
            If IsDBNull(salData("CCCD1TAXYDEMACODFLAG")) Then
                salData("CCCD1TAXYDEMACODFLAG") = 1
            Else
                salData("CCCD1TAXYDEMACODFLAG") = If(salData("CCCD1TAXYDEMACODFLAG") = 0, 1, salData("CCCD1TAXYDEMACODFLAG"))
            End If
            a_cod_poso = salData("CCCD1VOUCHERVALUE")
            a_cod_flag = salData("CCCD1TAXYDEMACODFLAG")
            If a_cod_flag = 2 Then
                If IsDBNull(salData("CCCD1TAXYDEMACODECHEQUEDATE")) Then
                    Throw New Exception("Δεν έχετε συμπληρώσει ημ/νία επιταγής για την αντικαταβολή")
                Else
                    a_cod_date = salData("CCCD1TAXYDEMACODECHEQUEDATE")
                End If
            Else
                a_cod_date = Nothing
            End If
        Else
            a_cod_poso = 0.0
            a_cod_flag = 0
            a_cod_date = Nothing
        End If

        Dim a_rec_title = If(IsDBNull(salData("CCCD1SHIPNAME")), "", salData("CCCD1SHIPNAME"))
        Dim a_rec_thl_1 = If(IsDBNull(salData("CCCD1SHIPCELLPHONE")), "", salData("CCCD1SHIPCELLPHONE"))
        Dim a_rec_address = If(IsDBNull(salData("CCCD1SHIPADDRESS")), "", salData("CCCD1SHIPADDRESS"))
        Dim a_rec_tk = If(IsDBNull(salData("CCCD1SHIPZIP")), "", salData("CCCD1SHIPZIP"))
        Dim a_rec_area = If(IsDBNull(salData("CCCD1SHIPCITY")), "", salData("CCCD1SHIPCITY"))
        Dim a_rec_sxolia = If(IsDBNull(salData("CCCD1VOUCHERCOMMENTS")), "", salData("CCCD1VOUCHERCOMMENTS"))
        Dim a_rec_temaxia = If(salData("CCCD1VOUCHERQUANTITY").ToString = "" Or salData("CCCD1VOUCHERQUANTITY") < 1, 1, salData("CCCD1VOUCHERQUANTITY"))
        Dim a_rec_ref = If(salData("FINDOC") < 0, If(IsDBNull(salData("FINCODE")), "", salData("FINCODE")), If(IsDBNull(salData("CMPFINCODE")), "", salData("CMPFINCODE")))

        Dim WeightType = credentials("WEIGHTTYPE")
        Dim a_rec_baros = 0.0
        If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
            Dim IteTable As XTable = XModule.GetTable("ITELINES")
            If IteTable.Count > 0 Then
                For i = 0 To IteTable.Count - 1
                    Dim IteWeight = If(IteTable(i, "WEIGHT").ToString = "", 0.0, IteTable(i, "WEIGHT"))
                    a_rec_baros = +IteWeight
                Next

                a_rec_baros = If(a_rec_baros < 0.5, 0.5, a_rec_baros)

            Else
                XSupport.Warning("Το παραστατικό δεν έχει είδη")
            End If
        Else
            a_rec_baros = If(salData("CCCD1VOUVHERWEIGHT").ToString = "" Or salData("CCCD1VOUVHERWEIGHT") < 0.5, 0.5, salData("CCCD1VOUVHERWEIGHT"))
        End If

        Dim new_sideta As New Web.taxydema.create.TAXYCREATESIDETA()
        Dim user_details As New Web.taxydema.create.INSERTUser_details()
        Dim vg_details As New Web.taxydema.create.INSERTVg_details()


        user_details.a_pel_code = credentials("TAXYDEMAPELCODE")
        user_details.a_pel_sub_code = If(IsDBNull(credentials("TAXYDEMAPELSUBCODE")), "", credentials("TAXYDEMAPELSUBCODE"))
        user_details.a_user_code = credentials("TAXYDEMAUSERCODE")
        user_details.a_user_pass = credentials("TAXYDEMAUSERPASS")

        vg_details.a_rec_title = a_rec_title
        vg_details.a_rec_address = a_rec_address
        vg_details.a_rec_area = a_rec_area
        vg_details.a_rec_tk = a_rec_tk
        vg_details.a_rec_thl_1 = a_rec_thl_1
        vg_details.a_rec_temaxia = a_rec_temaxia
        vg_details.a_rec_baros = a_rec_baros
        vg_details.a_rec_sxolia = a_rec_sxolia
        vg_details.a_rec_ref = a_rec_ref
        vg_details.a_cod_flag = a_cod_flag
        vg_details.a_cod_poso = a_cod_poso
        vg_details.a_cod_date = a_cod_date
        vg_details.a_sur_1 = "0"
        vg_details.a_sur_2 = "0"
        vg_details.a_sur_3 = "0"

        Dim webanswer As String
        Dim st_title As String = ""
        Dim taxydema_sideta As String = ""
        Dim taxydema_doc_sideta As String = ""
        Dim taxydema_par_sideta As String = ""
        Dim taxydema_child_no As String() = {}

        webanswer = new_sideta.INSERT(user_details, vg_details, st_title, taxydema_sideta, taxydema_doc_sideta, taxydema_par_sideta, taxydema_child_no)

        If webanswer = "0" Then
            Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
            If SubvTable.Count > 0 Then 'Αν έχει ήδη άλλα Subvouchers
                While SubvTable.Count > 0
                    SubvTable.Current.Delete() 'Τα διαγραφω γραμμη γραμμη
                End While
            End If

            If a_rec_temaxia > 1 Then
                Dim SubRow As XRow = SubvTable.Current
                For Each item In taxydema_child_no
                    If item = "" Then
                        Exit For
                    Else
                        SubRow.Append()
                        SubRow("VOUCHER") = item.Trim()
                        SubRow.Post()
                    End If
                Next
            End If
            salData("CCCD1VOUCHERNO") = taxydema_sideta.Trim()
            salData("CCCD1VOUCHERVALUE") = a_cod_poso
            salData("CCCD1VOUVHERWEIGHT") = a_rec_baros
            salData("CCCD1VOUCHERQUANTITY") = a_rec_temaxia
            salData("CCCD1TAXYDEMACODFLAG") = If(a_cod_flag = 0, 0, If(a_cod_flag = 1, 1, 2))
            salData("CCCD1VOUCHEREXECUTION") = 1
            salData("CCCD1VOUCHERDELETED") = 0
            salData("CCCD1VOUCHERPRINTED") = 0
        Else
            Throw New Exception(st_title)

        End If
        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)

        Dim sideta_print As Object
        If credentials("VOUCHERPRINTTYPE") = 1 Then
            sideta_print = New Web.taxydema.print.TAXYPRINTSIDETA()
        Else
            sideta_print = New Web.taxydema.printA6.TAXYPRINTSIDETAA6()
        End If

        Dim STFLAG As String
        Dim STTITLE As String = ""
        Dim B64 As String = ""

        STFLAG = sideta_print.PRINT(
            credentials("TAXYDEMAUSERCODE"),
            credentials("TAXYDEMAUSERPASS"),
            credentials("TAXYDEMAPELCODE"),
            salData("CCCD1VOUCHERNO"),
            STTITLE,
            B64
            )
        If STFLAG = 0 Then
            Dim strFileLocation = credentials("FOLDERPATH") + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
            Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1VOUCHERNO").ToString + ".pdf"
            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
            If Not folderexists Then
                Directory.CreateDirectory(strFileLocation)
            End If
            If Not pdfgexists Then
                Dim pdfBytes As Byte() = Convert.FromBase64String(B64)
                File.WriteAllBytes(strPDFLocation, pdfBytes)
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

            Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
            If SubvTable.Count > 0 Then 'Αν έχει Subvouchers
                For k As Integer = 0 To SubvTable.Count - 1
                    STFLAG = sideta_print.PRINT(
                                credentials("TAXYDEMAUSERCODE"),
                                credentials("TAXYDEMAUSERPASS"),
                                credentials("TAXYDEMAPELCODE"),
                                SubvTable(k, "VOUCHER"),
                                STTITLE,
                                B64
                            )
                    If STFLAG = 0 Then
                        strPDFLocation = strFileLocation + "\" + SubvTable(k, "VOUCHER") + ".pdf"
                        folderexists = Directory.Exists(strFileLocation)
                        pdfgexists = File.Exists(strPDFLocation)
                        If Not folderexists Then
                            Directory.CreateDirectory(strFileLocation)
                        End If
                        If Not pdfgexists Then
                            Dim pdfBytes As Byte() = Convert.FromBase64String(B64)
                            File.WriteAllBytes(strPDFLocation, pdfBytes)
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
                    End If
                Next
            End If

            salData("CCCD1VOUCHERPRINTED") = 1
            XSupport.Warning("Ολοκλήρωση εκτύπωσης")


        Else
            Throw New Exception(STTITLE)
        End If



        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim delete_vg As New Web.taxydema.delete.TAXYDELETESIDETA()
        Dim user_details As New Web.taxydema.delete.DELETEUser_details

        user_details.a_pel_code = credentials("TAXYDEMAPELCODE")
        user_details.a_user_code = credentials("TAXYDEMAUSERCODE")
        user_details.a_user_pass = credentials("TAXYDEMAUSERPASS")

        Dim webanswer As String
        Dim st_title As String = ""

        webanswer = delete_vg.DELETE(
                    user_details,
                    saldata("CCCD1VOUCHERNO"),
                    st_title
                    )
        If webanswer = 0 Then
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
            Throw New Exception(st_title)
        End If
        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim tt_vg As New Web.taxydema.track.TAXYTTSIDETA()
        Dim user_details As New Web.taxydema.track.READUser_details
        Dim tt_rec(99) As Web.taxydema.track.READResponseTt_rec

        user_details.a_pel_code = credentials("TAXYDEMAPELCODE")
        user_details.a_user_code = credentials("TAXYDEMAUSERCODE")
        user_details.a_user_pass = credentials("TAXYDEMAUSERPASS")

        Dim webanswer As String
        Dim st_title As String = ""
        Dim pod_date As String = ""
        Dim pod_time As String = ""
        Dim pod_name As String = ""

        webanswer = tt_vg.READ(
                    user_details,
                    saldata("CCCD1VOUCHERNO"),
                    st_title,
                    pod_date,
                    pod_time,
                    pod_name,
                    tt_rec
                    )
        If webanswer = 0 Then
            Dim Trform = New TrackingForm()
            For Each item In tt_rec
                If item.tt_date = "" Then
                    Exit For
                Else
                    Dim x As String() = {item.tt_date + " " + item.tt_time, item.tt_status_title, item.tt_station_title, ""}
                    Trform.DataGridView1.Rows.Add(x)
                End If
            Next
            Trform.Show()

        Else
            Throw New Exception(st_title)
        End If

        Return 0
    End Function

    Public Function GetMassVoucher11700011(ds As XTable, i As Integer, Success_list As List(Of String), Error_list_Messages As List(Of String), Error_list_Fincode As List(Of String), Error_list_Findoc As List(Of String), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=5 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Error_list_Messages.Add("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
        Else
            Dim credentials As XRow = credentialsTable.Current

            Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(ds(i, "PAYMENT").ToString) And Not IsDBNull(ds(i, "PAYMENT"))
            Dim a_cod_poso
            Dim a_cod_flag
            Dim a_cod_date = Nothing
            If vItsCod Then
                a_cod_poso = If(ds.Item(i, "CCCD1VOUCHERVALUE") > 0.0, ds.Item(i, "CCCD1VOUCHERVALUE"), ds.Item(i, "SUMAMNT"))
                If IsDBNull(ds(i, "CCCD1TAXYDEMACODFLAG")) Then
                    a_cod_flag = 1
                Else
                    a_cod_flag = If(ds(i, "CCCD1TAXYDEMACODFLAG") = 0, 1, ds(i, "CCCD1TAXYDEMACODFLAG"))
                End If
                If a_cod_flag = 2 Then
                    If IsDBNull(ds(i, "CCCD1TAXYDEMACODECHEQUEDATE")) Then
                        Error_list_Messages.Add("Δεν έχετε συμπληρώσει ημ/νία επιταγής για την αντικαταβολή")
                        Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                        Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                    Else
                        a_cod_date = ds(i, "CCCD1TAXYDEMACODECHEQUEDATE")
                    End If
                Else
                    a_cod_date = Nothing
                End If
            Else
                a_cod_poso = 0.0
                a_cod_flag = 0
                a_cod_date = Nothing
            End If

            Dim a_rec_title = If(IsDBNull(ds(i, "CCCD1SHIPNAME")), "", ds(i, "CCCD1SHIPNAME"))
            Dim a_rec_thl_1 = If(IsDBNull(ds(i, "CCCD1SHIPCELLPHONE")), "", ds(i, "CCCD1SHIPCELLPHONE"))
            Dim a_rec_address = If(IsDBNull(ds(i, "CCCD1SHIPADDRESS")), "", ds(i, "CCCD1SHIPADDRESS"))
            Dim a_rec_tk = If(IsDBNull(ds(i, "CCCD1SHIPZIP")), "", ds(i, "CCCD1SHIPZIP"))
            Dim a_rec_area = If(IsDBNull(ds(i, "CCCD1SHIPCITY")), "", ds(i, "CCCD1SHIPCITY"))
            Dim a_rec_sxolia = If(IsDBNull(ds(i, "CCCD1VOUCHERCOMMENTS")), "", ds(i, "CCCD1VOUCHERCOMMENTS"))
            Dim a_rec_temaxia = If(ds(i, "CCCD1VOUCHERQUANTITY").ToString = "" Or ds(i, "CCCD1VOUCHERQUANTITY") < 1, 1, ds(i, "CCCD1VOUCHERQUANTITY"))
            Dim a_rec_ref = If(IsDBNull(ds(i, "FINCODE")), "", ds(i, "FINCODE"))

            Dim WeightType = credentials("WEIGHTTYPE")
            Dim a_rec_baros = 0.0
            If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
                Dim IteTable As XTable = XSupport.GetSQLDataSet("SELECT SUM(WEIGHT,0) SUM_WEIGHT FROM MTRLINES WHERE FINDOC=" + ds(i, "FINDOC").ToString())
                a_rec_baros = If(IteTable.Current("SUM_WEIGHT") < 0.5, 0.5, IteTable.Current("SUM_WEIGHT"))
            Else
                a_rec_baros = If(ds(i, "CCCD1VOUVHERWEIGHT").ToString = "" Or ds(i, "CCCD1VOUVHERWEIGHT") < 0.5, 0.5, ds(i, "CCCD1VOUVHERWEIGHT"))
            End If

            Dim new_sideta As New Web.taxydema.create.TAXYCREATESIDETA()
            Dim user_details As New Web.taxydema.create.INSERTUser_details()
            Dim vg_details As New Web.taxydema.create.INSERTVg_details()


            user_details.a_pel_code = credentials("TAXYDEMAPELCODE")
            user_details.a_pel_sub_code = If(IsDBNull(credentials("TAXYDEMAPELSUBCODE")), "", credentials("TAXYDEMAPELSUBCODE"))
            user_details.a_user_code = credentials("TAXYDEMAUSERCODE")
            user_details.a_user_pass = credentials("TAXYDEMAUSERPASS")

            vg_details.a_rec_title = a_rec_title
            vg_details.a_rec_address = a_rec_address
            vg_details.a_rec_area = a_rec_area
            vg_details.a_rec_tk = a_rec_tk
            vg_details.a_rec_thl_1 = a_rec_thl_1
            vg_details.a_rec_temaxia = a_rec_temaxia
            vg_details.a_rec_baros = a_rec_baros
            vg_details.a_rec_sxolia = a_rec_sxolia
            vg_details.a_rec_ref = a_rec_ref
            vg_details.a_cod_flag = a_cod_flag
            vg_details.a_cod_poso = a_cod_poso
            vg_details.a_cod_date = a_cod_date
            vg_details.a_sur_1 = "0"
            vg_details.a_sur_2 = "0"
            vg_details.a_sur_3 = "0"

            Dim webanswer As String
            Dim st_title As String = ""
            Dim taxydema_sideta As String = ""
            Dim taxydema_doc_sideta As String = ""
            Dim taxydema_par_sideta As String = ""
            Dim taxydema_child_no As String() = {}

            webanswer = new_sideta.INSERT(user_details, vg_details, st_title, taxydema_sideta, taxydema_doc_sideta, taxydema_par_sideta, taxydema_child_no)

            If webanswer = "0" Then
                Dim DelSubs As String = ("DELETE FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                XSupport.ExecuteSQL(DelSubs)

                If a_rec_temaxia > 1 Then
                    For Each item In taxydema_child_no
                        If item = "" Then
                            Exit For
                        Else
                            Dim vInsSub As String = "INSERT INTO CCCD1SUBVOUCHERS (VOUCHER, FINDOC) " +
                                                    "VALUES ('" + item.Trim() + "'," + ds(i, "FINDOC").ToString + ")"
                            XSupport.ExecuteSQL(vInsSub)
                        End If
                    Next
                End If


                Dim updatestr As String = "UPDATE FINDOC " +
                                            "SET CCCD1VOUCHERNO='" + taxydema_sideta.Trim() + "', " +
                                            "CCCD1VOUCHERVALUE=" + a_cod_poso.ToString.Replace(",", ".") + ", " +
                                            "CCCD1VOUVHERWEIGHT=" + a_rec_baros.ToString.Replace(",", ".") + ", " +
                                            "CCCD1VOUCHERQUANTITY=" + a_rec_temaxia.ToString + ", " +
                                            "CCCD1TAXYDEMACODFLAG=" + a_cod_flag.ToString + "," +
                                            "CCCD1VOUCHEREXECUTION= 1, " +
                                            "CCCD1VOUCHERDELETED= 0, " +
                                            "CCCD1VOUCHERPRINTED= 0 " +
                                            "WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString

                XSupport.ExecuteSQL(updatestr)
                Success_list.Add(ds.Item(i, "FINCODE").ToString + " : " + taxydema_sideta.Trim())
            Else
                Error_list_Messages.Add(st_title)
                Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
            End If

        End If
        Return 0
    End Function

    Public Function PrintMassVoucher11700012(ds As XTable, i As Integer, XModule As XModule, XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=5 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
        Else
            Dim credentials As XRow = credentialsTable.Current
            Dim folderpath = credentials("FOLDERPATH")
            Dim sideta_print As Object
            If credentials("VOUCHERPRINTTYPE") = 1 Then
                sideta_print = New Web.taxydema.print.TAXYPRINTSIDETA()
            Else
                sideta_print = New Web.taxydema.printA6.TAXYPRINTSIDETAA6()
            End If

            Dim STFLAG As String
            Dim STTITLE As String = ""
            Dim B64 As String = ""

            STFLAG = sideta_print.PRINT(
                    credentials("TAXYDEMAUSERCODE"),
                    credentials("TAXYDEMAUSERPASS"),
                    credentials("TAXYDEMAPELCODE"),
                    ds(i, "CCCD1VOUCHERNO"),
                    STTITLE,
                    B64
                    )

            If STFLAG = 0 Then
                Dim strFileLocation = credentials("FOLDERPATH") + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
                Dim strPDFLocation = strFileLocation + "\" + ds(i, "CCCD1VOUCHERNO").ToString + ".pdf"
                Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                If Not folderexists Then
                    Directory.CreateDirectory(strFileLocation)
                End If
                If Not pdfgexists Then
                    Dim pdfBytes As Byte() = Convert.FromBase64String(B64)
                    File.WriteAllBytes(strPDFLocation, pdfBytes)
                End If
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

                Dim dsSub = XSupport.GetSQLDataSet("SELECT VOUCHER FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                If dsSub.Count > 0 Then
                    Dim k As Integer
                    For k = 0 To dsSub.Count - 1
                        STFLAG = sideta_print.PRINT(
                                credentials("TAXYDEMAUSERCODE"),
                                credentials("TAXYDEMAUSERPASS"),
                                credentials("TAXYDEMAPELCODE"),
                                dsSub(k, "VOUCHER"),
                                STTITLE,
                                B64
                                )
                        If STFLAG = 0 Then
                            strPDFLocation = strFileLocation + "\" + dsSub(k, "VOUCHER").ToString + ".pdf"
                            folderexists = Directory.Exists(strFileLocation)
                            pdfgexists = File.Exists(strPDFLocation)
                            If Not folderexists Then
                                Directory.CreateDirectory(strFileLocation)
                            End If
                            If Not pdfgexists Then
                                Dim pdfBytes As Byte() = Convert.FromBase64String(B64)
                                File.WriteAllBytes(strPDFLocation, pdfBytes)
                            End If
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
                        End If

                    Next
                End If
            End If
        End If
        Return 0
    End Function
End Module
