var datatypeEnum, typeEnum;
var dateFormat = "dd/MM/yyyy";

$(function () {
    if (typeof datatypeEnum == "undefined") {
        datatypeEnum = {
            "json": "json",
            "text": "text"
        };
    }

    if (typeof typeEnum == "undefined") {
        typeEnum = {
            "get": "get",
            "post": "post"
        };
    }
});

function callwebservice(ajaxurl, parameter, callbackFunction, isErrorHandle, dataTypem, typeEnum) {

    if (typeof (parameter) === 'undefined') {
        parameter = '';
    }

    try {
        $.support.cors = true;
        $.ajax({
            url: ajaxurl,
            cache: false,
            dataType: dataTypem,
            data: parameter,
            timeout: 300000,
            type: typeEnum,
            success: function (data) {
                callbackFunction(data);
            },
            error: function (jqXhr, textStatus, errorThrown) {
                if (isErrorHandle === true) {
                    callbackFunction("error");
                }
                else {
                    if (errorThrown !== "") {
                        showErrorNotification("The following error occured: " + errorThrown);
                        waitingDialog.hide();
                    }
                    else {
                        showErrorNotification("There is an error while connecting to server. Please try again!");
                        waitingDialog.hide();
                    }
                }
            }
        });
    }
    catch (e) {
        showErrorNotification("Errour occurred " + e);
        waitingDialog.hide();
    }
}


function clearInputById(id) {
    $("#" + id).val("");
}

function setFocusById(id) {
    $("#" + id).focus();
}

function setInputValueById(id, value) {
    return $("#" + id).val(value);
}

function getInputValueById(id) {
    return $("#" + id).val();
}

function getInputTextById(id) {
    return $("#" + id).text();
}

function showErrorNotification(message) {
    var options = {};
    options.title = 'Warning!';
    options.description = message;
    options.message = 'alert';
    options.position = 'bottom-right',
        options.closeTimeout = 15000;
    options.showProgress= true;
    GrowlNotification.notify(options);
}

function showSuccessNotification(message) {
    var options = {};
    options.title = 'Success!';
    options.description = message;
    options.message = 'alert';
    options.position = 'bottom-right',
    options.closeTimeout = 15000;
    options.type = 'success',
    options.showProgress = true;
    GrowlNotification.notify(options);
}

function showSuccessNotificationStore(message) {
    var options = {};
    options.title = 'Success!';
    options.description = message;
    options.message = 'alert';
    options.position = 'bottom-right',
        options.closeTimeout = 15000;
    options.type = 'success',
        options.showProgress = true;
    GrowlNotification.notify(options);
}

function showErrorNotificationStore(message) {
    var options = {};
    options.title = 'Warning!';
    options.description = message;
    options.message = 'alert';
    options.position = 'bottom-right',
        options.closeTimeout = 15000;
    options.showProgress = true;
    GrowlNotification.notify(options);
}

function showCartStatus() {
    var element = $('.shopping-cart-status');
    var serviceUrl = element.attr('data-href');
    callwebservice(serviceUrl, '', getStatusCompleted, false, datatypeEnum.json, typeEnum.get);
}

function getStatusCompleted(data) {
    var element = $('.shopping-cart-status');

    var acartid = element.find('#acartid');

    var count = element.find('.count');

    var countValue = count.find('.value');

    if (data != null && data.Count && data.Count > 0) {
        countValue.text(data.Count);

        acartid.removeClass('invisible');

        acartid.addClass('visible');
    } else {
        acartid.removeClass('visible');

        acartid.addClass('invisible');
    }

    waitingDialog.hide();
}

function sidebarActiveLink() {
    $("li.nav-item").removeClass("active");
    $("li#" + window.controllerName).addClass("active");
}

function getLocalValue(key) {
    return localStorage.getItem(key);
}

function setLocalValue(key, value) {
    localStorage.setItem(key, value);
}