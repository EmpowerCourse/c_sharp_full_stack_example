(function (App, $, undefined) {
    App.UserAdmin = App.UserAdmin || {};
    App.UserAdmin.LastEditedUserId = null;
    var urls = {};

    App.UserAdmin.BindRowElements = function () {
        $('.edit').unbind();
        $('.delete').unbind();
        $('.revive').unbind();
        $('.pager-link').unbind();

        $('.edit').click(function () {
            var tr = $(this).closest('tr');
            var userId = parseInt(tr.find('.key').html());
            var userRoleList = tr.find('.user-role-list').html();
            App.UserAdmin.ShowEditDialog(userId);
        });
        $('.delete').click(function () {
            var tr = $(this).closest('tr');
            $.ajax({
                data: JSON.stringify({ id: tr.find('.key').html() }),
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                type: 'POST',
                url: urls.DeleteUser,
                success: function (result) {
                    if (result.Success) {
                        App.UserAdmin.ReloadUsers(1);
                    } else {
                        alert(result.Message);
                    }
                }
            });
        });
        $('.revive').click(function () {
            var tr = $(this).closest('tr');
            $.ajax({
                data: // JSON.stringify(
                    { id: tr.find('.key').html() }, // ),
                dataType: 'json',
                // contentType: 'application/json; charset=utf-8',
                type: 'POST',
                url: urls.ReviveUser,
                success: function (result) {
                    if (result.Success) {
                        App.UserAdmin.ReloadUsers(1);
                    } else {
                        toastr.error(result.Message);
                    }
                }
            });
        });
        $('.pager-link').click(function () {
            App.UserAdmin.ReloadUsers(parseInt($(this).html()));
        });
    };

    App.UserAdmin.ShowEditDialog = function (userId) {
        App.UserAdmin.LastEditedUserId = userId;
        var viewURL;
        if (userId > 0) {
            viewURL = urls.EditUserView + '/' + userId;
        } else {
            viewURL = urls.NewUserView;
        }
        viewURL += '?typeOfUser=' + typeOfUser;
        $.ajax({
            dataType: 'json',
            url: viewURL,
            cache: false,
            success: function (result) {
                if (result.Success) {
                    $('#panelDetails').unbind().show();
                    $('#panelDetailsContents').html(result.Message);
                    $(".userrole").popover({ offset: -200 });
                    $('#panelDetails').dialog({
                        height: 450,
                        width: 600,
                        closeOnEscape: true,
                        resizable: false,
                        draggable: true,
                        modal: true,
                        buttons: {
                            Confirm: function () {
                                var context = $(this);
                                var formData = $('#editForm').serialize();
                                $.ajax({
                                    data: formData,
                                    type: 'POST',
                                    url: urls.Commit,
                                    success: function (response) {
                                        if (response.Success === false) {
                                            alert(response.Message);
                                            return;
                                        } else if (response.Message != null && response.Message !== '') {
                                            alert(response.Message);
                                        }
                                        context.dialog("close");
                                        App.UserAdmin.ReloadUsers();
                                    }
                                });
                                return false;
                            },
                            Cancel: function () {
                                $(this).dialog("close");
                                return false;
                            }
                        },
                        close: function (event, ui) {
                            $(this).unbind();
                            return false;
                        }
                    });
                    $('#panelDetails').dialog("option", "title", 'Edit User');
                } else {
                    toastr.error(result.Message);
                }
            }
        });
    };

    App.UserAdmin.ReloadUsers = function (page) {
        var roleId = $('#FilterRole').val();
        if (roleId === 0) roleId = null;
        var name = $('#FilterName').val();
        var activeOnly = $('#FilterActive').is(':checked');
        if (!page) page = 1;
        $.ajax({
            data: { // JSON.stringify({
                roleId: roleId,
                name: name,
                activeOnly: activeOnly,
                page: page
            },
            // contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: urls.Filter,
            success: function (result) {
                if (result.Success) {
                    $("#user-list").replaceWith(result.Message);
                    App.UserAdmin.BindRowElements();
                } else {
                    toastr.error(result.Message);
                }
            }
        });
    };

    App.UserAdmin.Init = function (paramUrls) {
        urls = paramUrls;

        $(document).ready(function () {
            $('#filter-btn').click(function () {
                App.UserAdmin.ReloadUsers();
            });

            $('#add-user').click(function () {
                App.UserAdmin.ShowEditDialog();
            });

            $("input[id^='Filter']").keypress(function (evt) {
                var charCode = evt.charCode || evt.keyCode;
                if (charCode === 13) {
                    $('#filter-btn').trigger('click');
                }
            });
        });
    };
} (window.App = window.App || {}, jQuery));
