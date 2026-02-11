<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GruposInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.GruposInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/papelera.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-images.css" rel="stylesheet" />

    <style>
        .transition-hover {
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }
        .transition-hover:hover {
            transform: translateY(-2px);
            box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.1) !important;
        } 
    </style>

    <div id="headerGrupos" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="btnNuevoGrupo" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="btnNuevoGrupo_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO GRUPO
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaGrupos" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th> <th>FOTO</th> <th>NOMBRE</th> <th>CENTRO</th> <th>COORDINADOR</th> 
                        <th>CATEGORÍA</th> <th>CREACIÓN</th> <th class="text-center">PORTAFOLIO</th> <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptGrupoInv" runat="server" OnItemCommand="rptGrupoInv_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_gru") %></td>
                                <td><img src='<%# ObtenerImagenBase64(Eval("strFoto_gru")) %>' class="img-avatar-table" alt="Foto" /></td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("strNombre_gru") %></td>
                                <td class="text-start small text-muted"><%# Eval("strNombre_cen") %></td>
                                <td class="text-start"><%# Eval("strCoordinador_gru") %></td>
                                <td><span class="badge bg-light text-dark border"><%# Eval("strCategoria_gru") %></span></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechacrea_gru")).ToString("dd/MM/yyyy") %></td>
                                <td class="text-center">
                                    <asp:LinkButton ID="btnVerProyectos" runat="server" 
                                        CommandName="VerProyectos" 
                                        CommandArgument='<%# Eval("strId_gru") %>'
                                        Enabled='<%# Convert.ToInt32(Eval("TotalProyectos")) > 0 %>'
                                        CssClass='<%# Convert.ToInt32(Eval("TotalProyectos")) > 0 ? 
                                                     "btn btn-sm btn-outline-primary btn-pill fw-bold px-3 shadow-sm" : 
                                                     "btn btn-sm btn-light text-muted btn-pill border-0" %>'>
                                        <i class='<%# Convert.ToInt32(Eval("TotalProyectos")) > 0 ? 
                                                     "fa-solid fa-folder-open me-2" : 
                                                     "fa-solid fa-folder me-2" %>'></i>
                                        <%# Eval("TotalProyectos") %> Proyectos
                                    </asp:LinkButton>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnVerArchivo" runat="server" CommandName="Archivo" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-ver btn-sm rounded-circle me-1" ToolTip="Ver Resolución"><i class="fa-solid fa-eye"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnVerInt" runat="server" CommandName="VerIntegrantes" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-primary btn-sm rounded-circle me-1 text-white" ToolTip="Gestionar Integrantes"><i class="fa-solid fa-users"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar Grupo"><i class="fa-solid fa-pen"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirmarEliminar(this, '¿Está seguro de eliminar este proyecto? Esta acción no se puede deshacer.');" ToolTip="Eliminar"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormularioGrupo" runat="server" Visible="false">

        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-people-group me-2"></i>
                <asp:Label ID="lblTituloFormulario" runat="server" Text="Gestión de Grupo"></asp:Label>
            </h3>
            <asp:LinkButton ID="btnRegresarTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4 bg-white">

            <asp:HiddenField ID="hfIdGrupo" runat="server" />
            <asp:HiddenField ID="hfFotoActual" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />
            <asp:HiddenField ID="hfCoordNombre" runat="server" />
            <asp:HiddenField ID="hfCoordCedula" runat="server" />
            <asp:HiddenField ID="hfCoordArchivo" runat="server" />
            <asp:HiddenField ID="hfCoordApellidos" runat="server" />
            <asp:HiddenField ID="hfCoordCorreo" runat="server" />
            <asp:HiddenField ID="hfCoordCarrera" runat="server" />
            <asp:HiddenField ID="hfCoordFacultad" runat="server" />

            <h5 class="text-primary fw-bold mb-3 border-bottom pb-2">
                <i class="fa-solid fa-layer-group me-2"></i> DATOS GENERALES Y AFILIACIÓN
            </h5>

            <div class="row g-3 mb-4">
                <div class="col-12">
                    <label class="form-label text-muted small fw-bold">Nombre del Grupo <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombreGru" runat="server" CssClass="form-control" autocomplete="off" placeholder="Ingrese el nombre oficial..." />
                </div>

                <div class="col-12">
                    <label class="form-label text-muted small fw-bold">Centro de Investigación</label>
                    <asp:DropDownList ID="ddlCentro" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label text-muted small fw-bold">Facultad / Extensión <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlFacultadGrupo" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlFacultadGrupo_SelectedIndexChanged">
                        <asp:ListItem Text="-- Seleccione --" Value="" />
                        <asp:ListItem Value="CAREN">FACULTAD DE CIENCIAS AGROPECUARIAS (CAREN)</asp:ListItem>
                        <asp:ListItem Value="CIYA">FACULTAD DE CIENCIAS DE LA INGENIERIA (CIYA)</asp:ListItem>
                        <asp:ListItem Value="CAYE">FACULTAD DE CIENCIAS ADMINISTRATIVAS (CAYE)</asp:ListItem>
                        <asp:ListItem Value="CSAYE">FACULTAD DE CIENCIAS SOCIALES (CSAYE)</asp:ListItem>
                        <asp:ListItem Value="SALUD">FACULTAD CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                        <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                        <asp:ListItem Value="LAMANA">EXTENSION LA MANÁ</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label text-muted small fw-bold">Carrera / Departamento <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCarreraGrupo" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Seleccione Facultad Primero --" Value="" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label text-muted small fw-bold">Categoría</label>
                    <asp:DropDownList ID="ddlCategoriaGru" runat="server" CssClass="form-select">
                        <asp:ListItem Value="NUEVO">NUEVO</asp:ListItem>
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem>
                        <asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
                        <asp:ListItem Value="DISUELTO">DISUELTO</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label class="form-label text-muted small fw-bold">Fecha de Creación</label>
                    <asp:TextBox ID="txtFechaCreaGru" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-12">
                    <label class="form-label text-muted small fw-bold">Línea de Investigación</label>
                    <asp:DropDownList ID="ddlLineaInv" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Análisis, conservación y aprovechamiento racional de la biodiversidad, fauna y recursos naturales para el desarrollo sustentable y la prevención de desastres naturales.">Análisis, conservación y aprovechamiento racional de la biodiversidad, fauna y recursos naturales para el desarrollo sustentable y la prevención de desastres naturales.</asp:ListItem>
                        <asp:ListItem Value="Procesos tecnológicos, bioquímica, biomateriales, desarrollo y seguridad alimentaria.">Procesos tecnológicos, bioquímica, biomateriales, desarrollo y seguridad alimentaria.</asp:ListItem>
                        <asp:ListItem Value="Tecnología industrial, gestión de la producción, riesgos y seguridad laboral.">Tecnología industrial, gestión de la producción, riesgos y seguridad laboral.</asp:ListItem>
                        <asp:ListItem Value="Energías alternativas y renovables, eficiencia energética y protección ambiental.">Energías alternativas y renovables, eficiencia energética y protección ambiental.</asp:ListItem>
                        <asp:ListItem Value="Tecnología de la información y las comunicaciones, robótica, automatización y optimización de sistemas.">Tecnología de la información y las comunicaciones, robótica, automatización y optimización de sistemas.</asp:ListItem>
                        <asp:ListItem Value="Meteorología, hidrología, mecánica de fluidos, sistemas y obras hidráulicas.">Meteorología, hidrología, mecánica de fluidos, sistemas y obras hidráulicas.</asp:ListItem>
                        <asp:ListItem Value="Administración y economía para el desarrollo sostenible de organizaciones y sociedad.">Administración y economía para el desarrollo sostenible de organizaciones y sociedad.</asp:ListItem>
                        <asp:ListItem Value="Planificación y gestión del turismo sostenible y sustentable.">Planificación y gestión del turismo sostenible y sustentable.</asp:ListItem>
                        <asp:ListItem Value="Educación, derecho, equidad y estudio de género para el desarrollo biopsicosocial.">Educación, derecho, equidad y estudio de género para el desarrollo biopsicosocial.</asp:ListItem>
                        <asp:ListItem Value="Cultura, arte, diseño y comunicación para la transformación del ser humano y la sociedad.">Cultura, arte, diseño y comunicación para la transformación del ser humano y la sociedad.</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <h5 class="text-primary fw-bold mb-3 border-bottom pb-2 pt-2">
                <i class="fa-solid fa-user-tie me-2"></i> RESPONSABLE DEL GRUPO
            </h5>

            <div class="row g-3 mb-4">
                <div class="col-12">
                    <label class="form-label text-muted small fw-bold">Coordinador Asignado</label>
                    <div class="input-group gap-2">
                        <span class="input-group-text bg-light border-0"><i class="fa-solid fa-user-check text-primary"></i></span>
                        <asp:TextBox ID="txtCoordinadorGru" runat="server" CssClass="form-control" ReadOnly="true" placeholder="No se ha asignado un coordinador..."></asp:TextBox>
                        <asp:LinkButton ID="btnAgregarCoordinador" runat="server" CssClass="btn btn-primary px-4" OnClick="btnAgregarCoordinador_Click">
                            <i class="fa-solid fa-magnifying-glass me-2"></i> Asignar / Buscar
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <h5 class="text-primary fw-bold mb-3 border-bottom pb-2 pt-2">
                <i class="fa-solid fa-folder-open me-2"></i> DOCUMENTACIÓN E IDENTIDAD
            </h5>

            <div class="row g-4"> <div class="col-md-6 d-flex flex-column">
                    <label class="form-label text-muted small fw-bold">Resolución de Creación (PDF)</label>
        
                    <div class="utc-fileinput-wrapper flex-grow-1" id="wrapperArchivoGrupo">
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
                        <div class="utc-fileinput-preview" id="previewArchivoGrupo"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoGrupo"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivoGrupo">
                            <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />
                            <span class="small">Arrastre la resolución aquí</span>
                        </div>
                        <asp:FileUpload ID="flpArchivoGrupo" runat="server" CssClass="utc-fileinput-input" accept=".pdf,.doc,.docx" />
                    </div>
                </div>

                <div class="col-md-6 d-flex flex-column">
                    <label class="form-label text-muted small fw-bold">Identidad Visual</label>
        
                    <div class="p-4 bg-white rounded-3 shadow-sm text-center position-relative overflow-hidden flex-grow-1 d-flex flex-column justify-content-center mt-3" 
                         style="border: 2px solid #312783;"> 

                        <div class="d-flex flex-column align-items-center justify-content-center w-100 h-100">
                
                            <div class="utc-avatar-wrapper mb-4" onclick="triggerFotoUpload()">
                                <asp:Image ID="imgFotoVisual" runat="server" CssClass="utc-avatar-img" ImageUrl="~/DesignersUTC/Images/default-group.png" />
                                <div class="utc-avatar-overlay">
                                    <i class="fa-solid fa-pen-to-square"></i>
                                    <span>Cambiar</span>
                                </div>
                            </div>

                            <asp:FileUpload ID="flpFotoGrupo" runat="server" style="display:none;" onchange="previewAvatar(this)" accept="image/*" />

                            <div class="d-flex justify-content-center align-items-center gap-3 w-100">
                                <button type="button" class="btn btn-primary btn-pill px-4 py-2 shadow-sm d-inline-flex align-items-center justify-content-center" 
                                        onclick="triggerFotoUpload()" style="min-width: 180px;">
                                    <i class="fa-solid fa-upload me-2"></i> 
                                    <span class="fw-bold">Elegir Imagen</span>
                                </button>

                                <asp:LinkButton ID="btnEliminarFoto" runat="server" 
                                    CssClass="btn btn-outline-danger btn-pill px-3 py-2 shadow-sm d-inline-flex align-items-center justify-content-center" 
                                    OnClick="btnEliminarFoto_Click" ToolTip="Eliminar foto actual">
                                    <i class="fa-solid fa-trash"></i>
                                    <span class="fw-bold">Eliminar Imagen</span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardarGrupo" runat="server" 
                    CssClass="btn btn-primary btn-pill px-5 shadow-sm fw-bold" 
                    OnClientClick="return ValidarFormularioGrupo();" 
                    OnClick="btnGuardarGrupo_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> GUARDAR DATOS
                </asp:LinkButton>
            
                <asp:LinkButton ID="btnCancelarGrupo" runat="server" 
                    CssClass="btn btn-outline-secondary btn-pill px-4 fw-bold" 
                    OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> CANCELAR
                </asp:LinkButton>
            </div>

        </div>
    </asp:Panel>

    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        <asp:HiddenField ID="hfGrupoIdActual" runat="server" />
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES</h3>
            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnVerPapeleraInt" CssClass="btn btn-outline-danger btn-pill" OnClick="btnVerPapeleraInt_Click">
                    <i class="fa-solid fa-trash-can me-2"></i> PAPELERA
                </asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnNuevoIntegrante" CssClass="btn btn-primary btn-pill" OnClick="btnNuevoIntegrante_Click">
                    <i class="fa-solid fa-user-plus me-2"></i> NUEVO INTEGRANTE
                </asp:LinkButton>
                <asp:LinkButton runat="server" ID="btnVolverGrupos" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnVolverGrupos_Click">
                    <i class="fa-solid fa-arrow-left me-2"></i> VOLVER A GRUPOS
                </asp:LinkButton>
            </div>
        </div>
        
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaIntegrantes" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>INVESTIGADOR</th>
                        <th>FUNCIÓN</th>
                        <th>INICIO</th>
                        <th>FIN</th>
                        <th>ESTADO</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand">
                        <ItemTemplate>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "" : "table-secondary text-muted" %>'>
                        
                                <td><%# Eval("strId_int") %></td>
                                <td class="text-start fw-semibold text-primary">
                                    <%# Eval("strApellidos_int") + " " + Eval("strNombres_int") %>
                                </td>
                                <td class="text-start"><%# Eval("strFuncion_int") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_int")).ToString("dd/MM/yyyy") %></td>
                                <td>
                                    <%# Convert.ToDateTime(Eval("dtFechafin_int")).Year < 1900 ? 
                                        "<span class='badge bg-light text-secondary border'>Vigente</span>" : 
                                        Convert.ToDateTime(Eval("dtFechafin_int")).ToString("dd/MM/yyyy") %>
                                </td>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_int")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnVerCertificado" runat="server" 
                                        CommandName="VerCertificado" 
                                        CommandArgument='<%# Eval("strCertificado_int") %>'
    
                                        Visible='<%# !string.IsNullOrEmpty(Eval("strCertificado_int") as string) %>'
    
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" 
                                        ToolTip="Ver Certificado">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnEditarInt" runat="server" 
                                        CommandName="EditarInt" 
                                        CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" 
                                        ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnToggleEstado" runat="server" 
                                        CommandName="CambiarEstado" 
                                        CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "btn btn-outline-danger btn-sm rounded-circle me-1" : "btn btn-outline-success btn-sm rounded-circle me-1" %>'
                                        ToolTip='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "Dar de Baja" : "Reactivar" %>'>
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnHistorial" runat="server" 
                                        CommandName="Historial" 
                                        CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass="btn btn-info btn-sm rounded-circle text-white" 
                                        ToolTip="Ver Historial de Movimientos">
                                        <i class="fa-solid fa-clock-rotate-left"></i>
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlFormularioIntegrante" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES</h3>
            <asp:LinkButton ID="btnCancelarIntTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-user-plus me-2"></i> <asp:Label runat="server" ID="lblTituloFormInt" Text="Nuevo Integrante" />
            </h4>
            <asp:HiddenField ID="hfIdIntEdit" runat="server" />
        
            <asp:HiddenField ID="hfCertificadoIntVinculado" runat="server" />
            <asp:HiddenField ID="hfIdDocenteInt" runat="server" />

            <div class="mb-4">
                <label class="form-label fw-bold text-primary small">TIPO DE INTEGRANTE</label>
    
                <asp:DropDownList ID="ddlTipoInt" runat="server" 
                                  CssClass="form-select shadow-sm border-primary"
                                  AutoPostBack="true" 
                                  OnSelectedIndexChanged="ddlTipoInt_SelectedIndexChanged">
                    <asp:ListItem Text="Interno (Administrativo/Estudiante)" Value="Interno" Selected="True"/>
                    <asp:ListItem Text="Docente UTC" Value="Docente"/>
                    <asp:ListItem Text="Externo (Colaborador)" Value="Externo" />
                </asp:DropDownList>
            </div>

            <div id="pnlSeleccionDocenteInt" style="display:none;" class="mb-4 p-4 bg-primary bg-opacity-10 rounded-4 border border-primary border-opacity-25 shadow-sm">
                <label class="form-label fw-bold text-primary small"> SELECCIONAR DOCENTE CATEGORIZADO</label>
                <div class="input-group">
                    <span class="input-group-text bg-white border-end-0"><i class="fa-solid fa-user-graduate text-primary"></i></span>
                    <asp:DropDownList ID="ddlDocentesCategorizados" runat="server" CssClass="form-select border-start-0 shadow-none" 
                        AutoPostBack="true" OnSelectedIndexChanged="ddlDocentesCategorizados_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="form-text mt-2 small text-muted">
                    <i class="fa-solid fa-circle-info me-1"></i> Solo aparecen docentes con categorización vigente.
                </div>
            </div>
        
            <asp:Panel ID="pnlDatosPersonalesInt" runat="server">
                <div class="row g-3">
                    <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>
            
                    <div class="col-12">
                        <label class="form-label">Cédula <span class="text-danger">*</span></label>
                        <div class="input-group gap-2">
                            <asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" MaxLength="15" autocomplete="off" placeholder="Ingrese Cédula" />
        
                            <asp:LinkButton ID="btnValidarCedulaInt" runat="server" 
                                            CssClass="btn btn-primary" 
                                            OnClick="btnValidarCedulaInt_Click"
                                            CausesValidation="false" 
                                            ToolTip="Validar Disponibilidad">
                                <i class="fa-solid fa-magnifying-glass"></i> Validar
                            </asp:LinkButton>
                        </div>
                    </div>
                    <div class="col-md-6"><label class="form-label">Nombres</label><asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control" autocomplete="off" /></div>
                    <div class="col-md-6"><label class="form-label">Apellidos</label><asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control" autocomplete="off" /></div>
                    <div class="col-md-12"><label class="form-label">Correo</label><asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off" /></div>
            
                    <div id="divInternoInt" class="col-12 row g-3 m-0 p-0">
                        <div class="col-md-6">
                            <label class="form-label">Facultad / Extensión</label>
                            <asp:DropDownList ID="ddlFacultadInt" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlFacultadInt_SelectedIndexChanged">
                                <asp:ListItem Text="-- Seleccione --" Value="" />
                                <asp:ListItem Value="CAREN">FACULTAD DE CIENCIAS AGROPECUARIAS (CAREN)</asp:ListItem>
                                <asp:ListItem Value="CIYA">FACULTAD DE CIENCIAS DE LA INGENIERIA (CIYA)</asp:ListItem>
                                <asp:ListItem Value="CAYE">FACULTAD DE CIENCIAS ADMINISTRATIVAS (CAYE)</asp:ListItem>
                                <asp:ListItem Value="CSAYE">FACULTAD DE CIENCIAS SOCIALES (CSAYE)</asp:ListItem>
                                <asp:ListItem Value="SALUD">FACULTAD CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                                <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                                <asp:ListItem Value="LAMANA">EXTENSION LA MANÁ</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Carrera / Departamento</label>
                            <asp:DropDownList ID="ddlCarreraInt" runat="server" CssClass="form-select">
                                <asp:ListItem Text="-- Seleccione Facultad Primero --" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>
            
                    <div id="divExternoInt" class="col-12" style="display:none;">
                        <label class="form-label">Institución / Entidad de Origen</label>
                        <asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control" placeholder="Ej: Universidad Central..." autocomplete="off" />
                    </div>

                    <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Datos del Grupo</h6></div>
            
                    <div class="col-md-6">
                        <label class="form-label">Función Asignada</label>
                        <div class="input-group shadow-sm">
                            <span class="input-group-text bg-primary-subtle text-primary border-primary border-opacity-25">
                                <i class="fa-solid fa-id-badge"></i>
                            </span>
                            <asp:TextBox ID="txtFuncionInt" runat="server" CssClass="form-control bg-light text-primary fw-bold border-primary border-opacity-25" ReadOnly="true" Text="Miembro Investigador"></asp:TextBox>
                        </div>
                        <div class="form-text small text-muted"><i class="fa-solid fa-circle-info me-1"></i> Rol definido automáticamente por el sistema.</div>
                    </div>
            
                    <div class="col-md-6"><label class="form-label">Fecha Inicio</label><asp:TextBox ID="dtFechaIniInt" runat="server" CssClass="form-control" TextMode="Date" /></div>
                </div>

                <div class="d-flex justify-content-center gap-3 mt-4">
                    <asp:LinkButton ID="btnGuardarInt" runat="server" 
                        CssClass="btn btn-primary btn-pill px-4" 
                        OnClientClick="return ValidarFormularioIntegrante();"
                        OnClick="btnGuardarInt_Click">
                        <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Integrante
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click" CausesValidation="false">
                        <i class="fa-solid fa-ban me-2"></i> Cancelar
                    </asp:LinkButton>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalProyectosDetalle" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 shadow-lg rounded-4 overflow-hidden">
            
                <div class="modal-header utc-header py-3">
                    <div class="d-flex align-items-center w-100">
                        <i class="fa-solid fa-briefcase text-white fs-4 me-3"></i>
                        <div>
                            <h5 class="modal-title text-white fw-bold mb-0" id="lblGrupoTitulo" runat="server">
                                PORTAFOLIO DE PROYECTOS
                            </h5>
                            <small class="text-white-50">Historial de vinculación académica</small>
                        </div>
                    </div>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body bg-light p-4">
                
                    <div class="card border-0 shadow-sm rounded-4">
                        <div class="card-body p-0 overflow-hidden">
                            <div class="table-responsive">
                                <div class="container-fluid p-0">
                                    <div class="d-flex flex-column gap-3">
        
                                        <asp:Repeater ID="rptProyectosDetalle" runat="server">
                                            <ItemTemplate>
                                                <div class="card border-0 shadow-sm rounded-3 overflow-hidden mb-0 transition-hover">
                                                    <div class="card-body p-0 d-flex">
                        
                                                        <div class='<%# "flex-shrink-0 " + (
                                                                Eval("strEstado_pro").ToString() == "Aprobado" ? "bg-success" : 
                                                                Eval("strEstado_pro").ToString() == "Rechazado" ? "bg-danger" : 
                                                                "bg-warning"
                                                            ) %>' style="width: 6px;">
                                                        </div>

                                                        <div class="p-3 w-100 d-flex flex-column flex-lg-row align-items-center gap-3">
                            
                                                            <div class="d-flex align-items-center justify-content-center rounded-circle bg-primary bg-opacity-10 text-primary flex-shrink-0" 
                                                                 style="width: 45px; height: 45px;">
                                                                <i class="fa-solid fa-folder-open fs-5"></i>
                                                            </div>

                                                            <div class="flex-grow-1 text-center text-lg-start w-100">
                                                                <h6 class="mb-1 fw-bold text-dark text-uppercase" style="font-size: 0.95rem; letter-spacing: 0.5px;">
                                                                    <%# Eval("strTema_pro") %>
                                                                </h6>
                                                                <div class="d-flex flex-wrap justify-content-center justify-content-lg-start gap-3 small text-muted">
                                                                    <span>
                                                                        <i class="fa-solid fa-user-tie me-1 text-primary"></i>
                                                                        <%# Eval("strCoordinador_pro") %>
                                                                    </span>
                                                                    <span class="d-none d-lg-block">|</span>
                                                                    <span>
                                                                        <i class="fa-regular fa-clock me-1"></i>
                                                                        <%# Eval("strDuracion_pro") %>
                                                                    </span>
                                                                    <span class="d-none d-lg-block">|</span>
                                                                    <span>
                                                                        <i class="fa-regular fa-calendar-check me-1"></i>
                                                                        <%# Convert.ToDateTime(Eval("dtFehains_pro")).ToString("dd MMM yyyy") %>
                                                                    </span>
                                                                </div>
                                                            </div>

                                                            <div class="d-flex align-items-center gap-3 mt-2 mt-lg-0">
                                                                <div class="text-end">
                                                                    <span class='<%# "badge rounded-pill px-3 py-2 " + (
                                                                            Eval("strEstado_pro").ToString() == "Aprobado" ? "bg-success" : 
                                                                            Eval("strEstado_pro").ToString() == "Rechazado" ? "bg-danger" : 
                                                                            "bg-warning text-dark"
                                                                        ) %>'>
                                                                        <%# Eval("strEstado_pro") %>
                                                                    </span>
                                                                    <div class="small fw-bold text-secondary mt-1" 
                                                                         style='<%# Eval("intPuntaje_pro") == DBNull.Value ? "display:none;" : "" %>'>
                                                                        Puntaje: <span class="text-dark"><%# Eval("intPuntaje_pro") %></span>
                                                                    </div>
                                                                </div>

                                                                <asp:LinkButton ID="btnVerArchivoPro" runat="server" 
                                                                    CommandArgument='<%# Eval("strArchivo_pro") %>'
                                                                    CssClass="btn btn-outline-primary btn-sm rounded-circle d-flex align-items-center justify-content-center"
                                                                    Style="width: 40px; height: 40px;"
                                                                    Visible='<%# !string.IsNullOrEmpty(Eval("strArchivo_pro").ToString()) %>'
                                                                    ToolTip="Ver Documento Adjunto">
                                                                    <i class="fa-solid fa-file-pdf fs-6"></i>
                                                                </asp:LinkButton>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <asp:Panel ID="pnlSinProyectos" runat="server" Visible="false" CssClass="text-center py-5">
                                            <div class="mb-3 text-muted opacity-50">
                                                <i class="fa-solid fa-folder-open fa-3x"></i>
                                            </div>
                                            <h6 class="text-muted fw-bold">Sin Historial de Proyectos</h6>
                                            <p class="small text-secondary">Este grupo no tiene proyectos vinculados actualmente.</p>
                                        </asp:Panel>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            
                <div class="modal-footer bg-light border-top-0">
                    <button type="button" class="btn btn-secondary btn-pill px-4" data-bs-dismiss="modal">
                        Cerrar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
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
                                                <span class='badge rounded-pill px-3 <%# Eval("strAccion").ToString() == "BAJA" ? "badge-baja" : (Eval("strAccion").ToString().Contains("NUEVO") ? "badge-alta" : "badge-historial") %>'>
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

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content border-0 rounded-4 shadow-lg">
                <div class="modal-header border-bottom-0 py-2 px-3 bg-dark text-white">
                    <h6 class="modal-title" id="lblTituloPreview">Vista Previa del Reporte</h6>
                    <div>
                        <button type="button" class="btn btn-sm btn-light me-2" onclick="imprimirReporte()"><i class="fa-solid fa-print"></i> Imprimir</button>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                    </div>
                </div>
                <div class="modal-body p-4" style="background: white; min-height: 500px;">
                    <div id="arealmpresion" class="report-paper" runat="server" ClientIDMode="Static">
                        <div class="header-hero-banner"> 
                            <img src="https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png" alt="UTC Logo" />
                        </div>
                        <div class="header-info-split">
                            <div class="info-left">
                                <span class="system-label">Sistema de Gestión de Investigación</span>
                                <h1 class="doc-title">Historial de Movimientos</h1>
                            </div>
        
                            <div class="info-right">
                                <div class="meta-group">
                                    <span class="meta-label">Referencia</span>
                                    <asp:Label ID="lblRefId" runat="server" CssClass="meta-value ref-highlight" Text="N/A"></asp:Label>
                                </div>
                                <div class="meta-group">
                                    <span class="meta-label">Fecha de Emisión</span>
                                    <span class="meta-value"><%= DateTime.Now.ToString("dd/MM/yyyy") %></span>
                                </div>
                            </div>
                        </div>
    
                        <div class="mt-5"></div>

                        <div class="researcher-card">
                            <div class="card-row">
                                <div class="card-item">
                                    <span class="label">INVESTIGADOR</span>
                                    <asp:Label ID="lblReporteNombre" runat="server" CssClass="value"></asp:Label>
                                </div>
                                <div class="card-item">
                                    <span class="label">IDENTIFICACIÓN</span>
                                    <asp:Label ID="lblReporteCedula" runat="server" CssClass="value"></asp:Label>
                                </div>
                            </div>
                            <div class="card-row">
                                <div class="card-item">
                                    <span class="label">ROL / FUNCIÓN</span>
                                    <asp:Label ID="lblReporteFuncion" runat="server" CssClass="value"></asp:Label>
                                </div>
                                <div class="card-item">
                                    <span class="label">ESTADO ACTUAL</span>
                                    <asp:Label ID="lblReporteEstado" runat="server" CssClass="value"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="timeline-container">
                            <h4 class="timeline-title">Registro Cronológico de Eventos</h4>
        
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
                                                    <div class="action-badge <%# Eval("strAccion").ToString() == "BAJA" ? "bad" : "good" %>">
                                                        <%# Eval("strAccion") %>
                                                    </div>
                                                    <p class="description">
                                                        <strong>Motivo:</strong> <%# Eval("strMotivo") %>
                                                    </p>
                                                    <div class="user-signature">
                                                        <i class="fa-solid fa-user-check"></i> Procesado por: <%# Eval("strUsuario") %>
                                                    </div>
                                                </div>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>

                        <div class="report-legal-footer" style="margin-top: 80px;">
                            Documento generado automáticamente por el Sistema de Gestión CGI-UTC. 
                            La validez de este reporte está sujeta a los registros digitales institucionales.
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEstadoInt" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100"><i class="fa-solid fa-power-off me-2"></i> Cambio de Estado</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <p class="mb-3 text-center fs-5">¿Estás seguro de <strong id="accionEstadoTexto" class="text-primary">cambiar el estado</strong>?</p>
                    <div class="mb-3">
                        <asp:HiddenField ID="hfMotivoEstado" runat="server" ClientIDMode="Static" />
                        <label class="form-label fw-bold">Motivo del cambio</label>
                        <textarea id="txtMotivoEstado" class="form-control" rows="3" placeholder="Ingrese el motivo obligatorio..."></textarea>
                    </div>
                    <div class="bg-light p-3 rounded border small">
                        <p class="mb-1"><strong>Nombre:</strong> <span id="infoNombre"></span></p>
                        <p class="mb-1"><strong>Cédula:</strong> <span id="infoCedula"></span></p>
                        <p class="mb-1"><strong>Función:</strong> <span id="infoFuncion"></span></p>
                        <p class="mb-1"><strong>Estado actual:</strong> <span id="infoEstado"></span></p>
                    </div>
                    <asp:HiddenField ID="hfIdIntegranteEstado" runat="server" ClientIDMode="Static" />
                </div>
                <div class="modal-footer justify-content-center">
                    <asp:LinkButton ID="btnConfirmarCambioEstado" runat="server" CssClass="btn btn-pill btn-danger px-4" OnClientClick="return guardarMotivo();" OnClick="btnConfirmarCambioEstado_Click">
                        Confirmar Cambio
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalCoordinador" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content border-0 shadow-utc rounded-4">
                
                <div class="modal-header bg-utc text-white py-3">
                    <h5 class="modal-title fw-bold"><i class="fa-solid fa-user-tie me-2"></i> Nuevo Coordinador</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <asp:HiddenField ID="HiddenField1" runat="server" />
                <asp:HiddenField ID="HiddenField2" runat="server" />
                <asp:HiddenField ID="HiddenField3" runat="server" />
                <asp:HiddenField ID="HiddenField4" runat="server" />
                <asp:HiddenField ID="HiddenField5" runat="server" />
                <asp:HiddenField ID="HiddenField6" runat="server" />
                <asp:HiddenField ID="HiddenField7" runat="server" />
                <asp:HiddenField ID="hfCoordTipo" runat="server" />
                <asp:HiddenField ID="hfCoordEntidad" runat="server" />
                <asp:HiddenField ID="hfCoordIdDocente" runat="server" />

                <div class="modal-body bg-white p-4">
                
                    <div class="row align-items-center mb-3">
                        <div class="col-md-4">
                            <label class="form-label fw-bold text-dark mb-0">
                                Tipo de Vinculación:
                            </label>
                        </div>
                        <div class="col-md-8">
                            <asp:DropDownList ID="ddlTipoCoord" runat="server" 
                                              CssClass="form-select border-secondary shadow-none" 
                                              AutoPostBack="true" 
                                              OnSelectedIndexChanged="ddlTipoCoord_SelectedIndexChanged">
                                <asp:ListItem Value="Interno">INTERNO (Administrativo / Otro)</asp:ListItem>
                                <asp:ListItem Value="Docente">DOCENTE UTC (Búsqueda Automática)</asp:ListItem>
                                <asp:ListItem Value="Externo">EXTERNO (Otra Institución)</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <hr class="text-muted opacity-25 my-4">

                    <div id="pnlBusquedaDocente" style="display:none;" class="mb-4">
                        <div class="bg-light p-3 rounded-3 border border-primary border-opacity-25">
                            <label class="form-label fw-bold text-primary small mb-2">
                                <i class="fa-solid fa-user-tie me-1"></i> SELECCIONAR DOCENTE UTC
                            </label>
                            <div class="input-group">
                                <span class="input-group-text bg-white text-primary"><i class="fa-solid fa-magnifying-glass"></i></span>
                                <asp:DropDownList ID="ddlDocentesCoord" runat="server" CssClass="form-select" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlDocentesCoord_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="form-text small mt-1 text-muted">Seleccione un docente de la lista para autocompletar sus datos.</div>
                        </div>
                        <hr class="text-muted opacity-25 my-4">
                    </div>

                    <asp:Panel ID="pnlDatosPersonalesCoord" runat="server" style="display:none;">
                        
                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-regular fa-id-card me-2"></i> Información Personal
                        </h6>

                        <div class="row g-3 mb-4">
                            <div class="col-12">
                                <label class="form-label small text-muted">Cédula <span class="text-danger">*</span></label>
                                <div class="input-group gap-2">
                                    <asp:TextBox ID="txtCedulaCoord" runat="server" CssClass="form-control bg-light" MaxLength="15" autocomplete="off" />
        
                                    <asp:LinkButton ID="btnValidarCedulaCoord" runat="server" 
                                                    CssClass="btn btn-primary" 
                                                    OnClick="btnValidarCedulaCoord_Click"
                                                    CausesValidation="false">
                                        <i class="fa-solid fa-magnifying-glass"></i> Validar
                                    </asp:LinkButton>
                                </div>
                            </div>
                            
                            <div class="col-md-6">
                                <label class="form-label small text-muted">Nombres <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNombreCoord" runat="server" CssClass="form-control form-control-sm" autocomplete="off"/>
                                <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombreCoord" ErrorMessage="Requerido" Display="Dynamic" CssClass="text-danger small fw-bold" ValidationGroup="Coord" />
                            </div>

                            <div class="col-md-6">
                                <label class="form-label small text-muted">Apellidos <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtApellidoCoord" runat="server" CssClass="form-control form-control-sm" autocomplete="off"/>
                                <asp:RequiredFieldValidator ID="rfvApellido" runat="server" ControlToValidate="txtApellidoCoord" ErrorMessage="Requerido" Display="Dynamic" CssClass="text-danger small fw-bold" ValidationGroup="Coord" />
                            </div>

                            <div class="col-12">
                                <label class="form-label small text-muted">Correo Electrónico <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtCorreoCoord" runat="server" CssClass="form-control form-control-sm" TextMode="Email" autocomplete="off"/>
                                <asp:RequiredFieldValidator ID="rfvCorreo" runat="server" ControlToValidate="txtCorreoCoord" ErrorMessage="Requerido" Display="Dynamic" CssClass="text-danger small fw-bold" ValidationGroup="Coord" />
                            </div>
                        </div>

                        <hr class="text-muted opacity-25 my-4">

                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-solid fa-building-columns me-2"></i> Afiliación Institucional
                        </h6>

                        <div id="divInterno" class="row g-3 mb-4">
                            <div class="col-md-6">
                                <label class="form-label small text-muted">Facultad / Extensión</label>
                                <asp:DropDownList ID="ddlFacultadCoord" runat="server" CssClass="form-select form-select-sm" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlFacultadCoord_SelectedIndexChanged">
                                    <asp:ListItem Text="-- Seleccione --" Value="" />
                                    <asp:ListItem Value="CAREN">FACULTAD DE CIENCIAS AGROPECUARIAS (CAREN)</asp:ListItem>
                                    <asp:ListItem Value="CIYA">FACULTAD DE CIENCIAS DE LA INGENIERIA (CIYA)</asp:ListItem>
                                    <asp:ListItem Value="CAYE">FACULTAD DE CIENCIAS ADMINISTRATIVAS (CAYE)</asp:ListItem>
                                    <asp:ListItem Value="CSAYE">FACULTAD DE CIENCIAS SOCIALES (CSAYE)</asp:ListItem>
                                    <asp:ListItem Value="SALUD">FACULTAD CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                                    <asp:ListItem Value="PUJILI">EXTENSIÓN PUJILÍ</asp:ListItem>
                                    <asp:ListItem Value="LAMANA">EXTENSION LA MANÁ</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label small text-muted">Carrera / Departamento</label>
                                <asp:DropDownList ID="ddlCarreraCoord" runat="server" CssClass="form-select form-select-sm">
                                    <asp:ListItem Text="-- Seleccione Facultad Primero --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div id="divExterno" class="row g-3 mb-4" style="display:none;">
                            <div class="col-12">
                                <label class="form-label small text-muted">Entidad de Origen</label>
                                <asp:TextBox ID="txtEntidadCoord" runat="server" CssClass="form-control form-control-sm" placeholder="Ej: Universidad Central del Ecuador" autocomplete="off"/>
                                <asp:RequiredFieldValidator ID="rfvEntidad" runat="server" ControlToValidate="txtEntidadCoord" ErrorMessage="Requerido" Display="Dynamic" CssClass="text-danger small fw-bold" ValidationGroup="Coord" Enabled="false" />
                            </div>
                        </div>

                        <hr class="text-muted opacity-25 my-4">

                        <h6 class="text-primary fw-bold mb-3 small text-uppercase">
                            <i class="fa-solid fa-file-contract me-2"></i> Sustento Legal
                        </h6>
            
                        <asp:Panel ID="pnlCargaArchivo" runat="server">
                            <div class="utc-fileinput-wrapper p-3 border rounded-3" id="wrapperArchivoCoord">
                                <div class="utc-fileinput-header mb-2">
                                    <div class="d-flex justify-content-between align-items-center w-100">
                                        <span class="utc-fileinput-name small text-muted">Seleccione archivo PDF...</span>
                                        <button type="button" class="btn btn-outline-danger btn-sm py-0 px-2 remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                    </div>
                                </div>
                                <div class="utc-fileinput-preview" id="previewArchivoCoord"></div>
                                <div class="utc-fileinput-loader" id="loaderArchivoCoord"><i class="fa-solid fa-spinner fa-spin me-2"></i></div>
                                <div class="utc-dropzone mt-0 py-3 bg-light border-dashed text-center" id="dropzoneArchivoCoord">
                                    <i class="fa-solid fa-cloud-arrow-up fa-lg text-primary mb-1"></i>
                                    <div class="small text-muted">Clic para subir resolución</div>
                                </div>
                                <asp:FileUpload ID="flpArchivoCoord" runat="server" CssClass="utc-fileinput-input" accept=".pdf,.doc,.docx" />
                            </div>
                        </asp:Panel>

                        <asp:Panel ID="pnlArchivoRecuperado" runat="server" Visible="false">
                            <div class="d-flex align-items-center justify-content-between p-3 bg-light border rounded-3">
                                <div class="d-flex align-items-center">
                                    <i class="fa-solid fa-file-pdf text-danger fs-4 me-3"></i>
                                    <div>
                                        <h6 class="fw-bold mb-0 small text-dark">Documento Vinculado</h6>
                                        <small class="text-muted" style="font-size:0.75rem;">Archivo recuperado del sistema.</small>
                                    </div>
                                </div>
                                <div>
                                    <asp:HyperLink ID="lnkVerArchivo" runat="server" Target="_blank" CssClass="btn btn-sm btn-link text-decoration-none fw-bold">Ver</asp:HyperLink>
                                    <asp:LinkButton ID="btnCambiarArchivo" runat="server" CssClass="btn btn-sm btn-light text-danger border" OnClick="btnCambiarArchivo_Click">
                                        <i class="fa-solid fa-trash-can"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </asp:Panel>

                    </asp:Panel>
                    
                </div>

                <div class="modal-footer border-top-0 bg-white rounded-bottom-4 py-4">
                    <div class="row w-100 m-0">
                        <div class="col-12 text-center">
                            <asp:LinkButton ID="btnGuardarCoordModal" runat="server" 
                                CssClass="btn btn-primary btn-pill px-5 shadow fw-bold" 
                                OnClientClick="if(!ValidarModalCoordinador()) return false;" 
                                OnClick="btnGuardarCoordModal_Click" 
                                ValidationGroup="Coord">
                                <i class="fa-solid fa-check me-2"></i> ASIGNAR COORDINADOR
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <%-- MODAL PAPELERA PREMIUM: INTEGRANTES --%>
    <div class="modal fade" id="modalPapeleraIntegrantes" tabindex="-1" aria-hidden="true" data-bs-backdrop="static">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content utc-modal-premium">
                
                <div class="modal-header papelera-header-premium d-flex flex-column align-items-center justify-content-center text-white position-relative">
                    <button type="button" class="btn-close btn-close-white position-absolute top-0 end-0 m-4" data-bs-dismiss="modal"></button>
                    <div class="bg-white bg-opacity-25 rounded-circle p-3 mb-3 backdrop-blur">
                        <i class="fa-solid fa-trash-arrow-up fa-2x"></i>
                    </div>
                    <h4 class="fw-bold mb-1">Papelera de Integrantes</h4>
                    <p class="mb-0 small opacity-75">Recuperación de miembros dados de baja</p>
                </div>

                <div class="modal-body p-4 bg-light">
                    <asp:Repeater ID="rptPapeleraIntegrantes" runat="server" OnItemCommand="rptPapeleraIntegrantes_ItemCommand">
                        <ItemTemplate>
                            <div class="docente-trash-card p-3">
                                <div class="d-flex align-items-center justify-content-between mb-3">
                                    <div class="d-flex align-items-center gap-3">
                                        <div class="bg-light rounded-circle p-3 text-secondary border">
                                            <i class="fa-solid fa-user-xmark fa-lg"></i>
                                        </div>
                                        <div>
                                            <h6 class="fw-bold text-dark mb-1 text-uppercase">
                                                <%# Eval("strApellidos_int") %> <%# Eval("strNombres_int") %>
                                            </h6>
                                            <span class="status-badge-inactive">INACTIVO</span>
                                        </div>
                                    </div>
                                    
                                    <asp:LinkButton runat="server" CommandName="restaurar" CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass="btn btn-sm btn-success rounded-pill px-4 shadow-sm fw-bold" 
                                        OnClientClick="return confirm('¿Está seguro de restaurar a este integrante al grupo?');">
                                        <i class="fa-solid fa-rotate-left me-2"></i> RESTAURAR
                                    </asp:LinkButton>
                                </div>

                                <div class="d-flex mt-2 pt-3 border-top bg-white text-center">
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Cédula</span>
                                        <span class="value-bold"><%# Eval("strCedula_int") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Función</span>
                                        <span class="value-bold text-primary"><%# Eval("strFuncion_int") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Tipo</span>
                                        <span class="value-bold"><%# Eval("strTipo_int") %></span>
                                    </div>
                                    <div class="data-grid-item flex-fill">
                                        <span class="label-mini">Origen</span>
                                        <span class="value-bold text-muted" style="font-size: 0.7rem;">
                                            <%# Eval("strTipo_int").ToString() == "Externo" ? Eval("strEntidad_int") : Eval("strFacultad_int") %>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Panel ID="pnlVacio" runat="server" Visible='<%# rptPapeleraIntegrantes.Items.Count == 0 %>'>
                                <div class="text-center py-5">
                                    <div class="mb-3 text-muted opacity-25">
                                        <i class="fa-solid fa-trash-can fa-4x"></i>
                                    </div>
                                    <h6 class="fw-bold text-secondary">Papelera Vacía</h6>
                                    <p class="text-muted small mb-0">No hay integrantes inactivos en este grupo.</p>
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

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">

        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            order: [],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        Sys.Application.add_load(function () {
            initTable('#tablaGrupos');
            initTable('#tablaIntegrantes');

            if (typeof UTC_FileInput === 'function') {
                initFileInput('wrapperArchivoGrupo', '<%= flpArchivoGrupo.ClientID %>');

                if (document.getElementById('wrapperArchivoCoord')) {
                    UTC_FileInput({
                        wrapper: "wrapperArchivoCoord", dropzone: "dropzoneArchivoCoord",
                        preview: "previewArchivoCoord", loader: "loaderArchivoCoord",
                        input: "<%= flpArchivoCoord.ClientID %>"
                    });
                }
            }
        });

        function initTable(id) {
            const $table = $(id);
            if ($table.length) {
                if ($.fn.DataTable.isDataTable(id)) $table.DataTable().destroy();
                $table.DataTable(dtConfig);
            }
        }

        function initFileInput(wrapperId, inputId) {
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

        function previewImage(input, imgId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.getElementById(imgId);
                    if (img) { img.src = e.target.result; img.style.display = 'block'; }
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        function RenderizarEstadoVisual(tipo) {
            var pnlSeleccion = document.getElementById('pnlSeleccionDocenteInt');
            var divInterno = document.getElementById('divInternoInt');
            var divExterno = document.getElementById('divExternoInt');
            var pnlDatos = document.getElementById('<%= pnlDatosPersonalesInt.ClientID %>');

            var hfIdDoc = document.getElementById('<%= hfIdDocenteInt.ClientID %>');
            var hayDocenteSeleccionado = hfIdDoc && hfIdDoc.value !== "";

            if (tipo === 'Docente') {

                if (hayDocenteSeleccionado) {
                    if (pnlSeleccion) pnlSeleccion.style.display = 'block';

                    if (pnlDatos) pnlDatos.style.display = 'block';
                } else {
                    if (pnlSeleccion) pnlSeleccion.style.display = 'block';
                    if (pnlDatos) pnlDatos.style.display = 'none';
                }

                if (divInterno) divInterno.style.display = 'flex';
                if (divExterno) divExterno.style.display = 'none';
            }
            else if (tipo === 'Externo') {
                if (pnlSeleccion) pnlSeleccion.style.display = 'none';
                if (pnlDatos) pnlDatos.style.display = 'block';
                if (divInterno) divInterno.style.display = 'none';
                if (divExterno) divExterno.style.display = 'block';
            }
            else { 
                if (pnlSeleccion) pnlSeleccion.style.display = 'none';
                if (pnlDatos) pnlDatos.style.display = 'block';
                if (divInterno) divInterno.style.display = 'flex';
                if (divExterno) divExterno.style.display = 'none';
            }
        }

        function RenderizarModalCoord(tipo) {
            var pnlBusqueda = document.getElementById('pnlBusquedaDocente');
            var pnlDatos = document.getElementById('<%= pnlDatosPersonalesCoord.ClientID %>');
            var divInterno = document.getElementById('divInterno');
            var divExterno = document.getElementById('divExterno');

            var hfIdDoc = document.getElementById('<%= hfCoordIdDocente.ClientID %>');
            var hayDocenteSeleccionado = hfIdDoc && hfIdDoc.value !== "";

            abrirModalCoord(); 

            if (tipo === 'Docente') {

                if (hayDocenteSeleccionado) {
                    if (pnlBusqueda) pnlBusqueda.style.display = 'block';

                    if (pnlDatos) pnlDatos.style.display = 'block';
                } else {
                    if (pnlBusqueda) pnlBusqueda.style.display = 'block';
                    if (pnlDatos) pnlDatos.style.display = 'none';
                }

                if (divInterno) divInterno.style.display = 'flex';
                if (divExterno) divExterno.style.display = 'none';
            }
            else if (tipo === 'Externo') {
                if (pnlBusqueda) pnlBusqueda.style.display = 'none';
                if (pnlDatos) pnlDatos.style.display = 'block';
                if (divInterno) divInterno.style.display = 'none';
                if (divExterno) divExterno.style.display = 'block';
            }
            else { 
                if (pnlBusqueda) pnlBusqueda.style.display = 'none';
                if (pnlDatos) pnlDatos.style.display = 'block';
                if (divInterno) divInterno.style.display = 'flex';
                if (divExterno) divExterno.style.display = 'none';
            }
        }

        function abrirModalCoord() {
            var el = document.getElementById('modalCoordinador');
            if (el) {
                var modal = bootstrap.Modal.getOrCreateInstance(el);
                modal.show();
            }
        }

        function cerrarModalCoord() {
            var el = document.getElementById('modalCoordinador');
            if (el) {
                var modal = bootstrap.Modal.getInstance(el);
                if (modal) modal.hide();
            }
        }

        function AbrirModalEstado() {
            var el = document.getElementById('modalEstadoInt');
            if (el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }

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

        function esEmailValido(email) {
            if (email === "") return true;
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
        }

        function ValidarFormularioIntegrante() {
            var ddlTipo = document.getElementById('<%= ddlTipoInt.ClientID %>');
            var idCedula = '<%= txtCedulaInt.ClientID %>';
            var idCorreo = '<%= txtCorreoInt.ClientID %>';

            if (ddlTipo && ddlTipo.value === 'Docente') {
                var hfIdDoc = document.getElementById('<%= hfIdDocenteInt.ClientID %>');
                 if(!hfIdDoc || hfIdDoc.value === "") {
                     toastify('ww', 'Debe seleccionar un docente de la lista.', 'Validación');
                     return false;
                 }
                 return true;
            }

            var valCedula = document.getElementById(idCedula).value.trim();
            if (valCedula === "") { 
                 mostrarError(idCedula, 'La cédula es obligatoria.');
                 return false;
            }

            var valCorreo = document.getElementById(idCorreo).value.trim();
            if (!esEmailValido(valCorreo) || valCorreo === "") {
                mostrarError(idCorreo, 'El correo electrónico es inválido o está vacío.');
                return false;
            }
            return true;
        }

        function ValidarModalCoordinador() {
            var ddlTipo = document.getElementById('<%= ddlTipoCoord.ClientID %>');
        
            if (ddlTipo && ddlTipo.value === 'Docente') {
                 var hfIdDoc = document.getElementById('<%= hfCoordIdDocente.ClientID %>');
                 if(!hfIdDoc || hfIdDoc.value === "") {
                     toastify('ww', 'Seleccione un docente de la lista.', 'Validación');
                     return false;
                 }
                 var email = document.getElementById('<%= txtCorreoCoord.ClientID %>').value;
                 if(email.trim() === ""){
                     mostrarError('<%= txtCorreoCoord.ClientID %>', 'El correo es obligatorio.');
                     return false;
                 }
                 return true;
            }

            if (ddlTipo && ddlTipo.value !== 'Docente') {
                var idCedula = '<%= txtCedulaCoord.ClientID %>';
                var valCedula = document.getElementById(idCedula).value.trim();
                if (valCedula === "") {
                    mostrarError(idCedula, 'La cédula del coordinador es obligatoria.');
                    return false;
                }
            }
            return true;
        }

        function ValidarFormularioGrupo() {
            var nombre = document.getElementById('<%= txtNombreGru.ClientID %>').value;
            if(nombre.trim() === "") {
                mostrarError('<%= txtNombreGru.ClientID %>', 'Ingrese el nombre del grupo.');
                return false;
            }
            return true;
        }

        function guardarMotivo() {
            var txt = document.getElementById('txtMotivoEstado');
            var hf = document.getElementById('<%= hfMotivoEstado.ClientID %>');
            if (txt && hf) {
                if (!txt.value.trim()) { alert('Ingrese un motivo'); return false; }
                hf.value = txt.value.trim();
                return true;
            }
            return false;
        }

        function imprimirReporte() {
            var contenido = document.getElementById("arealmpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');

            ventana.document.write('<html><head><title>Reporte de Historial</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');

            ventana.document.write('<style>');
            ventana.document.write('body { font-family: "Segoe UI", sans-serif; -webkit-print-color-adjust: exact; print-color-adjust: exact; }');
            ventana.document.write('.report-paper { padding: 40px 50px; }');
            ventana.document.write('.header-hero-banner { background-color: #003876 !important; color: white !important; margin: -40px -50px 40px -50px; padding: 50px 20px 30px 20px; display: flex !important; justify-content: center !important; align-items: center !important; border-bottom: 6px solid #002a5c; }');
            ventana.document.write('.header-hero-banner img { height: 80px; width: auto; filter: brightness(0) invert(1); display: block; }');
            ventana.document.write('.header-info-split { display: flex; justify-content: space-between; border-bottom: 2px solid #003876; margin-bottom: 40px; padding-bottom: 25px; }');
            ventana.document.write('.doc-title { color: #003876; font-weight: 900; font-size: 2rem; text-transform: uppercase; }');
            ventana.document.write('.researcher-card { background-color: #f8faff; border-left: 4px solid #003876; padding: 20px; margin-bottom: 40px; border: 1px solid #e1e8f0; border-radius: 6px; }');
            ventana.document.write('.card-row { display: flex; justify-content: space-between; margin-bottom: 15px; }');
            ventana.document.write('.card-item .label { font-size: 0.7rem; color: #8898aa; font-weight: 700; display: block; text-transform: uppercase; }');
            ventana.document.write('.card-item .value { font-size: 1rem; font-weight: 600; color: #002a5c; }');
            ventana.document.write('.timeline-container { padding: 0 10px; }');
            ventana.document.write('.timeline-list { list-style: none; padding: 0; position: relative; }');
            ventana.document.write('.timeline-list::before { content: ""; position: absolute; top: 0; bottom: 0; left: 24px; width: 2px; background: #e9ecef; }');
            ventana.document.write('.timeline-item { position: relative; padding-left: 60px; margin-bottom: 30px; }');
            ventana.document.write('.timeline-marker { position: absolute; left: 18px; top: 0; width: 14px; height: 14px; border-radius: 50%; background: #fff; border: 3px solid #003876; z-index: 2; }');
            ventana.document.write('.action-badge { display: inline-block; padding: 4px 10px; border-radius: 4px; font-size: 0.7rem; font-weight: 800; text-transform: uppercase; margin-bottom: 6px; }');
            ventana.document.write('.action-badge.good { background: rgba(25, 135, 84, 0.1); color: #198754; }');
            ventana.document.write('.action-badge.bad { background: rgba(220, 53, 69, 0.1); color: #dc3545; }');
            ventana.document.write('.report-legal-footer { margin-top: 60px; border-top: 1px solid #eee; text-align: center; font-size: 0.65rem; color: #ccc; padding-top: 20px; }');
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

    <script>    
        function triggerFotoUpload() {
            var fileInput = document.getElementById('<%= flpFotoGrupo.ClientID %>');
            if(fileInput) fileInput.click();
        }

        function previewAvatar(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
            
                reader.onload = function (e) {
                    var img = document.getElementById('<%= imgFotoVisual.ClientID %>');
                    if (img) {
                        img.src = e.target.result;
                    }
                }

                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>

</asp:Content>