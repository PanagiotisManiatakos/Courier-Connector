﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
'
Namespace Web.taxydema.printA6
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="TAXYPRINTSIDETAA6", [Namespace]:="/TAXYPRINTSIDETAA6")>  _
    Partial Public Class TAXYPRINTSIDETAA6
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private PRINTOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.D1_CourierConnector.My.MySettings.Default.D1_CourierConnector_Web_taxydema_create_TAXYCREATESIDETA
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event PRINTCompleted As PRINTCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace:="/TAXYPRINTSIDETAA6", ResponseNamespace:="/TAXYPRINTSIDETAA6", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function PRINT(ByVal user_code As String, ByVal user_pass As String, ByVal pel_code As String, ByVal vg_code As String, ByRef st_title As String, ByRef b64_string As String) As <System.Xml.Serialization.XmlElementAttribute("st_flag", DataType:="integer")> String
            Dim results() As Object = Me.Invoke("PRINT", New Object() {user_code, user_pass, pel_code, vg_code})
            st_title = CType(results(1),String)
            b64_string = CType(results(2),String)
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub PRINTAsync(ByVal user_code As String, ByVal user_pass As String, ByVal pel_code As String, ByVal vg_code As String)
            Me.PRINTAsync(user_code, user_pass, pel_code, vg_code, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub PRINTAsync(ByVal user_code As String, ByVal user_pass As String, ByVal pel_code As String, ByVal vg_code As String, ByVal userState As Object)
            If (Me.PRINTOperationCompleted Is Nothing) Then
                Me.PRINTOperationCompleted = AddressOf Me.OnPRINTOperationCompleted
            End If
            Me.InvokeAsync("PRINT", New Object() {user_code, user_pass, pel_code, vg_code}, Me.PRINTOperationCompleted, userState)
        End Sub
        
        Private Sub OnPRINTOperationCompleted(ByVal arg As Object)
            If (Not (Me.PRINTCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent PRINTCompleted(Me, New PRINTCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")>  _
    Public Delegate Sub PRINTCompletedEventHandler(ByVal sender As Object, ByVal e As PRINTCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class PRINTCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property st_title() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(1),String)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property b64_string() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(2),String)
            End Get
        End Property
    End Class
End Namespace
