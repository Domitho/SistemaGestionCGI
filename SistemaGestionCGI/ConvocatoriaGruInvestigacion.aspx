<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConvocatoriaGruInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.ConvocatoriaGruInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- RECURSOS UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />

    <%-- ESTILOS LOCALES --%>
    <style>
        .form-stack { max-width: 100% !important; }
        textarea.form-control { resize: vertical; min-height: 100px; }
        .col-desc { width: 40%; }
        .modal-header.bg-dark { border-bottom: 0; }
    </style>

    <%-- HEADER PRINCIPAL --%>
    <div id="headerConvocatoria" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-bullhorn me-2"></i> 
            <asp:Label ID="lblTituloPrincipal" runat="server" Text="CONVOCATORIAS DE INVESTIGACIÓN"></asp:Label>
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="lbtNuevaConv" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="lbtNuevaConv_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVA CONVOCATORIA
            </asp:LinkButton>
            
            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: LISTADO --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaConvocatorias" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>NOMBRE</th>
                        <th class="col-desc">DESCRIPCIÓN</th>
                        <th>FECHA PUBLICACIÓN</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptConvocatorias" runat="server" OnItemCommand="rptConvocatorias_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_conv") %></td>
                                <td class="text-start fw-bold text-primary"><%# Eval("strNombre_conv") %></td>
                                <td class="text-start small text-muted"><%# HttpUtility.HtmlDecode(Eval("strDescripcion_conv").ToString()) %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_conv")).ToString("dd/MM/yyyy") %></td>
                                <td>
                                    <asp:LinkButton ID="btnVerArchivo" runat="server" CommandName="VerArchivo" CommandArgument='<%# Eval("strId_conv") %>'
                                        CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" ToolTip="Ver Documento">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_conv") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_conv") %>' 
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Está seguro de eliminar esta convocatoria?');" ToolTip="Eliminar">
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

    <%-- PANEL 2: AGREGAR --%>
    <asp:Panel ID="pnlAgregar" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-circle-plus me-2"></i> Formulario de Registro
            </h4>
            
            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label fw-bold">Nombre de la Convocatoria</label>
                    <asp:TextBox ID="txtNombreAdd" runat="server" CssClass="form-control" autocomplete="off" />
                    <asp:RequiredFieldValidator ID="rfvNombreAdd" runat="server" ControlToValidate="txtNombreAdd" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
                </div>

                <div class="col-12"> 
                    <label class="form-label">Fecha de Publicación</label>
                    <asp:TextBox ID="txtFechaIniAdd" runat="server" CssClass="form-control" TextMode="Date" />
                    <asp:RequiredFieldValidator ID="rfvFechaIniAdd" runat="server" ControlToValidate="txtFechaIniAdd" ErrorMessage="Requerido" CssClass="text-danger small" Display="Dynamic" ValidationGroup="Guardar" />
                </div>

                <div class="col-12">
                    <label class="form-label">Descripción / Bases</label>
                    <asp:TextBox ID="txtDescAdd" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                </div>

                <div class="col-12">
                    <label class="form-label fw-semibold">Archivo de Bases (PDF/Excel)</label>
                    
                    <%-- UTC FILE INPUT ADD --%>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoAdd">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-pdf"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
   
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar archivo..." />
                        <div class="utc-fileinput-preview" id="previewArchivoAdd"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoAdd"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        
                        <div class="utc-dropzone" id="dropzoneArchivoAdd">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra archivo aquí
                        </div>
                        
                        <asp:FileUpload ID="flpArchivoAdd" runat="server" CssClass="utc-fileinput-input" accept=".pdf,.doc,.docx,.xls,.xlsx" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="lbtGuardar" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="lbtGuardar_Click" ValidationGroup="Guardar" OnClientClick="return validarPesoArchivo('Add');">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                
                <asp:LinkButton ID="lbtCancelar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbtCancelar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL 3: EDITAR --%>
    <asp:Panel ID="pnlEditar" runat="server" Visible="false">
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-pen-to-square me-2"></i> Edición de Datos
            </h4>
            
            <asp:HiddenField ID="hfIdConvEdit" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />

            <div class="row g-3">
                <div class="col-12">
                    <label class="form-label fw-bold">Nombre de la Convocatoria</label>
                    <asp:TextBox ID="txtNombreEdit" runat="server" CssClass="form-control" autocomplete="off" />
                </div>

                <div class="col-12">
                    <label class="form-label">Fecha de Publicación</label>
                    <asp:TextBox ID="txtFechaIniEdit" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-12">
                    <label class="form-label">Descripción / Bases</label>
                    <asp:TextBox ID="txtDescEdit" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                </div>

                <div class="col-12">
                    <label class="form-label fw-semibold">Reemplazar Archivo (Opcional)</label>
                    
                    <%-- UTC FILE INPUT EDIT --%>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoEdit">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-pdf"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo nuevo</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>

                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar archivo..." />
                        <div class="utc-fileinput-preview" id="previewArchivoEdit"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoEdit"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        
                        <div class="utc-dropzone" id="dropzoneArchivoEdit">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra para reemplazar
                        </div>
                        
                        <asp:FileUpload ID="flpArchivoEdit" runat="server" CssClass="utc-fileinput-input" accept=".pdf,.doc,.docx,.xls,.xlsx" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="lbtActualizar" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="lbtActualizar_Click" ValidationGroup="Editar" OnClientClick="return validarPesoArchivo('Edit');">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Actualizar
                </asp:LinkButton>
                
                <asp:LinkButton ID="lbtCancelarEdit" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbtCancelarEdit_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL VISTA PREVIA --%>
    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title">Vista Previa</h6>
                    <div>
                        <a id="btnDescargarDirecto" href="#" target="_blank" class="btn btn-sm btn-outline-light me-2">
                            <i class="fa-solid fa-download"></i> Descargar
                        </a>
                        <button type="button" class="btn-close btn-close-white" onclick="CerrarVistaPrevia()"></button>
                    </div>
                </div>
                <div class="modal-body p-0" style="height: 80vh; background:white;">
                    <iframe id="framePdf" class="pdf-viewer-frame" style="width:100%; height:100%; border:none;"></iframe>
                </div>
            </div>
        </div>
    </div>

    <%-- SCRIPTS OPTIMIZADOS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>

    <script type="text/javascript">
        
        // Configuración centralizada
        const dtConfig = {
            responsive: true,
            autoWidth: false,
            ordering: true,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        // Función segura para UpdatePanels
        Sys.Application.add_load(function () {
            
            // Inicializar DataTables
            const $table = $('#tablaConvocatorias');
            if ($table.length) {
                if ($.fn.DataTable.isDataTable('#tablaConvocatorias')) $table.DataTable().destroy();
                $table.DataTable(dtConfig);
            }

            // Inicializar FileInputs si existe la librería
            if (typeof UTC_FileInput === 'function') {
                initInput("wrapperArchivoAdd", "<%= flpArchivoAdd.ClientID %>");
                initInput("wrapperArchivoEdit", "<%= flpArchivoEdit.ClientID %>");
            }
        });

        // Helper reutilizable
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

        // Validación de Peso (8MB)
        function validarPesoArchivo(tipo) {
            var inputId = tipo === 'Add' ? '<%= flpArchivoAdd.ClientID %>' : '<%= flpArchivoEdit.ClientID %>';
            var input = document.getElementById(inputId);

            if (input && input.files && input.files[0]) {
                var peso = input.files[0].size;
                var limite = 8 * 1024 * 1024; // 8MB
                if (peso > limite) {
                    alert('El archivo supera los 8MB permitidos.');
                    input.value = ""; // Limpiar
                    return false;
                }
            }
            return true;
        }

        // Funciones Modal
        function VerPDF(url) {
            document.getElementById('framePdf').src = url;
            document.getElementById('btnDescargarDirecto').href = url;
            var myModal = new bootstrap.Modal(document.getElementById('modalVistaPrevia'));
            myModal.show();
        }

        function CerrarVistaPrevia() {
            var el = document.getElementById('modalVistaPrevia');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
            document.getElementById('framePdf').src = 'about:blank';
        }
    </script>

</asp:Content>