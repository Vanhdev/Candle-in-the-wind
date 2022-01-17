$((function () {
    $('[data-target="#detail-modal"]').on('click', (e) => {
        e.preventDefault();
        var target = e.target; /// e.target point to icon.
        var id = $(target).data('id');
        var controller = $(target).data('controller');
        var url = "/" + controller + "/Details" + "?id=" + id;
        var placeholder = $('.placeholder-modal');
        $.get(url).done(function (data) {
            placeholder.html(data);
            placeholder.find("#detail-modal").modal('show');

            placeholder.find(".modal-button").on('click', () => {
                placeholder.find("#detail-modal").remove();
                $('.modal-backdrop').remove();
            });
            placeholder.find('#modal-edit').on('click', () => {
                window.location.href = "/" + controller + "/Edit" + "?id=" + id;
            });
        });
    });
}()));