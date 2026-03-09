<%@ Page Title="Dashboard Institucional" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaGestionCGI.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
    <link href="DesignersUTC/Styles/Dashboard.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <!-- HEADER -->
    <div class="card shadow-utc border-0 rounded-4 p-3 mb-4">
        <div class="d-flex justify-content-between align-items-center flex-wrap">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-gauge-high me-2"></i> Dashboard Institucional
            </h3>
            <span class="text-muted small mt-2 mt-md-0">
                <i class="fa-regular fa-calendar me-1"></i>
                <asp:Label ID="lblFechaActual" runat="server"></asp:Label>
            </span>
        </div>
    </div>

    <!-- KPI CARDS -->
    <div class="row g-3 mb-4">
        <!-- CENTROS -->
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 kpi-card">
                <i class="fa-solid fa-building-columns kpi-icon text-primary"></i>
                <div class="d-flex flex-column justify-content-between h-100">
                    <div>
                        <span class="kpi-value"><asp:Label ID="lblCentros" runat="server" Text="0"></asp:Label></span>
                        <div class="kpi-label mt-1">Centros de Inv.</div>
                    </div>
                    <div class="badge-subtotal bg-primary bg-opacity-10 text-primary mt-3 border border-primary border-opacity-10">
                        <i class="fa-solid fa-users me-2"></i>
                        <asp:Label ID="lblIntegrantesCentros" runat="server" Text="0"></asp:Label> Integrantes
                    </div>
                    <asp:Button ID="btnCentros" runat="server" CssClass="btn btn-primary btn-kpi" Text="Ver Centros"
                        OnClientClick="window.location='ModuloCentros.aspx'; return false;" />
                </div>
            </div>
        </div>

        <!-- CONVOCATORIAS -->
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 kpi-card">
                <i class="fa-solid fa-bullhorn kpi-icon text-danger"></i>
                <div class="d-flex flex-column justify-content-between h-100">
                    <div>
                        <span class="kpi-value"><asp:Label ID="lblConvocatorias" runat="server" Text="0"></asp:Label></span>
                        <div class="kpi-label mt-1">Convocatorias</div>
                    </div>
                    <div class="mt-3">
                        <span class="badge bg-danger bg-opacity-10 text-danger rounded-pill">Registradas</span>
                    </div>
                    <asp:Button ID="btnConvocatorias" runat="server" CssClass="btn btn-danger btn-kpi" Text="Ver Convocatorias"
                        OnClientClick="window.location='ModuloConvocatorias.aspx'; return false;" />
                </div>
            </div>
        </div>

        <!-- GRUPOS -->
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 kpi-card">
                <i class="fa-solid fa-users-gear kpi-icon text-warning"></i>
                <div class="d-flex flex-column justify-content-between h-100">
                    <div>
                        <span class="kpi-value"><asp:Label ID="lblGrupos" runat="server" Text="0"></asp:Label></span>
                        <div class="kpi-label mt-1">Grupos de Inv.</div>
                    </div>
                    <div class="badge-subtotal bg-warning bg-opacity-10 text-dark mt-3 border border-warning border-opacity-25">
                        <i class="fa-solid fa-users me-2"></i>
                        <asp:Label ID="lblIntegrantesGrupos" runat="server" Text="0"></asp:Label> Integrantes
                    </div>
                    <asp:Button ID="btnGrupos" runat="server" CssClass="btn btn-warning btn-kpi" Text="Ver Grupos"
                        OnClientClick="window.location='ModuloGrupos.aspx'; return false;" />
                </div>
            </div>
        </div>

        <!-- DOCENTES -->
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 kpi-card">
                <i class="fa-solid fa-chalkboard-user kpi-icon text-success"></i>
                <div class="d-flex flex-column justify-content-between h-100">
                    <div>
                        <span class="kpi-value"><asp:Label ID="lblTotalDocentes" runat="server" Text="0"></asp:Label></span>
                        <div class="kpi-label mt-1">Total Docentes</div>
                    </div>
                    <div class="mt-3">
                        <span class="badge bg-success bg-opacity-10 text-success rounded-pill">Categorizados</span>
                    </div>
                    <asp:Button ID="btnDocentes" runat="server" CssClass="btn btn-success btn-kpi" Text="Ver Docentes"
                        OnClientClick="window.location='ModuloDocentes.aspx'; return false;" />
                </div>
            </div>
        </div>
    </div>

    <!-- GRAFICOS -->
    <div class="row g-3">
        <!-- Docentes por Categoria -->
        <div class="col-12 col-lg-8">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100">
                <h5 class="utc-subtitle mb-3"><i class="fa-solid fa-chart-bar me-2"></i> Docentes por Categoría</h5>
                <div id="chartCategorias" class="chart-container"></div>
            </div>
        </div>
        <!-- Estado de Proyectos -->
        <div class="col-12 col-lg-4">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100">
                <h5 class="utc-subtitle mb-3"><i class="fa-solid fa-chart-pie me-2"></i> Estado de Proyectos</h5>
                <div class="d-flex justify-content-center align-items-center h-100">
                    <div id="chartEstados" class="w-100"></div>
                </div>
            </div>
        </div>
    </div>

    <!-- MODAL DOCENTES -->
    <div class="modal fade" id="modalDetalleDocentes" tabindex="-1" aria-labelledby="modalDetalleLabel" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-body" id="modalDetalleBody"></div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL PROYECTOS -->
    <div class="modal fade" id="modalDetalleProyectos" tabindex="-1" aria-labelledby="modalProyectosLabel" aria-hidden="true">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-body" id="modalProyectosBody"></div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
          </div>
        </div>
      </div>
    </div>

    <!-- SCRIPT -->
    <script>
        $(document).ready(function () {
            var rawDocentes = <%= JsonDocentes %>;
            var docentesDetalle = <%= JsonDocentesDetalle %>;
            var rawProyectos = <%= JsonProyectos %>;
            var proyectosDetalle = <%= JsonProyectosDetalle %>;

            // Función para normalizar estados (mayúsculas y quitar tildes)
            function normalizarEstado(texto) {
                if (!texto) return '';
                return texto
                    .toUpperCase()
                    .normalize('NFD')
                    .replace(/[\u0300-\u036f]/g, '') // elimina tildes
                    .trim();
            }

            // ---------------------------
            // Gráfico Docentes por Categoría
            // ---------------------------
            new ApexCharts(document.querySelector("#chartCategorias"), {
                series: [{ name: 'Docentes', data: rawDocentes.map(x => x.Value) }],
                chart: {
                    type: 'bar',
                    height: 350,
                    toolbar: { show: false },
                    events: {
                        dataPointSelection: function (event, chartContext, config) {
                            var index = config.dataPointIndex;
                            var categoria = rawDocentes[index].Label;
                            var total = rawDocentes[index].Value;

                            var detalleFiltrado = docentesDetalle.filter(d => d.Categoria === categoria);

                            var html = `<div class="modal-header-info">
                                            <span>Categoría: ${categoria}</span>
                                            <span>Total Docentes: ${detalleFiltrado.length}</span>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                                        </div>
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>Cédula</th><th>Nombres</th><th>Apellidos</th>
                                                    <th>Facultad</th><th>Carrera</th><th>Activo</th><th>Correo</th>
                                                </tr>
                                            </thead>
                                            <tbody>`;
                            detalleFiltrado.forEach(function (doc) {
                                html += `<tr>
                                            <td>${doc.Cedula}</td>
                                            <td>${doc.Nombres}</td>
                                            <td>${doc.Apellidos}</td>
                                            <td>${doc.Facultad}</td>
                                            <td>${doc.Carrera}</td>
                                            <td>${doc.Activo ? "Sí" : "No"}</td>
                                            <td>${doc.Correo}</td>
                                         </tr>`;
                            });
                            html += "</tbody></table>";

                            document.getElementById("modalDetalleBody").innerHTML = html;
                            new bootstrap.Modal(document.getElementById('modalDetalleDocentes')).show();
                        }
                    }
                },
                colors: ['#312783'],
                plotOptions: { bar: { borderRadius: 4, horizontal: true, barHeight: '60%' } },
                xaxis: { categories: rawDocentes.map(x => x.Label) },
                dataLabels: { enabled: true, formatter: val => val + " Docentes" },
                grid: { borderColor: '#f1f1f1' }
            }).render();

            // ---------------------------
            // Gráfico Estado de Proyectos
            // ---------------------------
            var colorMap = { 'EN EJECUCION': '#1b9e4b', 'EN REVISION': '#ffc107', 'FINALIZADO': '#312783' };

            new ApexCharts(document.querySelector("#chartEstados"), {
                series: rawProyectos.map(x => x.Value),
                labels: rawProyectos.map(x => x.Label),
                chart: {
                    type: 'donut',
                    height: 350,
                    events: {
                        dataPointSelection: function (event, chartContext, config) {
                            var estadoGrafico = normalizarEstado(rawProyectos[config.dataPointIndex].Label);

                            var proyectosFiltrados = proyectosDetalle.filter(p => normalizarEstado(p.Estado) === estadoGrafico);

                            var html = `<table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>Código</th><th>Proyecto</th><th>Coordinador</th><th>Periodo</th>
                                                    <th>Fecha Inicio</th>
                                                </tr>
                                            </thead>
                                            <tbody>`;
                            proyectosFiltrados.forEach(function (proy) {
                                html += `<tr>
                                            <td>${proy.Codigo}</td>
                                            <td>${proy.NombreProyecto}</td>
                                            <td>${proy.Coordinador}</td>
                                            <td>${proy.Periodo}</td>
                                            <td>${proy.FechaInicio}</td>
                                         </tr>`;
                            });
                            html += "</tbody></table>";

                            document.getElementById("modalProyectosBody").innerHTML = html;
                            new bootstrap.Modal(document.getElementById('modalDetalleProyectos')).show();
                        }
                    }
                },
                colors: rawProyectos.map(x => colorMap[x.Label] || '#6c757d'),
                plotOptions: { pie: { donut: { size: '65%' } } }
            }).render();
        });
    </script>
</asp:Content>