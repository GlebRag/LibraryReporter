$(document).ready(function () {
    $(".creationbook .update-avatar-input").click(function () {
        const profileBlock = $(this).closest(".creationbook");
        profileBlock.find(".update-avatar-button").css("display", "inline");

    });
})