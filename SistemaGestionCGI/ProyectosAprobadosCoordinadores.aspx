<%@ Page Title="Mis Proyectos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProyectosAprobadosCoordinadores.aspx.cs" Inherits="SistemaGestionCGI.ProyectosAprobadosCoordinadores" %>

<%@ Register Src="~/GeneradorInforme.ascx" TagPrefix="uc" TagName="GeneradorInforme" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-informes.css" rel="stylesheet" />

    <style>
    .project-card .btn-primary {
        border: 2px solid var(--utc-azul) !important;
        background-clip: padding-box;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .project-card .btn-outline-primary {
        border: 2px solid var(--utc-azul) !important;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .project-card .btn {
        min-height: 38px; 
        line-height: 1.2;
    }
</style>

    <%-- 2. ENCABEZADO DE PÁGINA --%>
    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
        <div>
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-layer-group me-2"></i> GESTIÓN DE PROYECTOS
            </h3>
            <p class="utc-subtitle small mb-0 mt-1 text-muted">Panel de Control para Coordinadores</p>
        </div>
        <div class="d-none d-md-block text-end">
             <div class="d-flex align-items-center gap-3">
                <span class="badge bg-light text-secondary border px-3 py-2">Rol: Coordinador</span>
            </div>
        </div>
    </div>

    <%-- 3. LISTADO DE PROYECTOS (TARJETAS) --%>
    <asp:Panel ID="pnlListadoTarjetas" runat="server">
        <div class="row g-4">
            <asp:Repeater ID="rptProyectosCoordinador" runat="server" OnItemCommand="rptProyectosCoordinador_ItemCommand" OnItemDataBound="rptProyectosCoordinador_ItemDataBound">
                <ItemTemplate>
                    <div class="col-12 col-md-6 col-xl-4">
                        <%-- Tarjeta Estándar UTC --%>
                        <div class="card shadow-utc border-0 h-100 project-card">
                            <div class="card-header bg-transparent border-0 pt-4 px-4 pb-0">
                                <span class='badge rounded-pill px-3 py-2 <%# 
                                    Eval("strEstado_ejec").ToString() == "FINALIZADO" ? "bg-success" : 
                                    Eval("strEstado_ejec").ToString() == "EN REVISION" ? "bg-warning text-dark" : 
                                    Eval("strEstado_ejec").ToString() == "CIERRE APROBADO" ? "bg-secondary" : "bg-primary" %>'>
                                    <%# Eval("strEstado_ejec") %>
                                </span>
                            </div>
                            
                            <div class="card-body px-4 pt-2 pb-4 d-flex flex-column">
                                <h5 class="fw-bold text-dark my-3 card-title-pro" title='<%# Eval("TituloProyecto") %>'>
                                    <%# Eval("TituloProyecto") %>
                                </h5>

                                <%-- Metadatos del proyecto --%>
                                <div class="bg-light rounded p-3 mb-3 d-flex justify-content-between text-center small border card-meta">
                                    <div class="flex-fill">
                                        <strong class="d-block text-primary">Inicio</strong>
                                        <%# Convert.ToDateTime(Eval("dtFechaini_ejec")).ToString("MMM yyyy").ToUpper() %>
                                    </div>
                                    <div class="border-start mx-2"></div>
                                    <div class="flex-fill">
                                        <strong class="d-block text-primary">Ciclo</strong>
                                        <%# Eval("strPeriodo_ejec").ToString().Split('-')[0] %>
                                    </div>
                                    <div class="border-start mx-2"></div>
                                    <div class="flex-fill">
                                        <strong class="d-block text-primary">Informes</strong>
                                        <%# Eval("CantidadInformes") %>
                                    </div>
                                </div>

                                <div class="text-center mb-2">
                                    <asp:Literal ID="litAlertaPlazo" runat="server"></asp:Literal>
                                </div>

                                <div class="mt-auto d-flex gap-2 border-top pt-3">
                                    <%-- BOTÓN EQUIPO --%>
                                    <asp:LinkButton ID="btnEquipo" runat="server" CommandName="Equipo" 
                                        CommandArgument='<%# Eval("strId_ejec") %>' 
                                        CssClass="btn btn-outline-primary rounded-3 btn-sm w-50 fw-bold">
                                        <i class="fa-solid fa-users me-2"></i> Equipo
                                    </asp:LinkButton>
    
                                    <%-- BOTÓN GESTIÓN --%>
                                    <asp:LinkButton ID="btnInformes" runat="server" CommandName="Informes" 
                                        CommandArgument='<%# Eval("strId_ejec") %>' 
                                        CssClass="btn btn-primary rounded-3 btn-sm w-50 fw-bold shadow-sm">
                                        <i class="fa-solid fa-folder-open me-2"></i> Gestión
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Panel ID="pnlVacio" runat="server" Visible='<%# rptProyectosCoordinador.Items.Count == 0 %>'>
                        <div class="col-12 text-center py-5">
                            <div class="mb-3 text-muted opacity-25"><i class="fa-solid fa-folder-plus fa-5x"></i></div>
                            <h4 class="fw-bold text-secondary">Sin Proyectos Asignados</h4>
                            <p class="text-muted">No registra proyectos aprobados en ejecución actualmente.</p>
                        </div>
                    </asp:Panel>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <%-- 4. PANEL DE EQUIPO (TABLA ESTILO UTC) --%>
    <asp:Panel ID="pnlEquipoListado" runat="server" Visible="false">
        <asp:HiddenField ID="hfIdEjecucionEquipo" runat="server" />
        
        <div class="d-flex justify-content-between align-items-center mb-3 bg-white p-3 rounded shadow-utc border header-utc-line">
             <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> EQUIPO DE TRABAJO</h3>
             <div class="d-flex gap-2">
                 <asp:LinkButton runat="server" ID="btnAbrirFormMiembro" Visible="false" />
                 <asp:LinkButton runat="server" ID="btnVolverTarjeta" CssClass="btn btn-outline-primary btn-pill px-4 btn-sm" OnClick="btnVolverTarjeta_Click">
                     <i class="fa-solid fa-arrow-left me-2"></i> VOLVER AL PANEL
                 </asp:LinkButton>
             </div>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc">
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
                    <asp:Repeater ID="rptMiembros" runat="server" OnItemDataBound="rptMiembros_ItemDataBound" OnItemCommand="rptMiembros_ItemCommand">
                        <ItemTemplate>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_miembro")) ? "" : "table-secondary text-muted" %>'>
                                <td class="fw-bold text-secondary"><%# Eval("strCedula_miembro") %></td>
                                <td class="text-start">
                                    <div class="fw-bold text-primary"><%# Eval("strApellidos_miembro") %></div>
                                    <div class="small text-muted"><%# Eval("strNombres_miembro") %></div>
                                </td>
                                <td><span class="badge bg-light text-dark border fw-normal"><%# Eval("strRol_miembro") %></span></td>
                                <td class="small text-muted text-start"><%# Eval("strFacultad_miembro") == "EXTERNO" ? Eval("strEntidad_miembro") : Eval("strFacultad_miembro") %></td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_miembro")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i> Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i> Inactivo</span>" 
                                    %>
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

    <%-- Formulario Oculto Miembros (Lógica Original preservada) --%>
    <asp:Panel ID="pnlFormularioMiembro" runat="server" Visible="false">
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
    </asp:Panel>

    <%-- 5. MODAL INFORMES (REPOSITORIO) --%>
    <div class="modal fade" id="modalInformes" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content shadow-utc border-0 rounded-4">
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-folder-tree me-2"></i> Repositorio del Proyecto</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body bg-light px-4 py-4">
                    <asp:HiddenField ID="hfIdEjecucionInforme" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfIdInformeEdit" runat="server" ClientIDMode="Static" />

                    <%-- Toolbar Acciones --%>
                    <div class="bg-white p-3 rounded-3 shadow-sm mb-4 border d-flex justify-content-between align-items-center flex-wrap gap-3">
                         <div><h6 class="fw-bold m-0 text-primary">Informes de Avance</h6><small class="text-muted">Documentación periódica</small></div>
                         <div class="d-flex gap-2">
                            <asp:LinkButton ID="btnAbrirGenerador" runat="server" CssClass="btn btn-outline-primary btn-pill btn-sm px-3" OnClick="btnAbrirGenerador_Click">
                                <i class="fa-solid fa-wand-magic-sparkles me-1"></i> Generar
                            </asp:LinkButton>
                            
                            <%-- Botón para abrir Subida --%>
                            <button type="button" id="btnSubirEscaneado" runat="server" class="btn btn-primary btn-pill btn-sm px-3" onclick="LimpiarYSubir()">
                                <i class="fa-solid fa-cloud-arrow-up me-1"></i> Subir PDF
                            </button>
                         </div>
                    </div>

                    <%-- Grid de Archivos --%>
                    <div class="row g-3 mb-2">
                        <asp:Repeater ID="rptInformes" runat="server" OnItemCommand="rptInformes_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-6">
                                    <div class="doc-tile-pro position-relative bg-white border rounded p-3 d-flex align-items-center shadow-sm h-100">
                                        <div class="me-3 fs-3 text-danger"><i class="fa-solid fa-file-pdf"></i></div>
                                        <div class="flex-grow-1 overflow-hidden">
                                            <h6 class="text-truncate mb-1 fw-bold text-dark" title='<%# Eval("strNombrePeriodo") %>'><%# Eval("strNombrePeriodo") %></h6>
                                            <span class="d-block small text-muted"><i class="fa-regular fa-calendar me-1"></i> <%# Convert.ToDateTime(Eval("dtFechaSubida")).ToString("dd MMM yyyy") %></span>
                                        </div>
                                        
                                        <div class="dropdown position-absolute top-0 end-0 p-2">
                                            <button class="btn btn-sm btn-link text-muted p-0 text-decoration-none" type="button" data-bs-toggle="dropdown"><i class="fa-solid fa-ellipsis-vertical fs-5 px-2"></i></button>
                                            <ul class="dropdown-menu dropdown-menu-end shadow-lg border-0 rounded-3">
                                                <li><a class="dropdown-item small py-2" href='<%# ResolveUrl(Eval("strArchivo_path").ToString()) %>' target="_blank"><i class="fa-solid fa-download me-2 text-primary"></i> Descargar</a></li>
                                                <li><hr class="dropdown-divider my-1"></li>
                                                <li><asp:LinkButton ID="btnEditarInf" runat="server" CommandName="EditarInforme" CommandArgument='<%# Eval("strId_informe") %>' CssClass="dropdown-item small text-warning py-2"><i class="fa-solid fa-pen me-2"></i> Renombrar</asp:LinkButton></li>
                                                <li><asp:LinkButton ID="btnEliminarInf" runat="server" CommandName="EliminarInforme" CommandArgument='<%# Eval("strId_informe") %>' CssClass="dropdown-item small text-danger py-2" OnClientClick="return confirm('¿Eliminar archivo?');"><i class="fa-solid fa-trash me-2"></i> Eliminar</asp:LinkButton></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <%-- Documentación Cierre --%>
                    <asp:Panel ID="pnlFaseCierre" runat="server" Visible="false" CssClass="mt-4 pt-4 border-top">
                        <h6 class="text-secondary fw-bold mb-3 small text-uppercase" style="letter-spacing: 1px;"><i class="fa-solid fa-flag-checkered me-2"></i> Documentación de Cierre</h6>
                        <div class="row g-3">
                            <div class="col-md-6">
                                <asp:HyperLink ID="lnkVerCierre" runat="server" Target="_blank" CssClass="d-block text-decoration-none">
                                    <div class="p-3 bg-white border rounded d-flex align-items-center shadow-sm">
                                        <div class="me-3 fs-3 text-secondary" id="iconCierreBox" runat="server"><i id="iconCierre" runat="server" class="fa-solid fa-file-contract"></i></div>
                                        <div><h6 class="mb-0 fw-bold text-dark">Informe de Cierre</h6><span id="lblEstadoCierre" runat="server" class="small text-muted">No disponible</span></div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                            <div class="col-md-6">
                                <asp:HyperLink ID="lnkVerFinal" runat="server" Target="_blank" CssClass="d-block text-decoration-none">
                                    <div class="p-3 bg-white border rounded d-flex align-items-center shadow-sm">
                                        <div class="me-3 fs-3 text-secondary" id="iconFinalBox" runat="server"><i id="iconFinal" runat="server" class="fa-solid fa-award"></i></div>
                                        <div><h6 class="mb-0 fw-bold text-dark">Informe Final</h6><span id="lblEstadoFinal" runat="server" class="small text-muted">No disponible</span></div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <%-- 6. MODAL SUBIR INFORME (ESTRUCTURA CORREGIDA TIPO UTC-FILEINPUT) --%>
    <%-- Se utiliza la estructura exacta que demostraste en CentrosInvestigacion --%>
    <div class="modal fade" id="modalSubirInforme" tabindex="-1" aria-hidden="true" style="z-index: 1060;" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered modal-md">
            <div class="modal-content shadow-utc rounded-4 border-0">
                <div class="modal-header bg-utc text-white py-2">
                    <h6 class="modal-title fw-bold" id="lblTituloModalInforme" runat="server">Cargar Documento</h6>
                    <button type="button" class="btn-close btn-close-white" onclick="CerrarSubModalUpload()"></button>
                </div>
                <div class="modal-body pt-3 px-4 pb-4">
                    
                    <div class="form-floating mb-3">
                        <asp:TextBox ID="txtNombrePeriodoInf" runat="server" CssClass="form-control form-control-sm" placeholder="Nombre" />
                        <label>Etiqueta / Nombre del Informe</label>
                    </div>
                    
                    <%-- ESTRUCTURA ESPECIAL PARA UTC-FILEINPUT.JS --%>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoInf">
                        
                        <%-- Header del input: Icono e info --%>
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-pdf"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2 w-100 ms-3">
                                <span class="utc-fileinput-name">Sin archivo</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>

                        <%-- Elementos de control del input --%>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
                        <div class="utc-fileinput-preview" id="previewArchivoInf"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoInf"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        
                        <%-- Dropzone Visual --%>
                        <div class="utc-dropzone" id="dropzoneArchivoInf">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />
                            <span class="text-primary fw-bold">Arrastra tu PDF aquí</span>
                        </div>
                        
                        <%-- FileUpload Original Oculto --%>
                        <asp:FileUpload ID="flpArchivoInf" runat="server" CssClass="utc-fileinput-input" accept=".pdf,.doc,.docx" />
                    </div>

                    <div class="d-grid mt-3">
                        <asp:LinkButton ID="btnGuardarInforme" runat="server" CssClass="btn btn-primary btn-pill shadow-sm py-2" OnClick="btnGuardarInforme_Click">
                            <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Archivo
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- PANELES OCULTOS BACKEND (SIN CAMBIOS) --%>
    <div style="display:none;">
        <asp:Panel ID="pnlArchivoCierreActual" runat="server"><asp:Label ID="lblNombreArchivoCierre" runat="server"></asp:Label><a id="lnkVerCierreActual" runat="server"></a></asp:Panel>
        <asp:Panel ID="pnlCierreBloqueado" runat="server"></asp:Panel>
        <asp:Panel ID="pnlCargaCierre" runat="server">
            <div id="wrapperCierre"><div id="dropzoneCierre"></div><div id="previewCierre"></div><asp:FileUpload ID="flpCierre" runat="server" /></div>
            <asp:LinkButton ID="btnGuardarCierre" runat="server" OnClick="btnGuardarCierre_Click"><asp:Literal ID="litBtnCierreTexto" runat="server" /></asp:LinkButton>
            <asp:LinkButton ID="btnAprobarCierre" runat="server"></asp:LinkButton>
        </asp:Panel>
        <asp:Panel ID="pnlArchivoFinalActual" runat="server"><asp:Label ID="lblNombreArchivoFinal" runat="server"></asp:Label><a id="lnkVerFinalActual" runat="server"></a></asp:Panel>
        <asp:Panel ID="pnlCargaFinal" runat="server">
            <div id="wrapperFinal"><div id="dropzoneFinal"></div><div id="previewFinal"></div><asp:FileUpload ID="flpFinal" runat="server" /></div>
            <asp:LinkButton ID="btnGuardarFinal" runat="server" OnClick="btnGuardarFinal_Click" />
        </asp:Panel>
    </div>

    <uc:GeneradorInforme ID="ucGenerador" runat="server" OnInformeGuardado="ucGenerador_InformeGuardado" />

    <%-- SCRIPTS E INICIALIZACIÓN --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        // Configuración DataTables
        const dtConfig = {
            responsive: true, autoWidth: false, pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        $(document).ready(function () {
            // Inicializar FileInput usando la función helper
            initMyFileInput("wrapperArchivoInf", "<%= flpArchivoInf.ClientID %>");

            // Inicializar DataTable si existe la tabla
            if ($('#tablaMiembros').length) {
                if ($.fn.DataTable.isDataTable('#tablaMiembros')) $('#tablaMiembros').DataTable().destroy();
                $('#tablaMiembros').DataTable(dtConfig);
            }
        });

        // Función wrapper para inicializar el componente UTC (Idéntica a Centros)
        function initMyFileInput(wrapperId, inputId) {
            if (typeof UTC_FileInput === 'function') {
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
            var modal = new bootstrap.Modal(el);
            modal.show();
        }

        function AbrirSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            var modal = new bootstrap.Modal(el);
            modal.show();
        }

        function CerrarSubModalUpload() {
            var el = document.getElementById('modalSubirInforme');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
        }

        function LimpiarYSubir() {
            document.getElementById('<%= hfIdInformeEdit.ClientID %>').value = "";
            document.getElementById('<%= lblTituloModalInforme.ClientID %>').innerText = "Cargar Documento";
            document.getElementById('<%= txtNombrePeriodoInf.ClientID %>').value = "";
            AbrirSubModalUpload();
        }
    </script>

</asp:Content>