(function (App, $, undefined) {
    App.UserAdmin = App.UserAdmin || {};
    App.UserAdmin.LastEditedUserId = null;
    var urls = {};

    App.UserAdmin.RowId = function (tr) {
        return parseInt(tr.attr('data-id'));
    };

    App.UserAdmin.BindRowElements = function () {
        $('.edit').unbind();
        $('.delete').unbind();
        $('.revive').unbind();
        $('.pager-link').unbind();

        $('.edit').click(function () {
            var tr = $(this).closest('tr');
            var userRoleList = tr.find('.user-role-list').html();
            App.UserAdmin.ShowEditDialog(
                App.UserAdmin.RowId(tr));
        });
        $('.delete').click(function () {
            var tr = $(this).closest('tr');
            $.ajax({
                data: { id: App.UserAdmin.RowId(tr) },
                dataType: 'json',
                type: 'POST',
                url: urls.DeleteUser,
                success: function (result) {
                    if (result.success) {
                        App.UserAdmin.ReloadUsers(1);
                    } else {
                        toastr.error(result.message);
                    }
                }
            });
        });
        $('.revive').click(function () {
            var tr = $(this).closest('tr');
            $.ajax({
                data: { id: App.UserAdmin.RowId(tr) },
                dataType: 'json',
                type: 'POST',
                url: urls.ReviveUser,
                success: function (result) {
                    if (result.success) {
                        App.UserAdmin.ReloadUsers(1);
                    } else {
                        toastr.error(result.message);
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
        var viewURL = (userId > 0)
            ? urls.EditUserView + '/' + userId
            : urls.NewUserView;
        $.ajax({
            dataType: 'html',
            url: viewURL,
            cache: false,
            success: function (result) {
                $('#user-edit-dialog .modal-body').html(result);
                $('#user-edit-dialog').modal('show');
                $(".userrole").popover({ offset: -200 });
            }
        });
    };

    App.UserAdmin.ReloadUsers = function (page) {
        var roleId = $('#FilterRole').val();
        if (roleId === 0) roleId = null;
        var name = $('#FilterName').val();
        var activeOnly = $('#FilterActive').is(':checked');
        $.ajax({
            data: {
                roleId: roleId,
                name: name,
                activeOnly: activeOnly,
                page: page
            },
            cache: false,
            dataType: 'html',
            type: 'POST',
            url: urls.Filter,
            success: function (result) {
                $("div#user-list").replaceWith(result);
                App.UserAdmin.BindRowElements();
            }
        });
    };

    App.UserAdmin.Init = function (paramUrls) {
        urls = paramUrls;

        $(document).ready(function () {
            $('#filter-btn').click(function () {
                App.UserAdmin.ReloadUsers(1);
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

            $('#commit-user').click(function (e) {
                e.preventDefault();
                $.ajax({
                    data: $('#editForm').serialize(),
                    type: 'POST',
                    url: urls.Commit,
                    success: function (response) {
                        if (response.success === false) {
                            toastr.error(response.message);
                            return;
                        } else if (response.message != null && response.message !== '') {
                            toastr.warning(response.message);
                        }
                        $('#user-edit-dialog').modal('hide');
                        App.UserAdmin.ReloadUsers(1);
                    }
                });
            });
        });
    };
} (window.App = window.App || {}, jQuery));
