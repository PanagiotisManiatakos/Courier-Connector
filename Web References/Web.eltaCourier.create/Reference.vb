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
Namespace Web.eltaCourier.create
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="CREATEAWB", [Namespace]:="/CREATEAWB")>  _
    Partial Public Class CREATEAWB
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private READOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.D1_CourierConnector.My.MySettings.Default.D1_CourierConnector_Web_eltaCourier_create_CREATEAWB
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
        Public Event READCompleted As READCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace:="/CREATEAWB", ResponseNamespace:="/CREATEAWB", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function READ( _
                    ByVal pel_user_code As String,  _
                    ByVal pel_user_pass As String,  _
                    ByVal pel_apost_code As String,  _
                    ByVal pel_apost_sub_code As String,  _
                    ByVal pel_user_lang As String,  _
                    ByVal pel_paral_name As String,  _
                    ByVal pel_paral_address As String,  _
                    ByVal pel_paral_area As String,  _
                    ByVal pel_paral_tk As String,  _
                    ByVal pel_paral_thl_1 As String,  _
                    ByVal pel_paral_thl_2 As String,  _
                    ByVal pel_service As String,  _
                    ByVal pel_baros As String,  _
                    ByVal pel_temaxia As String,  _
                    ByVal pel_paral_sxolia As String,  _
                    ByVal pel_sur_1 As String,  _
                    ByVal pel_sur_2 As String,  _
                    ByVal pel_sur_3 As String,  _
                    ByVal pel_ant_poso As String,  _
                    ByVal pel_ant_poso1 As String,  _
                    ByVal pel_ant_poso2 As String,  _
                    ByVal pel_ant_poso3 As String,  _
                    ByVal pel_ant_poso4 As String,  _
                    ByVal pel_ant_date1 As String,  _
                    ByVal pel_ant_date2 As String,  _
                    ByVal pel_ant_date3 As String,  _
                    ByVal pel_ant_date4 As String,  _
                    ByVal pel_asf_poso As String,  _
                    ByVal pel_ref_no As String,  _
                    ByRef st_title As String,  _
                    ByRef vg_code As String,  _
                    ByRef return_vg As String,  _
                    ByRef epitagh_vg As String,  _
                    <System.Xml.Serialization.XmlElementAttribute("vg_child")> ByRef vg_child() As String) As <System.Xml.Serialization.XmlElementAttribute("st_flag", DataType:="integer")> String
            Dim results() As Object = Me.Invoke("READ", New Object() {pel_user_code, pel_user_pass, pel_apost_code, pel_apost_sub_code, pel_user_lang, pel_paral_name, pel_paral_address, pel_paral_area, pel_paral_tk, pel_paral_thl_1, pel_paral_thl_2, pel_service, pel_baros, pel_temaxia, pel_paral_sxolia, pel_sur_1, pel_sur_2, pel_sur_3, pel_ant_poso, pel_ant_poso1, pel_ant_poso2, pel_ant_poso3, pel_ant_poso4, pel_ant_date1, pel_ant_date2, pel_ant_date3, pel_ant_date4, pel_asf_poso, pel_ref_no})
            st_title = CType(results(1),String)
            vg_code = CType(results(2),String)
            return_vg = CType(results(3),String)
            epitagh_vg = CType(results(4),String)
            vg_child = CType(results(5),String())
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub READAsync( _
                    ByVal pel_user_code As String,  _
                    ByVal pel_user_pass As String,  _
                    ByVal pel_apost_code As String,  _
                    ByVal pel_apost_sub_code As String,  _
                    ByVal pel_user_lang As String,  _
                    ByVal pel_paral_name As String,  _
                    ByVal pel_paral_address As String,  _
                    ByVal pel_paral_area As String,  _
                    ByVal pel_paral_tk As String,  _
                    ByVal pel_paral_thl_1 As String,  _
                    ByVal pel_paral_thl_2 As String,  _
                    ByVal pel_service As String,  _
                    ByVal pel_baros As String,  _
                    ByVal pel_temaxia As String,  _
                    ByVal pel_paral_sxolia As String,  _
                    ByVal pel_sur_1 As String,  _
                    ByVal pel_sur_2 As String,  _
                    ByVal pel_sur_3 As String,  _
                    ByVal pel_ant_poso As String,  _
                    ByVal pel_ant_poso1 As String,  _
                    ByVal pel_ant_poso2 As String,  _
                    ByVal pel_ant_poso3 As String,  _
                    ByVal pel_ant_poso4 As String,  _
                    ByVal pel_ant_date1 As String,  _
                    ByVal pel_ant_date2 As String,  _
                    ByVal pel_ant_date3 As String,  _
                    ByVal pel_ant_date4 As String,  _
                    ByVal pel_asf_poso As String,  _
                    ByVal pel_ref_no As String)
            Me.READAsync(pel_user_code, pel_user_pass, pel_apost_code, pel_apost_sub_code, pel_user_lang, pel_paral_name, pel_paral_address, pel_paral_area, pel_paral_tk, pel_paral_thl_1, pel_paral_thl_2, pel_service, pel_baros, pel_temaxia, pel_paral_sxolia, pel_sur_1, pel_sur_2, pel_sur_3, pel_ant_poso, pel_ant_poso1, pel_ant_poso2, pel_ant_poso3, pel_ant_poso4, pel_ant_date1, pel_ant_date2, pel_ant_date3, pel_ant_date4, pel_asf_poso, pel_ref_no, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub READAsync( _
                    ByVal pel_user_code As String,  _
                    ByVal pel_user_pass As String,  _
                    ByVal pel_apost_code As String,  _
                    ByVal pel_apost_sub_code As String,  _
                    ByVal pel_user_lang As String,  _
                    ByVal pel_paral_name As String,  _
                    ByVal pel_paral_address As String,  _
                    ByVal pel_paral_area As String,  _
                    ByVal pel_paral_tk As String,  _
                    ByVal pel_paral_thl_1 As String,  _
                    ByVal pel_paral_thl_2 As String,  _
                    ByVal pel_service As String,  _
                    ByVal pel_baros As String,  _
                    ByVal pel_temaxia As String,  _
                    ByVal pel_paral_sxolia As String,  _
                    ByVal pel_sur_1 As String,  _
                    ByVal pel_sur_2 As String,  _
                    ByVal pel_sur_3 As String,  _
                    ByVal pel_ant_poso As String,  _
                    ByVal pel_ant_poso1 As String,  _
                    ByVal pel_ant_poso2 As String,  _
                    ByVal pel_ant_poso3 As String,  _
                    ByVal pel_ant_poso4 As String,  _
                    ByVal pel_ant_date1 As String,  _
                    ByVal pel_ant_date2 As String,  _
                    ByVal pel_ant_date3 As String,  _
                    ByVal pel_ant_date4 As String,  _
                    ByVal pel_asf_poso As String,  _
                    ByVal pel_ref_no As String,  _
                    ByVal userState As Object)
            If (Me.READOperationCompleted Is Nothing) Then
                Me.READOperationCompleted = AddressOf Me.OnREADOperationCompleted
            End If
            Me.InvokeAsync("READ", New Object() {pel_user_code, pel_user_pass, pel_apost_code, pel_apost_sub_code, pel_user_lang, pel_paral_name, pel_paral_address, pel_paral_area, pel_paral_tk, pel_paral_thl_1, pel_paral_thl_2, pel_service, pel_baros, pel_temaxia, pel_paral_sxolia, pel_sur_1, pel_sur_2, pel_sur_3, pel_ant_poso, pel_ant_poso1, pel_ant_poso2, pel_ant_poso3, pel_ant_poso4, pel_ant_date1, pel_ant_date2, pel_ant_date3, pel_ant_date4, pel_asf_poso, pel_ref_no}, Me.READOperationCompleted, userState)
        End Sub
        
        Private Sub OnREADOperationCompleted(ByVal arg As Object)
            If (Not (Me.READCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent READCompleted(Me, New READCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
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
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0")>  _
    Public Delegate Sub READCompletedEventHandler(ByVal sender As Object, ByVal e As READCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9037.0"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class READCompletedEventArgs
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
        Public ReadOnly Property vg_code() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(2),String)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property return_vg() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(3),String)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property epitagh_vg() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(4),String)
            End Get
        End Property
        
        '''<remarks/>
        Public ReadOnly Property vg_child() As String()
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(5),String())
            End Get
        End Property
    End Class
End Namespace