<%@ Page Title="Mis Proyectos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProyectosAprobadosCoordinadores.aspx.cs" Inherits="SistemaGestionCGI.ProyectosAprobadosCoordinadores" %>
<%@ Register Src="~/GeneradorInforme.ascx" TagPrefix="uc" TagName="GeneradorInforme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-informes.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/proyectos-pro-coordinador.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/informes-coordinadores-pro.css" rel="stylesheet" />

    <div class="utc-hero p-3 p-md-4 mb-4 d-flex justify-content-between align-items-start flex-wrap gap-3">
        <div>
            <h3 class="utc-title mb-1"><i class="fa-solid fa-layer-group me-2"></i> GESTIÓN DE PROYECTOS</h3>
            <div class="text-muted fw-semibold">Panel de Control para Coordinadores</div>
        </div>
        <span class="utc-chip"><i class="fa-solid fa-user-tie"></i> Rol: Coordinador</span>
    </div>

    <asp:Panel ID="pnlListadoTarjetas" runat="server">
        <div class="row g-4">
            <asp:Repeater ID="rptProyectosCoordinador" runat="server"
                OnItemCommand="rptProyectosCoordinador_ItemCommand"
                OnItemDataBound="rptProyectosCoordinador_ItemDataBound">
                <ItemTemplate>
                    <div class="col-12">
                        <div class="card card-hero-project shadow-utc rounded-4 mb-4">
                            <div class="row g-0 h-100">
                    
                                <div class="col-lg-8 p-4 p-md-5 d-flex flex-column justify-content-center">
                        
                                    <div class="d-flex align-items-center gap-3 mb-3">
                                        <span class="badge rounded-pill bg-primary-subtle text-primary border border-primary-subtle px-3 py-2 fs-6">
                                            <i class="fa-solid fa-circle-play me-2"></i> <%# Eval("strEstado_ejec") %>
                                        </span>
                                        <span class="text-muted fw-bold text-uppercase small letter-spacing-1">
                                            PROYECTO ID: #<%# Eval("strId_ejec") %>
                                        </span>
                                    </div>

                                    <h2 class="fw-bold text-dark mb-4 lh-sm" style="color: var(--utc-azul) !important;">
                                        <%# Eval("TituloProyecto") %>
                                    </h2>

                                    <div class="row g-3 mb-4">
                                        <div class="col-md-4">
                                            <div class="metric-box-hero">
                                                <div class="metric-icon"><i class="fa-regular fa-calendar-check"></i></div>
                                                <small class="text-muted d-block text-uppercase fw-bold" style="font-size:0.7rem">Periodo Académico</small>
                                                <div class="fw-bold text-dark mt-1 lh-1"><%# Eval("strPeriodo_ejec") %></div>
                                            </div>
                                        </div>
                            
                                        <div class="col-md-4">
                                            <div class="metric-box-hero">
                                                <div class="metric-icon"><i class="fa-solid fa-hourglass-start"></i></div>
                                                <small class="text-muted d-block text-uppercase fw-bold" style="font-size:0.7rem">Fecha de Inicio</small>
                                                <div class="fw-bold text-dark mt-1 fs-5">
                                                    <%# Convert.ToDateTime(Eval("dtFechaini_ejec")).ToString("MMMM yyyy").ToUpper() %>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-4">
                                            <div class="metric-box-hero border-primary-subtle bg-primary-subtle bg-opacity-10">
                                                <div class="metric-icon text-primary"><i class="fa-solid fa-file-invoice"></i></div>
                                                <small class="text-primary d-block text-uppercase fw-bold" style="font-size:0.7rem">Avances Cargados</small>
                                                <div class="fw-bold text-primary mt-1 fs-4"><%# Eval("CantidadInformes") %></div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mt-auto pt-2 border-top">
                                        <div class="d-flex align-items-center gap-2">
                                            <span class="text-muted small">Estado del plazo:</span>
                                            <asp:Literal ID="litAlertaPlazo" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 action-column p-4 p-md-5 text-center position-relative">
                        
                                    <i class="fa-solid fa-layer-group position-absolute text-muted opacity-10" 
                                       style="font-size: 10rem; right: -20px; bottom: -20px; transform: rotate(-15deg);"></i>

                                    <div class="position-relative z-1">
                                        <div class="mb-4">
                                            <div class="bg-white rounded-circle shadow-sm d-inline-flex align-items-center justify-content-center mb-3" 
                                                 style="width: 80px; height: 80px;">
                                                <i class="fa-solid fa-sliders fa-2x" style="color: var(--utc-azul);"></i>
                                            </div>
                                            <h5 class="fw-bold text-dark">Panel de Control</h5>
                                            <p class="text-muted small">Gestiona tu equipo y evidencias</p>
                                        </div>

                                        <div class="d-grid gap-3">
                                            <asp:LinkButton ID="btnInformes" runat="server"
                                                CommandName="Informes" CommandArgument='<%# Eval("strId_ejec") %>'
                                                CssClass="btn btn-primary btn-pill shadow py-3 fw-bold fs-6">
                                                <i class="fa-solid fa-folder-open me-2"></i> GESTIONAR PROYECTO
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnEquipo" runat="server"
                                                CommandName="Equipo" CommandArgument='<%# Eval("strId_ejec") %>'
                                                CssClass="btn btn-white border btn-pill py-2 text-muted fw-semibold hover-lift bg-white">
                                                <i class="fa-solid fa-users me-2"></i> Ver Equipo de Trabajo
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlGestionProyecto" runat="server" Visible="false">
    
        <div class="card shadow-utc border-0 rounded-4 mb-3">
            <div class="card-body p-3 px-4 d-flex justify-content-between align-items-center">
        
                <div class="d-flex align-items-center gap-3">
                    <div class="bg-primary bg-opacity-10 p-2 rounded-circle d-flex align-items-center justify-content-center" style="width:45px; height:45px;">
                        <i class="fa-solid fa-folder-tree fa-lg" style="color: var(--utc-azul);"></i>
                    </div>
                    <div>
                        <h5 class="fw-bold text-dark mb-0" style="letter-spacing: 0.5px;">PANEL DE GESTIÓN</h5>
                        <small class="text-muted">Administra las evidencias y el historial del proyecto</small>
                    </div>
                </div>

                <asp:LinkButton ID="btnVolverDesdeGestion" runat="server" 
                    CssClass="btn btn-white border btn-pill px-4 text-muted shadow-sm hover-lift" 
                    OnClick="btnVolverTarjeta_Click">
                    <i class="fa-solid fa-arrow-left me-2"></i> Regresar al Listado
                </asp:LinkButton>

            </div>
        </div>

        <div class="card shadow-utc border-0 rounded-4 overflow-hidden">
        
            <div class="card-header bg-white pt-2 px-4 border-0">
                <ul class="nav nav-tabs-utc" id="myTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <button class="nav-link active" id="gestion-tab" data-bs-toggle="tab" data-bs-target="#gestion-pane" type="button" role="tab">
                            <i class="fa-solid fa-folder-open me-2"></i> Gestión de Avances
                        </button>
                    </li>
                    <li class="nav-item" role="presentation">
                        <button class="nav-link" id="cronologia-tab" data-bs-toggle="tab" data-bs-target="#cronologia-pane" type="button" role="tab">
                            <i class="fa-solid fa-clock-rotate-left me-2"></i> Línea de Tiempo
                        </button>
                    </li>
                </ul>
            </div>

            <div class="card-body p-4 bg-light bg-opacity-10">
                <div class="tab-content" id="myTabContent">
                
                    <div class="tab-pane fade show active" id="gestion-pane" role="tabpanel">
    
                        <div class="d-flex justify-content-between align-items-center mb-4 p-3 bg-white rounded-3 border shadow-sm">
                            <span class="fw-bold text-secondary small text-uppercase">Acciones Disponibles</span>
                            <asp:Literal ID="lblEstadoPeriodo" runat="server"></asp:Literal>
                            <div class="d-flex gap-2">
        
                                <asp:LinkButton ID="btnAbrirGenerador" runat="server" 
                                    CssClass="btn btn-outline-primary btn-sm btn-pill px-3 d-flex align-items-center gap-2" 
                                    OnClick="btnAbrirGenerador_Click">
                                    <i class="fa-solid fa-wand-magic-sparkles"></i> Generar
                                </asp:LinkButton>

                                <button type="button" id="btnSubirEscaneado" runat="server"
                                    class="btn btn-primary-utc btn-sm btn-pill text-white px-4 d-flex align-items-center gap-2 shadow-sm"
                                    onclick="LimpiarYSubir()">
                                    <i class="fa-solid fa-cloud-arrow-up"></i> Subir PDF
                                </button>

                            </div>
                        </div>

                        <div class="row g-3">
                            <asp:Repeater ID="rptInformes" runat="server" OnItemCommand="rptInformes_ItemCommand">
                                <ItemTemplate>
                                    <div class="col-12">
                                        <div class="bg-white p-3 border rounded-3 h-100 shadow-sm position-relative hover-lift">
            
                                            <div class="d-flex justify-content-between align-items-start">
                                                <div class="d-flex align-items-center gap-3 overflow-hidden me-3">
                                                    <div class="bg-light p-3 rounded-3 d-flex align-items-center justify-content-center flex-shrink-0" style="width: 55px; height: 55px;">
                                                        <i class='<%# GetFileIconClass(Eval("strArchivo_path")) %> fa-2x'></i>
                                                    </div>
                                                    <div class="overflow-hidden">
                                                        <p class="mb-1 fw-bold text-dark text-truncate" style="font-size: 1rem;">
                                                            <%# Eval("strNombrePeriodo") %>
                                                        </p>
                                                        <div class="d-flex align-items-center gap-2 flex-wrap">
                                                            <span class="badge bg-secondary bg-opacity-10 text-secondary border border-secondary-subtle" style="font-size: 0.7rem;">
                                                                <%# GetFileTypeLabel(Eval("strArchivo_path")) %>
                                                            </span>
                                                            <span class="text-muted small">|</span>
                                                            <small class="text-muted fw-semibold">
                                                                <i class="fa-regular fa-clock me-1"></i>
                                                                <%# Convert.ToDateTime(Eval("dtFechaSubida")).ToString("dd MMM, HH:mm") %>
                                                            </small>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="d-flex gap-2 flex-shrink-0 ms-2">
                                                    <a href='<%# ResolveUrl(Eval("strArchivo_path").ToString()) %>' target="_blank" class="btn-action-icon text-primary border-0"><i class="fa-solid fa-eye"></i></a>
                    
                                                    <asp:LinkButton ID="btnEditarInf" runat="server" CommandName="EditarInforme" CommandArgument='<%# Eval("strId_informe") %>' CssClass="btn-action-icon text-primary border-0"><i class="fa-solid fa-pen"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="btnEliminarInf" runat="server" CommandName="EliminarInforme" CommandArgument='<%# Eval("strId_informe") %>' CssClass="btn-action-icon text-danger border-0" OnClientClick="return confirm('¿Eliminar informe?');"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                                                </div>
                                            </div>

                                            <asp:Panel ID="pnlObservacion" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("strObservacion_informe") as string) %>' CssClass="mt-3">
    
                                                <div class="border border-warning rounded-2 overflow-hidden mb-2" style="background-color: #fffbf0;">
        
                                                    <div class="px-3 py-2 bg-warning bg-opacity-25 border-bottom border-warning border-opacity-25 d-flex align-items-center gap-2">
                                                        <i class="fa-solid fa-circle-exclamation text-warning-emphasis"></i>
                                                        <span class="fw-bold text-warning-emphasis small text-uppercase" style="letter-spacing: 0.5px;">Observación Administrativa</span>
                                                    </div>

                                                    <div class="p-3">
                                                        <p class="mb-3 text-dark opacity-75 small" style="line-height: 1.5; font-family: 'Segoe UI', sans-serif;">
                                                            <%# Eval("strObservacion_informe") %>
                                                        </p>

                                                        <div class="d-flex justify-content-end">
                
                                                            <asp:PlaceHolder ID="phNoLeido" runat="server" Visible='<%# Eval("dtFechaLectura_informe") == null %>'>
                                                                <div class="d-flex align-items-center gap-2 bg-white border rounded p-1 ps-3 shadow-sm">
                                                                    <span class="small text-muted fw-semibold">Acción requerida:</span>
                                                                   <asp:LinkButton ID="btnMarcarLeido" runat="server" 
                                                                        CommandName="MarcarLeido" 
                                                                        CommandArgument='<%# Eval("strId_informe") %>'
                                                                        CssClass="btn-confirmar-utc">
                                                                        <i class="fa-solid fa-file-signature"></i> 
                                                                        <span>Confirmar Lectura</span>
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </asp:PlaceHolder>

                                                            <asp:PlaceHolder ID="phLeido" runat="server" Visible='<%# Eval("dtFechaLectura_informe") != null %>'>
                                                                <div class="d-flex align-items-center gap-2 text-success border border-success border-opacity-25 bg-success bg-opacity-10 px-3 py-1 rounded" 
                                                                     title="Lectura confirmada por el coordinador">
                                                                    <i class="fa-solid fa-clipboard-check"></i>
                                                                    <div class="d-flex flex-column" style="line-height: 1;">
                                                                        <span class="fw-bold text-uppercase" style="font-size: 0.65rem;">Revisión Confirmada</span>
                                                                        <span class="small" style="font-size: 0.7rem;">
                                                                            <%# Convert.ToDateTime(Eval("dtFechaLectura_informe")).ToString("dd/MM/yyyy HH:mm") %>
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </asp:PlaceHolder>

                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:Panel>

                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <div id="sinDatos" runat="server" visible="false" class="col-12 text-center py-5">
                                <div class="mb-3 text-muted opacity-25"><i class="fa-regular fa-folder-open fa-4x"></i></div>
                                <h6 class="text-muted fw-bold">Sin informes cargados</h6>
                                <p class="small text-muted">Utiliza el botón "Subir PDF" para comenzar.</p>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="cronologia-pane" role="tabpanel">
    
                        <div class="d-flex align-items-center mb-3">
                            <i class="fa-solid fa-clock-rotate-left text-muted me-2"></i>
                            <span class="text-muted fw-bold small text-uppercase">Historial de Evidencias</span>
                        </div>

                        <div class="scroll-cronologia pe-2" style="max-height: 600px; overflow-y: auto;">
    
                            <asp:Repeater ID="rptPeriodos" runat="server" OnItemDataBound="rptPeriodos_ItemDataBound">
                                <ItemTemplate>
            
                                    <div class="d-flex align-items-center mb-3 mt-2">
                                        <span class="badge bg-warning bg-opacity-10 text-warning border border-warning fw-bold px-3 py-2 w-100 text-start">
                                            <i class="fa-solid fa-calendar-range me-2"></i> <%# Eval("NombrePeriodo") %>
                                        </span>
                                    </div>

                                    <div class="timeline-group mb-4 ps-2">
                
                                        <asp:Repeater ID="rptArchivosPeriodo" runat="server">
                                            <ItemTemplate>
                                                <div class="history-card <%# GetBorderColor(Eval("TipoDoc").ToString()) %> mb-3">
                                                    <div class="d-flex align-items-center gap-3">
                                
                                                        <div class="icon-box-history <%# GetIconBgClass(Eval("TipoDoc").ToString()) %>">
                                                            <i class='<%# GetIconClass(Eval("TipoDoc").ToString()) %>'></i>
                                                        </div>

                                                        <div class="flex-grow-1 overflow-hidden">
                                                            <div class="d-flex justify-content-between align-items-center mb-1">
                                                                <span class="badge bg-light text-dark border"><%# Eval("TipoDoc") %></span>
                                                                <small class="text-muted" style="font-size: 0.75rem;">
                                                                    <%# Convert.ToDateTime(Eval("Fecha")).ToString("dd MMM yyyy") %>
                                                                </small>
                                                            </div>
                                                            <h6 class="mb-0 fw-bold text-dark text-truncate" title='<%# Eval("Nombre") %>'>
                                                                <%# Eval("Nombre") %>
                                                            </h6>
                                                            <small class="text-muted d-block text-truncate">
                                                                <%# GetDescripcion(Eval("TipoDoc").ToString()) %>
                                                            </small>
                                                        </div>

                                                        <%-- Botones --%>
                                                        <div class="flex-shrink-0 ms-2 d-flex gap-2">
                                                            <a href='<%# ResolveUrl(Eval("Ruta").ToString()) %>' target="_blank" class="btn btn-light btn-sm text-primary border shadow-sm"><i class="fa-solid fa-eye"></i></a>
                                                            <a href='<%# ResolveUrl(Eval("Ruta").ToString()) %>' download class="btn btn-light btn-sm text-dark border shadow-sm"><i class="fa-solid fa-download"></i></a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </div>

                                </ItemTemplate>
                            </asp:Repeater>

                            <div id="sinHistorial" runat="server" visible="false" class="text-center py-4 text-muted">
                                No hay historial disponible.
                            </div>
                        </div>

                        <div style="display:none;">
                            <asp:HyperLink ID="lnkVerCierre" runat="server" />
                            <asp:HyperLink ID="lnkVerFinal" runat="server" />
                        </div>

                    </div>

                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEquipoListado" runat="server" Visible="false">
        <asp:HiddenField ID="hfIdEjecucionEquipo" runat="server" />
        <div class="utc-hero p-3 p-md-4 mb-3 d-flex justify-content-between align-items-start flex-wrap gap-3">
            <div>
                <h3 class="utc-title mb-1"><i class="fa-solid fa-users me-2"></i> EQUIPO DE TRABAJO</h3>
                <div class="text-muted fw-semibold">Listado de integrantes del proyecto</div>
            </div>
            <asp:LinkButton runat="server" ID="btnVolverTarjeta" CssClass="btn btn-outline-primary btn-pill px-4 w-auto" OnClick="btnVolverTarjeta_Click">
                <i class="fa-solid fa-arrow-left"></i> VOLVER
            </asp:LinkButton>
        </div>
        <div class="table-responsive bg-white p-3 rounded shadow-utc border">
            <table id="tablaMiembros" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr><th>CÉDULA</th><th>INTEGRANTE</th><th>ROL</th><th>FACULTAD / ORIGEN</th><th>ESTADO</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptMiembros" runat="server" OnItemDataBound="rptMiembros_ItemDataBound">
                        <ItemTemplate>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "" : "table-secondary text-muted" %>'>
                                <td class="fw-bold text-secondary small"><%# Eval("strCedula_miembro") %></td>
                                <td class="text-start">
                                    <div class="fw-bold text-primary"><%# Eval("strApellidos_miembro") %></div>
                                    <div class="small text-muted"><%# Eval("strNombres_miembro") %></div>
                                </td>
                                <td><span class="badge bg-light text-dark border fw-normal"><%# Eval("strRol_miembro") %></span></td>
                                <td class="small text-muted text-start"><%# Eval("strFacultad_miembro").ToString() == "EXTERNO" ? Eval("strEntidad_miembro") : Eval("strFacultad_miembro") %></td>
                                <td><%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "<span class='badge bg-success'>Activo</span>" : "<span class='badge bg-danger'>Inactivo</span>" %></td>
                                <asp:LinkButton ID="btnEditarM" runat="server" Visible="false" />
                                <asp:LinkButton ID="btnToggleEstado" runat="server" Visible="false" />
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalSubirInforme" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-lg rounded-4 border-0 modal-utc-shell">
                <div class="modal-header modal-utc-header text-white py-2">
                    <h6 class="modal-title fw-bold text-white" id="lblTituloModalInforme" runat="server">Cargar Documento</h6>
                    <button type="button" class="btn-close btn-close-white" onclick="CerrarSubModalUpload()"></button>
                </div>
                <div class="modal-body modal-utc-body p-4">
                    <asp:HiddenField ID="hfIdEjecucionInforme" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfIdInformeEdit" runat="server" ClientIDMode="Static" />
                    
                    <div class="mb-3">
                        <label class="form-label fw-bold small" style="color: var(--utc-azul);">Nombre / Etiqueta del Informe</label>
                        <asp:TextBox ID="txtNombrePeriodoInf" runat="server" CssClass="form-control" placeholder="Ej: Informe Trimestral 1" />
                    </div>

                    <div class="utc-fileinput-wrapper" id="wrapperArchivoInf">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-pdf"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2 w-100 ms-3">
                                <span class="utc-fileinput-name fw-semibold">Ningún archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" style="display:none;" />
                        <div class="utc-fileinput-preview" id="previewArchivoInf"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoInf" style="display:none;">
                            <i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...
                        </div>
                        <div class="utc-dropzone" id="dropzoneArchivoInf">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2" style="color: var(--utc-azul);"></i><br />
                            <span class="fw-bold" style="color: var(--utc-azul);">Arrastra tu PDF aquí o haz clic</span>
                        </div>
                        <asp:FileUpload ID="flpArchivoInf" runat="server" CssClass="utc-fileinput-input" accept=".pdf" />
                    </div>

                    <div class="d-grid mt-4">
                        <asp:LinkButton ID="btnGuardarInforme" runat="server"
                            CssClass="btn btn-primary w-100 btn-pill fw-bold text-white shadow-sm py-2"
                            OnClick="btnGuardarInforme_Click">
                            <i class="fa-solid fa-floppy-disk me-2"></i> GUARDAR ARCHIVO
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div style="display:none;">
        <asp:Panel ID="pnlFormularioMiembro" runat="server">
            <asp:Label ID="lblTituloFormMiembro" runat="server" />
            <asp:HiddenField ID="hfIdMiembroEdit" runat="server" />
            <asp:DropDownList ID="ddlTipoMiembro" runat="server" />
            <asp:TextBox ID="txtCedulaMiembro" runat="server" />
            <asp:TextBox ID="txtNombresMiembro" runat="server" />
            <asp:TextBox ID="txtApellidosMiembro" runat="server" />
            <asp:TextBox ID="txtCorreoMiembro" runat="server" />
            <asp:DropDownList ID="ddlRolMiembro" runat="server" />
            <asp:DropDownList ID="ddlFacultadMiembro" runat="server" />
            <asp:TextBox ID="txtCarreraMiembro" runat="server" />
            <asp:TextBox ID="txtEntidadMiembro" runat="server" />
            <asp:LinkButton ID="btnGuardarMiembro" runat="server" OnClick="btnGuardarMiembro_Click" />
            <asp:LinkButton ID="btnCancelarMiembro" runat="server" OnClick="btnCancelarMiembro_Click" />
            <asp:LinkButton runat="server" ID="btnAbrirFormMiembro" />
        </asp:Panel>
        <asp:Panel ID="pnlArchivoCierreActual" runat="server"><asp:Label ID="lblNombreArchivoCierre" runat="server"></asp:Label><a id="lnkVerCierreActual" runat="server"></a></asp:Panel>
        <asp:Panel ID="pnlCierreBloqueado" runat="server"></asp:Panel>
        <asp:Panel ID="pnlCargaCierre" runat="server"><div id="wrapperCierre"><div id="dropzoneCierre"></div><div id="previewCierre"></div><asp:FileUpload ID="flpCierre" runat="server" /></div><asp:LinkButton ID="btnGuardarCierre" runat="server" OnClick="btnGuardarCierre_Click"><asp:Literal ID="litBtnCierreTexto" runat="server" /></asp:LinkButton><asp:LinkButton ID="btnAprobarCierre" runat="server"></asp:LinkButton></asp:Panel>
        <asp:Panel ID="pnlArchivoFinalActual" runat="server"><asp:Label ID="lblNombreArchivoFinal" runat="server"></asp:Label><a id="lnkVerFinalActual" runat="server"></a></asp:Panel>
        <asp:Panel ID="pnlCargaFinal" runat="server"><div id="wrapperFinal"><div id="dropzoneFinal"></div><div id="previewFinal"></div><asp:FileUpload ID="flpFinal" runat="server" /></div><asp:LinkButton ID="btnGuardarFinal" runat="server" OnClick="btnGuardarFinal_Click" /></asp:Panel>
    </div>

    <uc:GeneradorInforme ID="ucGenerador" runat="server" OnInformeGuardado="ucGenerador_InformeGuardado" />

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>

    <script type="text/javascript">
        function AbrirSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            if(el) bootstrap.Modal.getOrCreateInstance(el).show();
        }

        function CerrarSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            if(el) {
                var modal = bootstrap.Modal.getInstance(el);
                if (modal) modal.hide();
            }
        }

        function LimpiarYSubir() {
            document.getElementById('<%= hfIdInformeEdit.ClientID %>').value = "";
            document.getElementById('<%= txtNombrePeriodoInf.ClientID %>').value = "";
            AbrirSubModalUpload();
        }

        Sys.Application.add_load(function () {
            if ($('#tablaMiembros').length) {
                if ($.fn.DataTable.isDataTable('#tablaMiembros')) $('#tablaMiembros').DataTable().destroy();
                $('#tablaMiembros').DataTable({
                    responsive: true, autoWidth: false, pageLength: 10,
                    language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                    dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>><'row'<'col-sm-12'tr>><'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
                });
            }

            if (typeof UTC_FileInput === 'function' && document.getElementById("wrapperArchivoInf")) {
                UTC_FileInput({
                    wrapper: "wrapperArchivoInf",
                    dropzone: "dropzoneArchivoInf",
                    preview: "previewArchivoInf",
                    loader: "loaderArchivoInf",
                    input: "<%= flpArchivoInf.ClientID %>"
                });
            }
        });
    </script>

</asp:Content>