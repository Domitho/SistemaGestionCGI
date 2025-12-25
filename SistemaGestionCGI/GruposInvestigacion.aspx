<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GruposInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.GruposInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- RECURSOS UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

    <%-- ESTILOS LOCALES OPTIMIZADOS --%>
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
        .img-avatar-table {
            width: 40px; height: 40px; object-fit: cover; border-radius: 50%; 
            border: 2px solid #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        /* Ajuste de formulario responsivo */
        .form-stack { max-width: 100% !important; }
    </style>

    <%-- HEADER PRINCIPAL --%>
    <div id="headerGrupos" runat="server" class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN
        </h3>
        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="lbtNuevoGruInv" CssClass="btn btn-primary btn-pill d-flex align-items-center" OnClick="lbtNuevoGruInv_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO GRUPO
            </asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnRegresarGruInv" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresarGruInv_Click" Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- PANEL 1: TABLA DE GRUPOS --%>
    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaGrupos" class="table table-bordered table-hover table-utc align-middle text-center" style="width:100%">
                <thead>
                    <tr>
                        <th>ID</th> <th>FOTO</th> <th>NOMBRE</th> <th>COORDINADOR</th> 
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
                                    <asp:LinkButton ID="btnVerArchivo" runat="server" CommandName="Archivo" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-ver btn-sm rounded-circle me-1" ToolTip="Ver archivo"><i class="fa-solid fa-paperclip"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnVerInt" runat="server" CommandName="VerIntegrantes" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" ToolTip="Integrantes"><i class="fa-solid fa-users"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar"><i class="fa-solid fa-pen"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("strId_gru") %>' CssClass="btn btn-eliminar btn-sm rounded-circle" OnClientClick="return confirm('¿Eliminar grupo?');" ToolTip="Eliminar"><i class="fa-solid fa-trash"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- PANEL 2: AGREGAR GRUPO --%>
    <asp:Panel ID="pnlAgregarGruInv" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN</h3>
            <asp:LinkButton ID="lbtCancelarGruInvTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbtCancelarGruInv_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Nuevo Grupo</h4>
            <div class="row g-3">
                <div class="col-12"><label class="form-label">Nombre del Grupo</label><asp:TextBox ID="strNombreGru" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-12"><label class="form-label">Coordinador</label><asp:TextBox ID="strNombreCoorGru" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-md-6"><label class="form-label">Fecha de Creación</label><asp:TextBox ID="dtFechaCreaGru" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-md-6">
                    <label class="form-label">Categoría</label>
                    <asp:DropDownList ID="ddlCatGruInv" runat="server" CssClass="form-select">
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem><asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Facultad / Extensión</label>
                    <asp:DropDownList ID="ddlFacultadGru" runat="server" CssClass="form-select">
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
                <div class="col-12">
                    <label class="form-label">Línea de Investigación</label>
                    <asp:DropDownList ID="strLineaInvGru1" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Tecnologias de la informacion y comunicacion (TICS)">Tecnologias de la informacion y comunicacion (TICS)</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Sublínea de Investigación</label>
                    <asp:DropDownList ID="strSLineaInvGru1" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Inteligencia artificial e inteligencia de negocios">Inteligencia artificial e inteligencia de negocios</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12 text-center">
                    <label class="form-label fw-bold d-block">Foto del Grupo</label>
                    <asp:FileUpload ID="fuFotoGrupoAdd" runat="server" CssClass="form-control mb-2" onchange="previewImage(this, 'previewFotoAdd')" />
                    <img id="previewFotoAdd" src="#" class="img-thumbnail rounded-circle" style="width:100px; height:100px; object-fit:cover; display:none;" />
                </div>
                <div class="col-12">
                    <label class="form-label fw-semibold">Archivo de Resolución</label>
                    <%-- UTC FILE INPUT ADD --%>
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
                        <div class="utc-dropzone" id="dropzoneArchivoAdd"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra archivo aquí</div>
                        <asp:FileUpload ID="flpArchivoAdd" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>
            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="lbtADDGruInv" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="lbtADDGruInv_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>
                <asp:LinkButton ID="lbtCancelarGruInv" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbtCancelarGruInv_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL 3: EDITAR GRUPO --%>
    <asp:Panel ID="pnlEditarGrupoInv" runat="server" Visible="false">
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN</h3>
            <asp:LinkButton ID="lbnCancellEditGruInvTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbnCancellEditGruInv_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-pen-to-square me-2"></i> Editar Grupo</h4>
            <asp:HiddenField ID="hfIdGrupoEdit" runat="server" />
            <asp:HiddenField ID="hfFotoActual" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />
            <div class="row g-3">
                <div class="col-12"><label class="form-label">Nombre del Grupo</label><asp:TextBox ID="txtGrupoInvEdit" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-12"><label class="form-label">Coordinador</label><asp:TextBox ID="txtNombreCoorGruInvEdit" runat="server" CssClass="form-control" autocomplete="off" /></div>
                <div class="col-md-6"><label class="form-label">Fecha de Creación</label><asp:TextBox ID="dtEditFechaCreaEdit" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-md-6">
                    <label class="form-label">Categoría</label>
                    <asp:DropDownList ID="ddlEditCategoria" runat="server" CssClass="form-select">
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem><asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label">Facultad / Extensión</label>
                    <asp:DropDownList ID="ddlFacultadGruEdit" runat="server" CssClass="form-select">
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
                <div class="col-12"><label class="form-label">Línea</label><asp:DropDownList ID="txtEditLineaIGru1" runat="server" CssClass="form-select"><asp:ListItem Value="Tecnologias de la informacion y comunicacion (TICS)">Tecnologias de la informacion y comunicacion (TICS)</asp:ListItem></asp:DropDownList></div>
                <div class="col-12"><label class="form-label">Sublínea</label><asp:DropDownList ID="txtEditSLineaIGru1" runat="server" CssClass="form-select"><asp:ListItem Value="Inteligencia artificial e inteligencia de negocios">Inteligencia artificial e inteligencia de negocios</asp:ListItem></asp:DropDownList></div>
                <div class="col-12 text-center">
                    <label class="form-label d-block">Foto Actual</label>
                    <asp:Image ID="imgFotoGruEdit" runat="server" CssClass="img-thumbnail rounded-circle mb-2" Width="80" Height="80" />
                    <asp:FileUpload ID="fuFotoGrupoEdit" runat="server" CssClass="form-control form-control-sm" />
                </div>
                <div class="col-12">
                    <label class="form-label">Reemplazar Archivo</label>
                    <%-- UTC FILE INPUT EDIT --%>
                    <div class="utc-fileinput-wrapper" id="wrapperArchivoEdit">
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
                        <div class="utc-fileinput-preview" id="previewArchivoEdit"></div>
                        <div class="utc-fileinput-loader" id="loaderArchivoEdit"><i class="fa-solid fa-spinner fa-spin me-2"></i> Cargando...</div>
                        <div class="utc-dropzone" id="dropzoneArchivoEdit"><i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-primary"></i><br />Arrastra archivo aquí</div>
                        <asp:FileUpload ID="flpArchivoEdit" runat="server" CssClass="utc-fileinput-input" />
                    </div>
                </div>
            </div>
            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="lbnEditGruInv" runat="server" CssClass="btn btn-primary btn-pill px-4" OnClick="lbnEditGruInv_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Actualizar
                </asp:LinkButton>
                <asp:LinkButton ID="lbnCancellEditGruInv" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbnCancellEditGruInv_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <%-- PANEL 4: GESTIÓN DE INTEGRANTES --%>
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
                    <tr><th>ID</th><th>NOMBRES</th><th>FUNCIÓN</th><th>INICIO</th><th>FIN</th><th>ESTADO</th><th>ACCIONES</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptIntegrantes" runat="server" OnItemCommand="rptIntegrantes_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_int") %></td>
                                <td class="text-start"><%# Eval("strApellidos_int") + " " + Eval("strNombres_int") %></td>
                                <td><%# Eval("strFuncion_int") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_int")).ToString("dd/MM/yyyy") %></td>
                                <td><%# Eval("dtFechafin_int") == DBNull.Value ? "-" : Convert.ToDateTime(Eval("dtFechafin_int")).ToString("dd/MM/yyyy") %></td>
                                <td><%# Convert.ToBoolean(Eval("bitActivo_int")) ? "<span class='badge bg-success'>Activo</span>" : "<span class='badge bg-danger'>Inactivo</span>" %></td>
                                <td>
                                    <asp:LinkButton ID="btnVerCertificado" runat="server" 
                                        CommandName="VerCertificado" 
                                        CommandArgument='<%# Eval("strCertificado_int") %>'
                    
                                        Visible='<%# Eval("strFuncion_int").ToString() == "Investigador Principal" && !string.IsNullOrEmpty(Eval("strCertificado_int") as string) %>'
                    
                                        CssClass="btn btn-success btn-sm rounded-circle me-1" 
                                        ToolTip="Ver Certificado de Categorización">
                                        <i class="fa-solid fa-eye"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEditarInt" runat="server" CommandName="EditarInt" CommandArgument='<%# Eval("strId_int") %>' CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar"><i class="fa-solid fa-pen"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnToggleEstado" runat="server" CommandName="CambiarEstado" CommandArgument='<%# Eval("strId_int") %>' CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" ToolTip="Cambiar estado"><i class="fa-solid fa-power-off"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="Historial" CommandArgument='<%# Eval("strId_int") %>' CssClass="btn btn-primary btn-sm rounded-circle me-1" ToolTip="Historial"><i class="fa-solid fa-clock-rotate-left"></i></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <%-- PANEL 5: FORMULARIO INTEGRANTE --%>
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
                <div class="col-md-4"><label class="form-label">Cédula</label><asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" MaxLength="15" autocomplete="off" /></div>
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
                <div class="col-12 mt-3"><h6 class="text-primary fw-bold border-bottom pb-2">Afiliación</h6></div>
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
                <div class="col-12 animate__animated animate__fadeIn" id="divCertificado" style="display:none;">
                    <label class="form-label fw-semibold text-primary">
                        <i class="fa-solid fa-certificate me-1"></i> Certificado de Categorización
                    </label>
    
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
                    <div class="form-text small">Requerido únicamente para Investigadores Principales.</div>
                    <%-- HiddenField para mantener la ruta al editar --%>
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

    <%-- MODAL: HISTORIAL --%>
    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content rounded-4 shadow-utc">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100">HISTORIAL DE MOVIMIENTOS</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <h6 class="fw-bold mb-3 text-primary text-center">INTEGRANTE: <asp:Label ID="lblNombreHistorial" runat="server" Text="..." /></h6>
                    <asp:HiddenField ID="hfIdIntegranteHistorial" runat="server" />
                    <div class="d-flex justify-content-end mb-3">
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" CssClass="btn btn-danger btn-pill px-4" OnClick="btnGenerarReporte_Click">
                            <i class="fa-solid fa-file-pdf me-2"></i> Generar Reporte Completo
                        </asp:LinkButton>
                    </div>
                    <div class="table-responsive bg-white p-3 rounded shadow-sm border">
                        <table class="table table-bordered align-middle text-center">
                            <thead class="table-light"><tr><th>Fecha</th><th>Acción</th><th>Motivo</th><th>Usuario</th></tr></thead>
                            <tbody>
                                <asp:Repeater ID="rptHistorial" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Convert.ToDateTime(Eval("dtFecha")).ToString("dd/MM/yyyy HH:mm") %></td>
                                            <td><span class='badge <%# Eval("strAccion").ToString() == "BAJA" ? "bg-danger" : "bg-success" %>'><%# Eval("strAccion") %></span></td>
                                            <td class="text-start"><%# Eval("strMotivo") %></td>
                                            <td><%# Eval("strUsuario") %></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Panel ID="pnlNoHistorial" runat="server" Visible='<%# rptHistorial.Items.Count == 0 %>'>
                                            <tr><td colspan="4" class="text-muted py-3">Sin movimientos registrados.</td></tr>
                                        </asp:Panel>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- MODAL: VISTA PREVIA --%>
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
                    <div id="areaImpresion"><asp:Literal ID="litReporteGenerado" runat="server"></asp:Literal></div>
                </div>
            </div>
        </div>
    </div>

    <%-- MODAL: ESTADO --%>
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
                        <p class="mb-0"><strong>Estado actual:</strong> <span id="infoEstado"></span></p>
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

    <%-- SCRIPTS OPTIMIZADOS --%>
    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">

        // Configuración DataTables centralizada
        const dtConfig = {
            responsive: true,
            autoWidth: false,
            pageLength: 10,
            language: { url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json" },
            order: [],
            dom: "<'row align-items-center mb-2'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6 text-end'f>><'row'<'col-sm-12'tr>><'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>"
        };

        // Función de inicio compatible con UpdatePanel
        Sys.Application.add_load(function () {
            
            // Inicializar tablas
            initTable('#tablaGrupos');
            initTable('#tablaIntegrantes');

            // Inicializar File Inputs
            if (typeof UTC_FileInput === 'function') {
                initFileInput('wrapperArchivoAdd', '<%= flpArchivoAdd.ClientID %>');
                initFileInput('wrapperArchivoEdit', '<%= flpArchivoEdit.ClientID %>');
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
                divInterno.style.display = 'none';
                divExterno.style.display = 'block';
            } else {
                divInterno.style.display = 'flex';
                divExterno.style.display = 'none';
            }
        }

        function ToggleFuncionIntegrante(el) {
            var val = el.value;
            var div = document.getElementById('divCertificado');

            if (val === 'Investigador Principal') {
                div.style.display = 'block';
            } else {
                div.style.display = 'none';
                // Opcional: Limpiar el input si lo ocultan, 
                // pero mejor dejarlo por si se equivocaron y vuelven a seleccionar.
            }
        }

        // Llamar para establecer estado inicial
        function InitFormulario() {
            var ddl = document.getElementById('<%= ddlTipoInt.ClientID %>');
            if (ddl) ToggleTipoIntegrante(ddl);

            var ddlFunc = document.getElementById('<%= ddlFuncionInt.ClientID %>');
            if (ddlFunc) ToggleFuncionIntegrante(ddlFunc);
        }

        // --- Funciones de Modals y Estado ---

        function guardarMotivo() {
            var txt = document.getElementById('txtMotivoEstado');
            var hf = document.getElementById('<%= hfMotivoEstado.ClientID %>');
            if (txt && hf) {
                if (!txt.value.trim()) {
                    alert('Ingrese un motivo');
                    return false;
                }
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