Imports Softone
Imports System.Windows.Forms
Imports System.Net

Namespace CourierConnector

    Partial Public Class S1

        Private Shared ReadOnly notif As New NotifyIcon With {
            .Icon = My.Resources.day1_logo,
            .Visible = True,
            .Tag = "DayOneInfo"
            }

        Public Class S1Init
            Inherits TXCode

            Public Overrides Sub Initialize()
                Try
                    Dim querStr As String = "SELECT COURIER,BLCDATE FROM CCCD1COURIERSMODULE"
                    Dim dsBlc As XTable = XSupport.GetSQLDataSet(querStr)

                    If dsBlc.Count > 0 Then
                        Dim i As Integer
                        For i = 0 To dsBlc.Count - 1
                            Dim courier As Integer = dsBlc(i, "COURIER")
                            Dim blcdate As Date = Convert.ToDateTime(dsBlc(i, "BLCDATE").ToString())
                            If (blcdate - Date.Now).TotalDays <= -1 Then
                                XSupport.Warning("Η άδεια του module " + CourierNames(courier) + " Connector έληξε την " + blcdate.ToString("dd/MM/yyyy") + "." + vbCrLf + "Προχωρήστε σε ανανέωση ώστε να συνεχίστε να χρησιμοποιείτε το module")
                            ElseIf (blcdate - Date.Now).TotalDays < 15 Then
                                XSupport.Warning("Η άδεια του module " + CourierNames(courier) + " Connector λήγει την " + blcdate.ToString("dd/MM/yyyy") + "." + vbCrLf + "Προχωρήστε σε ανανέωση ώστε να συνεχίστε να χρησιμοποιείτε το module" + vbCrLf + "Σημειώνεται πως με την λήξη, παύει άμεσα η πρόσβαση στις δυνατότητες του module.")
                            End If

                        Next
                    End If
                Catch ex As Exception
                    XSupport.Warning(ex.Message)
                End Try
            End Sub

            <WorksOn("SALDOC")>
            Public Class SALDOC
                Inherits TXCode
                Private SalTable As XTable
                Public Overrides Function EXECCOMMAND(cmd As Integer)
                    SalTable = XModule.GetTable("FINDOC")
                    Dim salData As XRow = SalTable.Current
                    ServicePointManager.Expect100Continue = True
                    ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)  'its same like SecurityProtocolType.Tls12
                    ServicePointManager.DefaultConnectionLimit = 9999

                    Try
                        Select Case cmd
                            Case 11700001 '----------ΕΚΔΟΣΗ VOUCHER----------
                                If salData("CCCD1VOUCHEREXECUTION") = 1 Then
                                    Throw New Exception("Έχει εκδοθεί voucher για την συγκεκριμένη αποστολή.")
                                End If
                                If IsDBNull(salData("SERIES")) Then
                                    Throw New Exception("Δεν έχετε επιλέξει σειρά")
                                End If
                                Dim Courier = salData("CCCD1COURIERCOMPANY")
                                If IsDBNull(Courier) Then
                                    Throw New Exception("Δεν έχετε επιλέξει μεταφορική εταιρεία")
                                End If
                                Dim proceed = CheckCanGo(Courier, XSupport)
                                If proceed.success Then
                                    Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY={1} AND SERIES LIKE '%{2}%' AND ISACTIVE=1", salData("COMPANY"), Courier, salData("SERIES"))
                                    Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
                                    If credentialsTable.Count = 0 Then
                                        Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά")
                                    End If
                                    Dim credentials As XRow = credentialsTable.Current
                                    If Courier = 1 Then '---------ACS
                                        notif.ShowBalloonTip(5000, "ACS", "Έκδοση", ToolTipIcon.Info)
                                        acs.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 2 Then '-----ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                        notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Έκδοση", ToolTipIcon.Info)
                                        geniki.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 3 Then '-----SPEEDEX
                                        notif.ShowBalloonTip(5000, "Speedex", "Έκδοση", ToolTipIcon.Info)
                                        speedex.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 4 Then '-----BOX NOW
                                        notif.ShowBalloonTip(5000, "Box Now", "Έκδοση", ToolTipIcon.Info)
                                        boxnow.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 5 Then '-----ΤΑΧΥΔΕΜΑ
                                        notif.ShowBalloonTip(5000, "Ταχυδέμα", "Έκδοση", ToolTipIcon.Info)
                                        taxydema.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 7 Then '-----ΕΛΤΑ COURIER
                                        notif.ShowBalloonTip(5000, "ΕΛΤΑ COURIER", "Έκδοση", ToolTipIcon.Info)
                                        eltaCourier.GetVoucher11700001(credentials, salData, XModule, XSupport)
                                    End If
                                Else
                                    ThrowError(Courier,proceed.code)

                                End If
                            Case 11700002 '----------ΕΚΤΥΠΩΣΗ VOUCHER----------
                                If Not salData("CCCD1VOUCHEREXECUTION") = 1 Then
                                    Throw New Exception("Δεν υπάρχει αποστολή προς εκτύπωση")
                                End If
                                If salData("CCCD1VOUCHEREXECUTION") = 1 And salData("CCCD1VOUCHERDELETED") = 1 Then
                                    Throw New Exception("Η αποστολή έχει ακυρωθεί")
                                End If

                                If IsDBNull(salData("SERIES")) Then
                                    Throw New Exception("Δεν έχετε επιλέξει σειρά")
                                End If
                                Dim Courier = salData("CCCD1COURIERCOMPANY")
                                If IsDBNull(Courier) Then
                                    Throw New Exception("Δεν έχετε επιλέξει μεταφορική εταιρεία")
                                End If

                                Dim proceed = CheckCanGo(Courier, XSupport)
                                If proceed.success Then
                                    Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY={1} AND SERIES LIKE '%{2}%' AND ISACTIVE=1", salData("COMPANY"), Courier, salData("SERIES"))
                                    Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
                                    If credentialsTable.Count = 0 Then
                                        Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά")
                                    End If
                                    Dim credentials As XRow = credentialsTable.Current
                                    If Courier = 1 Then '----------ACS
                                        notif.ShowBalloonTip(5000, "ACS", "Έκτύπωση", ToolTipIcon.Info)
                                        acs.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 2 Then '-----ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                        notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Έκτύπωση", ToolTipIcon.Info)
                                        geniki.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 3 Then '-----SPEEDEX
                                        notif.ShowBalloonTip(5000, "Speedex", "Έκτύπωση", ToolTipIcon.Info)
                                        speedex.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 4 Then '-----ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                        notif.ShowBalloonTip(5000, "Box Now", "Έκτύπωση", ToolTipIcon.Info)
                                        boxnow.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 5 Then '-----ΤΑΧΥΔΕΜΑ
                                        notif.ShowBalloonTip(5000, "Ταχυδέμα", "Έκτύπωση", ToolTipIcon.Info)
                                        taxydema.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 7 Then '-----ΕΛΤΑ COURIER
                                        notif.ShowBalloonTip(5000, "ΕΛΤΑ COURIER", "Έκτύπωση", ToolTipIcon.Info)
                                        eltaCourier.PrintVoucher11700002(credentials, salData, XModule, XSupport)
                                    End If
                                Else
                                    ThrowError(Courier, proceed.code)
                                End If
                            Case 11700003 '----------ΑΚΥΡΩΣΗ VOUCHER----------
                                If Not salData("CCCD1VOUCHEREXECUTION") = 1 Then
                                    Throw New Exception("Δεν υπάρχει αποστολή προς ακύρωση")
                                End If
                                If salData("CCCD1VOUCHEREXECUTION") = 1 And salData("CCCD1VOUCHERDELETED") = 1 Then
                                    Throw New Exception("Η αποστολή έχει ακυρωθεί")
                                End If
                                If IsDBNull(salData("SERIES")) Then
                                    Throw New Exception("Δεν έχετε επιλέξει σειρά")
                                End If
                                Dim Courier = salData("CCCD1COURIERCOMPANY")
                                If IsDBNull(Courier) Then
                                    Throw New Exception("Δεν έχετε επιλέξει μεταφορική εταιρεία")
                                End If

                                Dim proceed = CheckCanGo(Courier, XSupport)
                                If proceed.success Then
                                    Dim ans = XSupport.AskYesNoCancel("Επιβεβαίωση Ακύρωσης", "Θέλετε να προχωρήσετε σε ακύρωση της αποστολής;")
                                    Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY={1} AND SERIES LIKE '%{2}%' AND ISACTIVE=1", salData("COMPANY"), Courier, salData("SERIES"))
                                    Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
                                    If credentialsTable.Count = 0 Then
                                        Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά")
                                    End If
                                    Dim credentials As XRow = credentialsTable.Current
                                    If ans = 6 Then
                                        If Courier = 1 Then '----------ACS
                                            notif.ShowBalloonTip(5000, "ACS", "Ακύρωση", ToolTipIcon.Info)
                                            acs.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        ElseIf Courier = 2 Then '-----ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                            notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Ακύρωση", ToolTipIcon.Info)
                                            geniki.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        ElseIf Courier = 3 Then '-----SPEEDEX
                                            notif.ShowBalloonTip(5000, "Speedex", "Ακύρωση", ToolTipIcon.Info)
                                            speedex.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        ElseIf Courier = 4 Then '-----BOX NOW
                                            notif.ShowBalloonTip(5000, "Box Now", "Ακύρωση", ToolTipIcon.Info)
                                            boxnow.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        ElseIf Courier = 5 Then '-----ΤΑΧΥΔΕΜΑ
                                            notif.ShowBalloonTip(5000, "Ταχυδέμα", "Ακύρωση", ToolTipIcon.Info)
                                            taxydema.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        ElseIf Courier = 7 Then '-----ΕΛΤΑ COURIER
                                            notif.ShowBalloonTip(5000, "ΕΛΤΑ COURIER", "Ακύρωση", ToolTipIcon.Info)
                                            eltaCourier.DeleteVoucher11700003(credentials, salData, XModule, XSupport)
                                        End If
                                    End If
                                Else
                                    ThrowError(Courier, proceed.code)
                                End If
                            Case 11700004 '----------TRACK VOUCHER----------
                                If Not salData("CCCD1VOUCHEREXECUTION") = 1 Then
                                    Throw New Exception("Δεν υπάρχει αποστολή για tracking")
                                End If

                                If IsDBNull(salData("SERIES")) Then
                                    Throw New Exception("Δεν έχετε επιλέξει σειρά")
                                End If
                                Dim Courier = salData("CCCD1COURIERCOMPANY")
                                If IsDBNull(Courier) Then
                                    Throw New Exception("Δεν έχετε επιλέξει μεταφορική εταιρεία")
                                End If

                                Dim proceed = CheckCanGo(Courier, XSupport)
                                If proceed.success Then
                                    Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY={1} AND SERIES LIKE '%{2}%' AND ISACTIVE=1", salData("COMPANY"), Courier, salData("SERIES"))
                                    Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
                                    If credentialsTable.Count = 0 Then
                                        Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά")
                                    End If
                                    Dim credentials As XRow = credentialsTable.Current
                                    If Courier = 1 Then '----------ACS
                                        notif.ShowBalloonTip(5000, "ACS", "Tracking", ToolTipIcon.Info)
                                        acs.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 2 Then '-----ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                        notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Tracking", ToolTipIcon.Info)
                                        geniki.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 3 Then '-----SPEEDEX
                                        notif.ShowBalloonTip(5000, "Speedex", "Tracking", ToolTipIcon.Info)
                                        speedex.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 4 Then '-----BOX NOW
                                        notif.ShowBalloonTip(5000, "Box Now", "Tracking", ToolTipIcon.Info)
                                        boxnow.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 5 Then '-----ΤΑΧΥΔΕΜΑ
                                        notif.ShowBalloonTip(5000, "Ταχυδέμα", "Tracking", ToolTipIcon.Info)
                                        taxydema.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    ElseIf Courier = 7 Then '-----ΕΛΤΑ Courier
                                        notif.ShowBalloonTip(5000, "ΕΛΤΑ Courier", "Tracking", ToolTipIcon.Info)
                                        eltaCourier.TrackVoucher11700004(credentials, salData, XModule, XSupport)
                                    End If
                                Else
                                    ThrowError(Courier, proceed.code)
                                End If
                            Case 11700011 '----------ΜΑΖΙΚΗ ΕΚΔΟΣΗ VOUCHER----------
                                Dim findocIn As String = ""
                                For i = 0 To UBound(XModule.Params)
                                    If XModule.Params(i) Is Nothing Then
                                        Exit For
                                    ElseIf XModule.Params(i).Split("=")(0) = "SELRECS" Then
                                        findocIn = XModule.Params(i).Split("=")(1).Replace("?", ",")
                                    End If
                                Next i

                                Dim Error_list_Messages As New List(Of String)()
                                Dim Error_list_Fincode As New List(Of String)()
                                Dim Error_list_Findoc As New List(Of String)()
                                Dim Success_list As New List(Of String)()

                                Dim str As String = String.Format("SELECT * FROM FINDOC WHERE {0} AND COMPANY={1}", findocIn, XSupport.ConnectionInfo.CompanyId)
                                Dim ds As XTable = XSupport.GetSQLDataSet(str)
                                notif.ShowBalloonTip(5000, "Courier", "Έναρξη έκδοσης", ToolTipIcon.Info)

                                For i = 0 To ds.Count - 1

                                    Dim Error_Message = ""

                                    Dim company = ds.Item(i, "COMPANY")
                                    Dim courier = ds.Item(i, "CCCD1COURIERCOMPANY")
                                    If ds.Item(i, "CCCD1VOUCHEREXECUTION") = 1 And ds.Item(i, "CCCD1VOUCHERDELETED") = 0 Then
                                        Error_list_Messages.Add("Έχει εκδοθεί ήδη Voucher για το παραστατικό ")
                                        Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                                        Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                                        Continue For
                                    Else
                                        If IsDBNull(courier) Then
                                            Error_list_Messages.Add("Δεν έχετε επιλέξει μεταφορική για το παραστατικό")
                                            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                                            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                                            Continue For
                                        Else
                                            Dim proceed = CheckCanGo(courier, XSupport)
                                            If proceed.success Then
                                                If courier = 1 Then '----------ACS
                                                    acs.GetMassVoucher11700011(ds, i, Success_list, Error_list_Messages, Error_list_Fincode, Error_list_Findoc, XSupport)
                                                ElseIf courier = 2 Then '------ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                                    geniki.GetMassVoucher11700011(ds, i, Success_list, Error_list_Messages, Error_list_Fincode, Error_list_Findoc, XSupport)
                                                ElseIf courier = 3 Then '------SPEEDEX
                                                    speedex.GetMassVoucher11700011(ds, i, Success_list, Error_list_Messages, Error_list_Fincode, Error_list_Findoc, XSupport)
                                                ElseIf courier = 4 Then '------ΒΟΧ ΝΟ΅΅W
                                                    boxnow.GetMassVoucher11700011(ds, i, Success_list, Error_list_Messages, Error_list_Fincode, Error_list_Findoc, XSupport)
                                                ElseIf courier = 5 Then '------ΤΑΧΥΔΕΜΑ
                                                    taxydema.GetMassVoucher11700011(ds, i, Success_list, Error_list_Messages, Error_list_Fincode, Error_list_Findoc, XSupport)
                                                End If
                                            Else
                                                ThrowError(courier, proceed.code)
                                                Exit For
                                            End If
                                        End If

                                    End If

                                Next i
                                If Success_list.Count > 0 Then
                                    Dim Success_Message_Title As String = "Επιτυχής έκδοση Voucher: "
                                    Dim Success_Message_Body As String = ""
                                    For i As Integer = 0 To Success_list.Count - 1 Step 1
                                        Success_Message_Body += vbCrLf + Success_list(i)
                                    Next
                                    XSupport.Warning(Success_Message_Title + Success_Message_Body)
                                End If

                                If Error_list_Findoc.Count > 0 Then
                                    Dim Errorform = New MassVoucherError()
                                    For i As Integer = 0 To Error_list_Findoc.Count - 1 Step 1
                                        Dim x As String() = {Error_list_Fincode(i), Error_list_Messages(i), Error_list_Findoc(i)}
                                        Errorform.DataGridView1.Rows.Add(x)
                                    Next
                                    XXX = XSupport
                                    Errorform.Show()
                                End If
                            Case 11700012 '----------ΜΑΖΙΚΗ ΕΚΤΥΠΩΣΗ VOUCHER----------
                                Dim findocIn As String = ""
                                For i = 0 To UBound(XModule.Params)
                                    If XModule.Params(i) Is Nothing Then
                                        Exit For
                                    ElseIf XModule.Params(i).Split("=")(0) = "SELRECS" Then
                                        findocIn = XModule.Params(i).Split("=")(1).Replace("?", ",")
                                    End If

                                Next i
                                Dim Error_list_Messages As New List(Of String)()
                                Dim Error_list_Fincode As New List(Of String)()
                                Dim Error_list_Findoc As New List(Of String)()
                                Dim Success_list As New List(Of String)()

                                Dim str As String = String.Format("SELECT * FROM FINDOC WHERE {0} AND COMPANY={1}", findocIn, XSupport.ConnectionInfo.CompanyId)
                                Dim ds As XTable = XSupport.GetSQLDataSet(str)
                                Dim VoucherListForGeniki As New List(Of List(Of String))
                                Dim OrderListForBoxNow As New List(Of List(Of String))
                                notif.ShowBalloonTip(5000, "Courier", "Έναρξη εκτύπωσης", ToolTipIcon.Info)

                                For i = 0 To ds.Count - 1

                                    Dim Error_Message = ""

                                    Dim courier = ds.Item(i, "CCCD1COURIERCOMPANY")
                                    If ds.Item(i, "CCCD1VOUCHEREXECUTION") = 0 Then
                                        Error_list_Messages.Add("Δεν υπάρχει Voucher προς εκτύπωση ")
                                        Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                                        Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                                        Continue For
                                    Else
                                        If IsDBNull(courier) Then
                                            Error_list_Messages.Add("Δεν έχετε επιλέξει μεταφορική για το παραστατικό")
                                            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                                            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                                            Continue For
                                        Else
                                            Dim proceed = CheckCanGo(courier, XSupport)
                                            If proceed.success Then
                                                If courier = 1 Then
                                                    acs.PrintMassVoucher11700012(ds, i, XModule, XSupport)
                                                ElseIf courier = 2 Then 'ΓΕΝΙΚΗ ΤΑΧΥΔΡΟΜΙΚΗ
                                                    VoucherListForGeniki.Add(New List(Of String)({ds.Item(i, "CCCD1VOUCHERNO").ToString, ds.Item(i, "FINDOC").ToString, ds.Item(i, "SERIES").ToString}))
                                                ElseIf courier = 3 Then 'SPEEDEX
                                                    speedex.PrintMassVoucher11700012(ds, i, XModule, XSupport)
                                                ElseIf courier = 4 Then 'BOX NOW
                                                    If Not IsNothing(ds.Item(i, "CCCD1BOXNOWLASTORDERNO")) And Not IsDBNull(ds.Item(i, "CCCD1BOXNOWLASTORDERNO")) Then
                                                        OrderListForBoxNow.Add(New List(Of String)({ds.Item(i, "CCCD1BOXNOWLASTORDERNO").ToString, ds.Item(i, "FINDOC").ToString, ds.Item(i, "SERIES").ToString}))
                                                    End If
                                                ElseIf courier = 5 Then 'ΤΑΧΥΔΕΜΑ
                                                    taxydema.PrintMassVoucher11700012(ds, i, XModule, XSupport)
                                                End If
                                            Else
                                                ThrowError(courier, proceed.code)
                                                Exit For
                                            End If
                                        End If

                                    End If
                                Next i
                                If VoucherListForGeniki.Count > 0 Then
                                    geniki.PrintMassVoucher11700012(VoucherListForGeniki, XSupport)
                                End If
                                If OrderListForBoxNow.Count > 0 Then
                                    boxnow.PrintMassVoucher11700012(OrderListForBoxNow, XSupport)
                                End If
                                XSupport.Warning("Ολοκλήρωση εκτύπωσης")
                                If Error_list_Findoc.Count > 0 Then
                                    Dim Errorform = New MassVoucherError()
                                    For i As Integer = 0 To Error_list_Findoc.Count - 1 Step 1
                                        Dim x As String() = {Error_list_Fincode(i), Error_list_Messages(i), Error_list_Findoc(i)}
                                        Errorform.DataGridView1.Rows.Add(x)
                                    Next
                                    XXX = XSupport
                                    Errorform.Show()
                                End If
                        End Select
                    Catch ex As Exception
                        XSupport.Exception(ex.Message)
                    End Try
                    Return 0
                End Function

            End Class

            <WorksOn("CCCD1COURIERCONFIG")>
            Public Class CCCD1COURIERCONFIG
                Inherits TXCode
                Public Overrides Function EXECCOMMAND(cmd As Integer)
                    ServicePointManager.Expect100Continue = True
                    ServicePointManager.SecurityProtocol = DirectCast(3072, SecurityProtocolType)  'its same like SecurityProtocolType.Tls12
                    ServicePointManager.DefaultConnectionLimit = 9999

                    Dim data As XRow = XModule.GetTable("CCCD1COURIERCONFIG").Current
                    If cmd = 11700001 Then
                        Dim courier = data("COURIERCOMPANY")
                        If IsDBNull(courier) Then
                            XSupport.Exception("Επιλέξτε Eταιρεία Courier")
                        Else
                            Dim response = Renew(XSupport.ConnectionInfo.SerialNum.ToString, courier)
                            If response.success Then
                                Dim signature = SimpleEncrypt(courier.ToString + "&" + response.blcdate)
                                Dim ModuleTbl = XSupport.GetSQLDataSet("SELECT * FROM CCCD1COURIERSMODULE WHERE COURIER=" + courier.ToString)
                                If ModuleTbl.Count > 0 Then
                                    XSupport.ExecuteSQL("UPDATE CCCD1COURIERSMODULE SET BLCDATE='" + Convert.ToDateTime(response.blcdate).ToString("yyyyMMdd") + "', SIGNATURE='" + signature + "' WHERE COURIER=" + courier.ToString)
                                Else
                                    XSupport.ExecuteSQL("INSERT INTO CCCD1COURIERSMODULE (COURIER,BLCDATE,SIGNATURE) VALUES(" + courier.ToString + ",'" + Convert.ToDateTime(response.blcdate).ToString("yyyyMMdd") + "','" + signature + "')")
                                End If
                                XSupport.Warning(CourierNames(courier) + vbCrLf + "Επιτυχής αναθεώρηση αδειας " + vbCrLf + "Λήξη: " + Convert.ToDateTime(response.blcdate).ToString("dd/MM/yyyy"))
                            Else
                                XSupport.Exception("Error: " + response.error)
                            End If
                        End If
                    End If
                    Return 0
                End Function
            End Class

            <WorksOn("CCCD1FINALIZEVOUCHER")>
            Public Class CCCD1FINALIZEVOUCHER
                Inherits TXCode
                Public Overrides Function EXECCOMMAND(cmd As Integer)
                    Dim data As XRow = XModule.GetTable("CCCD1FINALIZEVOUCHER").Current
                    If cmd = 11700711 Then
                        Process.Start(data("FILELOCATION"))
                    End If
                    Return 0
                End Function

                Public Overrides Sub BeforePost()
                    Dim data As XRow = XModule.GetTable("CCCD1FINALIZEVOUCHER").Current
                    Try
                        Dim courierConfig = data("CCCD1COURIERCONFIG")
                        If IsDBNull(courierConfig) Then
                            Throw New Exception("Δεν έχετε επιλέξει Εταιρεία Courier")
                        End If
                        Dim queryStr = String.Format("SELECT * FROM CCCD1COURIERCONFIG WHERE CCCD1COURIERCONFIG={0}", courierConfig)
                        Dim credentials = XSupport.GetSQLDataSet(queryStr).Current


                        Dim proceed = CheckCanGo(credentials("COURIERCOMPANY"), XSupport)
                        If proceed.success Then
                            If credentials("COURIERCOMPANY") = 1 Then
                                notif.ShowBalloonTip(5000, "ACS", "Οριστικοποίηση", ToolTipIcon.Info)
                                acs.FinalizeVoucher(credentials, data, XSupport)
                            ElseIf credentials("COURIERCOMPANY") = 2 Then
                                notif.ShowBalloonTip(5000, "Γενική Ταχυδρομική", "Οριστικοποίηση", ToolTipIcon.Info)
                                geniki.FinalizeVoucher(credentials, data, XSupport)
                            End If
                        Else
                            ThrowError(credentials("COURIERCOMPANY"), proceed.code)
                        End If

                    Catch ex As Exception
                        XSupport.Exception(ex.Message)
                    End Try

                End Sub

            End Class
        End Class
    End Class
End Namespace