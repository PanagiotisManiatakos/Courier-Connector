Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Web
Imports Microsoft.Office
Imports Microsoft.VisualBasic.ApplicationServices
Imports Softone

Module eltaCourier
    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
        Dim pel_ant_poso
        Dim pel_ant_poso1
        Dim pel_ant_poso2 = ""
        Dim pel_ant_poso3 = ""
        Dim pel_ant_poso4 = ""
        Dim pel_ant_date1
        Dim pel_ant_date2 = ""
        Dim pel_ant_date3 = ""
        Dim pel_ant_date4 = ""
        If vItsCod Then
            salData("CCCD1VOUCHERVALUE") = If(salData("CCCD1VOUCHERVALUE") > 0.0, salData("CCCD1VOUCHERVALUE"), salData("SUMAMNT"))
            pel_ant_poso = If(salData("CCCD1ELTACCHEQUE") = 1, 0, salData("CCCD1VOUCHERVALUE"))
            pel_ant_poso1 = If(salData("CCCD1ELTACCHEQUE") = 1, salData("CCCD1VOUCHERVALUE"), 0)
            If salData("CCCD1ELTACCHEQUE") = 1 Then
                If IsDBNull(salData("CCCD1ELTACCHEQUEDATE")) Then
                    Throw New Exception("Δεν έχετε συμπληρώσει ημ/νία επιταγής για την αντικαταβολή")
                Else
                    pel_ant_date1 = salData("CCCD1ELTACCHEQUEDATE")
                End If
            Else
                pel_ant_date1 = Nothing
            End If
        Else
            pel_ant_poso = 0.0
            pel_ant_poso1 = 0.0
            pel_ant_date1 = Nothing
        End If

        Dim pel_service = If(IsDBNull(salData("CCCD1ELTACSERVICE")), 1, salData("CCCD1ELTACSERVICE"))
        Dim pel_sur_1 = If(IsDBNull(salData("CCCD1ELTACSUR1")), 0, salData("CCCD1ELTACSUR1"))
        Dim pel_sur_2 = If(IsDBNull(salData("CCCD1ELTACSUR2")), 0, salData("CCCD1ELTACSUR2"))
        Dim pel_sur_3 = If(IsDBNull(salData("CCCD1ELTACSUR3")), 0, salData("CCCD1ELTACSUR3"))

        Dim pel_asf_poso = If(IsDBNull(salData("CCCD1ELTACINSAMOUNT")), 0, salData("CCCD1ELTACINSAMOUNT"))

        Dim pel_paral_name = If(IsDBNull(salData("CCCD1SHIPNAME")), "", salData("CCCD1SHIPNAME"))
        Dim pel_paral_thl_1 = If(IsDBNull(salData("CCCD1SHIPCELLPHONE")), "", salData("CCCD1SHIPCELLPHONE"))
        Dim pel_paral_thl_2 = If(IsDBNull(salData("CCCD1SHIPCELLPHONE")), "", salData("CCCD1SHIPCELLPHONE"))
        Dim pel_paral_address = If(IsDBNull(salData("CCCD1SHIPADDRESS")), "", salData("CCCD1SHIPADDRESS"))
        Dim pel_paral_tk = If(IsDBNull(salData("CCCD1SHIPZIP")), "", salData("CCCD1SHIPZIP"))
        Dim pel_paral_area = If(IsDBNull(salData("CCCD1SHIPCITY")), "", salData("CCCD1SHIPCITY"))
        Dim pel_paral_sxolia = If(IsDBNull(salData("CCCD1VOUCHERCOMMENTS")), "", salData("CCCD1VOUCHERCOMMENTS"))
        Dim pel_temaxia = If(salData("CCCD1VOUCHERQUANTITY").ToString = "" Or salData("CCCD1VOUCHERQUANTITY") < 1, 1, salData("CCCD1VOUCHERQUANTITY"))
        Dim pel_ref_no = If(salData("FINDOC") < 0, If(IsDBNull(salData("FINCODE")), "", salData("FINCODE")), If(IsDBNull(salData("CMPFINCODE")), "", salData("CMPFINCODE")))

        Dim WeightType = credentials("WEIGHTTYPE")
        Dim pel_baros = 0.0
        If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
            Dim IteTable As XTable = XModule.GetTable("ITELINES")
            If IteTable.Count > 0 Then
                For i = 0 To IteTable.Count - 1
                    Dim IteWeight = If(IteTable(i, "WEIGHT").ToString = "", 0.0, IteTable(i, "WEIGHT"))
                    pel_baros = +IteWeight
                Next

                pel_baros = If(pel_baros < 0.5, 0.5, pel_baros)

            Else
                XSupport.Warning("Το παραστατικό δεν έχει είδη")
            End If
        Else
            pel_baros = If(salData("CCCD1VOUVHERWEIGHT").ToString = "" Or salData("CCCD1VOUVHERWEIGHT") < 0.5, 0.5, salData("CCCD1VOUVHERWEIGHT"))
        End If

        Dim pel_user_code As String = If(IsDBNull(credentials("ELTACCODE")), "", credentials("ELTACCODE"))
        Dim pel_user_pass As String = If(IsDBNull(credentials("ELTACPASS")), "", credentials("ELTACPASS"))
        Dim pel_apost_code As String = If(IsDBNull(credentials("ELTACAPOSTCODE")), "", credentials("ELTACAPOSTCODE"))
        Dim pel_apost_sub_code As String = If(IsDBNull(credentials("ELTACSUBCODE")), "", credentials("ELTACSUBCODE"))
        Dim pel_user_lang As String = ""


        Dim webanswer
        Dim st_title As String = ""
        Dim vg_code As String = ""
        Dim return_vg As String = ""
        Dim epitagh_vg As String = ""
        Dim vg_child As String() = {}

        Dim createWB As New Web.eltaCourier.create.CREATEAWB()

        webanswer = createWB.READ(
            pel_user_code,
            pel_user_pass,
            pel_apost_code,
            pel_apost_sub_code,
            pel_user_lang,
            pel_paral_name,
            pel_paral_address,
            pel_paral_area,
            pel_paral_tk,
            pel_paral_thl_1,
            pel_paral_thl_2,
            pel_service,
            pel_baros,
            pel_temaxia,
            pel_paral_sxolia,
            pel_sur_1,
            pel_sur_2,
            pel_sur_3,
            pel_ant_poso,
            pel_ant_poso1,
            pel_ant_poso2,
            pel_ant_poso3,
            pel_ant_poso4,
            pel_ant_date1,
            pel_ant_date2,
            pel_ant_date3,
            pel_ant_date4,
            pel_asf_poso,
            pel_ref_no,
            st_title,
            vg_code,
            return_vg,
            epitagh_vg,
            vg_child)

        If webanswer = "0" Then
            Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
            If SubvTable.Count > 0 Then 'Αν έχει ήδη άλλα Subvouchers
                While SubvTable.Count > 0
                    SubvTable.Current.Delete() 'Τα διαγραφω γραμμη γραμμη
                End While
            End If

            If pel_temaxia > 1 Then
                Dim SubRow As XRow = SubvTable.Current
                For Each item In vg_child
                    If item = "" Then
                        Exit For
                    Else
                        SubRow.Append()
                        SubRow("VOUCHER") = item.Trim()
                        SubRow.Post()
                    End If
                Next
            End If
            salData("CCCD1VOUCHERNO") = vg_code.Trim()
            salData("CCCD1VOUCHERVALUE") = pel_ant_poso + pel_ant_poso1
            salData("CCCD1VOUVHERWEIGHT") = pel_baros
            salData("CCCD1VOUCHERQUANTITY") = pel_temaxia
            salData("CCCD1VOUCHEREXECUTION") = 1
            salData("CCCD1VOUCHERDELETED") = 0
            salData("CCCD1VOUCHERPRINTED") = 0
        Else
            Throw New Exception(st_title)

        End If
        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)

        Dim pel_user_code As String = If(IsDBNull(credentials("ELTACCODE")), "", credentials("ELTACCODE"))
        Dim pel_user_pass As String = If(IsDBNull(credentials("ELTACPASS")), "", credentials("ELTACPASS"))
        Dim pel_apost_code As String = If(IsDBNull(credentials("ELTACAPOSTCODE")), "", credentials("ELTACAPOSTCODE"))
        Dim vg_code As String = salData("CCCD1VOUCHERNO")
        Dim paper_size = credentials("VOUCHERPRINTTYPE")

        Dim print As New Web.eltaCourier.print.PELB64VG()


        Dim st_flag As String
        Dim st_title As String = ""
        Dim b64_string As String = ""

        st_flag = print.READ(
            pel_user_code,
            pel_user_pass,
            pel_apost_code,
            vg_code,
            paper_size,
            st_title,
            b64_string
            )
        If st_flag = 0 Then
            Dim strFileLocation = credentials("FOLDERPATH") + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
            Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1VOUCHERNO").ToString + ".pdf"
            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
            If Not folderexists Then
                Directory.CreateDirectory(strFileLocation)
            End If
            If Not pdfgexists Then
                Dim pdfBytes As Byte() = Convert.FromBase64String(b64_string)
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
                    st_flag = print.READ(
                                pel_user_code,
                                pel_user_pass,
                                pel_apost_code,
                                SubvTable(k, "VOUCHER"),
                                paper_size,
                                st_title,
                                b64_string)
                    If st_flag = 0 Then
                        strPDFLocation = strFileLocation + "\" + SubvTable(k, "VOUCHER") + ".pdf"
                        folderexists = Directory.Exists(strFileLocation)
                        pdfgexists = File.Exists(strPDFLocation)
                        If Not folderexists Then
                            Directory.CreateDirectory(strFileLocation)
                        End If
                        If Not pdfgexists Then
                            Dim pdfBytes As Byte() = Convert.FromBase64String(b64_string)
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
                    Else
                        Throw New Exception(st_title)
                    End If
                Next
            End If

            salData("CCCD1VOUCHERPRINTED") = 1
            XSupport.Warning("Ολοκλήρωση εκτύπωσης")


        Else
            Throw New Exception(st_title)
        End If



        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)

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


        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, saldata As XRow, XModule As XModule, XSupport As XSupport)
        Dim track As New Web.eltaCourier.track.PELTT01()
        Dim wpel_code As String = If(IsDBNull(credentials("ELTACCODE")), "", credentials("ELTACCODE"))
        Dim wpel_pass As String = If(IsDBNull(credentials("ELTACPASS")), "", credentials("ELTACPASS"))
        Dim wpel_user As String = If(IsDBNull(credentials("ELTACAPOSTCODE")), "", credentials("ELTACAPOSTCODE"))
        Dim vg_code As String = saldata("CCCD1VOUCHERNO")
        Dim wpel_ref = If(saldata("FINDOC") < 0, If(IsDBNull(saldata("FINCODE")), "", saldata("FINCODE")), If(IsDBNull(saldata("CMPFINCODE")), "", saldata("CMPFINCODE")))

        Dim webanswer As String
        Dim st_title As String = ""
        Dim st_flag As String = ""
        Dim pod_date As String = ""
        Dim pod_time As String = ""
        Dim pod_name As String = ""
        Dim web_status As Web.eltaCourier.track.READResponseWeb_status()
        Dim web_status_counter As String = ""

        webanswer = track.READ(
                    wpel_user,
                    wpel_code,
                    wpel_pass,
                    saldata("CCCD1VOUCHERNO"),
                    wpel_ref,
                    st_flag,
                    st_title,
                    pod_date,
                    pod_time,
                    pod_name,
                    web_status,
                    web_status_counter
                    )
        If webanswer = 0 Then
            Dim Trform = New TrackingForm()
            For Each item In web_status
                If item.web_date() = "" Then
                    Exit For
                Else
                    Dim x As String() = {item.web_date() + " " + item.web_time(), item.web_status_title(), item.web_station(), ""}
                    Trform.DataGridView1.Rows.Add(x)
                End If
            Next
            Trform.Show()

        Else
            Throw New Exception(st_title)
        End If

        Return 0
    End Function
End Module
