tinymce.init({
    selector: 'textarea#Mensaje',
    skin: (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'oxide-dark' : 'oxide'),
    content_css: (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'default'),
    language: 'es',
    plugins: 'image link media table charmap hr pagebreak wordcount preview insertdatetime code',
    menubar: 'file edit insert format',
    height: '30em',
    toolbar: [
        'btnGuardar | undo redo | fontselect fontsizeselect styleselect | code',
        'bold italic underline | alignleft aligncenter alignright alignjustify | table | link | pagebreak' ],
    setup: function (editor) {
        editor.ui.registry.addButton('btnGuardar', {
            text: 'Guardar',
            onAction: function () {
                $('#frmEditar').submit();
            }
        });
    },
    insertdatetime_formats: ['%Y-%m-%d', '%d-%m-%Y', '%A %d, %Y']
});