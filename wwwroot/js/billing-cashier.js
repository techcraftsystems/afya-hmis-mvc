String.prototype.toAccounting = function() {
    var str = parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    if (str.charAt(0) == '-') {
        return '(' + str.substring(1, 40) + ')';
    }
    else {
        return str;
    }
};

jq(function() {
    jq('.pickadate').pickadate({
        format: 'dd/mm/yyyy'
    });

    tbl = $("#billing-queue-table").DataTable({
        responsive: true,
        'columnDefs': [{
            "orderable": false,
            "targets": [1,4]
        }, {
            className: "dt-nowrap", 
            "targets": [3]
        }, {
            className: "text-center", 
            "targets": [6,8]
        }, {
            className: "text-right", 
            "targets": [7]
        }, {
            "visible": false,
            "targets": [0]
        }],
        "displayLength": 25,
        "language": {
            "emptyTable": "No bills found in the queue",
            "zeroRecords": "No bills matched the criteria"
        }
    });

    jq('#queue-filter').keyup(function(){
        jq('div.dataTables_filter input').val(jq(this).val());
        jq('div.dataTables_filter input').keyup();
    });

    jq('#billing-queue-table').on('click', 'a.redirect-link', function(){
        toastr.info('This Action is currently disabled.', 'Action disabled');
    });

    jq('i.get-queue').click(function(){
        GetBillingCashierQueue();
    });

    jq('input.pickadate, select.pickadata').change(function(){
        GetBillingCashierQueue();
    });

    jq('form input, form select').on('keypress',function(e) {
        if(e.which == 13) {
            GetBillingCashierQueue();
        }
    });

    setInterval(function() {
        GetBillingCashierQueue();
    }, 60000); 
});

function GetBillingCashierQueue() {
    jq.ajax({
        dataType: "json",
        url: '/Billing/GetBillingCashierQueue',
        data: {
            "start":    jq("#queue-start").val(),
            "stop":     jq("#queue-stop").val(),
            "flag":     jq("#queue-flag").val(),
        },
        success: function(results) {
            tbl.clear().draw();
            
            jq.each(results, function(i, bl) {
                var iStatus = "";
                var iIcons = '&nbsp; <a href="/billing/bill?p=' + bl.visit.patient.uuid + '&date=' + bl.date + '"><i class="feather icon-airplay td-action"></i></a> <a class="pointer redirect-link" data-uuid="' + bl.visit.patient.uuid + '" data-date="' + bl.date + '"> <i class="feather icon-x danger td-action"></i></a>';

                if (bl.flag.id == 0){
                    iStatus = '<span class="badge"><span class="">' + bl.flag.name + '</span> </span>';
                }
                else if (bl.flag.id == 1){
                    iStatus = '<span class="badge badge-success"><span class="">' + bl.flag.name + '</span> </span>';
                }
                else if (bl.flag.id == 99){
                    iStatus = '<span class="badge badge-black"><span class="">' + bl.flag.name + '</span> </span>';
                }
                else {
                    iStatus = '<span class="badge badge-info"><span class="blue-text">' + bl.flag.name + '</span> </span>';
                }

                tbl.row.add([
                    bl.id,
                    bl.date,
                    bl.visit.patient.identifier,
                    "<a class='blue-text' href='/patients/" + bl.visit.patient.uuid + "'>" + bl.visit.patient.person.name + "</a>",
                    bl.visit.patient.age,
                    bl.visit.patient.person.gender,
                    iStatus,
                    bl.amount.toString().toAccounting(),
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