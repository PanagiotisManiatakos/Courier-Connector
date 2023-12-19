Imports System.Drawing.Printing
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Softone

Module acs

    Private Class ACSResponse
        Public ACSExecution_HasError As Boolean
        Public ACSExecutionErrorMessage As String
        Public ACSOutputResponce As ACSOutputResponse
    End Class

    Private Class ACSOutputResponse
        Public ACSValueOutput As List(Of ACSValueOutput)
        Public ACSTableOutput As Object
    End Class

    Private Class ACSValueOutput
        Public Voucher_No As String
        Public Voucher_No_Return As Object
        Public Error_Message As String
        Public ACSObjectOutput As Object
        Public PickupList_No As String
    End Class

    Private Class CreateVoucher
        Public ACSAlias As String = "ACS_Create_Voucher"
        Public ACSInputParameters As New CreateVoucherParameters
    End Class

    Private Class CreateMultipleVouchers
        Public ACSAlias As String = "ACS_Get_Multipart_Vouchers"
        Public ACSInputParameters As New CreateMultipleVoucherParameters

    End Class

    Private Class CreateVoucherParameters
        <JsonProperty(NullValueHandling:=NullValueHandling.Include)>
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Pickup_Date As String
        Public Sender As String
        Public Recipient_Name As String
        Public Recipient_Address As String
        Public Recipient_Address_Number As String
        Public Recipient_Zipcode As Object = Nothing
        Public Recipient_Region As String
        Public Recipient_Phone As String
        Public Recipient_Cell_Phone As String
        Public Recipient_Floor As String
        Public Recipient_Company_Name As Object = Nothing
        Public Recipient_Country As String
        Public Acs_Station_Destination As String
        Public Acs_Station_Branch_Destination As String
        Public Billing_Code As String
        Public Charge_Type As Integer
        Public Cost_Center_Code As String
        Public Item_Quantity As Integer = 1
        Public Weight As Double = 0.5
        Public Dimension_X_In_Cm As String
        Public Dimension_Y_in_Cm As String
        Public Dimension_Z_in_Cm As String
        Public Cod_Ammount As Object
        Public Cod_Payment_Way As Object
        Public Acs_Delivery_Products As Object
        Public Insurance_Ammount As Object = Nothing
        Public Delivery_Notes As String
        Public Appointment_Until_Time As String
        Public Recipient_Email As String
        Public Reference_Key1 As String
        Public Reference_Key2 As String
        Public With_Return_Voucher As String
        Public Content_Type_ID As String
        Public Language As String
    End Class

    Private Class CreateMultipleVoucherParameters
        <JsonProperty(NullValueHandling:=NullValueHandling.Include)>
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Language As String
        Public Main_Voucher_No As String
    End Class

    Private Class PrintVoucher
        Public ACSAlias As String = "ACS_Print_Voucher"
        Public ACSInputParameters As New PrintVoucherParameters
    End Class

    Private Class PrintVoucherParameters
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Voucher_No As String
        Public Print_Type As Integer
        Public Start_Position As Integer
    End Class

    Private Class DeleteVoucher
        Public ACSAlias As String = "ACS_Delete_Voucher"
        Public ACSInputParameters As New DeleteVoucherParameters
    End Class

    Private Class DeleteVoucherParameters
        <JsonProperty(NullValueHandling:=NullValueHandling.Include)>
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Voucher_No As String
        Public Language As String = Nothing
    End Class

    Private Class PickupList
        Public ACSAlias As String = "ACS_Issue_Pickup_List"
        Public ACSInputParameters As New PickupLisParameterst

    End Class

    Private Class PickupLisParameterst
        <JsonProperty(NullValueHandling:=NullValueHandling.Include)>
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Language As String = "GR"
        Public Pickup_Date As String
        Public MyData = Nothing
    End Class

    Private Class PrintPickUpList
        Public ACSAlias As String = "ACS_Print_Pickup_List"
        Public ACSInputParameters As New PrintPickUpListParameters
    End Class

    Private Class PrintPickUpListParameters
        <JsonProperty(NullValueHandling:=NullValueHandling.Include)>
        Public Company_ID As String
        Public Company_Password As String
        Public User_ID As String
        Public User_Password As String
        Public Language As String = "GR"
        Public Pickup_Date As String
        Public Mass_Number As String
    End Class

    Private Class TrackVoucher
        Public ACSAlias As String = "ACS_TrackingDetails"
        Public ACSInputParameters As New DeleteVoucherParameters
    End Class

    Private Function SendJsonPost(json As String, url As String, apiKey As String)
        Dim request As WebRequest = WebRequest.Create(url)
        request.Method = "POST"
        request.ContentType = "application/json"
        request.Headers.Add("AcsApiKey", apiKey)

        Dim bytes As Byte() = Encoding.UTF8.GetBytes(json)

        Using stream As Stream = request.GetRequestStream()
            stream.Write(bytes, 0, bytes.Length)
        End Using

        Dim response As WebResponse = request.GetResponse()
        Dim responseStream As Stream = response.GetResponseStream()
        Dim responseObj As ACSResponse
        Using reader As New StreamReader(responseStream)
            Dim responseJson As String = reader.ReadToEnd()
            responseObj = JsonConvert.DeserializeObject(Of ACSResponse)(responseJson)
        End Using
        Return responseObj
    End Function

    Public Function GetVoucher11700001(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(salData("PAYMENT").ToString) And Not IsDBNull(salData("PAYMENT"))
        Dim Cod_Ammount
        Dim Cod_Payment_Way As Integer?
        Dim Acs_Delivery_Products
        Dim Charge_Type
        If vItsCod Then
            salData("CCCD1VOUCHERVALUE") = If(salData("CCCD1VOUCHERVALUE") > 0.0, salData("CCCD1VOUCHERVALUE"), salData("SUMAMNT"))
            Cod_Ammount = salData("CCCD1VOUCHERVALUE")
            Cod_Payment_Way = If(IsDBNull(salData("CCCD1ACSPAYMENT")), 0, Cod_Payment_Way)
            Acs_Delivery_Products = salData("CCCD1ACSPRODUCTS")
            Acs_Delivery_Products = If(IsDBNull(Acs_Delivery_Products), "COD", If(Acs_Delivery_Products.ToString.Split(",").Contains("COD"), Acs_Delivery_Products, Acs_Delivery_Products + ",COD"))

        Else
            Cod_Ammount = Nothing
            Cod_Payment_Way = Nothing
            Acs_Delivery_Products = Nothing
        End If

        Dim InsAmmount = If(IsDBNull(salData("CCCD1ACSINSAMMOUNT")), Nothing, salData("CCCD1ACSINSAMMOUNT"))
        If Not Acs_Delivery_Products = Nothing Then
            If Acs_Delivery_Products.ToString.Split(",").Contains("INS") Then
                If InsAmmount > 0.0 Then
                    InsAmmount = If(InsAmmount > 3000.0, 3000.0, InsAmmount)
                Else
                    XSupport.Exception("Πρέπει να εισάγετε ποσό ασφάλισης μεγαλύτερο απο 0")
                End If
            Else
                InsAmmount = Nothing
            End If
        End If

        Charge_Type = If(IsDBNull(salData("CCCD1ACSCHARGE")), "2", salData("CCCD1ACSCHARGE"))

        Dim trdrname = salData("CCCD1SHIPNAME")
        Dim trdrphone01 = salData("CCCD1SHIPCELLPHONE")
        Dim Recipient_Address = salData("CCCD1SHIPADDRESS")
        Dim Recipient_Zipcode = salData("CCCD1SHIPZIP")
        Dim Recipient_Region = salData("CCCD1SHIPCITY")
        Dim Recipient_Country = If(IsDBNull(salData("CCCD1ACSDELIVCOUNTRY")), "GR", salData("CCCD1ACSDELIVCOUNTRY"))
        Dim Delivery_Notes = If(IsDBNull(salData("CCCD1VOUCHERCOMMENTS")), "", salData("CCCD1VOUCHERCOMMENTS"))
        Dim sendername As String = credentials("ACSSENDERNAME")
        Dim pickupdate = If(IsDBNull(salData("CCCD1VOUCHERDATE")), Date.Today, salData("CCCD1VOUCHERDATE"))

        Dim Item_Quantity = If(salData("CCCD1VOUCHERQUANTITY").ToString = "" Or salData("CCCD1VOUCHERQUANTITY") < 1, 1, salData("CCCD1VOUCHERQUANTITY"))
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

        Dim createVoucher As New CreateVoucher With {
            .ACSInputParameters = New CreateVoucherParameters With {
                .Company_ID = credentials("COMPANYID").ToString,
                .Company_Password = credentials("COMPANYPASS").ToString,
                .User_ID = credentials("USERNAME").ToString,
                .User_Password = credentials("PASSWORD").ToString,
                .Billing_Code = credentials("BILLINGCODE").ToString,
                .Pickup_Date = pickupdate.ToString,
                .Sender = sendername.ToString,
                .Recipient_Name = trdrname.ToString,
                .Recipient_Address = Recipient_Address.ToString,
                .Recipient_Zipcode = Recipient_Zipcode.ToString,
                .Recipient_Region = Recipient_Region.ToString,
                .Recipient_Phone = trdrphone01.ToString,
                .Recipient_Country = Recipient_Country.ToString,
                .Charge_Type = Charge_Type,
                .Item_Quantity = Item_Quantity,
                .Weight = Weight,
                .Cod_Ammount = Cod_Ammount,
                .Cod_Payment_Way = Cod_Payment_Way,
                .Acs_Delivery_Products = Acs_Delivery_Products,
                .Insurance_Ammount = InsAmmount,
                .Delivery_Notes = Delivery_Notes.ToString
            }
        }

        Dim json As String = JsonConvert.SerializeObject(createVoucher)
        Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))


        If Not responseObj.ACSExecution_HasError Then
            If responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = "" Or responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = Nothing Then
                Dim voucherno = responseObj.ACSOutputResponce.ACSValueOutput.First().Voucher_No
                salData("CCCD1VOUCHERNO") = voucherno.ToString

                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then 'Αν έχει ήδη άλλα Subvouchers
                    While SubvTable.Count > 0
                        SubvTable.Current.Delete() 'Τα διαγραφω γραμμη γραμμη
                    End While

                End If

                If (Item_Quantity > 1) Then ' Αν τα τεμαχια ειναι περισοτερα απο 1 τοτε κανω κλήση παλι για να γεμισω το Subvoucher
                    Dim createMultipleVouchers As New CreateMultipleVouchers With {
                            .ACSInputParameters = New CreateMultipleVoucherParameters With {
                                .Company_ID = credentials("COMPANYID").ToString,
                                .Company_Password = credentials("COMPANYPASS").ToString,
                                .User_ID = credentials("USERNAME").ToString,
                                .User_Password = credentials("PASSWORD").ToString,
                                .Main_Voucher_No = voucherno
                            }
                        }
                    json = JsonConvert.SerializeObject(createMultipleVouchers)
                    Dim MultipleResponseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

                    If Not MultipleResponseObj.ACSExecution_HasError Then
                        Dim SubRow As XRow = SubvTable.Current
                        For Each item As JObject In MultipleResponseObj.ACSOutputResponce.ACSTableOutput("Table_Data")
                            SubRow.Append()
                            SubRow("VOUCHER") = item("MultiPart_Voucher_No").ToString
                            SubRow.Post()
                        Next
                    Else
                        XSupport.Exception(MultipleResponseObj.ACSExecutionErrorMessage)
                    End If

                End If

                salData("CCCD1ACSDELIVCOUNTRY") = Recipient_Country.ToString
                salData("CCCD1ACSPRODUCTS") = If(Acs_Delivery_Products = Nothing, "", Acs_Delivery_Products)
                salData("CCCD1VOUCHERDATE") = pickupdate
                salData("CCCD1VOUVHERWEIGHT") = Weight
                salData("CCCD1VOUCHERVALUE") = If(Cod_Ammount = Nothing, 0.0, Cod_Ammount)
                salData("CCCD1ACSINSAMMOUNT") = If(InsAmmount = Nothing, 0.0, InsAmmount)
                salData("CCCD1ACSCHARGE") = If(Charge_Type = "1", 1, 2)
                salData("CCCD1VOUCHEREXECUTION") = 1
                salData("CCCD1VOUCHERDELETED") = 0
                salData("CCCD1VOUCHERPRINTED") = 0
                salData("CCCD1VOUCHERQUANTITY") = Item_Quantity
                salData("CCCD1ACSPAYMENT") = Cod_Payment_Way
            Else
                XSupport.Exception(responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message)
            End If
        Else
            XSupport.Exception(responseObj.ACSExecutionErrorMessage)
        End If
        Return 0
    End Function

    Public Function PrintVoucher11700002(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim folderpath = credentials("FOLDERPATH")
        Dim ClosingDate As Date = salData("CCCD1VOUCHERDATE")

        Dim printVoucher As New PrintVoucher With {
            .ACSInputParameters = New PrintVoucherParameters With {
                .Company_ID = credentials("COMPANYID"),
                .Company_Password = credentials("COMPANYPASS"),
                .User_ID = credentials("USERNAME"),
                .User_Password = credentials("PASSWORD"),
                .Voucher_No = salData("CCCD1VOUCHERNO"),
                .Print_Type = credentials("VOUCHERPRINTTYPE"),
                .Start_Position = If(IsDBNull(credentials("ACSPRINTSTARTPOSITION")), 1, credentials("ACSPRINTSTARTPOSITION"))
            }
        }

        Dim json As String = JsonConvert.SerializeObject(printVoucher)
        Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

        If Not responseObj.ACSExecution_HasError Then
            Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
            Dim strPDFLocation = strFileLocation + "\" + salData("CCCD1VOUCHERNO").ToString + ".pdf"
            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
            If Not folderexists Then
                Directory.CreateDirectory(strFileLocation)
            End If
            If Not pdfgexists Then
                Dim pdfBytes As Byte() = Convert.FromBase64String(responseObj.ACSOutputResponce.ACSValueOutput.First().ACSObjectOutput)
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
            salData("CCCD1VOUCHERPRINTED") = 1
            XSupport.Warning("Ολοκλήρωση εκτύπωσης")

        Else
            XSupport.Exception(responseObj.ACSExecutionErrorMessage)
        End If
        Return 0
    End Function

    Public Function DeleteVoucher11700003(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim deleteVoucher As New DeleteVoucher With {
            .ACSInputParameters = New DeleteVoucherParameters With {
                .Company_ID = credentials("COMPANYID"),
                .Company_Password = credentials("COMPANYPASS"),
                .User_ID = credentials("USERNAME"),
                .User_Password = credentials("PASSWORD"),
                .Voucher_No = salData("CCCD1VOUCHERNO")
            }
        }

        Dim json As String = JsonConvert.SerializeObject(deleteVoucher)
        Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

        If Not responseObj.ACSExecution_HasError Then
            If responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = "" Or responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = Nothing Then
                salData("CCCD1VOUCHERDELETED") = 1
                salData("CCCD1VOUCHEREXECUTION") = 0
                salData("CCCD1VOUCHERPRINTED") = 0
                salData("CCCD1VOUCHERNO") = ""
                salData("CCCD1VOUCHERVALUE") = 0.0
                Dim SubvTable = XModule.GetTable("CCCD1SUBVOUCHERS")
                If SubvTable.Count > 0 Then
                    While SubvTable.Count > 0
                        SubvTable.Current.Delete()
                    End While
                End If
                XSupport.Warning("Ολοκλήρωση ακύρωσης")
            Else
                XSupport.Exception(responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message)
            End If
        Else
            XSupport.Exception(responseObj.ACSExecutionErrorMessage)
        End If
        Return 0
    End Function

    Public Function TrackVoucher11700004(credentials As XRow, salData As XRow, XModule As XModule, XSupport As XSupport)
        Dim trackVoucher As New TrackVoucher With {
           .ACSInputParameters = New DeleteVoucherParameters With {
               .Company_ID = credentials("COMPANYID"),
               .Company_Password = credentials("COMPANYPASS"),
               .User_ID = credentials("USERNAME"),
               .User_Password = credentials("PASSWORD"),
               .Voucher_No = salData("CCCD1VOUCHERNO")
           }
       }

        Dim json As String = JsonConvert.SerializeObject(trackVoucher)
        Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))
        If Not responseObj.ACSExecution_HasError Then
            If responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = "" Or responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = Nothing Then
                Dim Trform = New TrackingForm()
                For Each item As JObject In responseObj.ACSOutputResponce.ACSTableOutput("Table_Data")
                    Dim x As String() = {item.Item("checkpoint_date_time"), item.Item("checkpoint_action"), item.Item("checkpoint_location"), item.Item("checkpoint_notes")}
                    Trform.DataGridView1.Rows.Add(x)
                Next
                Trform.Show()
            Else
                XSupport.Exception(responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message)
            End If
        Else
            XSupport.Exception(responseObj.ACSExecutionErrorMessage)
        End If
        Return 0
    End Function

    Public Function GetMassVoucher11700011(ds As XTable, i As Integer, Success_list As List(Of String), Error_list_Messages As List(Of String), Error_list_Fincode As List(Of String), Error_list_Findoc As List(Of String), XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=1 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Error_list_Messages.Add("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
        Else
            Dim credentials As XRow = credentialsTable.Current

            Dim VCompanyID = credentials("COMPANYID")
            Dim vCompanyPass = credentials("COMPANYPASS")
            Dim vUsername = credentials("USERNAME")
            Dim vPassword = credentials("PASSWORD")
            Dim vUrl = New Uri(credentials("URL"))
            Dim vApikey = credentials("APIKEY")
            Dim vBillingCode = credentials("BILLINGCODE")
            Dim vItsIn As Boolean = credentials("SERIES").ToString.Split(",").Contains(ds.Item(i, "SERIES").ToString)
            Dim Cod_Ammount
            Dim Cod_Payment_Way As Integer?
            Dim Acs_Delivery_Products
            Dim Charge_Type

            Dim vItsCod As Boolean = credentials("PAYMENT").ToString.Split(",").Contains(ds.Item(i, "PAYMENT").ToString)
            If vItsCod Then
                Cod_Ammount = If(ds.Item(i, "CCCD1VOUCHERVALUE") > 0.0, ds.Item(i, "CCCD1VOUCHERVALUE"), ds.Item(i, "SUMAMNT"))
                Cod_Payment_Way = ds.Item(i, "CCCD1ACSPAYMENT")
                Acs_Delivery_Products = ds.Item(i, "CCCD1ACSPRODUCTS")
                Acs_Delivery_Products = If(IsDBNull(Acs_Delivery_Products), "COD", If(Acs_Delivery_Products.ToString.Split(",").Contains("COD"), Acs_Delivery_Products, Acs_Delivery_Products + ",COD"))
                Cod_Payment_Way = If(IsDBNull(Cod_Payment_Way), 0, Cod_Payment_Way)

            Else
                Cod_Ammount = Nothing
                Cod_Payment_Way = Nothing
                Acs_Delivery_Products = Nothing
            End If

            Dim InsAmmount = If(IsDBNull(ds(i, "CCCD1ACSINSAMMOUNT")), Nothing, ds(i, "CCCD1ACSINSAMMOUNT"))
            If Not Acs_Delivery_Products = Nothing Then

                If Acs_Delivery_Products.ToString.Split(",").Contains("INS") Then
                    If InsAmmount > 0.0 Then
                        InsAmmount = If(InsAmmount > 3000.0, 3000.0, InsAmmount)
                    Else
                        Error_list_Messages.Add("Πρέπει να εισάγετε ποσό ασφάλισης μεγαλύτερο απο 0")
                        Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                        Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                        Return 0
                    End If
                Else
                    InsAmmount = Nothing
                End If
            End If

            Charge_Type = If(IsDBNull(ds.Item(i, "CCCD1ACSCHARGE")), 2, ds.Item(i, "CCCD1ACSCHARGE"))

            Dim trdrname = ds(i, "CCCD1SHIPNAME")
            Dim trdrphone01 = ds(i, "CCCD1SHIPCELLPHONE")
            Dim Recipient_Address = ds(i, "CCCD1SHIPADDRESS")
            Dim Recipient_Zipcode = ds(i, "CCCD1SHIPZIP")
            Dim Recipient_Region = ds(i, "CCCD1SHIPCITY")
            Dim Recipient_Country = If(IsDBNull(ds.Item(i, "CCCD1ACSDELIVCOUNTRY")), "GR", ds.Item(i, "CCCD1ACSDELIVCOUNTRY"))
            Dim Delivery_Notes = If(IsDBNull(ds.Item(i, "CCCD1VOUCHERCOMMENTS")), "", ds.Item(i, "CCCD1VOUCHERCOMMENTS"))
            Dim pickupdate = If(IsDBNull(ds.Item(i, "CCCD1VOUCHERDATE")), Date.Today, ds.Item(i, "CCCD1VOUCHERDATE"))

            Dim Item_Quantity = If(ds(i, "CCCD1VOUCHERQUANTITY").ToString = "" Or ds(i, "CCCD1VOUCHERQUANTITY") < 1, 1, ds(i, "CCCD1VOUCHERQUANTITY"))
            Dim WeightType = credentials("WEIGHTTYPE")
            Dim Weight = 0.0
            If WeightType = 1 Then 'YPOLOGISMOS BAROUS APO ITELINES
                Dim IteTable As XTable = XSupport.GetSQLDataSet("Select * FROM MTRLINES WHERE SODTYPE=51 And FINDOC=" + ds(i, "FINDOC").ToString)
                If IteTable.Count > 0 Then
                    For y As Integer = 0 To IteTable.Count - 1
                        Dim IteWeight = If(IteTable(y, "WEIGHT").ToString = "", 0.0, IteTable(y, "WEIGHT"))
                        Weight = +IteWeight
                    Next

                    Weight = If(Weight < 0.5, 0.5, Weight)

                Else
                    Weight = 0.5
                End If
            Else
                Weight = If(ds(i, "CCCD1VOUVHERWEIGHT").ToString = "" Or ds(i, "CCCD1VOUVHERWEIGHT") < 0.5, 0.5, ds(i, "CCCD1VOUVHERWEIGHT"))
            End If

            Dim createVoucher As New CreateVoucher With {
                .ACSInputParameters = New CreateVoucherParameters With {
                    .Company_ID = credentials("COMPANYID").ToString,
                    .Company_Password = credentials("COMPANYPASS").ToString,
                    .User_ID = credentials("USERNAME").ToString,
                    .User_Password = credentials("PASSWORD").ToString,
                    .Billing_Code = credentials("BILLINGCODE").ToString,
                    .Pickup_Date = pickupdate.ToString,
                    .Sender = credentials("ACSSENDERNAME").ToString,
                    .Recipient_Name = trdrname.ToString,
                    .Recipient_Address = Recipient_Address.ToString,
                    .Recipient_Zipcode = Recipient_Zipcode.ToString,
                    .Recipient_Region = Recipient_Region.ToString,
                    .Recipient_Phone = trdrphone01.ToString,
                    .Recipient_Country = Recipient_Country.ToString,
                    .Charge_Type = Charge_Type,
                    .Item_Quantity = Item_Quantity,
                    .Weight = Weight,
                    .Cod_Ammount = Cod_Ammount,
                    .Cod_Payment_Way = Cod_Payment_Way,
                    .Acs_Delivery_Products = Acs_Delivery_Products,
                    .Insurance_Ammount = InsAmmount,
                    .Delivery_Notes = Delivery_Notes.ToString
                }
            }

            Dim json As String = JsonConvert.SerializeObject(createVoucher)
            Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))


            If Not responseObj.ACSExecution_HasError Then
                If responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = "" Or responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = Nothing Then
                    Dim voucherno = responseObj.ACSOutputResponce.ACSValueOutput.First().Voucher_No

                    Dim DelSubs As String = ("DELETE FROM CCCD1SUBVOUCHERS WHERE FINDOC=" + ds(i, "FINDOC").ToString)
                    XSupport.ExecuteSQL(DelSubs)

                    If (Item_Quantity > 1) Then ' Αν τα τεμαχια ειναι περισοτερα απο 1 τοτε κανω κλήση παλι για να γεμισω το Subvoucher
                        Dim createMultipleVouchers As New CreateMultipleVouchers With {
                            .ACSInputParameters = New CreateMultipleVoucherParameters With {
                                .Company_ID = credentials("COMPANYID").ToString,
                                .Company_Password = credentials("COMPANYPASS").ToString,
                                .User_ID = credentials("USERNAME").ToString,
                                .User_Password = credentials("PASSWORD").ToString,
                                .Main_Voucher_No = voucherno
                            }
                        }
                        json = JsonConvert.SerializeObject(createMultipleVouchers)
                        Dim MultipleResponseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

                        If Not MultipleResponseObj.ACSExecution_HasError Then
                            For Each item As JObject In MultipleResponseObj.ACSOutputResponce.ACSTableOutput("Table_Data")
                                Dim vInsSub As String = "INSERT INTO CCCD1SUBVOUCHERS (VOUCHER, FINDOC) " +
                                                        "VALUES ('" + item("MultiPart_Voucher_No").ToString + "'," + ds(i, "FINDOC").ToString + ")"
                                XSupport.ExecuteSQL(vInsSub)
                            Next
                        Else
                            Error_list_Messages.Add(MultipleResponseObj.ACSExecutionErrorMessage)
                            Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                            Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                        End If

                    End If

                    Dim updatestr As String = "UPDATE FINDOC " +
                                               "SET CCCD1VOUCHERNO='" + voucherno.ToString + "', " +
                                                "CCCD1ACSDELIVCOUNTRY='" + Recipient_Country.ToString + "', " +
                                                "CCCD1ACSPAYMENT=" + If(Cod_Payment_Way Is Nothing, 0.ToString, Cod_Payment_Way.ToString) + ", " +
                                                "CCCD1ACSPRODUCTS='" + If(Acs_Delivery_Products = Nothing, "", Acs_Delivery_Products.ToString) + "', " +
                                                "CCCD1ACSCHARGE=" + Charge_Type.ToString + ", " +
                                                "CCCD1VOUCHERDATE='" + String.Format("{0:yyyMMdd}", pickupdate) + "', " +
                                                "CCCD1VOUCHERQUANTITY=" + Item_Quantity.ToString.Replace(",", ".") + ", " +
                                                "CCCD1VOUVHERWEIGHT=" + Weight.ToString.Replace(",", ".") + ", " +
                                                "CCCD1VOUCHERVALUE=" + If(Cod_Ammount = Nothing, 0.0.ToString, Cod_Ammount.ToString.Replace(",", ".")) + ", " +
                                                "CCCD1ACSINSAMMOUNT=" + If(InsAmmount = Nothing, 0.0.ToString, InsAmmount.ToString.Replace(",", ".")) + ", " +
                                                "CCCD1VOUCHEREXECUTION= 1, " +
                                                "CCCD1VOUCHERDELETED= 0, " +
                                                "CCCD1VOUCHERPRINTED= 0 " +
                                                "WHERE FINDOC=" + ds.Item(i, "FINDOC").ToString

                    XSupport.ExecuteSQL(updatestr)
                    Success_list.Add(ds.Item(i, "FINCODE").ToString + " : " + voucherno.ToString)
                Else
                    Error_list_Messages.Add(responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message)
                    Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                    Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
                End If


            Else
                Error_list_Messages.Add(responseObj.ACSExecutionErrorMessage)
                Error_list_Fincode.Add(ds.Item(i, "FINCODE").ToString)
                Error_list_Findoc.Add(ds.Item(i, "FINDOC").ToString)
            End If


        End If

        Return 0
    End Function

    Public Function PrintMassVoucher11700012(ds As XTable, i As Integer, XModule As XModule, XSupport As XSupport)
        Dim queryStr = String.Format("SELECT TOP 1 * FROM CCCD1COURIERCONFIG WHERE COMPANY={0} AND COURIERCOMPANY=1 AND SERIES LIKE '%{1}%' AND ISACTIVE=1", ds(i, "COMPANY"), ds(i, "SERIES"))
        Dim credentialsTable = XSupport.GetSQLDataSet(queryStr)
        If credentialsTable.Count = 0 Then
            Throw New Exception("Δεν βρέθηκε παραμετροποίηση για την συγκεκριμένη σειρά " + ds(i, "SERIES"))
        Else
            Dim credentials As XRow = credentialsTable.Current
            Dim folderpath = credentials("FOLDERPATH")
            Dim ClosingDate As Date = ds(i, "CCCD1VOUCHERDATE")

            Dim printVoucher As New PrintVoucher With {
                .ACSInputParameters = New PrintVoucherParameters With {
                    .Company_ID = credentials("COMPANYID"),
                    .Company_Password = credentials("COMPANYPASS"),
                    .User_ID = credentials("USERNAME"),
                    .User_Password = credentials("PASSWORD"),
                    .Voucher_No = ds(i, "CCCD1VOUCHERNO"),
                    .Print_Type = credentials("VOUCHERPRINTTYPE"),
                    .Start_Position = If(IsDBNull(credentials("ACSPRINTSTARTPOSITION")), 1, credentials("ACSPRINTSTARTPOSITION"))
                }
            }

            Dim json As String = JsonConvert.SerializeObject(printVoucher)
            Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

            If Not responseObj.ACSExecution_HasError Then
                Dim strFileLocation = folderpath + "\" + ClosingDate.ToString("yyyy") + "\" + ClosingDate.ToString("MMMM") + "\" + ClosingDate.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Vouchers"
                Dim strPDFLocation = strFileLocation + "\" + ds(i, "CCCD1VOUCHERNO").ToString + ".pdf"
                Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                If Not folderexists Then
                    Directory.CreateDirectory(strFileLocation)
                End If
                If Not pdfgexists Then
                    Dim pdfBytes As Byte() = Convert.FromBase64String(responseObj.ACSOutputResponce.ACSValueOutput.First().ACSObjectOutput)
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
                XSupport.ExecuteSQL("UPDATE FINDOC SET CCCD1VOUCHERPRINTED=1 WHERE FINDOC=" + ds(i, "FINDOC").ToString)
            Else
                Throw New Exception(responseObj.ACSExecutionErrorMessage)
            End If
        End If

        Return 0

    End Function

    Public Function FinalizeVoucher(credentials As XRow, data As XRow, XSupport As XSupport)
        Dim issuePickUpList As New PickupList With {
            .ACSInputParameters = New PickupLisParameterst With {
                    .Company_ID = credentials("COMPANYID"),
                    .Company_Password = credentials("COMPANYPASS"),
                    .User_ID = credentials("USERNAME"),
                    .User_Password = credentials("PASSWORD"),
                    .Pickup_Date = Date.Now.ToString("yyyy-MM-dd")
            }
        }

        Dim json As String = JsonConvert.SerializeObject(issuePickUpList)
        Dim responseObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))

        If Not responseObj.ACSExecution_HasError Then
            If responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = "" Or responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message = Nothing Then
                Dim pickuNo = responseObj.ACSOutputResponce.ACSValueOutput.First().PickupList_No
                If Not pickuNo = Nothing Then
                    Dim ans = XSupport.AskYesNoCancel("Επιτυχής οριστικοποίησης", "Θέλετε να προχωρήσετε σε δημιουργία αρχείου pdf;")
                    If ans = 6 Then
                        Dim printPickUp As New PrintPickUpList With {
                        .ACSInputParameters = New PrintPickUpListParameters With {
                        .Mass_Number = pickuNo,
                        .Company_ID = credentials("COMPANYID"),
                        .Company_Password = credentials("COMPANYPASS"),
                        .User_ID = credentials("USERNAME"),
                        .User_Password = credentials("PASSWORD"),
                        .Pickup_Date = Date.Now.ToString("yyyy-MM-dd")
                            }
                        }

                        json = JsonConvert.SerializeObject(printPickUp)
                        Dim printObj As ACSResponse = SendJsonPost(json, credentials("URL"), credentials("APIKEY"))
                        If Not printObj.ACSExecution_HasError Then
                            Dim strFileLocation = credentials("FOLDERPATH") + "\" + Date.Now.ToString("yyyy") + "\" + Date.Now.ToString("MMMM") + "\" + Date.Now.ToString("dd-MM dddd", CultureInfo.CreateSpecificCulture("el-GR")) + "\" + "Λίστες"
                            Dim strPDFLocation = strFileLocation + "\" + pickuNo.ToString + ".pdf"
                            Dim folderexists As Boolean = Directory.Exists(strFileLocation)
                            Dim pdfgexists As Boolean = File.Exists(strPDFLocation)
                            If Not folderexists Then
                                Directory.CreateDirectory(strFileLocation)
                            End If
                            If Not pdfgexists Then
                                Dim pdfBytes As Byte() = Convert.FromBase64String(printObj.ACSOutputResponce.ACSValueOutput.First().ACSObjectOutput.item("PDFData"))
                                File.WriteAllBytes(strPDFLocation, pdfBytes)
                            End If

                            'If credentials("INSTANTPRINT") = 1 Then
                            '    Dim settings As New PrinterSettings
                            '    Dim psi As New ProcessStartInfo

                            '    settings.PrinterName = Chr(34) + credentials("PRINTER") + Chr(34)

                            '    psi.UseShellExecute = True
                            '    psi.Verb = "print"
                            '    psi.WindowStyle = ProcessWindowStyle.Hidden
                            '    psi.Arguments = settings.PrinterName.ToString()
                            '    psi.FileName = strPDFLocation

                            '    Process.Start(psi)
                            'End If
                            data("EXECUTIONOK") = 1
                            data("FILELOCATION") = strFileLocation
                            XSupport.Warning("Επιτυχής δημιουργία αρχείου " + pickuNo.ToString)
                        Else
                            Throw New Exception(responseObj.ACSExecutionErrorMessage)
                        End If

                    End If
                Else
                    XSupport.Warning("Επιτυχής οριστικοποίησης ACS")
                End If

            Else
                Dim errorMessage As String = responseObj.ACSOutputResponce.ACSValueOutput.First().Error_Message
                Dim Errorform = New MassVoucherError()
                Dim counter As Integer = 0
                For Each item As JObject In responseObj.ACSOutputResponce.ACSTableOutput("Table_Data")
                    Dim ds = XSupport.GetSQLDataSet("SELECT TOP 1 FINCODE,FINDOC FROM FINDOC WHERE CCCD1VOUCHERNO='" + item("Unprinted_Vouchers").ToString + "'")
                    If ds.Count = 1 Then
                        counter += 1
                        Dim x As String() = {ds.Current("FINCODE"), "Δεν έχει εκτυπωθεί", ds.Current("FINDOC")}
                        Errorform.DataGridView1.Rows.Add(x)
                    End If
                Next
                XSupport.Warning(errorMessage)
                If counter > 0 Then
                    XXX = XSupport
                    Errorform.Show()
                End If
            End If
        Else
            Throw New Exception(responseObj.ACSExecutionErrorMessage)
        End If

        Return 0
    End Function
End Module
