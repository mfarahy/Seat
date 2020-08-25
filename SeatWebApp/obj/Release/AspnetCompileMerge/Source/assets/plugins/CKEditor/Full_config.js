CKEDITOR.editorConfig = function( config ) {
    config.extraPlugins = 'autosave,templates';
    config.language = 'fa';
    config.height = 400;
    config.skin = 'bootstrapck';
    config.filebrowserBrowseUrl = '/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/ckfinder/ckfinder.html?type=Images';
    config.filebrowserFlashBrowseUrl = '/ckfinder/ckfinder.html?type=Flash';
    config.filebrowserUploadUrl = '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = '/ckfinder/core/connector/php/connector.php?command=QuickUpload&type=Flash';
    config.contentsLangDirection = 'rtl';

    config.stylesSet = 'my_styles:styles.js';
};