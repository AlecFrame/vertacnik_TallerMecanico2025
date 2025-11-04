/**
 * ModalForm.js - script genérico para manejar modales con formularios cargados por AJAX
 *
 * @param {object} options - configuración del modal
 * @param {string} options.modalId - ID del modal (por ejemplo '#modalUsuario')
 * @param {string} options.formId - ID del formulario dentro del modal
 * @param {string} options.createUrl - URL para crear (por ejemplo '/Usuarios/Crear')
 * @param {string} options.editUrl - URL base para editar (por ejemplo '/Usuarios/Editar')
 * @param {string} options.createButtonId - ID del botón de crear (por ejemplo '#btnCrearUsuario')
 * @param {string} options.editButtonClass - clase CSS para botones de editar (por ejemplo '.btn-editar-usuario')
 */
function configurarModalForm(options) {

    const {
        modalId,
        formId,
        createUrl,
        editUrl,
        createButtonId,
        editButtonClass
    } = options;

    const $modal = $(modalId);
    const $body = $modal.find(".modal-body");

    function abrirModal(url) {
        // función para (re)ligar el handler de submit del formulario dentro del modal
        function bindFormSubmit() {
            $(formId).off("submit").on("submit", function (e) {
                e.preventDefault();
                const form = this;
                const $form = $(this);
                const formData = new FormData(form);

                const token = $form.find('input[name="__RequestVerificationToken"]').val();
                if (token) formData.append('__RequestVerificationToken', token);

                $.ajax({
                    url: $form.attr("action"),
                    type: $form.attr("method") || 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        ...(token ? { 'RequestVerificationToken': token } : {})
                    },
                    success: function (result) {
                        try {
                            // Si el servidor devolvió JSON (éxito o errores)
                            if (typeof result === "object" && result.success !== undefined) {
                                if (result.success) {
                                    $modal.modal("hide");
                                    location.reload();
                                } else {
                                    console.warn("Errores:", result.errors);
                                }
                                return;
                            }

                            // Si es HTML (form con errores), volver a renderizar dentro del modal
                            if (typeof result === "string") {
                                $body.html(result);
                                $.validator.unobtrusive.parse(formId);
                                // volver a ligar el submit para que siga funcionando tras reemplazar el HTML
                                bindFormSubmit();
                                return;
                            }

                            // fallback por si acaso
                            $modal.modal("hide");
                            location.reload();

                        } catch (e) {
                            console.error("Error procesando respuesta del servidor:", e);
                        }
                    },
                    error: function (xhr, status, err) {
                        console.error(`Error al guardar datos (${modalId}):`, err);
                        console.error('Status:', xhr.status, 'Response:', xhr.responseText);
                    }
                });
            });
        }

        $body.load(url, function () {
            $modal.modal("show");
            $.validator.unobtrusive.parse(formId);
            // ligar submit la primera vez
            bindFormSubmit();
        });
    }

    /* Saque este ejemplo específico para Inquilinos, usa un form generico sin el file o form Data
    function abrirModalInquilino(url) {
        $("#contenidoModalInquilino").load(url, function () {
            var modal = new bootstrap.Modal(document.getElementById('modalInquilino'));
            modal.show();
            modal.off("submit").on("submit", "#formInquilino", function (e) {
                e.preventDefault();
                var form = $(this);
                $.ajax({
                    type: form.attr('method'),
                    url: form.attr('action'),
                    data: form.serialize(),
                    success: function (response) {
                        if (response.success) {
                            modal.hide();
                            location.reload(); // Recargar la página para ver los cambios
                        } else {
                            $("#contenidoModalInquilino").html(response); // Mostrar errores de validación
                        }
                    }
                });
            });
        });
    }
    */

    // Botón de crear
    if (createButtonId) {
        $(document).on("click", createButtonId, function () {
            abrirModal(createUrl);
        });
    }

    // Botones de editar
    if (editButtonClass) {
        $(document).on("click", editButtonClass, function () {
            const id = $(this).data("id");
            abrirModal(`${editUrl}/${id}`);
        });
    }
}