<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CertificadoGrupo.aspx.cs" Inherits="SistemaGestionCGI.CertificadoGrupo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reporte Institucional UTC - Historial del Integrante</title>
    <meta charset="utf-8" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 30px;
            background: #eef3f8;
            font-family: "Segoe UI", Arial, sans-serif;
            color: #1f2937;
        }

        .contenedor {
            max-width: 1100px;
            margin: 0 auto;
        }

        .documento {
            background: #ffffff;
            border-radius: 18px;
            overflow: hidden;
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.10);
            border: 1px solid #dbe4f0;
        }

        .encabezado {
            background: linear-gradient(135deg, #0b3d91 0%, #0d57c6 100%);
            color: white;
            padding: 28px 36px;
        }

        .encabezado-top {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 20px;
            flex-wrap: wrap;
        }

        .brand {
            display: flex;
            align-items: center;
            gap: 18px;
        }

        .logo-box {
            width: 82px;
            height: 82px;
            background: rgba(255,255,255,0.15);
            border: 1px solid rgba(255,255,255,0.25);
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            backdrop-filter: blur(4px);
        }

        .logo-box img {
            max-width: 70px;
            max-height: 70px;
            object-fit: contain;
        }

        .brand-text h1 {
            margin: 0;
            font-size: 28px;
            font-weight: 800;
            letter-spacing: .5px;
        }

        .brand-text h2 {
            margin: 6px 0 0 0;
            font-size: 15px;
            font-weight: 400;
            opacity: .95;
        }

        .encabezado-badge {
            background: rgba(255,255,255,0.16);
            border: 1px solid rgba(255,255,255,0.22);
            padding: 12px 18px;
            border-radius: 999px;
            font-size: 13px;
            font-weight: 600;
            white-space: nowrap;
        }

        .cuerpo {
            padding: 32px 36px 28px 36px;
        }

        .titulo-reporte {
            margin-bottom: 24px;
            padding-bottom: 14px;
            border-bottom: 3px solid #e8eef7;
        }

        .titulo-reporte h3 {
            margin: 0;
            color: #0b3d91;
            font-size: 24px;
            font-weight: 800;
        }

        .titulo-reporte p {
            margin: 8px 0 0 0;
            color: #6b7280;
            font-size: 14px;
        }

        .seccion {
            margin-bottom: 28px;
        }

        .seccion-titulo {
            margin: 0 0 16px 0;
            font-size: 18px;
            color: #0b3d91;
            font-weight: 800;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .seccion-titulo::before {
            content: "";
            width: 8px;
            height: 24px;
            border-radius: 999px;
            background: #0d57c6;
            display: inline-block;
        }

        .info-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 14px;
        }

        .info-card {
            background: #f8fbff;
            border: 1px solid #d9e7f7;
            border-radius: 16px;
            padding: 16px 18px;
            min-height: 90px;
        }

        .label {
            display: block;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: .5px;
            color: #6b7280;
            margin-bottom: 8px;
            font-weight: 700;
        }

        .valor {
            font-size: 16px;
            font-weight: 700;
            color: #111827;
            line-height: 1.4;
        }

        .timeline {
            position: relative;
            margin-top: 10px;
            padding-left: 34px;
        }

        .timeline::before {
            content: "";
            position: absolute;
            left: 10px;
            top: 0;
            bottom: 0;
            width: 4px;
            border-radius: 999px;
            background: linear-gradient(to bottom, #0d57c6, #8fb7f0);
        }

        .timeline-item {
            position: relative;
            margin-bottom: 18px;
            background: #ffffff;
            border: 1px solid #d9e7f7;
            border-radius: 18px;
            padding: 18px 20px;
            box-shadow: 0 4px 12px rgba(13, 87, 198, 0.06);
        }

        .timeline-item::before {
            content: "";
            position: absolute;
            left: -31px;
            top: 22px;
            width: 16px;
            height: 16px;
            background: #0d57c6;
            border: 4px solid #ffffff;
            border-radius: 50%;
            box-shadow: 0 0 0 2px #0d57c6;
        }

        .timeline-fecha {
            font-size: 13px;
            font-weight: 700;
            color: #0d57c6;
            margin-bottom: 8px;
        }

        .timeline-accion {
            font-size: 18px;
            font-weight: 800;
            color: #0b3d91;
            margin-bottom: 8px;
        }

        .timeline-detalle {
            color: #374151;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 4px;
        }

        .vacio {
            background: #fff8e8;
            border: 1px solid #f1d48a;
            color: #8a5a00;
            padding: 16px 18px;
            border-radius: 14px;
            font-weight: 600;
        }

        .pie {
            background: #f8fbff;
            border-top: 1px solid #dbe4f0;
            padding: 18px 36px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            flex-wrap: wrap;
        }

        .pie-texto {
            font-size: 13px;
            color: #6b7280;
        }

        .acciones {
            margin-top: 24px;
            text-align: center;
        }

        .btn-imprimir {
            border: none;
            background: linear-gradient(135deg, #0b3d91 0%, #0d57c6 100%);
            color: white;
            padding: 14px 28px;
            border-radius: 999px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 8px 18px rgba(13, 87, 198, 0.22);
        }

        .btn-imprimir:hover {
            opacity: .95;
        }

        @media (max-width: 900px) {
            .info-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 640px) {
            body {
                padding: 14px;
            }

            .encabezado,
            .cuerpo,
            .pie {
                padding-left: 18px;
                padding-right: 18px;
            }

            .info-grid {
                grid-template-columns: 1fr;
            }

            .brand-text h1 {
                font-size: 22px;
            }

            .brand-text h2 {
                font-size: 13px;
            }
        }

        @media print {
            body {
                background: #ffffff;
                padding: 0;
            }

            .acciones {
                display: none;
            }

            .documento {
                box-shadow: none;
                border: none;
                border-radius: 0;
                margin-top: 0;
            }

            .utc-header,
            .timeline::before,
            .timeline-item::before {
                -webkit-print-color-adjust: exact;
                print-color-adjust: exact;
            }
        }

        .utc-header {
            width: 100%;
            background: #332c7c;
            padding: 24px 0;
            text-align: center;
        }

        .utc-header img {
            height: 82px;
            max-width: 90%;
        }

        .barra-acciones {
            display: flex;
            justify-content: flex-start;
            margin-bottom: 20px;
        }

        .btn-regresar {
            border: 1px solid #0b3d91;
            background: #ffffff;
            color: #0b3d91;
            padding: 10px 18px;
            border-radius: 999px;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
        }

        .btn-regresar:hover {
            background: #f0f6ff;
        }

    </style>

    <script>
        function imprimirReporte() {
            window.print();
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            <div class="documento">

                <div class="utc-header">
                    <img src="https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png" alt="Universidad Técnica de Cotopaxi"/>
                </div>

                <div class="cuerpo">
                    <div class="titulo-reporte">
                        <h3>Reporte de Línea de Tiempo</h3>
                        <p>Detalle cronológico de movimientos y participación del integrante dentro del módulo de investigación.</p>
                    </div>

                    <div class="seccion">
                        <h4 class="seccion-titulo">Información General</h4>
                        <asp:Literal ID="litDatos" runat="server"></asp:Literal>
                    </div>

                    <div class="seccion">
                        <h4 class="seccion-titulo">Historial de Acciones</h4>
                        <asp:Literal ID="litTimeline" runat="server"></asp:Literal>
                    </div>

                    <div class="acciones">
                        <button type="button" class="btn-imprimir" onclick="imprimirReporte()">
                            Imprimir / Guardar como PDF
                        </button>
                    </div>
                </div>

                <div class="pie">
                    <div class="pie-texto">
                        Universidad Técnica de Cotopaxi · Sistema de Gestión CGI
                    </div>
                    <div class="pie-texto">
                        Documento generado automáticamente
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>

<script>
    function imprimirReporte() {
        window.print();
    }

    function cerrarVista() {
        if (window.parent && window.parent !== window) {
            var modalElement = window.parent.document.getElementById('modalReporte');
            if (modalElement) {
                var modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) {
                    modal.hide();
                }
            }
        } else {
            history.back();
        }
    }
</script>