function btnClose(e) {
    var parent = $('.toast__close').parent('.toast');
    parent.fadeOut("slow", function() { $(this).remove(); } );
}

let counterToast=0;
function notification(baslik,aciklama,bildirimTipi,timeout){
if(timeout==null){
	timeout=5000;
}
let colorClass;
let icon;
if(bildirimTipi=="success"){
colorClass="toast--green";
    icon ='<i class="fa fa-check-square custom_icon"></i>';
}else if(bildirimTipi=="error"){
colorClass="toast--yellow";
    icon = '<i class="fa fa-exclamation-triangle custom_icon"></i>';
}else{
colorClass="toast--blue";
    icon = '<i class="fa fa-exclamation-circle custom_icon"></i>';
}
let addedToast="toast"+counterToast;
    let notificate = '<div id=' + addedToast + ' class="toast ' + colorClass + ' add-margin"><div class="toast__icon">' + icon + '</div><div class="toast__content"><p class="toast__type">' + baslik + '</p><p class="toast__message">' + aciklama +'</p></div><div class="toast__close"><button type="button" class="btnClose" onclick="btnClose(this)"><span class="fa fa-times"></span></button></div></div>';

$(".toast__cell").append(notificate);
$(".toast").slideDown();
let selectedToast='#'+addedToast;
setTimeout(function(){$(selectedToast).fadeOut("slow", function() { $(this).remove(); } );},timeout);
counterToast++;
}
$(".toast__cell").delegate('.toast__close', 'click', function () {
    var parent = $('.toast__close').parent('.toast');
    parent.fadeOut("slow", function() { $(this).remove(); } );
});