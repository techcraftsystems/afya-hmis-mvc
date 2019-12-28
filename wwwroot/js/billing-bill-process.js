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
    jq('#invoice-bill-table tbody input.check-line').change(function(){
        GetTRowsSummary();    
    });

    jq('input.th-check-all').change(function(){
        jq('#invoice-bill-table tbody input.check-line').prop("checked", jq(this).is(':checked'));
        GetTRowsSummary();
    });

    jq('body').addClass('ecommerce-application');
    
    jq('button.btn-save').click(function(){
        if (jq('input.tf-check-summary:checked').length == 0){
            toastr.error('No bill items selected for Invoicing', 'Validation Failed');
            return false;
        }

        jq('form.invoice-form').submit();        
    });

    GetTRowsSummary();
});

function GetTRowsSummary(){
    var trows = jq('#invoice-bill-table tbody tr');
    var tfoot = jq('#invoice-bill-table tfoot tr');
    var count = 0;
    var summs = 0;

    trows.each(function() {
        if (jq(this).find('td:eq(0) input.check-line:checked').length == 1){
            count += eval(jq(this).find('td:eq(5)').html().trim().replace(',', ''));
            summs += eval(jq(this).find('td:eq(6)').html().trim().replace(',', ''));
        }
    });

    if (count == 0){
        tfoot.find('th:eq(0) input').prop("checked", false);
        jq('button.btn-save').removeClass('btn-primary').addClass('btn-outline-primary');
    }
    else {
        tfoot.find('th:eq(0) input').prop("checked", true);
        jq('button.btn-save').removeClass('btn-outline-primary').addClass('btn-primary');
    }

    tfoot.find('th.count-field').html(count.toString().toAccounting());
    tfoot.find('th.summs-field').html(summs.toString().toAccounting());
}