
let esEdicionDuracion = false;

/**
 * 
 * @param {boolean} esEdit 
 */
function AbrirModalDuracion(esEdit) {
    esEdicionDuracion = esEdit;
    let sufijo = esEdit ? "Edit" : ""; 

    document.getElementById('tmpAnios').value = document.getElementById('hfAnios' + sufijo).value || 0;
    document.getElementById('tmpMeses').value = document.getElementById('hfMeses' + sufijo).value || 0;
    document.getElementById('tmpSemanas').value = document.getElementById('hfSemanas' + sufijo).value || 0;
    document.getElementById('tmpDias').value = document.getElementById('hfDias' + sufijo).value || 0;

    ActualizarPreview();

    var el = document.getElementById('modalDuracion');
    var modal = bootstrap.Modal.getOrCreateInstance(el);
    modal.show();
}

/**
 * 
 * @param {string} tipo
 * @param {number} cantidad 
 */
function Step(tipo, cantidad) {
    let inputId = "";
    if (tipo === 'anios') inputId = 'tmpAnios';
    if (tipo === 'meses') inputId = 'tmpMeses';
    if (tipo === 'semanas') inputId = 'tmpSemanas';
    if (tipo === 'dias') inputId = 'tmpDias';

    let input = document.getElementById(inputId);
    if (!input) return; 

    let valor = parseInt(input.value) + cantidad;

    if (valor < 0) valor = 0;
    if (tipo === 'meses' && valor > 11) valor = 11; 
    if (tipo === 'semanas' && valor > 4) valor = 4;
    if (tipo === 'dias' && valor > 30) valor = 30;

    input.value = valor;
    ActualizarPreview();
}

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

    let lbl = document.getElementById('lblLivePreview');
    if (lbl) lbl.innerText = resultado;
}

function GuardarDuracion() {
    let sufijo = esEdicionDuracion ? "Edit" : "";

    document.getElementById('hfAnios' + sufijo).value = document.getElementById('tmpAnios').value;
    document.getElementById('hfMeses' + sufijo).value = document.getElementById('tmpMeses').value;
    document.getElementById('hfSemanas' + sufijo).value = document.getElementById('tmpSemanas').value;
    document.getElementById('hfDias' + sufijo).value = document.getElementById('tmpDias').value;

    let display = document.getElementById('txtDuracionDisplay' + sufijo);
    let previewText = document.getElementById('lblLivePreview').innerText;

    if (display) display.value = previewText;

    var el = document.getElementById('modalDuracion');
    var modal = bootstrap.Modal.getInstance(el);
    modal.hide();
}