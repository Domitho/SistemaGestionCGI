<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaGestionCGI.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
    
    <style>
        /* Estilos específicos para las Tarjetas KPI del Dashboard */
        .kpi-icon {
            font-size: 2.5rem;
            opacity: 0.2;
            position: absolute;
            right: 20px;
            top: 20px;
        }
        .kpi-value {
            font-size: 2.5rem;
            font-weight: 700;
            color: var(--utc-azul);
            line-height: 1;
        }
        .kpi-label {
            font-size: 0.85rem;
            font-weight: 600;
            text-transform: uppercase;
            color: #6c757d;
            letter-spacing: 0.5px;
        }
        .chart-container {
            min-height: 350px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="card shadow-utc border-0 rounded-4 p-3 mb-4">
        <div class="d-flex justify-content-between align-items-center">
            <h3 class="utc-title mb-0">
                <i class="fa-solid fa-gauge-high me-2"></i> Dashboard Institucional
            </h3>
            <span class="text-muted small">
                <i class="fa-regular fa-calendar me-1"></i> 
                <asp:Label ID="lblFechaActual" runat="server"></asp:Label>
            </span>
        </div>
    </div>

    <div class="row g-3 mb-4">
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden">
                <i class="fa-solid fa-building-columns kpi-icon text-primary"></i>
                <div class="d-flex flex-column">
                    <span class="kpi-value">
                        <asp:Label ID="lblCentros" runat="server" Text="0"></asp:Label>
                    </span>
                    <span class="kpi-label mt-2">Centros de Inv.</span>
                </div>
                <div class="mt-3">
                     <a href="CentInv.aspx" class="btn btn-sm btn-outline-primary rounded-pill px-3" style="font-size: 0.75rem;">
                         Ver detalle <i class="fa-solid fa-arrow-right ms-1"></i>
                     </a>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden">
                <i class="fa-solid fa-bullhorn kpi-icon text-success"></i>
                <div class="d-flex flex-column">
                    <span class="kpi-value" style="color: var(--utc-verde) !important;">
                        <asp:Label ID="lblConvocatorias" runat="server" Text="0"></asp:Label>
                    </span>
                    <span class="kpi-label mt-2">Convocatorias</span>
                </div>
                <div class="mt-3">
                     <span class="badge bg-success bg-opacity-10 text-success rounded-pill">Activas</span>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden">
                <i class="fa-solid fa-users-gear kpi-icon text-warning"></i>
                <div class="d-flex flex-column">
                    <span class="kpi-value text-warning">
                        <asp:Label ID="lblGruInv" runat="server" Text="0"></asp:Label>
                    </span>
                    <span class="kpi-label mt-2">Grupos de Inv.</span>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden">
                <i class="fa-solid fa-user-graduate kpi-icon text-info"></i>
                <div class="d-flex flex-column">
                    <span class="kpi-value text-info">
                        <asp:Label ID="lblIntegrantes" runat="server" Text="0"></asp:Label>
                    </span>
                    <span class="kpi-label mt-2">Integrantes</span>
                </div>
            </div>
        </div>
    </div>

    <div class="row g-3">
        <div class="col-12 col-lg-8">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100">
                <h5 class="utc-subtitle mb-3">
                    <i class="fa-solid fa-chart-column me-2"></i> Docentes por Categoría
                </h5>
                <div id="chartCategorias" class="chart-container"></div>
            </div>
        </div>

        <div class="col-12 col-lg-4">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100">
                <h5 class="utc-subtitle mb-3">
                    <i class="fa-solid fa-chart-pie me-2"></i> Estado de Proyectos
                </h5>
                <div id="chartEstados" class="d-flex justify-content-center align-items-center"></div>
            </div>
        </div>

        <div class="col-12">
            <div class="card shadow-utc border-0 rounded-4 p-4">
                <h5 class="utc-subtitle mb-3">
                    <i class="fa-solid fa-book-open me-2"></i> Publicaciones por Tipo
                </h5>
                <div id="chartPublicaciones"></div>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            // Paleta de Colores UTC
            const utcAzul = '#312783';
            const utcVerde = '#1b9e4b';
            const utcRojo = '#d9534f';
            const utcCeleste = '#00b4ff';

            // --- 1. LEER DATOS INYECTADOS DESDE EL SERVIDOR ---
            // Usamos las variables públicas declaradas en el Code-Behind
            var rawCategorias = <%= JsonCategorias %>;
            var rawEstados = <%= JsonEstados %>;
            var rawPublicaciones = <%= JsonPublicaciones %>;

            // --- 2. RENDERIZAR GRÁFICO 1: CATEGORÍAS ---
            var optionsCat = {
                series: [{
                    name: 'Docentes',
                    data: rawCategorias.map(function (x) { return x.value; })
                }],
                chart: { type: 'bar', height: 350, toolbar: { show: false }, fontFamily: 'Segoe UI, sans-serif' },
                plotOptions: { bar: { borderRadius: 4, horizontal: true, barHeight: '70%' } },
                colors: [utcAzul],
                xaxis: {
                    categories: rawCategorias.map(function (x) { return x.label; })
                },
                dataLabels: { enabled: true, textAnchor: 'start', offsetX: 0, formatter: function (val) { return val + " Docentes" } }
            };
            var chartCat = new ApexCharts(document.querySelector("#chartCategorias"), optionsCat);
            chartCat.render();

            // --- 3. RENDERIZAR GRÁFICO 2: ESTADOS ---
            var optionsEst = {
                series: rawEstados.map(function (x) { return x.value; }),
                labels: rawEstados.map(function (x) { return x.label; }),
                chart: { type: 'donut', height: 320, fontFamily: 'Segoe UI, sans-serif' },
                colors: [utcVerde, '#ffc107', utcRojo],
                plotOptions: { pie: { donut: { size: '65%', labels: { show: true, total: { show: true, label: 'Total', color: utcAzul } } } } },
                legend: { position: 'bottom' },
                dataLabels: { enabled: false }
            };
            var chartEst = new ApexCharts(document.querySelector("#chartEstados"), optionsEst);
            chartEst.render();

            // --- 4. RENDERIZAR GRÁFICO 3: PUBLICACIONES ---
            var optionsPub = {
                series: [{
                    name: 'Cantidad',
                    data: rawPublicaciones.map(function (x) { return x.value; })
                }],
                chart: { height: 300, type: 'bar', toolbar: { show: false }, fontFamily: 'Segoe UI, sans-serif' },
                colors: [utcCeleste],
                plotOptions: { bar: { columnWidth: '40%', borderRadius: 5, distributed: true } },
                xaxis: {
                    categories: rawPublicaciones.map(function (x) { return x.label; }),
                    labels: { style: { fontSize: '12px' } }
                },
                legend: { show: false }
            };
            var chartPub = new ApexCharts(document.querySelector("#chartPublicaciones"), optionsPub);
            chartPub.render();
        });
    </script>

</asp:Content>