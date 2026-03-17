<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Certificados.aspx.cs" Inherits="SistemaGestionCGI.Certificados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .modal-header-utc {
            background: linear-gradient(135deg, #0b3d91 0%, #0d57c6 100%);
        }

        .modal-header-utc .modal-title {
            font-weight: 800;
        }

        .modal-header-utc .btn-close {
            filter: invert(1) grayscale(100%) brightness(200%);
        }

        #modalReporte .modal-content {
            background: #fff;
        }

        #modalReporte .modal-body {
            background: #f4f7fb;
        }
    </style>

    <%-- HEADER PRINCIPAL --%>
    <div class="bg-white p-4 mb-4 rounded shadow-utc border header-utc-line">
        <div class="d-flex align-items-center justify-content-between flex-wrap gap-3">
            <div>
                <h3 class="utc-title mb-1">
                    <i class="fa-solid fa-certificate me-2"></i>
                    CONSULTA DE CERTIFICADOS
                </h3>
                <p class="text-muted mb-0">
                    Ingrese la cédula para consultar la participación registrada en los módulos de investigación.
                </p>
            </div>

            <asp:LinkButton runat="server" ID="btnRegresar" CssClass="btn btn-outline-primary btn-pill px-4" OnClick="btnRegresar_Click" CausesValidation="false">
                <i class="fa-solid fa-chevron-left me-2"></i> REGRESAR
            </asp:LinkButton>
        </div>
    </div>

    <%-- BUSCADOR --%>
    <asp:Panel ID="pnlBusqueda" runat="server" CssClass="mb-4">
        <div class="bg-white rounded-4 shadow-utc p-4 border-0">
            <div class="row justify-content-center">
                <div class="col-lg-8">
                    <div class="text-center mb-4">
                        <div class="mb-3">
                            <i class="fa-solid fa-id-card fa-2x text-primary"></i>
                        </div>
                        <h4 class="utc-subtitle mb-2">Buscar persona por cédula</h4>
                        <p class="text-muted mb-0">Consulte si la persona registra participación en grupos de investigación.</p>
                    </div>

                    <div class="row g-3 align-items-end">
                        <div class="col-md-8">
                            <label class="form-label fw-bold">Número de cédula</label>
                            <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control form-control-lg" MaxLength="10" autocomplete="off" placeholder="Ingrese la cédula" />
                        </div>

                        <div class="col-md-4">
                            <asp:LinkButton ID="btnBuscar" runat="server" CssClass="btn btn-primary btn-pill w-100 py-3" OnClick="btnBuscar_Click">
                                <i class="fa-solid fa-magnifying-glass me-2"></i> Buscar
                            </asp:LinkButton>
                        </div>

                        <div class="col-12 text-center">
                            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-semibold text-danger"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <%-- FICHA DE PERSONA --%>
    <asp:Panel ID="pnlPersona" runat="server" Visible="false" CssClass="mb-4">
        <div class="bg-white p-4 rounded-4 shadow-utc border-0">
            <div class="d-flex align-items-center gap-3 mb-3">
                <div class="rounded-circle bg-primary bg-opacity-10 d-flex align-items-center justify-content-center" style="width:64px; height:64px;">
                    <i class="fa-solid fa-user text-primary fa-xl"></i>
                </div>
                <div>
                    <h4 class="utc-subtitle mb-1">Información de la Persona</h4>
                    <p class="text-muted mb-0">Datos generales recuperados desde el registro encontrado.</p>
                </div>
            </div>

            <div class="row g-3 mt-1">
                <div class="col-md-4">
                    <div class="border rounded-4 p-3 h-100 bg-light-subtle">
                        <span class="text-muted small d-block">Cédula</span>
                        <asp:Label ID="lblCedula" runat="server" CssClass="fw-bold fs-5"></asp:Label>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="border rounded-4 p-3 h-100 bg-light-subtle">
                        <span class="text-muted small d-block">Nombres</span>
                        <asp:Label ID="lblNombres" runat="server" CssClass="fw-bold fs-5"></asp:Label>
                    </div>
                </div>

                <div class="col-md-4">
                    <div class="border rounded-4 p-3 h-100 bg-light-subtle">
                        <span class="text-muted small d-block">Apellidos</span>
                        <asp:Label ID="lblApellidos" runat="server" CssClass="fw-bold fs-5"></asp:Label>
                    </div>
                </div>
            </div>

            <div class="mt-4 p-3 rounded-4 bg-primary bg-opacity-10 border border-primary-subtle">
                <i class="fa-solid fa-circle-info me-2 text-primary"></i>
                <span class="fw-semibold">Resumen:</span>
                La persona registra participación en
                <strong><asp:Label ID="lblTotalGrupos" runat="server"></asp:Label></strong>
                grupo(s) de investigación.
            </div>
        </div>
    </asp:Panel>

    <%-- RESULTADOS INFORMATIVOS --%>
    <asp:Panel ID="pnlResultados" runat="server" Visible="false">
        <div class="mb-3">
            <h4 class="utc-subtitle">
                <i class="fa-solid fa-folder-open me-2"></i>
                Participaciones Encontradas
            </h4>
            <p class="text-muted mb-0">Revise el detalle de la participación registrada para esta persona.</p>
        </div>

        <div class="row g-4">
            <asp:Repeater ID="rptGrupos" runat="server" OnItemCommand="rptGrupos_ItemCommand">
                <ItemTemplate>
                    <div class="col-12">
                        <div class="bg-white rounded-4 shadow-utc p-4 border-0 h-100">
                            <div class="d-flex justify-content-between align-items-start flex-wrap gap-3 mb-3">
                                <div>
                                    <span class="badge bg-primary mb-2 px-3 py-2"><%# Eval("Modulo") %></span>
                                    <h5 class="fw-bold text-primary mb-1"><%# Eval("NombreGrupo") %></h5>
                                    <p class="text-muted mb-0">Código: <%# Eval("IdGrupo") %></p>
                                </div>

                                <div>
                                    <span class='badge px-3 py-2 <%# Eval("Estado").ToString() == "Activo" ? "bg-success" : "bg-secondary" %>'>
                                        <%# Eval("Estado") %>
                                    </span>
                                </div>
                            </div>

                            <div class="row g-3">
                                <div class="col-md-4">
                                    <div class="border rounded-4 p-3 h-100">
                                        <span class="text-muted small d-block">Función</span>
                                        <span class="fw-semibold"><%# Eval("Funcion") %></span>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div class="border rounded-4 p-3 h-100">
                                        <span class="text-muted small d-block">Fecha de inicio</span>
                                        <span class="fw-semibold"><%# Eval("FechaInicio") %></span>
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <div class="border rounded-4 p-3 h-100">
                                        <span class="text-muted small d-block">Fecha de fin</span>
                                        <span class="fw-semibold"><%# string.IsNullOrWhiteSpace(Eval("FechaFin").ToString()) ? "No registra" : Eval("FechaFin") %></span>
                                    </div>
                                </div>
                            </div>

                            <div class="d-flex flex-wrap gap-2 mt-4">

                                <asp:LinkButton ID="btnDescargar" runat="server"
                                    CssClass="btn btn-primary btn-pill px-4"
                                    CommandName="DescargarCertificado"
                                    CommandArgument='<%# Eval("IdIntegrante") %>'
                                    CausesValidation="false">
                                    <i class="fa-solid fa-eye me-2"></i> Vista previa
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <div class="modal fade" id="modalReporte" tabindex="-1" aria-labelledby="modalReporteLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg rounded-4 overflow-hidden">

                <div class="modal-header modal-header-utc text-white border-0">
                    <h5 class="modal-title fw-bold" id="modalReporteLabel">
                        <i class="fa-solid fa-file-lines me-2"></i> Vista previa del reporte
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body p-0 overflow-hidden" style="height: 80vh;">
                    <iframe id="iframeReporte"
                        src=""
                        style="width: 100%; height: 100%; border: none; display: block;"
                        scrolling="yes">
                    </iframe>
                </div>

                <div class="modal-footer bg-white">
                    <button type="button" class="btn btn-outline-secondary btn-pill px-4" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i> Cerrar
                    </button>
                </div>

            </div>
        </div>
    </div>

    <script type="text/javascript">
        function abrirModalReporte(url) {
            var iframe = document.getElementById('iframeReporte');
            iframe.src = url;

            var modal = new bootstrap.Modal(document.getElementById('modalReporte'));
            modal.show();
        }
    </script>

</asp:Content>