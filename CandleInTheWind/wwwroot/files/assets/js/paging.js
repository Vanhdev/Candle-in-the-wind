$((function () {
    function getList(controller, page) {
        var filter = $("#search-text").val();
        var url = "/" + controller + "/Index?";
        if (page) {
            url += "page=" + page;
        }
        if (filter) {
            url += "&search=" + filter;
        }
        window.location.href = url;
        //$.get("/Products", { page: page, search: filter });
    }
    $('li a.page-link').on('click', (e) => {
        var page = $(e.target).data('page');
        getList($(e.target).parents("ul.pagination").data('controller'), page);
        //$.get("/Products/Index", { page: page, search: filter });
    });
    $('#search-button').on('click', (e) => {
        getList($(e.target).data('controller'), 1);
    });
}));