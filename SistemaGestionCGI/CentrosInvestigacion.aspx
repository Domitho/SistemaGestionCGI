<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CentrosInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CentrosInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <div id="headerCentros" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-building-columns me-2"></i> CENTROS DE INVESTIGACIÓN
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevo" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO CENTRO
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaCentros" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>NOMBRE</th>
                        <th>FACULTAD</th>
                        <th>DIRECTOR</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCentros" runat="server" OnItemCommand="rptCentros_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_cen") %></td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("strNombre_cen") %></td>
                                <td class="text-start small text-muted"><%# Eval("strFacultad_cen") %></td>
                                <td class="text-start"><%# Eval("NombreDirector") ?? "--- SIN ASIGNAR ---" %></td>
                                <td>
                                    <asp:LinkButton ID="btnIntegrantes" runat="server" CommandName="Integrantes" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1 text-white" ToolTip="Gestionar Integrantes">
                                        <i class="fa-solid fa-users"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Eliminar centro?');" ToolTip="Eliminar">
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
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-building-columns me-2"></i> GESTIÓN DE CENTRO
            </h3>
            <asp:LinkButton ID="btnRegresar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-pen me-2"></i> Datos del Centro
            </h4>

            <asp:HiddenField ID="hfIdCentro" runat="server" />
            
            <div class="row g-3">
                <div class="col-md-12">
                    <label class="form-label">Nombre del Centro <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
                <div class="col-md-12">
                    <label class="form-label fw-bold text-primary">Director Encargado</label>
                    <div class="input-group">
                        <span class="input-group-text bg-white text-primary border-end-0"><i class="fa-solid fa-user-tie"></i></span>
                        
                        <asp:DropDownList ID="ddlDirector" runat="server" CssClass="form-select border-start-0 bg-light">
                            <asp:ListItem Text="-- Sin Director Asignado --" Value="" />
                        </asp:DropDownList>
                        
                        <button type="button" class="btn btn-outline-primary" onclick="AbrirModalNuevoDirector()">
                            <i class="fa-solid fa-plus me-1"></i> Nuevo
                        </button>
                    </div>
                    <small class="text-muted">* Seleccione un integrante existente o registre uno nuevo directamente.</small>
                </div>
                
                <div class="col-md-6">
                    <label class="form-label">Facultad</label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem>CIENCIAS DE LA INGENIERÍA Y APLICADAS</asp:ListItem>
                        <asp:ListItem>CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES</asp:ListItem>
                        <asp:ListItem>CIENCIAS ADMINISTRATIVAS Y ECONÓMICAS</asp:ListItem>
                        <asp:ListItem>CIENCIAS SOCIALES ARTES Y EDUCACIÓN</asp:ListItem>
                        <asp:ListItem>CIENCIAS DE LA SALUD</asp:ListItem>
                        <asp:ListItem>EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem>EXTENSIÓN LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Área</label><asp:TextBox ID="txtArea" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Ubicación</label><asp:TextBox ID="txtUbicacion" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Fecha Aprobación</label><asp:TextBox ID="txtFechaAprobacion" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-12"><label class="form-label">Líneas de Inv.</label><asp:TextBox ID="txtLineas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                <div class="col-12"><label class="form-label">Misión</label><asp:TextBox ID="txtMision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                <div class="col-12"><label class="form-label">Visión</label><asp:TextBox ID="txtVision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardar_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarCentro" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnNuevoIntegrante" CssClass="btn btn-primary btn-pill" OnClick="btnNuevoIntegrante_Click">
                    <i class="fa-solid fa-user-plus me-2"></i> NUEVO INTEGRANTE
                </asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnVolverCentro" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnVolverCentro_Click">
                    <i class="fa-solid fa-arrow-left me-2"></i> VOLVER A CENTROS
                </asp:LinkButton>
            </div>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <h5 class="text-primary border-bottom pb-2 mb-3">
                <small class="text-muted small fw-normal">Centro:</small> 
                <asp:Label ID="lblNombreCentroSeleccionado" runat="server" Font-Bold="true"></asp:Label>
            </h5>
            
            <table id="tablaIntegrantes" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>CÉDULA</th>
                        <th>NOMBRE</th>
                        <th>FUNCIÓN</th>
                        <th>TIPO</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand">
                        <ItemTemplate>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "" : "table-secondary text-muted" %>'>
                                <td><%# Eval("strCedula_cin") %></td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("NombreCompleto") %></td>
                                <td class="text-start"><%# Eval("strFuncion_cin") %></td>
                                <td><%# Eval("strTipo_cin") %></td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_cin")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="Historial" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" ToolTip="Ver Historial de Movimientos">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditarInt" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminarInt" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "btn btn-outline-danger btn-sm rounded-circle" : "btn btn-outline-success btn-sm rounded-circle" %>' 
                                        ToolTip='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "Dar de Baja" : "Activar" %>'>
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormularioInt" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES</h3>
            <asp:LinkButton ID="btnCancelarIntTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                OnClick="btnCancelarInt_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-user-pen me-2"></i> Datos del Integrante
            </h4>

            <asp:HiddenField ID="hfIdIntegrante" runat="server" />
            
            <div class="row g-3">
                <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>
                <div class="col-md-4"><label class="form-label">Cédula</label><asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" MaxLength="15"/></div>
                <div class="col-md-4"><label class="form-label">Nombres</label><asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control"/></div>
                <div class="col-md-4"><label class="form-label">Apellidos</label><asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control"/></div>
                <div class="col-md-6"><label class="form-label">Correo</label><asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email"/></div>
                
                <div class="col-md-6">
                    <label class="form-label">Tipo</label>
                    <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select">
                        <asp:ListItem>Interno (UTC)</asp:ListItem>
                        <asp:ListItem>Externo</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Carrera / Entidad</label><asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control" placeholder="Carrera o Universidad de origen"/></div>

                <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Datos de Asignación</h6></div>
                <div class="col-md-6">
                    <label class="form-label fw-bold">Función (Cargo)</label>
                    <asp:DropDownList ID="ddlFuncionInt" runat="server" CssClass="form-select">
                        <asp:ListItem>Investigador</asp:ListItem>
                        <asp:ListItem>Coordinador</asp:ListItem>
                        <asp:ListItem>Analista</asp:ListItem>
                        <asp:ListItem Value="Director" class="fw-bold">DIRECTOR (Jefe del Centro)</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardarInt" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarInt_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content rounded-4 shadow-utc border-0">
                
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100 text-center">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i> HISTORIAL DE MOVIMIENTOS
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-white">
                    <div class="d-flex justify-content-between align-items-center mb-3 border-bottom pb-3">
                        <h6 class="fw-bold text-secondary mb-0">
                            INTEGRANTE: <asp:Label ID="lblNombreHistorial" runat="server" CssClass="text-primary text-uppercase" Text="..." />
                        </h6>
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" 
                            CssClass="btn btn-danger btn-pill btn-sm px-4 shadow-sm" 
                            OnClick="btnGenerarReporte_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte PDF
                        </asp:LinkButton>
                    </div>

                    <asp:HiddenField ID="hfIdIntegranteHistorial" runat="server" />

                    <div class="table-responsive rounded border-0">
                        <table class="table table-sm table-hover table-historial-utc align-middle text-center mb-0">
                            <thead>
                                <tr>
                                    <th style="width: 15%">FECHA</th>
                                    <th style="width: 15%">ACCIÓN</th>
                                    <th style="width: 55%">MOTIVO / DETALLE</th>
                                    <th style="width: 15%">USUARIO</th>
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
                                                <span class='badge rounded-pill px-3 <%# 
                                                    Eval("strAccion").ToString() == "BAJA" ? "badge-baja" : 
                                                    (Eval("strAccion").ToString().Contains("NUEVO") || Eval("strAccion").ToString() == "VINCULACIÓN" ? "badge-alta" : "badge-historial") 
                                                %>'>
                                                    <%# Eval("strAccion") %>
                                                </span>
                                            </td>
                                            <td class="text-start fst-italic text-muted small ps-3">
                                                <%# Eval("strMotivo") %>
                                            </td>
                                            <td class="small fw-bold text-secondary">
                                                <i class="fa-solid fa-user-check me-1 opacity-50"></i>
                                                <%# Eval("strUsuario") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="pnlNoHistorial" runat="server" Visible='<%# rptHistorial.Items.Count == 0 %>'>
                                            <tr>
                                                <td colspan="4" class="p-4 text-center text-muted">
                                                    <i class="fa-solid fa-folder-open fa-2x mb-2 d-block opacity-25"></i>
                                                    Sin movimientos registrados en el historial.
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
                     <button type="button" class="btn btn-outline-secondary btn-pill px-5" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEstadoInt" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 rounded-4">
                <div class="modal-header bg-primary text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-power-off me-2"></i> CAMBIO DE ESTADO</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="text-center mb-4">
                        <i class="fa-solid fa-circle-exclamation fa-3x text-warning mb-3"></i>
                        <p class="fs-5">¿Estás seguro de <strong class="text-primary">cambiar el estado</strong> de este integrante?</p>
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label fw-bold text-secondary">Motivo del cambio <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMotivoEstado" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Ingrese la justificación..."></asp:TextBox>
                        <asp:HiddenField ID="hfIdIntegranteEstado" runat="server" />
                    </div>
                </div>
                <div class="modal-footer justify-content-center border-0 pb-4">
                    <asp:LinkButton ID="btnConfirmarCambioEstado" runat="server" CssClass="btn btn-pill btn-danger px-5 shadow-sm" OnClick="btnConfirmarCambioEstado_Click">
                        <i class="fa-solid fa-check me-2"></i> Confirmar Cambio
                    </asp:LinkButton>
                    <button type="button" class="btn btn-pill btn-outline-secondary px-4" data-bs-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title" id="lblTituloPreview">Vista Previa del Reporte</h6>
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
                                <span class="d-block text-uppercase small fw-bold text-secondary">Sistema de Gestión de Investigación</span>
                                <h1 class="doc-title mb-0" style="color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase;">Historial de Movimientos</h1>
                            </div>
                            <div class="info-right text-end">
                                <div class="meta-group">
                                    <span class="d-block text-uppercase small fw-bold text-secondary">Referencia</span>
                                    <asp:Label ID="lblRefId" runat="server" CssClass="fw-bold fs-5 text-dark" Text="N/A"></asp:Label>
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
                                    <span class="d-block small fw-bold text-uppercase text-secondary">INVESTIGADOR</span>
                                    <asp:Label ID="lblReporteNombre" runat="server" CssClass="fs-5 fw-bold text-primary"></asp:Label>
                                </div>
                                <div class="col-6 text-end">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">IDENTIFICACIÓN</span>
                                    <asp:Label ID="lblReporteCedula" runat="server" CssClass="fs-5 fw-bold text-dark"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-6">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">ROL / FUNCIÓN</span>
                                    <asp:Label ID="lblReporteFuncion" runat="server" CssClass="fw-bold text-dark"></asp:Label>
                                </div>
                                <div class="col-6 text-end">
                                    <span class="d-block small fw-bold text-uppercase text-secondary">ESTADO ACTUAL</span>
                                    <asp:Label ID="lblReporteEstado" runat="server" CssClass="fw-bold"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="timeline-container ps-2">
                            <h4 class="mb-4 pb-2 border-bottom fw-bold text-secondary">Registro Cronológico de Eventos</h4>
                            
                            <ul class="timeline-list list-unstyled position-relative ps-4" style="border-left: 2px solid #e9ecef;">
                                <asp:Repeater ID="rptReporteHistorial" runat="server">
                                    <ItemTemplate>
                                        <li class="timeline-item mb-4 position-relative">
                                            <div class="timeline-marker position-absolute bg-white border border-3 border-primary rounded-circle" 
                                                 style="width: 16px; height: 16px; left: -25px; top: 5px;"></div>
                                            
                                            <div class="timeline-content ps-3">
                                                <div class="timeline-header d-flex justify-content-between mb-1">
                                                    <span class="date fw-bold text-dark"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd 'de' MMMM, yyyy") %></span>
                                                    <span class="time text-muted small"><%# Convert.ToDateTime(Eval("dtFecha")).ToString("HH:mm") %></span>
                                                </div>
                                                
                                                <div class="timeline-body bg-light p-3 rounded border-start border-4" 
                                                     style='<%# Eval("strAccion").ToString() == "BAJA" ? "border-color: #dc3545 !important;" : "border-color: #198754 !important;" %>'>
                                                    
                                                    <div class="action-badge d-inline-block px-2 py-1 rounded small fw-bold mb-2 text-uppercase"
                                                         style='<%# Eval("strAccion").ToString() == "BAJA" ? "background: rgba(220,53,69,0.1); color: #dc3545;" : "background: rgba(25,135,84,0.1); color: #198754;" %>'>
                                                        <%# Eval("strAccion") %>
                                                    </div>
                                                    
                                                    <p class="description mb-2 small text-muted">
                                                        <strong>Motivo:</strong> <%# Eval("strMotivo") %>
                                                    </p>
                                                    
                                                    <div class="user-signature small text-secondary">
                                                        <i class="fa-solid fa-user-check me-1"></i> Procesado por: <strong><%# Eval("strUsuario") %></strong>
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
                            La validez de este reporte está sujeta a los registros digitales institucionales.</p>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalNuevoDirector" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content shadow-utc border-0 rounded-4">
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title"><i class="fa-solid fa-user-tie me-2"></i> Registrar Nuevo Director</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    
                    <div class="alert alert-light border-start border-primary border-4 shadow-sm small text-muted mb-4">
                        <i class="fa-solid fa-circle-info text-primary me-2"></i> Este registro se asignará automáticamente como <strong>DIRECTOR</strong> del centro actual.
                    </div>

                    <div class="row g-3">
                        <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>
                        
                        <div class="col-md-4">
                            <label class="form-label">Cédula <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCedulaDirModal" runat="server" CssClass="form-control" MaxLength="15" autocomplete="off"/>
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Nombres <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtNombresDirModal" runat="server" CssClass="form-control" autocomplete="off" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">Apellidos <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtApellidosDirModal" runat="server" CssClass="form-control" autocomplete="off" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Correo Institucional</label>
                            <asp:TextBox ID="txtCorreoDirModal" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Tipo de Vinculación</label>
                            <asp:DropDownList ID="ddlTipoDirModal" runat="server" CssClass="form-select" onchange="ToggleTipoDirector(this)">
                                <asp:ListItem Text="Interno (UTC)" Value="Interno" Selected="True"/>
                                <asp:ListItem Text="Externo" Value="Externo" />
                            </asp:DropDownList>
                        </div>

                        <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Académicos</h6></div>

                        <div id="divDirInterno" class="col-12 row g-3 m-0 p-0">
                            <div class="col-md-6">
                                <label class="form-label">Carrera / Departamento</label>
                                <asp:TextBox ID="txtCarreraDirModal" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Facultad</label>
                                <asp:DropDownList ID="ddlFacultadDirModal" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- Seleccione --" Value="" />
                                    <asp:ListItem>CIENCIAS DE LA INGENIERÍA Y APLICADAS</asp:ListItem>
                                    <asp:ListItem>CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES</asp:ListItem>
                                    <asp:ListItem>CIENCIAS ADMINISTRATIVAS Y ECONÓMICAS</asp:ListItem>
                                    <asp:ListItem>CIENCIAS SOCIALES ARTES Y EDUCACIÓN</asp:ListItem>
                                    <asp:ListItem>CIENCIAS DE LA SALUD</asp:ListItem>
                                    <asp:ListItem>EXTENSIÓN PUJILÍ</asp:ListItem>
                                    <asp:ListItem>EXTENSIÓN LA MANÁ</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div id="divDirExterno" class="col-12" style="display:none;">
                            <label class="form-label">Institución de Origen</label>
                            <asp:TextBox ID="txtEntidadDirModal" runat="server" CssClass="form-control" placeholder="Universidad o Entidad..." />
                        </div>
                    </div>
                </div>
                <div class="modal-footer border-0 bg-light justify-content-center">
                    <asp:LinkButton ID="btnGuardarDirectorModal" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardarDirectorModal_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Director
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <%-- JAVASCRIPT --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script>

        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        function initTables() {
            if ($.fn.DataTable.isDataTable('#tablaCentros')) $('#tablaCentros').DataTable().destroy();
            if ($.fn.DataTable.isDataTable('#tablaIntegrantes')) $('#tablaIntegrantes').DataTable().destroy();

            $('#tablaCentros').DataTable(dtConfig);
            $('#tablaIntegrantes').DataTable(dtConfig);
        }

        $(function () { initTables(); });
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () { initTables(); });

        function AbrirModalNuevoDirector() {
            var myModal = new bootstrap.Modal(document.getElementById('modalNuevoDirector'));
            myModal.show();
        }

        function ToggleTipoDirector(el) {
            var tipo = el.value;
            var divInt = document.getElementById('divDirInterno');
            var divExt = document.getElementById('divDirExterno');
            if (tipo === "Externo") {
                divInt.style.display = 'none';
                divExt.style.display = 'block';
            } else {
                divInt.style.display = 'flex';
                divExt.style.display = 'none';
            }
        }

        function imprimirReporte() {
            var contenido = document.getElementById("arealmpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');

            ventana.document.write('<html><head><title>Reporte de Historial</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');
            ventana.document.write('<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">');

            ventana.document.write('<style>');
            ventana.document.write('body { font-family: "Segoe UI", sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; }');
            ventana.document.write('.report-paper { padding: 40px 50px; }');

            ventana.document.write('.header-hero-banner { background-color: #003876 !important; color: white !important; margin: -40px -50px 40px -50px; padding: 30px; text-align: center; border-bottom: 6px solid #002a5c; display: block; }');
            ventana.document.write('.header-hero-banner img { height: 70px; width: auto; filter: brightness(0) invert(1); }');

            ventana.document.write('.doc-title { color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase; }');

            ventana.document.write('.researcher-card { background-color: #f8faff !important; border-left: 5px solid #003876 !important; padding: 20px; margin-bottom: 40px; border: 1px solid #e1e8f0; border-radius: 6px; }');

            ventana.document.write('.timeline-list { list-style: none; padding: 0; position: relative; margin-left: 10px; border-left: 2px solid #e9ecef; }');
            ventana.document.write('.timeline-item { position: relative; padding-left: 30px; margin-bottom: 30px; }');
            ventana.document.write('.timeline-marker { position: absolute; left: -9px; top: 0; width: 16px; height: 16px; border-radius: 50%; background: #fff; border: 3px solid #003876; z-index: 2; }');

            ventana.document.write('.action-badge { display: inline-block; padding: 4px 10px; border-radius: 4px; font-size: 0.7rem; font-weight: 800; text-transform: uppercase; margin-bottom: 6px; }');

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