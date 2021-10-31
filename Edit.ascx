<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="ffacsa.Modules.Cotizacion.Edit" %>
<%@ Register Assembly="DevExpress.Web.v17.2, Version=17.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<!-- Menu -->
<div>
    <dx:ASPxMenu ID="ASPxMenuActualizar" runat="server" OnItemClick="ASPxMenuActualizar_ItemClick">
        <Items>
            <dx:MenuItem Name="buttonActualizar" Text="Actualizar" Image-Url="~/Portals/0/fIcons/aceptar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Actualizar Cotización">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonDatosEntrega" Text="Datos de Entrega" Image-Url="~/Portals/0/fIcons/entrega.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Datos de Entrega">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonEnviar" Text="Enviar a FFACSA" Image-Url="~/Portals/0/fIcons/enviar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Enviar Cotización a FFACSA">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonAutorizar" Text="Autorizar" Image-Url="~/Portals/0/fIcons/autoriza.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Autorizar Cotización">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonRechazar" Text="Rechazar" Image-Url="~/Portals/0/fIcons/rechazar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Rechazar Cotización">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonImprimir" Text="Imprimir" Image-Url="~/Portals/0/fIcons/impresion.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Imprimir Cotización">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonGuardarPlantilla" Text="Guardar como Solución" Image-Url="~/Portals/0/fIcons/guardar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Guardar como Solución">
            </dx:MenuItem>
            <dx:MenuItem Name="buttonCerrar" Text="Cerrar" Image-Url="~/Portals/0/fIcons/cerrar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Cerrar">
            </dx:MenuItem>
        </Items>
    </dx:ASPxMenu>
</div>

<br />
<!-- Panel de datos Maestros -->
<div>
    <div class="row">
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelCotiEditar" runat="server" Text="Número de Cotización"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxBuscarCotizacion" runat="server" Width="170px" Enabled="false"></dx:ASPxTextBox>
         </div>
        <dx:ASPxLabel ID="ASPxLabelNombrePlantilla" runat="server" Text="Nombre de la Solución"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxPlantilla" runat="server" Width="170px"></dx:ASPxTextBox>
    </div>
        <br />
        <br />
    <div class="row ">    
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelCliente" runat="server" Text="Cliente"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxCliente" runat="server" Width="170px" Enabled="false"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelVendedor" runat="server" Text="Vendedor"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxVendedor" runat="server" Width="170px" Enabled="false"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelNIT" runat="server" Text="NIT"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxNIT" runat="server" Width="170px" CssClass="txtDatos"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelNombre" runat="server" Text="Contacto"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxContacto" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>       
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDireccion" runat="server" Text="Direccion"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDireccion" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelTelefono" runat="server" Text="Telefono"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxTelefono" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelEmail" runat="server" Text="Email"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxEmail" runat="server" Width="170px"></dx:ASPxTextBox>            
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelAgencia" runat="server" Text="Agencia" Visible="false"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxAgencia" runat="server" Width="170px" Visible="false"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelFecha" runat="server" Text="Fecha"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxFecha" runat="server" Width="170px" Enabled="false"></dx:ASPxTextBox>
        </div>    
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelPrestatario" runat="server" Text="Nombre del Prestatario"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxPrestatario" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDirEntrega" runat="server" Text="Direccion de Entrega"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDirEntrega" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelTelPrestatario" runat="server" Text="Teléfono del Prestatario"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxTelPrestatario" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelProceso" runat="server" Text="Estado de la Cotización"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxEstadoCoti" runat="server" Width="170px" Enabled="false" Text="EN PROCESO"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDatos1Entrega" runat="server" Text="Contacto de Entrega 1" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDatos1Entrega" Text="PERSONA RECIBE MATERIAL" runat="server" Width="170px" Enabled="false" Visible="true"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDPI" runat="server" Text="DPI Persona Entrega" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDPI1" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div>          
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelNombrePrestatario1" runat="server" Text="Persona Entrega" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxPersonaEntrega1" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div> 
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelObservaciones1" runat="server" Text="Observaciones/Comentarios" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxObservaciones1" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDatos2Entrega" runat="server" Text="Persona de Entrega 2" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDatos2Entrega" Text="PERSONA RECIBE MATERIAL" runat="server" Width="170px" Enabled="false" Visible="true"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDPI1" runat="server" Text="DPI Persona Entrega" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDPI2" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div>  
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelNombrePrestatario2" runat="server" Text="Persona Entrega" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxPersonaEntrega2" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div> 
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelObservaciones2" runat="server" Text="Observaciones/Comentarios" Visible="true"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxObservaciones2" runat="server" Width="170px" Enabled="true" Visible="true"></dx:ASPxTextBox>
        </div>
        <div class="row col-xs-3">
            <dx:ASPxLabel ID="ASPxLabelDocStatus" runat="server" Text="DocStatus" Visible="false"></dx:ASPxLabel>
            <dx:ASPxTextBox ID="ASPxTextBoxDocStatusOld" runat="server" Width="170px" Enabled="true" Visible="false"></dx:ASPxTextBox>
        </div>
   </div>
</div>
<br />
<!-- Detalle de la Cotizacion -->

