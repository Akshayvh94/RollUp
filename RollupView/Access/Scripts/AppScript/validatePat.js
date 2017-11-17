$(document).ready(function () {

    $('#close').click(function () {
        $("#msgSource").hide();
    });
    $('#close1').click(function () {
        $("#msgSource1").hide();
    });
    var emsg = $('#modelRetMsg').val();
    if (emsg != "") {
        $('#msgSource1').show();
        $('#moderlRetrnMsg').append(emsg);
    }

    $("#submitForm").click(function () {

        var selectedAccount = $("#ddlAcccountName").val();
        var newpat = $('#newpat').val();
        if (selectedAccount == "") {
            $("#msg").text("Please Enter Account Name");
            $("#msgSource").show();
            return;
        }
        if (newpat == "") {
            $("#msg").text("Please Enter PAT for the account");
            $("#msgSource").show();

        }
        else {
            $('#hdnnewpats').val(newpat);
            $("#submitForm").prop('disabled', true);
            $('#newpat').prop('disabled', true);
            $("#Dashboardform").submit();
            $("#ddlAcccountName").prop('disabled', true);
            $("#loader").show();
        }
    });

});