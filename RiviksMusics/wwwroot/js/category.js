$(document).ready(function () {
    $("#category").DataTable({
        columnDefs: [{
            "defaultContent": "-",
            "targets": "_all"
        }]
    })

});