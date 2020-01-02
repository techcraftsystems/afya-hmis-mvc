jq(function() {
    jq('.pickadate').pickadate({
        format: 'dd/mm/yyyy'
    });

    tbl = $("#queue-table").DataTable({
        responsive: true,
        'columnDefs': [{
            "orderable": false,
            "targets": [1,4]
        }, {
            className: "dt-nowrap", 
            "targets": [3]
        }, {
            className: "text-center",
            "orderable": false,
            "targets": [8]
        }, {
            "visible": false,
            "targets": [0]
        }],
        "displayLength": 25,
        "language": {
            "emptyTable": "No patients found in the queue",
            "zeroRecords": "No patients matched the criteria"
        }
    });

    jq('#queue-priority').change(function(){
        jq('#queue-filter').keyup();
    });

    jq('#queue-filter').keyup(function(){
        jq('div.dataTables_filter input').val(jq(this).val() + " " + jq('#queue-priority').val());
        jq('div.dataTables_filter input').keyup();
    });

    jq('#queue-table').on('click', 'a.redirect-link', function(){
        toastr.info('This Action is currently disabled.', 'Action disabled');
    });

    jq('i.get-queue').click(function(){
        GetTriageQueue();
    });

    jq('input.pickadate, select.pulladata').change(function(){
        GetTriageQueue();
    });

    jq('select.pulladata').change(function(){
        SetCookie();
    });

    setInterval(function() {
        GetTriageQueue();
    }, 30000); 
});

function GetTriageQueue() {
    jq.ajax({
        dataType: "json",
        url: '/Outpatient/GetOutpatientQueue',
        data: {
            "room": jq("#Room_Id").val(),
            "date": jq("#queue-date").val()
        },
        success: function(results) {
            tbl.clear().draw();
            
            jq.each(results, function(i, bl) {
                var iStatus = (bl.seenBy.id == 0 ? "—" : "Dr. " + bl.seenBy.name);
                var iIcons = '&nbsp; <a href="/outpatient/doctor?qid=' + bl.id + '&pt=' + bl.visit.patient.uuid + '"><i class="feather icon-airplay td-action"></i></a> <a class="pointer redirect-link" data-idnt="' + bl.id + '"> <i class="feather icon-x danger td-action"></i></a>';
             
                tbl.row.add([
                    bl.ix,
                    bl.date,
                    bl.visit.patient.identifier,
                    "<a class='blue-text' href='/patients/" + bl.visit.patient.uuid + "'>" + bl.visit.patient.person.name + "</a>",
                    bl.visit.patient.age,
                    bl.visit.patient.person.gender,
                    bl.priority.name,
                    iStatus,
                    iIcons
                ]).draw(false);
            })
        },
        error: function(xhr, ajaxOptions, thrownError) {
            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            jq('a.buy-now.vs-button').removeClass('hidden');
        }
    }); 
}

function SetCookie() {
    jq.ajax({
        dataType: "json",
        url: '/Outpatient/SetCookie',
        data: {
            "value": jq("#Room_Id").val(),
            "queue": "doctor.room"
        }
    }); 
}