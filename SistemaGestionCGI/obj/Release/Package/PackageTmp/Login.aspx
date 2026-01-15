<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaGestionCGI.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>UTC - Sistema Integrado de Gestión</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" rel="stylesheet" />

    <link href="DesignersUTC/Styles/login-full.css" rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>

    <script type="text/javascript">
        function toastify(type, msg, title) {
            // Configuración
            toastr.options = {
                "closeButton": true,
                "progressBar": true,
                "positionClass": "toast-top-right",
                "timeOut": "4000"
            };

            // Mapeo
            if (type === 'ee' || type === 'error') toastr.error(msg, title);
            else if (type === 'ww' || type === 'warning') toastr.warning(msg, title);
            else if (type === 'ss' || type === 'success') toastr.success(msg, title);
            else toastr.info(msg, title);
        }
    </script>
</head>

<body>
    <div class="page-wrapper">
        <header class="utc-header">
            <img src="https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png" alt="UTC" />
            <div class="utc-header-title">SISTEMA INTEGRADO DE GESTIÓN</div>
            <div style="width:200px;"></div>
        </header>

        <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <div class="d-flex justify-content-center mt-5" style="min-height:70vh;">
                <div class="modal-dialog modal-login">
                    <div class="modal-content">
                        <div class="avatar">
                            <img src="public/avatar.png" alt="Avatar" />
                        </div>
                        <h4>INGRESE SUS<br />CREDENCIALES</h4>

                        <div class="text-center mt-2">
                            <asp:Label ID="LblMsg" CssClass="text-danger fw-bold" runat="server"></asp:Label>
                        </div>

                        <div class="mt-4">
                            <div class="mb-3">
                                <asp:TextBox ID="UserName" CssClass="form-control" Placeholder="Nombre de Usuario" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <asp:TextBox ID="Password" TextMode="Password" CssClass="form-control" Placeholder="Contraseña" runat="server"></asp:TextBox>
                            </div>
                            <asp:Button ID="LoginButton" runat="server" CssClass="btn w-100 mt-2" Text="Inicio de sesión" OnClick="LoginButton_Click2" />
                        </div>

                         <div class="modal-footer mt-4 d-flex justify-content-center">
                            <a href="#" target="_blank">¿Olvidó su contraseña?</a>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>