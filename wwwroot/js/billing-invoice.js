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
    jq('button.btn-print').click(function(){
        window.print();
    });

    jq('li.edit-action').click(function(){
        window.location.href = "/billing/invoice/edit?qp=" + invnumb;
    });

    jq('li.email-action').click(function(){
        toastr.error('Action to email the patient is currently not enabled.', "Email Failed");
    });

    jq('li.payment-action').click(function(){
        jq("#invoice-payment-modal-table tbody tr.pymts").each(function(i, row) {
            if (i == 0){
                jq(this).removeClass('hidden');
            }
            else{
                if (!jq(this).hasClass('hidden')){
                    jq(this).addClass('hidden');
                }
            }

            jq(this).find('td:eq(0)').text(eval(i+1) + '.');
            jq(this).find('td:eq(1) select').val('1');
            jq(this).find('td:eq(2) input').val('');
            jq(this).find('td:eq(3) input').val('');
            jq(this).find('td:eq(4) input').val('0.00');
            jq(this).find('td:eq(5) input').val('');
        });

        jq('#invoice-payment-modal').modal();
    });

    jq('button.btn-add-row').click(function(){
        jq("#invoice-payment-modal-table tbody tr.pymts").each(function() {
            if (jq(this).hasClass('hidden')){
                jq(this).removeClass('hidden');
                return false;
            }
        });
    });

    jq('i.btn-remove-row').click(function(){
        var trows = jq("#invoice-payment-modal-table tbody tr.pymts");

        if (trows.not(".hidden").length == 1){
            toastr.error('You cannot remove this row. You need atleast one row present', "Remove Failed");
            return false;
        }

        jq(this).closest('tr').remove();

        jq("#invoice-payment-modal-table tbody tr.pymts").each(function(i, row) {
            jq(this).find('td:eq(0)').html(eval(i+1) + '.');
            console.log(row);
        });
    });

    jq('button.btn-make-payment').click(function(){
        var counts = 0;
        var totals = 0;

        jq("#invoice-payment-modal-table tbody tr.pymts").not(".hidden").each(function(i, row) {
            var mode = jq(this).find('td:eq(1) select').val();
            var refs = jq(this).find('td:eq(2) input').val().trim();
            var amts = jq(this).find('td:eq(4) input').val().replace(',','').trim();

            totals += amts;

            if (mode != 1 && refs == ""){
                toastr.error('Reference number in row ' + eval(i+1) + ' cannot be blank', "Verification Failed");
                counts++;
            }

            if (eval(amts) <= 0){
                toastr.error('Invalid amount in row ' + eval(i+1) + '. Amount must be more than Zero', "Verification Failed");
                counts++;
            }
        });

        jq('#Payment_Amount').val(totals);

        if (totals > invamts){
            toastr.error('Amount paid of KES ' + totals.toString().toAccounting() + ' cannot be more than invoice balance of KES ' + invamts.toString().toAccounting(), "Verification Failed");
            counts++;
        }

        if (counts != 0){
            return false;
        }

        toastr.success('Posting payments of KES ' + totals.toString().toAccounting() + ' for the Invoice 00' + invnumb, "Payment");
        jq('form.payment-form').submit();
    });

    if (message != ""){
        setTimeout(
            function() {
                toastr.error(message, 'Payment Error');
            },
        500);
    }
});
