/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
    config.filebrowserBrowseUrl = '/Areas/Admin/plugin/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/Areas/Admin/plugin/ckfinder/ckfinder.html?type=Images';
    config.filebrowserFlashBrowseUrl = '/Areas/Admin/plugin/ckfinder/ckfinder.html?type=Flash';
    config.filebrowserUploadUrl = '/Areas/Admin/plugin/ckfinder/connector?command=QuickUpload&type=Files';
    config.filebrowserImageUploadUrl = '/Areas/Admin/plugin/ckfinder/core/connector/aspx/lang/connector.aspx?command=QuickUpload&type=Images';
    config.filebrowserFlashUploadUrl = '/Areas/Admin/plugin/ckfinder/core/connector/aspx/lang/connector.aspx?command=QuickUpload&type=Flash';
    // Sample configuration options:
    // config.uiColor = '#BDE31E';
    // config.language = 'fr';
    // config.removePlugins = 'basket';
    //config.toolbarGroups = [
    //    { name: 'clipboard', groups: ['clipboard', 'undo'] },
    //    { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
    //    { name: 'links' },
    //    { name: 'insert' },
    //    { name: 'forms' },
    //    { name: 'tools' },
    //    { name: 'document', groups: ['mode', 'document', 'doctools'] },
    //    { name: 'others' },
    //    '/',
    //    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
    //    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
    //    { name: 'styles' },
    //    { name: 'colors' },
    //    { name: 'about' },
    //];

    //config.removeButtons = "Underline,Subscript,Superscript";
    //config.format_tags = 'p;h1;h2;h3;pre';
    //config.removeDialogTabs = 'image:advanced;link:advanced';
};
