<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InscripcionProyectos.aspx.cs" Inherits="SistemaGestionCGI.InscripcionProyectos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- ESTILOS Y RECURSOS --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />

    <style>
        /* Estilos centralizados para Modals UTC */
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

        /* Asegura que los formularios no desborden en móviles */
        .form-stack {
            max-width: 100% !important;
        }
    </style>

    <%-- HEADER PRINCIPAL --%>
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

    <%-- PANEL 1: TABLA DE PROYECTOS (GRILLA) --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaProyectos" class="table table-bordered table-hover table-utc align-middle text-center" style="width: 100%">
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
                                <td class="text-start text-uppercase small"><%# Eval("NombreCoordinadorCompleto") %></td>
                                <td><%# Eval("strDuracion_pro") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFehains_pro")).ToString("dd/MM/yyyy") %></td>
                                <td class="text-start"><%# Eval("strNombre_gru") %></td>
                                <td class="text-center align-middle">
                                    <%# Eval("intPuntaje_pro") == null || Eval("intPuntaje_pro") == DBNull.Value
                                        ? "<span class='badge rounded-pill bg-secondary bg-opacity-25 text-secondary border border-secondary fw-normal'><i class='fa-solid fa-hourglass-start me-1'></i> Por Calificar</span>" 
                                        : "<span class='fw-bold fs-5 text-dark'>" + Eval("intPuntaje_pro") + " <small class='text-muted fs-6'>pts</small></span>" 
                                    %>
                                </td>
                                <td>
                                    <span class='<%# 
                                        Eval("strEstado_pro").ToString() == "Aprobado" ? "badge bg-success" : 
                                        Eval("strEstado_pro").ToString() == "Rechazado" ? "badge bg-danger" : 
                                        "badge bg-warning text-dark" %>'>
                                        <%# Eval("strEstado_pro") %>
                                    </span>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnVer" runat="server" CommandName="ver" CommandArgument='<%# Eval("strArchivo_pro") %>'
                                        CssClass="btn btn-ver btn-sm rounded-circle me-1" ToolTip="Ver archivo">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnCambiarEstado" runat="server"
                                        CommandName="CambiarEstado" 
                                        CommandArgument='<%# Eval("strId_pro") %>'
    
                                        Enabled='<%# Eval("strEstado_pro").ToString() != "Aprobado" %>'
    
                                        CssClass='<%# Eval("strEstado_pro").ToString() == "Aprobado" ? 
                                                     "btn btn-secondary btn-sm rounded-circle me-1" : 
                                                     "btn btn-warning btn-sm rounded-circle me-1" %>'
    
                                        ToolTip='<%# Eval("strEstado_pro").ToString() == "Aprobado" ? 
                                                     "Proyecto Aprobado" : 
                                                     "Gestionar Estado" %>'>
    
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="editar" CommandArgument='<%# Eval("strId_pro") %>'
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1" ToolTip="Editar proyecto">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="eliminar" CommandArgument='<%# Eval("strId_pro") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Desea eliminar este proyecto?');" ToolTip="Eliminar">
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

    <%-- PANEL 2: FORMULARIO DE REGISTRO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Proyecto
            </h4>

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Grupo de Investigación</label>
                    <asp:DropDownList ID="ddlGrupo" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlGrupo_SelectedIndexChanged"></asp:DropDownList>
                </div>

                <asp:Panel ID="pnlInfoGrupo" runat="server" Visible="false" CssClass="col-12 animate__animated animate__fadeIn">
                    <div class="alert alert-primary shadow-sm border-0 d-flex align-items-center" role="alert">
                        <div class="me-3 display-6"><i class="fa-solid fa-users-viewfinder"></i></div>
                        <div>
                            <h6 class="alert-heading fw-bold mb-1"><asp:Label ID="lblNombreGrupoInfo" runat="server"></asp:Label></h6>
                            <p class="mb-0 small opacity-75">
                                <i class="fa-solid fa-list-check me-1"></i> Líneas: <asp:Label ID="lblLineasInfo" runat="server"></asp:Label>
                            </p>
                            <hr class="my-2 opacity-25">
                            <p class="mb-0 small">
                                <i class="fa-solid fa-circle-info me-1"></i> Seleccione un integrante. Si es <strong>externo</strong>, regístrelo con (+).
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
                    <div class="form-text small text-muted">Si el coordinador no aparece, agréguelo aquí.</div>
                </div>

                <div class="col-12">
                    <label class="form-label">Titulo del Proyecto</label>
                    <asp:TextBox ID="txtTema" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold">Duración Estimada</label>
    
                    <div class="input-group">
       
                        <asp:TextBox ID="txtDuracionDisplay" runat="server" 
                            CssClass="form-control bg-white text-primary fw-bold border-start-0 border-end-0" 
                            placeholder="Seleccione..." ReadOnly="true" ClientIDMode="Static" />
        
                        <button type="button" class="btn btn-outline-primary" onclick="AbrirModalDuracion(false)">
                            <i class="fa-solid fa-stopwatch me-2"></i> Definir Tiempo
                        </button>
                    </div>

                    <asp:HiddenField ID="hfAnios" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfMeses" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfSemanas" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfDias" runat="server" ClientIDMode="Static" Value="0" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Puntuación (Opcional)</label>
                    <asp:TextBox ID="txtPuntaje" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 95" />
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

                <%-- FILE INPUT --%>
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
                        <div class="utc-fileinput-loader" id="loaderArchivo"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivo"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />Arrastra un archivo aquí.</div>
                        <asp:FileUpload ID="flpArchivo" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                    <div class="form-text">Formatos permitidos: PDF, XLS, XLSX (máx 8MB)</div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnGuardar" runat="server" 
                    CssClass="btn btn-primary btn-pill px-4" 
                    OnClientClick="return ValidarPuntajeProyecto(false);"
                    OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL 3: EDICIÓN DE PROYECTO --%>
    <asp:Panel ID="pnlEdicion" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-pen-to-square me-2"></i> Editar Proyecto
            </h4>
            <asp:HiddenField ID="hfIdEdit" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />

            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Grupo</label>
                    <asp:DropDownList ID="ddlGrupoEdit" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlGrupoEdit_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <label class="form-label">Coordinador</label>
                    <asp:DropDownList ID="ddlCoordinadorEdit" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione Grupo Primero --" Value="" />
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Titulo del Proyecto</label>
                    <asp:TextBox ID="txtTemaEdit" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold">Duración Estimada</label>
    
                    <div class="input-group">
                        <asp:TextBox ID="txtDuracionDisplayEdit" runat="server" 
                            CssClass="form-control bg-white text-primary fw-bold border-start-0 border-end-0" 
                            placeholder="Seleccione..." ReadOnly="true" ClientIDMode="Static" />
        
                        <button type="button" class="btn btn-outline-primary" onclick="AbrirModalDuracion(true)">
                            <i class="fa-solid fa-stopwatch me-2"></i> Definir Tiempo
                        </button>
                    </div>

                    <asp:HiddenField ID="hfAniosEdit" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfMesesEdit" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfSemanasEdit" runat="server" ClientIDMode="Static" Value="0" />
                    <asp:HiddenField ID="hfDiasEdit" runat="server" ClientIDMode="Static" Value="0" />
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
                    <label class="form-label fw-bold">Archivo Actual</label>
                    <asp:Label ID="lblArchivoActual" runat="server" CssClass="d-block mb-2 text-primary fw-semibold"></asp:Label>
                </div>
                
                <%-- FILE INPUT EDICION --%>
                <div class="col-12">
                    <label class="form-label fw-semibold">Reemplazar Archivo (opcional)</label>
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
                        <div class="utc-fileinput-loader" id="loaderArchivoEdit"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivoEdit"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2"></i><br />Arrastra un archivo aquí.</div>
                        <asp:FileUpload ID="flpArchivoEdit" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                    <div class="form-text">Formatos permitidos: PDF, XLS, XLSX (máx 8MB)</div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnActualizar" runat="server" 
                    CssClass="btn btn-primary btn-pill px-4" 
                    OnClientClick="return ValidarPuntajeProyecto(true);" 
                    OnClick="btnActualizar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Actualizar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarEdit" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarEdit_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL: ESTADO --%>
    <div class="modal fade" id="modalEstadoPro" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-power-off me-2"></i> Gestionar Estado</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="bg-light p-3 rounded border mb-3">
                        <asp:HiddenField ID="hfldProyectoEstado" runat="server" ClientIDMode="Static" />
                        <p class="mb-1"><strong>ID:</strong> <span id="infoldPro"></span></p>
                        <p class="mb-1"><strong>Tema:</strong> <span id="infoTemaPro" class="text-primary fw-bold"></span></p>
                        <p class="mb-0"><strong>Estado Actual:</strong> <span id="infoEstadoPro"></span></p>
                    </div>

                    <div class="mb-3">
                        <label class="form-label fw-bold">Seleccione el nuevo estado:</label>
                        <asp:DropDownList ID="ddlNuevoEstado" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="-- Seleccione una Acción --" Selected="True" />
                            <asp:ListItem Value="Aprobado" Text="✅ APROBAR PROYECTO" class="text-success fw-bold" />
                            <asp:ListItem Value="Rechazado" Text="❌ RECHAZAR PROYECTO" class="text-danger fw-bold" />
                        </asp:DropDownList>
                    </div>
                
                    <div class="mb-1">
                         <label class="form-label small">Observación (Opcional)</label>
                         <asp:TextBox ID="txtObservacionEstado" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Motivo del cambio..."></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnConfirmarEstadoPro" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnConfirmarEstadoPro_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Cambio
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <%-- MODAL: NUEVO INTEGRANTE --%>
    <div class="modal fade" id="modalNuevoIntegrante" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content border-0 shadow-utc rounded-4">
            
                <div class="modal-header bg-utc text-white py-3">
                    <h5 class="modal-title fw-bold"><i class="fa-solid fa-user-plus me-2"></i> Nuevo Integrante</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-white p-4">
                
                    <div class="row align-items-center mb-3">
                        <div class="col-md-4">
                            <label class="form-label fw-bold text-dark mb-0">Tipo de Vinculación:</label>
                        </div>
                        <div class="col-md-8">
                            <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select border-secondary shadow-none"
                                onchange="ToggleTipoIntegranteModal()">
                                <asp:ListItem Value="Interno" Selected="True">INTERNO (Estudiante / Administrativo)</asp:ListItem>
                                <asp:ListItem Value="Docente">DOCENTE UTC (Búsqueda Automática)</asp:ListItem>
                                <asp:ListItem Value="Externo">EXTERNO (Otra Institución)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <hr class="text-muted opacity-25 my-4">

                    <div id="pnlBusquedaDocente" style="display:none;" class="mb-4">
                        <div class="bg-light p-3 rounded-3 border border-dashed">
                            <label class="form-label fw-bold text-primary small mb-2">
                                <i class="fa-solid fa-magnifying-glass me-1"></i> BÚSQUEDA INSTITUCIONAL
                            </label>
                            <div class="input-group">
                                <asp:TextBox ID="txtBuscarCedula" runat="server" CssClass="form-control" 
                                    placeholder="Ingrese cédula del docente..." MaxLength="10"></asp:TextBox>
                                <asp:LinkButton ID="btnBuscarDocente" runat="server" CssClass="btn btn-primary px-4" 
                                    OnClick="btnBuscarDocente_Click">
                                    <i class="fa-solid fa-search"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                        <hr class="text-muted opacity-25 my-4">
                    </div>

                    <div id="pnlDatosPersonales">
                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-regular fa-id-card me-2"></i> Información Personal
                        </h6>
                    
                        <div class="row g-3 mb-4">
                            <div class="col-md-4">
                                <label class="form-label small text-muted">Cédula <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control form-control-sm bg-light" MaxLength="15" autocomplete="off"/>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label small text-muted">Nombres <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control form-control-sm" autocomplete="off"/>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label small text-muted">Apellidos <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control form-control-sm" autocomplete="off"/>
                            </div>
                            <div class="col-12">
                                <label class="form-label small text-muted">Correo Electrónico <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control form-control-sm" TextMode="Email" autocomplete="off"/>
                            </div>
                        </div>

                        <hr class="text-muted opacity-25 my-4">

                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-solid fa-building-columns me-2"></i> Afiliación y Rol
                        </h6>

                        <div id="divInternoModal" class="row g-3 mb-4">
                            <div class="col-md-6">
                                <label class="form-label small text-muted">Facultad/Extensión</label>
                                <asp:DropDownList ID="ddlFacultadInt" runat="server" CssClass="form-select form-select-sm">
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
                                <label class="form-label small text-muted">Carrera / Departamento</label>
                                <asp:TextBox ID="txtCarreraInt" runat="server" CssClass="form-control form-control-sm" autocomplete="off"/>
                            </div>
                        </div>

                        <div id="divExternoModal" class="row g-3 mb-4" style="display:none;">
                            <div class="col-12">
                                <label class="form-label small text-muted">Entidad de Origen</label>
                                <asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control form-control-sm" 
                                    placeholder="Ej: Universidad Central del Ecuador" autocomplete="off"/>
                            </div>
                        </div>

                        <div class="row g-3 mb-4">
                            <div class="col-12">
                                <label class="form-label small text-muted fw-bold">Función Asignada</label>
                                <div class="input-group">
                                    <span class="input-group-text bg-primary-subtle text-primary border-primary border-opacity-25">
                                        <i class="fa-solid fa-id-badge"></i>
                                    </span>
                                    <asp:TextBox ID="txtFuncionDisplay" runat="server" 
                                        CssClass="form-control bg-light text-primary fw-bold border-primary border-opacity-25" 
                                        ReadOnly="true" Text="Miembro Investigador"></asp:TextBox>
                                </div>
                                <div class="form-text small text-muted mt-1">
                                    <i class="fa-solid fa-circle-info me-1"></i> Rol definido automáticamente para proyectos.
                                </div>
                            </div>
                        </div>

                    </div> </div>

                <div class="modal-footer border-top-0 bg-white rounded-bottom-4 py-4 justify-content-center">
                    <asp:LinkButton ID="btnGuardarIntegrante" runat="server" 
                        CssClass="btn btn-primary btn-pill px-5 shadow fw-bold"
                        OnClientClick="return ValidarNuevoIntegrante();" 
                        OnClick="btnGuardarIntegrante_Click">
                        <i class="fa-solid fa-check me-2"></i> GUARDAR INTEGRANTE
                    </asp:LinkButton>
                </div>

            </div>
        </div>
    </div>

    <div class="modal fade" id="modalDuracion" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 rounded-4">
                <div class="modal-header bg-utc text-white border-0">
                    <h5 class="modal-title w-100 text-center"><i class="fa-solid fa-hourglass-half me-2"></i> Configurar Duración</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body bg-light">
                
                    <div class="text-center mb-4 pt-2">
                        <h6 class="text-muted small text-uppercase mb-1">Tiempo Total</h6>
                        <div class="h4 fw-bold text-primary" id="lblLivePreview">0 Meses</div>
                    </div>

                    <div class="row g-3">
                        <div class="col-6">
                            <div class="card border-0 shadow-sm h-100">
                                <div class="card-body text-center p-2">
                                    <small class="text-muted d-block mb-2"><i class="fa-solid fa-calendar me-1"></i> Años</small>
                                    <div class="d-flex justify-content-center align-items-center gap-2">
                                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-circle" onclick="Step('anios', -1)"><i class="fa-solid fa-minus"></i></button>
                                        <input type="number" id="tmpAnios" class="form-control text-center fw-bold border-0 p-0 bg-transparent" style="width: 40px" value="0" readonly>
                                        <button type="button" class="btn btn-sm btn-outline-primary rounded-circle" onclick="Step('anios', 1)"><i class="fa-solid fa-plus"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="card border-0 shadow-sm h-100">
                                <div class="card-body text-center p-2">
                                    <small class="text-muted d-block mb-2"><i class="fa-solid fa-calendar-days me-1"></i> Meses</small>
                                    <div class="d-flex justify-content-center align-items-center gap-2">
                                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-circle" onclick="Step('meses', -1)"><i class="fa-solid fa-minus"></i></button>
                                        <input type="number" id="tmpMeses" class="form-control text-center fw-bold border-0 p-0 bg-transparent" style="width: 40px" value="0" readonly>
                                        <button type="button" class="btn btn-sm btn-outline-primary rounded-circle" onclick="Step('meses', 1)"><i class="fa-solid fa-plus"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="card border-0 shadow-sm h-100">
                                <div class="card-body text-center p-2">
                                    <small class="text-muted d-block mb-2"><i class="fa-solid fa-calendar-week me-1"></i> Semanas</small>
                                    <div class="d-flex justify-content-center align-items-center gap-2">
                                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-circle" onclick="Step('semanas', -1)"><i class="fa-solid fa-minus"></i></button>
                                        <input type="number" id="tmpSemanas" class="form-control text-center fw-bold border-0 p-0 bg-transparent" style="width: 40px" value="0" readonly>
                                        <button type="button" class="btn btn-sm btn-outline-primary rounded-circle" onclick="Step('semanas', 1)"><i class="fa-solid fa-plus"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-6">
                            <div class="card border-0 shadow-sm h-100">
                                <div class="card-body text-center p-2">
                                    <small class="text-muted d-block mb-2"><i class="fa-solid fa-sun me-1"></i> Días</small>
                                    <div class="d-flex justify-content-center align-items-center gap-2">
                                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-circle" onclick="Step('dias', -1)"><i class="fa-solid fa-minus"></i></button>
                                        <input type="number" id="tmpDias" class="form-control text-center fw-bold border-0 p-0 bg-transparent" style="width: 40px" value="0" readonly>
                                        <button type="button" class="btn btn-sm btn-outline-primary rounded-circle" onclick="Step('dias', 1)"><i class="fa-solid fa-plus"></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer border-0 justify-content-center pb-4">
                    <button type="button" class="btn btn-primary btn-pill px-5" onclick="GuardarDuracion()">
                        <i class="fa-solid fa-check me-2"></i> Aplicar Tiempo
                    </button>
                </div>
            </div>
        </div>
    </div>

    <%-- SCRIPTS OPTIMIZADOS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script src="DesignersUTC/Scripts/utc-selector-tiempo.js"></script>

    <script type="text/javascript">

        const dtConfigProyectos = {
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
            columnDefs: [{ targets: -1, orderable: false, searchable: false }]
        };

        Sys.Application.add_load(function () {
            
            const tabla = '#tablaProyectos';
            if ($.fn.DataTable && $.fn.DataTable.isDataTable(tabla)) {
                $(tabla).DataTable().destroy();
            }
            if ($(tabla).length) {
                $(tabla).DataTable(dtConfigProyectos);
            }

            if (typeof UTC_FileInput === 'function') {
                if (document.getElementById('wrapperArchivo')) {
                    UTC_FileInput({
                        wrapper: "wrapperArchivo", dropzone: "dropzoneArchivo", preview: "previewArchivo", loader: "loaderArchivo",
                        input: "<%= flpArchivo.ClientID %>", pdfjsLibUrl: "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/4.0.379/pdf.min.js"
                    });
                }
                if (document.getElementById('wrapperArchivoEdit')) {
                    UTC_FileInput({
                        wrapper: "wrapperArchivoEdit", dropzone: "dropzoneArchivoEdit", preview: "previewArchivoEdit", loader: "loaderArchivoEdit",
                        input: "<%= flpArchivoEdit.ClientID %>", pdfjsLibUrl: "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/4.0.379/pdf.min.js"
                    });
                }
            }

            if (document.getElementById('wrapperCertificadoModal')) {
                UTC_FileInput({
                    wrapper: "wrapperCertificadoModal", dropzone: "dropzoneCertificadoModal",
                    preview: "previewCertificadoModal", loader: "loaderCertificadoModal",
                });
            }

        });

        function AbrirModalEstadoPro() {
            var el = document.getElementById('modalEstadoPro');
            var modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
        }

        function AbrirModalNuevoIntegrante() {
            var grupoSelect = document.getElementById('<%= ddlGrupo.ClientID %>');
            var grupoText = grupoSelect.options[grupoSelect.selectedIndex].text;

            if (grupoSelect.value === "" || grupoText.includes("--")) {
                alert("Primero seleccione un Grupo de Investigación.");
                return;
            }
            document.getElementById('lblGrupoModalJS').innerText = grupoText;

            var el = document.getElementById('modalNuevoIntegrante');
            var modal = new bootstrap.Modal(el);
            modal.show();

            ResetFormularioIntegrante();
        }

        function ToggleTipoIntegrante(source) {
            var tipo = source.value;
            var divInterno = document.getElementById('divInterno');
            var divExterno = document.getElementById('divExterno');

            if (tipo === "Externo") {
                divInterno.style.display = 'none';
                divExterno.style.display = 'block';
            } else {
                divInterno.style.display = 'flex';
                divExterno.style.display = 'none';
            }
        }

        function ResetFormularioIntegrante() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            if (ddl) {
                ToggleTipoIntegrante(ddl);
            }
        }

        function ToggleFuncionModal(el) {
            var val = el.value;
            var div = document.getElementById('divCertificadoModal');
            if (div) {
                if (val === 'Investigador Principal') {
                    div.style.display = 'block';
                } else {
                    div.style.display = 'none';
                }
            }
        }

        function ResetFormularioIntegrante() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            if (ddl) ToggleTipoIntegrante(ddl);
        }

        function ToggleTipoIntegranteModal() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            var tipo = ddl.value;

            var pnlBusqueda = document.getElementById('pnlBusquedaDocente');
            var pnlDatos = document.getElementById('pnlDatosPersonales');

            var divInterno = document.getElementById('divInternoModal');
            var divExterno = document.getElementById('divExternoModal');

            var txtNombre = document.getElementById('<%= txtNombresInt.ClientID %>');

            if (tipo === 'Docente') {
                pnlBusqueda.style.display = 'block';

                if (txtNombre.value.trim() === "") {
                    pnlDatos.style.display = 'none';
                } else {
                    pnlDatos.style.display = 'block';
                }

            } else {
                pnlBusqueda.style.display = 'none';
                pnlDatos.style.display = 'block'; 
            }

            if (tipo === 'Externo') {
                divInterno.style.display = 'none';
                divExterno.style.display = 'flex';
            } else {
                divInterno.style.display = 'flex';
                divExterno.style.display = 'none';
            }
        }

        function AbrirModalNuevoIntegrante() {
            var el = document.getElementById('modalNuevoIntegrante');
            var modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
            ToggleTipoIntegranteModal();
        }

    </script>

    <script type="text/javascript">

        // 1. Mostrar Error (Reutilizable)
        function mostrarError(campoId, mensaje) {
            if (typeof toastify === 'function') {
                toastify('ww', mensaje, 'Sistema');
            } else {
                alert(mensaje);
            }
            var campo = document.getElementById(campoId);
            if (campo) {
                campo.classList.add('is-invalid');
                campo.focus();
                // Quitar rojo al escribir
                campo.addEventListener('input', function () {
                    this.classList.remove('is-invalid');
                }, { once: true });
            }
        }

        // 2. Algoritmo Cédula Ecuatoriana
        function esCedulaValida(cedula) {
            if (cedula.length !== 10 || isNaN(cedula)) return false;
            var provincia = parseInt(cedula.substring(0, 2), 10);
            if (provincia < 1 || (provincia > 24 && provincia !== 30)) return false;
            var tercerDigito = parseInt(cedula.substring(2, 3), 10);
            if (tercerDigito >= 6) return false;

            var coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];
            var verificador = parseInt(cedula.substring(9, 10), 10);
            var suma = 0;

            for (var i = 0; i < 9; i++) {
                var valor = parseInt(cedula.substring(i, i + 1), 10) * coeficientes[i];
                suma += (valor >= 10) ? valor - 9 : valor;
            }

            var digitoCalculado = (suma % 10 === 0) ? 0 : (10 - (suma % 10));
            return verificador === digitoCalculado;
        }

        // 3. Regex Correo
        function esEmailValido(email) {
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
        }

        // 4. FUNCIÓN PRINCIPAL QUE LLAMA EL BOTÓN
        function ValidarNuevoIntegrante() {
            var idCedula = '<%= txtCedulaInt.ClientID %>';
            var idCorreo = '<%= txtCorreoInt.ClientID %>';

            var valCedula = document.getElementById(idCedula).value.trim();
            if (!esCedulaValida(valCedula)) {
                mostrarError(idCedula, 'La cédula ingresada no es válida.');
                return false;
            }

            var valCorreo = document.getElementById(idCorreo).value.trim();
            if (!esEmailValido(valCorreo)) {
                mostrarError(idCorreo, 'El formato del correo es incorrecto.');
                return false; 
            }

            return true; 
        }

        function ValidarPuntajeProyecto(esEdicion) {
            var idCampo = esEdicion ? '<%= txtPuntajeEdit.ClientID %>' : '<%= txtPuntaje.ClientID %>';
            var idTema = esEdicion ? '<%= txtTemaEdit.ClientID %>' : '<%= txtTema.ClientID %>';

            var inputPuntaje = document.getElementById(idCampo);
            var inputTema = document.getElementById(idTema);

            if (inputTema && inputTema.value.trim() === "") {
                mostrarError(idTema, 'El título del proyecto es obligatorio.');
                return false;
            }

            if (inputPuntaje) {
                var valor = inputPuntaje.value.trim();

                if (valor !== "") {
                    var puntaje = parseFloat(valor);

                    if (isNaN(puntaje)) {
                        mostrarError(idCampo, 'Ingrese un valor numérico válido.');
                        return false;
                    }

                    if (puntaje < 0) {
                        mostrarError(idCampo, 'La calificación no puede ser menor a 0.');
                        return false;
                    }

                    if (puntaje > 150) {
                        mostrarError(idCampo, 'La calificación no puede superar los 150 puntos.');
                        return false;
                    }
                }
            }

            return true; 
        }

    </script>

</asp:Content>