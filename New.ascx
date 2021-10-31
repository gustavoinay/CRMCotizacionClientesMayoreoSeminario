<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="New.ascx.cs" Inherits="ffacsa.Modules.Cotizacion.New" %>
<%@ Register Assembly="DevExpress.Web.v17.2, Version=17.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<!-- Menu -->
<div>
    <dx:ASPxMenu ID="MenuNuevaCoti" runat="server" OnItemClick="MenuNuevaCoti_ItemClick">
        <Items>
            <dx:MenuItem Name="buttonGuardar" Text="Guardar" Image-Url="~/Portals/0/fIcons/guardar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Guardar Cotización">
            </dx:MenuItem>
            <dx:MenuItem BeginGroup="True" Name="buttonCerrar" Text="Cerrar" Image-Url="~/Portals/0/fIcons/Cerrar.svg" Image-Width="24" Image-Height="24" Image-ToolTip="Cerrar">
            </dx:MenuItem>
        </Items>
    </dx:ASPxMenu>
</div>
<br />
<!-- Panel de datos Maestros -->
<div class="row">
    <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelCliente" runat="server" Text="Cliente"></dx:ASPxLabel>
            <dx:ASPxGridLookup ID="ASPxGridLookup1" runat="server" AutoGenerateColumns="False" MultiTextSeparator="-" TextFormatString="{0} - {1}" KeyFieldName="CardCode" AutoPostBack="True" OnValueChanged="ASPxGridLookup1_ValueChanged">
                <GridViewProperties>
                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True"></SettingsBehavior>
                </GridViewProperties>
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="CardCode" SortIndex="0" SortOrder="Ascending" Caption="C&#243;digo" VisibleIndex="0" Name="CardCode"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="CardName" Caption="Cliente" VisibleIndex="1" Name="CardName"></dx:GridViewDataTextColumn>
                </Columns>
        </dx:ASPxGridLookup>
        </div>
        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelVendedor" runat="server" Text="Vendedor"></dx:ASPxLabel>
        <dx:ASPxGridLookup ID="ASPxGridLookup2" runat="server" AutoGenerateColumns="False" MultiTextSeparator="-" TextFormatString="{0} - {1}" KeyFieldName="SlpCode">
            <Columns>
                <dx:GridViewDataTextColumn FieldName="SlpCode" Name="SlpCode" Caption="Codigo" VisibleIndex="0"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="SlpName" Name="SlpName" Caption="Vendedor" VisibleIndex="1"></dx:GridViewDataTextColumn>
            </Columns>
        </dx:ASPxGridLookup>
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
    <!--

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelNumeroDocumento" runat="server" Text="Numero"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxNumDocumento" runat="server" Width="170px" Enabled="false"></dx:ASPxTextBox>
         </div>   
    -->
        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelFecha" runat="server" Text="Fecha"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxFecha" runat="server" Width="170px"></dx:ASPxTextBox>
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
        <dx:ASPxLabel ID="ASPxLabelTipoSolucion" runat="server" Text="Tipo de Solución"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxTipoSolucion" runat="server" Width="170px"></dx:ASPxTextBox>
        </div> 

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelTrimestre" runat="server" Text="Trimestre"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxTrimestre" runat="server" Width="170px"></dx:ASPxTextBox>
        </div> 

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelSupervisorConstruccion" runat="server" Text="Supervisor de la construcción"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxSupervisorConstruccion" runat="server" Width="170px"></dx:ASPxTextBox>
        </div>     

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelTelSupervisor" runat="server" Text="Teléfono del supervisor"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxTelSupervisor" runat="server" Width="170px"></dx:ASPxTextBox>
        </div> 

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelProceso" runat="server" Text="Estado de la Cotización"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxEstadoCoti" runat="server" Width="170px" Enabled="false" Text="CREANDO COTIZACIÓN"></dx:ASPxTextBox>
        </div>

        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelEstado" runat="server" Text="Estado" Visible="false"></dx:ASPxLabel>
        <dx:ASPxTextBox ID="ASPxTextBoxEstado" runat="server" Width="170px" Visible ="false"></dx:ASPxTextBox>
        </div>

    
