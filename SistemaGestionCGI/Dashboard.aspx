<%@ Page Title="Dashboard Institucional" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaGestionCGI.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>

    <style>
        /* Estilos Específicos del Dashboard basados en UTC Design */
        .kpi-card {
            transition: transform 0.2s ease-in-out;
        }
        .kpi-card:hover {
            transform: translateY(-5px);
        }
        .kpi-icon {
            font-size: 2.5rem;
            opacity: 0.15;
            position: absolute;
            right: 20px;
            top: 20px;
        }
        .kpi-value {
            font-size: 2.2rem;
            font-weight: 700;
            color: var(--utc-azul); /* [cite: 17] */
            line-height: 1;
        }
        .kpi-label {
            font-size: 0.85rem;
            font-weight: 600;
            text-transform: uppercase;
            color: #6c757d;
            letter-spacing: 0.5px;
        }
        /* Badge para subtotales (Integrantes) */
        .badge-subtotal {
            font-size: 0.75rem;
            font-weight: 500;
            padding: 5px 10px;
            border-radius: 50rem;
            display: inline-flex;
            align-items: center;
            width: fit-content;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="card shadow-utc border-0 rounded-4 p-3 mb-4"> <div class="d-flex justify-content-between align-items-center flex-wrap">
            <h3 class="utc-title mb-0"> <i class="fa-solid fa-gauge-high me-2"></i> Dashboard Institucional
            </h3>
            <span class="text-muted small mt-2 mt-md-0">
                <i class="fa-regular fa-calendar me-1"></i>
                <asp:Label ID="lblFechaActual" runat="server"></asp:Label> </span>
        </div>
    </div>

    <div class="row g-3 mb-4">
        
        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden kpi-card">
                <i class="fa-solid fa-building-columns kpi-icon text-primary"></i> <div class="d-flex flex-column h-100 justify-content-between">
                    <div>
                        <span class="kpi-value">
                            <asp:Label ID="lblCentros" runat="server" Text="0"></asp:Label>
                        </span>
                        <div class="kpi-label mt-1">Centros de Inv.</div>
                    </div>
                    <div class="badge-subtotal bg-primary bg-opacity-10 text-primary mt-3 border border-primary border-opacity-10">
                        <i class="fa-solid fa-users me-2"></i>
                        <asp:Label ID="lblIntegrantesCentros" runat="server" Text="0"></asp:Label> Integrantes
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden kpi-card">
                <i class="fa-solid fa-bullhorn kpi-icon text-danger"></i> <div class="d-flex flex-column h-100 justify-content-between">
                    <div>
                        <span class="kpi-value" style="color: var(--utc-rojo) !important;">
                            <asp:Label ID="lblConvocatorias" runat="server" Text="0"></asp:Label>
                        </span>
                        <div class="kpi-label mt-1">Convocatorias</div>
                    </div>
                    <div class="mt-3">
                        <span class="badge bg-danger bg-opacity-10 text-danger rounded-pill">Registradas</span>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden kpi-card">
                <i class="fa-solid fa-users-gear kpi-icon text-warning"></i> <div class="d-flex flex-column h-100 justify-content-between">
                    <div>
                        <span class="kpi-value text-warning">
                            <asp:Label ID="lblGrupos" runat="server" Text="0"></asp:Label>
                        </span>
                        <div class="kpi-label mt-1">Grupos de Inv.</div>
                    </div>
                     <div class="badge-subtotal bg-warning bg-opacity-10 text-dark mt-3 border border-warning border-opacity-25">
                        <i class="fa-solid fa-users me-2"></i>
                        <asp:Label ID="lblIntegrantesGrupos" runat="server" Text="0"></asp:Label> Integrantes
                    </div>
                </div>
            </div>
        </div>

        <div class="col-12 col-md-6 col-xl-3">
            <div class="card shadow-utc border-0 rounded-4 h-100 p-3 position-relative overflow-hidden kpi-card">
                <i class="fa-solid fa-chalkboard-user kpi-icon text-success"></i>
                <div class="d-flex flex-column h-100 justify-content-between">
                    <div>
                        <span class="kpi-value" style="color: var(--utc-verde) !important;">
                            <asp:Label ID="lblTotalDocentes" runat="server" Text="0"></asp:Label>
                        </span>
                        <div class="kpi-label mt-1">Total Docentes</div>
                    </div>
                    <div class="mt-3">
                        <span class="badge bg-success bg-opacity-10 text-success rounded-pill">Categorizados</span>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="row g-3">
        
        <div class="col-12 col-lg-8">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100"> <h5 class="utc-subtitle mb-3">
                    <i class="fa-solid fa-chart-bar me-2"></i> Docentes por Categoría
                </h5>
                <div id="chartCategorias" class="chart-container"></div> </div>
        </div>

        <div class="col-12 col-lg-4">
            <div class="card shadow-utc border-0 rounded-4 p-4 h-100">
                <h5 class="utc-subtitle mb-3">
                    <i class="fa-solid fa-chart-pie me-2"></i> Estado de Proyectos
                </h5>
                <div class="d-flex justify-content-center align-items-center h-100">
                    <div id="chartEstados" class="w-100"></div> </div>
            </div>
        </div>

    </div>

    <script>
        $(document).ready(function () {
            // Colores Institucionales (Coincidentes con CSS) [cite: 130-132]
            const utcAzul = '#312783';
            const utcVerde = '#1b9e4b';
            const utcAmarillo = '#ffc107';
            const utcRojo = '#d9534f';

            // 1. Obtener datos serializados desde el Backend
            var rawDocentes = <%= JsonDocentes %>;
            var rawProyectos = <%= JsonProyectos %>;

            // --- GRÁFICO 1: DOCENTES POR CATEGORÍA (Barras Horizontales) ---
            var optionsCat = {
                series: [{
                    name: 'Docentes',
                    data: rawDocentes.map(function (x) { return x.Value; })
                }],
                chart: {
                    type: 'bar',
                    height: 350,
                    toolbar: { show: false },
                    fontFamily: 'Segoe UI, sans-serif'
                },
                colors: [utcAzul], // Color principal UTC
                plotOptions: {
                    bar: {
                        borderRadius: 4,
                        horizontal: true,
                        barHeight: '60%',
                        dataLabels: { position: 'bottom' }
                    }
                },
                xaxis: {
                    categories: rawDocentes.map(function (x) { return x.Label; }),
                },
                dataLabels: {
                    enabled: true,
                    textAnchor: 'start',
                    style: { colors: ['#fff'] },
                    formatter: function (val, opt) {
                        return val + " Docentes"
                    },
                    offsetX: 0,
                },
                grid: { borderColor: '#f1f1f1' }
            };

            var chartCat = new ApexCharts(document.querySelector("#chartCategorias"), optionsCat);
            chartCat.render();


            // --- GRÁFICO 2: ESTADO DE PROYECTOS (Donut) ---
            // Mapeo manual de colores para asegurar semántica (Verde=Ejecución, etc.)
            var colorMap = {
                'EN EJECUCION': utcVerde,
                'EN REVISION': utcAmarillo,
                'FINALIZADO': utcAzul
            };

            var optionsProy = {
                series: rawProyectos.map(function (x) { return x.Value; }),
                labels: rawProyectos.map(function (x) { return x.Label; }),
                chart: {
                    type: 'donut',
                    height: 350,
                    fontFamily: 'Segoe UI, sans-serif'
                },
                // Asignar colores dinámicamente según la etiqueta, o usar gris por defecto
                colors: rawProyectos.map(function (x) { return colorMap[x.Label] || '#6c757d'; }),

                plotOptions: {
                    pie: {
                        donut: {
                            size: '65%',
                            labels: {
                                show: true,
                                name: { show: true },
                                value: { show: true, fontSize: '20px', fontWeight: 600, color: utcAzul },
                                total: {
                                    show: true,
                                    showAlways: true,
                                    label: 'Total',
                                    color: '#6c757d',
                                    formatter: function (w) {
                                        return w.globals.seriesTotals.reduce((a, b) => { return a + b }, 0)
                                    }
                                }
                            }
                        }
                    }
                },
                legend: {
                    position: 'bottom',
                    itemMargin: { horizontal: 5, vertical: 5 }
                },
                dataLabels: { enabled: false }, // Limpiar visualmente el donut
                tooltip: {
                    y: { formatter: function (val) { return val + " Proyectos" } }
                }
            };

            var chartProy = new ApexCharts(document.querySelector("#chartEstados"), optionsProy);
            chartProy.render();
        });
    </script>

</asp:Content>