/* ==========================================
   UTC SELECTOR DE TIEMPO (Steppers Logic)
   ========================================== */

// Variable global para controlar el modo (Nuevo o Edición)
let esEdicionDuracion = false;

/**
 * Abre el modal y carga los datos actuales desde los HiddenFields
 * @param {boolean} esEdit - True si estamos editando, False si es nuevo
 */
function AbrirModalDuracion(esEdit) {
    esEdicionDuracion = esEdit;
    let sufijo = esEdit ? "Edit" : ""; // Detecta si usar los IDs de Edición (hfAniosEdit)

    // 1. Cargar valores actuales. Si están vacíos o nulos, usa 0.
    // Usamos '|| 0' para seguridad.
    document.getElementById('tmpAnios').value = document.getElementById('hfAnios' + sufijo).value || 0;
    document.getElementById('tmpMeses').value = document.getElementById('hfMeses' + sufijo).value || 0;
    document.getElementById('tmpSemanas').value = document.getElementById('hfSemanas' + sufijo).value || 0;
    document.getElementById('tmpDias').value = document.getElementById('hfDias' + sufijo).value || 0;

    // Actualizamos el texto de vista previa antes de mostrar
    ActualizarPreview();

    // 2. Abrir Modal Bootstrap
    var el = document.getElementById('modalDuracion');
    // Usamos getOrCreateInstance para evitar duplicados en Bootstrap 5
    var modal = bootstrap.Modal.getOrCreateInstance(el);
    modal.show();
}

/**
 * Aumenta o disminuye el valor de un stepper
 * @param {string} tipo - 'anios', 'meses', 'semanas', 'dias'
 * @param {number} cantidad - +1 o -1
 */
function Step(tipo, cantidad) {
    // Identificar input temporal del modal
    let inputId = "";
    if (tipo === 'anios') inputId = 'tmpAnios';
    if (tipo === 'meses') inputId = 'tmpMeses';
    if (tipo === 'semanas') inputId = 'tmpSemanas';
    if (tipo === 'dias') inputId = 'tmpDias';

    let input = document.getElementById(inputId);
    if (!input) return; // Seguridad

    let valor = parseInt(input.value) + cantidad;

    // Validaciones lógicas y límites
    if (valor < 0) valor = 0;
    if (tipo === 'meses' && valor > 11) valor = 11; // Tope lógico
    if (tipo === 'semanas' && valor > 4) valor = 4;
    if (tipo === 'dias' && valor > 30) valor = 30;

    input.value = valor;
    ActualizarPreview();
}

/**
 * Genera el texto legible (Ej: "1 Año, 2 Meses") en el modal
 */
function ActualizarPreview() {
    let a = parseInt(document.getElementById('tmpAnios').value) || 0;
    let m = parseInt(document.getElementById('tmpMeses').value) || 0;
    let s = parseInt(document.getElementById('tmpSemanas').value) || 0;
    let d = parseInt(document.getElementById('tmpDias').value) || 0;

    let texto = [];
    if (a > 0) texto.push(a + (a === 1 ? " Año" : " Años"));
    if (m > 0) texto.push(m + (m === 1 ? " Mes" : " Meses"));
    if (s > 0) texto.push(s + (s === 1 ? " Semana" : " Semanas"));
    if (d > 0) texto.push(d + (d === 1 ? " Día" : " Días"));

    let resultado = texto.length > 0 ? texto.join(", ") : "Sin definir";

    // Actualizamos la etiqueta del Modal
    let lbl = document.getElementById('lblLivePreview');
    if (lbl) lbl.innerText = resultado;
}

/**
 * Transfiere los datos del Modal a los HiddenFields del formulario ASP
 */
function GuardarDuracion() {
    let sufijo = esEdicionDuracion ? "Edit" : "";

    // 1. Guardar valores del Modal en los HiddenFields estáticos
    document.getElementById('hfAnios' + sufijo).value = document.getElementById('tmpAnios').value;
    document.getElementById('hfMeses' + sufijo).value = document.getElementById('tmpMeses').value;
    document.getElementById('hfSemanas' + sufijo).value = document.getElementById('tmpSemanas').value;
    document.getElementById('hfDias' + sufijo).value = document.getElementById('tmpDias').value;

    // 2. Mostrar el texto bonito en el TextBox visible (Input Group)
    let display = document.getElementById('txtDuracionDisplay' + sufijo);
    let previewText = document.getElementById('lblLivePreview').innerText;

    if (display) display.value = previewText;

    // 3. Cerrar Modal
    var el = document.getElementById('modalDuracion');
    var modal = bootstrap.Modal.getInstance(el);
    modal.hide();
}