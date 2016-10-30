    // about SkyWayAPI
    SpeechRec.config({
               "NrFlag":true,
               'SkyWayKey':'998ae0cd-a6db-48e6-b94c-79fef890eb1d',
               'OpusWorkerUrl':'static/libopus.worker.js'
    });
     
    $("#start_rec").click(function(){
        console.log(SpeechRec.availability());
        SpeechRec.start();
        $("#info").text("Start speech recognition");
        $("#result").text(" ");
    });
    

    SpeechRec.on_config(function(conf){
        $("info").text(conf)
    });

    SpeechRec.on_proc(function(info){
        console.log(info);
    });
       
    SpeechRec.on_result(function(result){
        $("#info").text("The result of speech recognition is below this")
        $("#result").text(result.candidates[0].speech);
    });

    SpeechRec.on_no_result(function(result){
        $("info").text("No result");
        console.log("No result");
    });

    SpeechRec.on_error(function(e){
        console.log(e);
    });

