<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SistemaGestionCGI.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <%-- RECURSOS DE ESTILO UTC --%>
    <link href="DesignersUTC/Styles/utc-full-design.css" rel="stylesheet" />
        <link href="DesignersUTC/Styles/utc-fileinput.css" rel="stylesheet" />

    <style>
        /* --- ESTILOS ESPECÍFICOS PARA TARJETAS DE USUARIO (UTC) --- */
        
        .user-grid-container {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 1.5rem;
        }

        .user-card-pro {
            background: #fff;
            border-radius: 16px;
            padding: 25px;
            position: relative;
            border: 1px solid #eef2f7;
            box-shadow: 0 4px 20px rgba(0,0,0,0.05); /* Sombra suave inicial */
            transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            overflow: hidden;
        }

        /* Efecto Hover Institucional */
        .user-card-pro:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 30px rgba(0, 56, 118, 0.15); /* Sombra Azul UTC */
            border-color: var(--utc-azul);
        }

        /* Avatar con colores UTC */
        .pro-avatar {
            width: 80px;
            height: 80px;
            border-radius: 50%;
            /* Degradado Institucional */
            background: linear-gradient(135deg, var(--utc-azul) 0%, var(--utc-azul-oscuro) 100%);
            color: #fff;
            font-size: 2rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            margin-bottom: 15px;
            border: 4px solid #fff;
            box-shadow: 0 5px 15px rgba(0, 56, 118, 0.2);
            text-shadow: 1px 1px 2px rgba(0,0,0,0.2);
        }

        /* Indicador de Estado */
        .status-badge {
            position: absolute;
            top: 15px;
            right: 15px;
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 0.65rem;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .st-active { background-color: #d1e7dd; color: #0f5132; }
        .st-inactive { background-color: #f8d7da; color: #842029; }

        /* Rol Badge */
        .role-pill {
            display: inline-block;
            padding: 4px 12px;
            background: #eef2f7;
            color: var(--utc-azul-oscuro);
            border-radius: 6px;
            font-size: 0.7rem;
            font-weight: 700;
            text-transform: uppercase;
            margin-bottom: 10px;
            border: 1px solid rgba(0,0,0,0.05);
        }
        /* Resaltar Coordinador */
        .role-coord { 
            background: #fff3cd; 
            color: #856404; 
            border-color: #ffecb5;
        }

        /* Botones de acción */
        .card-actions {
            margin-top: auto;
            width: 100%;
            display: flex;
            gap: 10px;
            justify-content: center;
            padding-top: 15px;
            border-top: 1px solid #f8f9fa;
        }

        .btn-card-action {
            width: 38px;
            height: 38px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: 0.2s;
            border: none;
            cursor: pointer;
            text-decoration: none;
        }
        
        /* Botón EDITAR (Amarillo con ícono oscuro) */
        .btn-card-edit { 
            background-color: #ffc107 !important; /* Amarillo estándar */
            color: #212529 !important;            /* Ícono oscuro para contraste */
            border: none;
        }
        .btn-card-edit:hover { 
            background-color: #e0a800 !important; /* Amarillo más oscuro al pasar el mouse */
            color: #000 !important;
            transform: scale(1.1); 
        }
        
        /* Botón ELIMINAR (Rojo con ícono rojo) */
        .btn-card-del { 
            background-color: #fff; 
            border: 1px solid #dc3545; 
            color: #dc3545; 
        }
        .btn-card-del:hover { 
            background-color: #dc3545; 
            color: #fff; 
            transform: scale(1.1); 
        }

    </style>

    <%-- ENCABEZADO PRINCIPAL --%>
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

    <%-- PANEL DE LISTADO --%>
    <asp:Panel ID="pnlGrilla" runat="server">
        
        <%-- BARRA DE BÚSQUEDA FLOTANTE --%>
        <div class="bg-white p-3 rounded-4 shadow-utc mb-4 border d-flex align-items-center">
            <i class="fa-solid fa-magnifying-glass text-primary fs-5 ms-2 me-3"></i>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control border-0 shadow-none bg-transparent fs-6" 
                placeholder="Buscar por nombre de usuario, rol o ID..." AutoPostBack="true" OnTextChanged="txtBuscar_TextChanged"></asp:TextBox>
        </div>

        <%-- GRID DE TARJETAS --%>
        <div class="user-grid-container">
            <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                <ItemTemplate>
                    
                    <div class="user-card-pro">
                        <%-- Badge de Estado --%>
                        <div class='status-badge <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "st-active" : "st-inactive" %>'>
                             <%# Convert.ToBoolean(Eval("bActivo_usu")) ? "<i class='fa-solid fa-circle me-1' style='font-size:6px; vertical-align:middle;'></i>ACTIVO" : "<i class='fa-solid fa-circle me-1' style='font-size:6px; vertical-align:middle;'></i>INACTIVO" %>
                        </div>

                        <%-- Avatar --%>
                        <div class="pro-avatar">
                            <%# ObtenerIniciales(Eval("strNombre_usu").ToString()) %>
                        </div>

                        <%-- Nombre y Rol --%>
                        <h5 class="fw-bold text-dark mb-1 text-truncate w-100" title='<%# Eval("strNombre_usu") %>'>
                            <%# Eval("strNombre_usu") %>
                        </h5>
                        
                        <span class='role-pill <%# Eval("strRol_usu").ToString() == "COORDINADOR" ? "role-coord" : "" %>'>
                            <%# Eval("strRol_usu") %>
                        </span>

                        <div class="text-muted small mb-3">
                            <i class="fa-solid fa-fingerprint me-1 text-primary"></i> ID: <strong><%# Eval("intId_usu") %></strong>
                        </div>

                        <%-- Acciones --%>
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
                        </div>
                    </div>

                </ItemTemplate>
            </asp:Repeater>
        </div>
        
        <%-- Mensaje Sin Resultados --%>
        <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="text-center py-5 bg-white rounded-4 shadow-utc mt-3">
            <div class="opacity-25 mb-3">
                <i class="fa-solid fa-user-slash fa-4x text-muted"></i>
            </div>
            <h5 class="text-muted fw-bold">No se encontraron usuarios coincidentes</h5>
            <p class="small text-secondary">Intente con otro término de búsqueda.</p>
        </asp:Panel>

    </asp:Panel>

    <%-- PANEL DE FORMULARIO --%>
    <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
        
        <div class="row justify-content-center">
            <div class="col-md-8 col-lg-6">
                
                <div class="card border-0 shadow-utc rounded-4 overflow-hidden">
                    
                    <%-- Cabecera del Formulario --%>
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

                        <%-- Sección Credenciales --%>
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

                        <%-- Sección Permisos --%>
                        <div class="mb-4">
                            <h6 class="text-uppercase small fw-bold text-secondary mb-3 border-bottom pb-2">
                                <i class="fa-solid fa-shield-halved me-1"></i> Permisos y Estado
                            </h6>
                            
                            <div class="mb-3">
                                <label class="form-label small fw-bold text-muted">Rol del Sistema</label>
                                <div class="input-group">
                                    <span class="input-group-text bg-light text-primary"><i class="fa-solid fa-id-card-clip"></i></span>
                                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select shadow-none">
                                        <asp:ListItem Text="-- Seleccionar Rol --" Value="" />
                                        <asp:ListItem Text="ADMINISTRADOR" Value="ADMINISTRADOR" />
                                        <asp:ListItem Text="COORDINADOR" Value="COORDINADOR" />
                                        <%-- Puedes agregar más roles aquí --%>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-check form-switch p-3 bg-light rounded-3 border d-flex align-items-center justify-content-between">
                                <div>
                                    <label class="form-check-label fw-bold text-dark" for="<%= chkActivo.ClientID %>" style="cursor: pointer;">Cuenta Habilitada</label>
                                    <div class="small text-muted" style="font-size: 0.75rem;">El usuario podrá iniciar sesión</div>
                                </div>
                                <input class="form-check-input ms-0 shadow-none" type="checkbox" id="chkActivo" runat="server" checked style="width: 3em; height: 1.5em; cursor: pointer;">
                            </div>
                        </div>

                        <%-- Botones de Acción --%>
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

</asp:Content>