// ==========================================
// UTILIDADES DE VALIDACIÓN (SISTEMA UTC)
// ==========================================

// Función auxiliar para mostrar errores
function mostrarError(campoId, mensaje) {
    if (typeof toastify === 'function') {
        toastify('ww', mensaje, 'Sistema');
    } else {
        alert(mensaje);
    }
    var campo = document.getElementById(campoId);
    if (campo) {
        campo.classList.add('is-invalid');
        campo.focus();
        campo.addEventListener('input', function () {
            this.classList.remove('is-invalid');
        }, { once: true });
    }
}

// Validador de Email
function esEmailValido(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

/**
 * VALIDADOR DE CÉDULA ECUATORIANA (ALGORITMO MÓDULO 10)
 */
function esCedulaValida(cedula) {
    // 1. Validar que sean solo números y longitud 10
    if (cedula.length !== 10 || isNaN(cedula)) return false;

    // 2. Validar Código de Provincia (01-24) y casos especiales (30)
    var provincia = parseInt(cedula.substring(0, 2), 10);
    if (provincia < 1 || (provincia > 24 && provincia !== 30)) return false;

    // 3. Validar Tercer Dígito (Personas naturales: 0-5)
    var tercerDigito = parseInt(cedula.substring(2, 3), 10);
    if (tercerDigito >= 6) return false;

    // 4. Algoritmo verificador
    var coeficientes = [2, 1, 2, 1, 2, 1, 2, 1, 2];
    var verificador = parseInt(cedula.substring(9, 10), 10);
    var suma = 0;

    for (var i = 0; i < 9; i++) {
        var valor = parseInt(cedula.substring(i, i + 1), 10) * coeficientes[i];
        suma += (valor >= 10) ? valor - 9 : valor;
    }

    var digitoCalculado = (suma % 10 === 0) ? 0 : (10 - (suma % 10));

    return verificador === digitoCalculado;
}

// ==========================================
// 1. VALIDACIÓN DE GRUPO
// ==========================================
function ValidarFormularioGrupo() {
    var idCentro = '<%= ddlCentro.ClientID %>';
    var idNombre = '<%= txtNombreGru.ClientID %>';
    var idCoord = '<%= txtCoordinadorGru.ClientID %>';
    var idFecha = '<%= txtFechaCreaGru.ClientID %>';

    var valCentro = document.getElementById(idCentro).value;
    if (valCentro === "" || valCentro === "0") {
        mostrarError(idCentro, 'Debe seleccionar un Centro de Investigación.');
        return false;
    }

    var valNombre = document.getElementById(idNombre).value;
    if (valNombre.trim().length < 5) {
        mostrarError(idNombre, 'El nombre del grupo es muy corto o está vacío.');
        return false;
    }

    var valCoord = document.getElementById(idCoord).value;
    if (valCoord.trim() === "") {
        mostrarError(idCoord, 'Es obligatorio asignar un Coordinador Principal.');
        return false;
    }

    var valFecha = document.getElementById(idFecha).value;
    if (valFecha === "") {
        mostrarError(idFecha, 'Ingrese la fecha de creación del grupo.');
        return false;
    }

    return true;
}

// ==========================================
// 2. VALIDACIÓN DE INTEGRANTE (ACTUALIZADA)
// ==========================================
function ValidarFormularioIntegrante() {
    var idCedula = '<%= txtCedulaInt.ClientID %>';
    var idNombres = '<%= txtNombresInt.ClientID %>';
    var idApellidos = '<%= txtApellidosInt.ClientID %>';
    var idCorreo = '<%= txtCorreoInt.ClientID %>';
    var idTipo = '<%= ddlTipoInt.ClientID %>';
    var idCarrera = '<%= txtCarreraInt.ClientID %>';
    var idFacultad = '<%= ddlFacultadInt.ClientID %>';
    var idEntidad = '<%= txtEntidadInt.ClientID %>';

    // --- VALIDACIÓN DE CÉDULA ECUATORIANA ---
    var valCedula = document.getElementById(idCedula).value.trim();
    if (!esCedulaValida(valCedula)) {
        mostrarError(idCedula, 'La cédula ingresada es incorrecta o no es válida en Ecuador.');
        return false;
    }

    if (document.getElementById(idNombres).value.trim() === "") {
        mostrarError(idNombres, 'Ingrese los nombres del integrante.');
        return false;
    }
    if (document.getElementById(idApellidos).value.trim() === "") {
        mostrarError(idApellidos, 'Ingrese los apellidos del integrante.');
        return false;
    }

    var valCorreo = document.getElementById(idCorreo).value;
    if (!esEmailValido(valCorreo)) {
        mostrarError(idCorreo, 'El formato del correo electrónico es incorrecto.');
        return false;
    }

    var tipo = document.getElementById(idTipo).value;

    if (tipo === 'Interno' || tipo === 'Docente') {
        var valFacultad = document.getElementById(idFacultad).value;
        if (valFacultad === "" || valFacultad === "-- Seleccione --") {
            mostrarError(idFacultad, 'Seleccione la Facultad o Extensión.');
            return false;
        }
        if (document.getElementById(idCarrera).value.trim() === "") {
            mostrarError(idCarrera, 'El campo Carrera / Departamento es obligatorio.');
            return false;
        }
    }
    else if (tipo === 'Externo') {
        if (document.getElementById(idEntidad).value.trim() === "") {
            mostrarError(idEntidad, 'Debe especificar la Institución de Origen para externos.');
            return false;
        }
    }

    return true;
}

// ==========================================
// 3. VALIDACIÓN DE COORDINADOR (MODAL)
// ==========================================
function ValidarModalCoordinador() {
    var ddlTipo = document.getElementById('<%= ddlTipoCoord.ClientID %>');

    // Si es búsqueda por docente, validamos que se haya cargado un nombre
    if (ddlTipo.value === 'Docente') {
        var nombreCargado = document.getElementById('<%= txtNombreCoord.ClientID %>').value;
        if (nombreCargado === "") {
            if (typeof toastify === 'function') toastify('ww', 'Debe buscar y seleccionar un docente antes de asignar.', 'Sistema');
            else alert('Debe buscar y seleccionar un docente.');
            return false;
        }
    }
    else {
        // Si es Interno o Externo manual, validamos la cédula también
        var idCedulaCoord = '<%= txtCedulaCoord.ClientID %>';
        var cedulaCoord = document.getElementById(idCedulaCoord).value.trim();

        if (!esCedulaValida(cedulaCoord)) {
            mostrarError(idCedulaCoord, 'La cédula del coordinador no es válida.');
            return false;
        }
    }

    return true;
}