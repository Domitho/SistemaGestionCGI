<%@ Page Title="Categorización Docente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="CategorizacionDocentes.aspx.cs" Inherits="SistemaGestionCGI.CategorizacionDocentes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-form-docentes.css" rel="stylesheet" />
    <style>
        .form-stack { max-width: 100% !important; }
    </style>

    <%-- ENCABEZADO PRINCIPAL --%>
    <div id="headerListado" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-address-card me-2 text-primary"></i> CATEGORIZACIÓN DOCENTE
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevoRegistro" CssClass="btn btn-primary btn-pill d-flex align-items-center shadow-sm" OnClick="btnNuevoRegistro_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO REGISTRO
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: LISTADO --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCategorizacion" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>FECHA</th>
                        <th>DOCENTE</th>
                        <th>CATEGORÍA</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_cat") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFecha_cat")).ToString("dd/MM/yyyy") %></td>
                                <td class="text-start fw-semibold text-primary">
                                    <%# Eval("strApellidos_doc") %> <%# Eval("strNombres_doc") %>
                                    <br /><small class="text-muted"><%# Eval("strCedula_doc") %></small>
                                </td>
                                <td><span class="badge bg-light text-dark border"><%# Eval("strCategorizacion") %></span></td>
                                <td>
                                    <asp:LinkButton runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cat") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cat") %>' 
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" ToolTip="Eliminar"
                                        OnClientClick="return confirm('¿Está seguro de eliminar este registro?');">
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

    <%-- PANEL 2: FORMULARIO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-address-card me-2"></i> CATEGORIZACIÓN DOCENTE
            </h3>
            <asp:LinkButton ID="btnRegresar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4 bg-white">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-pen me-2"></i> 
                <asp:Label ID="lblTituloFormulario" runat="server" Text="Datos de la Categorización"></asp:Label>
            </h4>
            
            <asp:HiddenField ID="hfIdCat" runat="server" />

            <div class="row g-3">
                 <div class="col-12">
                    <label class="form-label fw-bold text-secondary small">Seleccione Docente <span class="text-danger">*</span></label>
                    <div class="input-group shadow-sm">
                        <asp:DropDownList ID="ddlDocente" runat="server" CssClass="form-select"></asp:DropDownList>
                        <button type="button" class="btn btn-success" data-bs-toggle="modal" data-bs-target="#modalNuevoDocente" title="Registrar Nuevo Docente">
                            <i class="fa-solid fa-plus"></i>
                        </button>
                    </div>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Fecha de Categorización <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtFechaCat" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Categoría <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">-- Seleccione --</asp:ListItem>
                        <asp:ListItem>AUXILIAR 1</asp:ListItem>
                        <asp:ListItem>AUXILIAR 2</asp:ListItem>
                        <asp:ListItem>AUXILIAR 3</asp:ListItem>
                        <asp:ListItem>AGREGADO 1</asp:ListItem>
                        <asp:ListItem>AGREGADO 2</asp:ListItem>
                        <asp:ListItem>AGREGADO 3</asp:ListItem>
                        <asp:ListItem>PRINCIPAL 1</asp:ListItem>
                        <asp:ListItem>PRINCIPAL 2</asp:ListItem>
                        <asp:ListItem>PRINCIPAL 3</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="d-flex justify-content-center gap-3 mt-5">
                    <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardar_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelar_Click" CausesValidation="false">
                        <i class="fa-solid fa-ban me-2"></i> Cancelar
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL DE REGISTRO RÁPIDO --%>
    <div class="modal fade" id="modalNuevoDocente" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 rounded-4 overflow-hidden">
                <div class="modal-header bg-utc text-white border-0 py-3">
                    <h5 class="modal-title w-100 text-center fw-bold">
                        <i class="fa-solid fa-user-plus me-2"></i> REGISTRO DE NUEVO DOCENTE
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
            
                <div class="modal-body p-4 bg-light-utc">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Cédula de Identidad <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCedulaNuevo" runat="server" CssClass="form-control" placeholder="Ej: 050..." MaxLength="10" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Función / Cargo <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlFuncionNuevo" runat="server" CssClass="form-select">
                                <asp:ListItem Value="">-- Seleccione --</asp:ListItem>
                                <asp:ListItem>DOCENTE TITULAR</asp:ListItem>
                                <asp:ListItem>DOCENTE CONTRATADO</asp:ListItem>
                                <asp:ListItem>DOCENTE INVITADO</asp:ListItem>
                                <asp:ListItem>INVESTIGADOR</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Nombres <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtNombresNuevo" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Apellidos <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtApellidosNuevo" runat="server" CssClass="form-control" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Facultad <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlFacultadNuevo" runat="server" CssClass="form-select">
                                <asp:ListItem Value="">-- Seleccione --</asp:ListItem>
                                <asp:ListItem>CIYA</asp:ListItem>
                                <asp:ListItem>CAREN</asp:ListItem>
                                <asp:ListItem>CSCP</asp:ListItem>
                                <asp:ListItem>CIYA</asp:ListItem>
                                <asp:ListItem>GAD</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label fw-bold text-dark small">Carrera <span class="text-danger">*</span></label>
                            <asp:DropDownList ID="ddlCarreraNuevo" runat="server" CssClass="form-select">
                                <asp:ListItem Value="">-- Seleccione --</asp:ListItem>
                                <asp:ListItem>INGENIERÍA EN SISTEMAS</asp:ListItem>
                                <asp:ListItem>SOFTWARE</asp:ListItem>
                                <asp:ListItem>ELECTRICIDAD</asp:ListItem>
                                <asp:ListItem>CONTABILIDAD Y AUDITORÍA</asp:ListItem>
                                <asp:ListItem>VETERINARIA</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer border-0 justify-content-center bg-white pb-4">
                    <asp:LinkButton ID="btnGuardarDocenteRapido" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardarDocenteRapido_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> GUARDAR DOCENTE
                    </asp:LinkButton>
                    <button type="button" class="btn btn-outline-secondary btn-pill px-4" data-bs-dismiss="modal">CANCELAR</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function initPlugins() {
            // Solo mantenemos la inicialización de la tabla de datos
            if ($.fn.DataTable && !$.fn.DataTable.isDataTable('#tablaCategorizacion')) {
                $('#tablaCategorizacion').DataTable({
                    responsive: true,
                    language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                    pageLength: 10,
                    dom: "<'row'<'col-md-6'l><'col-md-6'f>>rt<'row'<'col-md-5'i><'col-md-7'p>>"
                });
            }
        }

        Sys.Application.add_load(function () {
            initPlugins();
        });
    </script>

</asp:Content>