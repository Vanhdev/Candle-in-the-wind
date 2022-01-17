$((function () {
    var url;
    var target;

    $('body').append(`
        <!--Start Confirm delete  -->
                <div class="modal fade" id="delete-modal" tabindex="-1" role="dialog">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title delete-modal-title">Xác nhận xóa</h4>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body delete-modal-body">
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger waves-effect waves-light " id="confirm-delete">Xóa</button>
                                <button type="button" class="btn btn-default waves-effect " data-dismiss="modal">Hủy </button>
                            </div>
                        </div>
                    </div>
                </div>
`);
    //Delete Action
    var id;
    var controller;
    $(".delete").on('click', (e) => {
        e.preventDefault();
        target = e.target.parentNode; /// e.target point to icon.
        id = $(target).data('id');
        controller = $(target).data('controller');
        var action = $(target).data('action');
        var message = $(target).data('message');
        url = "/" + controller + "/" + action + "?id=" + id;
        $(".delete-modal-body").text(message ? message : "Bạn có chắc muốn xóa?");
        $("#delete-modal").modal('show');
    });

    $("#confirm-delete").on('click', () => {
        $.get(url)
            .always(() => {
                $("#delete-modal").modal('hide');
                window.location.href = "/" + controller;
            });
    });

}()));