<%@ Page Title="Dashboard Institucional" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="SistemaGestionCGI.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
    <link href="DesignersUTC/Styles/Dashboard.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="dashboardExportable">

        <!-- HEADER -->
        <div class="card shadow-utc border-0 rounded-4 p-3 mb-4">
            <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
                <div>
                    <h3 class="utc-title mb-0">
                        <i class="fa-solid fa-gauge-high me-2"></i> Dashboard Institucional
                    </h3>
                    <span class="text-muted small mt-2 mt-md-0 d-inline-block">
                        <i class="fa-regular fa-calendar me-1"></i>
                        <asp:Label ID="lblFechaActual" runat="server"></asp:Label>
                    </span>
                </div>

                <div class="d-flex flex-wrap gap-2 no-print">
                    <button type="button" class="btn btn-outline-secondary btn-sm rounded-pill" onclick="imprimirDashboard()">
                        <i class="fa-solid fa-print me-1"></i> Imprimir
                    </button>

                    <button type="button" class="btn btn-danger btn-sm rounded-pill" onclick="exportarDashboardPDF()">
                        <i class="fa-solid fa-file-pdf me-1"></i> PDF
                    </button>

                    <button type="button" class="btn btn-success btn-sm rounded-pill" onclick="exportarDashboardExcel()">
                        <i class="fa-solid fa-file-excel me-1"></i> Excel
                    </button>
                </div>
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
                            <asp:Label ID="lblIntegrantesCentros" runat="server" Text="0"></asp:Label>
                        </div>
                        <asp:Button ID="btnCentros" runat="server" CssClass="btn btn-primary btn-kpi" Text="Ver Centros"
                            OnClientClick="window.location='CentrosInvestigacion.aspx'; return false;" />
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
                            OnClientClick="window.location='ConvocatoriaGruInvestigacion.aspx'; return false;" />
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
                            <div class="kpi-label mt-1">Grupos de Investigacion</div>
                        </div>
                        <div class="badge-subtotal bg-warning bg-opacity-10 text-dark mt-3 border border-warning border-opacity-25">
                            <i class="fa-solid fa-users me-2"></i>
                            <asp:Label ID="lblIntegrantesGrupos" runat="server" Text="0"></asp:Label>
                        </div>
                        <asp:Button ID="btnGrupos" runat="server" CssClass="btn btn-warning btn-kpi" Text="Ver Grupos"
                            OnClientClick="window.location='GruposInvestigacion.aspx'; return false;" />
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
                            OnClientClick="window.location='CategorizacionDocentes.aspx'; return false;" />
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
        <div class="modal fade" id="modalDetalleDocentes" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content modal-utc">
                    <div class="modal-body p-0" id="modalDetalleBody"></div>
                    <div class="modal-footer border-0 px-4 pb-4 pt-0 bg-white">
                        <button type="button" class="btn btn-secondary rounded-pill px-4" data-bs-dismiss="modal">
                            <i class="fa-solid fa-xmark me-1"></i> Cerrar
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <!-- MODAL PROYECTOS -->
        <div class="modal fade" id="modalDetalleProyectos" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content modal-utc">
                    <div class="modal-body p-0" id="modalProyectosBody"></div>
                    <div class="modal-footer border-0 px-4 pb-4 pt-0 bg-white">
                        <button type="button" class="btn btn-secondary rounded-pill px-4" data-bs-dismiss="modal">
                            <i class="fa-solid fa-xmark me-1"></i> Cerrar
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Exportación Dashboard -->
    <script src="https://cdn.jsdelivr.net/npm/xlsx/dist/xlsx.full.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <!-- SCRIPT -->
    <script>
        let rawDocentes = [];
        let docentesDetalle = [];
        let rawProyectos = [];
        let proyectosDetalle = [];

        $(document).ready(function () {
            rawDocentes = <%= JsonDocentes %>;
            docentesDetalle = <%= JsonDocentesDetalle %>;
            rawProyectos = <%= JsonProyectos %>;
            proyectosDetalle = <%= JsonProyectosDetalle %>;

            function normalizarEstado(texto) {
                if (!texto) return '';
                return texto
                    .toUpperCase()
                    .normalize('NFD')
                    .replace(/[\u0300-\u036f]/g, '') 
                    .trim();
            }

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

                            var html = `
                                <div class="modal-utc-header">
                                    <div class="modal-utc-header-content">
                                        <div>
                                            <h5 class="modal-utc-title mb-2">
                                                <i class="fa-solid fa-users-viewfinder me-2"></i>Detalle de Docentes
                                            </h5>
                                            <div class="d-flex flex-wrap gap-2">
                                                <span class="badge rounded-pill modal-badge-info">
                                                    <i class="fa-solid fa-layer-group me-1"></i> Categoría: ${categoria}
                                                </span>
                                                <span class="badge rounded-pill modal-badge-total">
                                                    <i class="fa-solid fa-user-group me-1"></i> Total: ${detalleFiltrado.length}
                                                </span>
                                            </div>
                                        </div>
                                        <button type="button" class="btn-close btn-close-white modal-utc-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                                    </div>
                                </div>

                                <div class="modal-utc-content">
                                    <div class="modal-table-shell">
                                        <div class="table-responsive">
                                            <table id="tablaDetalleDocentes" class="table table-striped table-hover table-bordered align-middle w-100 modal-utc-table">
                                                <thead>
                                                    <tr>
                                                        <th>Cédula</th>
                                                        <th>Nombres</th>
                                                        <th>Apellidos</th>
                                                        <th>Facultad</th>
                                                        <th>Carrera</th>
                                                        <th>Activo</th>
                                                        <th>Correo</th>
                                                    </tr>
                                                </thead>
                                                <tbody>`;
                            detalleFiltrado.forEach(function (doc) {
                                html += `<tr>
                                            <td>${doc.Cedula || '-'}</td>
                                            <td>${doc.Nombres || '-'}</td>
                                            <td>${doc.Apellidos || '-'}</td>
                                            <td>${doc.Facultad || '-'}</td>
                                            <td>${doc.Carrera || '-'}</td>
                                            <td>${doc.Activo ? "Sí" : "No"}</td>
                                            <td>${doc.Correo || '-'}</td>
                                         </tr>`;
                            });
                            html += `</tbody></table></div></div></div>`;

                            document.getElementById("modalDetalleBody").innerHTML = html;
                            new bootstrap.Modal(document.getElementById('modalDetalleDocentes')).show();

                            setTimeout(function () {
                                if ($.fn.DataTable.isDataTable('#tablaDetalleDocentes')) {
                                    $('#tablaDetalleDocentes').DataTable().destroy();
                                }

                                $('#tablaDetalleDocentes').DataTable({
                                    responsive: true,
                                    pageLength: 5,
                                    dom: '<"modal-dt-toolbar d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3"Bf>rt<"d-flex justify-content-between align-items-center flex-wrap mt-3"lip>',
                                    buttons: [
                                        {
                                            extend: 'excelHtml5',
                                            text: '<i class="fa-solid fa-file-excel"></i><span>Excel</span>',
                                            className: 'btn btn-success btn-sm rounded-pill',
                                            title: 'Detalle_Docentes_' + categoria,
                                            exportOptions: { columns: ':visible' }
                                        },
                                        {
                                            extend: 'pdfHtml5',
                                            text: '<i class="fa-solid fa-file-pdf"></i><span>PDF</span>',
                                            className: 'btn btn-danger btn-sm rounded-pill',
                                            title: 'Detalle_Docentes_' + categoria,
                                            orientation: 'landscape',
                                            pageSize: 'A4',
                                            exportOptions: { columns: ':visible' }
                                        },
                                        {
                                            extend: 'print',
                                            text: '<i class="fa-solid fa-print"></i><span>Imprimir</span>',
                                            className: 'btn btn-warning btn-sm rounded-pill',
                                            title: 'Detalle de Docentes - ' + categoria,
                                            exportOptions: { columns: ':visible' }
                                        }
                                    ],
                                    language: {
                                        url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
                                    }
                                });
                            }, 150);
                        }
                    }
                },
                colors: ['#312783'],
                plotOptions: { bar: { borderRadius: 4, horizontal: true, barHeight: '60%' } },
                xaxis: { categories: rawDocentes.map(x => x.Label) },
                dataLabels: { enabled: true, formatter: val => val + " Docentes" },
                grid: { borderColor: '#f1f1f1' }
            }).render();

            var colorMap = {
                'EN EJECUCION': '#1b9e4b',
                'EN REVISION': '#ffc107',
                'FINALIZADO': '#312783'
            };

            new ApexCharts(document.querySelector("#chartEstados"), {
                series: rawProyectos.map(x => x.Value),
                labels: rawProyectos.map(x => x.Label),
                chart: {
                    type: 'donut',
                    height: 350,
                    toolbar: { show: false },
                    events: {
                        dataPointSelection: function (event, chartContext, config) {

                            var estado = rawProyectos[config.dataPointIndex].Label;
                            var estadoNormalizado = normalizarEstado(estado);

                            var proyectosFiltrados = proyectosDetalle.filter(function (p) {
                                return normalizarEstado(p.Estado) === estadoNormalizado;
                            });

                            var html = `
                                <div class="modal-utc-header">
                                    <div class="modal-utc-header-content">
                                        <div>
                                            <h5 class="modal-utc-title mb-2">
                                                <i class="fa-solid fa-diagram-project me-2"></i>
                                                Detalle de Proyectos
                                            </h5>

                                            <div class="d-flex flex-wrap gap-2 justify-content-center">
                                                <span class="badge rounded-pill modal-badge-info">
                                                    <i class="fa-solid fa-bars-progress me-1"></i>
                                                    Estado: ${estado}
                                                </span>

                                                <span class="badge rounded-pill modal-badge-total">
                                                    <i class="fa-solid fa-folder-open me-1"></i>
                                                    Total: ${proyectosFiltrados.length}
                                                </span>
                                            </div>
                                        </div>

                                        <button type="button"
                                                class="btn-close btn-close-white modal-utc-close"
                                                data-bs-dismiss="modal"
                                                aria-label="Cerrar"></button>
                                    </div>
                                </div>

                                <div class="modal-utc-content">
                                    <div class="modal-table-shell">

                                        <div class="table-responsive">

                                            <table id="tablaDetalleProyectos"
                                                   class="table table-striped table-hover table-bordered align-middle w-100 modal-utc-table">

                                                <thead>
                                                    <tr>
                                                        <th>Código</th>
                                                        <th>Proyecto</th>
                                                        <th>Coordinador</th>
                                                        <th>Periodo</th>
                                                        <th>Fecha Inicio</th>
                                                    </tr>
                                                </thead>

                                                <tbody>`;

                            if (proyectosFiltrados.length > 0) {

                                proyectosFiltrados.forEach(function (proy) {

                                    html += `
                                    <tr>
                                        <td>${proy.Codigo || '-'}</td>
                                        <td>${proy.NombreProyecto || '-'}</td>
                                        <td>${proy.Coordinador || '-'}</td>
                                        <td>${proy.Periodo || '-'}</td>
                                        <td>${proy.FechaInicio || '-'}</td>
                                    </tr>`;
                                });

                            } else {

                                html += `
                                <tr>
                                    <td colspan="5" class="text-center text-muted">
                                        No existen registros para este estado
                                    </td>
                                </tr>`;
                            }

                            html += `</tbody></table></div></div></div>`;

                            document.getElementById("modalProyectosBody").innerHTML = html;

                            const modal = new bootstrap.Modal(
                                document.getElementById('modalDetalleProyectos')
                            );

                            modal.show();

                            setTimeout(function () {

                                if ($.fn.DataTable.isDataTable('#tablaDetalleProyectos')) {
                                    $('#tablaDetalleProyectos').DataTable().destroy();
                                }

                                $('#tablaDetalleProyectos').DataTable({

                                    responsive: true,
                                    pageLength: 5,

                                    dom: '<"modal-dt-toolbar d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3"Bf>rt<"d-flex justify-content-between align-items-center flex-wrap mt-3"lip>',

                                    buttons: [
                                        {
                                            extend: 'excelHtml5',
                                            text: '<i class="fa-solid fa-file-excel"></i><span>Excel</span>',
                                            className: 'btn btn-success btn-sm rounded-pill',
                                            title: 'Detalle_Proyectos_' + estado,
                                            exportOptions: { columns: ':visible' }
                                        },
                                        {
                                            extend: 'pdfHtml5',
                                            text: '<i class="fa-solid fa-file-pdf"></i><span>PDF</span>',
                                            className: 'btn btn-danger btn-sm rounded-pill',
                                            title: 'Detalle_Proyectos_' + estado,
                                            orientation: 'landscape',
                                            pageSize: 'A4',
                                            exportOptions: { columns: ':visible' }
                                        },
                                        {
                                            extend: 'print',
                                            text: '<i class="fa-solid fa-print"></i><span>Imprimir</span>',
                                            className: 'btn btn-warning btn-sm rounded-pill',
                                            title: 'Detalle de Proyectos - ' + estado,
                                            exportOptions: { columns: ':visible' }
                                        }
                                    ],

                                    language: {
                                        url: 'https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json'
                                    }

                                });

                            }, 150);
                        }
                    }
                },

                colors: rawProyectos.map(x => colorMap[x.Label] || '#6c757d'),

                plotOptions: {
                    pie: {
                        donut: {
                            size: '65%'
                        }
                    }
                }

            }).render();
        });
    </script>

    <script>
        function imprimirDashboard() {
            window.print();
        }

        function exportarDashboardExcel() {
            var wb = XLSX.utils.book_new();

            var resumen = [
                ["Indicador", "Valor"],
                ["Centros de Investigación", $("#<%= lblCentros.ClientID %>").text().trim()],
                ["Integrantes de Centros", $("#<%= lblIntegrantesCentros.ClientID %>").text().trim()],
                ["Convocatorias", $("#<%= lblConvocatorias.ClientID %>").text().trim()],
                ["Grupos de Investigación", $("#<%= lblGrupos.ClientID %>").text().trim()],
                ["Integrantes de Grupos", $("#<%= lblIntegrantesGrupos.ClientID %>").text().trim()],
                ["Total Docentes", $("#<%= lblTotalDocentes.ClientID %>").text().trim()]
            ];

            var wsResumen = XLSX.utils.aoa_to_sheet(resumen);
            XLSX.utils.book_append_sheet(wb, wsResumen, "Resumen");

            var wsDocentes = XLSX.utils.json_to_sheet(rawDocentes.map(function (x) {
                return {
                    Categoria: x.Label,
                    TotalDocentes: x.Value
                };
            }));
            XLSX.utils.book_append_sheet(wb, wsDocentes, "DocentesCategoria");

            var wsDetalleDocentes = XLSX.utils.json_to_sheet(docentesDetalle.map(function (d) {
                return {
                    Cedula: d.Cedula || '',
                    Nombres: d.Nombres || '',
                    Apellidos: d.Apellidos || '',
                    Facultad: d.Facultad || '',
                    Carrera: d.Carrera || '',
                    Activo: d.Activo ? 'Sí' : 'No',
                    Correo: d.Correo || ''
                };
            }));
            XLSX.utils.book_append_sheet(wb, wsDetalleDocentes, "DetalleDocentes");

            var wsProyectos = XLSX.utils.json_to_sheet(rawProyectos.map(function (x) {
                return {
                    Estado: x.Label,
                    Total: x.Value
                };
            }));
            XLSX.utils.book_append_sheet(wb, wsProyectos, "ProyectosEstado");

            var wsDetalleProyectos = XLSX.utils.json_to_sheet(proyectosDetalle.map(function (p) {
                return {
                    Codigo: p.Codigo || '',
                    Proyecto: p.NombreProyecto || '',
                    Coordinador: p.Coordinador || '',
                    Periodo: p.Periodo || '',
                    FechaInicio: p.FechaInicio || '',
                    Estado: p.Estado || ''
                };
            }));
            XLSX.utils.book_append_sheet(wb, wsDetalleProyectos, "DetalleProyectos");

            XLSX.writeFile(wb, "Dashboard_Institucional.xlsx");
        }


        async function exportarDashboardPDF() {
            const elemento = document.getElementById('dashboardExportable');
            const { jsPDF } = window.jspdf;

            const canvas = await html2canvas(elemento, {
                scale: 2,
                useCORS: true,
                scrollY: -window.scrollY
            });

            const imgData = canvas.toDataURL('image/png');
            const pdf = new jsPDF('p', 'mm', 'a4');

            const pageWidth = 210;
            const pageHeight = 297;
            const margin = 10;
            const usableWidth = pageWidth - (margin * 2);
            const imgHeight = (canvas.height * usableWidth) / canvas.width;

            let heightLeft = imgHeight;
            let position = margin;

            pdf.addImage(imgData, 'PNG', margin, position, usableWidth, imgHeight);
            heightLeft -= (pageHeight - margin * 2);

            while (heightLeft > 0) {
                position = heightLeft - imgHeight + margin;
                pdf.addPage();
                pdf.addImage(imgData, 'PNG', margin, position, usableWidth, imgHeight);
                heightLeft -= (pageHeight - margin * 2);
            }

            pdf.save('Dashboard_Institucional.pdf');
        }
    </script>

</asp:Content>