<dx:ASPxPageControl ID="ASPxPageControlCoti" runat="server" ActiveTabIndex="0">
    <TabPages>
        <dx:TabPage Name="DetalleCotizacion" Text="Detalle">
            <ContentCollection>
                <dx:ContentControl runat="server">
                    <dx:ASPxGridView runat="server" AutoGenerateColumns="False" KeyFieldName="LineNum" Width="100%" ID="gridCotizacion" OnRowInserting="grid_RowInserting" OnRowUpdating="grid_RowUpdating" OnRowDeleting="gridCotizacion_RowDeleting" OnToolbarItemClick="gridCotizacion_ToolbarItemClick">
                        <Settings ShowFooter="True"></Settings>

                        <SettingsExport FileName="Cotizaciones" EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG"></SettingsExport>
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="LineNum" ReadOnly="True" ShowInCustomizationForm="True" Caption="N&#250;mero de L&#237;nea" VisibleIndex="0">
                                <EditFormSettings Visible="False"></EditFormSettings>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataComboBoxColumn FieldName="ItemCode" ShowInCustomizationForm="True" Name="comboItems" Caption="C&#243;digo de Art&#237;culo" VisibleIndex="1">
                                <PropertiesComboBox DataSourceID="itemsData" TextField="ItemCode" ValueField="ItemCode"></PropertiesComboBox>
                                <EditItemTemplate>
                                    <dx:ASPxGridLookup ID="gridLookupItems" runat="server" AutoGenerateColumns="False" DataSourceID="itemsData" KeyFieldName="ItemCode" OnLoad="glCategory_Load" TextFormatString="{0} - {1}" Value='<%# Bind("ItemCode") %>' Width="260px" MultiTextSeparator="-">
                                        <GridViewProperties>
                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" AllowSelectSingleRowOnly="True" />
                                        </GridViewProperties>
                                        <Columns>
                                            <dx:GridViewDataTextColumn FieldName="ItemCode" Caption="Artículo" VisibleIndex="0"></dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn FieldName="ItemName" Caption="Descripción" VisibleIndex="1"></dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:ASPxGridLookup>
                                </EditItemTemplate>
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataTextColumn FieldName="Dscription" ShowInCustomizationForm="True" Caption="Descripci&#243;n" VisibleIndex="2">
                                <EditFormSettings Visible="False"></EditFormSettings>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Quantity" ShowInCustomizationForm="True" Caption="Cantidad" VisibleIndex="3">
                                <BatchEditModifiedCellStyle HorizontalAlign="Left"></BatchEditModifiedCellStyle>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Price" ShowInCustomizationForm="True" Caption="Precio" VisibleIndex="4">
                                <PropertiesTextEdit DisplayFormatString="Q ###,##0.00"></PropertiesTextEdit>

                                <EditFormSettings Visible="False"></EditFormSettings>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="GTotal" ShowInCustomizationForm="True" Caption="Total de l&#237;nea" VisibleIndex="5">
                                <PropertiesTextEdit DisplayFormatString="Q ###,##0.00"></PropertiesTextEdit>

                                <EditFormSettings Visible="False"></EditFormSettings>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewCommandColumn ShowNewButton="True" ShowEditButton="True" ShowDeleteButton="True" ShowInCustomizationForm="True" VisibleIndex="6"></dx:GridViewCommandColumn>
                        </Columns>
                        <Toolbars>
                            <dx:GridViewToolbar EnableAdaptivity="True">
                                <Items>
                                    <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar a Excel"></dx:GridViewToolbarItem>
                                </Items>
                            </dx:GridViewToolbar>
                        </Toolbars>
                        <TotalSummary>
                            <dx:ASPxSummaryItem ShowInColumn="GTotal" SummaryType="Sum" FieldName="GTotal" DisplayFormat="Q ###,##0.00" ValueDisplayFormat="N2" Tag="Total"></dx:ASPxSummaryItem>
                        </TotalSummary>
                        <Styles>
                            <Footer Font-Bold="True"></Footer>
                        </Styles>
                    </dx:ASPxGridView>
                </dx:ContentControl>
            </ContentCollection>
        </dx:TabPage>
        <dx:TabPage Name="Documentos" Text="Documentos Adjuntos">
            <ContentCollection>
                <dx:ContentControl runat="server">
                     <dx:ASPxGridView ID="ASPxGridViewAdjuntos" runat="server" AutoGenerateColumns="False" KeyFieldName="ID_DOC" SettingsBehavior-AllowFocusedRow="true" >
                        <Settings ShowFilterRow="True" />
                        <SettingsSearchPanel Visible="True" />
                        <Columns>
                            <dx:GridViewDataTextColumn Caption="Archivo" FieldName="NOM_ARCHIVO" Name="NOM_ARCHIVO" VisibleIndex="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataColumn Caption="Documento" FieldName="ID_DOC" VisibleIndex="0">
                                <DataItemTemplate>
                                    <dx:ASPxButton runat="server" ID="buttonEditRow" Image-Url="~/Portals/0/fIcons/descargar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Descargar" Text="Descargar" OnClick="buttonEditRow_Click"></dx:ASPxButton>
                                </DataItemTemplate>
                            </dx:GridViewDataColumn>
                        </Columns>
                     </dx:ASPxGridView>
                    <br />
                    <div runat="server" id="fileUpload">
                        <input runat="server" id="File1" type="file" />
                        <br />
                        <asp:Button runat="server" Text="Cargar Documento" CssClass="btn-primary" ID="Button1" OnClick="Upload"></asp:Button>
                    </div>
                    <br />
                    <!--<div runat="server" id="fileDownLoad">
                        <dx:ASPxButton runat="server" Text="Descargar Vale" ID="buttonVale" OnClick="buttonVale_Click"></dx:ASPxButton>
                        
                    </div>
                        -->
                </dx:ContentControl>
            </ContentCollection>
        </dx:TabPage>
    </TabPages>
</dx:ASPxPageControl>
<!-- DATASOURCE DE LOS ARTICULOS-->
<asp:SqlDataSource ID="itemsData" runat="server"></asp:SqlDataSource>
    


