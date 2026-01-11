<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CentrosInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CentrosInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- RECURSOS DE ESTILO (UTC DESIGN) --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <%-- ENCABEZADO PRINCIPAL (Estilo Grupos) --%>
    <div id="headerCentros" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-building-columns me-2"></i> CENTROS DE INVESTIGACIÓN
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevo" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO CENTRO
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: LISTADO (GRILLA) --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCentros" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>NOMBRE</th>
                        <th>FACULTAD</th>
                        <th>DIRECTOR</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCentros" runat="server" OnItemCommand="rptCentros_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_cen") %></td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("strNombre_cen") %></td>
                                <td class="text-start small text-muted"><%# Eval("strFacultad_cen") %></td>
                                <td class="text-start"><%# Eval("NombreDirector") ?? "--- SIN ASIGNAR ---" %></td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_cen")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnIntegrantes" runat="server" CommandName="Integrantes" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1 text-white" ToolTip="Gestionar Integrantes">
                                        <i class="fa-solid fa-users"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Eliminar centro?');" ToolTip="Eliminar">
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

    <%-- PANEL 2: FORMULARIO DE CENTRO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-building-columns me-2"></i> GESTIÓN DE CENTRO
            </h3>
            <asp:LinkButton ID="btnRegresar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-pen me-2"></i> Datos del Centro
            </h4>

            <asp:HiddenField ID="hfIdCentro" runat="server" />
            
            <div class="row g-3">
                <div class="col-md-12">
                    <label class="form-label">Nombre del Centro <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
                <div class="col-md-12">
                    <label class="form-label fw-bold text-primary">Director Encargado</label>
                    <div class="input-group">
                        <span class="input-group-text bg-light border-end-0"><i class="fa-solid fa-user-tie text-primary"></i></span>
                        <asp:TextBox ID="txtDirectorActual" runat="server" CssClass="form-control bg-light border-start-0" ReadOnly="true" placeholder="Sin asignar..." />
                    </div>
                    <small class="text-muted">* Para asignar, guarde el centro y use el botón de Integrantes en la lista principal.</small>
                </div>
                
                <div class="col-md-6">
                    <label class="form-label">Facultad</label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem>CIENCIAS DE LA INGENIERÍA Y APLICADAS</asp:ListItem>
                        <asp:ListItem>CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES</asp:ListItem>
                        <asp:ListItem>CIENCIAS ADMINISTRATIVAS Y ECONÓMICAS</asp:ListItem>
                        <asp:ListItem>CIENCIAS SOCIALES ARTES Y EDUCACIÓN</asp:ListItem>
                        <asp:ListItem>CIENCIAS DE LA SALUD</asp:ListItem>
                        <asp:ListItem>EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem>EXTENSIÓN LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Área</label><asp:TextBox ID="txtArea" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Ubicación</label><asp:TextBox ID="txtUbicacion" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Fecha Aprobación</label><asp:TextBox ID="txtFechaAprobacion" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-12"><label class="form-label">Líneas de Inv.</label><asp:TextBox ID="txtLineas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                <div class="col-12"><label class="form-label">Misión</label><asp:TextBox ID="txtMision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                <div class="col-12"><label class="form-label">Visión</label><asp:TextBox ID="txtVision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarCentro" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL 3: LISTADO DE INTEGRANTES --%>
    <%-- PANEL 3: LISTADO DE INTEGRANTES (DISEÑO IDÉNTICO A GRUPOS) --%>
    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        
        <%-- ENCABEZADO TIPO CARD (Igual a Grupos) --%>
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnNuevoIntegrante" CssClass="btn btn-primary btn-pill" OnClick="btnNuevoIntegrante_Click">
                    <i class="fa-solid fa-user-plus me-2"></i> NUEVO INTEGRANTE
                </asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnVolverCentro" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnVolverCentro_Click">
                    <i class="fa-solid fa-arrow-left me-2"></i> VOLVER A CENTROS
                </asp:LinkButton>
            </div>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <%-- Subtítulo de contexto --%>
            <h5 class="text-primary border-bottom pb-2 mb-3">
                <small class="text-muted small fw-normal">Centro:</small> 
                <asp:Label ID="lblNombreCentroSeleccionado" runat="server" Font-Bold="true"></asp:Label>
            </h5>
            
            <table id="tablaIntegrantes" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>CÉDULA</th>
                        <th>NOMBRE</th>
                        <th>FUNCIÓN</th>
                        <th>TIPO</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand">
                        <ItemTemplate>
                            <%-- LÓGICA DE COLOR: Blanco si está activo, Gris si está inactivo (Igual a Grupos) --%>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "" : "table-secondary text-muted" %>'>
                                
                                <td><%# Eval("strCedula_cin") %></td>
                                
                                <%-- Nombre en Azul y Negrita --%>
                                <td class="text-start fw-semibold text-primary">
                                    <%# Eval("NombreCompleto") %>
                                </td>
                                
                                <td class="text-start"><%# Eval("strFuncion_cin") %></td>
                                <td><%# Eval("strTipo_cin") %></td>
                                
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_cin")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                                
                                <td>
                                    <asp:LinkButton ID="btnEditarInt" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminarInt" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "btn btn-outline-danger btn-sm rounded-circle" : "btn btn-outline-success btn-sm rounded-circle" %>' 
                                        OnClientClick="return confirm('¿Está seguro de cambiar el estado de este integrante?');" 
                                        ToolTip='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "Dar de Baja" : "Activar" %>'>
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- PANEL 4: FORMULARIO INTEGRANTE --%>
    <asp:Panel ID="pnlFormularioInt" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES</h3>
            <asp:LinkButton ID="btnCancelarIntTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                OnClick="btnCancelarInt_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-user-pen me-2"></i> Datos del Integrante
            </h4>

            <asp:HiddenField ID="hfIdIntegrante" runat="server" />
            
            <div class="row g-3">
                <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>
                <div class="col-md-4"><label class="form-label">Cédula</label><asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" MaxLength="15"/></div>
                <div class="col-md-4"><label class="form-label">Nombres</label><asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control"/></div>
                <div class="col-md-4"><label class="form-label">Apellidos</label><asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control"/></div>
                <div class="col-md-6"><label class="form-label">Correo</label><asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email"/></div>
                
                <div class="col-md-6">
                    <label class="form-label">Tipo</label>
                    <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select">
                        <asp:ListItem>Interno (UTC)</asp:ListItem>
                        <asp:ListItem>Externo</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Carrera / Entidad</label><asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control" placeholder="Carrera o Universidad de origen"/></div>

                <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Datos de Asignación</h6></div>
                <div class="col-md-6">
                    <label class="form-label fw-bold">Función (Cargo)</label>
                    <asp:DropDownList ID="ddlFuncionInt" runat="server" CssClass="form-select">
                        <asp:ListItem>Investigador</asp:ListItem>
                        <asp:ListItem>Coordinador</asp:ListItem>
                        <asp:ListItem>Analista</asp:ListItem>
                        <asp:ListItem Value="Director" class="fw-bold">DIRECTOR (Jefe del Centro)</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardarInt" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarInt_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- SCRIPTS NECESARIOS (IGUAL QUE EN GRUPOS) --%>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.8/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.8/js/dataTables.bootstrap5.min.js"></script>
    <script>
        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        function initTables() {
            if ($.fn.DataTable.isDataTable('#tablaCentros')) $('#tablaCentros').DataTable().destroy();
            if ($.fn.DataTable.isDataTable('#tablaIntegrantes')) $('#tablaIntegrantes').DataTable().destroy();

            $('#tablaCentros').DataTable(dtConfig);
            $('#tablaIntegrantes').DataTable(dtConfig);
        }

        // Inicialización al cargar la página y después de UpdatePanel
        $(function () { initTables(); });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () { initTables(); });
    </script>

</asp:Content>