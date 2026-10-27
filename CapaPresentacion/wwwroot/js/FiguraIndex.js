
let tablaData;
let idEditar = 0;
const controlador = "Figuras";

$(function () {
    // 1. Cargar el Select de Tipos de Juego
    obtenerTipoFiguras();
    obtenerTipoFigurasPrueba();

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

$("#btnNuevoReg").on("click", function () {

    idEditar = 0;

    $("#modalFigura").modal("show");
})

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