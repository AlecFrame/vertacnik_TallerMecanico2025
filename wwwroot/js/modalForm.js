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
 * @param {function} [options.onFormSubmitted] - función callback opcional a ejecutar después de un submit exitoso
 */

function configurarModalForm(options) {

    const {
        modalId,
        formId,
        createUrl,
        editUrl,
        createButtonId,
        editButtonClass,
        onFormSubmitted
    } = options;

    const $modal = $(modalId);
    const $body = $modal.find(".modal-body");
    // Submit handler delegado (se declara una vez) para evitar races: intercepta cualquier submit
    function delegatedSubmitHandler(e) {
        const $form = $(e.currentTarget);
        try {
            e.preventDefault();
        } catch (err) {
            // si no se pudo prevenir, dejar que continue (no debería ocurrir)
        }

        // detectar si hay file inputs con archivos -> usar FormData, si no -> serialize
        const fileInput = $form.find("input[type='file']")[0];
        const hasFile = fileInput && fileInput.files && fileInput.files.length > 0;

        const token = $form.find('input[name="__RequestVerificationToken"]').val();

        if (hasFile) {
            const fd = new FormData($form[0]);
            if (token) fd.append('__RequestVerificationToken', token);

            $.ajax({
                url: $form.attr('action'),
                type: $form.attr('method') || 'POST',
                data: fd,
                processData: false,
                contentType: false,
                headers: { 'X-Requested-With': 'XMLHttpRequest', ...(token ? { 'RequestVerificationToken': token } : {}) },
                success: function (result) {
                    handleAjaxResult(result, $form);
                },
                error: function (xhr) {
                    console.error('Error en petición AJAX (with files):', xhr.status, xhr.responseText);
                }
            });
        } else {
            const data = $form.serialize();
            $.ajax({
                url: $form.attr('action'),
                type: $form.attr('method') || 'POST',
                data: data,
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                success: function (result) {
                    handleAjaxResult(result, $form);
                },
                error: function (xhr) {
                    console.error('Error en petición AJAX (no files):', xhr.status, xhr.responseText);
                }
            });
        }
    }

    // función que procesa la respuesta AJAX del servidor (JSON o HTML)
    function handleAjaxResult(result, $form) {
        try {
            if (typeof result === 'object' && result !== null && result.success !== undefined) {
                if (result.success) {
                    // éxito
                    $modal.modal('hide');
                    if (typeof onFormSubmitted === 'function') {
                        onFormSubmitted();
                    }else {
                        location.reload();
                    }
                } else {
                    // resultado JSON con errores; si trae html, insertarlo, si no, mostrar console
                    if (result.html) {
                        $body.html(result.html);
                        // validar que la librería unobtrusive esté disponible
                        if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive && typeof jQuery.validator.unobtrusive.parse === 'function') {
                            jQuery.validator.unobtrusive.parse($form.selector || 'form');
                        } else {
                            console.warn('jquery.validate.unobtrusive.parse no está disponible. Asegúrate de que jquery.validate.unobtrusive se cargó correctamente.');
                        }
                    } else if (Array.isArray(result.errors)) {
                        console.warn('Errores:', result.errors);
                    }
                }
                return;
            }

            // Si nos llegaron HTML directamente (string) lo insertamos
            if (typeof result === 'string') {
                $body.html(result);
                if (window.jQuery && jQuery.validator && jQuery.validator.unobtrusive && typeof jQuery.validator.unobtrusive.parse === 'function') {
                    jQuery.validator.unobtrusive.parse($form.selector || 'form');
                } else {
                    console.warn('jquery.validate.unobtrusive.parse no está disponible. Asegúrate de que jquery.validate.unobtrusive se cargó correctamente.');
                }
                return;
            }

            // fallback
            $modal.modal('hide');
            if (typeof onFormSubmitted === 'function') {
                onFormSubmitted();
            }else {
                location.reload();
            }
        } catch (e) {
            console.error('Error procesando resultado AJAX:', e);
        }
    }

    // Delegar el submit para el selector del formId: esto evita condiciones donde el submit ocurra
    // antes de que el handler local sea ligado (por ejemplo al reemplazar HTML dentro del modal)
    $(document).off('submit', formId).on('submit', formId, delegatedSubmitHandler);

    function abrirModal(url) {
        $body.load(url, function () {
            $modal.modal('show');
            $.validator.unobtrusive.parse(formId);
        });
    }

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