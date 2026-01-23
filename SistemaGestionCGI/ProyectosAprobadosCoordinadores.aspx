<%@ Page Title="Mis Proyectos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ProyectosAprobadosCoordinadores.aspx.cs" Inherits="SistemaGestionCGI.ProyectosAprobadosCoordinadores" %>
<%@ Register Src="~/GeneradorInforme.ascx" TagPrefix="uc" TagName="GeneradorInforme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Tus estilos van en archivo aparte. Aquí solo links institucionales si ya los usas --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-informes.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/proyectos-pro-coordinador.css" rel="stylesheet" />

    <%-- ENCABEZADO --%>
    <div class="utc-hero p-3 p-md-4 mb-4 d-flex justify-content-between align-items-start flex-wrap gap-3">
        <div>
            <h3 class="utc-title mb-1"><i class="fa-solid fa-layer-group me-2"></i> GESTIÓN DE PROYECTOS</h3>
            <div class="text-muted fw-semibold">Panel de Control para Coordinadores</div>
        </div>
        <span class="utc-chip"><i class="fa-solid fa-user-tie"></i> Rol: Coordinador</span>
    </div>

    <%-- LISTADO DE PROYECTOS --%>
    <asp:Panel ID="pnlListadoTarjetas" runat="server">
        <div id="contenedorProyectos" class="row g-4 justify-content-start">
            <asp:Repeater ID="rptProyectosCoordinador" runat="server"
                OnItemCommand="rptProyectosCoordinador_ItemCommand"
                OnItemDataBound="rptProyectosCoordinador_ItemDataBound">

                <ItemTemplate>
                    <div class="col-12 col-md-6 col-xl-4 project-col">
                        <div class="card project-card border-0 d-flex flex-column">

                            <%-- Top: Estado + ID --%>
                            <div class="project-top">
                                <span class="project-badge">
                                    <i class="fa-solid fa-circle-info"></i> <%# Eval("strEstado_ejec") %>
                                </span>
                                <div class="project-id">ID: <%# Eval("strId_ejec") %></div>
                            </div>

                            <%-- Título --%>
                            <h5 class="project-title"><%# Eval("TituloProyecto") %></h5>

                            <%-- Barra resumen (más informativa y compacta) --%>
                            <div class="metric-row">
                                <div class="metric-box">
                                    <div class="metric-item">
                                        <span class="metric-label"><i class="fa-solid fa-tag me-1"></i> Periodo</span><br />
                                        <span class="metric-value"><%# Eval("strPeriodo_ejec") %></span>
                                    </div>

                                    <div class="metric-item">
                                        <span class="metric-label"><i class="fa-solid fa-calendar-day me-1"></i> Inicio</span><br />
                                        <span class="metric-value"><%# Convert.ToDateTime(Eval("dtFechaini_ejec")).ToString("MMM yyyy").ToUpper() %></span>
                                    </div>

                                    <div class="metric-item">
                                        <span class="metric-label"><i class="fa-solid fa-file-invoice me-1"></i> Informes</span><br />
                                        <span class="metric-value"><%# Eval("CantidadInformes") %></span>
                                    </div>
                                </div>
                            </div>

                            <%-- Alerta de plazo (si aplica) --%>
                            <div class="utc-alert-slot">
                                <asp:Literal ID="litAlertaPlazo" runat="server"></asp:Literal>
                            </div>

                            <%-- Detalle informativo (no clickeable) --%>
                            <div class="project-detail">
                                <div class="project-detail-box">
                                    <div class="info-row mt-0">
                                        <span class="left"><i class="fa-solid fa-calendar-day"></i> Inicio exacto</span>
                                        <span class="right"><%# Convert.ToDateTime(Eval("dtFechaini_ejec")).ToString("dd MMM yyyy") %></span>
                                    </div>

                                    <div class="info-row">
                                        <span class="left"><i class="fa-regular fa-calendar"></i> Fin</span>
                                        <span class="right">
                                            <%# (Eval("dtFechafin_ejec") != DBNull.Value && Eval("dtFechafin_ejec") != null
                                                ? Convert.ToDateTime(Eval("dtFechafin_ejec")).ToString("dd MMM yyyy")
                                                : "No definido") %>
                                        </span>
                                    </div>

                                    <div class="info-row">
                                        <span class="left"><i class="fa-solid fa-circle-info"></i> Estado</span>
                                        <span class="right"><%# Eval("strEstado_ejec") %></span>
                                    </div>
                                </div>

                                <div class="project-detail-box">
                                    <div class="info-row mt-0">
                                        <span class="left"><i class="fa-solid fa-file-invoice"></i> Informes cargados</span>
                                        <span class="right"><%# Eval("CantidadInformes") %></span>
                                    </div>

                                    <div class="info-row">
                                        <span class="left"><i class="fa-solid fa-hashtag"></i> Ejecución</span>
                                        <span class="right"><%# Eval("strId_ejec") %></span>
                                    </div>

                                    <div class="info-row">
                                        <span class="left"><i class="fa-solid fa-user"></i> Perfil</span>
                                        <span class="right">Coordinador</span>
                                    </div>
                                </div>
                            </div>

                            <%-- ÚNICAS ACCIONES (solo botones) --%>
                            <div class="project-actions">
                                <asp:LinkButton ID="btnEquipo" runat="server"
                                    CommandName="Equipo" CommandArgument='<%# Eval("strId_ejec") %>'
                                    CssClass="btn btn-outline-primary btn-pill w-100">
                                    <i class="fa-solid fa-users"></i> Equipo
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnInformes" runat="server"
                                    CommandName="Informes" CommandArgument='<%# Eval("strId_ejec") %>'
                                    CssClass="btn btn-primary btn-pill w-100 text-white shadow-sm">
                                    <i class="fa-solid fa-folder-open"></i> Gestión
                                </asp:LinkButton>
                            </div>

                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <%-- PANEL EQUIPO (SIN CARD) --%>
    <asp:Panel ID="pnlEquipoListado" runat="server" Visible="false">
        <asp:HiddenField ID="hfIdEjecucionEquipo" runat="server" />

        <div class="utc-hero p-3 p-md-4 mb-3 d-flex justify-content-between align-items-start flex-wrap gap-3">
            <div>
                <h3 class="utc-title mb-1"><i class="fa-solid fa-users me-2"></i> EQUIPO DE TRABAJO</h3>
                <div class="text-muted fw-semibold">Listado de integrantes del proyecto</div>
            </div>

            <asp:LinkButton runat="server" ID="btnVolverTarjeta"
                CssClass="btn btn-outline-primary btn-pill px-4 w-auto"
                OnClick="btnVolverTarjeta_Click">
                <i class="fa-solid fa-arrow-left"></i> VOLVER
            </asp:LinkButton>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc border">
            <table id="tablaMiembros" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>CÉDULA</th>
                        <th>INTEGRANTE</th>
                        <th>ROL</th>
                        <th>FACULTAD / ORIGEN</th>
                        <th>ESTADO</th>
                    </tr>
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
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_miembro"))
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i> Activo</span>"
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i> Inactivo</span>" %>
                                </td>
                                <asp:LinkButton ID="btnEditarM" runat="server" Visible="false" />
                                <asp:LinkButton ID="btnToggleEstado" runat="server" Visible="false" />
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- MODAL INFORMES (REPOSITORIO) --%>
    <div class="modal fade" id="modalInformes" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 modal-utc-shell">

                <div class="modal-header modal-utc-header text-white py-3">
                    <h5 class="modal-title w-100 text-center fw-bold text-white">
                        <i class="fa-solid fa-folder-tree me-2"></i> REPOSITORIO DE PROYECTO
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body modal-utc-body p-4">
                    <asp:HiddenField ID="hfIdEjecucionInforme" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfIdInformeEdit" runat="server" ClientIDMode="Static" />

                    <div class="repo-summary p-3 p-md-4 mb-4 d-flex justify-content-between align-items-center flex-wrap gap-3">
                        <div>
                            <div class="fw-bold mb-0" style="color: var(--utc-azul);">Informes de Avance</div>
                            <div class="small text-muted fw-semibold">Documentación periódica del proyecto</div>
                        </div>

                        <div class="repo-actions">
                            <asp:LinkButton ID="btnAbrirGenerador" runat="server"
                                CssClass="btn btn-outline-utc btn-utc-modal"
                                OnClientClick="CerrarParaAbrirGenerador();" OnClick="btnAbrirGenerador_Click">
                                <i class="fa-solid fa-wand-magic-sparkles me-2"></i> Generar
                            </asp:LinkButton>

                            <button type="button" id="btnSubirEscaneado" runat="server"
                                class="btn btn-primary-utc btn-utc-modal text-white shadow-sm"
                                onclick="LimpiarYSubir()">
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Subir PDF
                            </button>
                        </div>
                    </div>

                    <div class="row g-3">
                        <asp:Repeater ID="rptInformes" runat="server" OnItemCommand="rptInformes_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-6">
                                    <div class="doc-tile d-flex align-items-center justify-content-between">
                
                                        <div class="d-flex align-items-center gap-3 flex-grow-1 overflow-hidden">
                                            <div class="doc-ico"><i class="fa-solid fa-file-pdf"></i></div>

                                            <div class="flex-grow-1 overflow-hidden">
                                                <p class="doc-title text-truncate mb-1"><%# Eval("strNombrePeriodo") %></p>
                                                <div class="doc-meta">
                                                    <i class="fa-regular fa-calendar me-1"></i>
                                                    <%# Convert.ToDateTime(Eval("dtFechaSubida")).ToString("dd MMM yyyy") %>
                                                </div>
                                            </div>
                                        </div>

                                        <%-- ACCIONES (EDITAR / ELIMINAR) --%>
                                        <div class="d-flex gap-2 ms-2 flex-shrink-0">
                                            <asp:LinkButton ID="btnEditarInf" runat="server"
                                                CommandName="EditarInforme"
                                                CommandArgument='<%# Eval("strId_informe") %>'
                                                CssClass="btn btn-outline-primary btn-sm btn-pill px-3"
                                                ToolTip="Editar">
                                                <i class="fa-solid fa-pen"></i>
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnEliminarInf" runat="server"
                                                CommandName="EliminarInforme"
                                                CommandArgument='<%# Eval("strId_informe") %>'
                                                CssClass="btn btn-outline-danger btn-sm btn-pill px-3"
                                                ToolTip="Eliminar"
                                                OnClientClick="return confirm('¿Deseas eliminar este informe?');">
                                                <i class="fa-solid fa-trash"></i>
                                            </asp:LinkButton>
                                        </div>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <%-- Documentación Cierre --%>
                    <asp:Panel ID="pnlFaseCierre" runat="server" Visible="false" CssClass="mt-4 pt-4 border-top">
                        <h6 class="text-secondary fw-bold mb-3 small text-uppercase">
                            <i class="fa-solid fa-flag-checkered me-2"></i> Documentación Final
                        </h6>

                        <div class="row g-3">
                            <div class="col-md-6">
                                <asp:HyperLink ID="lnkVerCierre" runat="server" Target="_blank" CssClass="d-block text-decoration-none">
                                    <div class="doc-tile">
                                        <div class="doc-ico"><i id="iconCierre" runat="server" class="fa-solid fa-file-contract"></i></div>
                                        <div class="flex-grow-1">
                                            <p class="doc-title mb-1">Informe de Cierre</p>
                                            <span id="lblEstadoCierre" runat="server" class="doc-meta">No disponible</span>
                                        </div>
                                        <div class="text-muted"><i class="fa-solid fa-arrow-up-right-from-square"></i></div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-md-6">
                                <asp:HyperLink ID="lnkVerFinal" runat="server" Target="_blank" CssClass="d-block text-decoration-none">
                                    <div class="doc-tile">
                                        <div class="doc-ico"><i id="iconFinal" runat="server" class="fa-solid fa-award"></i></div>
                                        <div class="flex-grow-1">
                                            <p class="doc-title mb-1">Informe Final</p>
                                            <span id="lblEstadoFinal" runat="server" class="doc-meta">No finalizado</span>
                                        </div>
                                        <div class="text-muted"><i class="fa-solid fa-arrow-up-right-from-square"></i></div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>

    <%-- MODAL SUBIR PDF --%>
    <div class="modal fade" id="modalSubirInforme" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-lg rounded-4 border-0 modal-utc-shell">
                <div class="modal-header modal-utc-header text-white py-2">
                    <h6 class="modal-title fw-bold text-white" id="lblTituloModalInforme" runat="server">Cargar Documento</h6>
                    <button type="button" class="btn-close btn-close-white" onclick="CerrarSubModalUpload()"></button>
                </div>

                <div class="modal-body modal-utc-body p-4">
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
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn" aria-label="Renombrar archivo">
                                        <i class="fa-solid fa-pen"></i>
                                    </button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn" aria-label="Quitar archivo">
                                        <i class="fa-solid fa-xmark"></i>
                                    </button>
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

    <%-- Paneles ocultos requeridos por lógica original (legacy) --%>
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
        function CerrarParaAbrirGenerador() {
            var modalEl = document.getElementById('modalInformes');
            var instance = bootstrap.Modal.getInstance(modalEl);
            if (instance) instance.hide();
        }

        function AbrirModalInformes() {
            bootstrap.Modal.getOrCreateInstance(document.getElementById('modalInformes')).show();
        }

        function AbrirSubModalUpload() {
            bootstrap.Modal.getOrCreateInstance(document.getElementById('modalSubirInforme')).show();
        }

        function CerrarSubModalUpload() {
            var modal = bootstrap.Modal.getInstance(document.getElementById('modalSubirInforme'));
            if (modal) modal.hide();
        }

        function LimpiarYSubir() {
            document.getElementById('<%= hfIdInformeEdit.ClientID %>').value = "";
            document.getElementById('<%= txtNombrePeriodoInf.ClientID %>').value = "";
            AbrirSubModalUpload();
        }

        Sys.Application.add_load(function () {

            // Layout: si solo hay 1 proyecto -> centra y ensancha
            var contenedor = document.getElementById("contenedorProyectos");
            if (contenedor) {
                var cards = contenedor.querySelectorAll(".project-col");
                if (cards.length === 1) contenedor.classList.add("single-project-layout");
            }

            if ($('#tablaMiembros').length) {
                if ($.fn.DataTable.isDataTable('#tablaMiembros')) $('#tablaMiembros').DataTable().destroy();
                $('#tablaMiembros').DataTable({
                    responsive: true,
                    autoWidth: false,
                    pageLength: 10,
                    language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                    dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +
                         "<'row'<'col-sm-12'tr>>" +
                         "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
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
