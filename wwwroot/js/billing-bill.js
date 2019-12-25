var refs = 0;
var flag = false;
var viod = false;

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
    jq('#pending-bill-table tbody input').change(function(){
        GetTRowsSummary();    
    });

    jq('#bill-cancel-modal-table tbody').on('change', 'input', function(){
        if (jq('#bill-cancel-modal-table tbody input:checked').length == 0){
            jq('button.btn-void-items').removeClass('btn-danger');
            jq('button.btn-void-items').addClass('btn-outline-danger');
        }
        else {
            jq('button.btn-void-items').addClass('btn-danger');
            jq('button.btn-void-items').removeClass('btn-outline-danger');
        }
    });

    jq('input.th-check-all').change(function(){
        jq('#pending-bill-table tbody input').prop("checked", jq(this).is(':checked'));
        GetTRowsSummary();
    });

    jq('a.bill-opts').click(function(){
        var modal = jq('#bill-details-modal');
        var code = jq(this).data('code');

        refs = jq(this).data('idnt');
        flag = false;
        
        modal.find('h5.modal-title').html("Bill #00" + refs);
        modal.find('span.modal-client-code').html(code);
        
        GetBillsItems();     
    });

    jq('a.bill-view').click(function(){
        var modal = jq('#bill-details-modal');
        var code = jq(this).data('code');

        refs = jq(this).data('idnt');
        flag = false;
        viod = false;
        
        modal.find('h5.modal-title').html("Bill #00" + refs);
        modal.find('span.modal-client-code').html(code);
        
        GetBillsItems();     
    });

    jq('a.bill-canc').click(function(){
        var modal = jq('#bill-cancel-modal');
        var code = jq(this).data('code');

        refs = jq(this).data('idnt');
        
        modal.find('h5.modal-title').html("Cancel Bill #00" + refs);
        modal.find('span.modal-client-code').html(code);

        jq('button.btn-void-items').removeClass('btn-danger');
        jq('button.btn-void-items').addClass('btn-outline-danger');

        jq('textarea.bill-cancel-notes').val('');
        
        GetCancellBillItems(); 
    });

    jq('button.btn-process').click(function(){
        if (jq('input.tf-check-summary:checked').length == 0){
            toastr.error('No bills selected for Invoicing', 'Validation Failed');
            return false;
        }

        window.location.href = "/billing/bill/process?p=" + uuid + "&date=" + date + "&bills=" + bill;
    });

    jq('button.btn-void-items').click(function(){
        if (jq('#bill-cancel-modal-table tbody input:checked').length == 0){
            toastr.error('Voiding Failed. No items selected for Cancellation');
            return false;
        }

        if (jq('textarea.bill-cancel-notes').val().trim() == ""){
            toastr.error('Specify reason for bill items cancellation');
            return false;
        }

        CancellBillItems();
    });

    jq('body').addClass('ecommerce-application');    
});

function CancellBillItems(){
    var trows = jq('#bill-cancel-modal-table tbody tr');
    var items = "";

    trows.each(function(i, row) {
        if (jq(this).find('input:checked').length == 1){
            items += (items == "" ? "" : ",") + jq(this).data('idnt');
        }
    });
    
    jq.ajax({
        dataType: "json",
        url: '/Billing/CancelBillItems',
        data: {
            "idnt":  refs,
            "items": items,
            "notes": jq('textarea.bill-cancel-notes').val()
        },
        success: function(result) {
            if (result == true){
                toastr.success("Successfully voided items in Bill #00" + refs, "Void Successful");
            }

            setTimeout(function() {
                window.location.href = "/billing/bill?p=" + uuid + "&date=" + date;                
            }, 1000); 
        },
        error: function(xhr, ajaxOptions, thrownError) {
            toastr.error("Failed to void the Bill. " + thrownError);
            console.log(xhr.status + ', ' + thrownError);
        }
    }); 

}

