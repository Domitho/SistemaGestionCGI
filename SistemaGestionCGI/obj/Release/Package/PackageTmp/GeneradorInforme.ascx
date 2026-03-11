<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneradorInforme.ascx.cs" Inherits="SistemaGestionCGI.GeneradorInforme" %>

<style>
    .wizard-steps { display: flex; justify-content: center; margin-bottom: 25px; position: relative; }
    .wizard-steps::before { content: ""; position: absolute; top: 15px; left: 10%; right: 10%; height: 2px; background: #e9ecef; z-index: 0; }
    
    .step-item { position: relative; z-index: 1; text-align: center; width: 18%; } 
    .step-circle {
        width: 32px; height: 32px; border-radius: 50%; background-color: #fff; border: 2px solid #e9ecef;
        color: #adb5bd; display: flex; align-items: center; justify-content: center; margin: 0 auto 5px auto;
        font-weight: bold; transition: all 0.3s; font-size: 12px;
    }
    .step-item.active .step-circle { background-color: #312783; border-color: #312783; color: #fff; transform: scale(1.1); }
    .step-item.completed .step-circle { background-color: #198754; border-color: #198754; color: #fff; }
    .step-label { font-size: 0.65rem; font-weight: 700; color: #adb5bd; text-transform: uppercase; }
    .step-item.active .step-label { color: #312783; }

    .form-grid { border: 1px solid #dee2e6; border-radius: 8px; overflow: hidden; background-color: #fff; margin-bottom: 20px; box-shadow: 0 2px 5px rgba(0,0,0,0.03); }
    .form-row { display: flex; border-bottom: 1px solid #dee2e6; }
    .form-row:last-child { border-bottom: none; }
    .form-label-cell { width: 30%; background-color: #f8f9fa; padding: 10px 15px; font-size: 0.75rem; font-weight: 700; color: #495057; text-transform: uppercase; border-right: 1px solid #dee2e6; display: flex; align-items: center; }
    .form-label-cell i { color: #312783; margin-right: 8px; width: 20px; text-align: center; }
    .form-input-cell { flex: 1; padding: 0; background-color: #fff; }
    .clean-input { width: 100%; border: none; padding: 8px 12px; font-size: 0.9rem; outline: none; font-family: 'Segoe UI', sans-serif; }
    .clean-input:focus { background-color: #fcfcfc; box-shadow: inset 3px 0 0 #312783; }
    .section-header-row { background-color: #312783; color: white; padding: 6px 15px; font-weight: bold; font-size: 0.85rem; text-transform: uppercase; }
    textarea.clean-input { min-height: 60px; resize: vertical; }
</style>

<div class="modal fade" id="modalGeneradorInforme" tabindex="-1" aria-hidden="true" style="z-index: 1065;" ClientIDMode="Static">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content border-0 rounded-4 bg-light">
            
            <div class="modal-header border-0 bg-white pt-3 pb-0 rounded-top-4 flex-column">
                <div class="d-flex justify-content-between w-100 align-items-center mb-2 px-2">
                    <h5 class="fw-bold text-dark mb-0"><i class="fa-solid fa-file-contract me-2 text-primary"></i>Generador de Informe Final</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                
                <div class="wizard-steps w-100 mt-2">
                    <div class="step-item active" id="stInd1"><div class="step-circle">1</div><div class="step-label">Datos</div></div>
                    <div class="step-item" id="stInd2"><div class="step-circle">2</div><div class="step-label">Equipo</div></div>
                    <div class="step-item" id="stInd3"><div class="step-circle">3</div><div class="step-label">Resultados</div></div>
                    <div class="step-item" id="stInd4"><div class="step-circle">4</div><div class="step-label">Producción</div></div>
                    <div class="step-item" id="stInd5"><div class="step-circle">5</div><div class="step-label">Cierre</div></div>
                </div>
            </div>

            <div class="modal-body px-4 pb-4 pt-0">
                <asp:HiddenField ID="hfIdEjecucionInterno" runat="server" />

                <div id="step1" class="step-section">
                    <div class="form-grid">
                        <div class="section-header-row">1. Datos Informativos</div>
                        <div class="form-row"><div class="form-label-cell">Grupo Inv.</div><div class="form-input-cell"><asp:TextBox ID="txtGenGrupoInv" runat="server" CssClass="clean-input"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Período</div><div class="form-input-cell"><asp:TextBox ID="txtGenPeriodo" runat="server" CssClass="clean-input" placeholder="OCTUBRE 2025 - MARZO 2026"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">% Cumplimiento</div><div class="form-input-cell"><asp:TextBox ID="txtGenAvance" runat="server" CssClass="clean-input" TextMode="Number"></asp:TextBox></div></div>
                    </div>
                    <div class="form-grid">
                        <div class="section-header-row">Matriz de Actividades (Punto 1)</div>
                        <div class="form-row"><div class="form-label-cell">Componente / Objetivos</div><div class="form-input-cell"><asp:TextBox ID="txtGenObjetivos" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Act. Planificadas</div><div class="form-input-cell"><asp:TextBox ID="txtGenPlanificadas" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Act. Ejecutadas</div><div class="form-input-cell"><asp:TextBox ID="txtGenEjecutadas" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Docentes / Investigadores</div><div class="form-input-cell"><asp:TextBox ID="txtGenParticipantes" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Institución / Carrera</div><div class="form-input-cell"><asp:TextBox ID="txtGenInstitucion" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Resultados Alcanzados</div><div class="form-input-cell"><asp:TextBox ID="txtGenResultados" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                    </div>
                </div>

                <div id="step2" class="step-section d-none">
                    <div class="form-grid">
                        <div class="section-header-row">2.1 Docentes Participantes</div>
                        <div class="form-row"><div class="form-label-cell">Cédula (Buscar)</div><div class="form-input-cell"><asp:TextBox ID="txtDocCedula" runat="server" CssClass="clean-input" AutoPostBack="true" OnTextChanged="txtDocCedula_TextChanged" placeholder="Ingrese CI y Enter..."></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Nombres</div><div class="form-input-cell"><asp:TextBox ID="txtDocNombres" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Carrera / Facultad</div><div class="form-input-cell"><asp:TextBox ID="txtDocCarrera" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Horas Participación</div><div class="form-input-cell"><asp:TextBox ID="txtDocHoras" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Obs</div><div class="form-input-cell"><asp:TextBox ID="txtDocObs" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                    </div>
                    <div class="form-grid">
                        <div class="section-header-row">2.2 Estudiantes Participantes</div>
                        <div class="form-row"><div class="form-label-cell">Nombres</div><div class="form-input-cell"><asp:TextBox ID="txtEstNombres" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Cédula</div><div class="form-input-cell"><asp:TextBox ID="txtEstCedula" runat="server" CssClass="clean-input"></asp:TextBox></div></div>
        
                        <div class="form-row"><div class="form-label-cell">Carrera</div><div class="form-input-cell"><asp:TextBox ID="txtEstCarrera" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Actividad Realizada</div><div class="form-input-cell"><asp:TextBox ID="txtEstActividad" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">Obs</div><div class="form-input-cell"><asp:TextBox ID="txtEstObs" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                    </div>
                </div>

                <div id="step3" class="step-section d-none">
                    <div class="alert alert-info py-2 small mb-2"><i class="fa-solid fa-info-circle"></i> En estas secciones, separe los registros con "Enter".</div>
                    
                    <div class="form-grid">
                        <div class="section-header-row">3. Titulaciones Derivadas</div>
                        <div class="form-row"><div class="form-label-cell">Detalle Estudiantes</div><div class="form-input-cell"><asp:TextBox ID="txtTitDetalle" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="4" placeholder="Nombre, Cédula, Título, Periodo..."></asp:TextBox></div></div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row">4. Integración Curricular / Vinculación</div>
                        <div class="form-row"><div class="form-label-cell">Detalle Actividades</div><div class="form-input-cell"><asp:TextBox ID="txtVinculacion" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="4" placeholder="Actividad, Sector, Beneficiarios, Responsable..."></asp:TextBox></div></div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row">5. Innovación / Propiedad Intelectual</div>
                        <div class="form-row"><div class="form-label-cell">Registros SENADI</div><div class="form-input-cell"><asp:TextBox ID="txtInnovacion" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Actividad, Participantes, Cédula..."></asp:TextBox></div></div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row">6. Convenios Interinstitucionales</div>
                        <div class="form-row"><div class="form-label-cell">Convenios Suscritos</div><div class="form-input-cell"><asp:TextBox ID="txtConvenios" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Entidad, Vigencia, Fecha, Responsable..."></asp:TextBox></div></div>
                    </div>
                </div>

                <div id="step4" class="step-section d-none">
                     <div class="form-grid">
                        <div class="section-header-row">7.1 Artículos Científicos</div>
                        <div class="form-row"><div class="form-input-cell"><asp:TextBox ID="txtProdArticulos" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="4" placeholder="Año, Base Datos, Título, Autores, Revista, ISSN, DOI..."></asp:TextBox></div></div>
                    </div>
                    <div class="form-grid">
                        <div class="section-header-row">7.2 Libros</div>
                        <div class="form-row"><div class="form-input-cell"><asp:TextBox ID="txtProdLibros" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Año, ISBN, Título, Estado..."></asp:TextBox></div></div>
                    </div>
                    <div class="form-grid">
                        <div class="section-header-row">7.3 Capítulos de Libros</div>
                        <div class="form-row"><div class="form-input-cell"><asp:TextBox ID="txtProdCapitulos" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Año, ISBN, Libro, Capítulo..."></asp:TextBox></div></div>
                    </div>
                    <div class="form-grid">
                        <div class="section-header-row">7.4 Ponencias</div>
                        <div class="form-row"><div class="form-input-cell"><asp:TextBox ID="txtProdPonencias" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Evento, Lugar, Fecha, Título, Estado..."></asp:TextBox></div></div>
                    </div>
                </div>

                <div id="step5" class="step-section d-none">
                    <div class="form-grid">
                        <div class="section-header-row">8. Ejecución Presupuestaria</div>
                        <div class="form-row"><div class="form-label-cell">Rubro</div><div class="form-input-cell"><asp:TextBox ID="txtPresupRubro" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                        <div class="form-row">
                             <div class="form-label-cell">Valores ($) y %</div>
                             <div class="form-input-cell d-flex">
                                 <asp:TextBox ID="txtPresupAsignado" runat="server" CssClass="clean-input border-end" placeholder="Asignado ($)"></asp:TextBox>
                                 <asp:TextBox ID="txtPresupEjecutado" runat="server" CssClass="clean-input border-end" placeholder="Ejecutado ($)"></asp:TextBox>
                                 <asp:TextBox ID="txtPresupPorcentaje" runat="server" CssClass="clean-input" placeholder="%"></asp:TextBox>
                             </div>
                        </div>
                        <div class="form-row"><div class="form-label-cell">Observaciones</div><div class="form-input-cell"><asp:TextBox ID="txtPresupObservacion" runat="server" CssClass="clean-input" TextMode="MultiLine"></asp:TextBox></div></div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row">9. Conclusiones y Recomendaciones</div>
                        <div class="form-row"><div class="form-label-cell">9.1 Conclusiones</div><div class="form-input-cell"><asp:TextBox ID="txtConclusiones" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="4"></asp:TextBox></div></div>
                        <div class="form-row"><div class="form-label-cell">9.2 Recomendaciones</div><div class="form-input-cell"><asp:TextBox ID="txtRecomendaciones" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="4"></asp:TextBox></div></div>
                    </div>
                </div>
            </div>

            <div class="modal-footer border-top-0 bg-white pb-4 px-4 justify-content-between rounded-bottom-4 shadow-lg-up">
                <button type="button" id="btnAtras" class="btn btn-light border px-4 fw-bold text-secondary d-none" onclick="navegar(-1)">Atrás</button>
                <button type="button" id="btnSiguiente" class="btn btn-primary px-4 fw-bold shadow-sm" onclick="navegar(1)" style="background-color: #312783;">Siguiente</button>

                <div id="btnGroupFinal" class="d-none">
                    <span class="text-muted small me-2 fw-bold">FINALIZAR:</span>
                    <div class="btn-group shadow">
                        <asp:LinkButton ID="btnGenerarWord" runat="server" CssClass="btn btn-primary" OnClick="btnGenerar_Click" CommandArgument="WORD"><i class="fa-solid fa-file-word me-1"></i> DOC</asp:LinkButton>
                        <asp:LinkButton ID="btnGenerarPdf" runat="server" CssClass="btn btn-danger" OnClick="btnGenerar_Click" CommandArgument="PDF"><i class="fa-solid fa-file-pdf me-1"></i> PDF</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    let pasoActual = 1;
    let idProyectoActual = 0;
    const STORAGE_KEY_PREFIX = 'UTC_Draft_';

    function resetWizard(idEjecucion) {
        idProyectoActual = idEjecucion;
        pasoActual = 1;

        actualizarVista();
        iniciarAutoGuardado();
        setTimeout(restaurarBorrador, 200);
    }

    function navegar(dir) {
        pasoActual += dir;

        if (pasoActual < 1) pasoActual = 1;
        if (pasoActual > 5) pasoActual = 5;

        actualizarVista();
    }

    function actualizarVista() {
        for (let i = 1; i <= 5; i++) {
            let step = document.getElementById('step' + i);
            if (step) step.classList.add('d-none');
        }

        document.getElementById('step' + pasoActual).classList.remove('d-none');

        const btnAtras = document.getElementById('btnAtras');
        const btnSig = document.getElementById('btnSiguiente');
        const btnFin = document.getElementById('btnGroupFinal');

        btnAtras.classList.toggle('d-none', pasoActual === 1);

        btnSig.classList.toggle('d-none', pasoActual === 5);

        btnFin.classList.toggle('d-none', pasoActual !== 5);

        for (let i = 1; i <= 5; i++) {
            let item = document.getElementById('stInd' + i);
            if (!item) continue;

            let circle = item.querySelector('.step-circle');
            item.classList.remove('active', 'completed');

            if (i < pasoActual) {
                item.classList.add('completed');
                circle.innerHTML = '<i class="fa-solid fa-check"></i>'; // Check
            }
            else if (i === pasoActual) {
                item.classList.add('active');
                circle.innerHTML = i;
            }
            else {
                circle.innerHTML = i;
            }
        }
    }

    // 4. LÓGICA DE AUTO-GUARDADO (Se mantiene igual)
    function iniciarAutoGuardado() {
        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');
        inputs.forEach(input => {
            input.removeEventListener('input', manejarInput);
            input.addEventListener('input', manejarInput);
        });
    }

    function manejarInput() {
        if (idProyectoActual === 0) return;
        const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + this.id;
        localStorage.setItem(key, this.value);
    }

    function restaurarBorrador() {
        if (idProyectoActual === 0) return;
        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');
        inputs.forEach(input => {
            const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + input.id;
            const savedValue = localStorage.getItem(key);
            if (savedValue !== null && input.value === "") {
                input.value = savedValue;
            }
        });
    }

    function limpiarBorrador() {
        if (idProyectoActual === 0) return;
        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');
        inputs.forEach(input => {
            const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + input.id;
            localStorage.removeItem(key);
        });
    }
</script>