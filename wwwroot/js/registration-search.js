jq(function() {
    var patientTable;

    jq('#patient-age').formatter({
        'pattern': '{{999}}',
        'persistent': true
    });

    tbl = $("#patient-table").DataTable({
        responsive: true,
        'columnDefs': [{
            "orderable": false,
            "targets": [2,5,6,7]
        }, {
            className: "dt-nowrap", 
            "targets": [1,4]
        }, {
            className: "text-center", 
            "targets": [6,7]
        }],
        "displayLength": 25,
        "language": {
            "emptyTable": "No patients found in the database",
            "zeroRecords": "No patients matched the criteria"
        }
    });

    jq('#patient-table').on('click', 'a.redirect-link', function(){
        var status = jq(this).data('status');
        var uuid = jq(this).data('uuid');

        if (status == 2){
            toastr.error('You can not create a visit for an admitted patient.', 'Patient Admitted');
            return false;
        }
        if (status == 99){
            toastr.error('You can not create a visit for a dead patient.', 'Patient Dead');
            return false;
        }

        window.location.href = "/registration/visit?p=" + uuid;
    });

    jq('a.btn-floating.btn-large').click(function(){
        var redirect = "?name=" + jq('#patient-names').val().trim();
        if (jq('#patient-identification').val().trim() != ''){
            redirect += "&id=" + jq('#patient-identification').val().trim();
        }
        if (jq('#patient-contact').val().trim() != ''){
            redirect += "&phone=" + jq('#patient-contact').val().trim();
        }
        if (jq('#patient-age').val().trim() != ''){
            redirect += "&age=" + jq('#patient-age').val().trim();
        }
        if (jq('#patient-gender').val().trim() != ''){
            redirect += "&gender=" + jq('#patient-gender').val().trim();
        }

        window.location.href = "/registration/new" + redirect;
    });

    jq('i.get-patients').click(function(){
        SearchPatients();
    });

    jq('form input, form select').on('keypress',function(e) {
        if(e.which == 13) {
            SearchPatients();
        }
    });

    jq('a.btn-add-new').click(function(){
        var params = "";

        if (jq('#patient-names').val().trim() != ""){
            params = "?name=" + jq('#patient-names').val().trim()
        }   

        if (jq('#patient-identification').val().trim() != ""){
            params += (params == "" ? "?" : "&") + "id=" + jq('#patient-identification').val().trim();
        }

        if (jq('#patient-contact').val().trim() != ""){
            params += (params == "" ? "?" : "&") + "phone=" + jq('#patient-contact').val().trim();
        }

        if (jq('#patient-age').val().trim() != ""){
            params += (params == "" ? "?" : "&") + "age=" + jq('#patient-age').val().trim();
        }

        if (jq('#patient-gender').val().trim() != ""){
            params += (params == "" ? "?" : "&") + "patient_gender=" + jq('#patient-gender').val().trim();
        }

        console.log(params);
        window.location.href = "/registration/new" + params;
    });

    jq('div.users-list-table').hide();
});

function SearchPatients() {
    jq.ajax({
        dataType: "json",
        url: '/Registration/SearchPatients',
        data: {
            "names":        jq("#patient-names").val(),
            "identifier":   jq("#patient-identification").val(),
            "phone":        jq("#patient-contact").val(),
            "age":          jq("#patient-age").val(),
            "gender":       jq("#patient-gender").val(),
            "visit":        jq("#patient-visit").val()
        },
        success: function(results) {
            //var tbl = $('#users-list-datatable').DataTable();
            tbl.clear().draw();
            
            
            //jq('#users-list-datatable tbody').empty();
            jq.each(results, function(i, pt) {
                var lastVisit = (pt.lastVisit == "01/01/1900" ? "Never" : pt.lastVisit);
                var iStatus = "";
                var iIcons = '&nbsp; <a href="/registration/edit?p=' + pt.uuid + '"><i class="feather icon-edit-1 td-action"></i></a><a class="pointer redirect-link" data-uuid="' + pt.uuid + '" data-status="' + pt.status.id + '"> <i class="feather icon-airplay td-action"></i></a>';

                if (pt.status.id == 0){
                    iStatus = '<span class="badge badge-success"><span class="">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 1){
                    iStatus = '<span class="badge"><span class="">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 2){
                    iStatus = '<span class="badge badge-black"><span class="">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 3){
                    iStatus = '<span class="badge badge-warning"><span class="">' + pt.status.name + '</span> </span>';
                }
                else if (pt.status.id == 99){
                    iStatus = '<span class="badge badge-danger"><span class="">' + pt.status.name + '</span> </span>';
                }
                else {
                    iStatus = '<span class="badge badge-info"><span class="blue-text">' + pt.status.name + '</span> </span>';
                }

                tbl.row.add( [
                    pt.identifier,
                    "<a class='blue-text' href='/patients/" + pt.uuid + "'>" + pt.person.name + "</a>",
                    pt.age,
                    (pt.person.gender == "m" ? "Male" : "Female"),
                    pt.person.address.telephone,
                    lastVisit,
                    iStatus,
                    iIcons
                ] ).draw( false );
            })
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            jq('div.users-list-table').show();
            jq('a.buy-now.vs-button').removeClass('hidden');
        }
    }); 
}

function validateRegistrationForm(){
    return true;
}