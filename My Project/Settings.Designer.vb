﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.8.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://testvoucher.taxydromiki.gr/JobServicesV2.asmx")>  _
        Public ReadOnly Property D1_CourierConnector_service_taxydromiki_test_JobServicesV2() As String
            Get
                Return CType(Me("D1_CourierConnector_service_taxydromiki_test_JobServicesV2"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://voucher.taxydromiki.gr/JobServicesV2.asmx")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydromiki_JobServicesV2() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydromiki_JobServicesV2"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://online.taxydema.gr")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydema_create_TAXYCREATESIDETA() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydema_create_TAXYCREATESIDETA"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://online.taxydema.gr")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydema_delete_TAXYDELETESIDETA() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydema_delete_TAXYDELETESIDETA"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://online.taxydema.gr")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydema_print_TAXYPRINTSIDETA() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydema_print_TAXYPRINTSIDETA"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://online.taxydema.gr")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydema_printA6_TAXYPRINTSIDETAA6() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydema_printA6_TAXYPRINTSIDETAA6"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://online.taxydema.gr")>  _
        Public ReadOnly Property D1_CourierConnector_Web_taxydema_track_TAXYTTSIDETA() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_taxydema_track_TAXYTTSIDETA"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://www.devspdxws.gr/accesspoint.asmx")>  _
        Public ReadOnly Property D1_CourierConnector_Web_speedex_test_AccessPoint() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_speedex_test_AccessPoint"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("https://spdxws.gr/accesspoint.asmx")>  _
        Public ReadOnly Property D1_CourierConnector_Web_speedex_AccessPoint() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_speedex_AccessPoint"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://212.205.47.226:9003")>  _
        Public ReadOnly Property D1_CourierConnector_Web_eltaCourier_create_CREATEAWB() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_eltaCourier_create_CREATEAWB"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://10.10.9.23:9003")>  _
        Public ReadOnly Property D1_CourierConnector_Web_eltaCourier_print_PELB64VG() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_eltaCourier_print_PELB64VG"),String)
            End Get
        End Property
        
        <Global.System.Configuration.ApplicationScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.WebServiceUrl),  _
         Global.System.Configuration.DefaultSettingValueAttribute("http://212.205.47.226:9003")>  _
        Public ReadOnly Property D1_CourierConnector_Web_eltaCourier_track_PELTT01() As String
            Get
                Return CType(Me("D1_CourierConnector_Web_eltaCourier_track_PELTT01"),String)
            End Get
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.D1_CourierConnector.My.MySettings
            Get
                Return Global.D1_CourierConnector.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
