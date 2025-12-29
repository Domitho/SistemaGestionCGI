<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CategorizacionDocentes.aspx.cs" Inherits="SistemaGestionCGI.CategorizacionDocentes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%-- RECURSOS DE ESTILO UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <%-- HEADER --%>
    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-graduation-cap me-2"></i> GESTIÓN DE CATEGORIZACIÓN
        </h3>
        <div class="d-flex gap-2">
            <asp:LinkButton runat="server" ID="btnNuevo" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO DOCENTE
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" Visible="false" OnClick="btnRegresar_Click" CausesValidation="false">
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
                                <td class="text-start fw-bold"><%# Eval("NombreCompleto") %></td>
                                <td class="small"><%# Eval("strFacultad_doc") %></td>
                                
                                <%-- Columna Categoría con Badges --%>
                                <td>
                                    <span class='<%# string.IsNullOrEmpty(Eval("strCategorizacion")?.ToString()) ? "badge bg-secondary opacity-50" : "badge bg-primary" %>'>
                                        <%# string.IsNullOrEmpty(Eval("strCategorizacion")?.ToString()) ? "SIN ASIGNAR" : Eval("strCategorizacion") %>
                                    </span>
                                </td>
                                
                                <td><%# Eval("dtFechaCategorizacion") == null ? "-" : Convert.ToDateTime(Eval("dtFechaCategorizacion")).ToString("dd/MM/yyyy") %></td>
                                
                                <td>
                                    <%-- Botón Editar/Asignar --%>
                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="editar" CommandArgument='<%# Eval("strId_doc") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Asignar o Cambiar Categoría">
                                        <i class="fa-solid fa-pen-to-square"></i>
                                    </asp:LinkButton>

                                    <%-- Botón Historial --%>
                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="historial" CommandArgument='<%# Eval("strId_doc") %>'
                                        CssClass="btn btn-info btn-sm rounded-circle text-white me-1" ToolTip="Ver Historial">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>

                                    <%-- Botón Eliminar (Solo si tiene categoría) --%>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="eliminar" CommandArgument='<%# Eval("strId_doc") %>'
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" 
                                        Visible='<%# !string.IsNullOrEmpty(Eval("strCategorizacion")?.ToString()) %>'
                                        OnClientClick="return confirm('¿Está seguro de QUITAR la categoría actual de este docente? Esta acción quedará registrada en el historial.');" 
                                        ToolTip="Quitar Categoría">
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
                <%-- Cédula --%>
                <div class="col-md-6">
                    <label class="form-label">Cédula <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" placeholder="Ingrese Cédula" />
                </div>

                <%-- Nombres --%>
                <div class="col-md-6">
                    <label class="form-label">Nombres <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" placeholder="Nombres del docente" />
                </div>

                <%-- Apellidos --%>
                <div class="col-md-6">
                    <label class="form-label">Apellidos <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" placeholder="Apellidos del docente" />
                </div>

                <%-- Facultad (DROPDOWN) --%>
                <div class="col-md-6">
                    <label class="form-label">Facultad <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem Value="CIYA">CIENCIAS DE LA INGENIERÍA Y APLICADAS (CIYA)</asp:ListItem>
                        <asp:ListItem Value="CAREN">CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES (CAREN)</asp:ListItem>
                        <asp:ListItem Value="CAYE">CIENCIAS ADMINISTRATIVAS Y ECONÓMICAS (CAYE)</asp:ListItem>
                        <asp:ListItem Value="CSAYE">CIENCIAS SOCIALES, ARTES Y EDUCACIÓN (CSAYE)</asp:ListItem>
                        <asp:ListItem Value="SALUD">CIENCIAS DE LA SALUD</asp:ListItem>
                        <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem Value="LAMANA">EXTENSIÓN LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- Carrera (DROPDOWN) --%>
                <div class="col-md-6">
                    <label class="form-label">Carrera <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCarrera" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem Value="SISTEMAS">INGENIERÍA EN SISTEMAS DE INFORMACIÓN</asp:ListItem>
                        <asp:ListItem Value="INDUSTRIAL">INGENIERÍA INDUSTRIAL</asp:ListItem>
                        <asp:ListItem Value="ELECTROMECANICA">ELECTROMECÁNICA</asp:ListItem>
                        <asp:ListItem Value="AGRONOMIA">AGRONOMÍA</asp:ListItem>
                        <asp:ListItem Value="VETERINARIA">MEDICINA VETERINARIA</asp:ListItem>
                        <asp:ListItem Value="ADMINISTRACION">ADMINISTRACIÓN DE EMPRESAS</asp:ListItem>
                        <asp:ListItem Value="CONTABILIDAD">CONTABILIDAD Y AUDITORÍA</asp:ListItem>
                        <asp:ListItem Value="ENFERMERIA">ENFERMERÍA</asp:ListItem>
                        <asp:ListItem Value="OTRA">OTRA</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-12"><hr class="text-muted opacity-25" /></div>

                <%-- Categoría (DROPDOWN) --%>
                <div class="col-md-6">
                    <label class="form-label">Categoría Asignada <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem Value="TITULAR PRINCIPAL 1">TITULAR PRINCIPAL 1</asp:ListItem>
                        <asp:ListItem Value="TITULAR PRINCIPAL 2">TITULAR PRINCIPAL 2</asp:ListItem>
                        <asp:ListItem Value="TITULAR PRINCIPAL 3">TITULAR PRINCIPAL 3</asp:ListItem>
                        <asp:ListItem Value="TITULAR AGREGADO 1">TITULAR AGREGADO 1</asp:ListItem>
                        <asp:ListItem Value="TITULAR AGREGADO 2">TITULAR AGREGADO 2</asp:ListItem>
                        <asp:ListItem Value="TITULAR AGREGADO 3">TITULAR AGREGADO 3</asp:ListItem>
                        <asp:ListItem Value="TITULAR AUXILIAR 1">TITULAR AUXILIAR 1</asp:ListItem>
                        <asp:ListItem Value="TITULAR AUXILIAR 2">TITULAR AUXILIAR 2</asp:ListItem>
                        <asp:ListItem Value="OCASIONAL 1">OCASIONAL 1</asp:ListItem>
                        <asp:ListItem Value="OCASIONAL 2">OCASIONAL 2</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- Fecha --%>
                <div class="col-md-6">
                    <label class="form-label">Fecha Resolución <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtFecha" runat="server" TextMode="Date" CssClass="form-control" />
                </div>
            </div>

            <%-- Botones --%>
            <div class="d-flex justify-content-center gap-3 flex-wrap mt-4">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 py-2" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Ficha
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary btn-pill px-5 py-2" OnClick="btnRegresar_Click">
                    Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- MODAL DE HISTORIAL --%>
    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered"> <%-- AQUI ESTÁ EL XL --%>
            <div class="modal-content shadow-utc border-0">
                
                <%-- Header Azul Gradiente --%>
                <div class="modal-header bg-utc">
                    <h5 class="modal-title w-100 text-center text-white">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i> HISTORIAL DE CAMBIOS
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">

                    <%-- 1. CAMPO OCULTO PARA EL ID (Necesario para generar el reporte) --%>
                    <asp:HiddenField ID="hfIdDocenteHistorial" runat="server" />

                    <%-- 2. BOTÓN GENERAR REPORTE (Alineado a la derecha) --%>
                    <div class="d-flex justify-content-end mb-3">
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" 
                            CssClass="btn btn-danger btn-pill px-4 shadow-sm" 
                            OnClick="btnGenerarReporte_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte Completo
                        </asp:LinkButton>
                    </div>

                    <div class="table-responsive rounded border-0">
                        <%-- Tabla con estilos UTC --%>
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
                                                <span class="badge badge-historial rounded-pill px-3">
                                                    <%# Eval("strAccion") %>
                                                </span>
                                            </td>
                                            
                                            <td class="text-muted small text-start ps-4">
                                                <i class="fa-solid fa-arrow-right-from-bracket me-1 text-danger opacity-50"></i>
                                                <%# Eval("strValorAnterior") %>
                                            </td>
                                            
                                            <td class="text-primary fw-bold small text-start ps-4">
                                                <i class="fa-solid fa-arrow-right-to-bracket me-1"></i>
                                                <%# Eval("strValorNuevo") %>
                                            </td>
                                            
                                            <td class="text-start fst-italic text-muted small"><%# Eval("strMotivo") %></td>
                                            
                                            <td class="small fw-bold text-secondary"><%# Eval("strUsuario") %></td>
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
                     <button type="button" class="btn btn-outline-primary btn-pill px-5" data-bs-dismiss="modal">
                         Cerrar Historial
                     </button>
                </div>
            </div>
        </div>
    </div>

    <%-- =====================================================================
         MODAL VISTA PREVIA REPORTE (REPLICADO DEL EJEMPLO)
         ===================================================================== --%>
    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                <%-- Cabecera del Modal (Negra) --%>
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title">Vista Previa del Reporte</h6>
                    <div>
                        <button type="button" class="btn btn-sm btn-light me-2" onclick="imprimirReporte()"><i class="fa-solid fa-print"></i> Imprimir</button>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                </div>

                <%-- Cuerpo del Reporte (Hoja Blanca) --%>
                <div class="modal-body p-4" style="background: white; min-height: 500px;">
                    <div id="areaImpresion" class="report-paper">
                        
                        <%-- 1. HERO BANNER (LOGO) --%>
                        <div class="header-hero-banner">
                            <img src="https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png" alt="UTC Logo" />
                        </div>

                        <%-- 2. CABECERA DIVIDIDA (TITULO Y METADATA) --%>
                        <div class="header-info-split">
                            <div class="info-left">
                                <span class="system-label">Dirección de Investigación</span>
                                <h1 class="doc-title">Ficha de Categorización</h1>
                            </div>
        
                            <div class="info-right">
                                <div class="meta-group">
                                    <span class="meta-label">ID Referencia</span>
                                    <asp:Label ID="lblRefId" runat="server" CssClass="meta-value ref-highlight" Text="DOC-000"></asp:Label>
                                </div>
                                <div class="meta-group">
                                    <span class="meta-label">Fecha de Emisión</span>
                                    <span class="meta-value"><%= DateTime.Now.ToString("dd/MM/yyyy") %></span>
                                </div>
                            </div>
                        </div>
        
                        <div class="mt-5"></div>

                        <%-- 3. CARD DE DATOS (REJILLA DE INFORMACIÓN) --%>
                        <div class="researcher-card">
                            <div class="card-row">
                                <div class="card-item">
                                    <span class="label">DOCENTE</span>
                                    <asp:Label ID="lblReporteNombre" runat="server" CssClass="value"></asp:Label>
                                </div>
                                <div class="card-item">
                                    <span class="label">CÉDULA</span>
                                    <asp:Label ID="lblReporteCedula" runat="server" CssClass="value"></asp:Label>
                                </div>
                            </div>
                            <div class="card-row">
                                <div class="card-item">
                                    <span class="label">FACULTAD</span>
                                    <asp:Label ID="lblReporteFacultad" runat="server" CssClass="value"></asp:Label>
                                </div>
                                <div class="card-item">
                                    <span class="label">CARRERA</span>
                                    <asp:Label ID="lblReporteCarrera" runat="server" CssClass="value"></asp:Label>
                                </div>
                            </div>
                            <div class="card-row">
                                <div class="card-item" style="flex: 2;">
                                    <span class="label">CATEGORÍA ACTUAL</span>
                                    <asp:Label ID="lblReporteCategoria" runat="server" CssClass="value text-primary fw-bold"></asp:Label>
                                </div>
                                <div class="card-item">
                                    <span class="label">F. RESOLUCIÓN</span>
                                    <asp:Label ID="lblReporteFecha" runat="server" CssClass="value"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <%-- 4. TIMELINE (HISTORIAL CRONOLÓGICO) --%>
                        <div class="timeline-container">
                            <h4 class="timeline-title">Historial de Cambios y Movimientos</h4>
        
                            <ul class="timeline-list">
                                <asp:Repeater ID="rptReporteHistorial" runat="server">
                                    <ItemTemplate>
                                        <li class="timeline-item">
                                            <div class="timeline-marker"></div>
                                            <div class="timeline-content">
                                                <div class="timeline-header">
                                                    <span class="date"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd 'de' MMMM, yyyy") %></span>
                                                    <span class="time"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("HH:mm") %></span>
                                                </div>
                            
                                                <div class="timeline-body">
                                                    <%-- Badge condicional según acción --%>
                                                    <div class="action-badge <%# Eval("strAccion").ToString().Contains("BAJA") || Eval("strAccion").ToString().Contains("ELIMINACION") ? "bad" : "good" %>">
                                                        <%# Eval("strAccion") %>
                                                    </div>
                                                    <p class="description">
                                                        <strong>Detalle:</strong> <%# Eval("strMotivo") %>
                                                    </p>
                                                    <div class="user-signature">
                                                        <i class="fa-solid fa-user-check"></i> Responsable: <%# Eval("strUsuario") %>
                                                    </div>
                                                </div>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>

                        <%-- 5. FOOTER LEGAL --%>
                        <div class="report-legal-footer" style="margin-top: 80px;">
                            Documento generado automáticamente por el Sistema de Gestión CGI-UTC. 
                            Información válida para procesos internos de la Dirección de Investigación.
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- SCRIPTS --%>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#tablaDocentes').DataTable({
                responsive: true,
                language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
                dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
            });
        });

        function abrirModalHistorial() {
            var myModal = new bootstrap.Modal(document.getElementById('modalHistorial'));
            myModal.show();
        }

        function imprimirReporte() {
            var contenido = document.getElementById("areaImpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');
            ventana.document.write('<html><head><title>Ficha de Categorización</title>');
            // Importante: Incluir Bootstrap y tu CSS personalizado para que se vea igual
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');
            // Aquí deberías apuntar a tu CSS de reporte si lo tienes publicado, si no, se verán estilos básicos
            // Para asegurar estilos, copiamos los básicos:
            ventana.document.write('<style>body{font-family: sans-serif;} .header-hero-banner img{width:100%;} .researcher-card{border:1px solid #ccc; padding:15px; margin-bottom:20px; border-radius:8px;} .card-row{display:flex; border-bottom:1px solid #eee; padding:5px 0;} .label{font-weight:bold; font-size:0.8rem; color:#666; width:120px; display:inline-block;} .value{font-weight:bold;}</style>');
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