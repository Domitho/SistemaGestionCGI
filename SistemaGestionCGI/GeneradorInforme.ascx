<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneradorInforme.ascx.cs" Inherits="SistemaGestionCGI.GeneradorInforme" %>

<style>
    /* === 1. STEPPER (Mantenemos el que te gustó) === */
    .wizard-steps {
        display: flex; justify-content: center; margin-bottom: 25px; position: relative;
    }
    .wizard-steps::before {
        content: ""; position: absolute; top: 15px; left: 20%; right: 20%; height: 2px; background: #e9ecef; z-index: 0;
    }
    .step-item { position: relative; z-index: 1; text-align: center; width: 120px; }
    .step-circle {
        width: 32px; height: 32px; border-radius: 50%; background-color: #fff; border: 2px solid #e9ecef;
        color: #adb5bd; display: flex; align-items: center; justify-content: center; margin: 0 auto 5px auto;
        font-weight: bold; transition: all 0.3s;
    }
    .step-item.active .step-circle { background-color: #312783; border-color: #312783; color: #fff; transform: scale(1.1); }
    .step-item.completed .step-circle { background-color: #198754; border-color: #198754; color: #fff; }
    .step-label { font-size: 0.7rem; font-weight: 700; color: #adb5bd; text-transform: uppercase; }
    .step-item.active .step-label { color: #312783; }

    /* === 2. NUEVO DISEÑO: FORMULARIO TIPO TABLA (ROWS) === */
    .form-grid {
        border: 1px solid #dee2e6;
        border-radius: 8px;
        overflow: hidden; /* Para que los bordes internos no se salgan */
        background-color: #fff;
        margin-bottom: 20px;
        box-shadow: 0 2px 5px rgba(0,0,0,0.03);
    }

    .form-row {
        display: flex;
        border-bottom: 1px solid #dee2e6;
    }
    .form-row:last-child {
        border-bottom: none;
    }

    /* La Etiqueta (Celda Izquierda) */
    .form-label-cell {
        width: 25%; /* Ancho fijo para etiquetas */
        background-color: #f8f9fa; /* Gris muy claro */
        padding: 12px 15px;
        display: flex;
        align-items: center;
        font-size: 0.8rem;
        font-weight: 700;
        color: #495057;
        text-transform: uppercase;
        border-right: 1px solid #dee2e6;
    }
    
    /* El Icono en la etiqueta */
    .form-label-cell i {
        color: #312783;
        margin-right: 8px;
        font-size: 1rem;
        width: 20px; text-align: center;
    }

    /* El Input (Celda Derecha) */
    .form-input-cell {
        flex: 1; /* Toma el resto del espacio */
        padding: 0;
        background-color: #fff;
        display: flex;
        align-items: center;
    }

    /* El Textbox ASP dentro de la celda */
    .clean-input {
        width: 100%;
        border: none;
        padding: 10px 15px;
        font-size: 0.95rem;
        color: #212529;
        outline: none;
        font-family: 'Segoe UI', sans-serif;
        background: transparent;
    }
    .clean-input:focus {
        background-color: #fdfdfd; 
        box-shadow: inset 3px 0 0 #312783; /* Indicador azul a la izquierda al enfocar */
    }
    
    textarea.clean-input {
        resize: vertical;
        min-height: 50px;
    }

    /* Títulos de Sección */
    .section-header-row {
        background-color: #312783;
        color: white;
        padding: 8px 15px;
        font-weight: bold;
        font-size: 0.9rem;
        display: flex;
        align-items: center;
        text-transform: uppercase;
    }
</style>

<div class="modal fade" id="modalGeneradorInforme" tabindex="-1" aria-hidden="true" style="z-index: 1065;" ClientIDMode="Static">
    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content border-0 rounded-4" style="background-color: #f4f6f9;">
            
            <div class="modal-header border-0 bg-white pt-3 pb-0 rounded-top-4 flex-column">
                <div class="d-flex justify-content-between w-100 align-items-center mb-2 px-2">
                    <h5 class="fw-bold text-dark mb-0"><i class="fa-solid fa-file-contract me-2 text-primary"></i>Generador de Informe</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                
                <div class="wizard-steps w-100 mt-2">
                    <div class="step-item active" id="stInd1">
                        <div class="step-circle">1</div>
                        <div class="step-label">Actividades</div>
                    </div>
                    <div class="step-item" id="stInd2">
                        <div class="step-circle">2</div>
                        <div class="step-label">Equipo</div>
                    </div>
                    <div class="step-item" id="stInd3">
                        <div class="step-circle">3</div>
                        <div class="step-label">Presupuesto</div>
                    </div>
                </div>
            </div>

            <div class="modal-body px-4 pb-4 pt-0">
                <asp:HiddenField ID="hfIdEjecucionInterno" runat="server" />

                <div id="step1" class="step-section">
                    
                    <div class="form-grid">
                        <div class="section-header-row"><i class="fa-solid fa-circle-info me-2"></i> Datos Generales</div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-users-rectangle"></i> Grupo Inv.</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtGenGrupoInv" runat="server" CssClass="clean-input" placeholder="Nombre del Grupo de Investigación"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-regular fa-calendar"></i> Período</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtGenPeriodo" runat="server" CssClass="clean-input" placeholder="Ej: Octubre 2024 - Marzo 2025"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-chart-line"></i> % Avance</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtGenAvance" runat="server" CssClass="clean-input" TextMode="Number" placeholder="0"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row"><i class="fa-solid fa-table me-2"></i> Matriz de Actividades (Punto 1)</div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-bullseye"></i> 1. Objetivos</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenObjetivos" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="2"></asp:TextBox></div>
                        </div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-regular fa-clipboard"></i> 2. Planificadas</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenPlanificadas" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox></div>
                        </div>

                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-check-double"></i> 3. Ejecutadas</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenEjecutadas" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox></div>
                        </div>

                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-users"></i> 4. Participantes</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenParticipantes" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="2"></asp:TextBox></div>
                        </div>

                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-building-columns"></i> 5. Institución</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenInstitucion" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="2"></asp:TextBox></div>
                        </div>

                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-award"></i> 7. Resultados</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtGenResultados" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="2"></asp:TextBox></div>
                        </div>
                    </div>
                </div>

                <div id="step2" class="step-section d-none">
                    
                    <div class="form-grid">
                        <div class="section-header-row bg-primary"><i class="fa-solid fa-chalkboard-user me-2"></i> 2.1. Docentes Participantes</div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-signature"></i> Nombres</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtDocNombres" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Lista de nombres..."></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-id-card"></i> Cédulas</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtDocCedula" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-briefcase"></i> Carrera/Facultad</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtDocCarrera" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-regular fa-clock"></i> Horas / Obs</div>
                            <div class="form-input-cell">
                                <div class="row w-100 m-0">
                                    <div class="col-6 p-0 border-end"><asp:TextBox ID="txtDocHoras" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Horas"></asp:TextBox></div>
                                    <div class="col-6 p-0"><asp:TextBox ID="txtDocObs" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3" placeholder="Observaciones"></asp:TextBox></div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-grid">
                        <div class="section-header-row bg-success"><i class="fa-solid fa-graduation-cap me-2"></i> 2.2. Estudiantes Participantes</div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-signature"></i> Nombres</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtEstNombres" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-id-card"></i> Cédulas</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtEstCedula" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-school"></i> Carrera</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtEstCarrera" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-laptop-file"></i> Actividad/Tesis</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtEstActividad" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-regular fa-comment"></i> Observaciones</div>
                            <div class="form-input-cell">
                                <asp:TextBox ID="txtEstObs" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="step3" class="step-section d-none">
                    <div class="form-grid">
                        <div class="section-header-row bg-dark"><i class="fa-solid fa-sack-dollar me-2"></i> Gestión Financiera</div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-tags"></i> Rubro</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtPresupRubro" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="6"></asp:TextBox></div>
                        </div>
                        
                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-money-bill-wave"></i> Valores ($)</div>
                            <div class="form-input-cell">
                                <div class="d-flex w-100">
                                    <div class="flex-fill border-end p-2">
                                        <small class="text-muted d-block fw-bold text-uppercase">Asignado</small>
                                        <asp:TextBox ID="txtPresupAsignado" runat="server" CssClass="clean-input p-0" TextMode="MultiLine" Rows="6"></asp:TextBox>
                                    </div>
                                    <div class="flex-fill border-end p-2">
                                        <small class="text-muted d-block fw-bold text-uppercase">Ejecutado</small>
                                        <asp:TextBox ID="txtPresupEjecutado" runat="server" CssClass="clean-input p-0" TextMode="MultiLine" Rows="6"></asp:TextBox>
                                    </div>
                                    <div class="flex-fill p-2" style="max-width: 80px;">
                                        <small class="text-muted d-block fw-bold text-uppercase">%</small>
                                        <asp:TextBox ID="txtPresupPorcentaje" runat="server" CssClass="clean-input p-0" TextMode="MultiLine" Rows="6"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-label-cell"><i class="fa-solid fa-clipboard-check"></i> Observaciones</div>
                            <div class="form-input-cell"><asp:TextBox ID="txtPresupObservacion" runat="server" CssClass="clean-input" TextMode="MultiLine" Rows="5"></asp:TextBox></div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal-footer border-top-0 bg-white pb-4 px-4 justify-content-between rounded-bottom-4 shadow-lg-up">
                <button type="button" id="btnAtras" class="btn btn-light border px-4 fw-bold text-secondary d-none" onclick="navegar(-1)">
                    <i class="fa-solid fa-chevron-left me-2"></i> Atrás
                </button>
                
                <button type="button" id="btnSiguiente" class="btn btn-primary px-4 fw-bold shadow-sm" onclick="navegar(1)" style="background-color: #312783; border-color: #312783;">
                    Siguiente <i class="fa-solid fa-chevron-right ms-2"></i>
                </button>

                <div id="btnGroupFinal" class="d-none">
                    <span class="text-muted small me-2 fw-bold text-uppercase">Exportar:</span>
                    <div class="btn-group shadow">
                        <asp:LinkButton ID="btnGenerarWord" runat="server" CssClass="btn btn-primary fw-bold" OnClick="btnGenerar_Click" CommandArgument="WORD" style="background-color: #2b5797; border-color: #2b5797;">
                            <i class="fa-solid fa-file-word me-2"></i> WORD
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnGenerarPdf" runat="server" CssClass="btn btn-danger fw-bold" OnClick="btnGenerar_Click" CommandArgument="PDF" style="background-color: #b30b00; border-color: #b30b00;">
                            <i class="fa-solid fa-file-pdf me-2"></i> PDF
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    let pasoActual = 1;
    let idProyectoActual = 0; // Variable para diferenciar borradores
    const STORAGE_KEY_PREFIX = 'UTC_Draft_';

    // 1. INICIALIZACIÓN (Llamada desde C#)
    // Ahora recibe el ID para saber de quién es el borrador
    function resetWizard(idEjecucion) {
        idProyectoActual = idEjecucion; // Guardamos el ID actual
        pasoActual = 1;

        actualizarVista();

        // Iniciamos escuchas de eventos una sola vez
        iniciarAutoGuardado();

        // Recuperar datos ESPECÍFICOS de este proyecto
        setTimeout(restaurarBorrador, 200);
    }

    // 2. AUTO-GUARDADO (CON ID DE PROYECTO)
    function iniciarAutoGuardado() {
        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');

        inputs.forEach(input => {
            // Removemos listeners anteriores para no duplicar (seguridad por si se llama varias veces)
            input.removeEventListener('input', manejarInput);
            input.addEventListener('input', manejarInput);
        });
    }

    function manejarInput() {
        if (idProyectoActual === 0) return; // Seguridad

        // CLAVE ÚNICA: UTC_Draft_105_txtObjetivos
        const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + this.id;
        localStorage.setItem(key, this.value);
    }

    // 3. RESTAURAR (CON ID DE PROYECTO)
    function restaurarBorrador() {
        if (idProyectoActual === 0) return;

        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');

        inputs.forEach(input => {
            // Buscamos la clave específica de ESTE proyecto
            const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + input.id;
            const savedValue = localStorage.getItem(key);

            // Solo restauramos si hay algo guardado y el campo está vacío
            // (Si la BDD trajo datos, respetamos la BDD)
            if (savedValue !== null && input.value === "") {
                input.value = savedValue;
            }
        });
    }

    // 4. LIMPIAR (SOLO ESTE PROYECTO)
    function limpiarBorrador() {
        if (idProyectoActual === 0) return;

        const inputs = document.querySelectorAll('#modalGeneradorInforme input[type="text"], #modalGeneradorInforme textarea, #modalGeneradorInforme input[type="number"]');
        inputs.forEach(input => {
            const key = STORAGE_KEY_PREFIX + idProyectoActual + '_' + input.id;
            localStorage.removeItem(key);
        });
    }

    // 5. NAVEGACIÓN VISUAL (Igual que antes)
    function navegar(direccion) {
        pasoActual += direccion;
        if (pasoActual < 1) pasoActual = 1;
        if (pasoActual > 3) pasoActual = 3;
        actualizarVista();
    }

    function actualizarVista() {
        [1, 2, 3].forEach(p => document.getElementById('step' + p).classList.add('d-none'));
        document.getElementById('step' + pasoActual).classList.remove('d-none');

        const btnAtras = document.getElementById('btnAtras');
        const btnSig = document.getElementById('btnSiguiente');
        const btnFin = document.getElementById('btnGroupFinal');

        btnAtras.classList.toggle('d-none', pasoActual === 1);
        btnSig.classList.toggle('d-none', pasoActual === 3);
        btnFin.classList.toggle('d-none', pasoActual !== 3);

        for (let i = 1; i <= 3; i++) {
            let item = document.getElementById('stInd' + i);
            let circle = item.querySelector('.step-circle');
            item.classList.remove('active', 'completed');

            if (i < pasoActual) { item.classList.add('completed'); circle.innerHTML = '<i class="fa-solid fa-check"></i>'; }
            else if (i === pasoActual) { item.classList.add('active'); circle.innerHTML = i; }
            else { circle.innerHTML = i; }
        }
    }
</script>