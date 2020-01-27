jq(function() {    
  jq('ul.nav-pills-custom li.nav-item').click(function(){
      jq('span.line-vs-tabs').css('top', (jq(this).index()*40)+'px');
  });

  jq('button.btn-next, button.btn-prev').click(function(){
    var index = jq(this).data('tabs');
    jq('ul.nav-pills').find('li:eq(' + index + ') a').click();
  });

  jq('a.nav-triage').click(function(){
    GetTriageDetails(jq(this).data('triage'));
  });

  var tour = new Shepherd.Tour({
    classes: 'shadow-md bg-purple-dark',
    scrollTo: true
  })
  
  tour.addStep('step-1', {
    text: 'Begin Patient Triage',
    attachTo: 'a.tour-pos bottom',
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

function GetTriageDetails(idnt) {
  jq.ajax({
      dataType: "json",
      url: '/Outpatient/GetTriageDetails',
      data: {
          "idnt": idnt
      },
      success: function(results) {
        var tbody = jq('table.triage-details tbody')
        tbody.empty();

        if (results.temparature.value.trim() != ""){
          tbody.append('<tr><td>TEMPARATURE</td><td>' + results.temparature.value.trim() + ' &#8451;</td><td></td></tr>');
        }
        if (results.bpSystolic.value.trim() != "" || results.bpDiastolic.value.trim()){
          tbody.append('<tr><td>BLOOD PRESSURE</td><td>' + results.bpSystolic.value.trim() + ' / ' + results.bpDiastolic.value.trim() + '</td><td></td></tr>');
        }
        if (results.respiratoryRate.value.trim() != ""){
          tbody.append('<tr><td>RESPIRATORY RATE</td><td>' + results.respiratoryRate.value.trim() + '</td><td></td></tr>');
        }
        if (results.pulseRate.value.trim() != ""){
          tbody.append('<tr><td>PULSE RATE</td><td>' + results.pulseRate.value.trim() + '</td><td></td></tr>');
        }
        if (results.oxygenSaturation.value.trim() != ""){
          tbody.append('<tr><td>OXYGEN SATURATION</td><td>' + results.oxygenSaturation.value.trim() + '</td><td></td></tr>');
        }
        if (results.mobility.value.trim() != ""){
          tbody.append('<tr><td>MOBILITY</td><td>' + results.mobility.value.trim() + '</td><td></td></tr>');
        }
        if (results.trauma.value.trim() != ""){
          tbody.append('<tr><td>TRAUMA</td><td>' + results.trauma.value.trim() + '</td><td></td></tr>');
        }
        if (results.avpu.value.trim() != ""){
          tbody.append('<tr><td>AVPU</td><td>' + results.avpu.value.trim() + '</td><td></td></tr>');
        }
        if (results.weight.value.trim() != ""){
          tbody.append('<tr><td>WEIGHT</td><td>' + results.weight.value.trim() + '</td><td></td></tr>');
        }
        if (results.height.value.trim() != ""){
          tbody.append('<tr><td>HEIGHT</td><td>' + results.height.value.trim() + '</td><td></td></tr>');
        }
        if (results.bmi.value.trim() != ""){
          tbody.append('<tr><td>BMI</td><td>' + results.bmi.value.trim() + '</td><td></td></tr>');
        }
        if (results.muac.value.trim() != ""){
          tbody.append('<tr><td>MUAC</td><td>' + results.muac.value.trim() + '</td><td></td></tr>');
        }
        if (results.chest.value.trim() != ""){
          tbody.append('<tr><td>CHEST CIRCUMFERENCE</td><td>' + results.chest.value.trim() + '</td><td></td></tr>');
        }
        if (results.abdominal.value.trim() != ""){
          tbody.append('<tr><td>ABDOMINAL CIRCUMFERENCE</td><td>' + results.abdominal.value.trim() + '</td><td></td></tr>');
        }

        var tbody = jq('table.sbar-details tbody')
        tbody.empty();
        tbody.append('<tr><td>SITUATION</td><td>' + (results.situation.value.trim() == "" ? "N/A" : results.situation.value) + '</td></tr>');
        tbody.append('<tr><td>BACKGROUND</td><td>' + (results.background.value.trim() == "" ? "N/A" : results.background.value) + '</td></tr>');
        tbody.append('<tr><td>ASSESSMENT</td><td>' + (results.assessment.value.trim() == "" ? "N/A" : results.assessment.value) + '</td></tr>');
        tbody.append('<tr><td>RECOMMENDATION</td><td>' + (results.recommendation.value.trim() == "" ? "N/A" : results.recommendation.value) + '</td></tr>');

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
        toastr.info('Consultation has been marked as Started.', 'OPD');
      },
      error: function(xhr, ajaxOptions, thrownError) {  
          console.log(xhr.status + ' : ' + thrownError);
      }
  });
}

function validateTriageForm(){
  toastr.success('Saving Triage details..', 'Triage', { "timeOut": 0, "closeButton": true });
  return true;
}