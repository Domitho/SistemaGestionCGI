<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EjecucionProAprobados.aspx.cs" Inherits="SistemaGestionCGI.EjecucionProAprobados" %>
<%@ Register Src="~/GeneradorInforme.ascx" TagPrefix="uc" TagName="GeneradorInforme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-informes.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <div id="headerEjecucion" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-person-digging me-2"></i> PROYECTOS EN EJECUCIÓN
        </h3>

        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevoEjecucion" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevoEjecucion_Click">
                <i class="fa-solid fa-plus me-2"></i> INICIAR EJECUCIÓN
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaEjecucion" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>PROYECTO</th>
                        <th>COORDINADOR</th>
                        <th>PERIODO</th>
                        <th>INICIO</th>
                        <th>FIN</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptEjecucion" runat="server" OnItemCommand="rptEjecucion_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_ejec") %></td>
                                <td class="text-start fw-bold text-primary"><%# Eval("TituloProyecto") %></td>
                                <td class="text-start"><%# Eval("strCoordinador_ejec") %></td>
                                <td><%# Eval("strPeriodo_ejec") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_ejec")).ToString("dd/MM/yyyy") %></td>
                                <td><%# Eval("dtFechafin_ejec") != DBNull.Value ? Convert.ToDateTime(Eval("dtFechafin_ejec")).ToString("dd/MM/yyyy") : "-" %></td>
                                <td>
                                    <span class='badge bg-info text-dark'><%# Eval("strEstado_ejec") %></span>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_ejec") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEquipo" runat="server" CommandName="Equipo" CommandArgument='<%# Eval("strId_ejec") %>'
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1" ToolTip="Gestionar Integrantes">
                                        <i class="fa-solid fa-users-gear"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnInformes" runat="server" CommandName="Informes" CommandArgument='<%# Eval("strId_ejec") %>'
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" ToolTip="Subir Informes/Avances">
                                        <i class="fa-solid fa-folder-open"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_ejec") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle"
                                        OnClientClick="return confirm('¿Está seguro de eliminar este registro y su equipo?');" ToolTip="Eliminar">
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

    <asp:Panel ID="pnlAgregar" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-circle-plus me-2"></i> Iniciar Nueva Ejecución
            </h4>

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label fw-bold">Proyecto Aprobado</label>
                    <asp:DropDownList ID="ddlProyectosAprobados" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlProyectosAprobados_SelectedIndexChanged">
                    </asp:DropDownList>
                    <div class="form-text text-primary">
                        <i class="fa-solid fa-circle-info"></i> Solo se muestran proyectos aprobados pendientes de iniciar ejecución.
                    </div>
                </div>

                <div class="col-12">
                    <label class="form-label">Coordinador</label>
                    <asp:TextBox ID="txtCoordinadorAdd" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Fecha Inicio</label>
                    <asp:TextBox ID="txtFechaIniAdd" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Periodo / Ciclo</label>
                    <asp:TextBox ID="txtPeriodoAdd" runat="server" CssClass="form-control" placeholder="Ej: Octubre 2025 - Marzo 2026" />
                </div>

                <div class="col-12">
                    <label class="form-label fw-semibold">Informe Inicial / Planificación (Opcional)</label>
                    
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoAdd">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-paperclip"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
                        <div class="utc-fileinput-preview" id="previewArchivoAdd"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoAdd"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivoAdd"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />Arrastra archivo aquí.</div>
                        <asp:FileUpload ID="flpArchivoAdd" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="btnGuardarNew" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarNew_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarNew" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarNew_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEditar" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-pen-to-square me-2"></i> Editar Ejecución
            </h4>

            <asp:HiddenField ID="hfIdEjecEdit" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label fw-bold">Proyecto</label>
                    <asp:TextBox ID="txtProyectoReadOnly" runat="server" CssClass="form-control" ReadOnly="true" BackColor="#e9ecef" />
                </div>

                <div class="col-12">
                    <label class="form-label">Coordinador</label>
                    <asp:TextBox ID="txtCoordinadorEdit" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Fecha Inicio</label>
                    <asp:TextBox ID="txtFechaIniEdit" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Fecha Fin</label>
                    <asp:TextBox ID="txtFechaFinEdit" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-12">
                    <label class="form-label">Periodo / Ciclo</label>
                    <asp:TextBox ID="txtPeriodoEdit" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                    <label class="form-label fw-semibold">Reemplazar Archivo (Opcional)</label>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoEdit">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-paperclip"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
                        <div class="utc-fileinput-preview" id="previewArchivoEdit"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoEdit"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivoEdit"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />Arrastra archivo aquí.</div>
                        <asp:FileUpload ID="flpArchivoEdit" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="btnGuardarEdit" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarEdit_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Actualizar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarEdit" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarEdit_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEquipoListado" runat="server" Visible="false">
        <asp:HiddenField ID="hfIdEjecucionEquipo" runat="server" />

        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> EQUIPO DE TRABAJO
            </h3>

            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnAbrirFormMiembro"
                    CssClass="btn btn-primary btn-pill d-flex align-items-center"
                    OnClick="btnAbrirFormMiembro_Click">
                    <i class="fa-solid fa-user-plus me-2"></i> NUEVO INTEGRANTE
                </asp:LinkButton>

                <asp:LinkButton runat="server" ID="btnVolverDeEquipo"
                    CssClass="btn btn-outline-primary btn-pill px-4"
                    OnClick="btnVolverDeEquipo_Click">
                    <i class="fa-solid fa-chevron-left me-2"></i> VOLVER A PROYECTOS
                </asp:LinkButton>
            </div>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaMiembros" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>CÉDULA</th>
                        <th>NOMBRES</th>
                        <th>APELLIDOS</th>
                        <th>FACULTAD</th>
                        <th>ROL</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptMiembros" runat="server" OnItemCommand="rptMiembros_ItemCommand">
                        <ItemTemplate>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "" : "table-secondary text-muted" %>'>
                                <td><%# Eval("strId_miembro") %></td>
                                <td><%# Eval("strCedula_miembro") %></td>
                                <td class="text-start"><%# Eval("strNombres_miembro") %></td>
                                <td class="text-start"><%# Eval("strApellidos_miembro") %></td>
                                <td><%# Eval("strFacultad_miembro") %></td>
                                <td><%# Eval("strRol_miembro") %></td>

                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_miembro")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>

                                <td>
                                    <asp:LinkButton ID="btnEditarM" runat="server"
                                        CommandName="EditarMiembro" CommandArgument='<%# Eval("strId_miembro") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnToggleEstado" runat="server"
                                        CommandName="CambiarEstado" CommandArgument='<%# Eval("strId_miembro") %>'
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "btn btn-outline-danger btn-sm rounded-circle me-1" : "btn btn-outline-success btn-sm rounded-circle me-1" %>'
                                        ToolTip='<%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "Dar de Baja" : "Reactivar" %>'>
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnHistorial" runat="server"
                                        CommandName="VerHistorial" CommandArgument='<%# Eval("strId_miembro") %>'
                                        CssClass="btn btn-info btn-sm rounded-circle text-white" ToolTip="Ver Historial">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormularioMiembro" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users-gear me-2"></i> EQUIPO DE TRABAJO
            </h3>

            <asp:LinkButton ID="btnVolverFormMiembro" runat="server"
                CssClass="btn btn-outline-primary btn-pill px-4"
                OnClick="btnCancelarMiembro_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-user-plus me-2"></i>
                <asp:Label runat="server" ID="lblTituloFormMiembro" Text="Nuevo Integrante" />
            </h4>

            <asp:HiddenField ID="hfIdMiembroEdit" runat="server" />

            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Cédula de Identidad</label>
                    <asp:TextBox ID="txtCedulaMiembro" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Rol en el Proyecto</label>
                    <asp:DropDownList ID="ddlRolMiembro" runat="server" CssClass="form-select">
                        <asp:ListItem>Investigador</asp:ListItem>
                        <asp:ListItem>Ayudante de Investigación</asp:ListItem>
                        <asp:ListItem>Tesista</asp:ListItem>
                        <asp:ListItem>Técnico de Apoyo</asp:ListItem>
                        <asp:ListItem>Externo</asp:ListItem>
                        <asp:ListItem>Director</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-12">
                    <label class="form-label">Facultad / Extensión</label>
                    <asp:DropDownList ID="ddlFacultadMiembro" runat="server" CssClass="form-select">
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
                    <label class="form-label">Nombres</label>
                    <asp:TextBox ID="txtNombresMiembro" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Apellidos</label>
                    <asp:TextBox ID="txtApellidosMiembro" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnGuardarMiembro" runat="server" CssClass="btn btn-primary btn-pill px-4"
                    OnClick="btnGuardarMiembro_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Integrante
                </asp:LinkButton>

                <asp:LinkButton ID="btnCancelarMiembro" runat="server" CssClass="btn btn-outline-primary btn-pill px-4"
                    OnClick="btnCancelarMiembro_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalInformes" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 shadow-utc rounded-4">

                <div class="modal-header bg-utc text-white position-relative d-flex justify-content-center align-items-center py-3">
                    <h5 class="modal-title fw-bold m-0">
                        <i class="fa-solid fa-folder-open me-2"></i> Archivos del Proyecto
                    </h5>
                    <button type="button" class="btn-close btn-close-white position-absolute end-0 me-3" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-light p-4">
                    <asp:HiddenField ID="hfIdEjecucionInforme" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfIdInformeEdit" runat="server" ClientIDMode="Static" />

                    <div class="utc-toolbar">
                        <div class="d-flex align-items-center gap-3">
                            <div class="bg-light rounded-circle p-2 d-flex align-items-center justify-content-center" style="width: 45px; height: 45px;">
                                <i class="fa-solid fa-folder-tree text-primary fs-5"></i>
                            </div>
                            <div class="utc-toolbar-text">
                                <h6>Repositorio Digital</h6>
                                <small>Gestione los informes de avance del proyecto</small>
                            </div>
                        </div>

                        <div class="d-flex gap-2">
                            <asp:LinkButton ID="btnAbrirGenerador" runat="server" CssClass="btn btn-outline-primary d-flex align-items-center" OnClick="btnAbrirGenerador_Click">
                                <i class="fa-solid fa-wand-magic-sparkles me-2"></i> Generar Informe
                            </asp:LinkButton>

                            <button type="button" class="btn-upload-modern" onclick="LimpiarYSubir()">
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Subir Escaneado
                            </button>
                        </div>

                    </div>

                    <div class="row g-3">
                        <asp:Repeater ID="rptInformes" runat="server" OnItemCommand="rptInformes_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-4 col-sm-6">

                                    <div class="file-card" onclick="GestionarArchivoDirecto('<%# ResolveUrl(Eval("strArchivo_path").ToString()) %>')">

                                        <div class="position-absolute top-0 end-0 p-2 d-flex gap-1" style="z-index: 10;">

                                            <asp:LinkButton ID="btnEditarInf" runat="server"
                                                CommandName="EditarInforme" CommandArgument='<%# Eval("strId_informe") %>'
                                                CssClass="btn btn-sm btn-light rounded-circle shadow-sm text-primary"
                                                OnClientClick="event.stopPropagation();"
                                                ToolTip="Corregir archivo">
                                                <i class="fa-solid fa-pen"></i>
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnEliminarInf" runat="server"
                                                CommandName="EliminarInforme" CommandArgument='<%# Eval("strId_informe") %>'
                                                CssClass="btn btn-sm btn-light rounded-circle shadow-sm text-danger"
                                                OnClientClick="event.stopPropagation(); return confirm('¿CONFIRMACIÓN:\n\nVa a eliminar este documento permanentemente.\n¿Continuar?');"
                                                ToolTip="Eliminar">
                                                <i class="fa-solid fa-trash-can"></i>
                                            </asp:LinkButton>
                                        </div>

                                        <div class="file-card-preview">
                                            <i class='<%# Eval("strArchivo_path").ToString().ToLower().EndsWith(".pdf") 
                                                ? "fa-solid fa-file-pdf text-danger" 
                                                : "fa-solid fa-file-word text-primary" %>'>
                                            </i>
                                        </div>

                                        <div class="file-card-body">
                                            <div class="file-card-title" title='<%# Eval("strNombrePeriodo") %>'>
                                                <%# Eval("strNombrePeriodo") %>
                                            </div>
                                            <div class="file-card-meta">
                                                <span>
                                                    <i class="fa-solid fa-calendar-days me-1"></i>
                                                    <%# Convert.ToDateTime(Eval("dtFechaSubida")).ToString("dd MMM") %>
                                                </span>
                                                <span><i class="fa-solid fa-download text-muted"></i></span>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </ItemTemplate>

                            <FooterTemplate>
                                <asp:Panel ID="pnlNoData" runat="server" Visible='<%# rptInformes.Items.Count == 0 %>'>
                                    <div class="text-center py-5 text-muted opacity-50">
                                        <i class="fa-regular fa-folder-open fa-4x mb-3"></i>
                                        <h5>Carpeta vacía</h5>
                                        <p>Usa el botón "Subir Nuevo" para agregar informes.</p>
                                    </div>
                                </asp:Panel>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="modal-footer bg-light border-top-0 justify-content-center">
                    <button type="button" class="btn btn-secondary btn-pill px-5" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <uc:GeneradorInforme ID="ucGenerador" runat="server" OnInformeGuardado="ucGenerador_InformeGuardado" />

    <div class="modal fade" id="modalSubirInforme" tabindex="-1" aria-hidden="true" style="z-index: 1060;" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-lg border-0 rounded-4">

                <div class="modal-header border-bottom-0 pb-0">
                    <h5 class="modal-title fw-bold text-primary" id="lblTituloModalInforme" runat="server">
                        <i class="fa-solid fa-cloud-arrow-up"></i> Subir Informe
                    </h5>
                    <button type="button" class="btn-close" onclick="CerrarSubModalUpload()"></button>
                </div>

                <div class="modal-body pt-3 px-4 pb-4">
                    <p class="text-muted small mb-3">El archivo se vinculará al proyecto actual.</p>

                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtNombrePeriodoInf" runat="server" CssClass="form-control" placeholder="Nombre" />
                        <label>Nombre del Periodo / Informe</label>
                    </div>

                    <label class="form-label fw-bold small text-secondary">Archivo Word (.doc, .docx)</label>

                    <div class="utc-fileinput-wrapper" id="wrapperArchivoInf">

                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-word"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>

                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Nuevo nombre del archivo..." />

                        <div class="utc-fileinput-preview" id="previewArchivoInf"></div>

                        <div class="utc-fileinput-loader" id="loaderArchivoInf">
                            <i class="fa-solid fa-spinner fa-spin me-2"></i> Procesando...
                        </div>

                        <div class="utc-dropzone" id="dropzoneArchivoInf">
                            <div class="mb-2">
                                <i class="fa-solid fa-file-word fa-2x text-primary me-2"></i>
                                <i class="fa-solid fa-file-pdf fa-2x text-danger"></i>
                            </div>
                            Arrastra documento Word o PDF aquí o haz clic
                        </div>

                        <asp:FileUpload ID="flpArchivoInf" runat="server" CssClass="utc-fileinput-input"
                            accept=".doc,.docx,.pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/pdf" />
                    </div>

                    <div class="form-text small text-muted mb-3">
                        <i class="fa-solid fa-circle-info me-1"></i> Si está editando, suba un archivo solo si desea reemplazar el actual.
                    </div>

                    <div class="d-grid gap-2">
                        <asp:LinkButton ID="btnGuardarInforme" runat="server"
                            CssClass="btn btn-primary btn-lg shadow-sm"
                            OnClientClick="return validarPesoArchivo();"
                            OnClick="btnGuardarInforme_Click">
                            <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Informe
                        </asp:LinkButton>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" style="z-index: 1070;" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                
                <%-- Cabecera Oscura --%>
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title" id="lblTituloPreview" runat="server" ClientIDMode="Static">Vista Previa</h6>
                    <div>
                        <button type="button" id="btnImprimirReporte" class="btn btn-sm btn-light me-2" onclick="imprimirReporteJS()" style="display: none;" runat="server" ClientIDMode="Static">
                            <i class="fa-solid fa-print"></i> Imprimir
                        </button>

                        <a id="btnDescargarDirecto" href="#" target="_blank" class="btn btn-sm btn-outline-light me-2">
                            <i class="fa-solid fa-download"></i> Descargar
                        </a>

                        <button type="button" class="btn-close btn-close-white" onclick="CerrarVistaPrevia()"></button>
                    </div>
                </div>

                <div class="modal-body p-4" style="background: white; min-height: 500px;">
                    
                    <iframe id="framePdf" class="pdf-viewer-frame" style="width: 100%; height: 100%; min-height: 500px; border: none; display:none;"></iframe>

                    <asp:Panel ID="pnlReporteHtml" runat="server" Visible="false">
                        <div id="arealmpresion" class="report-paper" ClientIDMode="Static">
                            <asp:Literal ID="litReporteGenerado" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEstadoMiembro" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-shield-halved me-2"></i> Auditoría de Estado</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p class="text-center fs-5">¿Confirmar cambio de estado?</p>

                    <div class="alert alert-light border text-center small">
                        <strong id="lblNombreMiembroEstado">...</strong><br />
                        <span id="lblRolMiembroEstado" class="text-muted">...</span>
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-bold">Motivo del cambio (Obligatorio)</label>
                        <textarea id="txtMotivoCambio" class="form-control" rows="3" placeholder="Especifique la razón..."></textarea>

                        <asp:HiddenField ID="hfMotivoHidden" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="hfIdMiembroEstado" runat="server" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnConfirmarEstado" runat="server"
                        CssClass="btn btn-primary btn-pill px-4"
                        OnClientClick="return guardarMotivoJS();"
                        OnClick="btnConfirmarEstado_Click">
                        <i class="fa-solid fa-check me-2"></i> Confirmar
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalHistorialMiembro" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content shadow-utc border-0 rounded-4">
                
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100 text-center">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i> HISTORIAL DE MOVIMIENTOS
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-white">
                    
                    <div class="d-flex justify-content-between align-items-center mb-3 border-bottom pb-3">
                        <h6 class="fw-bold text-secondary mb-0">
                            INTEGRANTE: <asp:Label ID="lblNombreHistorial" runat="server" CssClass="text-primary text-uppercase" Text="..." />
                        </h6>

                        <asp:HiddenField ID="hfIdIntegranteHistorial" runat="server" />

                        <asp:LinkButton ID="btnGenerarReporteHistorial" runat="server"
                            CssClass="btn btn-danger btn-pill btn-sm px-4 shadow-sm"
                            OnClick="btnGenerarReporteHistorial_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte Oficial
                        </asp:LinkButton>
                    </div>

                    <div class="table-responsive rounded border-0">
                        <table class="table table-sm table-hover table-historial-utc align-middle text-center mb-0">
                            <thead>
                                <tr>
                                    <th style="width: 15%">FECHA</th>
                                    <th style="width: 15%">ACCIÓN</th>
                                    <th style="width: 55%">MOTIVO / DETALLE</th>
                                    <th style="width: 15%">USUARIO</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptHistorialMiembro" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-secondary fw-bold" style="font-size: 0.85rem;">
                                                <%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd/MM/yyyy HH:mm") %>
                                            </td>
                                            <td>
                                                <span class='badge rounded-pill px-3 <%# 
                                                    Eval("strAccion").ToString().Contains("BAJA") ? "badge-baja" : 
                                                    (Eval("strAccion").ToString().Contains("NUEVO") || Eval("strAccion").ToString().Contains("REACTIVAR") ? "badge-alta" : "badge-historial") 
                                                %>'>
                                                    <%# Eval("strAccion") %>
                                                </span>
                                            </td>
                                            <td class="text-start fst-italic text-muted small ps-3">
                                                <%# Eval("strMotivo") %>
                                            </td>
                                            <td class="small fw-bold text-secondary">
                                                <i class="fa-solid fa-user-check me-1 opacity-50"></i>
                                                <%# Eval("strUsuario") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="pnlNoData" runat="server" Visible='<%# rptHistorialMiembro.Items.Count == 0 %>'>
                                            <tr>
                                                <td colspan="4" class="p-4 text-center text-muted">
                                                    <i class="fa-solid fa-folder-open fa-2x mb-2 d-block opacity-25"></i>
                                                    Sin movimientos registrados en el historial.
                                                </td>
                                            </tr>
                                        </asp:Panel>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
                
                <div class="modal-footer justify-content-center border-0 pt-0 pb-4">
                     <button type="button" class="btn btn-outline-secondary btn-pill px-5" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>

    <script type="text/javascript">

        Sys.Application.add_load(function () {

            initTable('#tablaEjecucion');
            initTable('#tablaMiembros');

            if (typeof UTC_FileInput === 'function') {
                initInput("wrapperArchivoAdd", "<%= flpArchivoAdd.ClientID %>");
                initInput("wrapperArchivoEdit", "<%= flpArchivoEdit.ClientID %>");
                initInput("wrapperArchivoInf", "<%= flpArchivoInf.ClientID %>");
            }
        });

        function initTable(id) {
            if ($.fn.DataTable && $.fn.DataTable.isDataTable(id)) {
                $(id).DataTable().destroy();
            }
            if ($(id).length) {
                $(id).DataTable({
                    responsive: true,
                    autoWidth: false,
                    ordering: true,
                    pageLength: 10,
                    language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                    dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row mb-3'<'col-sm-12 text-center'B>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
                    buttons: [
                        { extend: 'excelHtml5', className: 'btn btn-success btn-sm rounded-pill mx-1', text: '<i class="fa-solid fa-file-excel"></i> Excel' },
                        { extend: 'pdfHtml5', className: 'btn btn-danger btn-sm rounded-pill mx-1', text: '<i class="fa-solid fa-file-pdf"></i> PDF', orientation: 'landscape' },
                        { extend: 'print', className: 'btn btn-secondary btn-sm rounded-pill mx-1', text: '<i class="fa-solid fa-print"></i>' }
                    ]
                });
            }
        }

        function initInput(wrapperId, inputId) {
            if (document.getElementById(wrapperId)) {
                UTC_FileInput({
                    wrapper: wrapperId,
                    dropzone: wrapperId.replace("wrapper", "dropzone"),
                    preview: wrapperId.replace("wrapper", "preview"),
                    loader: wrapperId.replace("wrapper", "loader"),
                    input: inputId
                });
            }
        }

        function AbrirModalInformes() {
            var el = document.getElementById('modalInformes');
            var modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
        }

        function AbrirSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            var modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
        }

        function CerrarSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
        }

        function LimpiarYSubir() {
            document.getElementById('<%= hfIdInformeEdit.ClientID %>').value = "";
            document.getElementById('<%= lblTituloModalInforme.ClientID %>').innerText = "Subir Informe";
            document.getElementById('<%= txtNombrePeriodoInf.ClientID %>').value = "";
            AbrirSubModalUpload();
        }

        function guardarMotivoJS() {
            var txt = document.getElementById('txtMotivoCambio');
            var hf = document.getElementById('hfMotivoHidden');
            if (txt.value.trim() === "") {
                alert("Debe ingresar un motivo.");
                return false;
            }
            hf.value = txt.value;
            return true;
        }

        function GestionarArchivoDirecto(url) {
            if (!url || url.trim() === "") {
                alert("No se encuentra el archivo físico.");
                return;
            }

            var extension = url.split('.').pop().toLowerCase();

            if (extension === 'pdf') {
                var frame = document.getElementById('framePdf');
                frame.src = url;
                frame.style.display = 'block';

                document.getElementById('lblTituloPreview').innerText = "Visualización de Documento";

                var btnDL = document.getElementById('btnDescargarDirecto');
                btnDL.href = url;
                btnDL.setAttribute('download', url.split('/').pop());
                var btnPrint = document.getElementById('btnImprimirReporte');
                if (btnPrint) btnPrint.style.display = 'none';

                var pnlHtml = document.getElementById('<%= pnlReporteHtml.ClientID %>');
                if (pnlHtml) pnlHtml.style.display = 'none';

                new bootstrap.Modal(document.getElementById('modalVistaPrevia')).show();

            } else {
                var link = document.createElement('a');
                link.href = url;
                link.download = url.split('/').pop(); 
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }

        function CerrarVistaPrevia() {
            var el = document.getElementById('modalVistaPrevia');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();

            document.getElementById('framePdf').src = '';
        }

        function validarPesoArchivo() {

            var inputId = '<%= flpArchivoInf.ClientID %>';
            var input = document.getElementById(inputId);

            if (!input) {
                console.error("ERROR: No se encontró el input con ID: " + inputId);
                return true;
            }

            if (input.files && input.files[0]) {
                var archivo = input.files[0];
                var pesoBytes = archivo.size;
                var pesoMB = (pesoBytes / (1024 * 1024)).toFixed(2);
                var limiteBytes = 8 * 1024 * 1024;

                console.log("Archivo seleccionado: " + archivo.name);
                console.log("Peso actual: " + pesoMB + " MB");

                if (pesoBytes > limiteBytes) {
                    console.warn("VALIDACIÓN FALLIDA: Excede los 8MB");

                    try {
                        toastify('error', 'El archivo pesa ' + pesoMB + ' MB. El límite es 8 MB.', 'Error de Peso');
                    } catch (e) {
                        console.error("Toastify falló, usando alert nativo. Error: " + e);
                        alert('El archivo pesa ' + pesoMB + ' MB. El límite es 8 MB.');
                    }

                    input.value = "";
                    var preview = document.getElementById('previewArchivoInf');
                    if (preview) preview.innerHTML = "";

                    var dropzone = document.getElementById('dropzoneArchivoInf');
                    if (dropzone) dropzone.style.display = 'block';

                    return false;
                }
            } else {
                console.log("No se ha seleccionado ningún archivo.");
            }

            console.log("Validación exitosa o sin archivo. Continuando...");
            return true;
        }

    </script>

    <script>
        function imprimirReporteJS() {
            var contenido = document.getElementById("arealmpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');
            ventana.document.write('<html><head><title>Reporte de Historial</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');
            ventana.document.write('<style>');
            ventana.document.write('body { font-family: "Segoe UI", sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; margin: 0; }');
            ventana.document.write('.report-paper { padding: 40px 50px; background: white; }');
            ventana.document.write('.header-hero-banner { background-color: #003876 !important; color: white !important; margin: -40px -50px 40px -50px; padding: 50px 20px 30px 20px; display: flex !important; justify-content: center !important; align-items: center !important; border-bottom: 6px solid #002a5c; }');
            ventana.document.write('.header-hero-banner img { height: 80px; width: auto; filter: brightness(0) invert(1); display: block; }');

            ventana.document.write('.header-info-split { display: flex; justify-content: space-between; border-bottom: 2px solid #003876; margin-bottom: 40px; padding-bottom: 25px; }');
            ventana.document.write('.doc-title { color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase; }');
            ventana.document.write('.system-label { font-size: 0.7rem; font-weight: 700; color: #999; text-transform: uppercase; letter-spacing: 2px; display: block; margin-bottom: 5px; }');

            ventana.document.write('.meta-group { margin-bottom: 10px; text-align: right; }');
            ventana.document.write('.meta-label { font-size: 0.65rem; text-transform: uppercase; color: #aaa; font-weight: 700; display: block; }');
            ventana.document.write('.meta-value { font-size: 1rem; font-weight: 700; color: #333; display: block; }');
            ventana.document.write('.ref-highlight { color: #dc3545; font-family: Consolas, monospace; font-size: 1.1rem; }');

            ventana.document.write('.researcher-card { background-color: #f8faff; border-left: 4px solid #003876; padding: 20px; margin-bottom: 40px; border: 1px solid #e1e8f0; border-radius: 6px; }');
            ventana.document.write('.card-row { display: flex; justify-content: space-between; margin-bottom: 15px; }');
            ventana.document.write('.card-item { flex: 1; padding-right: 10px; }'); // Añadido para asegurar distribución
            ventana.document.write('.card-item .label { font-size: 0.7rem; color: #8898aa; font-weight: 700; display: block; text-transform: uppercase; }');
            ventana.document.write('.card-item .value { font-size: 1rem; font-weight: 600; color: #002a5c; }');

            ventana.document.write('.timeline-container { padding: 0 10px; }');
            ventana.document.write('.timeline-title { font-size: 0.9rem; text-transform: uppercase; font-weight: 700; color: #999; border-bottom: 1px solid #eee; padding-bottom: 10px; margin-bottom: 25px; }');
            ventana.document.write('.timeline-list { list-style: none; padding: 0; position: relative; }');
            ventana.document.write('.timeline-list::before { content: ""; position: absolute; top: 0; bottom: 0; left: 24px; width: 2px; background: #e9ecef; }');
            ventana.document.write('.timeline-item { position: relative; padding-left: 60px; margin-bottom: 30px; page-break-inside: avoid; }');
            ventana.document.write('.timeline-marker { position: absolute; left: 18px; top: 0; width: 14px; height: 14px; border-radius: 50%; background: #fff; border: 3px solid #003876; z-index: 2; }');
            ventana.document.write('.timeline-header { margin-bottom: 6px; display: flex; align-items: baseline; gap: 10px; }');
            ventana.document.write('.timeline-header .date { font-weight: 700; color: #333; font-size: 0.9rem; }');

            ventana.document.write('.action-badge { display: inline-block; padding: 4px 10px; border-radius: 4px; font-size: 0.7rem; font-weight: 800; text-transform: uppercase; margin-bottom: 6px; }');
            ventana.document.write('.action-badge.good { background: rgba(25, 135, 84, 0.1); color: #198754; }');
            ventana.document.write('.action-badge.bad { background: rgba(220, 53, 69, 0.1); color: #dc3545; }');

            ventana.document.write('.report-legal-footer { margin-top: 60px; border-top: 1px solid #eee; text-align: center; font-size: 0.65rem; color: #ccc; padding-top: 20px; text-transform: uppercase; }');
            ventana.document.write('</style>');

            ventana.document.write('</head><body>');
            ventana.document.write(contenido);
            ventana.document.write('</body></html>');

            ventana.document.close();
            ventana.focus();

            setTimeout(function () {
                ventana.print();
                ventana.close();
            }, 500);
        }
    </script>

</asp:Content>