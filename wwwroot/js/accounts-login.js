jq(function() {
    if (message != ""){
        setTimeout(
            function() {
                toastr.error(message, 'Login Error');
            },
        500);
    }

    if (change == 1) {
        setTimeout(
          function() {
            jq('#changepw').modal('open')
          },
        500);
    }
});