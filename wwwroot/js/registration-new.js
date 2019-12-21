jq(function() {
    var patientTable;

    jq('.pickadate').pickadate({
        format: 'dd/mm/yyyy'
    });

    jq('#Patient_Person_Address_Telephone').formatter({
        'pattern': '+{{999}} {{999}} {{999999}}',
        'persistent': true
    });

    $("#patient-form").validate({
        rules: {
            DateOfBirth: {
                required: true,
                minlength: 1
            },
            'Patient.Person.Name': {
                required: true,
                minlength: 6
            },
            'Patient.PI.Identifier': {                
                required: true
            },
            'Patient.Person.Address.Email': {
                email: true
            },
            'Patient.Person.Address.Telephone': {
                required: true,
                minlength: 10
            }
        },
        errorElement: 'div'
    });

    jq('#DateOfBirth').on('blur', function(){
        jq.ajax({
            dataType: "text",
            url: '/Registration/GetDateFromString',
            data: {
                "value": jq(this).val()
            },
            success: function(results) {
                jq('#DateOfBirth').val(results);
            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            }
        });
    });

    jq("input.pickadate").attr("readonly", false); 
});

function validateForm(){
    var count = jq('#Patient_Person_Name').val().split(' ').length;
    if (count < 2){
        jq('#Patient_Person_Name').addClass('error');
        toastr.error('Specify atleast two Patient Names.', 'Validation Failed');
        return false;
    }

    if(/^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[13-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/.test(jq('input.pickadate').val()) == false){
        toastr.error('Invalid Date', 'Validation Failed');
        return false;
    }

    return true;
}