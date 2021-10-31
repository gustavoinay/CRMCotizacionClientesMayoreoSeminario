<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="ffacsa.Modules.Cotizacion.View" %>
<%@ Register Assembly="DevExpress.Web.v17.2, Version=17.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>


<div>
    <dx:ASPxMenu ID="ASPxMenu1" runat="server" OnItemClick="ASPxMenu1_ItemClick">
        <Items>
            <dx:MenuItem Name="buttonNew" Text="Nueva" Image-Url="~/Portals/0/fIcons/NewDocument.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Agregar Cotización">
            </dx:MenuItem>           
        </Items>
    </dx:ASPxMenu>
</div>
<br/>
<div>
    <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="False" KeyFieldName="DocNum" >
        <Settings ShowFilterRow="True" />
        <SettingsSearchPanel Visible="True" />
        <Columns>
            <dx:GridViewDataColumn Caption="Documento" FieldName="DocNum" VisibleIndex="0">
                <DataItemTemplate>
                    <dx:ASPxButton runat="server" ID="buttonEditRow" RenderMode="Link" Image-Url="~/Portals/0/fIcons/Edit.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Editar Cotización" Text='<%# Eval("DocNum") %>' AutoPostBack="false" OnClick="buttonEditRow_Click" ></dx:ASPxButton>
                </DataItemTemplate>
            </dx:GridViewDataColumn>
            <dx:GridViewDataTextColumn Caption="Fecha" FieldName="DOCDATE" Name="DOCDATE" VisibleIndex="1">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Codigo Cliente" FieldName="CARDCODE" Name="CARDCODE" VisibleIndex="2">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Cliente" FieldName="CARDNAME" Name="CARDNAME" VisibleIndex="3">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="NIT" FieldName="U_FACNIT" Name="U_FACNIT" VisibleIndex="4">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Contacto" FieldName="U_FACNOM" Name="U_FACNOM" VisibleIndex="5">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Teléfono" FieldName="U_TELEFONOS" Name="U_TELEFONOS" VisibleIndex="6">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Prestatario" FieldName="NOM_PRESTATARIO" Name="NOM_PRESTATARIO" VisibleIndex="7">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Total" FieldName="DOCTOTAL" Name="DOCTOTAL" VisibleIndex="8">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Estado" FieldName="DES_ESTADO" Name="DES_ESTADO"  VisibleIndex="9">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Referencia" FieldName="NUMFACTURA" Name="NUMFACTURA" VisibleIndex="10">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Factura" FieldName="NUMERO_ELECTRONICO" Name="NUMERO_ELECTRONICO" VisibleIndex="11">
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn Caption="Comentarios" FieldName="COMMENTS" Name="COMMENTS" VisibleIndex="12">
            </dx:GridViewDataTextColumn>
        </Columns>
    </dx:ASPxGridView>
</div>