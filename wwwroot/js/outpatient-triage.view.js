var painScale;

jq(function() {    
  jq('li.nav-item').click(function(){
      jq('span.line-vs-tabs').css('top', (jq(this).index()*40)+'px');
  });

  jq('button.btn-next, button.btn-prev').click(function(){
    var index = jq(this).data('tabs');
    jq('ul.nav-pills').find('li:eq(' + index + ') a').click();
  });

  jq('#SendTo_Room_Type_Id').change(function(){
    GetRoomsIEnumerable();
  });
    
  painScale = document.getElementById('pips-range');
  noUiSlider.create(painScale, {
      range: {
        'min': [0],
        '20%': [2],
        '40%': [4],
        '60%': [6],
        '80%': [8],
        'max': [10]
      },
      start: 0,
      direction: 'ltr',
      pips: {
        mode: 'range',
        density: 3
      }
  });

  painScale.noUiSlider.on('update', function(values, handle) {
    var group = jq('div.bullets-group-1');
    var value = eval(values[0]).toFixed();

    jq('#Triage_PainScale_Value').val(value);

    group.find('div').removeClass('bold-text');
    if (value == 0){
      group.find('div:eq(0)').addClass('bold-text');
    }
    else if (value <= 2){
      group.find('div:eq(1)').addClass('bold-text');
    }
    else if (value <= 4){
      group.find('div:eq(2)').addClass('bold-text');
    }
    else if (value <= 6){
      group.find('div:eq(3)').addClass('bold-text');
    }
    else if (value <= 8){
      group.find('div:eq(4)').addClass('bold-text');
    }
    else {
      group.find('div:eq(5)').addClass('bold-text');
    }
  });

  jq.contextMenu({
      selector: '#btn-context-menu',
      trigger: 'hover',
      autoHide: true,
      callback: function (key, options) {
        var r = "clicked " + key;
        console.log(r);
      },
      items: {
        "Opts1": { name: "Add Pharmaceuticals" },
        "Opts2": { name: "Add Non-Pharma" },
        "Opts3": { name: "Add Procedures" },
      }
  });

  var tour = new Shepherd.Tour({
    classes: 'shadow-md bg-purple-dark',
    scrollTo: true
  })
  
  tour.addStep('step-1', {
    text: 'Begin Patient Triage',
    attachTo: 'div.patient-idnt bottom',
    buttons: [
      {
        text: "Cancel",
        action: tour.complete
      }, {
        text: "Start",
        action: function(){
          BeginTriage();
          tour.complete();
        }
      }
    ]
  });

  if (sid == 0){
    setTimeout(function(){
      tour.start();
    }, 1500);
  }
});

function GetRoomsIEnumerable() {
  var types = jq('#SendTo_Room_Id');
  types.find('option').remove();
  types.append('<option value="">Loading..</option>');

  jq.ajax({
      dataType: "json",
      url: '/Registration/GetRoomsIEnumerable',
      data: {
          "type": jq("#SendTo_Room_Type_Id").val()
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
      }
  }); 
}

function BeginTriage(){
  jq.ajax({
      dataType: "json",
      url: '/Outpatient/StartTriage',
      data: {
          "qid": qid
      },
      success: function() {
        toastr.info('Patient Triage has been marked as Started.', 'Triage');
      },
      error: function(xhr, ajaxOptions, thrownError) {  
          console.log(xhr.status + ' : ' + thrownError);
      }
  });
}

function validateTriageForm(){
  if (jq('#SendTo_Room_Id').val() == ""){
    toastr.error('Kindly specify room to send patient', 'Validation');
    return false;
  }

  toastr.success('Saving Triage details..', 'Triage', { "timeOut": 0, "closeButton": true });
  return true;
}