Imports Newtonsoft.Json
Imports Softone
Imports System.IO
Imports System.Net
Imports System.Text

Module functions
    Private Class RenewBody
        Public url As String
        Public data As RenewBodyData
    End Class

    Private Class RenewBodyData
        Public sn As String
        Public instType As Integer
    End Class

    Public Function CheckCanGo(courier As Integer, XSupport As XSupport)
        Dim queryStr As String = String.Format("SELECT TOP 1 BLCDATE,SIGNATURE FROM CCCD1COURIERSMODULE WHERE COURIER={0}", courier)
        Dim dsBlc As XTable = XSupport.GetSQLDataSet(queryStr)
        If dsBlc.Count > 0 Then
            Dim signature = SimpleDecrypt(dsBlc.Current("SIGNATURE"))
            Dim parts As String() = signature.Split("&")
            If Not parts.Length = 2 Then
                Return New With {
                    .success = False,
                    .code = 1
                }

            End If
            Dim signatureCourier As Integer = Integer.Parse(parts(0))
            Dim signatureDate As Date = Date.Parse(Convert.ToDateTime(parts(1).ToString()))
            Dim blcdate As Date = Convert.ToDateTime(dsBlc.Current("BLCDATE").ToString())
            If Not courier = signatureCourier Then
                Return New With {
                    .success = False,
                    .code = 1
                }
            End If
            If Not signatureDate = blcdate Then
                Return New With {
                    .success = False,
                    .code = 1
                }
            End If
            If (blcdate - Date.Now).TotalDays <= -1 Then
                Return New With {
                    .success = False,
                    .code = 0
                }
            End If

            Return New With {
                .success = True
            }
            Return blcdate > Date.Now
        Else
            Return New With {
                .success = False,
                .code = -1
            }
        End If
    End Function

    Public Function ThrowError(courier As Integer, code As Integer)
        If code = 0 Then
            Throw New Exception("Η άδεια του module " + CourierNames(courier) + " Connector έληξε." + vbCrLf + "Προχωρήστε σε ανανέωση ώστε να χρησιμοποιείτε το module")
        ElseIf code = -1 Then
            Throw New Exception("Δεν υπάρχει ενεργή άδεια για το module " + CourierNames(courier) + " Connector." + vbCrLf + "Επικοινωνήστε με την Day Οne για ενεργοποίηση του module")
        ElseIf code = 1 Then
            Throw New Exception(CourierNames(courier) + " Connector" + vbCrLf + "Λανθασμένο Signature ενεργοποίησης")
        End If
        Return 0
    End Function

    Public Function Renew(sn As String, courier As Integer) As Object
        Dim url As String = dayoneApiURL
        Dim data As New RenewBody With {
            .url = dayoneRenewURL,
            .data = New RenewBodyData With {
                .sn = sn,
                .instType = CourierCodesInverted(courier)
            }
        }
        Try
            Dim json = JsonConvert.SerializeObject(data)
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"

            Dim postData As Byte() = Encoding.UTF8.GetBytes(json)
            request.ContentLength = postData.Length

            Dim requestStream As Stream = request.GetRequestStream()
            requestStream.Write(postData, 0, postData.Length)
            requestStream.Close()

            Dim response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
            Dim responseStream As Stream = response.GetResponseStream()
            Dim reader As New StreamReader(responseStream)
            Dim responseData As String = reader.ReadToEnd()

            Dim result As New With {
        .success = False,
        .blcdate = "",
        .error = ""
        }

            Dim responseObj = JsonConvert.DeserializeObject(responseData)
            If responseObj.item("success") Then
                result.success = True
                result.blcdate = responseObj.item("data").first().item("BLCKDATE")
            Else
                result.success = False
                result.error = responseObj.item("error")
            End If
            Return result
        Catch ex As Exception
            Return New With {
                .success = False,
                .error = ex.Message
                }
        End Try
    End Function

    Public Function SimpleEncrypt(data As String) As String
        Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(data)
        Dim keyBytes As Byte() = Encoding.UTF8.GetBytes("Softone1!")
        Dim encryptedBytes(dataBytes.Length - 1) As Byte

        For i As Integer = 0 To dataBytes.Length - 1
            Dim keyIndex As Integer = i Mod keyBytes.Length
            encryptedBytes(i) = dataBytes(i) Xor keyBytes(keyIndex)
        Next

        Return Convert.ToBase64String(encryptedBytes)
    End Function

    Public Function SimpleDecrypt(encryptedData As String) As String
        Dim encryptedBytes As Byte() = Convert.FromBase64String(encryptedData)
        Dim keyBytes As Byte() = Encoding.UTF8.GetBytes("Softone1!")
        Dim decryptedBytes(encryptedBytes.Length - 1) As Byte

        For i As Integer = 0 To encryptedBytes.Length - 1
            Dim keyIndex As Integer = i Mod keyBytes.Length
            decryptedBytes(i) = encryptedBytes(i) Xor keyBytes(keyIndex)
        Next

        Return Encoding.UTF8.GetString(decryptedBytes)
    End Function

End Module
