<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CentrosInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CentrosInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%-- ESTILOS UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-vista-integrantes.css" rel="stylesheet" />

    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-building-columns me-2"></i> CENTROS DE INVESTIGACIÓN
        </h3>
        <div class="d-flex gap-2">
            <asp:LinkButton runat="server" ID="btnNuevo" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO CENTRO
            </asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" Visible="false" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: GRILLA  --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCentros" class="table table-bordered table-hover table-utc align-middle text-center" style="width: 100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>NOMBRE DEL CENTRO</th>
                        <th>FACULTAD</th>
                        <th>DIRECTOR</th>
                        <th>UBICACIÓN</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCentros" runat="server" OnItemCommand="rptCentros_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_cen") %></td>
                                <td class="text-start fw-semibold"><%# Eval("strNombre_cen") %></td>
                                <td class="text-start"><%# Eval("strFacultad_cen") %></td>
                                <td class="text-start"><%# Eval("NombreDirector") %></td>
                                <td><%# Eval("strUbicacion_cen") %></td>
                                <td>
                                    
                                    <%-- Botón Ver Integrantes --%>
                                    <asp:LinkButton ID="btnVerIntegrantes" runat="server" CommandName="verIntegrantes" 
                                        CommandArgument='<%# Eval("strId_cen") %>'
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1" ToolTip="Ver Integrantes">
                                        <i class="fa-solid fa-users"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="editar" CommandArgument='<%# Eval("strId_cen") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="eliminar" CommandArgument='<%# Eval("strId_cen") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Desea eliminar este centro?');" ToolTip="Eliminar">
                                        <i class="fa-solid fa-trash"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-signature me-2"></i> 
                <asp:Label ID="lblTituloFormulario" runat="server"></asp:Label>
            </h4>
            <asp:HiddenField ID="hfIdCentro" runat="server" />
            
            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Nombre del Centro <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" autocomplete="off" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Facultad / Extensión</label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem>FACULTAD DE CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES (CAREN)</asp:ListItem>
                        <asp:ListItem>FACULTAD DE CIENCIAS DE LA INGENIERIA Y APLICADAS (CIYA)</asp:ListItem>
                        <asp:ListItem>FACULTAD DE CIENCIAS ADMINISTRATIVAS Y ECONOMICAS (CAYE)</asp:ListItem>
                        <asp:ListItem>FACULTAD DE CIENCIAS SOCIALES ARTES Y EDUCACION (CSAYE)</asp:ListItem>
                        <asp:ListItem>FACULTAD CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                        <asp:ListItem>EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem>EXTENSION LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Director Encargado</label>
                    <asp:DropDownList ID="ddlDirector" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Área de Conocimiento</label>
                    <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Ubicación Física</label>
                    <asp:TextBox ID="txtUbicacion" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                     <label class="form-label">Líneas de Investigación</label>
                     <asp:TextBox ID="txtLineas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Separe por comas..." />
                </div>

                <div class="col-md-6">
                     <label class="form-label">Misión</label>
                     <asp:TextBox ID="txtMision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="col-md-6">
                     <label class="form-label">Visión</label>
                     <asp:TextBox ID="txtVision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>

                 <div class="col-md-4">
                    <label class="form-label">Fecha de Aprobación</label>
                    <asp:TextBox ID="txtFechaAprobacion" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap mt-5">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL: LISTADO DE INTEGRANTES --%>
    <div class="modal fade" id="modalIntegrantes" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content shadow-utc border-0 rounded-4">
            
                <%-- Cabecera Institucional --%>
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title fw-bold">
                        <i class="fa-solid fa-users-viewfinder me-2"></i> 
                        Talento Humano del Centro
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <%-- Cuerpo con Diseño de Tarjetas/Lista --%>
                <div class="modal-body p-0 bg-light">
                    <%-- Nombre del Centro (Contexto) --%>
                    <div class="p-3 bg-white border-bottom">
                        <small class="text-uppercase text-muted fw-bold ls-1">Centro Seleccionado</small>
                        <h5 class="text-primary mb-0"><asp:Label ID="lblCentroModal" runat="server" Text="..."></asp:Label></h5>
                    </div>

                    <%-- Contenedor de la lista --%>
                    <div class="p-3">
                        <asp:Repeater ID="rptIntegrantesModal" runat="server">
                            <HeaderTemplate>
                                <div class="list-group shadow-sm">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="list-group-item list-group-item-action d-flex align-items-center p-3 border-0 border-bottom">
                                    <%-- Icono / Avatar --%>
                                    <div class="flex-shrink-0 me-3">
                                        <div class="bg-primary bg-opacity-10 text-primary rounded-circle d-flex align-items-center justify-content-center" style="width: 45px; height: 45px;">
                                            <i class="fa-solid fa-user"></i>
                                        </div>
                                    </div>
                                
                                    <%-- Datos Principales --%>
                                    <div class="flex-grow-1">
                                        <h6 class="mb-0 fw-bold text-dark"><%# Eval("NombreCompleto") %></h6>
                                        <small class="text-muted d-block">
                                            <i class="fa-solid fa-briefcase me-1"></i> <%# Eval("strFuncion_int") %> 
                                            <span class="mx-1">|</span> 
                                            <i class="fa-solid fa-layer-group me-1"></i> <%# Eval("strNombre_gru") %>
                                        </small>
                                    </div>

                                    <%-- Badges / Estado --%>
                                    <div class="text-end">
                                        <span class='badge rounded-pill <%# Eval("strTipo_int").ToString() == "Interno" ? "bg-success" : "bg-warning text-dark" %>'>
                                            <%# Eval("strTipo_int") %>
                                        </span>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <%-- Mensaje si no hay datos --%>
                                <asp:Label ID="lblVacio" runat="server" Visible='<%# rptIntegrantesModal.Items.Count == 0 %>' 
                                    CssClass="d-block text-center p-4 text-muted">
                                    <i class="fa-solid fa-user-slash fa-2x mb-2 opacity-50"></i><br/>
                                    No hay integrantes registrados en este centro.
                                </asp:Label>
                                </div> </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <%-- Pie del Modal --%>
                <div class="modal-footer border-0 bg-white">
                    <button type="button" class="btn btn-outline-primary btn-pill px-4" data-bs-dismiss="modal">
                        Cerrar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        const dtConfig = {
            responsive: true, autoWidth: false, ordering: true, pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row mb-3'<'col-sm-12 text-center'B>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                { extend: 'excelHtml5', text: '<i class="fa-solid fa-file-excel"></i> Excel', className: 'btn btn-success btn-sm rounded-pill mx-1' },
                { extend: 'pdfHtml5', text: '<i class="fa-solid fa-file-pdf"></i> PDF', className: 'btn btn-danger btn-sm rounded-pill mx-1', orientation: 'landscape' },
                { extend: 'print', text: '<i class="fa-solid fa-print"></i> Imprimir', className: 'btn btn-secondary btn-sm rounded-pill mx-1' }
            ]
        };

        Sys.Application.add_load(function () {
            if ($.fn.DataTable && $.fn.DataTable.isDataTable('#tablaCentros')) {
                $('#tablaCentros').DataTable().destroy();
            }
            if ($('#tablaCentros').length) {
                $('#tablaCentros').DataTable(dtConfig);
            }
        });

        function AbrirModalIntegrantes() {
            var el = document.getElementById('modalIntegrantes');
            var modal = new bootstrap.Modal(el);
            modal.show();
        }

    </script>
</asp:Content>