
var FollowingController = function () {

    var init = function () {
        $(".js-toggle-following").click(toggleFollowing);
    };

    var toggleFollowing = function (e) {
        var button = $(e.target);

        if (!button.hasClass("btn-success")) {
            $.post("/api/following", { followeeId: button.attr("data-user-id") })
                .done(function () {
                    button.addClass("btn-success")
                          .text("Following");
                })
                .fail(fail);
        }
        else {
            $.ajax({
                url: "/api/following/" + button.attr("data-user-id"),
                method: "DELETE"

            })
                .done(function () {
                    button.removeClass("btn-success")
                          .text("Follow ?");
                })
                .fail(fail);
        }

    };

    var fail = function () {
        alert("Something failed!");
    };

    return {
        init: init
    }

}();

