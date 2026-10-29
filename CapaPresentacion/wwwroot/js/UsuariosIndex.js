
let tablaData;
let idEditar = 0;
const controlador = "Usuarios";

function ObtenerFecha() {
    const d = new Date();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${day}/${month}/${d.getFullYear()}`;
}

$(function () {

    $("#txtFecha").datepicker({
        todayHighlight: true,
        language: "es",
        format: "dd/mm/yyyy",
        autoclose: true
    });

    $("#txtFecha").val(ObtenerFecha());

    // 2. Inicializar DataTables
    tablaData = $('#tbData').DataTable({
        responsive: true,
        ajax: {
            url: `/${controlador}/ListaUsuarios`,
            type: "GET",
            datatype: "json",
            // IMPORTANTE: Le decimos a DataTables que nuestra lista está dentro de la propiedad "data"
            dataSrc: "data"
        },
        columns: [
            { data: "idUsuario", visible: false, searchable: false },
            {
                title: "Usuarios",
                data: null,
                className: "text-start align-middle",
                render: function (data, type, row) {
                    return `<div class="fw-bold text-dark">${row.nombreCompleto}</div>
                        <div class="fs-12px text-muted"><i class="fa fa-envelope me-1"></i>${row.correo}</div>`;
                }
            },
            { title: "Rol", data: "nombreRol" },
            {
                title: "Estado",
                data: "estado",
                render: function (data) {
                    return data
                        ? `<span
                                class="badge border border-warning text-warning px-2 pt-5px pb-5px rounded fs-12px d-inline-flex align-items-center">
                                <i class="fa fa-check fs-9px fa-fw me-5px"></i>Activo
                            </span>`
                        : `<span
                                class="badge border border-danger text-danger px-2 pt-5px pb-5px rounded fs-12px d-inline-flex align-items-center">
                                <i class="fa fa-times fs-9px fa-fw me-5px"></i>Inactivo
                            </span>`;
                }
            },
            {
                title: "Suscripción",
                data: "fechaVencimientoStr",
                render: function (data) {
                    return `<span class="badge bg-primary bg-opacity-15 text-primary fw-bold fs-12px me-2">${data}</span>`;
                }
            },
            {
                title: "Acción",
                data: "idUsuario",
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button class="btn btn-primary btn-sm btn-editar" data-id="${data}" title="Editar">
                            <i class="fas fa-pencil-alt"></i>
                        </button>
                        <button class="btn btn-danger btn-sm btn-estado ms-1" data-id="${data}" title="Estado">
                            <i class="fas fa-flag"></i>
                        </button>
                    `;
                }
            }
        ],
        order: [[0, "desc"]],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

});


$('#tbData tbody').on('click', '.btn-estado', function () {
    let fila = $(this).closest('tr');
    if (fila.hasClass('child')) fila = fila.prev();
    let data = tablaData.row(fila).data();

    let estatus = !data.estado;
    let estadoTexto = estatus ? "ACTIVAR" : "INACTIVAR";

    swal({
        title: "¿Estás seguro?",
        text: `¿Deseas ${estadoTexto} la suscripción del usuario ${data.nombreCompleto}?`,
        icon: "warning",
        buttons: ["Cancelar", "Si aceptar"],
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            fetch(`/${controlador}/ActualizarEstado?idUsuario=${data.idUsuario}&estado=${estatus}`, {
                method: "PUT"
            })
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(responseJson => {
                    if (responseJson.estado) {
                        MensajeToast("Hecho", responseJson.mensaje, "success");
                        tablaData.ajax.reload();
                    } else {
                        alertaTimer("Error", responseJson.mensaje, "warning");
                    }
                })
                .catch(() => {
                    alertaTimer("Error!", "No se pudo conectar con el servidor.", "error");
                });
        }
    });
});

$('#tbData tbody').on('click', '.btn-editar', function () {
    let fila = $(this).closest('tr');
    if (fila.hasClass('child')) fila = fila.prev();

    let data = tablaData.row(fila).data();
    idEditar = data.idUsuario;

    // Llenamos los inputs del modal
    $("#txtNombres").val(data.nombreCompleto);
    $("#txtCelular").val(data.celular);
    $("#txtCorreo").val(data.correo);
    $("#txtFecha").val(data.fechaVencimientoStr);

    $("#lblTituloModal").text("Editar Usuario");
    $("#mdUsuario").modal("show");
});

$("#btnNuevoUsuario").on("click", function () {
    idEditar = 0;

    // Limpia todos los inputs que tengan la clase 'model'
    $(".model").val("");
    $("#txtFecha").val(ObtenerFecha());

    $("#lblTituloModal").text("Registrar Usuario");
    $("#mdUsuario").modal("show");
});

function habilitarBoton() {
    $('#btnGuardarUsuario').prop('disabled', false);
}

$("#btnGuardarUsuario").on("click", function () {

    $('#btnGuardarUsuario').prop('disabled', true);

    const inputs = $("input.model").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() === "");

    if (inputs_sin_valor.length > 0) {
        const nombreCampo = inputs_sin_valor[0].name;
        const mensaje = `Debe completar el campo: "${nombreCampo}"`;
        MensajeToast("Advertencia", mensaje, "warning");

        // El selector ahora funcionará correctamente porque quitamos el espacio del 'name' en el HTML
        $(`input[name="${nombreCampo}"]`).focus();

        //$(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        //$(`input[name="${inputs_sin_valor[0].name}"]`).trigger("focus");
        habilitarBoton();
        return;
    }

    let fechaStr = $("#txtFecha").val().trim();

    if (fechaStr === "") {
        MensajeToast("Campo requerido", "La fecha es obligatoria.", "warning");
        habilitarBoton();
        return;
    }
    // Armamos el objeto final para C#
    const objeto = {
        IdUsuario: idEditar,
        NombreCompleto: $("#txtNombres").val().trim(),
        Celular: $("#txtCelular").val().trim(),
        Correo: $("#txtCorreo").val().trim(),
        FechaVencimientoStr: fechaStr
    };

    $("#mdUsuario").find("div.modal-content").LoadingOverlay("show");

    let urlAccion = idEditar != 0 ? `/${controlador}/Editar` : `/${controlador}/Guardar`;
    let metodoApi = idEditar != 0 ? "PUT" : "POST";

    fetch(urlAccion, {
        method: metodoApi,
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(objeto)
    })
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                MensajeToast("Mensaje", responseJson.mensaje, "success");
                $("#mdUsuario").modal("hide");
                tablaData.ajax.reload();
            } else {
                alertaTimer("Error!", responseJson.mensaje, "warning");
            }
        })
        .catch(() => {
            alertaTimer("Error!", "No se pudo conectar con el servidor.", "warning");
        })
        .finally(() => {
            $("#mdUsuario").find("div.modal-content").LoadingOverlay("hide");
            habilitarBoton();
        });

});


// fin