Imports System.Collections.Specialized
Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Reflection
Imports System.Security.Policy
Imports System.ServiceModel.Security
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Softone

Module boxnow

    Private ErrorCodes As New Dictionary(Of String, String) From {
        {"P400", "Invalid request data"},
        {"P401", "Invalid request origin location reference"},
        {"P402", "Invalid request destination location reference"},
        {"P403", "You are not allowed to use AnyAPM-SameAPM delivery"},
        {"P404", "Invalid import CSV"},
        {"P405", "Invalid Phone Number"},
        {"P406", "Invalid Compartment/Parcel Size"},
        {"P407", "Invalid Couuntry Code"},
        {"P410", "Order Number Conflict"},
        {"P411", "You are not eligible to use Cash-on-delivery payment type"},
        {"P420", "Parcel not ready for cancel"},
        {"P430", "Parcel not ready for AnyAPM confirmation"}
    }

    Private TracingCodes As New Dictionary(Of String, String) From {
        {"new", "Parcel has been registered to the system"},
        {"delivered", "Parcel has been delivered"},
        {"expired", "Parcel expired and will be returned to the sender"},
        {"returned", "Parcel has been returned to the sender"},
        {"in-depot", "Parcel is in one of our warehouses"},
        {"final-destination", "Parcel has reached its final destination, waiting for pickup"},
        {"canceled", "Parcel order had been canceled by the sender"},
        {"accepted-for-return", "Parcel has been accepted from customer and will be returned to the sender"},
        {"missing", "Box Now pickup courier was unable to obtain the parcel for delivery"},
        {"accepted-to-locker", "Parcel has been accepted from customer and will be sent to the recipient"}
    }

    Private Class DeliveryRequestClass
        Public orderNumber As String
        Public invoiceValue As String = "0.0"
        Public paymentMode As String
        Public amountToBeCollected As String = "0.00"
        Public allowReturn As Boolean = True
        Public origin As Location
        Public destination As Location
        Public items As List(Of Items)
    End Class

    Private Class Location
        Public contactNumber As String
        Public contactEmail As String
        Public contactName As String
        Public locationId As String
    End Class

    Private Class Items
        Public id As String
        Public name As String
        Public value As String = "0.00"
        Public weight As Double
    End Class

    Private Class LabelPrint
        Public orderNumbers As List(Of String)
        Public paperSize As String = "A6"
        Public perPage As Integer = 1
    End Class

    Private Function Authentication(client_id As String, client_secret As String, url As String) As Dictionary(Of String, Object)
        Dim webClient As New WebClient()

        Dim resByte As Byte()
        Dim resString As String
        Dim reqString() As Byte
        Dim reqData As New Dictionary(Of String, Object) From {
            {"grant_type", "client_credentials"},
            {"client_id", client_id},
            {"client_secret", client_secret}
        }

        Try
            webClient.Headers("content-type") = "application/json"
            reqString = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reqData, Formatting.Indented))
            resByte = webClient.UploadData(url + "/auth-sessions", "post", reqString)
            resString = Encoding.UTF8.GetString(resByte)
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(resString)
            'Dim access_token = jsonResulttodict.Item("access_token")
            webClient.Dispose()
            Return jsonResulttodict
        Catch ex As WebException
            Dim SR = New StreamReader(ex.Response.GetResponseStream())
            Dim response = SR.ReadToEnd()
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(response)
            Return jsonResulttodict
        End Try
    End Function

    Public Function SendJsonPost(json As String, url As String, token As String) As Object

        Dim request As WebRequest = WebRequest.Create(url)
        request.Method = "POST"
        request.ContentType = "application/json"
        request.Headers.Add("Authorization", "Bearer " + token)

        Dim bytes As Byte() = Encoding.UTF8.GetBytes(json)

        Using stream As Stream = request.GetRequestStream()
            stream.Write(bytes, 0, bytes.Length)
        End Using

        Try
            Dim response As WebResponse = request.GetResponse()
            Dim responseStream As Stream = response.GetResponseStream()
            Dim responseObj As Object
            Using reader As New StreamReader(responseStream)
                Dim responseJson As String = reader.ReadToEnd()
                responseObj = JsonConvert.DeserializeObject(responseJson)
            End Using
            Return responseObj
        Catch ex As WebException
            Dim SR = New StreamReader(ex.Response.GetResponseStream())
            Dim response = SR.ReadToEnd()
            Dim jsonResulttodict = JsonConvert.DeserializeObject(response)
            Return jsonResulttodict
        End Try
    End Function

    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim mandatory As String = ""
        Dim fincode = If(salData("FINDOC") < 0, salData("CMPFINCODE"), salData("FINCODE"))
        If IsDBNull(fincode) Then
            mandatory = "Αριθμό Παραστατικού"
        End If
        Dim Destination_Name = salData("CCCD1SHIPNAME")
        If IsDBNull(Destination_Name) Then
            mandatory += If(mandatory = "", "Όνομα Παράδοσης", "," & vbCrLf & "Όνομα Παράδοσης")
        End If
        If IsDBNull(salData("CCCD1SHIPCELLPHONE")) Then
            mandatory += If(mandatory = "", "Τηλ. Παράδοσης", "," & vbCrLf & "Τηλ. Παράδοσης")
        End If
        Dim Destination_Email = salData("CCCD1SHIPEMAIL")
        If IsDBNull(Destination_Email) Then
            mandatory += If(mandatory = "", "e-mail Παράδοσης", "," & vbCrLf & "e-mail Παράδοσης")
        End If
        Dim Destination_Location = salData("CCCD1BOXNOWLOCID")
        If IsDBNull(Destination_Location) Then
            mandatory += If(mandatory = "", "Location Παράδοσης", "," & vbCrLf & "Location Παράδοσης")
        End If
        If IsDBNull(salData("CCCD1VOUCHERQUANTITY")) Then
            mandatory += If(mandatory = "", "Τεμάχια", "," & vbCrLf & "Τεμάχια")
        Else
            If Not salData("CCCD1VOUCHERQUANTITY") > 0 Then
                mandatory += If(mandatory = "", "Τεμάχια", "," & vbCrLf & "Τεμάχια")
            End If
        End If
        If IsDBNull(salData("CCCD1VOUVHERWEIGHT")) Then
            mandatory += If(mandatory = "", "Βάρος", "," & vbCrLf & "Βάρος")
        Else
            If Not salData("CCCD1VOUVHERWEIGHT") > 0 Then
                mandatory += If(mandatory = "", "Βάρος", "," & vbCrLf & "Βάρος")
            End If
        End If
        Dim Destination_Number = If(salData("CCCD1SHIPCELLPHONE").Substring(0, 1) = "+", salData("CCCD1SHIPCELLPHONE"), "+30" & salData("CCCD1SHIPCELLPHONE"))

        Dim paymentMode
        Dim ammountToBeCollected = 0.00
        Dim invoiceValue = 0.00
        Dim vPayment = If(IsDBNull(credentials("PAYMENT")), "", credentials("PAYMENT"))
        Dim vItsCod As Boolean = vPayment.ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
        If vItsCod Then
            salData("CCCD1BOXNOWPAYMENTMODE") = 2
            paymentMode = "cod"
            salData("CCCD1VOUCHERVALUE") = salData("SUMAMNT")
            If IsDBNull(salData("CCCD1VOUCHERVALUE")) Then
                mandatory += If(mandatory = "", "Αξια Voucher", "," & vbCrLf & "Αξια Voucher")
            Else
                If Not salData("CCCD1VOUCHERVALUE") > 0 Then
                    mandatory += If(mandatory = "", "Αξια Voucher", "," & vbCrLf & "Αξια Voucher")
                Else
                    ammountToBeCollected = salData("CCCD1VOUCHERVALUE")
                End If
            End If
        Else
            salData("CCCD1BOXNOWPAYMENTMODE") = 1
            paymentMode = "prepaid"
        End If

        If Not mandatory = "" Then
            Throw New Exception("Δεν έχετε συμπληρώσει:" & vbCrLf & mandatory)
        End If

        Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
        If session.ContainsKey("code") Then
            Throw New Exception("Error on session")
        End If

        Dim Mtrdoc_Table = XModule.GetTable("MTRDOC")
        Dim querStr = String.Format("SELECT CCCD1BOXNOWLOCID LOCATION FROM WHOUSE WHERE COMPANY={0} AND WHOUSE={1}", XSupport.ConnectionInfo.CompanyId, Mtrdoc_Table.Current("WHOUSE"))
        Dim Whouse_ds As XTable = XSupport.GetSQLDataSet(querStr)

        If IsDBNull(Whouse_ds.Current("LOCATION")) Then
            Throw New Exception("Δεν έχετε συμπληρώσει Warehouse ID στον αποθηκευτικό χώρο.")
        End If

        Dim rand As New Random()
        Dim orderNumber = rand.Next(10000, 99999).ToString + "-" + If(salData("FINDOC") < 0, salData("CMPFINCODE"), salData("FINCODE"))
        Dim allowReturn As Boolean = salData("CCCD1BOXNOWALLOWRETURN") = 1
        If IsDBNull(salData("SUMAMNT")) Then
            invoiceValue = 0.00
        Else
            invoiceValue = If(salData("SUMAMNT"), 0.00)
        End If


        Dim obj As New DeliveryRequestClass With {
            .orderNumber = orderNumber,
            .invoiceValue = invoiceValue.ToString("0.00").Replace(",", "."),
            .paymentMode = paymentMode,
            .amountToBeCollected = ammountToBeCollected.ToString("0.00").Replace(",", "."),
            .allowReturn = allowReturn,
            .origin = New Location With {
                .contactEmail = credentials("BOXNOWEMAIL"),
                .contactName = credentials("BOXNOWORIGINNAME"),
                .contactNumber = credentials("BOXNOWPHONE"),
                .locationId = Whouse_ds.Current("LOCATION")
            },
            .destination = New Location With {
                .contactEmail = Destination_Email,
                .contactName = Destination_Name,
                .contactNumber = Destination_Number,
                .locationId = Destination_Location
            },
            .items = New List(Of Items)
        }

        For i = 1 To salData("CCCD1VOUCHERQUANTITY")
            Dim item As New Items With {
                .id = i.ToString,
                .name = "Parcel-" + i.ToString,
                .weight = salData("CCCD1VOUVHERWEIGHT") / salData("CCCD1VOUCHERQUANTITY")
            }
            obj.items.Add(item)
        Next

        salData("CCCD1SHIPCELLPHONE") = Destination_Number
        Dim json As String = JsonConvert.SerializeObject(obj)
        Dim requestDelivery = SendJsonPost(json, credentials("URL") + "/delivery-requests", session("access_token"))
        If requestDelivery.ContainsKey("code") Then
            Throw New Exception("Error " & requestDelivery("code") & vbCrLf & ErrorCodes(requestDelivery("code")))
        End If

        Dim parcels = requestDelivery.Item("parcels").ToString
        Dim parcelstodict = JsonConvert.DeserializeObject(parcels)

        Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
        If SubvTable.Count > 0 Then 'Αν έχει ήδη άλλα Subvouchers
            While SubvTable.Count > 0
                SubvTable.Current.Delete() 'Τα διαγραφω γραμμη γραμμη
            End While

        End If


        For Each item As JObject In parcelstodict
            SubvTable.Current.Append()
            SubvTable.Current("VOUCHER") = item("id").ToString
            SubvTable.Current.Post()
        Next

        salData("CCCD1BOXNOWLASTORDERNO") = orderNumber
        salData("CCCD1VOUCHEREXECUTION") = 1
        salData("CCCD1VOUCHERDELETED") = 0

        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
        If session.ContainsKey("code") Then
            Throw New Exception("Error on session")
        End If
        Dim folderpath = credentials("FOLDERPATH")
        Dim strFileLocation = folderpath + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
        Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1BOXNOWLASTORDERNO").ToString + ".pdf"
        Dim folderexists As Boolean = Directory.Exists(strFileLocation)
        Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
        If Not folderexists Then
            Directory.CreateDirectory(strFileLocation)
        End If
        Try
            If Not pdfgexists Then
                Dim printVoucher As New LabelPrint With {
                    .paperSize = If(credentials("VOUCHERPRINTTYPE") = 4, "A4", "A6"),
                    .perPage = credentials("BOXNOWPERPAGE"),
                    .orderNumbers = New List(Of String) From {
                        salData("CCCD1BOXNOWLASTORDERNO").ToString
                    }
                }
                Dim json As String = JsonConvert.SerializeObject(printVoucher)
                Dim request As WebRequest = WebRequest.Create(credentials("URL") + "/labels:search")
                request.Method = "POST"
                request.ContentType = "application/json"
                request.Headers.Add("Authorization", "Bearer " + session("access_token"))

                Using streamWriter As New StreamWriter(request.GetRequestStream())
                    streamWriter.Write(json)
                    streamWriter.Flush()
                    streamWriter.Close()
                End Using

                Try
                    Dim response As WebResponse = request.GetResponse()
                    Dim contentType As String = response.ContentType

                    If contentType = "application/pdf" Then
                        ' Download the PDF file
                        Using responseStream As Stream = response.GetResponseStream()
                            Using fileStream As New FileStream(strPDFLocation, FileMode.Create)
                                responseStream.CopyTo(fileStream)
                            End Using
                        End Using
                    ElseIf contentType = "application/json" Then
                        ' Read and process the JSON response
                        Dim responseObj As Object
                        Using responseStream As Stream = response.GetResponseStream()
                            Using streamReader As New StreamReader(responseStream)
                                Dim jsonResponse As String = streamReader.ReadToEnd()
                                responseObj = JsonConvert.DeserializeObject(jsonResponse)
                                Throw New Exception("Error " & responseObj("code") & vbCrLf & ErrorCodes(responseObj("code")))
                            End Using
                        End Using
                    End If

                    response.Close()
                Catch ex As WebException
                    Dim SR = New StreamReader(ex.Response.GetResponseStream())
                    Dim response = SR.ReadToEnd()
                    Dim jsonResulttodict = JsonConvert.DeserializeObject(response)
                    Throw New Exception("Error " & jsonResulttodict("code") & vbCrLf & ErrorCodes(jsonResulttodict("code")))
                End Try
                If credentials("INSTANTPRINT") Then
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
                salData("CCCD1VOUCHERPRINTED") = 1
                XSupport.Warning("Ολοκλήρωση εκτύπωσης")
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try


        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
        If session.ContainsKey("code") Then
            Throw New Exception("Error on session")
        End If

        Dim ParcelTbl = XModule.GetTable("CCCD1SUBVOUCHERS")
        While ParcelTbl.Count > 0
            Dim webClient As New WebClient()
            Dim resString As String
            Dim resByte As Byte()
            Try
                webClient.Headers.Add("Authorization", "Bearer " + session("access_token"))
                resByte = webClient.UploadValues(credentials("URL") + "/parcels/" + ParcelTbl.Current("VOUCHER") + ":cancel", "POST", New NameValueCollection())
                resString = Encoding.UTF8.GetString(resByte)
                Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(resString)
                webClient.Dispose()
                If jsonResulttodict Is Nothing Then
                    ParcelTbl.Current.Delete()
                End If
            Catch ex As WebException
                Dim SR = New StreamReader(ex.Response.GetResponseStream())
                Dim response = SR.ReadToEnd()
                Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(response)
                If jsonResulttodict.ContainsKey("code") Then
                    Throw New Exception("Error " & jsonResulttodict("code") & vbCrLf & jsonResulttodict("message"))
                End If
            End Try
        End While
        salData("CCCD1VOUCHEREXECUTION") = 0
        salData("CCCD1VOUCHERDELETED") = 1
        salData("CCCD1VOUCHERPRINTED") = 0
        salData("CCCD1BOXNOWLASTORDERNO") = ""
        XSupport.Warning("Ολοκλήρωση ακύρωσης")
        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
        If session.ContainsKey("code") Then
            Throw New Exception("Error on session")
        End If

        Dim webClient As New WebClient()
        Dim resString As String
        Try
            webClient.Headers.Add("Authorization", "Bearer " + session("access_token"))
            resString = webClient.DownloadString(credentials("URL") + "/parcels?orderNumber=" + salData("CCCD1BOXNOWLASTORDERNO"))
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(resString)
            webClient.Dispose()
            If jsonResulttodict.ContainsKey("error") Then
                Throw New Exception("Error while fetching")
            End If
            Dim Trform = New TrackingForm()
            If CInt(jsonResulttodict.Item("count")) > 0 Then
                Dim data = jsonResulttodict.Item("data").first().Item("events")
                For Each item In data
                    Trform.DataGridView1.Rows.Add(item.Item("createTime"), TracingCodes(item.Item("type")), item.Item("locationDisplayName"), "")
                Next
                Trform.Show()
            Else
                Throw New Exception("No tracking history available")
            End If
        Catch ex As WebException
            Dim SR = New StreamReader(ex.Response.GetResponseStream())
            Dim response = SR.ReadToEnd()
            Dim jsonResulttodict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(response)
            If jsonResulttodict.ContainsKey("error") Then
                Throw New Exception("Error while fetching")
            End If
        End Try

        Return 0
    End Function

    Public Function GetMassVoucher11700011(ds As XTable, i As Integer, Success_list As List(Of String), Error_list_Messages As List(Of String), Error_list_Fincode As List(Of String), Error_list_Findoc As List(Of String), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=4 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Error_list_Messages.Add("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
        Else
            Dim credentials = credentialsTable.Current
            Dim entryError = ""
            Dim Destination_Name = ds.Item(i, "CCCD1SHIPNAME")
            Dim Destination_Email = ds.Item(i, "CCCD1SHIPEMAIL")
            Dim Destination_Location = ds.Item(i, "CCCD1BOXNOWLOCID")
            If IsDBNull(Destination_Name) Then
                entryError += If(entryError = "", "Όνομα Παράδοσης", ", Όνομα Παράδοσης")
            End If
            If IsDBNull(ds.Item(i, "CCCD1SHIPCELLPHONE")) Then
                entryError += If(entryError = "", "Τηλ. Παράδοσης", ", Τηλ. Παράδοσης")
            End If
            If IsDBNull(Destination_Email) Then
                entryError += If(entryError = "", "e-mail Παράδοσης", ", e-mail Παράδοσης")
            End If
            If IsDBNull(Destination_Location) Then
                entryError += If(entryError = "", "Location Παράδοσης", ", Location Παράδοσης")
            End If
            If IsDBNull(ds.Item(i, "CCCD1BOXNOWPAYMENTMODE")) Then
                entryError += If(entryError = "", "Τρόπο πληρωμής", ", Τρόπο πληρωμής")
            End If
            If IsDBNull(ds.Item(i, "CCCD1VOUCHERQUANTITY")) Then
                entryError += If(entryError = "", "Τεμάχια", ", Τεμάχια")
            Else
                If Not ds.Item(i, "CCCD1VOUCHERQUANTITY") > 0 Then
                    entryError += If(entryError = "", "Τεμάχια", ", Τεμάχια")
                End If
            End If

            If IsDBNull(ds.Item(i, "CCCD1VOUVHERWEIGHT")) Then
                entryError += If(entryError = "", "Βαρος", ", Βαρος")
            Else
                If Not ds.Item(i, "CCCD1VOUVHERWEIGHT") > 0 Then
                    entryError += If(entryError = "", "Βαρος", ", Βαρος")
                End If
            End If

            If ds.Item(i, "CCCD1BOXNOWPAYMENTMODE") = 2 Then
                If IsDBNull(ds.Item(i, "CCCD1VOUCHERVALUE")) Then
                    entryError += If(entryError = "", "Αξία Αντικαταβολης", ", Αξία Αντικαταβολης")
                Else
                    If Not ds.Item(i, "CCCD1VOUCHERVALUE") > 0 Then
                        entryError += If(entryError = "", "Αξία Αντικαταβολης", ", Αξία Αντικαταβολης")
                    End If
                End If
            End If
            If Not entryError = "" Then
                Error_list_Messages.Add("Δεν έχετε συμπληρώσει τα πεδία : " + entryError)
                Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
            Else
                Dim Destination_Number = If(ds(i, "CCCD1SHIPCELLPHONE").Substring(0, 1) = "+", ds(i, "CCCD1SHIPCELLPHONE"), "+30" & ds(i, "CCCD1SHIPCELLPHONE"))
                Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
                If session.ContainsKey("code") Then
                    Throw New Exception("Error on session")
                End If
                Dim ammountToBeCollected = 0.00
                Dim invoiceValue = 0.00

                If IsDBNull(ds.Item(i, "SUMAMNT")) Then
                    invoiceValue = 0.00
                Else
                    invoiceValue = If(ds.Item(i, "SUMAMNT"), 0.00)
                End If

                If ds.Item(i, "CCCD1BOXNOWPAYMENTMODE") = 2 Then
                    ammountToBeCollected = ds.Item(i, "CCCD1VOUCHERVALUE")
                End If
                Dim paymentMode = If(ds.Item(i, "CCCD1BOXNOWPAYMENTMODE") = 2, "cod", "prepaid")
                Dim rand As New Random()
                Dim allowReturn As Boolean = ds.Item(i, "CCCD1BOXNOWALLOWRETURN") = 1
                Dim orderNumber = rand.Next(10000, 99999).ToString + "-" + ds.Item(i, "FINCODE")

                Dim Mtrdoc_Table = XSupport.GetSQLDataSet("SELECT WHOUSE FROM MTRDOC WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString)
                Dim querStr = String.Format("SELECT CCCD1BOXNOWLOCID LOCATION FROM WHOUSE WHERE COMPANY={0} AND WHOUSE={1}", XSupport.ConnectionInfo.CompanyId, Mtrdoc_Table.Current("WHOUSE"))
                Dim Whouse_ds As XTable = XSupport.GetSQLDataSet(querStr)


                Dim obj As New DeliveryRequestClass With {
                    .orderNumber = orderNumber,
                    .invoiceValue = invoiceValue.ToString("0.00").Replace(",", "."),
                    .paymentMode = paymentMode,
                    .amountToBeCollected = ammountToBeCollected.ToString("0.00").Replace(",", "."),
                    .allowReturn = allowReturn,
                    .origin = New Location With {
                        .contactEmail = credentials("BOXNOWEMAIL"),
                        .contactName = credentials("BOXNOWORIGINNAME"),
                        .contactNumber = credentials("BOXNOWPHONE"),
                        .locationId = Whouse_ds.Current("LOCATION")
                    },
                    .destination = New Location With {
                        .contactEmail = Destination_Email,
                        .contactName = Destination_Name,
                        .contactNumber = Destination_Number,
                        .locationId = Destination_Location
                    },
                    .items = New List(Of Items)
                }


                For y = 1 To ds.Item(i, "CCCD1VOUCHERQUANTITY")
                    Dim item As New Items With {
                        .id = i.ToString,
                        .name = "Parcel-" + i.ToString,
                        .weight = ds.Item(i, "CCCD1VOUVHERWEIGHT") / ds.Item(i, "CCCD1VOUCHERQUANTITY")
                    }
                    obj.items.Add(item)
                Next y

                Dim json As String = JsonConvert.SerializeObject(obj)
                Dim requestDelivery = SendJsonPost(json, credentials("URL") + "/delivery-requests", session("access_token"))
                If requestDelivery.ContainsKey("code") Then
                    Error_list_Messages.Add("Error " & requestDelivery("code") & ErrorCodes(requestDelivery("code")))
                    Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                    Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                Else
                    Dim parcels = requestDelivery.Item("parcels").ToString
                    Dim parcelstodict = JsonConvert.DeserializeObject(parcels)
                    For Each item As JObject In parcelstodict
                        Dim insertStr As String = "INSERT INTO CCCD1SUBVOUCHERS (VOUCHER, FINDOC) " +
                                                "VALUES ('" + item("id").ToString + "'," + ds.Item(i, "FINDOC").ToString + ")"
                        XSupport.ExecuteSQL(insertStr)
                        Dim UpdateStr As String = "UPDATE FINDOC SET CCCD1BOXNOWLASTORDERNO='" + orderNumber + "', CCCD1VOUCHEREXECUTION=1, CCCD1VOUCHERDELETED=0, CCCD1SHIPCELLPHONE='" + Destination_Number + "' WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString
                        XSupport.ExecuteSQL(UpdateStr)
                        Success_list.Add(ds.Item(i, "FINCODE").ToString + " : " + item("id").ToString)
                    Next
                End If
            End If
        End If
        Return 0
    End Function

    Public Function PrintMassVoucher11700012(orderList As List(Of List(Of String)), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=4 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", XSupport.ConnectionInfo.CompanyId, orderList(0)(2))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + orderList(0)(2))
        Else
            Dim credentials As XRow = credentialsTable.Current
            Dim session = Authentication(credentials("BOXNOWCLIENTID"), credentials("BOXNOWCLIENTSECRET"), credentials("URL"))
            If session.ContainsKey("code") Then
                Throw New Exception("Error on session")
            End If
            Dim folderpath = credentials("FOLDERPATH")
            Dim strFileLocation = folderpath + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
            Dim strPDFLocation = strFileLocation + "\AllDay.pdf"
            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
            If Not folderexists Then
                Directory.CreateDirectory(strFileLocation)
            End If
            If pdfgexists Then
                File.Delete(strPDFLocation)
            End If
            Try
                Dim findocIn As String = ""

                Dim printVoucher As New LabelPrint With {
                    .paperSize = If(credentials("VOUCHERPRINTTYPE") = 4, "A4", "A6"),
                    .perPage = credentials("BOXNOWPERPAGE")
                }
                Dim Counter = 0
                Dim orderNumbers As New List(Of String)
                For Each order In orderList
                    Counter += 1
                    If Counter = orderList.Count Then
                        findocIn += order(1)
                    Else
                        findocIn += order(1) + ","
                    End If

                    orderNumbers.Add(order(0))
                Next
                printVoucher.orderNumbers = orderNumbers
                Dim json As String = JsonConvert.SerializeObject(printVoucher)
                Dim request As WebRequest = WebRequest.Create(credentials("URL") + "/labels:search")
                request.Method = "POST"
                request.ContentType = "application/json"
                request.Headers.Add("Authorization", "Bearer " + session("access_token"))

                Using streamWriter As New StreamWriter(request.GetRequestStream())
                    streamWriter.Write(json)
                    streamWriter.Flush()
                    streamWriter.Close()
                End Using

                Try
                    Dim response As WebResponse = request.GetResponse()
                    Dim contentType As String = response.ContentType

                    If contentType = "application/pdf" Then
                        ' Download the PDF file
                        Using responseStream As Stream = response.GetResponseStream()
                            Using fileStream As New FileStream(strPDFLocation, FileMode.Create)
                                responseStream.CopyTo(fileStream)
                            End Using
                        End Using
                    ElseIf contentType = "application/json" Then
                        ' Read and process the JSON response
                        Dim responseObj As Object
                        Using responseStream As Stream = response.GetResponseStream()
                            Using streamReader As New StreamReader(responseStream)
                                Dim jsonResponse As String = streamReader.ReadToEnd()
                                responseObj = JsonConvert.DeserializeObject(jsonResponse)
                                Throw New Exception("Error " & responseObj("code") & vbCrLf & ErrorCodes(responseObj("code")))
                            End Using
                        End Using
                    End If

                    response.Close()
                Catch ex As WebException
                    Dim SR = New StreamReader(ex.Response.GetResponseStream())
                    Dim response = SR.ReadToEnd()
                    Dim jsonResulttodict = JsonConvert.DeserializeObject(response)
                    Throw New Exception("Error " & jsonResulttodict("code") & vbCrLf & ErrorCodes(jsonResulttodict("code")))
                End Try

                If credentials("INSTANTPRINT") Then
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
                If Not findocIn = "" Then
                    Dim updatestr As String = "UPDATE FINDOC " +
                                      "SET CCCD1VOUCHERPRINTED=1" +
                                      "WHERE FINDOC IN (" + findocIn + ")"

                    XSupport.ExecuteSQL(updatestr)
                End If
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try
        End If
        Return 0
    End Function
End Module
