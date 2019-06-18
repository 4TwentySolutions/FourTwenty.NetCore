var userSord;
var userSidx;
var curPage;

$(function () {



    $(document.body).delegate('#usersPager a', 'click', async function (e) {
        window.invokePager(e,
            {
                url: window.usersUrls.list,
                sidx: userSidx,
                sord: userSord,
                customFilter: JSON.stringify(getAllTableFilters($('#users-table').prev())),
                elementToUpdate: document.getElementById("usersBlock"),
                successCallback: function () { rebindItems(); }
            });
    });
});

function showUserInfo(elem, id) {
    var $button = $(elem);
    $button.buttonV2("loading");
    $.ajax({
        url: window.usersUrls.showInfo,
        data: {
            id: id
        },
        type: 'GET',
        complete: function () {
            $button.buttonV2("reset");
        },
        success: function (result) {
            $('body').prepend(result);
            $("#userModal").modal("show");

            $.validator.unobtrusive.parse($("#add-edit-user-form"));
            $("#add-edit-user-form").ajaxForm(function (answer) {

                if (answer.isSuccess == false) {
                    $("#add-edit-user-form").find(".alert").remove();
                    $("#add-edit-user-form").find(".modal-body").prepend(`<div class="alert alert-danger alert-dismissible fade show" role="alert">
                            <strong>Warning!</strong> <ul><li>${answer.errors.join("</li><li>")}</li></ul>
                            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>`);
                } else {
                    $("#userModal").modal("hide");
                    $('#usersBlock').html(answer);
                    rebindItems();
                }

            });
            $('#userModal')
                .on('shown.bs.modal',
                    function (e) {

                        $("#userModal").find(".selectpicker").selectpicker();
                    });
            $('#userModal')
                .on('hidden.bs.modal',
                    function (e) {
                        $("#userModal").remove();
                    });
        }
    });
}


async function deleteUserInfo(btn, id) {
    try {
        var result = await deleteItem({
            buttonLoadingElement: btn,
            data: {
                userId: id,
                page: curPage,
                customFilter: JSON.stringify(getAllTableFilters($('#users-pager').prev()))
            },
            url: window.usersUrls.delete,
            alertTitle: "Delete user",
            alertMessage: `Are you sure you want to delete user?`
        });
        if (result.isSuccess === false) {
            window.bootbox.alert({
                size: "small",
                title: "User delete error",
                message: "<div class='text-center'><i class='d-block fas fa-7x fa-times-circle'></i><p class='mt-3'>Error while delete user</p></div>"
            });
        } else {
            $('#usersBlock').html(result);
        }
    } catch (e) {
        if (e instanceof ConfirmPopupError || e instanceof ApiError) {
            return;
        } else {
            alert(e);
        }
    }
    //confirmDialog.show("Delete user?",
    //    "Are you sure you want to delete user?",
    //    null,
    //    null,
    //    function () {
    //        $(btn).buttonV2('loading');
    //        $.ajax({
    //            url: window.usersUrls.delete,
    //            data: {
    //                userId: id,
    //                page: curPage,
    //                customFilter: JSON.stringify(getAllTableFilters($('#users-table')))
    //            },
    //            type: 'POST',
    //            dataType: "html",
    //            success: function (result) {
    //                if (result.isSuccess == false) {
    //                    confirmDialog.info("User delete error", "Error while delete user");
    //                } else {
    //                    $('#usersBlock').html(result);
    //                    rebindItems();
    //                }
    //            },
    //            complete: function () {
    //                $(btn).button('reset');
    //            }
    //        });
    //    });
}


function rebindItems() {
    $('.form-check-input').on('change', function () {
        $(this).parent().toggleClass("active");
        $(this).closest(".media").toggleClass("active");
    });
}

function changePasswordBlock() {
    $('.password-block').removeClass('hidden');
    $('#changePswButton').addClass('hidden');
}