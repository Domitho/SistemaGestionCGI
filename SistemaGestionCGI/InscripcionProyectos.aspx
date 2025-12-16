<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InscripcionProyectos.aspx.cs" Inherits="SistemaGestionCGI.InscripcionProyectos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />

    <style>
        .modal-header.bg-utc {
            background: linear-gradient(90deg, var(--utc-azul) 0%, var(--utc-azul-oscuro) 100%) !important;
            color: #fff !important;
            border-top-left-radius: 10px !important;
            border-top-right-radius: 10px !important;
        }
        .modal-header.bg-utc .modal-title {
            color: #fff !important;
            font-weight: 600 !important;
        }
    </style>

    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-clipboard-list me-2"></i> INSCRIPCIÓN DE PROYECTOS
        </h3>

        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevo"
                CssClass="btn btn-primary btn-pill d-flex align-items-center"
                OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO PROYECTO
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresar"
                CssClass="btn btn-outline-primary btn-pill px-4"
                Visible="false" CausesValidation="false"
                OnClick="btnRegresar_Click">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaProyectos" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>TEMA</th>
                        <th>COORDINADOR</th>
                        <th>DURACIÓN</th>
                        <th>FECHA REGISTRO</th>
                        <th>GRUPO</th>
                        <th>CALIFICACIÓN</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptProyectos" runat="server" OnItemCommand="rptProyectos_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_pro") %></td>
                                <td class="text-start"><%# Eval("strTema_pro") %></td>
                                <td class="text-start"><%# Eval("strCoordinador_pro") %></td>
                                <td><%# Eval("strDuracion_pro") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFehains_pro")).ToString("dd/MM/yyyy") %></td>
                                <td class="text-start"><%# Eval("strNombre_gru") %></td>
                                <td class="text-center align-middle">
                                    <%# Eval("intPuntaje_pro") == null || Eval("intPuntaje_pro") == DBNull.Value
                                        ? 
                                        "<span class='badge rounded-pill bg-secondary bg-opacity-25 text-secondary border border-secondary fw-normal'>" +
                                            "<i class='fa-solid fa-hourglass-start me-1'></i> Por Calificar" +
                                        "</span>" 
                                        : 
                                        "<span class='fw-bold fs-5 text-dark'>" + Eval("intPuntaje_pro") + " <small class='text-muted fs-6'>pts</small></span>" 
                                    %>
                                </td>
                                <td>
                                    <span class='<%# Eval("strEstado_pro").ToString() == "Pendiente" 
                                                    ? "badge bg-warning" 
                                                    : "badge bg-success" %>'>
                                            <%# Eval("strEstado_pro") %>
                                    </span>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnVer" runat="server"
                                        CommandName="ver" CommandArgument='<%# Eval("strArchivo_pro") %>'
                                        CssClass="btn btn-ver btn-sm rounded-circle me-1"
                                        ToolTip="Ver archivo">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEstado" runat="server"
                                        CommandName="estado" CommandArgument='<%# Eval("strId_pro") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1"
                                        ToolTip="Cambiar estado">
                                        <i class="fa-solid fa-arrows-rotate"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server"
                                        CommandName="editar" CommandArgument='<%# Eval("strId_pro") %>'
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1"
                                        ToolTip="Editar proyecto">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server"
                                        CommandName="eliminar" CommandArgument='<%# Eval("strId_pro") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle"
                                        OnClientClick="return confirm('¿Desea eliminar este proyecto?');"
                                        ToolTip="Eliminar">
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
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Proyecto
            </h4>
            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Grupo de Investigación</label>
                    <asp:DropDownList ID="ddlGrupo" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlGrupo_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

                <asp:Panel ID="pnlInfoGrupo" runat="server" Visible="false" CssClass="col-12 animate__animated animate__fadeIn">
                    <div class="alert alert-primary shadow-sm border-0 d-flex align-items-center" role="alert">
                        <div class="me-3 display-6">
                            <i class="fa-solid fa-users-viewfinder"></i>
                        </div>
                        <div>
                            <h6 class="alert-heading fw-bold mb-1"><asp:Label ID="lblNombreGrupoInfo" runat="server"></asp:Label></h6>
                            <p class="mb-0 small opacity-75">
                                <i class="fa-solid fa-list-check me-1"></i> Líneas: <asp:Label ID="lblLineasInfo" runat="server"></asp:Label>
                            </p>
                            <hr class="my-2 opacity-25">
                            <p class="mb-0 small">
                                <i class="fa-solid fa-circle-info me-1"></i> Seleccione un integrante de la lista inferior. 
                                Si es un <strong>colaborador externo</strong>, regístrelo con el botón (+).
                            </p>
                        </div>
                    </div>
                </asp:Panel>

                <div class="col-12">
                    <label class="form-label">Coordinador del Proyecto</label>
    
                    <div class="d-flex gap-2">
                        <asp:DropDownList ID="ddlCoordinador" runat="server" CssClass="form-select w-100">
                            <asp:ListItem Text="-- Seleccione Grupo Primero --" Value="" />
                        </asp:DropDownList>
        
                        <button type="button" class="btn btn-outline-primary text-nowrap" onclick="AbrirModalNuevoIntegrante()">
                            <i class="fa-solid fa-plus"></i> Nuevo
                        </button>
                    </div>
                    <div class="form-text small text-muted">Si el coordinador no aparece en la lista, agréguelo aquí.</div>
                </div>

                <div class="col-12">
                    <label class="form-label">Titulo del Proyecto</label>
                    <asp:TextBox ID="txtTema" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Duración</label>
                    <asp:TextBox ID="txtDuracion" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Puntuación Obtenida (Opcional)</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtPuntaje" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 95" />
                    </div>
                    <div class="form-text small">Dejar vacío si aún no ha sido calificado.</div>
                </div>
                <div class="col-12">
                    <label class="form-label">Fecha de Inicio</label>
                    <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-12">
                    <label class="form-label">Convocatoria</label>
                    <asp:DropDownList ID="ddlConv" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label fw-semibold">Archivo de convocatoria</label>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivo">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-paperclip"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Ningún archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i> Renombrar</button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Nuevo nombre..." />
                        <div class="utc-fileinput-preview" id="previewArchivo"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivo"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando archivo…</div>
                        <div class="utc-dropzone" id="dropzoneArchivo">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />
                            Arrastra un archivo o haz clic aquí.
                        </div>
                        <asp:FileUpload ID="flpArchivo" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                    <div class="form-text">Formatos permitidos: PDF, XLS, XLSX (máx 8MB)</div>
                </div>
            </div>
            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEdicion" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-pen-to-square me-2"></i> Editar Proyecto
            </h4>
            <asp:HiddenField ID="hfIdEdit" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />
            
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Grupo</label>
                    <asp:DropDownList ID="ddlGrupoEdit" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlGrupoEdit_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Coordinador</label>
                    <asp:DropDownList ID="ddlCoordinadorEdit" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione Grupo Primero --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Titulo del Proyecto</label>
                    <asp:TextBox ID="txtTemaEdit" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Duración</label>
                    <asp:TextBox ID="txtDuracionEdit" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-bold text-primary">Puntaje Asignado</label>
                    <asp:TextBox ID="txtPuntajeEdit" runat="server" CssClass="form-control" TextMode="Number" />
                </div>
                <div class="col-12">
                    <label class="form-label">Fecha de Inicio</label>
                    <asp:TextBox ID="txtFechaEdit" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
                <div class="col-12">
                    <label class="form-label">Convocatoria</label>
                    <asp:DropDownList ID="ddlConvEdit" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label fw-bold">Archivo Actual Convocatoria</label>
                    <asp:Label ID="lblArchivoActual" runat="server" CssClass="d-block mb-2 text-primary fw-semibold"></asp:Label>
                </div>
                <div class="col-12">
                    <label class="form-label fw-semibold">Reemplazar Archivo Convocatoria (opcional)</label>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoEdit">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-paperclip"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Ningún archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i> Renombrar</button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Nuevo nombre..." />
                        <div class="utc-fileinput-preview" id="previewArchivoEdit"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoEdit"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando archivo…</div>
                        <div class="utc-dropzone" id="dropzoneArchivoEdit">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />
                            Arrastra un archivo o haz clic aquí.
                        </div>
                        <asp:FileUpload ID="flpArchivoEdit" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                    <div class="form-text">Formatos permitidos: PDF, XLS, XLSX (máx 8MB)</div>
                </div>
            </div>
            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnActualizar" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnActualizar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Actualizar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarEdit" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarEdit_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalEstadoPro" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100">
                        <i class="fa-solid fa-power-off me-2"></i>
                        <span id="tituloEstadoPro">Cambio de Estado del Proyecto</span>
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p class="mb-3">
                        ¿Estás seguro que deseas <strong id="accionEstadoTextoPro">cambiar</strong> el estado del proyecto?
                    </p>
                    <div class="bg-light p-3 rounded border">
                        <asp:HiddenField ID="hfIdProyectoEstado" runat="server" ClientIDMode="Static" />
                        <p class="mb-1"><strong>ID Proyecto:</strong> <span id="infoIdPro"></span></p>
                        <p class="mb-1"><strong>Tema:</strong> <span id="infoTemaPro"></span></p>
                        <p class="mb-1"><strong>Estado actual:</strong> <span id="infoEstadoPro" class="badge bg-warning text-dark px-2 py-1"></span></p>
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnConfirmarEstadoPro" runat="server" CssClass="btn btn-pill btn-success px-4" OnClick="btnConfirmarEstadoPro_Click">
                        <i class="fa-solid fa-check me-2"></i> Confirmar
                    </asp:LinkButton>
                    <button type="button" class="btn btn-outline-primary btn-pill px-4" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i> Cancelar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalNuevoIntegrante" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content shadow-utc border-0 rounded-4">
            
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title"><i class="fa-solid fa-user-plus me-2"></i> Registrar Nuevo Integrante</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body p-4">
                    <div class="alert alert-light border-start border-primary border-4 shadow-sm small text-muted mb-4">
                        <i class="fa-solid fa-circle-info text-primary me-2"></i> 
                        Se vinculará al grupo: <strong class="text-dark" id="lblGrupoModalJS">...</strong>
                    </div>

                    <div class="row g-3">
                        <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>

                        <div class="col-md-4">
                            <label class="form-label">Cédula <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" placeholder="Ej: 050..." MaxLength="15"/>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Nombres <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Apellidos <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Correo Electrónico</label>
                            <asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">Tipo de Integrante</label>
                            <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select" onchange="ToggleTipoIntegrante(this)">
                                <asp:ListItem Text="Interno (UTC)" Value="Interno" Selected="True" />
                                <asp:ListItem Text="Externo (Colaborador)" Value="Externo" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-12 mt-4"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Académicos / Función</h6></div>

                        <div id="divInterno" class="col-12 row g-3 m-0 p-0" runat="server" ClientIDMode="Static">
                            <div class="col-md-6">
                                <label class="form-label">Carrera / Departamento</label>
                                <asp:TextBox ID="txtCarreraInt" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Facultad / Extensión</label>
                                <asp:DropDownList ID="ddlFacultadInt" runat="server" CssClass="form-select">
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
                        </div>

                        <div id="divExterno" class="col-12" style="display:none;" runat="server" ClientIDMode="Static">
                            <label class="form-label">Institución / Entidad de Origen <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control" placeholder="Ej: Universidad Central, Empresa Eléctrica..." />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">Función en el Grupo</label>
                            <asp:TextBox ID="txtFuncionInt" runat="server" CssClass="form-control" placeholder="Ej: Investigador Principal..." />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Observación</label>
                            <asp:TextBox ID="txtObservacionInt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                        </div>
                    </div>
                </div>

                <div class="modal-footer border-0 bg-light justify-content-center">
                    <asp:LinkButton ID="btnGuardarIntegrante" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardarIntegrante_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Integrante
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script>
        function AbrirModalNuevoIntegrante() {
            // Obtenemos el texto del grupo seleccionado para mostrarlo en el modal
            var grupoText = $("#<%= ddlGrupo.ClientID %> option:selected").text();
            if (grupoText == "" || grupoText.includes("--")) {
                alert("Primero seleccione un Grupo de Investigación.");
                return;
            }
            document.getElementById('lblGrupoModalJS').innerText = grupoText;

            var el = document.getElementById('modalNuevoIntegrante');
            var modal = new bootstrap.Modal(el);
            modal.show();

            if (typeof ResetFormularioIntegrante === "function") {
                ResetFormularioIntegrante();
            }

        }
    </script>

    <script type="text/javascript">
        Sys.Application.add_load(function () {
            const tabla = '#tablaProyectos';
            if ($.fn.DataTable && $.fn.DataTable.isDataTable(tabla)) {
                $(tabla).DataTable().destroy();
            }
            $(tabla).DataTable({
                responsive: true,
                autoWidth: false,
                ordering: true,
                order: [],
                pageLength: 10,
                language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row mb-3'<'col-sm-12 text-center'B>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
                buttons: [
                    { extend: 'excelHtml5', text: '<i class="fa-solid fa-file-excel"></i> Excel', className: 'btn btn-success btn-sm rounded-pill mx-1' },
                    { extend: 'pdfHtml5', text: '<i class="fa-solid fa-file-pdf"></i> PDF', className: 'btn btn-danger btn-sm rounded-pill mx-1', orientation: 'landscape', pageSize: 'A4' },
                    { extend: 'print', text: '<i class="fa-solid fa-print"></i> Imprimir', className: 'btn btn-secondary btn-sm rounded-pill mx-1' }
                ],
                columnDefs: [ { targets: -1, orderable: false, searchable: false } ]
            });
        });
    </script>

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>

    <script>
        UTC_FileInput({
            wrapper: "wrapperArchivo", dropzone: "dropzoneArchivo", preview: "previewArchivo", loader: "loaderArchivo",
            input: "<%= flpArchivo.ClientID %>", pdfjsLibUrl: "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/4.0.379/pdf.min.js"
        });
        UTC_FileInput({
            wrapper: "wrapperArchivoEdit", dropzone: "dropzoneArchivoEdit", preview: "previewArchivoEdit", loader: "loaderArchivoEdit",
            input: "<%= flpArchivoEdit.ClientID %>", pdfjsLibUrl: "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/4.0.379/pdf.min.js"
        });
    </script>

    <script>
        function AbrirModalEstadoPro() {
            var el = document.getElementById('modalEstadoPro');
            var modal = bootstrap.Modal.getInstance(el);
            if (!modal) {
                modal = new bootstrap.Modal(el);
            }
            modal.show();
        }
    </script>

    <script>
        function ToggleTipoIntegrante(el) {
            // 'el' es el DropDownList que disparó el evento. No necesitamos buscarlo por ID.
            var tipo = el.value;

            // Como usamos ClientIDMode="Static", podemos usar los IDs directos
            var divInterno = document.getElementById('divInterno');
            var divExterno = document.getElementById('divExterno');

            if (tipo === "Externo") {
                divInterno.style.display = 'none';
                divExterno.style.display = 'block';
            } else {
                divInterno.style.display = 'flex'; // 'flex' porque es un row
                divExterno.style.display = 'none';
            }
        }

        // Esta función extra asegura que al abrir el modal (si hubo error) se vea bien
        function ResetFormularioIntegrante() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            if (ddl) {
                // Forzamos la ejecución para acomodar los divs según lo que esté seleccionado
                ToggleTipoIntegrante(ddl);
            }
        }
    </script>

</asp:Content>