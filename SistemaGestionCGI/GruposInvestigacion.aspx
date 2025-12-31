<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GruposInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.GruposInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- =====================================================================
         RECURSOS DE ESTILO (UTC DESIGN)
         ===================================================================== --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <style>
        /* Ajustes específicos para Modales y Tablas */
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
        .img-avatar-table {
            width: 40px; height: 40px; object-fit: cover; border-radius: 50%; 
            border: 2px solid #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        .form-stack { max-width: 100% !important; }
    </style>

    <%-- =====================================================================
         ENCABEZADO PRINCIPAL (Visible en Vistas de Listado)
         ===================================================================== --%>
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

    <%-- =====================================================================
         PANEL 1: LISTADO DE GRUPOS (GRILLA)
         ===================================================================== --%>
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
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Está seguro de eliminar este grupo?');" ToolTip="Eliminar"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- =====================================================================
         PANEL 2: FORMULARIO UNIFICADO DE GRUPO (CREAR / EDITAR)
    ===================================================================== --%>
    <asp:Panel ID="pnlFormularioGrupo" runat="server" Visible="false">
    
        <%-- 1. ENCABEZADO DEL FORMULARIO (BOTÓN REGRESAR ARRIBA) --%>
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-people-group me-2"></i> 
                <asp:Label ID="lblTituloFormulario" runat="server" Text="Gestión de Grupo"></asp:Label>
            </h3>
            <asp:LinkButton ID="btnRegresarTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <%-- 2. CARD DEL FORMULARIO --%>
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
        
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-file-pen me-2"></i> Datos del Grupo
            </h4>
        
            <asp:HiddenField ID="hfIdGrupo" runat="server" />
            <asp:HiddenField ID="hfFotoActual" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />

            <div class="row g-3">
                <%-- NUEVO CAMPO: CENTRO DE INVESTIGACIÓN --%>
                <div class="col-12">
                    <label class="form-label">Centro de Investigación</label>
                    <asp:DropDownList ID="ddlCentro" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="col-12">
                    <label class="form-label">Nombre del Grupo <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombreGru" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
            
                <div class="col-12">
                    <label class="form-label">Coordinador</label>
                    <asp:TextBox ID="txtCoordinadorGru" runat="server" CssClass="form-control" autocomplete="off" />
                </div>
            
                <div class="col-md-6">
                    <label class="form-label">Fecha de Creación</label>
                    <asp:TextBox ID="txtFechaCreaGru" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
            
                <div class="col-md-6">
                    <label class="form-label">Categoría</label>
                    <asp:DropDownList ID="ddlCategoriaGru" runat="server" CssClass="form-select">
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem>
                        <asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-12">
                    <label class="form-label">Línea de Investigación</label>
                    <asp:DropDownList ID="ddlLineaInv" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Tecnologias de la informacion y comunicacion (TICS)">Tecnologias de la informacion y comunicacion (TICS)</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Sublínea de Investigación</label>
                    <asp:DropDownList ID="ddlSublineaInv" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Inteligencia artificial e inteligencia de negocios">Inteligencia artificial e inteligencia de negocios</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- FOTO DEL GRUPO --%>
                <div class="col-12 text-center mt-4">
                    <label class="form-label fw-bold d-block">Foto del Grupo</label>
                    <asp:Image ID="imgFotoActual" runat="server" CssClass="img-thumbnail rounded-circle mb-2" Width="100" Height="100" Visible="false" />
                    <div class="d-flex justify-content-center">
                        <div class="col-md-6">
                            <asp:FileUpload ID="flpFotoGrupo" runat="server" CssClass="form-control form-control-sm" onchange="previewImage(this, 'previewFoto')" />
                        </div>
                    </div>
                    <img id="previewFoto" src="#" class="img-thumbnail rounded-circle mt-2" style="width:100px; height:100px; object-fit:cover; display:none;" />
                </div>

                <%-- ARCHIVO DE RESOLUCIÓN --%>
                <div class="col-12 mt-4">
                    <label class="form-label fw-semibold">Archivo de Resolución</label>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoGrupo">
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
                        <div class="utc-dropzone" id="dropzoneArchivoGrupo"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra archivo aquí</div>
                        <asp:FileUpload ID="flpArchivoGrupo" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>

            <%-- BOTONES INFERIORES --%>
            <div class="d-flex justify-content-center gap-3 mt-5">
                <asp:LinkButton ID="btnGuardarGrupo" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow-sm" OnClick="btnGuardarGrupo_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Datos
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarGrupo" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- =====================================================================
         PANEL 3: GESTIÓN DE INTEGRANTES (LISTADO)
         ===================================================================== --%>
    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        <asp:HiddenField ID="hfGrupoIdActual" runat="server" />
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES</h3>
            <div class="d-flex gap-2">
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
                            <%-- 1. FILA CON ESTILO CONDICIONAL (Gris si está inactivo) --%>
                            <tr class='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "" : "table-secondary text-muted" %>'>
                        
                                <td><%# Eval("strId_int") %></td>
                        
                                <%-- Nombre en Negrita y alineado a la izquierda --%>
                                <td class="text-start fw-semibold text-primary">
                                    <%# Eval("strApellidos_int") + " " + Eval("strNombres_int") %>
                                </td>
                        
                                <td class="text-start"><%# Eval("strFuncion_int") %></td>
                        
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_int")).ToString("dd/MM/yyyy") %></td>
                        
                                <td>
                                    <%# Eval("dtFechafin_int") == DBNull.Value ? "-" : Convert.ToDateTime(Eval("dtFechafin_int")).ToString("dd/MM/yyyy") %>
                                </td>
                        
                                <%-- 2. BADGES DE ESTADO CON ICONOS --%>
                                <td>
                                    <%# Convert.ToBoolean(Eval("bitActivo_int")) 
                                        ? "<span class='badge bg-success'><i class='fa-solid fa-check me-1'></i>Activo</span>" 
                                        : "<span class='badge bg-danger'><i class='fa-solid fa-ban me-1'></i>Inactivo</span>" 
                                    %>
                                </td>
                        
                                <%-- 3. BOTONES DE ACCIÓN (DISEÑO REPLICADO) --%>
                                <td>
                                    <%-- Botón Ver Certificado (Solo si aplica) --%>
                                    <asp:LinkButton ID="btnVerCertificado" runat="server" 
                                        CommandName="VerCertificado" 
                                        CommandArgument='<%# Eval("strCertificado_int") %>'
                                        Visible='<%# Eval("strFuncion_int").ToString() == "Investigador Principal" && !string.IsNullOrEmpty(Eval("strCertificado_int") as string) %>'
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" 
                                        ToolTip="Ver Certificado">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>

                                    <%-- Botón Editar --%>
                                    <asp:LinkButton ID="btnEditarInt" runat="server" 
                                        CommandName="EditarInt" 
                                        CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" 
                                        ToolTip="Editar Datos">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>

                                    <%-- Botón Toggle Estado (Dinámico: Outline Rojo si está activo, Verde si está inactivo) --%>
                                    <asp:LinkButton ID="btnToggleEstado" runat="server" 
                                        CommandName="CambiarEstado" 
                                        CommandArgument='<%# Eval("strId_int") %>' 
                                        CssClass='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "btn btn-outline-danger btn-sm rounded-circle me-1" : "btn btn-outline-success btn-sm rounded-circle me-1" %>'
                                        ToolTip='<%# Convert.ToBoolean(Eval("bitActivo_int")) ? "Dar de Baja" : "Reactivar" %>'>
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>

                                    <%-- Botón Historial (Estilo Info Azul) --%>
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

    <%-- =====================================================================
         PANEL 4: FORMULARIO INTEGRANTE (CREAR / EDITAR)
         ===================================================================== --%>
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
            
            <div class="row g-3">
                <div class="col-12"><h6 class="text-primary fw-bold border-bottom pb-2">Datos Personales</h6></div>
                <div class="col-md-4"><label class="form-label">Cédula</label><asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" MaxLength="15" autocomplete="off"/></div>
                <div class="col-md-4"><label class="form-label">Nombres</label><asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-md-4"><label class="form-label">Apellidos</label><asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-md-6"><label class="form-label">Correo</label><asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email" autocomplete="off" /></div>
                <div class="col-md-6">
                    <label class="form-label">Tipo de Integrante</label>
                    <asp:DropDownList ID="ddlTipoInt" runat="server" CssClass="form-select" onchange="ToggleTipoIntegrante(this)">
                        <asp:ListItem Text="Interno (UTC)" Value="Interno" Selected="True"/>
                        <asp:ListItem Text="Externo (Colaborador)" Value="Externo" />
                    </asp:DropDownList>
                </div>
                
                <%-- Campos condicionales --%>
                <div id="divInterno" class="col-12 row g-3 m-0 p-0" runat="server" ClientIDMode="Static">
                    <div class="col-md-6"><label class="form-label">Carrera / Departamento</label><asp:TextBox ID="txtCarreraInt" runat="server" CssClass="form-control" autocomplete="off" /></div>
                    <div class="col-md-6">
                        <label class="form-label">Facultad / Extensión</label>
                        <asp:DropDownList ID="ddlFacultadInt" runat="server" CssClass="form-select">
                            <asp:ListItem Text="-- Seleccione --" Value="" />
                            <asp:ListItem>FACULTAD DE CIENCIAS AGROPECUARIAS Y RECURSOS NATURALES (CAREN)</asp:ListItem>
                            <asp:ListItem>FACULTAD DE CIENCIAS DE LA INGENIERIA Y APLICADAS (CIYA)</asp:ListItem>
                            <asp:ListItem>FACULTAD DE CIENCIAS ADMINISTRATIVAS Y ECONOMICAS (CAYE)</asp:ListItem>
                            <asp:ListItem>FACULTAD DE CIENCIAS SOCIALES ARTES Y EDUCACION (CSAYE)</asp:ListItem>
                            <asp:ListItem>FACULTAD CIENCIAS DE LA SALUD (CS)</asp:ListItem>
                            <asp:ListItem>EXTENSIÓN PUJILÍ</asp:ListItem>
                            <asp:ListItem>EXTENSION LA MANÁ</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div id="divExterno" class="col-12" style="display:none;" runat="server" ClientIDMode="Static">
                    <label class="form-label">Institución / Entidad de Origen</label>
                    <asp:TextBox ID="txtEntidadInt" runat="server" CssClass="form-control" placeholder="Ej: Universidad Central..." autocomplete="off" />
                </div>

                <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Datos del Grupo</h6></div>
                <div class="col-md-6">
                    <label class="form-label">Función</label>
                    <asp:DropDownList ID="ddlFuncionInt" runat="server" CssClass="form-select" onchange="ToggleFuncionIntegrante(this)">
                        <asp:ListItem Value="Investigador Principal">Investigador Principal</asp:ListItem>
                        <asp:ListItem Value="Miembro Investigador" Selected="True">Miembro Investigador</asp:ListItem>
                        <asp:ListItem Value="Coordinador">Coordinador</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <%-- Archivo Certificado --%>
                <div class="col-12 animate__animated animate__fadeIn" id="divCertificado" style="display:none;">
                    <label class="form-label fw-semibold text-primary"><i class="fa-solid fa-certificate me-1"></i> Certificado de Categorización</label>
                    <div class="utc-fileinput-wrapper" id="wrapperCertificadoInt">
                        <div class="utc-fileinput-header">
                            <div class="utc-fileinput-icon"><i class="fa-solid fa-file-contract"></i></div>
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <span class="utc-fileinput-name">Sin archivo seleccionado</span>
                                <div class="utc-fileinput-buttons d-flex gap-2">
                                    <button type="button" class="btn btn-outline-primary utc-btn-small rename-btn"><i class="fa-solid fa-pen-to-square"></i></button>
                                    <button type="button" class="btn btn-outline-danger utc-btn-small remove-btn"><i class="fa-solid fa-xmark"></i></button>
                                </div>
                            </div>
                        </div>
                        <input type="text" class="form-control form-control-sm utc-edit-name-field" placeholder="Nuevo nombre..." />
                        <div class="utc-fileinput-preview" id="previewCertificadoInt"></div>
                        <div class="utc-fileinput-loader" id="loaderCertificadoInt"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneCertificadoInt"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Subir Certificado (PDF)</div>
                        <asp:FileUpload ID="flpCertificadoInt" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                    <asp:HiddenField ID="hfCertificadoIntActual" runat="server" ClientIDMode="Static" />
                </div>
                
                <div class="col-md-6"><label class="form-label">Fecha Inicio</label><asp:TextBox ID="dtFechaIniInt" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-12"><label class="form-label">Observaciones</label><asp:TextBox ID="txtObservacionInt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" /></div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="btnGuardarInt" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="btnGuardarInt_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnCancelarInt_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- =====================================================================
         MODAL 1: DETALLE DE PROYECTOS
         ===================================================================== --%>
    <div class="modal fade" id="modalProyectosDetalle" tabindex="-1" aria-hidden="true" ClientIDMode="Static">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content shadow-utc border-0 rounded-4">
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100 text-center"><i class="fa-solid fa-list-check me-2"></i> Proyectos Asociados</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body bg-light">
                    <h6 class="text-primary fw-bold text-center mb-3 text-uppercase border-bottom pb-2" id="lblGrupoTitulo" runat="server">Grupo Seleccionado</h6>
                    <div class="table-responsive bg-white rounded shadow-sm p-3 border">
                        <asp:GridView ID="gvProyectosDetalle" runat="server" AutoGenerateColumns="false" 
                            CssClass="table table-hover table-modal table-borderless align-middle mb-0 text-center"
                            GridLines="Horizontal"
                            EmptyDataText="<div class='text-center p-4 text-muted'><i class='fa-solid fa-folder-open fa-3x mb-3 text-secondary opacity-50'></i><br>Este grupo aún no tiene proyectos registrados.</div>">
                            <Columns>
                                <asp:BoundField DataField="strId_pro" HeaderText="ID" ItemStyle-CssClass="fw-bold small text-muted" />
                                <asp:BoundField DataField="strTema_pro" HeaderText="Tema del Proyecto" ItemStyle-CssClass="text-start fw-semibold text-dark" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class='<%# 
                                            Eval("strEstado_pro").ToString() == "Aprobado" ? "badge bg-success rounded-pill" : 
                                            Eval("strEstado_pro").ToString() == "Rechazado" ? "badge bg-danger rounded-pill" : 
                                            "badge bg-warning text-dark rounded-pill" %>'>
                                            <%# Eval("strEstado_pro") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer border-0 justify-content-center pb-3">
                    <button type="button" class="btn btn-outline-secondary btn-pill px-4" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <%-- =====================================================================
         MODAL 2: HISTORIAL DE MOVIMIENTOS 
        ===================================================================== --%>
    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content rounded-4 shadow-utc border-0">
                
                <%-- Header Azul Gradiente --%>
                <div class="modal-header bg-utc text-white">
                    <h5 class="modal-title w-100 text-center">
                        <i class="fa-solid fa-clock-rotate-left me-2"></i> HISTORIAL DE MOVIMIENTOS
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body bg-white">
                    <%-- Info del Integrante --%>
                    <div class="d-flex justify-content-between align-items-center mb-3 border-bottom pb-3">
                        <h6 class="fw-bold text-secondary mb-0">
                            INTEGRANTE: <asp:Label ID="lblNombreHistorial" runat="server" CssClass="text-primary text-uppercase" Text="..." />
                        </h6>
                        
                        <%-- Botón Generar Reporte --%>
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" 
                            CssClass="btn btn-danger btn-pill btn-sm px-4 shadow-sm" 
                            OnClick="btnGenerarReporte_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte PDF
                        </asp:LinkButton>
                    </div>

                    <asp:HiddenField ID="hfIdIntegranteHistorial" runat="server" />

                    <%-- Tabla con Estilos UTC --%>
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
                                            <%-- Fecha --%>
                                            <td class="text-secondary fw-bold" style="font-size: 0.85rem;">
                                                <%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd/MM/yyyy HH:mm") %>
                                            </td>
                                            
                                            <%-- Acción (Badge) --%>
                                            <td>
                                                <span class='badge rounded-pill px-3 <%# Eval("strAccion").ToString() == "BAJA" ? "badge-baja" : (Eval("strAccion").ToString().Contains("NUEVO") ? "badge-alta" : "badge-historial") %>'>
                                                    <%# Eval("strAccion") %>
                                                </span>
                                            </td>
                                            
                                            <%-- Motivo --%>
                                            <td class="text-start fst-italic text-muted small ps-3">
                                                <%# Eval("strMotivo") %>
                                            </td>
                                            
                                            <%-- Usuario --%>
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

    <%-- =====================================================================
         MODAL 3: VISTA PREVIA REPORTE
         ===================================================================== --%>
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
                    <div id="arealmpresion" class="report-paper">
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

    <%-- =====================================================================
         MODAL 4: CAMBIO DE ESTADO (CONFIRMACIÓN)
         ===================================================================== --%>
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

    <%-- =====================================================================
         JAVASCRIPT
         ===================================================================== --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        // Configuración DataTables
        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            order: [],
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        Sys.Application.add_load(function () {
            // Inicializar tablas
            initTable('#tablaGrupos');
            initTable('#tablaIntegrantes');

            // Inicializar File Inputs
            if (typeof UTC_FileInput === 'function') {
                initFileInput('wrapperArchivoGrupo', '<%= flpArchivoGrupo.ClientID %>');
                initFileInput('wrapperCertificadoInt', '<%= flpCertificadoInt.ClientID %>');
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

        function ToggleTipoIntegrante(el) {
            var tipo = el.value;
            var divInterno = document.getElementById('divInterno');
            var divExterno = document.getElementById('divExterno');
            if (tipo === "Externo") {
                divInterno.style.display = 'none'; divExterno.style.display = 'block';
            } else {
                divInterno.style.display = 'flex'; divExterno.style.display = 'none';
            }
        }

        function ToggleFuncionIntegrante(el) {
            var val = el.value;
            var div = document.getElementById('divCertificado');
            div.style.display = (val === 'Investigador Principal') ? 'block' : 'none';
        }

        function InitFormulario() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            if (ddl) ToggleTipoIntegrante(ddl);
            var ddlFunc = document.getElementById('<%= ddlFuncionInt.ClientID %>');
            if (ddlFunc) ToggleFuncionIntegrante(ddlFunc);
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

        function AbrirModalEstado() {
            var el = document.getElementById('modalEstadoInt');
            if (el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }

        function imprimirReporte() {
            var contenido = document.getElementById("areaImpresion").innerHTML;
            var ventana = window.open('', 'PRINT', 'height=800,width=1000');
            ventana.document.write('<html><head><title>Reporte de Historial</title>');
            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');
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