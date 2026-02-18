<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CalificacionGruInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CalificacionGruInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- RECURSOS UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-calificacion-informe.css" rel="stylesheet" />

    <%-- HEADER PRINCIPAL --%>
    <div id="headerCalificacion" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-star me-2"></i> CALIFICACIÓN DE GRUPOS
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevaCalif" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevaCalif_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVA CALIFICACIÓN
            </asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL FILTROS --%>
    <asp:Panel ID="pnlFiltros" runat="server" Visible="true">
        <div class="bg-white p-3 mb-3 rounded shadow-utc border">
            <div class="row align-items-center">
                <div class="col-md-6 d-flex align-items-center gap-2">
                    <label class="fw-bold text-secondary">FILTRAR POR AÑO:</label>
                    <asp:DropDownList ID="ddlFiltroAnio" runat="server" AutoPostBack="true" CssClass="form-select w-auto" OnSelectedIndexChanged="ddlFiltroAnio_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-6 text-md-end mt-3 mt-md-0">
                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-pill" onclick="AbrirModalMetricas()">
                        <i class="fa-solid fa-sliders me-1"></i> Configurar Métricas
                    </button>
                </div>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL GRILLA --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCalificaciones" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th> <th>GRUPO</th> <th>PUNTAJE</th> <th>CATEGORÍA</th> 
                        <th>AÑO</th> <th>FECHA EVAL.</th> <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCalificaciones" runat="server" OnItemCommand="rptCalificaciones_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_valo") %></td>
                                <td class="text-start fw-bold text-primary"><%# Eval("NombreGrupo") %></td>
                                <td><span class="badge bg-secondary fs-6"><%# Eval("intPuntaje_valo") %></span></td>
                                <td>
                                    <%# 
                                        Eval("strCategoria_valo").ToString() == "CONSOLIDADO" ? "<span class='badge bg-success'>CONSOLIDADO</span>" : 
                                        (Eval("strCategoria_valo").ToString() == "EMERGENTE" ? "<span class='badge bg-warning text-dark'>EMERGENTE</span>" : 
                                        "<span class='badge bg-danger'>DISUELTO</span>")
                                    %>
                                </td>
                                <td><%# Eval("intAnioMetrica") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFecha_valo")).ToString("dd/MM/yyyy") %></td>
                                <td>
                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_valo") %>' CssClass="btn btn-warning btn-sm rounded-circle me-1 text-white"><i class="fa-solid fa-pen"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnVer" runat="server" CommandName="Ver" CommandArgument='<%# Eval("strId_valo") %>' CssClass="btn btn-ver btn-sm rounded-circle me-1" ToolTip="Ver Informe"><i class="fa-solid fa-eye"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_valo") %>' CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirmarEliminar(this, '¿Está seguro de eliminar este proyecto? Esta acción no se puede deshacer.');" ToolTip="Eliminar"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- PANEL FORMULARIO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-star me-2"></i> CALIFICACIÓN DE GRUPOS</h3>
            <asp:LinkButton ID="btnCancelarFormTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Calificación</h4>

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Grupo de Investigación <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlGrupoAdd" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Fecha Evaluación <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtFechaAdd" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-4">
                    <label class="form-label fw-bold text-primary">Año de la Métrica</label>
                    <asp:DropDownList ID="ddlAnioMetricaSeleccion" runat="server" CssClass="form-select border-primary"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlAnioMetricaSeleccion_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label class="form-label">Puntaje Obtenido</label>
                    <asp:TextBox ID="txtPuntajeAdd" runat="server" CssClass="form-control" TextMode="Number" placeholder="Pendiente..." />
                    <div class="form-text text-muted" style="font-size: 0.75rem;">
                        * Si ingresa puntaje, la Resolución es obligatoria.
                    </div>
                </div>

                <div class="col-12">
                    <div class="bg-light p-3 rounded-3 border shadow-sm">
                        <div class="d-flex align-items-center mb-3">
                            <div class="bg-primary bg-opacity-10 p-2 rounded-circle me-2 text-primary">
                                <i class="fa-solid fa-scale-balanced"></i>
                            </div>
                            <h6 class="mb-0 fw-bold text-primary">Criterios de Evaluación Aplicados</h6>
                        </div>
        
                        <asp:Label ID="lblReglaMetrica" runat="server" CssClass="d-block w-100"></asp:Label>
                    </div>
                </div>

                <div class="col-12">
                    <label class="form-label">Reconocimiento / Observación</label>
                    <asp:TextBox ID="txtReconocimientoAdd" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>

                <div class="col-12 mt-4">
                    <label class="form-label fw-bold mb-2">1. Informe de Evaluación (PDF) <span class="text-danger">*</span></label>
                 
                    <div id="pnlInfoEvaluacion" runat="server" class="d-flex align-items-center justify-content-between p-3 border rounded shadow-sm bg-white" visible="false">
                        <div class="d-flex align-items-center">
                            <div class="me-3 bg-light rounded p-2 text-danger"><i class="fa-solid fa-file-pdf fa-2x"></i></div>
                            <div>
                                <h6 class="mb-0 fw-bold text-dark">Documento Cargado</h6>
                                <asp:HyperLink ID="lnkVerEvaluacionActual" runat="server" Target="_blank" CssClass="small text-primary text-decoration-none">Ver PDF</asp:HyperLink>
                            </div>
                        </div>
                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-pill px-3" onclick="activarEdicionArchivo('Evaluacion')">
                            <i class="fa-solid fa-pen-to-square me-2"></i> Cambiar
                        </button>
                    </div>

                    <div id="pnlUploadEvaluacion" runat="server">
                        <div id="btnCancelarEdicionEvaluacion" class="text-end mb-1" style="display:none;">
                            <button type="button" class="btn btn-sm text-muted" onclick="cancelarEdicionArchivo('Evaluacion')"><i class="fa-solid fa-xmark"></i> Cancelar</button>
                        </div>
                        <div class="utc-fileinput-wrapper" id="wrapperArchivoAdd">
                            <div class="utc-fileinput-header">
                                <div class="utc-fileinput-icon"><i class="fa-solid fa-file-contract"></i></div>
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
                            <div class="utc-dropzone" id="dropzoneArchivoAdd"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra el PDF aquí</div>
                            <asp:FileUpload ID="flpArchivoAdd" runat="server" CssClass="utc-fileinput-input" 
                                AllowMultiple="true" 
                                accept=".pdf,.doc,.docx,.zip,.rar" />
                        </div>
                    </div>
                </div>

                <div class="col-12 mt-3" id="contenedorResolucionTotal" runat="server" visible="false">
                    <label class="form-label fw-bold mb-2 text-success">2. Documento de Resolución (PDF)</label>
                 
                    <div id="pnlInfoResolucion" runat="server" class="d-flex align-items-center justify-content-between p-3 border rounded shadow-sm bg-white" visible="false">
                        <div class="d-flex align-items-center">
                            <div class="me-3 bg-light rounded p-2 text-success"><i class="fa-solid fa-gavel fa-2x"></i></div>
                            <div>
                                <h6 class="mb-0 fw-bold text-dark">Resolución Cargada</h6>
                                <asp:HyperLink ID="lnkVerResolucionActual" runat="server" Target="_blank" CssClass="small text-success text-decoration-none">Ver PDF</asp:HyperLink>
                            </div>
                        </div>
                        <button type="button" class="btn btn-sm btn-outline-secondary rounded-pill px-3" onclick="activarEdicionArchivo('Resolucion')">
                            <i class="fa-solid fa-pen-to-square me-2"></i> Cambiar
                        </button>
                    </div>

                    <div id="pnlUploadResolucion" runat="server">
                        <div id="btnCancelarEdicionResolucion" class="text-end mb-1" style="display:none;">
                            <button type="button" class="btn btn-sm text-muted" onclick="cancelarEdicionArchivo('Resolucion')"><i class="fa-solid fa-xmark"></i> Cancelar</button>
                        </div>
                        <div class="utc-fileinput-wrapper" id="wrapperResolucion">
                            <div class="utc-fileinput-header">
                                <div class="utc-fileinput-icon"><i class="fa-solid fa-gavel"></i></div>
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <span class="utc-fileinput-name">Sin archivo</span>
                                    <div class="utc-fileinput-buttons d-flex gap-2">
                                        <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                        <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                    </div>
                                </div>
                            </div>
                            <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
                            <div class="utc-fileinput-preview" id="previewResolucion"></div>
                            <div class="utc-fileinput-loader" id="loaderResolucion"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                            <div class="utc-dropzone" id="dropzoneResolucion"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-success"></i><br />Arrastra la Resolución aquí</div>
                            <asp:FileUpload ID="flpResolucion" runat="server" CssClass="utc-fileinput-input" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5" OnClientClick="return ValidarCalificacion();" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Todo
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarForm" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL MÉTRICAS ACTUALIZADO (Paso clave) --%>
    <div class="modal fade" id="modalMetricas" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-sliders me-2"></i> Configurar Métricas</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label fw-bold">Año a configurar:</label>
                        <asp:DropDownList ID="ddlAnioMetricas" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>

                    <div class="row g-3 mb-3">
                        <div class="col-md-6">
                            <label class="form-label text-success fw-bold">
                                <i class="fa-solid fa-circle-check me-1"></i> Mínimo CONSOLIDADO
                            </label>
                            <asp:TextBox ID="txtMinConsolidado" runat="server" CssClass="form-control text-center fw-bold" TextMode="Number" placeholder="Ej: 80"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label text-warning fw-bold">
                                <i class="fa-solid fa-triangle-exclamation me-1"></i> Mínimo EMERGENTE
                            </label>
                            <asp:TextBox ID="txtMinEmergente" runat="server" CssClass="form-control text-center fw-bold" TextMode="Number" placeholder="Ej: 60"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="alert alert-secondary py-2 small">
                        <i class="fa-solid fa-info-circle me-1"></i> Regla: Puntajes inferiores al mínimo emergente (o 0) serán <strong>DISUELTO</strong>.
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnGuardarMetricas" runat="server" 
                        CssClass="btn btn-primary btn-pill px-4" 
                        OnClientClick="return ValidarMetricas();"
                        OnClick="btnGuardarMetricas_Click">
                        Guardar Configuración
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <%-- MODAL ARCHIVOS (Sin Cambios) --%>
    <div class="modal fade" id="modalArchivos" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg"> <div class="modal-content border-0 shadow-lg rounded-4">
            
                <div class="modal-header modal-header-utc border-0">
                    <div class="d-flex align-items-center">
                        <div class="bg-white bg-opacity-25 p-2 rounded-circle me-3">
                            <i class="fa-solid fa-folder-tree text-white"></i>
                        </div>
                        <div>
                            <h5 class="modal-title fw-bold mb-0">Documentación del Grupo</h5>
                            <small class="text-white-50" style="font-size: 0.85rem;">Visualización y Descarga de Evidencias</small>
                        </div>
                    </div>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body bg-light p-4">
                    <div class="row g-4">
                    
                        <div class="col-md-6">
                            <div class="card h-100 file-card rounded-4 p-3 shadow-sm">
                                <div class="card-body text-center">
                                    <span class="position-absolute top-0 start-50 translate-middle-x badge bg-primary bg-opacity-10 text-primary mt-3 rounded-pill px-3">
                                        Requisito Obligatorio
                                    </span>

                                    <div class="mx-auto mt-4 mb-3">
                                        <div id="iconInforme" runat="server" class="file-icon-box mx-auto shadow-sm">
                                        </div>
                                    </div>

                                    <h6 class="fw-bold text-dark mb-1">Informe de Evaluación</h6>
                                    <div id="divEstadoInforme" runat="server" class="mb-4">
                                    </div>

                                    <div class="d-grid">
                                        <asp:HyperLink ID="lnkDescargarInforme" runat="server" Target="_blank" CssClass="btn btn-outline-primary btn-pill fw-bold">
                                        </asp:HyperLink>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="card h-100 file-card rounded-4 p-3 shadow-sm">
                                <div class="card-body text-center">
                                    <span class="position-absolute top-0 start-50 translate-middle-x badge bg-success bg-opacity-10 text-success mt-3 rounded-pill px-3">
                                        Respaldo Legal
                                    </span>

                                    <div class="mx-auto mt-4 mb-3">
                                        <div id="iconResolucion" runat="server" class="file-icon-box mx-auto shadow-sm">
                                        </div>
                                    </div>

                                    <h6 class="fw-bold text-dark mb-1">Resolución Final</h6>
                                    <div id="divEstadoResolucion" runat="server" class="mb-4">
                                    </div>

                                    <div class="d-grid">
                                        <asp:HyperLink ID="lnkDescargarResolucion" runat="server" Target="_blank" CssClass="btn btn-outline-success btn-pill fw-bold">
                                        </asp:HyperLink>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            
                <div class="modal-footer border-0 bg-light justify-content-center pb-4">
                    <button type="button" class="btn btn-secondary btn-pill px-5 shadow-sm" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i> Cerrar Ventana
                    </button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function AbrirModalArchivos() {
            var el = document.getElementById('modalArchivos');
            if (el) {
                var modal = new bootstrap.Modal(el);
                modal.show();
            }
        }
    </script>

    <%-- SCRIPTS OPTIMIZADOS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        function activarEdicionArchivo(tipo) {
            var pnlInfo = document.querySelector('[id$="pnlInfo' + tipo + '"]');
            var pnlUpload = document.querySelector('[id$="pnlUpload' + tipo + '"]');
            var btnCancel = document.getElementById('btnCancelarEdicion' + tipo);

            if (pnlInfo) pnlInfo.style.display = 'none';
            if (pnlUpload) pnlUpload.style.display = 'block';
            if (btnCancel) btnCancel.style.display = 'block';
        }

        function cancelarEdicionArchivo(tipo) {
            var pnlInfo = document.querySelector('[id$="pnlInfo' + tipo + '"]');
            var pnlUpload = document.querySelector('[id$="pnlUpload' + tipo + '"]');

            if (pnlInfo) pnlInfo.style.display = 'flex';
            if (pnlUpload) pnlUpload.style.display = 'none';

            var fileInput = pnlUpload.querySelector('input[type="file"]');
            if (fileInput) fileInput.value = '';
        }

        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        Sys.Application.add_load(function () {

            const tabla = '#tablaCalificaciones';
            if ($.fn.DataTable && $.fn.DataTable.isDataTable(tabla)) {
                $(tabla).DataTable().destroy();
            }
            if ($(tabla).length) {
                $(tabla).DataTable(dtConfig);
            }

            var wrapperAdd = document.getElementById('wrapperArchivoAdd');
            if (wrapperAdd && typeof UTC_FileInput === 'function') {
                UTC_FileInput({
                    wrapper: 'wrapperArchivoAdd',
                    dropzone: 'dropzoneArchivoAdd',
                    preview: 'previewArchivoAdd',
                    loader: 'loaderArchivoAdd',
                    input: '<%= flpArchivoAdd.ClientID %>'
                });
            }

            var wrapperRes = document.getElementById('wrapperResolucion');
            if (wrapperRes && typeof UTC_FileInput === 'function') {
                UTC_FileInput({
                    wrapper: 'wrapperResolucion',
                    dropzone: 'dropzoneResolucion',
                    preview: 'previewResolucion',
                    loader: 'loaderResolucion',
                    input: '<%= flpResolucion.ClientID %>'
            });
            }

            var inputAdd = document.getElementById('<%= flpArchivoAdd.ClientID %>');
        if (inputAdd) inputAdd.setAttribute("multiple", "");

        var txtPuntaje = document.getElementById('<%= txtPuntajeAdd.ClientID %>');
        var divResolucion = document.getElementById('<%= contenedorResolucionTotal.ClientID %>');

        if (txtPuntaje && divResolucion) {

            var evaluarVisibilidad = function () {
                if (txtPuntaje.value.trim() !== "") {
                    if (divResolucion.style.display === 'none') $(divResolucion).slideDown();
                } else {
                    var pnlInfo = document.getElementById('<%= pnlInfoResolucion.ClientID %>');
                    if (!pnlInfo || pnlInfo.style.display === 'none') {
                        $(divResolucion).slideUp();
                    }
                }
            };

            txtPuntaje.addEventListener('input', evaluarVisibilidad);

                evaluarVisibilidad();
            }

        }); 

        function AbrirModalMetricas() {
            var el = document.getElementById('modalMetricas');
            if (el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }
    </script>

    <script type="text/javascript">
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
                campo.addEventListener('input', function () {
                    this.classList.remove('is-invalid');
                }, { once: true });
            }
        }

        function ValidarCalificacion() {
            var idGrupo = '<%= ddlGrupoAdd.ClientID %>';
            var idFecha = '<%= txtFechaAdd.ClientID %>';
            var idPuntaje = '<%= txtPuntajeAdd.ClientID %>';
   
            var grupo = document.getElementById(idGrupo);
            if (!grupo || grupo.value === "" || grupo.value === "0") { mostrarError(idGrupo, 'Seleccione Grupo.'); return false; }
   
            var fecha = document.getElementById(idFecha);
            if (!fecha || fecha.value === "") { mostrarError(idFecha, 'Ingrese Fecha.'); return false; }

            var inputPuntaje = document.getElementById(idPuntaje);
            var tienePuntaje = inputPuntaje && inputPuntaje.value.trim() !== ""; 

            var linkResolucion = document.getElementById('<%= lnkVerResolucionActual.ClientID %>');
            var existeResolucionVieja = linkResolucion && linkResolucion.offsetParent !== null;
   
            var inputResFile = document.getElementById('<%= flpResolucion.ClientID %>');
            var subiendoResolucionNueva = inputResFile && inputResFile.files.length > 0;

            var tieneResolucion = existeResolucionVieja || subiendoResolucionNueva;

            if (tienePuntaje && !tieneResolucion) {
                mostrarError(idPuntaje, 'Si ingresa puntaje, DEBE subir la Resolución.');
                return false;
            }

            if (!tienePuntaje && subiendoResolucionNueva) {
                mostrarError(idPuntaje, 'Si sube resolución, DEBE ingresar el Puntaje.');
                return false;
            }
   
            if (tienePuntaje) {
                 var val = parseFloat(inputPuntaje.value);
                 if (isNaN(val) || val < 0 || val > 100) {
                     mostrarError(idPuntaje, 'El puntaje debe ser entre 0 y 100.');
                     return false;
                 }
            }

            return true;
        }

        function ValidarMetricas() {
            var idMinCons = '<%= txtMinConsolidado.ClientID %>';
            var idMinEmer = '<%= txtMinEmergente.ClientID %>'; // Nuevo campo

            var inputCons = document.getElementById(idMinCons);
            var inputEmer = document.getElementById(idMinEmer);

            if (inputCons && inputCons.value.trim() === "") {
                mostrarError(idMinCons, 'Ingrese el puntaje para CONSOLIDADO.');
                return false;
            }

            if (inputEmer && inputEmer.value.trim() === "") {
                mostrarError(idMinEmer, 'Ingrese el puntaje para EMERGENTE.');
                return false;
            }

            var valCons = parseFloat(inputCons.value);
            var valEmer = parseFloat(inputEmer.value);

            if (isNaN(valCons) || valCons < 0 || valCons > 100) {
                mostrarError(idMinCons, 'El puntaje debe estar entre 0 y 100.');
                return false;
            }

            if (isNaN(valEmer) || valEmer < 0 || valEmer > 100) {
                mostrarError(idMinEmer, 'El puntaje debe estar entre 0 y 100.');
                return false;
            }

            if (valEmer >= valCons) {
                mostrarError(idMinEmer, 'El mínimo emergente debe ser MENOR que el consolidado.');
                return false;
            }

            return true;
        }
    </script>

</asp:Content>