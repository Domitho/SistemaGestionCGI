<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SistemaGestionCGI.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <%-- RECURSOS DE ESTILO UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />
    <link href="DesignersUTC/Styles/gestion-usuarios.css" rel="stylesheet" />

    <div class="d-flex justify-content-between align-items-center flex-wrap bg-white p-3 mb-4 rounded shadow-utc border header-utc-line">
        
        <div class="d-flex align-items-center">
            <div class="bg-light p-3 rounded-circle me-3 text-primary">
                <i class="fa-solid fa-users-gear fa-2x"></i>
            </div>
            <div>
                <h3 class="utc-title mb-0">DIRECTORIO DE USUARIOS</h3>
                <p class="text-muted small mb-0 mt-1" style="font-size: 0.85rem;">
                    Gestión de accesos, roles y perfiles administrativos del sistema.
                </p>
            </div>
        </div>
        
        <div class="mt-3 mt-md-0">
            <asp:LinkButton ID="btnNuevoUsuario" runat="server" CssClass="btn btn-primary btn-pill shadow-sm px-4 py-2 d-flex align-items-center" OnClick="btnNuevoUsuario_Click">
                <i class="fa-solid fa-user-plus me-2"></i> CREAR NUEVO PERFIL
            </asp:LinkButton>
        </div>

    </div>

    <asp:Panel ID="pnlGrilla" runat="server">
        
        <div class="bg-white p-3 rounded-4 shadow-utc mb-4 border d-flex align-items-center">
            <i class="fa-solid fa-magnifying-glass text-primary fs-5 ms-2 me-3"></i>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control border-0 shadow-none bg-transparent fs-6" 
                placeholder="Buscar por nombre de usuario, rol o ID..." AutoPostBack="true" OnTextChanged="txtBuscar_TextChanged"></asp:TextBox>
        </div>

        <div class="user-grid-container">
            <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                <ItemTemplate>
                    
                    <div class="user-card-pro">
                        <div class='status-badge <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "st-active" : "st-inactive" %>'>
                             <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "<i class='fa-solid fa-circle me-1' style='font-size:6px; vertical-align:middle;'></i>ACTIVO" : "<i class='fa-solid fa-circle me-1' style='font-size:6px; vertical-align:middle;'></i>INACTIVO" %>
                        </div>

                        <div class="pro-avatar">
                            <%# ObtenerIniciales(Eval("strNombre_usu").ToString()) %>
                        </div>

                        <h5 class="fw-bold text-dark mb-1 text-truncate w-100" title='<%# Eval("strNombre_usu") %>'>
                            <%# Eval("strNombre_usu") %>
                        </h5>
                        
                        <span class='role-pill <%# Eval("strRol_usu").ToString() == "COORDINADOR" ? "role-coord" : "" %>'>
                            <%# Eval("strRol_usu") %>
                        </span>

                        <div class="text-muted small mb-3">
                            <i class="fa-solid fa-fingerprint me-1 text-primary"></i> ID: <strong><%# Eval("intId_usu") %></strong>
                        </div>

                        <div class="card-actions">
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("intId_usu") %>'
                                CssClass="btn-card-action btn-card-edit shadow-sm" ToolTip="Editar Perfil">
                                <i class="fa-solid fa-pen"></i>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("intId_usu") %>'
                                CssClass="btn-card-action btn-card-del shadow-sm"
                                OnClientClick="return confirm('¿CONFIRMAR BAJA DE USUARIO?\n\nEl usuario perderá el acceso al sistema.');" ToolTip="Cambiar Estado / Baja">
                                <i class="fa-solid fa-power-off"></i>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnVerHistorial" runat="server"
                                CommandArgument='<%# Eval("intId_usu") %>'
                                CssClass="btn btn-outline-primary btn-sm me-1"
                                OnClick="btnVerHistorial_Click">
                                <i class="fa-solid fa-history me-1"></i> Historial
                            </asp:LinkButton>
                        </div>
                    </div>

                </ItemTemplate>
            </asp:Repeater>
        </div>
        
        <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="text-center py-5 bg-white rounded-4 shadow-utc mt-3">
            <div class="opacity-25 mb-3">
                <i class="fa-solid fa-user-slash fa-4x text-muted"></i>
            </div>
            <h5 class="text-muted fw-bold">No se encontraron usuarios coincidentes</h5>
            <p class="small text-secondary">Intente con otro término de búsqueda.</p>
        </asp:Panel>

    </asp:Panel>

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        
        <div class="row justify-content-center">
            <div class="col-md-8 col-lg-6">
                
                <div class="card border-0 shadow-utc rounded-4 overflow-hidden">
                    
                    <div class="card-header bg-white p-4 text-center border-0">
                        <div class="mb-3">
                            <span class="d-inline-flex align-items-center justify-content-center bg-primary bg-opacity-10 text-primary rounded-circle" style="width: 60px; height: 60px;">
                                <i class="fa-solid fa-user-shield fa-2x"></i>
                            </span>
                        </div>
                        <h4 class="fw-bold mb-0 text-primary">
                            <asp:Label ID="lblTituloFormulario" runat="server">Gestión de Usuario</asp:Label>
                        </h4>
                        <p class="text-muted small mb-0 mt-2">Complete la información del perfil de acceso</p>
                    </div>
                    
                    <div class="card-body p-4 p-md-5 pt-2 bg-white">
                        <asp:HiddenField ID="hfIdUsuario" runat="server" />

                        <div class="mb-4">
                            <h6 class="text-uppercase small fw-bold text-secondary mb-3 border-bottom pb-2">
                                <i class="fa-solid fa-key me-1"></i> Credenciales
                            </h6>
                            
                            <div class="form-floating mb-3">
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control rounded-3" placeholder="NombreUsuario" />
                                <label class="text-muted"><i class="fa-solid fa-user me-2"></i>Nombre de Usuario</label>
                            </div>

                            <div class="form-floating">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control rounded-3" TextMode="Password" placeholder="Contraseña" />
                                <label class="text-muted"><i class="fa-solid fa-lock me-2"></i>Contraseña</label>
                            </div>
                            <div class="form-text mt-2 small text-info fst-italic" id="lblInfoPass" runat="server" visible="false">
                                <i class="fa-solid fa-circle-info me-1"></i> Deje este campo vacío para mantener la contraseña actual.
                            </div>
                        </div>

                        <div class="mb-4">
                            <h6 class="text-uppercase small fw-bold text-secondary mb-3 border-bottom pb-2">
                                <i class="fa-solid fa-shield-halved me-1"></i> Permisos y Estado
                            </h6>
                            
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Rol del Sistema</label>
                                <div class="input-group">
                                    <span class="input-group-text bg-light text-primary"><i class="fa-solid fa-id-card-clip"></i></span>
        
                                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select shadow-none" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlRol_SelectedIndexChanged">
                                        <asp:ListItem Text="-- Seleccionar Rol --" Value="" />
                                        <asp:ListItem Text="ADMINISTRADOR" Value="ADMINISTRADOR" />
                                        <asp:ListItem Text="COORDINADOR" Value="COORDINADOR" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <asp:Panel ID="pnlSeleccionCoordinador" runat="server" Visible="false" CssClass="mb-4 animate__animated animate__fadeIn">
                                <div class="p-3 bg-primary bg-opacity-10 border border-primary rounded-3">
                                    <label class="form-label fw-bold text-primary small">
                                        <i class="fa-solid fa-link me-1"></i> Vincular con Proyecto Aprobado
                                    </label>
        
                                    <asp:DropDownList ID="ddlCandidatos" runat="server" CssClass="form-select border-primary shadow-sm" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlCandidatos_SelectedIndexChanged">
                                    </asp:DropDownList>
        
                                    <div class="form-text small text-primary mt-1">
                                        <i class="fa-solid fa-circle-info"></i> Seleccione un coordinador de la lista para autocompletar sus datos.
                                    </div>
                                </div>
                            </asp:Panel>

                            <div class="form-check form-switch p-3 bg-light rounded-3 border d-flex align-items-center justify-content-between">
                                <div>
                                    <label class="form-check-label fw-bold text-dark" for="<%= chkActivo.ClientID %>" style="cursor: pointer;">Cuenta Habilitada</label>
                                    <div class="small text-muted" style="font-size: 0.75rem;">El usuario podrá iniciar sesión</div>
                                </div>
                                <input class="form-check-input ms-0 shadow-none" type="checkbox" id="chkActivo" runat="server" checked style="width: 3em; height: 1.5em; cursor: pointer;">
                            </div>
                        </div>

                        <div class="d-grid gap-2 d-md-flex justify-content-md-center mt-5">
                            <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 py-2 shadow fw-bold" OnClick="btnGuardar_Click">
                                <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Cambios
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-outline-secondary btn-pill px-4 py-2 fw-bold" OnClick="btnCancelar_Click" CausesValidation="false">
                                Cancelar
                            </asp:LinkButton>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>

    <div class="modal fade" id="modalHistorial" tabindex="-1" aria-labelledby="modalHistorialLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content shadow-utc rounded-4 border-0">

                <div class="modal-header utc-modal-header border-0">
                    <div class="d-flex align-items-center">
                        <div class="utc-modal-icon me-3">
                            <i class="fa-solid fa-clock-rotate-left fa-lg"></i>
                        </div>
                        <div>
                            <h5 class="fw-bold mb-0 text-white" id="modalHistorialLabel">
                                Historial del Usuario
                            </h5>
                            <p class="small mb-0 mt-1 text-white-50">
                                Visualice todas las acciones realizadas
                            </p>
                        </div>
                    </div>

                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body p-3">
                    <asp:Repeater ID="rptHistorialModal" runat="server">
                        <HeaderTemplate>
                            <div class="table-responsive">
                                <table class="table table-striped table-bordered table-hover text-center">
                                    <thead class="bg-primary bg-opacity-10">
                                        <tr>
                                            <th>Fecha</th>
                                            <th>Evento</th>
                                            <th>Rol</th>
                                            <th>Activo</th>
                                            <th>Realizado por</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("FechaEvento", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("TipoEvento") %></td>
                                <td><%# Eval("Rol") %></td>
                                <td><%# Convert.ToBoolean(Eval("Activo")) ? "Sí" : "No" %></td>
                                <td><%# Eval("RealizadoPor") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                    </tbody>
                                </table>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="modal-footer border-0">
                    <button type="button" class="btn btn-outline-secondary btn-pill" data-bs-dismiss="modal">
                        Cerrar
                    </button>
                </div>

            </div>
        </div>
    </div>

</asp:Content>