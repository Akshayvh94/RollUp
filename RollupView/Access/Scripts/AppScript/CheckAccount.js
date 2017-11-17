var messageList = [];
var ErrorData = '';
var statusCount = 0;
var messagesCount = 11;
$(document).ready(function () {

    $("#txtAccName").val("");
    $("#txtPat").empty();
    $("#submitpat").click(function () {
        $('#err').empty();
        $('.errs').empty();

        var accname = $("#txtAccName").val();
        var pat = $("#txtPat").val();
        var bool = true;
        if (accname == "") {
            $("#txtAccName").css("border-color", "red"); bool = false;
        }
        else {
            $("#txtAccName").css("border-color", "");
        }
        if (pat == "") {
            $("#txtPat").css("border-color", "red"); bool = false;
        }
        else {
            $("#txtPat").css("border-color", "");
        }
        if (bool == true) {
            // $("#submitpat").prop("disabled", true);
            $("#Verify").submit();
        }
    });

    $('#btncreateHooks').click(function () {
        var project = $('#ProjectList option:selected').val();
        var name = $('#ProjectList option:selected').text();
        $('#diverror').empty();
        var choose = true;
        if (project == "") {
            $('#ProjectList').css("border-color", 'red'); choose = false;
        }
        else {
            $('#ProjectList').css("border-color", "");
        }
        if (choose == true) {
            $('#projectname').val(name);
            $("#Import").submit();
            $("#btncreateHooks").attr("disabled", "disabled");
            $('#ProjectList').attr("disabled", "disabled");
            $('#loading').show();
            
        }
        $('#diverror').empty();


    });

    $(".rmv").focus(function () {
        var da = $(this).prop("id");
        $("#" + da).css("border-color", "");
    });

});

function ProjectList(data) {
    $("#ProjectList").empty();
    var newval = "<option value='0'>Select Project</option>";
    $("#ProjectList").append(newval);
    for (var i = 0; i < data.value.length; i++) {
        var opt = new Option(data.value[i].name, data.value[i].id);
        $("#ProjectList").append(opt);
    }
}
