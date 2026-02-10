<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CentrosInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.CentrosInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfCentroIdActual" runat="server" />
    <asp:HiddenField ID="hfIdCentro" runat="server" />

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/papelera.css" rel="stylesheet" />

    <style>
        .popover {
            font-family: system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            font-size: 0.8rem; 
            max-width: 240px; 
            border: 1px solid rgba(49, 39, 131, 0.2); 
            box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15); 
            border-radius: 0.5rem;
        }

        .popover-header {
            background-color: #f8f9fa;
            color: #312783; 
            font-weight: 700;
            font-size: 0.85rem;
            padding: 0.5rem 0.75rem; 
            border-bottom: 1px solid #eaeaea;
        }

        .popover-body {
            padding: 0.6rem 0.75rem; 
            color: #444;
            line-height: 1.4;
        }
    
        .bs-popover-top > .popover-arrow::after {
            border-top-color: #f8f9fa;
        }
    </style>

    <%-- ENCABEZADO PRINCIPAL --%>
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

    <%-- 1. TABLA DE CENTROS --%>
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
                                    <asp:LinkButton ID="btnArchivos" runat="server" CommandName="Archivos" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-success btn-sm rounded-circle me-1 text-white" ToolTip="Ver Documentos Adjuntos">
                                        <i class="fa-solid fa-folder-open"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnIntegrantes" runat="server" CommandName="Integrantes" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1 text-white" ToolTip="Gestionar Integrantes">
                                        <i class="fa-solid fa-users"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cen") %>' 
                                        CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirmarEliminar(this, '¿Eliminar centro?');" ToolTip="Eliminar">
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

    <%-- 2. FORMULARIO DE CENTRO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-building-columns me-2"></i> GESTIÓN DE CENTRO
            </h3>
            <asp:LinkButton ID="btnRegresar" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4 text-start">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-pen me-2"></i> Datos del Centro
            </h4>
            
            <div class="row g-3">
                <div class="col-md-12">
                    <label class="form-label">Nombre del Centro <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-bold text-primary">Director Encargado</label>
                    <div class="input-group gap-2">
                        <asp:TextBox ID="txtDirector" runat="server" 
                            CssClass="form-control border-start-0 bg-light"     
                            ReadOnly="true" 
                            placeholder="-- Sin Director Asignado --" />
        
                        <button type="button" id="btnNuevoDirectorInput" runat="server" 
                                class="btn btn-primary" onclick="AbrirModalNuevoDirector()">
                            <i class="fa-solid fa-pen-to-square me-1"></i> Asignar
                        </button>
                    </div>
                    <small class="text-muted">* Para asignar un director, use el botón Asignar.</small>
                </div>
                                <div class="col-md-6"><label class="form-label">Área</label><asp:TextBox ID="txtArea" runat="server" CssClass="form-control" /></div>
                
                <div class="col-12">
                    <label class="form-label">Facultad</label>
                    <asp:DropDownList ID="ddlFacultad" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem Value="CAREN">CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES</asp:ListItem>
                        <asp:ListItem Value="CIYA">CIENCIAS DE LA INGENIERÍA Y APLICADAS</asp:ListItem>
                        <asp:ListItem Value="CAYE">CIENCIAS ADMINISTRATIVAS Y ECONÓMICAS</asp:ListItem>
                        <asp:ListItem Value="CSAYE">CIENCIAS SOCIALES ARTES Y EDUCACIÓN</asp:ListItem>
                        <asp:ListItem Value="SALUD">CIENCIAS DE LA SALUD</asp:ListItem>
                        <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem Value="LAMANA">EXTENSIÓN LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Ubicación</label><asp:TextBox ID="txtUbicacion" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Fecha Aprobación</label><asp:TextBox ID="txtFechaAprobacion" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-12">
                    <label class="form-label">Líneas de Investigación <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlLineas" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione una Línea --" Value="" />
                        <asp:ListItem>Análisis, conservación y aprovechamiento racional de la biodiversidad, fauna y recursos naturales para el desarrollo sustentable y la prevención de desastres naturales.</asp:ListItem>
                        <asp:ListItem>Procesos tecnológicos, bioquímica, biomateriales, desarrollo y seguridad alimentaria.</asp:ListItem>
                        <asp:ListItem>Producción y biotecnología animal.</asp:ListItem>
                        <asp:ListItem>Tecnología industrial, gestión de la producción, riesgos y seguridad laboral.</asp:ListItem>
                        <asp:ListItem>Energías alternativas y renovables, eficiencia energética y protección ambiental.</asp:ListItem>
                        <asp:ListItem>Tecnología de la información y las comunicaciones, robótica, automatización y optimización de sistemas.</asp:ListItem>
                        <asp:ListItem>Meteorología, hidrología, mecánica de fluidos, sistemas y obras hidráulicas.</asp:ListItem>
                        <asp:ListItem>Administración y economía para el desarrollo sostenible de organizaciones y sociedad.</asp:ListItem>
                        <asp:ListItem>Planificación y gestión del turismo sostenible y sustentable.</asp:ListItem>
                        <asp:ListItem>Educación, derecho, equidad y estudio de género para el desarrollo biopsicosocial.</asp:ListItem>
                        <asp:ListItem>Cultura, arte, diseño y comunicación para la transformación del ser humano y la sociedad.</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6"><label class="form-label">Misión</label><asp:TextBox ID="txtMision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
                <div class="col-md-6"><label class="form-label">Visión</label><asp:TextBox ID="txtVision" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>

                <asp:HiddenField ID="hfResolucionActual" runat="server" />
                <asp:HiddenField ID="hfAceptacionActual" runat="server" />

                <div class="row g-3 mt-2">
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Resolución de Creación</label>
                        <div class="utc-fileinput-wrapper" id="wrapperResolucion">
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
                            <div class="utc-fileinput-preview" id="previewResolucion"></div>
                            <div class="utc-fileinput-loader" id="loaderResolucion"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                            <div class="utc-dropzone" id="dropzoneResolucion">
                                <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Subir Resolución
                            </div>
                            <asp:FileUpload ID="flpResolucion" runat="server" CssClass="utc-fileinput-input" />
                        </div>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Documento de Aceptación</label>
                        <div class="utc-fileinput-wrapper" id="wrapperAceptacion">
                            <div class="utc-fileinput-header">
                                <div class="utc-fileinput-icon"><i class="fa-solid fa-check-double"></i></div>
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <span class="utc-fileinput-name">Sin archivo</span>
                                    <div class="utc-fileinput-buttons d-flex gap-2">
                                         <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                         <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                    </div>
                                </div>
                            </div>
                            <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Renombrar..." />
                            <div class="utc-fileinput-preview" id="previewAceptacion"></div>
                            <div class="utc-fileinput-loader" id="loaderAceptacion"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                            <div class="utc-dropzone" id="dropzoneAceptacion">
                                <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Subir Aceptación
                            </div>
                            <asp:FileUpload ID="flpAceptacion" runat="server" CssClass="utc-fileinput-input" />
                        </div>
                    </div>
                </div>

            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardar_Click" OnClientClick="return UTC_BloquearBoton(this);" UseSubmitBehavior="false">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarCentro" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- 3. TABLA DE INTEGRANTES --%>
    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnVerPapelera" 
                    CssClass="btn btn-outline-danger btn-pill" 
                    OnClick="btnVerPapelera_Click"
                    data-bs-toggle="popover" 
                    data-bs-trigger="hover focus"
                    title="Papelera de Integrantes" 
                    data-bs-content="Aquí puedes consultar el historial de eliminados y <b>restaurar</b> integrantes si es necesario.">
                    <i class="fa-solid fa-trash-can me-2"></i> PAPELERA
                </asp:LinkButton>
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
                        <th>FECHA FIN</th>
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
                                <td class="small">
                                    <%# Eval("dtFechaFin_cin") == DBNull.Value || Eval("dtFechaFin_cin") == null 
                                        ? "<span class='text-muted'> -- SIN FECHA FIN -- </span>" 
                                        : Convert.ToDateTime(Eval("dtFechaFin_cin")).ToString("dd/MM/yyyy") 
                                    %>
                                </td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_cin")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="Historial" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" 
                                        data-bs-toggle="popover" 
                                        data-bs-trigger="hover focus"
                                        title="Historial" 
                                        data-bs-content="Consulta la cronología de movimientos y cambios de este integrante.">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditarInt" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" 
                                        data-bs-toggle="popover" 
                                        data-bs-trigger="hover focus"
                                        title="Editar Datos" 
                                        data-bs-content="Modifica la información personal o de vinculación.">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEliminarInt" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "btn btn-outline-danger btn-sm rounded-circle" : "btn btn-outline-success btn-sm rounded-circle" %>' 
                                        data-bs-toggle="popover" 
                                        data-bs-trigger="hover focus"
                                        title='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "Dar de Baja" : "Reactivar Integrante" %>'
                                        data-bs-content='<%# Convert.ToBoolean(Eval("bitActivo_cin")) ? "El integrante pasará a estado <b>Inactivo</b>." : "El integrante volverá a estar <b>Activo</b> en el centro." %>'>
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
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            <asp:LinkButton ID="btnCancelarIntTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4 text-start">
        
            <h5 class="text-center fw-bold mb-4" style="color: var(--utc-azul-oscuro);">
                <i class="fa-solid fa-user-plus me-2"></i> DATOS DEL INTEGRANTE
            </h5>

            <div class="mb-4">
                <label class="fw-bold text-primary small text-uppercase mb-1 d-block">Tipo de Integrante</label>
    
                <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select shadow-sm" 
                    AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlTipoInt_SelectedIndexChanged">
                    <asp:ListItem Text="Interno (Administrativo/Estudiante)" Value="Interno" Selected="True"/>
                    <asp:ListItem Text="Externo (Invitado)" Value="Externo" />
                </asp:DropDownList>
            </div>

            <asp:HiddenField ID="hfIdIntegrante" runat="server" />

            <div class="row g-3">
            
                <hr class="text-muted opacity-25 my-4">
                <div class="col-12">
                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-regular fa-id-card me-2"></i> Información Personal
                        </h6>
                </div>

                <div class="col-md-12">
                    <label class="form-label small fw-bold text-secondary">Cédula</label>
                    <div class="input-group gap-2">
                        <asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" autocomplete="off" />
                        <asp:LinkButton ID="btnValidarCedulaInt" runat="server" CssClass="btn btn-outline-primary" OnClick="btnValidarCedulaInt_Click">
                            <i class="fa-solid fa-magnifying-glass me-1"></i> Validar
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="col-md-6">
                    <label class="form-label small fw-bold text-secondary">Nombres</label>
                    <asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control" autocomplete="off"/>
                </div>

                <div class="col-md-6">
                    <label class="form-label small fw-bold text-secondary">Apellidos</label>
                    <asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control" autocomplete="off"/>
                </div>

                <div class="col-12">
                    <label class="form-label small fw-bold text-secondary">Correo</label>
                    <asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off"/>
                </div>

                <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                    <i class="fa-solid fa-building-columns me-2"></i> Afiliación Institucional
                </h6>

                <asp:Panel ID="pnlIntInterno" runat="server" Visible="true" CssClass="col-12">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label small fw-bold text-secondary">Facultad / Extensión</label>
                            <asp:DropDownList ID="ddlFacultadInt" runat="server" CssClass="form-select" 
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFacultadInt_SelectedIndexChanged">
                                <asp:ListItem Text="-- Seleccione --" Value="" />
                                <asp:ListItem Value="CAREN">CIENCIAS AGROPECUARIAS (CAREN)</asp:ListItem>
                                <asp:ListItem Value="CIYA">CIENCIAS DE LA INGENIERIA (CIYA)</asp:ListItem>
                                <asp:ListItem Value="CAYE">CIENCIAS ADMINISTRATIVAS (CAYE)</asp:ListItem>
                                <asp:ListItem Value="CSAYE">CIENCIAS SOCIALES (CSAYE)</asp:ListItem>
                                <asp:ListItem Value="SALUD">CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                                <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                                <asp:ListItem Value="LAMANA">EXTENSION LA MANÁ</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-bold text-secondary">Carrera / Departamento</label>
                            <asp:DropDownList ID="ddlCarreraInt" runat="server" CssClass="form-select">
                                <asp:ListItem Text="-- Seleccione Facultad Primero --" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlIntExterno" runat="server" Visible="false" CssClass="col-12">
                    <label class="form-label small fw-bold text-secondary">Institución de Origen</label>
                    <asp:TextBox ID="txtEntidadExternoInt" runat="server" CssClass="form-control" placeholder="Universidad o Empresa..." autocomplete="off" />
                </asp:Panel>

                <div class="col-12">
                    <hr class="text-muted opacity-25 my-4">
                </div>

                <div class="col-md-6">
                    <label class="form-label small fw-bold text-secondary">Función Asignada</label>
                    <div class="input-group">
                        <span class="input-group-text border-end-0" style="background-color: #e3f2fd; color: #0d6efd;">
                            <i class="fa-solid fa-id-card"></i>
                        </span>
                        <asp:DropDownList ID="ddlFuncionInt" runat="server" CssClass="form-select border-start-0 text-primary fw-bold" Enabled="false">
                            <asp:ListItem Text="Miembro Investigador" Value="Miembro" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-md-6">
                    <label class="form-label small fw-bold text-secondary">Fecha Inicio</label>
                    <div class="input-group">
                        <input type="text" class="form-control" value="<%= DateTime.Now.ToString("dd/MM/yyyy") %>" readonly style="background-color: #fff;" />
                        <span class="input-group-text bg-white text-secondary"><i class="fa-regular fa-calendar"></i></span>
                    </div>
                </div>

            </div> <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardarInt" runat="server" CssClass="btn btn-primary btn-pill px-4 py-2 fw-bold shadow-sm" OnClick="btnGuardarInt_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Integrante
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-secondary btn-pill px-4 py-2 fw-bold" OnClick="btnCancelarInt_Click">
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
                <div class="modal-body p-4 text-start">
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
                            <div class="info-left text-start">
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

                        <div class="researcher-card p-4 mb-5 rounded-3 text-start" style="background-color: #f8faff; border: 1px solid #e1e8f0; border-left: 5px solid #003876;">
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

                        <div class="timeline-container ps-2 text-start">
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

    <div class="modal fade" id="modalNuevoDirector" tabindex="-1" aria-hidden="true" ClientIDMode="Static" data-bs-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content border-0 rounded-4 shadow-utc overflow-hidden">
            
                <div class="modal-header border-0" style="background: linear-gradient(90deg, var(--utc-azul) 0%, var(--utc-azul-oscuro) 100%) !important; color: #fff;">
                    <h5 class="modal-title fw-bold">
                        <i class="fa-solid fa-user-plus me-2"></i> NUEVO DIRECTOR
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body p-4 bg-white text-start">
                    <div class="row mb-4">
                        <div class="col-md-12">
                            <label class="form-label text-primary fw-bold small text-uppercase mb-1">Tipo de Vinculación</label>
                            
                            <asp:DropDownList ID="ddlTipoDirModal" runat="server" CssClass="form-select shadow-sm"
                                AutoPostBack="true" 
                                OnSelectedIndexChanged="ddlTipoDirModal_SelectedIndexChanged">
                                <asp:ListItem Text="INTERNO (Docente / Administrativo)" Value="Interno" Selected="True" />
                                <asp:ListItem Text="EXTERNO (Invitado)" Value="Externo" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-4">
                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-regular fa-id-card me-2"></i> INFORMACIÓN PERSONAL
                        </h6>
                        
                        <div class="row g-3">
                            <div class="col-md-12">
                                <label class="form-label text-secondary small fw-bold">Número de Cédula <span class="text-danger">*</span></label>
                                <div class="input-group gap-2">
                                    <asp:TextBox ID="txtCedulaDirModal" runat="server" CssClass="form-control" MaxLength="10" placeholder="Ej: 050..." autocomplete="off" />
                                    <asp:LinkButton ID="btnValidarCedula" runat="server" CssClass="btn btn-primary" OnClick="btnValidarCedula_Click">
                                        <i class="fa-solid fa-magnifying-glass me-1"></i> Validar
                                    </asp:LinkButton>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <label class="form-label text-secondary small fw-bold">Nombres <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNombresDirModal" runat="server" CssClass="form-control" autocomplete="off" placeholder="Nombres completos" />
                            </div>
                        
                            <div class="col-md-6">
                                <label class="form-label text-secondary small fw-bold">Apellidos <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtApellidosDirModal" runat="server" CssClass="form-control" autocomplete="off" placeholder="Apellidos completos" />
                            </div>

                            <div class="col-12">
                                <label class="form-label text-secondary small fw-bold">Correo Electrónico <span class="text-danger">*</span></label>
                                <div class="input-group">
                                    <span class="input-group-text bg-light"><i class="fa-solid fa-envelope"></i></span>
                                    <asp:TextBox ID="txtCorreoDirModal" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off" placeholder="ejemplo@utc.edu.ec" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:Panel ID="pnlDirInterno" runat="server" Visible="true">
                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-solid fa-building-columns me-2"></i> AFILIACIÓN INSTITUCIONAL
                        </h6>
                        <div class="row g-3 mb-4">
                            <div class="col-md-6">
                                <label class="form-label text-secondary small fw-bold mb-1">Facultad / Extensión</label>
                                <asp:DropDownList ID="ddlFacultadDirModal" runat="server" CssClass="form-select shadow-sm" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlFacultadDirModal_SelectedIndexChanged">
                                    <asp:ListItem Text="-- Seleccione --" Value="" />
                                    <asp:ListItem Value="CAREN">CIENCIAS AGROPECUARIAS (CAREN)</asp:ListItem>
                                    <asp:ListItem Value="CIYA">CIENCIAS DE LA INGENIERIA (CIYA)</asp:ListItem>
                                    <asp:ListItem Value="CAYE">CIENCIAS ADMINISTRATIVAS (CAYE)</asp:ListItem>
                                    <asp:ListItem Value="CSAYE">CIENCIAS SOCIALES (CSAYE)</asp:ListItem>
                                    <asp:ListItem Value="SALUD">CIENCIAS DE LA SALUD (CS)</asp:ListItem> 
                                    <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                                    <asp:ListItem Value="LAMANA">EXTENSION LA MANÁ</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label text-secondary small fw-bold mb-1">Carrera / Departamento</label>
                                <asp:DropDownList ID="ddlCarreraDirModal" runat="server" CssClass="form-select shadow-sm">
                                    <asp:ListItem Text="-- Seleccione Facultad Primero --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlDirExterno" runat="server" Visible="false">
                        <h6 class="utc-subtitle border-bottom pb-2 mb-3 text-secondary" style="font-size: 0.85rem; letter-spacing: 0.5px;">
                            <i class="fa-solid fa-briefcase me-2"></i> INSTITUCIÓN DE ORIGEN
                        </h6>
                        <div class="col-12">
                            <label class="form-label text-secondary small fw-bold">Nombre de la Institución / Empresa</label>
                            <asp:TextBox ID="txtEntidadDirModal" runat="server" CssClass="form-control" placeholder="Ej: Universidad Central del Ecuador" autocomplete="off" />
                        </div>
                    </asp:Panel>

                </div>

                <div class="modal-footer justify-content-center border-0 pb-4 pt-0 bg-white">
                    <asp:LinkButton ID="btnGuardarDirectorModal" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm fw-bold" OnClick="btnGuardarDirectorModal_Click">
                        <i class="fa-solid fa-check me-2"></i> ASIGNAR DIRECTOR
                    </asp:LinkButton>
                    <button type="button" class="btn btn-outline-secondary btn-pill px-4" data-bs-dismiss="modal">CANCELAR</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalDocumentos" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 rounded-4">
            
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title"><i class="fa-solid fa-folder-tree me-2"></i> Documentación del Centro</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body p-4 bg-light text-start">
    
                    <div class="text-center border-bottom pb-3 mb-4">
                        <h6 class="text-primary fw-bold text-uppercase mb-1">
                            <asp:Label ID="lblCentroDocNombre" runat="server" Text="Nombre del Centro"></asp:Label>
                        </h6>
                        <small class="text-muted">Gestión rápida de documentación digital</small>
                    </div>

                    <asp:HiddenField ID="hfIdCentroDocModal" runat="server" />
                    <asp:HiddenField ID="hfResModalActual" runat="server" />
                    <asp:HiddenField ID="hfAceModalActual" runat="server" />

                    <div class="row g-4">
        
                        <div class="col-md-6">
                            <div class="card h-100 border-0 shadow-sm">
                                <div class="card-header bg-white fw-bold text-primary border-bottom-0 pt-3">
                                    <i class="fa-solid fa-file-contract me-2"></i> Resolución de Creación
                                </div>
                                <div class="card-body">
                    
                                    <div class="utc-fileinput-wrapper" id="wrapperResModal">
                                        <div class="utc-fileinput-header">
                                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-pdf"></i></div>
                                            <div class="d-flex justify-content-between align-items-center mb-2">
                                                <span class="utc-fileinput-name">Sin archivo</span>
                                                <div class="utc-fileinput-buttons d-flex gap-2">
                                                     <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                                </div>
                                            </div>
                                        </div>
                        
                                        <div class="utc-fileinput-preview" id="previewResModal"></div>
                        
                                        <div class="utc-fileinput-loader" id="loaderResModal">
                                            <i class="fa-solid fa-spinner fa-spin me-2"></i> Procesando...
                                        </div>

                                        <div class="utc-dropzone" id="dropzoneResModal">
                                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />
                                            <small>Arrastre o Click para cambiar</small>
                                        </div>
                                        <asp:FileUpload ID="flpResModal" runat="server" CssClass="utc-fileinput-input" />
                                    </div>
                    
                                    <asp:HyperLink ID="lnkDescargarResModal" runat="server" Target="_blank" 
                                        CssClass="btn btn-link btn-sm text-decoration-none mt-2 d-block text-center">
                                        <i class="fa-solid fa-external-link-alt"></i> Ver documento actual
                                    </asp:HyperLink>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="card h-100 border-0 shadow-sm">
                                <div class="card-header bg-white fw-bold text-success border-bottom-0 pt-3">
                                    <i class="fa-solid fa-file-circle-check me-2"></i> Documento de Aceptación
                                </div>
                                <div class="card-body">
                    
                                    <div class="utc-fileinput-wrapper" id="wrapperAceModal">
                                        <div class="utc-fileinput-header">
                                            <div class="utc-fileinput-icon"><i class="fa-solid fa-check-double"></i></div>
                                            <div class="d-flex justify-content-between align-items-center mb-2">
                                                <span class="utc-fileinput-name">Sin archivo</span>
                                                <div class="utc-fileinput-buttons d-flex gap-2">
                                                     <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                                </div>
                                            </div>
                                        </div>
                        
                                        <div class="utc-fileinput-preview" id="previewAceModal"></div>

                                        <div class="utc-fileinput-loader" id="loaderAceModal">
                                            <i class="fa-solid fa-spinner fa-spin me-2"></i> Procesando...
                                        </div>

                                        <div class="utc-dropzone" id="dropzoneAceModal">
                                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-success"></i><br />
                                            <small>Arrastre o Click para cambiar</small>
                                        </div>
                                        <asp:FileUpload ID="flpAceModal" runat="server" CssClass="utc-fileinput-input" />
                                    </div>

                                    <asp:HyperLink ID="lnkDescargarAceModal" runat="server" Target="_blank" 
                                        CssClass="btn btn-link btn-sm text-decoration-none mt-2 d-block text-center text-success">
                                        <i class="fa-solid fa-external-link-alt"></i> Ver documento actual
                                    </asp:HyperLink>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="modal-footer justify-content-center border-0 pt-0 pb-4 bg-light">
                    <asp:LinkButton ID="btnActualizarDocs" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnActualizarDocs_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Cambios
                    </asp:LinkButton>
                    <button type="button" class="btn btn-outline-secondary btn-pill px-4" data-bs-dismiss="modal">Cerrar</button>
                </div>

            </div>
        </div>
    </div>

    <div class="modal fade" id="modalPapelera" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content utc-modal-premium">
            
                <%-- CABECERA PREMIUM --%>
                <div class="modal-header papelera-header-premium d-flex flex-column align-items-center justify-content-center text-white position-relative">
                    <button type="button" class="btn-close btn-close-white position-absolute top-0 end-0 m-4" data-bs-dismiss="modal"></button>
                    <div class="bg-white bg-opacity-25 rounded-circle p-3 mb-3 backdrop-blur">
                        <i class="fa-solid fa-trash-arrow-up fa-2x"></i>
                    </div>
                    <h4 class="fw-bold mb-1">Papelera de Integrantes</h4>
                    <p class="mb-0 small opacity-75">Gestión de recuperación de personal eliminado</p>
                </div>

                <div class="modal-body p-4 bg-light">
                    <asp:Repeater ID="rptPapelera" runat="server" OnItemCommand="rptPapelera_ItemCommand">
                        <ItemTemplate>
                            <div class="docente-trash-card p-3">
                                <div class="d-flex align-items-center justify-content-between mb-3">
                                    <div class="d-flex align-items-center gap-3">
                                        <div class="bg-light rounded-circle p-3 text-secondary border">
                                            <i class="fa-solid fa-user-xmark fa-lg"></i>
                                        </div>
                                        <div>
                                            <h6 class="fw-bold text-dark mb-1 text-uppercase"><%# Eval("NombreCompleto") %></h6>
                                            <span class="status-badge-inactive">ELIMINADO</span>
                                        </div>
                                    </div>
                                    <asp:LinkButton runat="server" CommandName="Restaurar" CommandArgument='<%# Eval("strId_cin") %>' 
                                        CssClass="btn btn-sm btn-success rounded-pill px-4 shadow-sm fw-bold" 
                                        OnClientClick="return confirm('¿Está seguro de restaurar a este integrante?');">
                                        <i class="fa-solid fa-rotate-left me-2"></i> RESTAURAR
                                    </asp:LinkButton>
                                </div>

                                <div class="d-flex mt-2 pt-3 border-top bg-white text-center">
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Cédula</span>
                                        <span class="value-bold"><%# Eval("strCedula_cin") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Función Anterior</span>
                                        <span class="value-bold text-primary"><%# Eval("strFuncion_cin") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Tipo</span>
                                        <span class="value-bold"><%# Eval("strTipo_cin") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Entidad / Fac.</span>
                                        <span class="value-bold text-muted" style="font-size: 0.7rem;">
                                            <%# Eval("strTipo_cin").ToString() == "Interno" ? Eval("strFacultad_cin") : Eval("strEntidad_cin") %>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Panel ID="pnlVacio" runat="server" Visible='<%# rptPapelera.Items.Count == 0 %>'>
                                <div class="text-center py-5">
                                    <div class="mb-3 text-muted opacity-25">
                                        <i class="fa-solid fa-trash-can fa-4x"></i>
                                    </div>
                                    <h6 class="fw-bold text-secondary">La papelera está vacía</h6>
                                    <p class="text-muted small mb-0">No hay integrantes eliminados recientemente.</p>
                                </div>
                            </asp:Panel>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal-footer border-0 bg-light justify-content-center pb-4">
                    <button type="button" class="btn btn-outline-secondary btn-pill px-5" data-bs-dismiss="modal">
                        Cerrar Ventana
                    </button>
                </div>
            </div>
        </div>
    </div>

    <%-- LIBRERÍAS EXTERNAS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">

        // Configuración global para DataTables
        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        Sys.Application.add_load(function () {

            initTables();

            if (typeof UTC_FileInput === 'function') {
                initMyFileInput('wrapperResolucion', '<%= flpResolucion.ClientID %>');
                initMyFileInput('wrapperAceptacion', '<%= flpAceptacion.ClientID %>');
            }

            cargarEstadoEdicion('wrapperResolucion', '<%= hfResolucionActual.ClientID %>');
            cargarEstadoEdicion('wrapperAceptacion', '<%= hfAceptacionActual.ClientID %>');

            var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
            var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
                return new bootstrap.Popover(popoverTriggerEl, { html: true, trigger: 'hover focus' })
            });
        });

        function initMyFileInput(wrapperId, inputId) {
            if (document.getElementById(wrapperId)) {
                UTC_FileInput({
                    wrapper: wrapperId,
                    dropzone: wrapperId.replace('wrapper', 'dropzone'),
                    preview: wrapperId.replace('wrapper', 'preview'),
                    loader: wrapperId.replace('wrapper', 'loader'),
                    input: inputId
                });
            }
        }

        function cargarEstadoEdicion(wrapperId, hiddenFieldId) {
            var hf = document.getElementById(hiddenFieldId);
            var wrapper = document.getElementById(wrapperId);

            if (hf && hf.value && wrapper) {
                var dropzone = document.getElementById(wrapperId.replace('wrapper', 'dropzone'));
                var preview = document.getElementById(wrapperId.replace('wrapper', 'preview'));
                var nameLabel = wrapper.querySelector('.utc-fileinput-name');
                var removeBtn = wrapper.querySelector('.remove-btn');
                var nombreArchivo = hf.value.split('/').pop().split('\\').pop();

                if (dropzone) dropzone.style.display = 'none';

                if (nameLabel) {
                    nameLabel.textContent = nombreArchivo;
                    nameLabel.classList.add('text-primary', 'fw-bold');
                }

                if (preview) {
                    preview.style.display = 'block';
                    var esImagen = /\.(jpg|jpeg|png|gif|webp)$/i.test(nombreArchivo);
                    var rutaWeb = hf.value.replace('~/', ''); 

                    if (esImagen) {
                        preview.innerHTML = '<div class="text-center mt-2"><img src="' + rutaWeb + '" style="height:60px; border-radius:4px; border:1px solid #ddd;" /></div>';
                    } else {
                        var icono = nombreArchivo.includes('.pdf') ? 'fa-file-pdf text-danger' : (nombreArchivo.includes('.doc') ? 'fa-file-word text-primary' : 'fa-file-lines');
                        preview.innerHTML = '<div class="alert alert-light border mt-2 py-1"><i class="fa-solid ' + icono + ' me-2"></i>Archivo Actual Cargado</div>';
                    }
                }

                if (removeBtn) {
                    removeBtn.style.display = 'block';
                    removeBtn.onclick = function () { hf.value = ''; };
                }
            }
        }

        function initTables() {
            if (document.getElementById('tablaCentros')) {
                if ($.fn.DataTable.isDataTable('#tablaCentros')) $('#tablaCentros').DataTable().destroy();
                $('#tablaCentros').DataTable(dtConfig);
            }
            if (document.getElementById('tablaIntegrantes')) {
                if ($.fn.DataTable.isDataTable('#tablaIntegrantes')) $('#tablaIntegrantes').DataTable().destroy();
                $('#tablaIntegrantes').DataTable(dtConfig);
            }
        }

        function AbrirModalNuevoDirector() {
            var el = document.getElementById('modalNuevoDirector');
            if (el) new bootstrap.Modal(el).show();
        }

        function abrirModalPapelera() {
            var el = document.getElementById('modalPapelera');
            if (el) {
                var m = bootstrap.Modal.getOrCreateInstance(el);
                m.show();
            }
        }

        function imprimirReporte() {
            var area = document.getElementById("arealmpresion");
            if (!area) return;

            var contenido = area.innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');
            ventana.document.write('<html><head><title>Reporte</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');
            ventana.document.write('<style>body{font-family:sans-serif;} .header-hero-banner{background:#003876!important;color:#fff!important;padding:20px;text-align:center;} .timeline-list{list-style:none;border-left:2px solid #ccc;padding-left:20px;}</style>');
            ventana.document.write('</head><body>' + contenido + '</body></html>');
            ventana.document.close();
            ventana.focus();
            setTimeout(function () { ventana.print(); ventana.close(); }, 500);
        }
    </script>

</asp:Content>