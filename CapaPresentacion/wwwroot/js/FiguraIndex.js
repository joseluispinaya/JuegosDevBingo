
let tablaData;
let idEditar = 0;
const controlador = "Figuras";

$(function () {
    // 1. Cargar el Select de Tipos de Juego
    obtenerTipoFiguras();
    //obtenerTipoFigurasPrueba();

    // 2. Inicializar DataTables
    tablaData = $('#tbData').DataTable({
        responsive: true,
        ajax: {
            url: `/${controlador}/ListaFiguras`,
            type: "GET",
            datatype: "json",
            // IMPORTANTE: Le decimos a DataTables que nuestra lista está dentro de la propiedad "data"
            dataSrc: "data"
        },
        columns: [
            { data: "idFigura", visible: false, searchable: false },
            { title: "ID Tipo", data: "idTipo", visible: false },
            { title: "Nombre de Figura", data: "nombre" },
            { title: "Configuración", data: "configuracionJSON", visible: false }, // Lo ocultamos para que no sature la tabla visualmente
            {
                title: "Acción",
                data: "idFigura",
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-primary btn-sm btn-editar" data-id="${data}" title="Editar">
                            <i class="fas fa-pencil-alt"></i>
                        </button>
                        <button class="btn btn-danger btn-sm btn-eliminar ms-1" data-id="${data}" title="Eliminar">
                            <i class="fas fa-trash"></i>
                        </button>
                    `;
                }
            }
        ],
        order: [[0, "desc"]], // Ordenamos por IdFigura de forma descendente (los más nuevos primero)
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

    // Escuchar cambios en el Select del Modal
    $("#cboTipoJuego").on("change", function () {
        // 1. Ocultar todos los paneles
        $(".config-panel").addClass("d-none");

        // 2. Obtener el texto del tipo de juego seleccionado (Ej: "coordenadas", "conteo")
        // Usamos el texto porque tu base de datos tiene "coordenadas", pero el value del select es un Id numérico.
        let tipoSeleccionado = $("#cboTipoJuego option:selected").text().toLowerCase().trim();

        // 3. Mostrar el panel correspondiente
        if (tipoSeleccionado === "coordenadas") {
            $("#panelCoordenadas").removeClass("d-none");
        }
        else if (tipoSeleccionado === "conteo") {
            $("#panelConteo").removeClass("d-none");
        }
        else if (tipoSeleccionado === "n_columnas") {
            $("#panelColumnas").removeClass("d-none");
        }
        else if (tipoSeleccionado === "n_filas") {
            $("#panelFilas").removeClass("d-none");
        }
    });

});

// Función para obtener los Tipos de Juego y llenar el Select
function obtenerTipoFiguras() {
    fetch(`/${controlador}/ListaTipoJuego`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                $("#cboTipoJuego").empty();
                $("#cboTipoJuego").append($("<option>").val("").text("Seleccione un tipo..."));

                responseJson.data.forEach((item) => {
                    $("#cboTipoJuego").append($("<option>").val(item.idTipo).text(item.nombre));
                });
            } else {
                swal("Error", responseJson.mensaje, "error");
            }
        })
        .catch((error) => {
            console.error("Error al cargar tipos de juego:", error);
        });
}

$('#tbData tbody').on('click', '.btn-eliminar', function () {
    let fila = $(this).closest('tr');
    if (fila.hasClass('child')) fila = fila.prev();
    let data = tablaData.row(fila).data();

    swal({
        title: "¿Estás seguro?",
        text: `Se eliminará la figura: ${data.nombre}`,
        icon: "warning",
        buttons: ["Cancelar", "Eliminar"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            // Hacemos el fetch enviando el ID en la URL
            fetch(`/${controlador}/Eliminar?id=${data.idFigura}`, {
                method: "DELETE"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(responseJson => {
                    if (responseJson.estado) {
                        swal("Eliminado!", responseJson.mensaje, "success");
                        tablaData.ajax.reload();
                    } else {
                        swal("Error!", responseJson.mensaje, "warning");
                    }
                })
                .catch(() => {
                    swal("Error!", "No se pudo conectar con el servidor.", "error");
                });
        }
    });
});


$('#tbData tbody').on('click', '.btn-editar', function () {
    let fila = $(this).closest('tr');
    if (fila.hasClass('child')) {
        fila = fila.prev();
    }
    let data = tablaData.row(fila).data();

    idEditar = data.idFigura;
    $("#txtNombreFigura").val(data.nombre);

    // 1. Limpiamos todos los checkboxes e inputs de los paneles por si quedaron sucios
    $(".chk-coord, .chk-columna, .chk-fila").prop("checked", false);
    $("#txtCantidadConteo").val(10);
    $("#txtCantColumnas, #txtCantFilas").val(1);

    // 2. Al cambiar el select, el evento "change" (que hicimos antes) mostrará el panel correcto automáticamente
    $("#cboTipoJuego").val(data.idTipo).trigger("change");

    // 3. Parseamos el JSON que viene de SQL Server
    let configObj = JSON.parse(data.configuracionJSON);
    let tipoTexto = $("#cboTipoJuego option:selected").text().toLowerCase().trim();

    // 4. Llenamos el panel correspondiente con los datos
    if (tipoTexto === "coordenadas" && configObj.coordenadas) {
        configObj.coordenadas.forEach(c => {
            // Buscamos el checkbox exacto usando sus atributos data y lo marcamos
            // c[0] es la Letra (ej: "B"), c[1] es el número (ej: 0)
            $(`.chk-coord[data-col='${c[0]}'][data-row='${c[1]}']`).prop("checked", true);
        });
    }
    else if (tipoTexto === "conteo" && configObj.cantidadObjetivo) {
        $("#txtCantidadConteo").val(configObj.cantidadObjetivo);
    }
    else if (tipoTexto === "n_columnas" && configObj.columnasValidas) {
        configObj.columnasValidas.forEach(col => {
            $(`.chk-columna[value='${col}']`).prop("checked", true);
        });
        $("#txtCantColumnas").val(configObj.cantidadRequerida);
    }
    else if (tipoTexto === "n_filas" && configObj.filasValidas) {
        configObj.filasValidas.forEach(filaNum => {
            $(`.chk-fila[value='${filaNum}']`).prop("checked", true);
        });
        $("#txtCantFilas").val(configObj.cantidadRequerida);
    }

    $("#modalFigura").modal("show");
});

$("#btnNuevoReg").on("click", function () {
    // 1. Reiniciar variables de control
    idEditar = 0;

    // 2. Limpiar campos principales
    $("#txtNombreFigura").val("");

    // 3. Reiniciar el select y forzar el evento "change" para que oculte cualquier panel que haya quedado abierto
    $("#cboTipoJuego").val("").trigger("change");

    // 4. Limpiar todos los checkboxes del cartón y de las opciones de filas/columnas
    $(".chk-coord, .chk-columna, .chk-fila").prop("checked", false);

    // 5. Restaurar los valores por defecto de los inputs numéricos
    $("#txtCantidadConteo").val(10);
    $("#txtCantColumnas").val(1);
    $("#txtCantFilas").val(1);

    // 6. Finalmente, abrir el modal impecable
    $("#modalFigura").modal("show");
});

$("#btnPruebaAlert").on("click", function () {
    ConfirmarAccion("Eliminar Cartón", "Es una prueba de alerta confirmacion", "Eliminar", () => {
        MensajeToast("Hecho", "Cartón eliminado.", "success");
    });
});

function habilitarBoton() {
    $('#btnGuardar').prop('disabled', false);
}

$("#btnGuardar").on("click", function () {

    $('#btnGuardar').prop('disabled', true);

    let idTipoSeleccionado = $("#cboTipoJuego").val();

    if ($("#txtNombreFigura").val().trim() === "") {
        MensajeToast("Campo incompleto", "Debe Ingresar Nombre.", "warning");
        habilitarBoton();
        return;
    }

    if (idTipoSeleccionado === "") {
        MensajeToast("Campo incompleto", "Debe seleccionar un tipo de Juego.", "warning");
        habilitarBoton();
        return;
    }

    let tipoTexto = $("#cboTipoJuego option:selected").text().toLowerCase().trim();
    let configObj = {}; // Aquí armaremos el JSON según el tipo

    if (tipoTexto === "coordenadas") {
        let coords = [];
        // Recorremos todos los checkboxes del cartón que estén marcados
        $(".chk-coord:checked").each(function () {
            coords.push([$(this).data("col"), parseInt($(this).data("row"))]);
        });

        if (coords.length === 0) {
            MensajeToast("Faltan datos", "Debe marcar al menos una casilla en el cartón.", "warning");
            habilitarBoton(); return;
        }
        configObj = { coordenadas: coords };
    }
    else if (tipoTexto === "conteo") {
        let cant = parseInt($("#txtCantidadConteo").val());
        if (isNaN(cant) || cant <= 0) {
            MensajeToast("Faltan datos", "Ingrese una cantidad válida para el conteo.", "warning");
            habilitarBoton(); return;
        }
        configObj = { cantidadObjetivo: cant };
    }
    else if (tipoTexto === "n_columnas") {
        let cols = [];
        $(".chk-columna:checked").each(function () { cols.push($(this).val()); });
        let cant = parseInt($("#txtCantColumnas").val());

        if (cols.length === 0 || isNaN(cant) || cant <= 0) {
            MensajeToast("Faltan datos", "Seleccione columnas válidas y una cantidad requerida.", "warning");
            habilitarBoton(); return;
        }
        configObj = { columnasValidas: cols, cantidadRequerida: cant };
    }
    else if (tipoTexto === "n_filas") {
        let filas = [];
        $(".chk-fila:checked").each(function () { filas.push(parseInt($(this).val())); });
        let cant = parseInt($("#txtCantFilas").val());

        if (filas.length === 0 || isNaN(cant) || cant <= 0) {
            MensajeToast("Faltan datos", "Seleccione filas válidas y una cantidad requerida.", "warning");
            habilitarBoton(); return;
        }
        configObj = { filasValidas: filas, cantidadRequerida: cant };
    }

    // Armamos el objeto final para C#
    const objeto = {
        IdFigura: idEditar,
        IdTipo: parseInt(idTipoSeleccionado),
        Nombre: $("#txtNombreFigura").val().trim(),
        ConfiguracionJSON: JSON.stringify(configObj)
    };

    $("#modalFigura").find("div.modal-content").LoadingOverlay("show");

    if (idEditar != 0) {

        fetch(`/${controlador}/Editar`, {
            method: "PUT",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    alertaTimer("Mensaje", responseJson.mensaje, "success");
                    $("#modalFigura").modal("hide");
                    idEditar = 0;
                    tablaData.ajax.reload();
                } else {
                    alertaTimer("Error!", responseJson.mensaje, "warning");
                }
            })
            .catch(() => {
                swal("Error!", "No se pudo conectar con el servidor.", "warning");
            })
            .finally(() => {
                $("#modalFigura").find("div.modal-content").LoadingOverlay("hide");
                habilitarBoton();
            });
    } else {
        fetch(`/${controlador}/Guardar`, {
            method: "POST",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        })
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if (responseJson.estado) {
                    alertaTimer("Mensaje", responseJson.mensaje, "success");
                    $("#modalFigura").modal("hide");
                    tablaData.ajax.reload();
                } else {
                    alertaTimer("Error!", responseJson.mensaje, "warning");
                }
            })
            .catch(() => {
                alertaTimer("Error!", "No se pudo conectar con el servidor.", "warning");
            })
            .finally(() => {
                $("#modalFigura").find("div.modal-content").LoadingOverlay("hide");
                habilitarBoton();
            });
    }
});


function obtenerTipoFigurasPrueba() {
    fetch(`/${controlador}/ListaTipoJuegoDapper`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            // Muestra el objeto JSON completo devuelto por tu controlador
            console.log("Datos recibidos del servidor:", responseJson);

            // Si quieres ver específicamente la lista de datos:
            // console.log("Lista de Tipos:", responseJson.data);
        })
        .catch((error) => {
            console.error("Error en la petición fetch:", error);
        });
}

// fin