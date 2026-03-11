window.UTC_DataTable = (function () {

    function buildConfig(options) {
        options = options || {};

        var exportTitle = options.exportTitle || 'Exportacion';
        var pageLength = options.pageLength || 10;
        var orientation = options.orientation || 'landscape';
        var pageSize = options.pageSize || 'A4';

        var exportColumns = ':visible';

        if (options.excludeLastColumn === true) {
            exportColumns = ':not(:last-child)';
        }

        if (Array.isArray(options.exportColumns)) {
            exportColumns = options.exportColumns;
        }

        return {
            responsive: true,
            autoWidth: false,
            ordering: options.ordering !== false,
            pageLength: pageLength,
            order: options.order || [],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.13.8/i18n/es-ES.json"
            },
            dom:
                "<'row align-items-center mb-3'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4 text-center'B><'col-sm-12 col-md-4 text-end'f>>" +
                "<'row'<'col-12'tr>>" +
                "<'row mt-3 align-items-center'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: '<i class="fa-solid fa-file-excel"></i><span>Excel</span>',
                    className: 'dt-btn-export',
                    title: exportTitle,
                    filename: exportTitle,
                    titleAttr: 'Exportar a Excel',
                    exportOptions: {
                        columns: exportColumns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    text: '<i class="fa-solid fa-file-pdf"></i><span>PDF</span>',
                    className: 'dt-btn-export',
                    title: exportTitle,
                    filename: exportTitle,
                    titleAttr: 'Exportar a PDF',
                    orientation: orientation,
                    pageSize: pageSize,
                    exportOptions: {
                        columns: exportColumns
                    }
                },
                {
                    extend: 'print',
                    text: '<i class="fa-solid fa-print"></i><span>Imprimir</span>',
                    className: 'dt-btn-export',
                    title: exportTitle,
                    titleAttr: 'Imprimir tabla',
                    exportOptions: {
                        columns: exportColumns
                    }
                }
            ]
        };
    }

    function init(selector, options) {
        var $table = $(selector);

        if (!$table.length) return null;

        if ($.fn.DataTable.isDataTable(selector)) {
            $table.DataTable().destroy();
        }

        return $table.DataTable(buildConfig(options));
    }

    return {
        init: init
    };

})();