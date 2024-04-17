$(document).ready(function () {
    $("#dataTable").DataTable({
        columnDefs: [{
            "defaultContent": "-",
            "targets": "_all"
        }]
    })

});
