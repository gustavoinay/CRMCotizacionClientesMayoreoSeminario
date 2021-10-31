<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="ffacsa.Modules.Cotizacion.View" %>
<%@ Register Assembly="DevExpress.Web.v17.2, Version=17.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<dx:ASPxGridView ID="ASPxGridView1" runat="server" Settings-ShowFilterBar="Hidden" Settings-ShowGroupPanel="True" Width="100%" SettingsPager-PageSize="20" Settings-ShowFilterRow="True" Settings-ShowFilterRowMenu="True" Settings-ShowFilterRowMenuLikeItem="True">


<SettingsPager PageSize="20"></SettingsPager>

<Settings ShowFilterRow="True" ShowFilterRowMenu="True" ShowFilterRowMenuLikeItem="True" ShowGroupPanel="True" ShowHeaderFilterButton="True"></Settings>
    <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
    <SettingsSearchPanel Visible="True" />


</dx:ASPxGridView>
