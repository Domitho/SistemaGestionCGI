<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EjecucionProAprobados.aspx.cs" Inherits="SistemaGestionCGI.EjecucionProAprobados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-informes.css" rel="stylesheet" />

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

    <div id="headerEjecucion" runat="server" 
         class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-person-digging me-2"></i> PROYECTOS EN EJECUCIÓN
        </h3>

        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevoEjecucion"
                CssClass="btn btn-primary btn-pill d-flex align-items-center"
                OnClick="btnNuevoEjecucion_Click">
                <i class="fa-solid fa-plus me-2"></i> INICIAR EJECUCIÓN
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresar"
                CssClass="btn btn-outline-primary btn-pill px-4"
                OnClick="btnRegresar_Click"
                Visible="false" CausesValidation="false">
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
                                            
                                    <asp:LinkButton ID="btnInformes" runat="server" 
                                        CommandName="Informes" CommandArgument='<%# Eval("strId_ejec") %>'
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" 
                                        ToolTip="Subir Informes/Avances">
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
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-circle-plus me-2"></i> Iniciar Nueva Ejecución
            </h4>
            
            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label fw-bold">Proyecto Aprobado</label>
                    <asp:DropDownList ID="ddlProyectosAprobados" runat="server" CssClass="form-select"
                        AutoPostBack="true" 
                        OnSelectedIndexChanged="ddlProyectosAprobados_SelectedIndexChanged">
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
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            
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
                        <th>ROL</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptMiembros" runat="server" OnItemCommand="rptMiembros_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_miembro") %></td>
                                <td><%# Eval("strCedula_miembro") %></td>
                                <td class="text-start"><%# Eval("strNombres_miembro") %></td>
                                <td class="text-start"><%# Eval("strApellidos_miembro") %></td>
                                <td><%# Eval("strRol_miembro") %></td>
                                <td>
                                    <asp:LinkButton ID="btnEditarM" runat="server" 
                                        CommandName="EditarMiembro" CommandArgument='<%# Eval("strId_miembro") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminarM" runat="server" 
                                        CommandName="EliminarMiembro" CommandArgument='<%# Eval("strId_miembro") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle"
                                        OnClientClick="return confirm('¿Quitar a este integrante del equipo?');"
                                        ToolTip="Quitar">
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

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            
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
                            <div class="bg-light rounded-circle p-2 d-flex align-items-center justify-content-center" style="width:45px; height:45px;">
                                <i class="fa-solid fa-folder-tree text-primary fs-5"></i>
                            </div>
                            <div class="utc-toolbar-text">
                                <h6>Repositorio Digital</h6>
                                <small>Gestione los informes de avance del proyecto</small>
                            </div>
                        </div>
                        
                        <button type="button" class="btn-upload-modern" onclick="LimpiarYSubir()">
                            <i class="fa-solid fa-cloud-arrow-up me-2"></i> Subir Nuevo
                        </button>
                    </div>

                    <div class="row g-3">
                        <asp:Repeater ID="rptInformes" runat="server" OnItemCommand="rptInformes_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-4 col-sm-6">
                                    
                                    <div class="file-card" onclick="DescargarWord('<%# Eval("strId_informe") %>')">
                                        
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
                                            <i class="fa-solid fa-file-word text-primary"></i>
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
                            <i class="fa-solid fa-file-word fa-2x mb-2 text-primary"></i><br />
                            Arrastra documento Word aquí o haz clic
                        </div>

                        <asp:FileUpload ID="flpArchivoInf" runat="server" CssClass="utc-fileinput-input" 
                            accept=".doc,.docx,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document"/>
                    </div>

                    <div class="form-text small text-muted mb-3">
                        <i class="fa-solid fa-circle-info me-1"></i> Si está editando, suba un archivo solo si desea reemplazar el actual.
                    </div>

                    <div class="d-grid gap-2">
                        <asp:LinkButton ID="btnGuardarInforme" runat="server" CssClass="btn btn-primary btn-lg shadow-sm"
                            OnClick="btnGuardarInforme_Click">
                            <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Informe
                        </asp:LinkButton>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" style="z-index: 1070;" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content border-0 rounded-4 shadow-lg" style="background: #333;">
                <div class="modal-header border-bottom-0 py-2 px-3">
                    <h6 class="modal-title text-white" id="lblTituloPreview">Vista Previa</h6>
                    <div>
                        <a id="btnDescargarDirecto" href="#" target="_blank" class="btn btn-sm btn-outline-light me-2">
                            <i class="fa-solid fa-download"></i> Descargar
                        </a>
                        <button type="button" class="btn-close btn-close-white" onclick="CerrarVistaPrevia()"></button>
                    </div>
                </div>
                <div class="modal-body p-0" style="height: 80vh;">
                    <iframe id="framePdf" class="pdf-viewer-frame"></iframe>
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
            // Limpia el HiddenField de edición para indicar que es nuevo
            document.getElementById('<%= hfIdInformeEdit.ClientID %>').value = "";
            document.getElementById('<%= lblTituloModalInforme.ClientID %>').innerText = "Subir Informe";
            document.getElementById('<%= txtNombrePeriodoInf.ClientID %>').value = "";
            AbrirSubModalUpload(); 
        }

        // Nueva función para WORD (Descarga directa)
        function DescargarWord(id) {
            // Usamos el Handler con tipo INFORME. Al ser Word, el navegador lo descargará.
            var url = 'VerArchivo.ashx?id=' + id + '&tipo=INFORME';
            window.location.href = url;
        }

        /* ASÍ DEBE QUEDAR (Versión nueva con Handler) */
        function VerPDF(id, tipo) {
            var url = 'VerArchivo.ashx?id=' + id + '&tipo=' + tipo; // Construye la URL segura

            document.getElementById('framePdf').src = url;
            document.getElementById('lblTituloPreview').innerText = "Visualización de Documento";
            document.getElementById('btnDescargarDirecto').href = url;

            var el = document.getElementById('modalVistaPrevia');
            var modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
        }

        function CerrarVistaPrevia() {
            var el = document.getElementById('modalVistaPrevia');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
            document.getElementById('framePdf').src = '';
        }

    </script>

</asp:Content>