(function (App, $, undefined) {

    App.InitializeAdminNewButton = function (containerScopeSelector, newButtonSelector, newUrl, editDialogElement, dialogTitle, callback) {
        $(containerScopeSelector).on('click', newButtonSelector, function () {
            $.get(newUrl, function (data) {
                editDialogElement.html(data);
                editDialogElement.dialog('option', { title: dialogTitle });
                editDialogElement.dialog('open');
                if (callback) {
                    callback();
                }
            });
            return false;
        });
    };

    App.InitializeEditButtons = function (containerScopeSelector, editButtonSelector, editDialogElement, dialogTitle, callback) {
        $(containerScopeSelector).on('click', editButtonSelector, function () {
            var targetURL = $(this).attr('href');
            $.ajax({
                url: targetURL,
                dataType: 'html',
                cache: false,
                type: 'GET',
                success: function (data) {
                    editDialogElement.html(data);
                    editDialogElement.dialog('option', { title: dialogTitle });
                    editDialogElement.dialog('open');
                    if (callback) {
                        callback();
                    }
                }
            });
            return false;
        });
    };

    App.InitializeRetireButton = function (containerScopeSelector, retireButtonSelector, successCallback) {
        $(containerScopeSelector).on('click', retireButtonSelector, function (e) {
            e.preventDefault();
            var url = $(this).attr('href');
            var id = $(this).data('id');
            $.ajax({
                url: url,
                dataType: 'json',
                data: { id: id },
                type: 'POST',
                success: function (result) {
                    if (result.Success) {
                        successCallback();
                    } else {
                        $.jGrowl(result.Message, { theme: 'error', sticky: true });
                    }
                }
            });
        });
    };

    App.InitializeEditDialog = function (dialogElement, width, height, successCallback) {
        dialogElement.dialog({
            autoOpen: false,
            modal: true,
            width: width,
            height: height,
            buttons: [
                {
                    text: 'Ok',
                    'class': 'btn btn-success admin-commit',
                    click: function () {
                        var form = dialogElement.find("form");
                        var data = form.serialize();
                        var url = form.attr('action');
                        var method = form.attr('method');
                        $.ajax({
                            url: url,
                            dataType: 'html',
                            cache: false,
                            data: data,
                            type: method,
                            success: function (markup) {
                                if (markup.length > 0) {
                                    dialogElement.html(markup);
                                    dialogElement.dialog("option", "position", "center");
                                    $('#edit-dialog').scrollTop();
                                } else {
                                    successCallback();
                                    dialogElement.dialog('close');
                                }
                            }
                        });
                    }
                },
                {
                    text: 'Cancel',
                    'class': 'btn btn-inverse',
                    click: function () {
                        dialogElement.dialog('close');
                    }
                }
            ]
        });
    };

    App.InitCommonAdminFunctions = function () {
        $(window).resize(function () {
            $("#edit-dialog").dialog("option", "position", "center");
        });

        //$("input#Item_Result_Name").keypress(function (evt) {
        //    var charCode = evt.charCode || evt.keyCode;
        //    if (charCode === 13) {
        //        $('.admin-commit').trigger('click');
        //    }
        //});
    };

    App.UpdateAdminList = function (page, listElement, getPageUrl, callback) {
        if (page === null || page === undefined) {
            page = $('.pagination').data('currentpage');
            if (page === null || page === undefined) {
                //if no pager, fall back to 1 as default
                page = 1;
            }
        }
        listElement.fadeOut('fast', function () {
            $.ajax({
                cache: false,
                url: getPageUrl + '?page=' + page,
                success: function (result) {
                    listElement.html(result);
                    listElement.fadeIn('fast');
                    if (callback) callback();
                }
            });
        });
    };
}(window.CF = window.CF || {}, jQuery));