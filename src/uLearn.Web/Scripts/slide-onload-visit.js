function smbVisit() {
    $.ajax({
        type: "POST",
        url: $("#VisitSlideUrl").data("url"),
        data: ""
    });
}