<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GruposInvestigacion.aspx.cs" Inherits="SistemaGestionCGI.GruposInvestigacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />

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
            width: 40px; height: 40px; object-fit: cover; border-radius: 50%; border: 2px solid #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
    </style>

    <div id="headerGrupos" runat="server" 
         class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
        
        <h3 class="utc-title mb-0">
            <i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN
        </h3>

        <div class="d-flex gap-2 mt-2 mt-md-0">
            <asp:LinkButton runat="server" ID="lbtNuevoGruInv"
                CssClass="btn btn-primary btn-pill d-flex align-items-center"
                OnClick="lbtNuevoGruInv_Click">
                <i class="fa-solid fa-plus me-2"></i> NUEVO GRUPO
            </asp:LinkButton>

            <asp:LinkButton runat="server" ID="btnRegresarGruInv"
                CssClass="btn btn-outline-primary btn-pill px-4"
                OnClick="btnRegresarGruInv_Click"
                Visible="false" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server" Visible="true">
        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaGrupos" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>FOTO</th>
                        <th>NOMBRE</th>
                        <th>COORDINADOR</th>
                        <th>CATEGORÍA</th>
                        <th>CREACIÓN</th>
                        <th>LÍNEA</th>
                        <th>ACCIONES</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptGrupoInv" runat="server" OnItemCommand="rptGrupoInv_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("strId_gru") %></td>
                                <td>
                                    <img src='<%# string.IsNullOrEmpty(Eval("strFoto_gru")?.ToString()) ? "img/default-user.png" : ResolveUrl(Eval("strFoto_gru").ToString()) %>'
                                         class="img-avatar-table" alt="Foto" />
                                </td>
                                <td class="text-start fw-semibold text-primary"><%# Eval("strNombre_gru") %></td>
                                <td class="text-start"><%# Eval("strCoordinador_gru") %></td>
                                <td><span class="badge bg-light text-dark border"><%# Eval("strCategoria_gru") %></span></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechacrea_gru")).ToString("dd/MM/yyyy") %></td>
                                <td class="text-start small"><%# Eval("strLineasinv_gru") %></td>
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

    <asp:Panel ID="pnlAgregarGruInv" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN</h3>
            <asp:LinkButton ID="lbtCancelarGruInvTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbtCancelarGruInv_Click" CausesValidation="false"><i class="fa-solid fa-chevron-left me-2"></i> REGRESAR</asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-file-circle-plus me-2"></i> Registrar Nuevo Grupo</h4>

            <div class="row g-3">
                <div class="col-12"><label class="form-label">Nombre del Grupo</label><asp:TextBox ID="strNombreGru" runat="server" CssClass="form-control" /></div>
                <div class="col-12"><label class="form-label">Coordinador</label><asp:TextBox ID="strNombreCoorGru" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Fecha de Creación</label><asp:TextBox ID="dtFechaCreaGru" runat="server" CssClass="form-control" TextMode="Date" /></div>
                
                <div class="col-md-6">
                    <label class="form-label">Categoría</label>
                    <asp:DropDownList ID="ddlCatGruInv" runat="server" CssClass="form-select">
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem><asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
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
                
                <asp:LinkButton ID="lbtCancelarGruInv" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                    OnClick="lbtCancelarGruInv_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEditarGrupoInv" runat="server" Visible="false">
        
        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0"><i class="fa-solid fa-people-group me-2"></i> GRUPOS DE INVESTIGACIÓN</h3>
            <asp:LinkButton ID="lbnCancellEditGruInvTop" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="lbnCancellEditGruInv_Click" CausesValidation="false"><i class="fa-solid fa-chevron-left me-2"></i> REGRESAR</asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            <h4 class="utc-subtitle mb-4 text-center"><i class="fa-solid fa-pen-to-square me-2"></i> Editar Grupo</h4>
            
            <asp:HiddenField ID="hfIdGrupoEdit" runat="server" />
            <asp:HiddenField ID="hfFotoActual" runat="server" />
            <asp:HiddenField ID="hfArchivoActual" runat="server" />

            <div class="row g-3">
                <div class="col-12"><label class="form-label">Nombre del Grupo</label><asp:TextBox ID="txtGrupoInvEdit" runat="server" CssClass="form-control" /></div>
                <div class="col-12"><label class="form-label">Coordinador</label><asp:TextBox ID="txtNombreCoorGruInvEdit" runat="server" CssClass="form-control" /></div>
                <div class="col-md-6"><label class="form-label">Fecha de Creación</label><asp:TextBox ID="dtEditFechaCreaEdit" runat="server" CssClass="form-control" TextMode="Date" /></div>
                <div class="col-md-6">
                    <label class="form-label">Categoría</label>
                    <asp:DropDownList ID="ddlEditCategoria" runat="server" CssClass="form-select">
                        <asp:ListItem Value="EMERGENTE">EMERGENTE</asp:ListItem><asp:ListItem Value="CONSOLIDADO">CONSOLIDADO</asp:ListItem>
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
                
                <asp:LinkButton ID="lbnCancellEditGruInv" runat="server" CssClass="btn btn-outline-primary btn-pill px-4" 
                    OnClick="lbnCancellEditGruInv_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlIntegrantes" runat="server" Visible="false">
        
        <asp:HiddenField ID="hfGrupoIdActual" runat="server" />

        <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-3 rounded shadow-utc border header-utc-line">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            
            <div class="d-flex gap-2">
                <asp:LinkButton runat="server" ID="btnNuevoIntegrante" CssClass="btn btn-primary btn-pill" 
                    OnClick="btnNuevoIntegrante_Click">
                    <i class="fa-solid fa-user-plus me-2"></i> NUEVO INTEGRANTE
                </asp:LinkButton>

                <asp:LinkButton runat="server" ID="btnVolverGrupos" CssClass="btn btn-outline-primary btn-pill px-4" 
                    OnClick="btnVolverGrupos_Click">
                    <i class="fa-solid fa-arrow-left me-2"></i> VOLVER A GRUPOS
                </asp:LinkButton>
            </div>
        </div>

        <div class="table-responsive bg-white p-3 rounded shadow-utc">
            <table id="tablaIntegrantes" class="table table-bordered table-hover table-utc align-middle text-center">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>NOMBRES</th>
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
                            <tr>
                                <td><%# Eval("strId_int") %></td>
                                <td class="text-start"><%# Eval("strApellidos_int") + " " + Eval("strNombres_int") %></td>
                                <td><%# Eval("strFuncion_int") %></td>
                                <td><%# Convert.ToDateTime(Eval("dtFechaini_int")).ToString("dd/MM/yyyy") %></td>
                                <td><%# Eval("dtFechafin_int") == DBNull.Value ? "-" : Convert.ToDateTime(Eval("dtFechafin_int")).ToString("dd/MM/yyyy") %></td>
                                <td><%# Convert.ToBoolean(Eval("bitActivo_int")) ? "<span class='badge bg-success'>Activo</span>" : "<span class='badge bg-danger'>Inactivo</span>" %></td>
                                <td>
                                    <asp:LinkButton ID="btnEditarInt" runat="server" CommandName="EditarInt" CommandArgument='<%# Eval("strId_int") %>'
                                        CssClass="btn btn-warning btn-sm rounded-circle me-1" ToolTip="Editar">
                                        <i class="fa-solid fa-pen"></i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnToggleEstado" runat="server" CommandName="CambiarEstado" CommandArgument='<%# Eval("strId_int") %>'
                                        CssClass="btn btn-info btn-sm rounded-circle me-1 text-white" ToolTip="Cambiar estado">
                                        <i class="fa-solid fa-power-off"></i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnHistorial" runat="server" CommandName="Historial" CommandArgument='<%# Eval("strId_int") %>'
                                        CssClass="btn btn-primary btn-sm rounded-circle me-1" ToolTip="Historial">
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
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-users me-2"></i> GESTIÓN DE INTEGRANTES
            </h3>
            
            <asp:LinkButton ID="btnCancelarIntTop" runat="server" 
                CssClass="btn btn-outline-primary btn-pill px-4" 
                OnClick="btnCancelarInt_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>

        <div class="form-stack w-100 mx-auto shadow-utc border-0 rounded-4 p-4" style="max-width: 100%;">
            
            <h4 class="utc-subtitle mb-4 text-center">
                <i class="fa-solid fa-user-plus me-2"></i> 
                <asp:Label runat="server" ID="lblTituloFormInt" Text="Nuevo Integrante" />
            </h4>
            
            <asp:HiddenField ID="hfIdIntEdit" runat="server" />

            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Cédula</label>
                    <asp:TextBox ID="txtCedulaInt" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Función</label>
                    <asp:DropDownList ID="ddlFuncionInt" runat="server" CssClass="form-select">
                        <asp:ListItem Value="Investigador Principal">Investigador Principal</asp:ListItem>
                        <asp:ListItem Value="Miembro Investigador">Miembro Investigador</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nombres</label>
                    <asp:TextBox ID="txtNombresInt" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Apellidos</label>
                    <asp:TextBox ID="txtApellidosInt" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Correo</label>
                    <asp:TextBox ID="txtCorreoInt" runat="server" CssClass="form-control" TextMode="Email" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Carrera</label>
                    <asp:TextBox ID="txtCarreraInt" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Fecha Inicio</label>
                    <asp:TextBox ID="dtFechaIniInt" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-12">
                    <label class="form-label">Observaciones</label>
                    <asp:TextBox ID="txtObservacionInt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>
            </div>

            <div class="d-flex justify-content-center gap-3 mt-4">
                <asp:LinkButton ID="btnGuardarInt" runat="server" CssClass="btn btn-primary btn-pill px-4"
                    OnClick="btnGuardarInt_Click">
                    <i class="fa-solid fa-floppy-disk me-2"></i> Guardar
                </asp:LinkButton>

                <asp:LinkButton ID="btnCancelarInt" runat="server" CssClass="btn btn-outline-primary btn-pill px-4"
                    OnClick="btnCancelarInt_Click" CausesValidation="false">
                    <i class="fa-solid fa-ban me-2"></i> Cancelar
                </asp:LinkButton>
            </div>
        </div>
    </asp:Panel>

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
                        <asp:LinkButton ID="btnGenerarReporte" runat="server" CssClass="btn btn-danger btn-pill px-4" 
                            OnClick="btnGenerarReporte_Click">
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

    <div class="modal fade" id="modalVistaPrevia" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
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
                <div class="modal-body p-4" style="background: white; min-height: 500px;">
                    <div id="areaImpresion">
                        <asp:Literal ID="litReporteGenerado" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalEstadoInt" tabindex="-1" aria-hidden="true" ClientIDMode="Static" runat="server">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-utc border-0">
                <div class="modal-header bg-utc text-white text-center">
                    <h5 class="modal-title w-100">
                        <i class="fa-solid fa-power-off me-2"></i> Cambio de Estado
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                
                <div class="modal-body">
                    <p class="mb-3 text-center fs-5">
                        ¿Estás seguro de <strong id="accionEstadoTexto" class="text-primary">cambiar el estado</strong>?
                    </p>

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
                    <asp:LinkButton ID="btnConfirmarCambioEstado" runat="server" 
                        CssClass="btn btn-pill btn-danger px-4"
                        OnClientClick="return guardarMotivo();" 
                        OnClick="btnConfirmarCambioEstado_Click">
                        Confirmar Cambio
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <script src="DesignersUTC/Scripts/utc-fileinput.js"></script>
    <script type="text/javascript">
        
        Sys.Application.add_load(function () {
            initTable('#tablaGrupos');
            initTable('#tablaIntegrantes');
            
            if(document.getElementById('wrapperArchivoAdd')) initFileInput('wrapperArchivoAdd', '<%= flpArchivoAdd.ClientID %>');
            if (document.getElementById('wrapperArchivoEdit')) initFileInput('wrapperArchivoEdit', '<%= flpArchivoEdit.ClientID %>');

            var wrapperAdd = document.getElementById('wrapperArchivoAdd');
            if (wrapperAdd) {
                UTC_FileInput({
                    wrapper: 'wrapperArchivoAdd',
                    dropzone: 'dropzoneArchivoAdd',
                    preview: 'previewArchivoAdd',
                    loader: 'loaderArchivoAdd',
                    input: '<%= flpArchivoAdd.ClientID %>'
                });
            }

            var wrapperEdit = document.getElementById('wrapperArchivoEdit');
            if (wrapperEdit) {
                UTC_FileInput({
                    wrapper: 'wrapperArchivoEdit',
                    dropzone: 'dropzoneArchivoEdit',
                    preview: 'previewArchivoEdit',
                    loader: 'loaderArchivoEdit',
                    input: '<%= flpArchivoEdit.ClientID %>'
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

        function initFileInput(wrapperId, inputId) {
            if (typeof UTC_FileInput === 'function') {
                UTC_FileInput({
                    wrapper: wrapperId, dropzone: wrapperId.replace('wrapper','dropzone'),
                    preview: wrapperId.replace('wrapper','preview'), loader: wrapperId.replace('wrapper','loader'),
                    input: inputId
                });
            }
        }

        function previewImage(input, imgId) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.getElementById(imgId);
                    if(img) { img.src = e.target.result; img.style.display = 'block'; }
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        function guardarMotivo() {
            var txt = document.getElementById('txtMotivoEstado');
            var hf = document.getElementById('<%= hfMotivoEstado.ClientID %>');
            if (txt && hf) {
                if (!txt.value.trim()) { toastify('warning', 'Ingrese un motivo', 'Atención'); return false; }
                hf.value = txt.value.trim();
                return true;
            }
            return false;
        }

        function AbrirModalEstado() {
            var el = document.getElementById('modalEstadoInt');
            if(el) { var modal = bootstrap.Modal.getOrCreateInstance(el); modal.show(); }
        }

        function descargarHistorialPDF() {
            var id = document.getElementById("<%= hfIdIntegranteHistorial.ClientID %>").value;
            if (id) window.open("HistorialPDF.aspx?id=" + id, "_blank");
        }

        function imprimirReporte() {
            var contenido = document.getElementById("areaImpresion").innerHTML;

            var ventana = window.open('', 'PRINT', 'height=800,width=1000');

            ventana.document.write('<html><head><title>Reporte de Historial - UTC</title>');

            ventana.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">');

            ventana.document.write('<link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />');
            ventana.document.write('<link href="DesignersUTC/Styles/modal-historial-reporte.css" rel="stylesheet" />');

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