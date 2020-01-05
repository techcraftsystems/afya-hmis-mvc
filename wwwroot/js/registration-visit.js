jq(function() {
    String.prototype.toAccounting = function() {
        var str = parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
        if (str.charAt(0) == '-') {
            return '(' + str.substring(1, 40) + ')';
        }
        else {
            return str;
        }
    };

    jq('#Queue_Room_Type_Id').change(function(){
        GetRoomsIEnumerable();
    });

    jq('#Queue_Room_Id, #Visit_ClientCode_Id').change(function(){
        var clnt_code = jq('#Visit_ClientCode_Id').val();
        var room_idnt = jq('#Queue_Room_Id').val();

        GetClientCodeBillableRate(room_idnt, clnt_code);
    });

    jq(".select2").select2({
        dropdownAutoWidth: true,
        width: '100%'
    });

    jq(".max-length").select2({
        dropdownAutoWidth: true,
        width: '100%',
        maximumSelectionLength: 10,
        placeholder: "Select Doctors"
    });

    jq('#Doctor').change(function(){
        jq('#DoctorString').val(jq('#Doctor').val().toString());
    });

    jq('#Waiver').change(function(){
        if (jq('#Waiver:checked').length == 1){
            jq('fieldset.waivers').removeClass('hidden');
        }
        else {
            jq('fieldset.waivers').addClass('hidden');
        }
    });
    
    jq('#Visit_ClientCode_Id').change();
    jq('#Waiver').change();
});

function validateForm(){
    jq('#DoctorString').val(jq('#Doctor').val().toString());

    if (jq('#Referral_Type_Id').val() != "") {
        if (jq('#Referral_Facility').val() == "" && jq('#Doctor').val().length == 0){
            toastr.error('Specify the referring doctor and/or Facility.', 'Verification Failed');
            return false;
        }

        if (jq('#Referral_Notes').val() == ""){
            toastr.error('Specify the referral details.', 'Verification Failed');
            return false;
        }
    }

    var count = jq('#Waiver:checked').length;
    if (count >= 1 && jq('#WaiverReason').val().trim() == ""){
        toastr.error('You must specify reason for Waiving.', 'Verification Failed');
        return false;
    }

    toastr.success('Creating visit details..', 'Visit', { "timeOut": 0, "closeButton": true });
    return true;
}

function GetRoomsIEnumerable() {
    var types = jq('#Queue_Room_Id');
    types.find('option').remove();
    types.append('<option value="">Loading..</option>');

    jq.ajax({
        dataType: "json",
        url: '/Registration/GetRoomsIEnumerable',
        data: {
            "type": jq("#Queue_Room_Type_Id").val()
        },
        success: function(results) {
            types.find('option').remove();
            jq.each(results, function(i, opt) {
                types.append('<option value="' + opt.value + '">' + opt.text + '</option>');
            });
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            jq('#Visit_ClientCode_Id').change();
        }
    }); 
}

function GetClientCodeBillableRate(room, code) {
    var amounts = 0;
    var service = 0;

    jq.ajax({
        dataType: "json",
        url: '/Registration/GetClientCodeBillableRate',
        data: {
            "room": room,
            "code": code
        },
        success: function(results) {
            amounts = results.amount;
            service = results.service.id;
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function(){
            jq('#Bill_Amount').val(amounts.toString().toAccounting());
            jq('#Item_Price').val(amounts);
            jq('#Item_Service_Id').val(service);
        }
    }); 
}