<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CalificacionGruInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CalificacionGruInvestigacion" %>

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

    <div id="headerCalificacion" runat="server" 
         class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-star me-2"></i> CALIFICACIÓN DE GRUPOS
        </h3>

        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevaCalif"
                CssClass="btn btn-primary btn-pill d-flex align-items-center"
                OnClick="btnNuevaCalif_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVA CALIFICACIÓN
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresar"
                CssClass="btn btn-outline-primary btn-pill px-4"
                OnClick="btnRegresar_Click"
                Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlFiltros" runat="server" Visible="true">
        <div class="bg-white p-3 mb-3 rounded shadow-utc border">
            <div class="row align-items-center">
                <div class="col-md-6 d-flex align-items-center gap-2">
                    <label class="fw-bold text-secondary">FILTRAR POR AÑO:</label>
                    <asp:DropDownList ID="ddlFiltroAnio" runat="server" AutoPostBack="true" 
                        CssClass="form-select w-auto" OnSelectedIndexChanged="ddlFiltroAnio_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="col-md-6 text-md-end mt-3 mt-md-0">
                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-pill" onclick="AbrirModalMetricas()">
                        <i class="fa-solid fa-sliders me-1"></i> Configurar Métricas
                    </button>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCalificaciones" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>GRUPO</th>
                        <th>PUNTAJE</th>
                        <th>CATEGORÍA</th>
                        <th>AÑO</th>
                        <th>FECHA EVAL.</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCalificaciones" runat="server" OnItemCommand="rptCalificaciones_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_valo") %></td>
                                <td class="text-start fw-bold text-primary"><%# Eval("NombreGrupo") %></td>
                                <td>
                                    <span class="badge bg-secondary fs-6"><%# Eval("intPuntaje_valo") %></span>
                                </td>
                                <td>
                                    <span class='badge <%# Eval("strCategoria_valo").ToString() == "CONSOLIDADO" ? "bg-success" : "bg-warning text-dark" %>'>
                                        <%# Eval("strCategoria_valo") %>
                                    </span>
                                </td>
                                <td><%# Eval("intAnioMetrica") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFecha_valo")).ToString("dd/MM/yyyy") %></td>
                                <td>
                                    <asp:LinkButton ID="btnVer" runat="server" CommandName="Ver" CommandArgument='<%# Eval("strInforme_valo") %>'
                                        CssClass="btn btn-ver btn-sm rounded-circle me-1" ToolTip="Ver Informe">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_valo") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle"
                                        OnClientClick="return confirm('¿Eliminar esta calificación?');" ToolTip="Eliminar">
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
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-star me-2"></i> CALIFICACIÓN DE GRUPOS</h3>
            <asp:LinkButton ID="btnCancelarFormTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Calificación</h4>

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label">Grupo de Investigación</label>
                    <asp:DropDownList ID="ddlGrupoAdd" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="col-md-4">
                    <label class="form-label">Fecha Evaluación</label>
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
                    <asp:TextBox ID="txtPuntajeAdd" runat="server" CssClass="form-control" TextMode="Number" />
                </div>

                <div class="col-12">
                    <div class="alert alert-info d-flex align-items-center shadow-sm" role="alert">
                        <i class="fa-solid fa-circle-info fa-2x me-3"></i>
                        <div>
                            <strong>Regla Aplicada:</strong>
                            <asp:Label ID="lblReglaMetrica" runat="server" Text="Seleccione un año para ver la regla."></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-12">
                    <label class="form-label">Reconocimiento / Observación</label>
                    <asp:TextBox ID="txtReconocimientoAdd" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="col-12">
                    <label class="form-label fw-semibold">Informe de Evaluación (PDF)</label>
                    
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
                        
                        <div class="utc-dropzone" id="dropzoneArchivoAdd">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />
                            Arrastra el PDF aquí o haz clic
                        </div>
                        
                        <asp:FileUpload ID="flpArchivoAdd" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Calificación
                </asp:LinkButton>

                <asp:LinkButton ID="btnCancelarForm" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                    OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalMetricas" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-sliders me-2"></i> Configurar Métricas</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label fw-bold">Año:</label>
                        <asp:DropDownList ID="ddlAnioMetricas" runat="server" CssClass="form-select"></asp:DropDownList>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Puntaje Mínimo para <strong>CONSOLIDADO</strong>:</label>
                        <asp:TextBox ID="txtMinConsolidado" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                        <div class="form-text">Menos de este puntaje será EMERGENTE.</div>
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnGuardarMetricas" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarMetricas_Click">
                        Guardar Configuración
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content border-0 rounded-4 shadow-lg" style="background: #333;">
                <div class="modal-header border-bottom-0 py-2 px-3">
                    <h6 class="modal-title text-white">Vista Previa</h6>
                    <button type="button" class="btn-close btn-close-white" onclick="CerrarVistaPrevia()"></button>
                </div>
                <div class="modal-body p-0" style="height: 85vh;">
                    <iframe id="framePdf" class="pdf-viewer-frame" style="width:100%; height:100%; border:none; background:white;"></iframe>
                </div>
            </div>
        </div>
    </div>

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        
        Sys.Application.add_load(function () {
            initTable('#tablaCalificaciones');

            var wrapper = document.getElementById('wrapperArchivoAdd');
            if (wrapper) {
                UTC_FileInput({
                    wrapper: 'wrapperArchivoAdd',
                    dropzone: 'dropzoneArchivoAdd',
                    preview: 'previewArchivoAdd',
                    loader: 'loaderArchivoAdd',
                    input: '<%= flpArchivoAdd.ClientID %>'
                });
            }
        });

        function initTable(id) {
            if ($.fn.DataTable && $.fn.DataTable.isDataTable(id)) $(id).DataTable().destroy();
            if ($(id).length) {
                $(id).DataTable({
                    responsive: true, autoWidth: false, pageLength: 10,
                    language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                    dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
                });
            }
        }

        function AbrirModalMetricas() {
            var el = document.getElementById('modalMetricas');
            if(el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }

        function VerPDF(url) {
            document.getElementById('framePdf').src = url;
            var el = document.getElementById('modalVistaPrevia');
            if(el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }

        function CerrarVistaPrevia() {
            var el = document.getElementById('modalVistaPrevia');
            var modal = bootstrap.Modal.getInstance(el);
            if(modal) modal.hide();
            document.getElementById('framePdf').src = '';
        }
    </script>

</asp:Content>