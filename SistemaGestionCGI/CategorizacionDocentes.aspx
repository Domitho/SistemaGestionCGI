<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CategorizacionDocentes.aspx.cs" Inherits="SistemaGestionCGI.CategorizacionDocentes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     
    <%-- RECURSOS DE ESTILO UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/papelera.css" rel="stylesheet" />

    <%-- ENCABEZADO PRINCIPAL --%>
    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-graduation-cap me-2"></i> GESTIÓN DE CATEGORIZACIÓN
        </h3>
        <div class="d-flex gap-2">
            <asp:LinkButton runat="server" ID="btnVerPapelera" CssClass="btn btn-outline-danger btn-pill" OnClick="btnVerPapelera_Click">
                <i class="fa-solid fa-trash-can me-2"></i> PAPELERA
            </asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnNuevo" CssClass="btn btn-primary btn-pill d-flex align-items-center"
                OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO DOCENTE
            </asp:LinkButton>
            
            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" Visible="false"
                OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: GRILLA DE DOCENTES --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaDocentes" class="table table-bordered table-hover table-utc align-middle text-center" style="width: 100%">
                <thead>
                    <tr>
                        <th>CÉDULA</th>
                        <th>DOCENTE</th>
                        <th>FACULTAD</th>
                        <th>CATEGORÍA ACTUAL</th>
                        <th>FECHA RESOLUCIÓN</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptDatos" runat="server" OnItemCommand="rptDatos_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strCedula_doc") %></td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("NombreCompleto") %></td>
                                <td class="small text-muted"><%# Eval("NombreFacultad") %></td>
                                
                                <td>
                                    <span class='<%# string.IsNullOrEmpty(Eval("NombreCategoria")?.ToString()) ? "badge bg-secondary opacity-50 rounded-pill px-3" : "badge bg-primary rounded-pill px-3" %>'>
                                        <%# string.IsNullOrEmpty(Eval("NombreCategoria")?.ToString()) ? "SIN ASIGNAR" : Eval("NombreCategoria") %>
                                    </span>
                                </td>
                                
                                <td><%# Eval("dtFechaCategorizacion") == null ? "-" : Convert.ToDateTime(Eval("dtFechaCategorizacion")).ToString("dd/MM/yyyy") %></td>
                                
                                <td>
                                    <asp:LinkButton ID="btnVerCertificado" runat="server" 
                                        CommandName="VerCertificado" 
                                        CommandArgument='<%# Eval("strCertificado_doc") %>'
                                        Visible='<%# !string.IsNullOrEmpty(Eval("strCertificado_doc") as string) %>'
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" 
                                        ToolTip="Ver Certificado de Categorización">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="editar" CommandArgument='<%# Eval("strId_doc") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Asignar o Cambiar Categoría">
                                        <i class="fa-solid fa-pen-to-square"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="historial" CommandArgument='<%# Eval("strId_doc") %>'
                                        CssClass="btn btn-info btn-sm rounded-circle text-white me-1" ToolTip="Ver Historial">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="eliminar" CommandArgument='<%# Eval("strId_doc") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle"
                                        OnClientClick="return confirmarEliminar(this, '¿Está seguro de ENVIAR este docente a la papelera? Podrá restaurarlo posteriormente.');"
                                        ToolTip="Enviar a Papelera">
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

    <%-- PANEL 2: FORMULARIO DE GESTIÓN --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4 bg-white">
            <h4 class="utc-subtitle mb-4 text-center border-bottom pb-3">
                <i class="fa-solid fa-file-signature me-2"></i> Ficha del Docente
            </h4>
            <asp:HiddenField ID="hfIdDocente" runat="server" />
            
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Cédula <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" placeholder="Ingrese Cédula" MaxLength="10" />
                        <asp:LinkButton ID="btnValidarCedula" runat="server" CssClass="btn btn-primary" OnClick="btnValidarCedula_Click" CausesValidation="false">
                            <i class="fa-solid fa-magnifying-glass"></i> Validar
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nombres <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" placeholder="Nombres del docente" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Apellidos <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" placeholder="Apellidos del docente" />
                </div>

                <div class="col-md-6">
                    <label class="form-label">Correo Institucional <span class="text-muted small fw-normal">(Opcional)</span></label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" placeholder="ejemplo@utc.edu.ec"></asp:TextBox>
                </div>

                <div class="col-12">
                    <label class="form-label">Facultad <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlFacultad_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Carrera <span class="text-muted small fw-normal">(Opcional)</span></label>
                    <asp:DropDownList ID="ddlCarrera" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </div>

                <div class="col-12"><hr class="text-muted opacity-25" /></div>

                <div class="col-md-6">
                    <label class="form-label">Categoría Asignada <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label">Fecha Resolución <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" CssClass="form-control" />
                </div>

                <div class="col-12 mt-4">
                    <label class="form-label fw-semibold text-primary">
                        <i class="fa-solid fa-certificate me-1"></i> Certificado de Categorización <span class="text-muted small fw-normal">(Opcional)</span>
                    </label>
    
                    <asp:HiddenField ID="hfCertificadoActual" runat="server" ClientIDMode="Static" />

                    <div class="utc-fileinput-wrapper" id="wrapperCertificado">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-contract"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn">
                                        <i class="fa-solid fa-pen-to-square"></i>
                                    </button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn">
                                        <i class="fa-solid fa-xmark"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
        
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
        
                        <div class="utc-fileinput-preview" id="previewCertificado"></div>
        
                        <div class="utc-fileinput-loader" id="loaderCertificado">
                            <i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...
                        </div>
        
                        <div class="utc-dropzone" id="dropzoneCertificado">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />
                            Arrastra el certificado aquí o haz clic
                        </div>

                        <asp:FileUpload ID="flpCertificado" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>

            </div>

            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 py-2 shadow-sm" OnClick="btnGuardar_Click" OnClientClick="return UTC_BloquearBoton(this);" UseSubmitBehavior="false">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Ficha
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary btn-pill px-5 py-2" 
                    OnClick="btnRegresar_Click">
                    Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content rounded-4 shadow-utc border-0">
                
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100 text-center">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i> HISTORIAL DE CAMBIOS
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-white">
                    
                    <asp:HiddenField ID="hfIdDocenteHistorial" runat="server" />

                    <div class="d-flex justify-content-end mb-3 border-bottom pb-2">
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" 
                            CssClass="btn btn-danger btn-pill btn-sm px-4 shadow-sm" 
                            OnClick="btnGenerarReporte_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte Completo
                        </asp:LinkButton>
                    </div>

                    <div class="table-responsive rounded border-0">
                        <table class="table table-sm table-hover table-historial-utc align-middle text-center mb-0">
                            <thead>
                                <tr>
                                    <th style="width: 15%">FECHA</th>
                                    <th style="width: 15%">ACCIÓN</th>
                                    <th style="width: 20%">ANTERIOR</th>
                                    <th style="width: 20%">NUEVO</th>
                                    <th style="width: 20%">MOTIVO</th>
                                    <th style="width: 10%">USUARIO</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptHistorial" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="text-secondary fw-bold" style="font-size: 0.85rem;">
                                                <%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd/MM/yyyy HH:mm") %>
                                            </td>
                                            <td>
                                                <span class='badge rounded-pill px-3 <%# Eval("strAccion").ToString().Contains("BAJA") || Eval("strAccion").ToString().Contains("ELIMINACION") ? "badge-baja" : "badge-alta" %>'>
                                                    <%# Eval("strAccion") %>
                                                </span>
                                            </td>
                                            <td class="text-muted small text-start ps-3">
                                                <i class="fa-solid fa-arrow-right-from-bracket me-1 text-danger opacity-50"></i>
                                                <%# Eval("strValorAnterior") %>
                                            </td>
                                            <td class="text-primary fw-bold small text-start ps-3">
                                                <i class="fa-solid fa-arrow-right-to-bracket me-1"></i>
                                                <%# Eval("strValorNuevo") %>
                                            </td>
                                            <td class="text-start fst-italic text-muted small"><%# Eval("strMotivo") %></td>
                                            <td class="small fw-bold text-secondary">
                                                <i class="fa-solid fa-user-check me-1 opacity-50"></i>
                                                <%# Eval("strUsuario") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel Visible='<%# rptHistorial.Items.Count == 0 %>' runat="server">
                                            <tr>
                                                <td colspan="6" class="p-4 text-center text-muted">
                                                    <i class="fa-solid fa-folder-open fa-2x mb-2 d-block opacity-25"></i>
                                                    No hay movimientos registrados en el historial.
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
                    <button type="button" class="btn btn-outline-secondary btn-pill px-5" data-bs-dismiss="modal">
                        Cerrar Historial
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title">Vista Previa del Reporte</h6>
                    <div>
                        <button type="button" class="btn btn-sm btn-light me-2" onclick="imprimirReporte()">
                            <i class="fa-solid fa-print"></i> Imprimir
                        </button>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                </div>

                <div class="modal-body p-4" style="background: #525659; min-height: 500px;">
                    <div id="arealmpresion" class="report-paper bg-white mx-auto shadow-sm" style="max-width: 800px; min-height: 1000px; padding: 40px 50px;">
                        
                        <div class="header-hero-banner" style="background-color: #003876; color: white; margin: -40px -50px 40px -50px; padding: 30px; text-align: center; border-bottom: 6px solid #002a5c;">
                            <img src="https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png" alt="UTC Logo" style="height: 70px; filter: brightness(0) invert(1);" />
                        </div>

                        <div class="header-info-split d-flex justify-content-between border-bottom pb-4 mb-4" style="border-color: #003876 !important;">
                            <div class="info-left">
                                <span class="d-block text-uppercase small fw-bold text-secondary">Dirección de Investigación</span>
                                <h1 class="doc-title mb-0" style="color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase;">Ficha de Categorización</h1>
                            </div>
                            <div class="info-right text-end">
                                <div class="meta-group">
                                    <span class="d-block text-uppercase small fw-bold text-secondary">ID Referencia</span>
                                    <asp:Label ID="lblRefId" runat="server" CssClass="fw-bold fs-5 text-dark" Text="DOC-000"></asp:Label>
                                </div>
                                <div class="meta-group mt-2">
                                    <span class="d-block text-uppercase small fw-bold text-secondary">Fecha de Emisión</span>
                                    <span class="fw-bold text-dark"><%= DateTime.Now.ToString("dd/MM/yyyy") %></span>
                                </div>
                            </div>
                        </div>

                        <div class="researcher-card p-4 mb-5 rounded-3" style="background-color: #f8faff; border: 1px solid #e1e8f0; border-left: 5px solid #003876;">
                            <div class="row mb-3">
                                <div class="col-6">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">DOCENTE</span>
                                    <asp:Label ID="lblReporteNombre" runat="server" CssClass="fs-5 fw-bold text-primary"></asp:Label>
                                </div>
                                <div class="col-6 text-end">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">CÉDULA</span>
                                    <asp:Label ID="lblReporteCedula" runat="server" CssClass="fs-5 fw-bold text-dark"></asp:Label>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-6">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">FACULTAD</span>
                                    <asp:Label ID="lblReporteFacultad" runat="server" CssClass="fw-bold text-dark"></asp:Label>
                                </div>
                                <div class="col-6 text-end">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">CARRERA</span>
                                    <asp:Label ID="lblReporteCarrera" runat="server" CssClass="fw-bold text-dark"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-6">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">CATEGORÍA ACTUAL</span>
                                    <asp:Label ID="lblReporteCategoria" runat="server" CssClass="fw-bold fs-5 text-success"></asp:Label>
                                </div>
                                <div class="col-6 text-end">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">F. RESOLUCIÓN</span>
                                    <asp:Label ID="lblReporteFecha" runat="server" CssClass="fw-bold"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="timeline-container ps-2">
                            <h4 class="mb-4 pb-2 border-bottom fw-bold text-secondary">Historial de Cambios y Movimientos</h4>
                            <ul class="timeline-list list-unstyled position-relative ps-4" style="border-left: 2px solid #e9ecef;">
                                <asp:Repeater ID="rptReporteHistorial" runat="server">
                                    <ItemTemplate>
                                        <li class="timeline-item mb-4 position-relative">
                                            <div class="timeline-marker position-absolute bg-white border border-3 border-primary rounded-circle" style="width: 16px; height: 16px; left: -25px; top: 5px;"></div>
                                            <div class="timeline-content ps-3">
                                                <div class="timeline-header d-flex justify-content-between mb-1">
                                                    <span class="date fw-bold text-dark"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd 'de' MMMM, yyyy") %></span>
                                                    <span class="time text-muted small"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("HH:mm") %></span>
                                                </div>
                                                <div class="timeline-body bg-light p-3 rounded border-start border-4" style="border-color: #003876 !important;">
                                                    <div class="action-badge d-inline-block px-2 py-1 rounded small fw-bold mb-2 text-uppercase" style="background: rgba(0,56,118,0.1); color: #003876;">
                                                        <%# Eval("strAccion") %>
                                                    </div>
                                                    <p class="description mb-2 small text-muted">
                                                        <strong>Detalle:</strong> <%# Eval("strMotivo") %>
                                                    </p>
                                                    <div class="user-signature small text-secondary">
                                                        <i class="fa-solid fa-user-check me-1"></i> Responsable: <strong><%# Eval("strUsuario") %></strong>
                                                    </div>
                                                </div>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>

                        <div class="report-legal-footer mt-5 pt-4 border-top text-center text-muted small">
                            <p>Documento generado automáticamente por el Sistema de Gestión CGI-UTC.<br/>
                            Información válida para procesos internos de la Dirección de Investigación.</p>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalPapelera" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content utc-modal-premium">
            
                <div class="modal-header papelera-header-premium text-white">
                    <div class="d-flex align-items-center justify-content-between w-100">
                        <div class="d-flex align-items-center">
                            <div class="bg-white text-primary rounded-4 p-3 me-4 shadow-lg">
                                <i class="fa-solid fa-box-archive fs-3"></i>
                            </div>
                            <div>
                                <h4 class="fw-bold mb-0">Papelera</h4>
                                <p class="mb-0 text-white-50">Recuperación de registros de Docentes</p>
                            </div>
                        </div>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                </div>

                <div class="modal-body p-4 bg-light">
                    <div class="row">
                        <asp:Repeater ID="rptPapelera" runat="server" OnItemCommand="rptPapelera_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-6">
                                    <div class="docente-trash-card shadow-sm p-4 h-100">
                                        <div class="d-flex justify-content-between align-items-start mb-3">
                                            <div class="status-badge-inactive fw-bold">
                                                <i class="fa-solid fa-circle-exclamation me-1"></i> REGISTRO INACTIVO
                                            </div>
                                            <span class="text-muted small fw-mono"># <%# Eval("strId_doc") %></span>
                                        </div>

                                        <div class="d-flex align-items-center mb-4">
                                            <div class="avatar-utc rounded-circle bg-primary text-white d-flex align-items-center justify-content-center me-3 shadow-sm" style="width: 50px; height: 50px; font-size: 1.2rem;">
                                                <%# Eval("strApellidos_doc").ToString().Substring(0,1) %>
                                            </div>
                                            <div>
                                                <h5 class="mb-0 fw-bold text-dark"><%# Eval("NombreCompleto") %></h5>
                                                <p class="mb-0 text-primary small fw-semibold"><i class="fa-solid fa-id-card me-1"></i> <%# Eval("strCedula_doc") %></p>
                                            </div>
                                        </div>

                                        <div class="bg-light rounded-3 p-3 d-flex mb-4">
                                            <div class="data-grid-item flex-fill">
                                                <span class="label-mini">Facultad</span>
                                                <span class="value-bold"><%# Eval("NombreFacultad") %></span>
                                            </div>
                                            <div class="data-grid-item flex-fill">
                                                <span class="label-mini">Categoría</span>
                                                <span class="value-bold text-primary"><%# string.IsNullOrEmpty(Eval("NombreCategoria")?.ToString()) ? "Pte. Asignar" : Eval("NombreCategoria") %></span>
                                            </div>
                                        </div>

                                        <div class="mt-auto d-grid">
                                            <asp:LinkButton runat="server" CommandName="restaurar" CommandArgument='<%# Eval("strId_doc") %>'
                                                CssClass="btn btn-primary rounded-pill py-2 fw-bold shadow-sm">
                                                <i class="fa-solid fa-rotate-left me-2"></i> REINTEGRAR AL SISTEMA
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Panel Visible='<%# rptPapelera.Items.Count == 0 %>' runat="server" CssClass="w-100 py-5 text-center">
                                    <div class="bg-white rounded-5 p-5 shadow-sm d-inline-block mx-auto">
                                        <i class="fa-solid fa-leaf fa-4x text-success opacity-25 mb-3"></i>
                                        <h5 class="text-muted fw-bold">¡Todo está en orden!</h5>
                                        <p class="text-muted mb-0">No hay registros inactivos para procesar.</p>
                                    </div>
                                </asp:Panel>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="modal-footer bg-white border-0 py-3 px-4">
                    <button type="button" class="btn btn-light px-4 rounded-pill fw-bold text-muted" data-bs-dismiss="modal">SALIR</button>
                </div>
            </div>
        </div>
    </div>

    <%-- SCRIPTS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#tablaDocentes').DataTable({
                responsive: true,
                language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
            });
        });

        Sys.Application.add_load(function () {
            if (typeof UTC_FileInput === 'function') {
                initFileInput('wrapperCertificado', '<%= flpCertificado.ClientID %>');
            }
        });

        function initFileInput(wrapperId, inputId) {
            var wrapper = document.getElementById(wrapperId);
            if (wrapper) {
                UTC_FileInput({
                    wrapper: wrapperId,
                    dropzone: wrapperId.replace('wrapper', 'dropzone'),
                    preview: wrapperId.replace('wrapper', 'preview'),
                    loader: wrapperId.replace('wrapper', 'loader'),
                    input: inputId
                });
            }
        }

        function imprimirReporte() {
            var contenido = document.getElementById("arealmpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');

            ventana.document.write('<html><head><title>Ficha de Categorización</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');

            ventana.document.write('<style>');
            ventana.document.write('body { font-family: "Segoe UI", sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; margin: 0; }');
            ventana.document.write('.report-paper { padding: 40px 50px; background: white; }');
            ventana.document.write('.header-hero-banner { background-color: #003876 !important; color: white !important; margin: -40px -50px 40px -50px; padding: 30px; text-align: center; border-bottom: 6px solid #002a5c; display: block; }');
            ventana.document.write('.header-hero-banner img { height: 70px; width: auto; filter: brightness(0) invert(1); }');
            ventana.document.write('.doc-title { color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase; }');
            ventana.document.write('.researcher-card { background-color: #f8faff !important; border-left: 5px solid #003876 !important; padding: 20px; margin-bottom: 40px; border: 1px solid #e1e8f0; border-radius: 6px; }');
            ventana.document.write('.timeline-list { list-style: none; padding: 0; position: relative; margin-left: 10px; border-left: 2px solid #e9ecef; }');
            ventana.document.write('.timeline-item { position: relative; padding-left: 30px; margin-bottom: 30px; }');
            ventana.document.write('.timeline-marker { position: absolute; left: -9px; top: 0; width: 16px; height: 16px; border-radius: 50%; background: #fff; border: 3px solid #003876; z-index: 2; }');
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