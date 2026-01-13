<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SistemaGestionCGI.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />

    <style>
        /* --- ESTILOS PROFESIONALES (CARD VIEW) --- */
        
        .user-grid-container {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 1.5rem;
        }

        .user-card-pro {
            background: #fff;
            border-radius: 16px;
            padding: 25px;
            position: relative;
            border: 1px solid rgba(0,0,0,0.04);
            box-shadow: 0 4px 15px rgba(0,0,0,0.03);
            transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
        }

        .user-card-pro:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 30px rgba(49, 39, 131, 0.1);
            border-color: var(--utc-azul-oscuro);
        }

        /* Avatar circular con iniciales */
        .pro-avatar {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            color: var(--utc-azul);
            font-size: 2rem;
            font-weight: 800;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 15px;
            border: 4px solid #fff;
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
        }

        /* Indicador de Estado (Punto de luz) */
        .status-badge {
            position: absolute;
            top: 20px;
            right: 20px;
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 0.75rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .st-active { background-color: rgba(25, 135, 84, 0.1); color: #198754; }
        .st-inactive { background-color: rgba(220, 53, 69, 0.1); color: #dc3545; }

        /* Rol Badge */
        .role-pill {
            display: inline-block;
            padding: 4px 12px;
            background: var(--utc-azul);
            color: white;
            border-radius: 6px;
            font-size: 0.7rem;
            font-weight: 600;
            text-transform: uppercase;
            margin-bottom: 15px;
            box-shadow: 0 4px 10px rgba(49, 39, 131, 0.3);
        }
        .role-coord { background: #0dcaf0; color: #000; box-shadow: 0 4px 10px rgba(13, 202, 240, 0.3); }

        /* Botones de acción flotantes en la tarjeta */
        .card-actions {
            margin-top: auto; /* Empuja al fondo */
            width: 100%;
            display: flex;
            gap: 10px;
            justify-content: center;
            padding-top: 15px;
            border-top: 1px solid #f0f0f0;
        }

        .btn-card-action {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: 0.2s;
            border: none;
        }
        .btn-card-edit { background: #fff3cd; color: #856404; }
        .btn-card-edit:hover { background: #ffe69c; transform: scale(1.1); }
        
        .btn-card-del { background: #f8d7da; color: #721c24; }
        .btn-card-del:hover { background: #f1b0b7; transform: scale(1.1); }

    </style>

    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h3 class="utc-title mb-0">Directorio de Usuarios</h3>
            <p class="text-muted small mb-0">Gestión de accesos y perfiles administrativos</p>
        </div>
        
        <asp:LinkButton ID="btnNuevoUsuario" runat="server" CssClass="btn btn-primary btn-pill shadow px-4 py-2" OnClick="btnNuevoUsuario_Click">
            <i class="fa-solid fa-plus me-2"></i> Crear Nuevo Perfil
        </asp:LinkButton>
    </div>

    <asp:Panel ID="pnlGrilla" runat="server">
        
        <div class="bg-white p-3 rounded-4 shadow-sm mb-4 border d-flex align-items-center">
            <i class="fa-solid fa-search text-muted ms-2 me-3"></i>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control border-0 shadow-none bg-transparent" 
                placeholder="Buscar por nombre de usuario..." AutoPostBack="true" OnTextChanged="txtBuscar_TextChanged"></asp:TextBox>
        </div>

        <div class="user-grid-container">
            <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                <ItemTemplate>
                    
                    <div class="user-card-pro">
                        <div class='status-badge <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "st-active" : "st-inactive" %>'>
                             <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "Activo" : "Inactivo" %>
                        </div>

                        <div class="pro-avatar">
                            <%# ObtenerIniciales(Eval("strNombre_usu").ToString()) %>
                        </div>

                        <h5 class="fw-bold text-dark mb-1"><%# Eval("strNombre_usu") %></h5>
                        
                        <span class='role-pill <%# Eval("strRol_usu").ToString() == "COORDINADOR" ? "role-coord" : "" %>'>
                            <%# Eval("strRol_usu") %>
                        </span>

                        <div class="text-muted small mb-3">
                            <i class="fa-solid fa-id-badge me-1"></i> ID: <%# Eval("intId_usu") %>
                        </div>

                        <div class="card-actions">
                            <asp:LinkButton ID="btnEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("intId_usu") %>'
                                CssClass="btn-card-action btn-card-edit shadow-sm" ToolTip="Editar Perfil">
                                <i class="fa-solid fa-pen"></i>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("intId_usu") %>'
                                CssClass="btn-card-action btn-card-del shadow-sm"
                                OnClientClick="return confirm('¿CONFIRMAR BAJA DE USUARIO?\n\nEl usuario perderá el acceso al sistema.');" ToolTip="Desactivar">
                                <i class="fa-solid fa-power-off"></i>
                            </asp:LinkButton>
                        </div>
                    </div>

                </ItemTemplate>
            </asp:Repeater>
        </div>
        
        <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="text-center py-5">
            <div class="opacity-50 mb-3">
                <i class="fa-solid fa-users-slash fa-4x text-muted"></i>
            </div>
            <h5 class="text-muted">No se encontraron usuarios</h5>
        </asp:Panel>

    </asp:Panel>

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        
        <div class="row justify-content-center">
            <div class="col-md-8 col-lg-6">
                
                <div class="card border-0 shadow-lg rounded-4 overflow-hidden">
                    <div class="card-header bg-utc text-white p-4 text-center border-0">
                        <h4 class="fw-bold mb-0">
                            <i class="fa-solid fa-user-circle me-2"></i> 
                            <asp:Label ID="lblTituloFormulario" runat="server">Usuario</asp:Label>
                        </h4>
                    </div>
                    
                    <div class="card-body p-4 p-md-5 bg-white">
                        <asp:HiddenField ID="hfIdUsuario" runat="server" />

                        <div class="mb-4">
                            <label class="form-label text-uppercase small fw-bold text-muted">Credenciales</label>
                            
                            <div class="form-floating mb-3">
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="NombreUsuario" />
                                <label>Nombre de Usuario (Login)</label>
                            </div>

                            <div class="form-floating">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña" />
                                <label>Contraseña</label>
                            </div>
                            <div class="form-text mt-2 small text-primary" id="lblInfoPass" runat="server" visible="false">
                                <i class="fa-solid fa-circle-info me-1"></i> Deje en blanco para mantener la contraseña actual.
                            </div>
                        </div>

                        <hr class="opacity-10 my-4">

                        <div class="mb-4">
                            <label class="form-label text-uppercase small fw-bold text-muted">Permisos y Accesos</label>
                            
                            <div class="mb-3">
                                <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select form-select-lg">
                                    <asp:ListItem Text="-- Seleccionar Rol --" Value="" />
                                    <asp:ListItem Text="ADMINISTRADOR" Value="ADMINISTRADOR" />
                                    <asp:ListItem Text="COORDINADOR" Value="COORDINADOR" />
                                </asp:DropDownList>
                            </div>

                            <div class="form-check form-switch p-3 bg-light rounded border d-flex align-items-center">
                                <input class="form-check-input ms-0 me-3" type="checkbox" id="chkActivo" runat="server" checked style="transform: scale(1.3);">
                                <div>
                                    <label class="form-check-label fw-bold text-dark" for="chkActivo">Cuenta Habilitada</label>
                                    <div class="small text-muted">Permite iniciar sesión en el sistema</div>
                                </div>
                            </div>
                        </div>

                        <div class="d-grid gap-2 d-md-flex justify-content-md-end mt-5">
                            <asp:LinkButton ID="btnCancelar" runat="server" CssClass="btn btn-light rounded-pill px-4" OnClick="btnCancelar_Click" CausesValidation="false">
                                Cancelar
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-primary btn-pill px-5 shadow" OnClick="btnGuardar_Click">
                                <i class="fa-solid fa-floppy-disk me-2"></i> Guardar Usuario
                            </asp:LinkButton>
                        </div>

                    </div>
                </div>

            </div>
        </div>

    </asp:Panel>

</asp:Content>