</div>    
    <div class="row">
        <div class="row col-xs-3">
        <dx:ASPxLabel ID="ASPxLabelPlantilla" runat="server" Text="Basado en Solución"></dx:ASPxLabel>
       <dx:ASPxComboBox ID="ASPxComboBoxPlantilla" runat="server" ValueType="System.String"></dx:ASPxComboBox>
        </div> 
  
       <div class="row col-xs-3">
       <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text=""></dx:ASPxLabel>
           <br />
       <dx:ASPxButton ID="ASPxButtonBuscaPlantilla" runat="server" Text="Listar Solución" OnClick="ASPxButtonBuscaPlantilla_Click"></dx:ASPxButton>
       </div>
   
      </div>
   
    <br />
    <br />

<!-- Detalle de la Cotizacion -->

<br />

<div class="row">

<div class="col-xs-12">
    <dx:ASPxGridView ID="gridCotizacion" runat="server" AutoGenerateColumns="False" Width="100%"
    KeyFieldName="LineNum" OnRowInserting="grid_RowInserting" OnRowUpdating="grid_RowUpdating">
        <Settings ShowFooter="True" />
    <Columns>
        <dx:GridViewDataTextColumn FieldName="LineNum" Caption="Número de Línea" ReadOnly="true" VisibleIndex="0" EditFormSettings-Visible="False"/>

       
         <dx:GridViewDataComboBoxColumn FieldName="ItemCode" VisibleIndex="1" Name="comboItems" Caption="Código de Artículo">
		    <PropertiesComboBox DataSourceID="itemsData" TextField="ItemCode" ValueField="ItemCode"
			    ValueType="System.String">
		    </PropertiesComboBox>
		    <EditItemTemplate>
			    <dx:ASPxGridLookup ID="gridLookupItems" runat="server" AutoGenerateColumns="False" 
				    DataSourceID="itemsData" KeyFieldName="ItemCode" OnLoad="glCategory_Load" 
				    TextFormatString="{0} - {1}" Value='<%# Bind("ItemCode") %>' Width="260px" MultiTextSeparator="-">
				    <GridViewProperties>
					    <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" 
						    AllowSelectSingleRowOnly="True" />
				    </GridViewProperties>
				    <Columns>
					    <dx:GridViewDataTextColumn FieldName="ItemCode" Caption="Artículo" VisibleIndex="0">
                        </dx:GridViewDataTextColumn>

					    <dx:GridViewDataTextColumn FieldName="ItemName" Caption="Descripción" VisibleIndex="1">
					    </dx:GridViewDataTextColumn>
                        
				    </Columns>
			    </dx:ASPxGridLookup>
		    </EditItemTemplate>
		</dx:GridViewDataComboBoxColumn>
        
        <dx:GridViewDataTextColumn FieldName="Dscription" Caption="Descripción" VisibleIndex="2" EditFormSettings-Visible="False">
          </dx:GridViewDataTextColumn>

        <dx:GridViewDataTextColumn FieldName="Quantity" Caption="Cantidad" VisibleIndex="3" BatchEditModifiedCellStyle-HorizontalAlign="Left">
		</dx:GridViewDataTextColumn>
		
        <dx:GridViewDataTextColumn FieldName="Price" Caption="Precio" VisibleIndex="4" EditFormSettings-Visible="False">
            <PropertiesTextEdit DisplayFormatString="Q ###,##0.00">
            </PropertiesTextEdit>
		</dx:GridViewDataTextColumn>
        <dx:GridViewDataTextColumn FieldName="GTotal" Caption="Total de línea" VisibleIndex="5" EditFormSettings-Visible="False">
            <PropertiesTextEdit DisplayFormatString="Q ###,##0.00">
            </PropertiesTextEdit>
		</dx:GridViewDataTextColumn>
        <dx:GridViewCommandColumn VisibleIndex="6" ShowEditButton="True" ShowNewButton="True" ShowDeleteButton="True"/>
    </Columns>
        <TotalSummary>
            <dx:ASPxSummaryItem DisplayFormat="Q ###,##0.00" FieldName="GTotal" ShowInColumn="GTotal" SummaryType="Sum" ValueDisplayFormat="N2" />
        </TotalSummary>
        <Styles>
            <Footer Font-Bold="True">
            </Footer>
        </Styles>
</dx:ASPxGridView>

<asp:SqlDataSource ID="itemsData" runat="server"></asp:SqlDataSource>
    
</div>
</div>