function GetCancellBillItems(){
    var tbl = jq('#bill-cancel-modal-table');    
    tbl.find('tbody').empty();

    jq.ajax({
        dataType: "json",
        url: '/Billing/GetBillsItems',
        data: {
            "idnt":     refs,
            "process":  true
        },
        success: function(results) {
            jq.each(results, function(i, item) {
                var row = "<tr data-idnt='" + item.id + "'>";
                row += '<td><span class="vs-checkbox-con vs-checkbox-danger"><input type="checkbox"><span class="vs-checkbox"><span class="vs-checkbox--check"><i class="vs-icon feather icon-check"></i></span></span></span></td>';
                row += "<td>" + item.service.name + "</td>";
                row += "<td class='text-right'>" + item.price.toString().toAccounting() + "</td>";
                row += "<td class='text-right'>" + item.quantity.toString().toAccounting() + "</td>";
                row += "<td class='text-right'>" + eval(item.quantity * item.price).toString().toAccounting() + "</td>";
                row += "</tr>";

                tbl.find('tbody').append(row);
            })
            
            if (results.length == 0) {
                tbl.find('tbody').append("<tr><td class='text-center' colspan='5'>NO ITEMS FOUND</td></tr>");
            }
        },
        error: function(xhr, ajaxOptions, thrownError) {
            tbl.find('tbody').append("<tr><td class='text-center' colspan='5'>ERROR LOADING ITEMS</td></tr>");

            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            jq('#bill-cancel-modal').modal();
        }
    }); 
}

function GetBillsItems(){
    var tbl = jq('#bill-details-modal-table');
    var sum = 0.00;
    
    tbl.find('tbody').empty();

    jq.ajax({
        dataType: "json",
        url: '/Billing/GetBillsItems',
        data: {
            "idnt":    refs,
            "viod":    viod,
            "process": flag
        },
        success: function(results) {            
            
            jq.each(results, function(i, item) {
                sum += eval(item.quantity * item.price);

                var row = "<tr><td>" + eval(i+1) + "</td>";
                row += "<td>" + item.service.name + "</td>";
                row += "<td class='text-right'>" + item.price.toString().toAccounting() + "</td>";
                row += "<td class='text-right'>" + item.quantity.toString().toAccounting() + "</td>";
                row += "<td class='text-right'>" + eval(item.quantity * item.price).toString().toAccounting() + "</td>";
                row += "</tr>";

                tbl.find('tbody').append(row);
            })
            
            if (results.length == 0) {
                tbl.find('tbody').append("<tr><td class='text-center' colspan='5'>NO ITEMS FOUND</td></tr>");
            }
        },
        error: function(xhr, ajaxOptions, thrownError) {
            tbl.find('tbody').append("<tr><td class='text-center' colspan='5'>ERROR LOADING ITEMS</td></tr>");

            console.log(xhr.status);
            console.log(thrownError);
        },
        complete: function() {
            tbl.find('tfoot th.tf-summary').html(sum.toString().toAccounting());
            jq('#bill-details-modal').modal();
        }
    }); 
}

function GetTRowsSummary(){
    var trows = jq('#pending-bill-table tbody tr');
    var tfoot = jq('#pending-bill-table tfoot tr');
    var summs = 0;

    bill = "";

    trows.each(function() {
        if (jq(this).find('td:eq(0) input:checked').length == 1){
            summs += eval(jq(this).find('td:eq(6)').html().trim().replace(',', ''));
            bill += (bill == "" ? "" : ",")+jq(this).find('td:eq(3)').html().trim();
        }
    });

    if (summs == 0){
        tfoot.find('th:eq(0) input').prop("checked", false);
        tfoot.find('button').removeClass('btn-primary');
        tfoot.find('button').addClass('btn-outline-primary');
    }
    else {
        tfoot.find('th:eq(0) input').prop("checked", true);
        tfoot.find('button').addClass('btn-primary');
        tfoot.find('button').removeClass('btn-outline-primary');
    }

    tfoot.find('th.summary-field').html(summs.toString().toAccounting());
}